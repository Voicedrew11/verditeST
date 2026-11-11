using System.Reflection;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Events;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// The models carried between world ticks. Two drawers in the draw stage place and pose
/// models from records the world updates on a tick; around each drawer's draw-stage call
/// every listed record's position, angles and clip time are replaced by the ones
/// interpolated between its last two ticks (<see cref="RecordCarry"/>), and the tick's are
/// put back after it. Each list is a row of <see cref="Lists"/>:
///
/// - the entities, <c>func_800241D0</c>: 0xD4-byte records from <c>0x801BA138</c>, position
///   <c>+0x74</c>, angles <c>+0x8C</c>, clip time <c>+0x2A</c>;
/// - the objects, <c>func_800246E4</c>: 0x58-byte records from <c>0x801C2230</c>, those in
///   use named by the u16 indices at <c>0x801C74C4</c>, as many as the byte at
///   <c>0x801C7584</c>; position <c>+0x24</c>, angles <c>+0x34</c>, clip time <c>+0x10</c>.
///
///     ST_SMOOTH=0                 off, with the camera (on whenever pacing is)
///     ST_SMOOTH_MODELS=0          the models alone off
///     ST_SMOOTH_PROBE=1           a line a second: draws carried per list, snaps, packets
///
/// An overlay load re-primes. See "Models carried" in docs/SMOOTHING.md.
/// </summary>
public static class ModelSmoothing
{
    const uint ObjectIndices = 0x801C74C4, ObjectCount = 0x801C7584;
    const int ObjectSlots = (int)((ObjectIndices - 0x801C2230) / 0x58);

    static readonly CarriedList[] Lists =
    [
        new("entities", 0x800241D0, 0x80026D4C, 0x801BA138, 0xD4, 0x6100 / 0xD4, 0x74, 0x8C, 0x2A),
        new("objects", 0x800246E4, 0x80026D54, 0x801C2230, 0x58, ObjectSlots, 0x24, 0x34, 0x10, ListedObjects),
    ];

    static readonly ModInfo _self = new() { Id = "st.modelsmoothing", Name = "Model smoothing", Version = "1.0" };
    static readonly RecordCarry[] _carries = Lists.Select(l => new RecordCarry(l)).ToArray();

    static bool _enabled = true, _probe;
    static RecordCarry? _active;
    static bool _probing;
    static double _probeAt = -1.0;
    static long _frames, _distinct;
    static uint _cursorBefore;
    static ulong _lastHash;

    /// <summary>The switch, live (Settings ▸ Testing).</summary>
    public static bool Enabled { get => _enabled; set => _enabled = value; }

    public static void Configure(string? smooth, string? models, string? probe)
    {
        _enabled = smooth?.Trim() is not ("0" or "off") && models?.Trim() is not ("0" or "off");
        _probe = probe?.Trim() == "1";
    }

    public static void Install()
    {
        Event.AddListener<OverlayLoadedEvent>(_ => { foreach (var c in _carries) c.Reprime(); });
        HookAttach.OnOverlayLoad("model smoothing", Attach);
        Console.WriteLine($"[ST] model smoothing: {(_enabled ? "on" : "off")}");
    }

    static bool Attach()
    {
        SymbolRegistry.Build();
        var self = typeof(ModelSmoothing);
        var before = self.GetMethod(nameof(Before), BindingFlags.Public | BindingFlags.Static)!;
        var after = self.GetMethod(nameof(After), BindingFlags.Public | BindingFlags.Static)!;
        var targets = new List<MethodInfo>();
        foreach (var list in Lists)
        {
            if (SymbolRegistry.Resolve(MainLoop.Overlay, null, list.Routine) is not { } t) return false;
            HookManager.AddPre(_self, t, before);
            HookManager.AddPost(_self, t, after);
            targets.Add(t);
        }
        HookManager.Commit();
        return targets.All(HookAttach.Installed);
    }

    public static void Before(CpuContext c, IMemory m)
    {
        _active = null;
        _probing = false;
        if (!FramePacing.InMainLoopDraw) return;
        var carry = Array.Find(_carries, k => k.List.DrawSite == c.RA);
        if (carry is null) return;
        if (_probe && carry == _carries[0]) { _cursorBefore = Cursor(m); _probing = true; }   // with smoothing off too
        if (!_enabled || !FramePacing.Enabled) { carry.Reprime(); return; }
        carry.Carry(m, FramePacing.Ticks, FramePacing.IterationTicked, FramePacing.TickFraction);
        _active = carry;
    }

    public static void After(CpuContext c, IMemory m)
    {
        _active?.Restore(m);
        _active = null;
        if (_probing) { _probing = false; Probe(m); }
    }

    // The objects in use this frame: the drawer walks these indices from the last down.
    static IEnumerable<int> ListedObjects(IMemory m)
    {
        int count = m.ReadU8(ObjectCount);
        for (int k = 0; k < count && k < 0x60; k++) yield return m.ReadU16(ObjectIndices + (uint)(2 * k));
    }

    // The packet cursor of the frame being built: the record 0x80165394 points at, +8.
    static uint Cursor(IMemory m) => m.ReadU32(m.ReadU32(0x80165394) + 8);

    // What the entity list wrote this frame, hashed: a new hash on every frame means the
    // models are drawn differently on every frame (with the eye still, their poses).
    static ulong PacketHash(IMemory m, uint from, uint to)
    {
        ulong h = 1469598103934665603UL;
        if (to < from || to - from > 0x20000) return 0;
        for (uint a = from; a < to; a += 4) h = (h ^ m.ReadU32(a)) * 1099511628211UL;
        return h;
    }

    static void Probe(IMemory m)
    {
        _frames++;
        ulong hash = PacketHash(m, _cursorBefore, Cursor(m));
        if (hash != _lastHash) _distinct++;
        _lastHash = hash;
        double now = Environment.TickCount64 / 1000.0;
        if (_probeAt < 0.0) _probeAt = now;
        double dt = now - _probeAt;
        if (dt < 1.0) return;
        var lists = string.Join(", ", _carries.Select(k => $"{k.Carried / dt:0.0} {k.List.Name} carried/s, {k.Snaps} snap(s)"));
        Console.WriteLine($"[ST] model smoothing: {_frames / dt:0.0} frame(s)/s, {lists}, {_distinct / dt:0.0} distinct entity packets/s");
        Console.Out.Flush();
        _probeAt = now;
        _frames = _distinct = 0;
        foreach (var k in _carries) k.Carried = k.Snaps = 0;
    }
}
