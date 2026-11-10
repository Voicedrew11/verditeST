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
    const uint Scene = 0x80020A80, ModelList = 0x800241D0;
    const uint SceneSite = 0x80026D2C, ModelListSite = 0x80026D4C;   // the draw stage's calls, + 8
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
        if (SymbolRegistry.Resolve(MainLoop.Overlay, null, Scene) is not { } scene) return false;
        if (SymbolRegistry.Resolve(MainLoop.Overlay, null, ModelList) is not { } models) return false;
        foreach (var t in new[] { scene, models })
        {
            HookManager.AddPre(_self, t, Hook(nameof(Before)), order: int.MinValue);
            HookManager.AddPost(_self, t, Hook(nameof(After)), order: int.MaxValue);
        }
        HookManager.Commit();
        return HookAttach.Installed(scene) && HookAttach.Installed(models);
    }

    public static void Before(CpuContext c, IMemory m)
    {
        _restore = false;
        if (c.RA is not (SceneSite or ModelListSite) || !FramePacing.InMainLoopDraw) return;
        if (!Active) { _primed = false; return; }
        var handed = Eye.Read(m);
        if (c.RA == SceneSite) Sample(handed);
        if (!_primed) return;
        _handed = handed;
        Lerp(_prev, _cur, FramePacing.TickFraction).Write(m);
        _restore = true;
    }

    public static void After(CpuContext c, IMemory m)
    {
        if (!_restore) return;
        _restore = false;
        _handed.Write(m);
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
