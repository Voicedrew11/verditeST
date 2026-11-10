using System.Reflection;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Events;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// The eye the world is placed from, carried between world ticks. The map
/// (<c>func_8001CB88</c>, the scene's cell translations) and the model drawer
/// (<c>func_80022678</c>) do not read the camera's position: each subtracts the player's
/// own, <c>+0x248</c>/<c>+0x24C</c>/<c>+0x250</c> of the block at <c>0x80198F28</c>, with the
/// two s16 at <c>+0x2CE</c> and <c>+0x2D2</c> added to Y. Those change only on a tick, so
/// with the camera alone carried the view turned at the drawn rate but walked at 20 Hz.
/// Around the draw stage's calls of the scene and of the model list, the player's eye is
/// replaced by the one interpolated between the last two ticks, and the tick's is put
/// back after each, so nothing outside the draw sees the carried one.
///
/// On with the camera (<see cref="ViewSmoothing.Enabled"/>, <c>ST_SMOOTH</c>). A step over
/// <see cref="ViewSmoothing.SnapUnits"/> in one tick snaps; an overlay load re-primes.
/// See "The eye the world is placed from" in docs/SMOOTHING.md.
/// </summary>
public static class EyeSmoothing
{
    const uint SceneSite = 0x80026D2C;   // the draw stage's call of the scene, + 8: sampled here

    /// <summary>Every draw-stage call that places the world from the player's eye, and its
    /// return address: the scene (the map's cells), the model list, the effect
    /// <c>func_800246E4</c> and the second model list <c>func_800251C8</c>; the last two
    /// draw through the model drawer as well, so left at the tick they stepped against
    /// the carried world while walking.</summary>
    static readonly (uint Routine, uint Site)[] Placers =
    [
        (0x80020A80, SceneSite), (0x800241D0, 0x80026D4C), (0x800246E4, 0x80026D54), (0x800251C8, 0x80026D5C),
    ];
    const uint PlayerX = 0x80199170, PlayerY = 0x80199174, PlayerZ = 0x80199178;
    const uint LiftAAt = 0x801991F6, LiftBAt = 0x801991FA;          // s16s the eye adds to Y

    readonly record struct Eye(int X, int Y, int Z, short LiftA, short LiftB)
    {
        public static Eye Read(IMemory m) => new(
            (int)m.ReadU32(PlayerX), (int)m.ReadU32(PlayerY), (int)m.ReadU32(PlayerZ),
            (short)m.ReadU16(LiftAAt), (short)m.ReadU16(LiftBAt));

        public void Write(IMemory m)
        {
            m.WriteU32(PlayerX, (uint)X); m.WriteU32(PlayerY, (uint)Y); m.WriteU32(PlayerZ, (uint)Z);
            m.WriteU16(LiftAAt, (ushort)LiftA); m.WriteU16(LiftBAt, (ushort)LiftB);
        }
    }

    static readonly ModInfo _self = new() { Id = "st.eyesmoothing", Name = "Eye smoothing", Version = "1.0" };

    static Eye _prev, _cur, _handed;
    static bool _primed, _restore;
    static uint _site;

    // ST_SMOOTH_PROBE=1: a line a second of the model drawer's calls (func_80022678, which
    // subtracts the player's eye) made inside each placer, carried and not.
    const uint ModelDrawer = 0x80022678;
    static readonly bool _probe = Verdite.Core.Game.Env("SMOOTH_PROBE")?.Trim() == "1";
    static readonly long[] _carried = new long[4], _uncarried = new long[4];
    static int _inside = -1;
    static double _probeAt = -1.0;
    static long _tick = -1;

    static bool Active => ViewSmoothing.Enabled && FramePacing.Enabled;

    public static void Install()
    {
        Event.AddListener<OverlayLoadedEvent>(_ => _primed = false);
        HookAttach.OnOverlayLoad("eye smoothing", Attach);
    }

