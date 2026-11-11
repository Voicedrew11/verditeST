using RecompOne.Runtime.Memory;

namespace St;

/// <summary>
/// Where one drawer's records keep what places and poses a model: a position (three s32),
/// three angles (s16, 0x1000 a turn) and a clip time (u16, wrapping at 0x1000), in
/// records of <c>RecordBytes</c> from <c>Records</c>. <c>Listed</c> names the slots in use
/// this frame (all <c>MaxSlots</c> when null).
/// </summary>
public sealed record CarriedList(
    string Name, uint Routine, uint DrawSite, uint Records, uint RecordBytes, int MaxSlots,
    uint Position, uint Angles, uint ClipTime, Func<IMemory, IEnumerable<int>>? Listed = null);

/// <summary>
/// One record list carried between world ticks: on each drawn frame every listed slot's
/// position, angles and clip time are replaced by the ones interpolated between its last
/// two ticks, and <see cref="Restore"/> puts the tick's back. A slot that steps over
/// <see cref="SnapUnits"/> or <see cref="SnapAngle"/> in one tick, or that was not listed on
/// the tick before, is drawn as handed. See "Models carried" in docs/SMOOTHING.md.
/// </summary>
public sealed class RecordCarry(CarriedList list)
{
    public const int SnapUnits = 0x800;
    public const int SnapAngle = 0x300;
    const long Unseen = long.MinValue;

    readonly record struct Pose(int X, int Y, int Z, short A, short B, short C, short T);

    readonly Pose[] _prev = new Pose[list.MaxSlots], _cur = new Pose[list.MaxSlots], _handed = new Pose[list.MaxSlots];
    readonly long[] _seen = Enumerable.Repeat(Unseen, list.MaxSlots).ToArray();
    readonly List<int> _slots = new();
    long _tick = -1;
    bool _restore;

    public CarriedList List => list;
    public long Carried { get; set; }
    public long Snaps { get; set; }

    /// <summary>Forget every slot's ticks (an overlay load, smoothing switched off).</summary>
    public void Reprime() => Array.Fill(_seen, Unseen);

    /// <summary>Hand the drawer this frame's carried records. <paramref name="tick"/> is the
    /// world clock's count, <paramref name="ticked"/> whether this iteration ran a tick.</summary>
    public void Carry(IMemory m, long tick, bool ticked, double fraction)
    {
        CollectSlots(m);
        bool newTick = tick != _tick && ticked;
        long last = _tick;
        if (newTick) _tick = tick;
        foreach (int i in _slots)
        {
            var handed = _handed[i] = Read(m, Slot(i));
            if (newTick)
            {
                bool continuous = _seen[i] != Unseen && _seen[i] == last;
                _prev[i] = continuous ? _cur[i] : handed;
                _cur[i] = handed;
                _seen[i] = tick;
                if (Jump(_prev[i], _cur[i])) { _prev[i] = _cur[i]; Snaps++; }
            }
            else if (_seen[i] != _tick || handed != _cur[i])
            {
                // First seen between ticks, or moved without a tick: no pair; draw as handed.
                _prev[i] = _cur[i] = handed;
                _seen[i] = _tick;
                continue;
            }
            if (_prev[i] == _cur[i]) continue;
            Write(m, Slot(i), Lerp(_prev[i], _cur[i], fraction));
            Carried++;
        }
        _restore = true;
    }

    /// <summary>Put the tick's records back after the drawer.</summary>
    public void Restore(IMemory m)
    {
        if (!_restore) return;
        foreach (int i in _slots) Write(m, Slot(i), _handed[i]);
        _restore = false;
    }

    void CollectSlots(IMemory m)
    {
        _slots.Clear();
        if (list.Listed is null)
        {
            for (int i = 0; i < list.MaxSlots; i++) _slots.Add(i);
            return;
        }
        foreach (int i in list.Listed(m))
            if (i >= 0 && i < list.MaxSlots && !_slots.Contains(i)) _slots.Add(i);
    }

    uint Slot(int i) => list.Records + (uint)i * list.RecordBytes;

    Pose Read(IMemory m, uint r) => new(
        (int)m.ReadU32(r + list.Position), (int)m.ReadU32(r + list.Position + 4), (int)m.ReadU32(r + list.Position + 8),
        (short)m.ReadU16(r + list.Angles), (short)m.ReadU16(r + list.Angles + 2), (short)m.ReadU16(r + list.Angles + 4),
        (short)m.ReadU16(r + list.ClipTime));

    void Write(IMemory m, uint r, in Pose p)
    {
        m.WriteU32(r + list.Position, (uint)p.X); m.WriteU32(r + list.Position + 4, (uint)p.Y); m.WriteU32(r + list.Position + 8, (uint)p.Z);
        m.WriteU16(r + list.Angles, (ushort)p.A); m.WriteU16(r + list.Angles + 2, (ushort)p.B); m.WriteU16(r + list.Angles + 4, (ushort)p.C);
        m.WriteU16(r + list.ClipTime, (ushort)p.T);
    }

    static bool Jump(in Pose a, in Pose b) =>
        Math.Abs((long)b.X - a.X) > SnapUnits || Math.Abs((long)b.Y - a.Y) > SnapUnits ||
        Math.Abs((long)b.Z - a.Z) > SnapUnits || Math.Abs(Wrap(b.A - a.A)) > SnapAngle ||
        Math.Abs(Wrap(b.B - a.B)) > SnapAngle || Math.Abs(Wrap(b.C - a.C)) > SnapAngle ||
        Math.Abs(Wrap(b.T - a.T)) > SnapAngle;

    static int Wrap(int d) => ((d + 0x800) & 0xFFF) - 0x800;

    static Pose Lerp(in Pose a, in Pose b, double t) => new(
        Mix(a.X, b.X, t), Mix(a.Y, b.Y, t), Mix(a.Z, b.Z, t), Turn(a.A, b.A, t), Turn(a.B, b.B, t), Turn(a.C, b.C, t),
        Turn(a.T, b.T, t));

    static int Mix(int a, int b, double t) => (int)(a + Math.Round(((long)b - a) * t));

    static short Turn(short a, short b, double t)
    {
        int v = a + (int)Math.Round(Wrap(b - a) * t);
        if ((uint)a < 0x1000u && (uint)b < 0x1000u) v &= 0xFFF;
        return (short)v;
    }
}
