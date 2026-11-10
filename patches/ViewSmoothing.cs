using System.Reflection;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Events;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>The eye as stage 10 hands it to the draw stage: a position (three s32 and a
/// fourth word, kept as it came) and three angles, 0x1000 a turn.</summary>
public readonly record struct Camera(int X, int Y, int Z, int W, short Pitch, short Yaw, short Roll)
{
    public static Camera Read(IMemory m, uint pos, uint rot) => new(
        (int)m.ReadU32(pos), (int)m.ReadU32(pos + 4u), (int)m.ReadU32(pos + 8u), (int)m.ReadU32(pos + 12u),
        (short)m.ReadU16(rot), (short)m.ReadU16(rot + 2u), (short)m.ReadU16(rot + 4u));

    public void Write(IMemory m, uint pos, uint rot)
    {
        m.WriteU32(pos, (uint)X); m.WriteU32(pos + 4u, (uint)Y); m.WriteU32(pos + 8u, (uint)Z);
        m.WriteU16(rot, (ushort)Pitch); m.WriteU16(rot + 2u, (ushort)Yaw); m.WriteU16(rot + 4u, (ushort)Roll);
    }
}

/// <summary>
/// The camera carried between world ticks: each frame the main loop draws, the camera
/// set-up (<c>func_800176A8</c>, the draw stage's first call) is handed the eye
/// interpolated between the last two ticks at the clock's fraction, and the tick's eye
/// is put back after it. Stage 10 fills the two buffers on the main loop's stack only on
/// a tick, so between ticks they hold the last one.
///
///     ST_SMOOTH=0          off (on whenever pacing is)
///     ST_SMOOTH_PROBE=1    a line a second: frames drawn, distinct cameras, ticks, snaps
///
/// Interpolates, never extrapolates; a step larger than <see cref="SnapUnits"/> or
/// <see cref="SnapAngle"/> in one tick snaps, and an overlay load re-primes. Verdite1's
/// ViewSmoothing on this game's camera. See "The camera carried" in docs/SMOOTHING.md.
/// </summary>
public static class ViewSmoothing
{
    const uint CameraSetup = 0x800176A8;
    const uint DrawStageSite = 0x80026B30;   // the draw stage's call of the camera set-up, + 8

    /// <summary>A position step in one tick past which the view snaps (a warp, a load).</summary>
    public const int SnapUnits = 0x1000;

    /// <summary>An angle step in one tick past which the view snaps (a cut), 0x1000 a turn.</summary>
    public const int SnapAngle = 0x300;

    static readonly ModInfo _self = new() { Id = "st.viewsmoothing", Name = "View smoothing", Version = "1.0" };

    static bool _enabled = true, _probe;
    static Camera _prev, _cur, _handed, _probed;
    static uint _pos, _rot;
    static bool _primed, _restore;
    static long _tick = -1;
    static double _probeAt = -1.0;
    static long _frames, _moved, _samples, _snaps, _backward, _otherCell;

    static bool Active => _enabled && FramePacing.Enabled;

    public static void Configure(string? mode, string? probe)
    {
        _enabled = mode?.Trim() is not ("0" or "off");
        _probe = probe?.Trim() == "1";
    }

    public static void Install()
    {
        Event.AddListener<OverlayLoadedEvent>(_ => _primed = false);
        HookAttach.OnOverlayLoad("view smoothing", () =>
        {
            SymbolRegistry.Build();
            if (SymbolRegistry.Resolve(MainLoop.Overlay, null, CameraSetup) is not { } t) return false;
            var self = typeof(ViewSmoothing);
            HookManager.AddPre(_self, t, self.GetMethod(nameof(Before), BindingFlags.Public | BindingFlags.Static)!);
            HookManager.AddPost(_self, t, self.GetMethod(nameof(After), BindingFlags.Public | BindingFlags.Static)!);
            HookManager.Commit();
            return HookAttach.Installed(t);
        });
        Console.WriteLine($"[ST] view smoothing: {(_enabled ? "on" : "off")}");
    }

    public static void Before(CpuContext c, IMemory m)
    {
        _restore = false;
        if (c.RA != DrawStageSite || !FramePacing.InMainLoopDraw || c.A0 == 0 || c.A1 == 0) return;
        if (!Active) { _primed = false; return; }
        _pos = c.A0;
        _rot = c.A1;
        var handed = Camera.Read(m, _pos, _rot);

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
            _samples++;
            if (Jump(_prev, _cur)) { _prev = _cur; _snaps++; }
        }
        else if (handed != _cur)
        {
            // Moved without a tick of the world: no pair to carry.
            _prev = _cur = handed;
            _snaps++;
        }

        var view = MouseLook.Lead(Lerp(_prev, _cur, FramePacing.TickFraction), _cur);
        _handed = handed;
        view.Write(m, _pos, _rot);
        _restore = true;
        if (_probe) Probe(view);
    }

    public static void After(CpuContext c, IMemory m)
    {
        if (!_restore) return;
        _restore = false;
        _handed.Write(m, _pos, _rot);
    }

    static bool Jump(in Camera a, in Camera b) =>
        Math.Abs((long)b.X - a.X) > SnapUnits || Math.Abs((long)b.Y - a.Y) > SnapUnits ||
        Math.Abs((long)b.Z - a.Z) > SnapUnits ||
        Math.Abs(Wrap(b.Pitch - a.Pitch)) > SnapAngle || Math.Abs(Wrap(b.Yaw - a.Yaw)) > SnapAngle ||
        Math.Abs(Wrap(b.Roll - a.Roll)) > SnapAngle;

    static int Wrap(int d) => ((d + 0x800) & 0xFFF) - 0x800;

    static Camera Lerp(in Camera a, in Camera b, double t) => new(
        Mix(a.X, b.X, t), Mix(a.Y, b.Y, t), Mix(a.Z, b.Z, t), b.W,
        Turn(a.Pitch, b.Pitch, t), Turn(a.Yaw, b.Yaw, t), Turn(a.Roll, b.Roll, t));

    static int Mix(int a, int b, double t) => (int)(a + Math.Round(((long)b - a) * t));

    /// <summary>The short way round at 12 bits, kept in 0..0xFFF when both ends are.</summary>
    static short Turn(short a, short b, double t)
    {
        int v = a + (int)Math.Round(Wrap(b - a) * t);
        if ((uint)a < 0x1000u && (uint)b < 0x1000u) v &= 0xFFF;
        return (short)v;
    }

    static bool Against(long from, long to, long tickFrom, long tickTo) =>
        Math.Sign(to - from) * Math.Sign(tickTo - tickFrom) < 0;

    static void Probe(in Camera view)
    {
        _frames++;
        if (view != _probed) _moved++;
        // A step against the way the last tick went, on any axis: the carry going back.
        if (Against(_probed.X, view.X, _prev.X, _cur.X) || Against(_probed.Z, view.Z, _prev.Z, _cur.Z) ||
            Against(0, Wrap(view.Yaw - _probed.Yaw), 0, Wrap(_cur.Yaw - _prev.Yaw))) _backward++;
        // The cell cull (func_8001C100) reads the player's tick position, cells 0x1000 a side:
        // a carried eye in another cell than the tick's may see a cell the cull left out.
        if (((view.X ^ _cur.X) | (view.Y ^ _cur.Y) | (view.Z ^ _cur.Z)) >> 12 != 0) _otherCell++;
        _probed = view;
        double now = Environment.TickCount64 / 1000.0;
        if (_probeAt < 0.0) _probeAt = now;
        double dt = now - _probeAt;
        if (dt < 1.0) return;
        Console.WriteLine($"[ST] view smoothing: {_frames / dt:0.0} frame(s)/s, {_moved / dt:0.0} with a new camera, " +
                          $"{_samples / dt:0.0} tick sample(s)/s, {_snaps} snap(s), {_backward} backward, {_otherCell} in another cell; drawn [{view.X},{view.Y},{view.Z},{view.W}] " +
                          $"angles {view.Pitch} {view.Yaw} {view.Roll}; handed [{_cur.X},{_cur.Y},{_cur.Z}] yaw {_cur.Yaw}");
        Console.Out.Flush();
        _probeAt = now;
        _frames = _moved = _samples = _snaps = _backward = _otherCell = 0;
    }
}
