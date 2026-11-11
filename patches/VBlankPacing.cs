using System.Diagnostics;
using System.Reflection;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Events;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// Make every program's <c>VSync(n)</c> wait for n vblanks, as libetc's does.
///
///     ST_VBLANKPACING=0        leave VSync on the runtime's clock: the world runs at
///                              60 instead of 20 with pacing off (a comparison only)
///     ST_VBLANKPACING_PROBE=1  a line a second: calls held, by mode, and the mean wait
///
/// The runtime's non-blocking VSync presents and returns for any mode but the
/// queries, held only by its 60 Hz ceiling. ST.EXE's and VS.EXE's frame swaps call
/// VSync(3), so the console draws and ticks at most 20 times a second and the port
/// ran at 60; every program also waits 3 or 4 vblanks between two CD commands, and
/// STR.EXE and END.EXE wait 60 and 10.
/// This keeps a 60 Hz grid: a call returns at the first vblank at least n after the
/// last one returned (mode 0 is one). Queries (mode &lt; 0, and 1) pass through.
///
/// With <see cref="FramePacing"/> on, the main loop's own draw (stage 14 called from
/// the loop, <see cref="FramePacing.InMainLoopDraw"/>) is exempt: its frame boundary
/// is FramePacing's, as is a loop's that <see cref="PickupSmoothing"/> steps once a tick.
/// Other loops that call stage 14 themselves (the menu, fades) are held, so they step
/// at the console's rate. Unlike
/// Verdite3's, this runs with pacing off too, since that is what holds the game to
/// its own rate there. See "Frame pacing" in docs/DEVELOPMENT.md.
/// </summary>
public static class VBlankPacing
{
    public static readonly int VBlankWait = RecompOne.Runtime.Diagnostics.Profiler.Register(
        "VBlankPacing (VSync(n) held to n vblanks)", RecompOne.Runtime.Diagnostics.ProfileGroup.Wait);

    const double VBlankMs = 1000.0 / 60.0;

    /// <summary>Spin the last stretch; Thread.Sleep granularity is a few ms.</summary>
    const double SpinMs = 1.5;

    static readonly ModInfo _self = new()
    {
        Id = "st.vblankpacing",
        Name = "VBlank pacing",
        Version = "1.0",
        Description = "VSync(n) waits n vblanks, as the console's did.",
    };

    public static bool Enabled { get; set; } = true;
    static bool _probe;

    /// <summary>libetc's <c>VSync</c> in each program (docs/RECOMPILATION.md). Each is a
    /// program's own copy, so a call can only come from the program running.</summary>
    static readonly (string Overlay, uint Address)[] VSyncs =
    [
        ("str", 0x8001908C), (MainLoop.Overlay, MainLoop.VSync), ("title", 0x8001FA0C),
        ("optxt", 0x80017220), ("vs", 0x80044024), ("end", 0x80018BFC),
    ];

    static readonly Stopwatch _clock = Stopwatch.StartNew();

    /// <summary>When the last held call returned, on the 60 Hz grid; negative seeds it.</summary>
    static double _lastMs = -1.0;

    static double _windowStart;
    static readonly SortedDictionary<int, long> _heldByMode = [];
    static double _waitMs;
    static long _exempt;

    static readonly HashSet<string> _hooked = [];

    public static void Configure(string? enabled, string? probe)
    {
        Enabled = enabled?.Trim() != "0";
        _probe = probe?.Trim() == "1";
    }

    public static void Install()
    {
        // A program starts on a fresh grid; an area module loads inside ST.EXE.
        Event.AddListener<OverlayLoadedEvent>(e =>
        {
            if (!e.Name.StartsWith("fdat", StringComparison.Ordinal)) _lastMs = -1.0;
        });
        HookAttach.OnOverlayLoad("vblank pacing", Attach, "See \"Frame pacing\" in docs/DEVELOPMENT.md.");
    }

    static bool Attach()
    {
        SymbolRegistry.Build();
        MethodInfo M(string n) => typeof(VBlankPacing).GetMethod(n, BindingFlags.Public | BindingFlags.Static)!;

        List<(string, MethodInfo)> added = [];
        foreach (var (overlay, address) in VSyncs)
            if (!_hooked.Contains(overlay) && SymbolRegistry.Resolve(overlay, null, address) is { } t
                && HookManager.AddPre(_self, t, M(nameof(BeforeVSync)))) added.Add((overlay, t));
        HookManager.Commit();

        foreach (var (overlay, t) in added) if (HookAttach.Installed(t)) _hooked.Add(overlay);
        Console.WriteLine($"[ST] vblank pacing: {(Enabled ? "on" : "off")}, VSync held in {_hooked.Count}/{VSyncs.Length} programs");
        return _hooked.Count == VSyncs.Length;
    }

    /// <summary>A VSync call about to run: a present waits its mode's vblanks.</summary>
    public static void BeforeVSync(CpuContext c, IMemory m)
    {
        if (!Enabled || RecompOne.Runtime.Interrupts.Turbo) return;
        int mode = (int)c.A0;
        if (mode < 0 || mode == 1) return;
        if (FramePacing.Enabled && (FramePacing.InMainLoopDraw || FramePacing.InSteppedLoopDraw)) { _exempt++; return; }

        int vblanks = mode == 0 ? 1 : mode;
        double now = _clock.Elapsed.TotalMilliseconds;
        if (_lastMs < 0.0) _lastMs = now;

        // At least n vblanks after the last return, and on a vblank: a late caller
        // gets the next one, not the time it lost back.
        double due = _lastMs + vblanks * VBlankMs;
        if (due < now) due = _lastMs + Math.Ceiling((now - _lastMs) / VBlankMs) * VBlankMs;
        _lastMs = due;

        Wait(now, due);
        _waitMs += due > now ? due - now : 0.0;
        _heldByMode[mode] = _heldByMode.GetValueOrDefault(mode) + 1;
        Probe(now);
    }

    static void Wait(double now, double due)
    {
        if (now >= due) return;
        int profile = RecompOne.Runtime.Diagnostics.Profiler.Begin(VBlankWait);
        double sleepUntil = due - SpinMs;
        if (now < sleepUntil && (int)(sleepUntil - now) is > 0 and var ms) Thread.Sleep(ms);
        while (_clock.Elapsed.TotalMilliseconds < due) Thread.SpinWait(48);
        RecompOne.Runtime.Diagnostics.Profiler.End(profile);
    }

    static void Probe(double now)
    {
        if (!_probe) return;
        if (_windowStart <= 0.0) { _windowStart = now; return; }
        double elapsed = now - _windowStart;
        if (elapsed < 1000.0) return;

        long held = _heldByMode.Values.Sum();
        var modes = string.Join(", ", _heldByMode.Select(x => $"{x.Value * 1000.0 / elapsed:0.0} mode {x.Key}"));
        Console.WriteLine($"[ST] vblank pacing: {held * 1000.0 / elapsed:0.0} VSync call(s)/s held ({modes}), " +
                          $"mean wait {_waitMs / Math.Max(1, held):0.0} ms, {_exempt * 1000.0 / elapsed:0.0} exempted/s");
        _windowStart = now;
        _waitMs = 0.0;
        _exempt = 0;
        _heldByMode.Clear();
    }
}
