using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Events;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// Draw at any rate and tick the world at the game's own 20 Hz:
///
///     ST_PACING=0      off: the game's own pace (VBlankPacing holds it to 20)
///     ST_FPS=144|off   the picture's rate; off is uncapped
///     ST_TICKRATE=20   the world's (a comparison only)
///     ST_FPS_PROBE=1   a line a second (PacingProbe)
///
/// The game's frame gate func_8001535C (one vblank since it last passed) is skipped;
/// the frame boundary is the DrawOTag after a VSync call, paced there; main-loop
/// stages 1-13 run only on a tick of the world clock, and stage 14, which builds and
/// draws the frame, runs every frame. Verdite3's FramePacing on this game's loop.
/// See "Frame pacing" in docs/DEVELOPMENT.md and "The main loop" in
/// docs/GAME_INTERNALS.md.
/// </summary>
public static class FramePacing
{
    public static readonly int FloorWait = RecompOne.Runtime.Diagnostics.Profiler.Register(
        "FramePacing.Floor (frame cap)", RecompOne.Runtime.Diagnostics.ProfileGroup.Wait);

    /// <summary>The swap's VSync(3): at most 20 frames a second on the console, and
    /// every stage runs once a frame.</summary>
    public const double DefaultTickRate = 20.0;
    public const double DefaultFps = 144.0;

    public static double LogicHz { get; private set; } = DefaultTickRate;
    public static double TargetFps { get; private set; } = DefaultFps;
    public static bool Enabled { get; private set; } = true;
    public static bool Uncapped => TargetFps <= 0.0;

    const double SpinMs = 1.5;

    /// <summary>No frame boundary for this long and stage 1 ticks from the wall clock.</summary>
    const double BoundaryDeadMs = 500.0;

    /// <summary>A gap this long is the game not drawing (a disc read), not lateness.</summary>
    const double StallMs = 250.0;

    static readonly ModInfo _self = new() { Id = "st.framepacing", Name = "Frame pacing", Version = "1.0" };

    static readonly Stopwatch _clock = Stopwatch.StartNew();
    static double _due;
    static int _vsyncCalls;
    static double _lastBoundaryMs = -1.0;
    static bool _inGame;
    static double _hostFps = -1.0;

    // The world clock: credit in ticks, advanced by wall time at each boundary.
    static double _logicCredit;
    static double _logicClockMs = -1.0;
    static bool _tickThisFrame = true;

    // Decided once per main-loop iteration, at stage 1, so all thirteen agree even
    // when the menu inside stage 3 draws frames of its own between them.
    static bool _iterationTicks = true;

    static double _fallbackNextMs = -1.0;
    static bool _fallbackSaid;

    /// <summary>How far the world clock is toward its next tick, 0..1: where a frame drawn
    /// now falls between the last two ticks. 1 with pacing off or the boundary lost.</summary>
    public static double TickFraction =>
        !Enabled || _lastBoundaryMs < 0.0 || _clock.Elapsed.TotalMilliseconds - _lastBoundaryMs > BoundaryDeadMs
            ? 1.0 : Math.Clamp(_logicCredit, 0.0, 1.0);

    /// <summary>Frame boundaries reached: an identity, not a rate.</summary>
    public static long Frames { get; private set; }

    /// <summary>World ticks taken: an identity, not a rate.</summary>
    public static long Ticks { get; private set; }

    /// <summary>Whether the main-loop iteration now running ran its stages.</summary>
    public static bool IterationTicked => _iterationTicks;

    /// <summary>Inside the main loop's own call to stage 14, the frame it draws.</summary>
    public static bool InMainLoopDraw => _mainLoopDraws > 0;

    /// <summary>Stage 14 is drawing a frame the world did not tick on: what it would
    /// advance (<see cref="DrawStageTick"/>) is held. A stepped loop's repeat counts.</summary>
    public static bool DrawingIdleFrame =>
        Enabled && ((_mainLoopDraws > 0 && !_iterationTicks) || (InSteppedLoopDraw && SteppedLoopRepeat));