    static bool Attach()
    {
        SymbolRegistry.Build();
        MethodInfo Hook(string n) => typeof(EyeSmoothing).GetMethod(n, BindingFlags.Public | BindingFlags.Static)!;
        var targets = new List<MethodInfo>();
        foreach (var (routine, _) in Placers)
        {
            if (SymbolRegistry.Resolve(MainLoop.Overlay, null, routine) is not { } t) return false;
            targets.Add(t);
        }
        foreach (var t in targets)
        {
            HookManager.AddPre(_self, t, Hook(nameof(Before)), order: int.MinValue);
            HookManager.AddPost(_self, t, Hook(nameof(After)), order: int.MaxValue);
        }
        if (_probe && SymbolRegistry.Resolve(MainLoop.Overlay, null, ModelDrawer) is { } drawer)
        {
            HookManager.AddPre(_self, drawer, Hook(nameof(ProbeModel)));
            foreach (var t in targets)
            {
                HookManager.AddPre(_self, t, Hook(nameof(ProbeEnter)), order: int.MaxValue);
                HookManager.AddPost(_self, t, Hook(nameof(ProbeLeave)), order: int.MinValue);
            }
        }
        HookManager.Commit();
        return targets.All(HookAttach.Installed);
    }

    public static void Before(CpuContext c, IMemory m)
    {
        _restore = false;
        if (!IsPlacerSite(c.RA) || !FramePacing.InMainLoopDraw) return;
        if (!Active) { _primed = false; return; }
        var handed = Eye.Read(m);
        if (c.RA == SceneSite) Sample(handed);
        if (!_primed) return;
        _handed = handed;
        Lerp(_prev, _cur, FramePacing.TickFraction).Write(m);
        _restore = true;
        _site = c.RA;
    }

    public static void After(CpuContext c, IMemory m)
    {
        if (!_restore) return;
        _restore = false;
        _handed.Write(m);
    }

    public static void ProbeEnter(CpuContext c, IMemory m) =>
        _inside = FramePacing.InMainLoopDraw ? Array.FindIndex(Placers, p => p.Site == c.RA) : -1;

    public static void ProbeLeave(CpuContext c, IMemory m)
    {
        if (_inside != Array.FindIndex(Placers, p => p.Site == c.RA) || _inside < 0) return;
        _inside = -1;
        if (c.RA != Placers[^1].Site) return;
        double now = Environment.TickCount64 / 1000.0;
        if (_probeAt < 0.0) _probeAt = now;
        double dt = now - _probeAt;
        if (dt < 1.0) return;
        var parts = Placers.Select((p, k) => $"{p.Routine:X8} {_carried[k] / dt:0.0} carried {_uncarried[k] / dt:0.0} not");
        Console.WriteLine($"[ST] eye smoothing: model draws/s inside {string.Join(", ", parts)}");
        Console.Out.Flush();
        _probeAt = now;
        Array.Clear(_carried); Array.Clear(_uncarried);
    }

    public static void ProbeModel(CpuContext c, IMemory m)
    {
        if (_inside < 0) return;
        if (_restore) _carried[_inside]++; else _uncarried[_inside]++;
    }

    static bool IsPlacerSite(uint ra)
    {
        foreach (var (_, site) in Placers) if (ra == site) return true;
        return false;
    }

    /// <summary>Once a frame, at the scene: the last two ticks' eyes.</summary>
    static void Sample(in Eye handed)
    {
        long tick = FramePacing.Ticks;
        if (!_primed)
        {
            _prev = _cur = handed;
            _primed = true;
            _tick = tick;
        }
        else if (tick != _tick && FramePacing.IterationTicked)
        {
            _tick = tick;
            _prev = _cur;
            _cur = handed;
            if (Jump(_prev, _cur)) _prev = _cur;
        }
        else if (handed != _cur)
        {
            // Moved without a tick of the world: no pair to carry.
            _prev = _cur = handed;
        }
    }

    static bool Jump(in Eye a, in Eye b) =>
        Math.Abs((long)b.X - a.X) > ViewSmoothing.SnapUnits || Math.Abs((long)b.Y - a.Y) > ViewSmoothing.SnapUnits ||
        Math.Abs((long)b.Z - a.Z) > ViewSmoothing.SnapUnits ||
        Math.Abs(b.LiftA - a.LiftA) > ViewSmoothing.SnapUnits || Math.Abs(b.LiftB - a.LiftB) > ViewSmoothing.SnapUnits;

    static Eye Lerp(in Eye a, in Eye b, double t) => new(
        Mix(a.X, b.X, t), Mix(a.Y, b.Y, t), Mix(a.Z, b.Z, t),
        (short)Mix(a.LiftA, b.LiftA, t), (short)Mix(a.LiftB, b.LiftB, t));

    static int Mix(int a, int b, double t) => (int)(a + Math.Round(((long)b - a) * t));
}
