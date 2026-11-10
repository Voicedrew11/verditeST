using System.Reflection;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Events;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// The models carried between world ticks: around the model list
/// (<c>func_800241D0</c>, the draw stage's call), every entity record's position and
/// angles are replaced by the ones interpolated between the last two ticks at the
/// clock's fraction, and the tick's are put back after it. The model drawer
/// <c>func_80022678</c> reads them there: the position at <c>+0x74</c> (three s32) and the
/// angles at <c>+0x8C</c>, <c>+0x8E</c>, <c>+0x90</c> (0x1000 a turn), of 0xD4-byte records
/// from <c>0x801BA138</c>.
///
///     ST_SMOOTH=0                 off, with the camera (on whenever pacing is)
///     ST_SMOOTH_MODELS=0          the models alone off
///     ST_SMOOTH_PROBE=1           a line a second with the camera's: entities carried, snaps
///
/// A record whose step in one tick is over <see cref="SnapUnits"/> or <see cref="SnapAngle"/>
/// snaps (a new entity in the slot, a teleport); an overlay load re-primes. See "Models
/// carried" in docs/SMOOTHING.md.
/// </summary>
public static class ModelSmoothing
{
    const uint ModelList = 0x800241D0;
    const uint DrawStageSite = 0x80026D4C;   // the draw stage's call of the model list, + 8
    const uint Records = 0x801BA138;
    const uint RecordBytes = 0xD4;
    const int Slots = 0x6100 / 0xD4;         // the records end where the list's count is
    const uint Position = 0x74, Angles = 0x8C;

    public const int SnapUnits = 0x800;
    public const int SnapAngle = 0x300;

    readonly record struct Pose(int X, int Y, int Z, short A, short B, short C);

    static readonly ModInfo _self = new() { Id = "st.modelsmoothing", Name = "Model smoothing", Version = "1.0" };

    static bool _enabled = true, _probe;
    static readonly Pose[] _prev = new Pose[Slots], _cur = new Pose[Slots], _handed = new Pose[Slots];
    static bool _primed, _restore;
    static long _tick = -1;
    static double _probeAt = -1.0;
    static long _carried, _snaps, _frames;

    public static void Configure(string? smooth, string? models, string? probe)
    {
        _enabled = smooth?.Trim() is not ("0" or "off") && models?.Trim() is not ("0" or "off");
        _probe = probe?.Trim() == "1";
    }

    public static void Install()
    {
        Event.AddListener<OverlayLoadedEvent>(_ => _primed = false);
        HookAttach.OnOverlayLoad("model smoothing", () =>
        {
            SymbolRegistry.Build();
            if (SymbolRegistry.Resolve(MainLoop.Overlay, null, ModelList) is not { } t) return false;
            var self = typeof(ModelSmoothing);
            HookManager.AddPre(_self, t, self.GetMethod(nameof(Before), BindingFlags.Public | BindingFlags.Static)!);
            HookManager.AddPost(_self, t, self.GetMethod(nameof(After), BindingFlags.Public | BindingFlags.Static)!);
            HookManager.Commit();
            return HookAttach.Installed(t);
        });
        Console.WriteLine($"[ST] model smoothing: {(_enabled ? "on" : "off")}");
    }

    public static void Before(CpuContext c, IMemory m)
    {
        _restore = false;
        if (!_enabled || !FramePacing.Enabled || c.RA != DrawStageSite || !FramePacing.InMainLoopDraw) return;
        for (int i = 0; i < Slots; i++) _handed[i] = Read(m, Slot(i));

        long tick = FramePacing.Ticks;
        if (!_primed)
        {
            Array.Copy(_handed, _prev, Slots);
            Array.Copy(_handed, _cur, Slots);
            _primed = true;
            _tick = tick;
        }
        else if (tick != _tick && FramePacing.IterationTicked)
        {
            _tick = tick;
            Array.Copy(_cur, _prev, Slots);
            Array.Copy(_handed, _cur, Slots);
            for (int i = 0; i < Slots; i++)
                if (Jump(_prev[i], _cur[i])) { _prev[i] = _cur[i]; _snaps++; }
        }

        double t = FramePacing.TickFraction;
        for (int i = 0; i < Slots; i++)
        {
            // Moved without a tick: no pair to carry; draw what was handed.
            if (_handed[i] != _cur[i]) { _prev[i] = _cur[i] = _handed[i]; continue; }
            if (_prev[i] == _cur[i]) continue;
            Write(m, Slot(i), Lerp(_prev[i], _cur[i], t));
            _carried++;
        }
        _restore = true;
        if (_probe) Probe();
    }

    public static void After(CpuContext c, IMemory m)
    {
        if (!_restore) return;
        _restore = false;
        for (int i = 0; i < Slots; i++) Write(m, Slot(i), _handed[i]);
    }

    static uint Slot(int i) => Records + (uint)i * RecordBytes;

    static Pose Read(IMemory m, uint r) => new(
        (int)m.ReadU32(r + Position), (int)m.ReadU32(r + Position + 4), (int)m.ReadU32(r + Position + 8),
        (short)m.ReadU16(r + Angles), (short)m.ReadU16(r + Angles + 2), (short)m.ReadU16(r + Angles + 4));

    static void Write(IMemory m, uint r, in Pose p)
    {
        m.WriteU32(r + Position, (uint)p.X); m.WriteU32(r + Position + 4, (uint)p.Y); m.WriteU32(r + Position + 8, (uint)p.Z);
        m.WriteU16(r + Angles, (ushort)p.A); m.WriteU16(r + Angles + 2, (ushort)p.B); m.WriteU16(r + Angles + 4, (ushort)p.C);
    }

    static bool Jump(in Pose a, in Pose b) =>
        Math.Abs((long)b.X - a.X) > SnapUnits || Math.Abs((long)b.Y - a.Y) > SnapUnits ||
        Math.Abs((long)b.Z - a.Z) > SnapUnits || Math.Abs(Wrap(b.A - a.A)) > SnapAngle ||
        Math.Abs(Wrap(b.B - a.B)) > SnapAngle || Math.Abs(Wrap(b.C - a.C)) > SnapAngle;

    static int Wrap(int d) => ((d + 0x800) & 0xFFF) - 0x800;

    static Pose Lerp(in Pose a, in Pose b, double t) => new(
        Mix(a.X, b.X, t), Mix(a.Y, b.Y, t), Mix(a.Z, b.Z, t), Turn(a.A, b.A, t), Turn(a.B, b.B, t), Turn(a.C, b.C, t));

    static int Mix(int a, int b, double t) => (int)(a + Math.Round(((long)b - a) * t));

    static short Turn(short a, short b, double t)
    {
        int v = a + (int)Math.Round(Wrap(b - a) * t);
        if ((uint)a < 0x1000u && (uint)b < 0x1000u) v &= 0xFFF;
        return (short)v;
    }

    static void Probe()
    {
        _frames++;
        double now = Environment.TickCount64 / 1000.0;
        if (_probeAt < 0.0) _probeAt = now;
        double dt = now - _probeAt;
        if (dt < 1.0) return;
        Console.WriteLine($"[ST] model smoothing: {_frames / dt:0.0} frame(s)/s, {_carried / dt:0.0} entity draws carried/s, {_snaps} snap(s)");
        Console.Out.Flush();
        _probeAt = now;
        _frames = _carried = _snaps = 0;
    }
}