    /// <summary>A loop of the game's own that steps once a tick (<see cref="PickupSmoothing"/>)
    /// is drawing a frame: presented at the drawn rate, not held by VSync.</summary>
    public static bool InSteppedLoopDraw { get; set; }

    /// <summary>That frame repeats the loop's step, drawn between two ticks.</summary>
    public static bool SteppedLoopRepeat { get; set; }

    /// <summary>The world clock ticked at the last frame boundary.</summary>
    public static bool TickedAtBoundary => _tickThisFrame;

    static int _mainLoopDraws;

    /// <summary>Raised at each frame boundary, on the game thread, once the frame's
    /// table is drawn and before the cap's wait.</summary>
    public static event Action<IMemory>? Boundary;

    static readonly HashSet<uint> _gatedSites =
        MainLoop.Stages.Where(s => s != MainLoop.Draw).Select(s => s.ReturnAddress).ToHashSet();

    public static void Configure(string? pacing, string? fps, string? tickRate, string? probe)
    {
        Enabled = pacing?.Trim() != "0";
        PacingProbe.On = probe?.Trim() == "1";
        if (!string.IsNullOrWhiteSpace(tickRate))
        {
            if (!double.TryParse(tickRate, NumberStyles.Float, CultureInfo.InvariantCulture, out double hz) || !double.IsFinite(hz))
                throw new ArgumentException($"ST_TICKRATE: cannot read '{tickRate}'");
            LogicHz = Math.Clamp(hz, 5.0, 60.0);
        }
        if (string.IsNullOrWhiteSpace(fps)) return;
        if (fps.Equals("off", StringComparison.OrdinalIgnoreCase)) TargetFps = 0.0;
        else if (double.TryParse(fps, NumberStyles.Float, CultureInfo.InvariantCulture, out double rate) && double.IsFinite(rate))
            TargetFps = Math.Clamp(rate, 5.0, 1000.0);
        else throw new ArgumentException($"ST_FPS: cannot read '{fps}'");
    }

    public static void Install()
    {
        // Only ST.EXE has a world to pace. The other programs keep the runtime's own
        // 60 Hz ceiling and their own waits; an area module loads inside ST.EXE.
        Event.AddListener<OverlayLoadedEvent>(e =>
        {
            if (_hostFps < 0.0) _hostFps = RecompOne.Runtime.Runtime.TargetFps;
            if (e.Name.StartsWith("fdat", StringComparison.Ordinal)) return;
            _inGame = e.Name == MainLoop.Overlay;
            _logicClockMs = -1.0;
            _lastBoundaryMs = -1.0;
            _mainLoopDraws = 0;
            if (Enabled) RecompOne.Runtime.Runtime.TargetFps = _inGame ? HostCeiling() : _hostFps;
        });
        Event.AddListener<VSyncEvent>(_ => PacingProbe.Vblank());
        if (!Enabled)
        {
            Console.WriteLine("[ST] pacing: off (ST_PACING=0), the game's own pace");
            return;
        }
        HookAttach.OnOverlayLoad("pacing", Attach, "See \"Frame pacing\" in docs/DEVELOPMENT.md.");
    }

    /// <summary>The drawn rate, live (Settings ▸ Testing): 0 is uncapped.</summary>
    public static void SetTarget(double fps)
    {
        TargetFps = fps <= 0.0 ? 0.0 : Math.Clamp(fps, 5.0, 1000.0);
        if (Enabled && _inGame) RecompOne.Runtime.Runtime.TargetFps = HostCeiling();
    }

    // RecompOne's own ceiling paces per VSync call; it is only a backstop here.
    static double HostCeiling() => Uncapped ? 0.0 : Math.Max(60.0, TargetFps * 2.0);

    static bool _otHooked, _vsHooked, _gateHooked;
    static readonly HashSet<uint> _stagesHooked = [];

    static bool Attach()
    {
        SymbolRegistry.Build();
        MethodInfo M(string n) => typeof(FramePacing).GetMethod(n, BindingFlags.Public | BindingFlags.Static)!;
        MethodInfo? Resolve(uint address) => SymbolRegistry.Resolve(MainLoop.Overlay, null, address);

        MethodInfo? ot = null, vs = null, gate = null;
        List<(uint, MethodInfo)> stages = [];

        // ST_PACING_NOBOUNDARY=1 leaves DrawOTag unhooked, to test the watchdog.
        bool noBoundary = Verdite.Core.Game.Env("PACING_NOBOUNDARY") == "1";
        if (!noBoundary && !_otHooked && Resolve(MainLoop.DrawOTag) is { } o
            && HookManager.AddPost(_self, o, M(nameof(AfterDrawOTag)))) ot = o;
        if (!_vsHooked && Resolve(MainLoop.VSync) is { } v && HookManager.AddPre(_self, v, M(nameof(BeforeVSync)))) vs = v;
        if (!_gateHooked && Resolve(MainLoop.FrameGate) is { } g && HookManager.AddPre(_self, g, M(nameof(BeforeFrameGate)))) gate = g;
        foreach (var stage in MainLoop.Stages)
        {
            if (_stagesHooked.Contains(stage.Routine) || Resolve(stage.Routine) is not { } t) continue;
            // Stage 14 is marked, not gated, at the outermost orders so the mark covers
            // every other hook on it.
            bool added = stage == MainLoop.Draw
                ? HookManager.AddPre(_self, t, M(nameof(BeforeDraw)), int.MinValue)
                  && HookManager.AddPost(_self, t, M(nameof(AfterDraw)), int.MaxValue)
                : HookManager.AddPre(_self, t, M(nameof(BeforeStage)));
            if (added) stages.Add((stage.Routine, t));
        }
        HookManager.Commit();

        if (ot != null && HookAttach.Installed(ot)) _otHooked = true;
        if (vs != null && HookAttach.Installed(vs)) _vsHooked = true;
        if (gate != null && HookAttach.Installed(gate)) _gateHooked = true;
        foreach (var (a, t) in stages) if (HookAttach.Installed(t)) _stagesHooked.Add(a);

        int wanted = MainLoop.Stages.Length;
        Console.WriteLine($"[ST] pacing: {(Uncapped ? "uncapped" : $"{TargetFps:0.#} fps")}, " +
                          $"boundary {(_otHooked ? "DrawOTag" : "NOT hooked")} + {(_vsHooked ? "VSync" : "NO VSync")}, " +
                          $"{_stagesHooked.Count}/{wanted} stage(s) gated or marked, frame gate {(_gateHooked ? "skipped" : "IN PLACE")}, " +
                          $"world at {LogicHz:0.#} Hz");
        bool complete = _otHooked && _vsHooked && _gateHooked && _stagesHooked.Count == wanted;
        if (!complete) Console.Error.WriteLine("[ST] pacing: attach incomplete -- will try the rest on the next overlay load.");
        return complete;
    }

    public static void BeforeDraw(CpuContext c, IMemory m)
    {
        if (c.RA == MainLoop.Draw.ReturnAddress) _mainLoopDraws++;
    }

    /// <summary>Runs even if another pre skipped the body, since HookManager runs every
    /// post; the callee restored the return address before returning.</summary>
    public static void AfterDraw(CpuContext c, IMemory m)
    {
        if (_mainLoopDraws > 0 && c.RA == MainLoop.Draw.ReturnAddress) _mainLoopDraws--;
    }

    /// <summary>The game's frame gate, skipped; the count it would have zeroed is.</summary>
    public static bool BeforeFrameGate(CpuContext c, IMemory m)
    {
        m.WriteU32(MainLoop.GateCount, 0u);
        return false;
    }

    public static void BeforeVSync(CpuContext c, IMemory m)
    {
        int mode = (int)c.A0;
        if (mode >= 0 && mode != 1) _vsyncCalls++;
    }

    /// <summary>The frame boundary: the ordering table drawn after a VSync call.</summary>
    public static void AfterDrawOTag(CpuContext c, IMemory m)
    {
        if (_vsyncCalls == 0 || !_inGame) return;
        _vsyncCalls = 0;
        Frames++;

        double now = _clock.Elapsed.TotalMilliseconds;
        _lastBoundaryMs = now;
        PacingProbe.Frame(m, now, _iterationTicks);
        Boundary?.Invoke(m);
        AdvanceLogicClock(now);
        if (!Uncapped) Floor(1000.0 / TargetFps);
    }

    static void AdvanceLogicClock(double nowMs)
    {
        bool first = _logicClockMs < 0.0;
        double dt = first ? 0.0 : nowMs - _logicClockMs;
        _logicClockMs = nowMs;

        if (first || dt > StallMs) _logicCredit = 1.0;
        else if (dt > 0.0) _logicCredit = Math.Min(_logicCredit + dt * LogicHz / 1000.0, 2.0);

        _tickThisFrame = _logicCredit >= 1.0;
        if (!_tickThisFrame) return;
        _logicCredit -= 1.0;
        PacingProbe.Tick();
        Ticks++;
    }

    /// <summary>Skip a main-loop stage on an iteration the world clock did not tick.
    /// Only the loop's own calls are gated; a stage called from elsewhere runs.</summary>
    public static bool BeforeStage(CpuContext c, IMemory m)
    {
        if (!_gatedSites.Contains(c.RA)) return true;
        if (c.RA == MainLoop.First.ReturnAddress)
        {
            double now = _clock.Elapsed.TotalMilliseconds;
            bool live = _lastBoundaryMs >= 0.0 && now - _lastBoundaryMs <= BoundaryDeadMs;
            _iterationTicks = live ? _tickThisFrame : FallbackTick(now);
        }
        PacingProbe.Stage(_iterationTicks);
        return _iterationTicks;
    }

    /// <summary>No boundary for half a second: the world ticks from the wall clock
    /// here instead, and the loop is paced here, so a lost hook cannot run the world
    /// at the drawn rate.</summary>
    static bool FallbackTick(double now)
    {
        if (!_fallbackSaid)
        {
            _fallbackSaid = true;
            Console.Error.WriteLine($"[ST] pacing: no frame boundary reached -- ticking at {LogicHz:0.#} Hz " +
                                    "from the wall clock at stage 1 instead. See \"Frame pacing\" in docs/DEVELOPMENT.md.");
        }
        double period = 1000.0 / LogicHz;
        if (_fallbackNextMs < 0.0 || now - _fallbackNextMs > 4.0 * period) _fallbackNextMs = now;
        bool tick = now >= _fallbackNextMs;
        if (tick) { _fallbackNextMs += period; PacingProbe.FallbackTick(); Ticks++; }
        _tickThisFrame = tick;
        if (!Uncapped) Floor(1000.0 / TargetFps);
        return tick;
    }

    static void Floor(double min)
    {
        double now = _clock.Elapsed.TotalMilliseconds;
        if (_due < now - min) _due = now;
        if (now < _due)
        {
            int profile = RecompOne.Runtime.Diagnostics.Profiler.Begin(FloorWait);
            double sleepUntil = _due - SpinMs;
            if (now < sleepUntil && (int)(sleepUntil - now) is > 0 and var ms) Thread.Sleep(ms);
            while (_clock.Elapsed.TotalMilliseconds < _due) Thread.SpinWait(48);
            RecompOne.Runtime.Diagnostics.Profiler.End(profile);
        }
        _due += min;
    }
}
