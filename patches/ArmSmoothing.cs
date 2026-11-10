using System.Reflection;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Events;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// The arm's swing carried between world ticks. <c>func_80021AC4(hand)</c>, called by the
/// draw stage for hands 0 and 1, poses the arm from that hand's swing time, the s16 at
/// <c>player+0x130 + 2 * hand</c> (<c>-1</c> at rest), with the clip the byte at
/// <c>player+0x150 + hand</c> names. The time steps 0x114 a tick, so the arm's pose
/// stepped at 20 Hz. Around each of the draw stage's two calls the time is replaced by
/// the one interpolated between the last two ticks', and the tick's put back after it.
///
/// A swing's first tick (from rest), its end (back to rest), a change of clip and a step
/// over <see cref="SnapTime"/> are not interpolated. On with the models
/// (<see cref="ModelSmoothing.Enabled"/>, <c>ST_SMOOTH_MODELS</c>); <c>ST_SMOOTH_PROBE=1</c>
/// adds a line a second. See "The arm" in docs/SMOOTHING.md.
/// </summary>
public static class ArmSmoothing
{
    const uint Routine = 0x80021AC4;
    const uint SwingTimes = 0x80199058;   // player+0x130: an s16 a hand, -1 at rest
    const uint Clips = 0x80199078;        // player+0x150: a byte a hand
    const short AtRest = -1;
    const int SnapTime = 0x800;
    static readonly uint[] Sites = [0x80026D1C, 0x80026D24];   // the draw stage's calls, hand 0 and 1, + 8

    readonly record struct Swing(short Time, byte Clip)
    {
        public bool Swinging => Time != AtRest;
    }

    sealed class Hand
    {
        public Swing Prev, Cur, Handed;
        public long Tick = -1;
        public bool Primed, Restore;
    }

    static readonly ModInfo _self = new() { Id = "st.armsmoothing", Name = "Arm smoothing", Version = "1.0" };
    static readonly Hand[] _hands = [new(), new()];
    static readonly bool _probe = Verdite.Core.Game.Env("SMOOTH_PROBE")?.Trim() == "1";
    static long _swingFrames, _carried, _distinct, _snaps;
    static short _lastDrawn = AtRest;
    static double _probeAt = -1.0;

    static bool Active => ModelSmoothing.Enabled && FramePacing.Enabled;

    public static void Install()
    {
        Event.AddListener<OverlayLoadedEvent>(_ => { foreach (var h in _hands) h.Primed = false; });
        HookAttach.OnOverlayLoad("arm smoothing", Attach);
    }

    static bool Attach()
    {
        SymbolRegistry.Build();
        if (SymbolRegistry.Resolve(MainLoop.Overlay, null, Routine) is not { } target) return false;
        MethodInfo Hook(string n) => typeof(ArmSmoothing).GetMethod(n, BindingFlags.Public | BindingFlags.Static)!;
        HookManager.AddPre(_self, target, Hook(nameof(Before)));
        HookManager.AddPost(_self, target, Hook(nameof(After)));
        HookManager.Commit();
        return HookAttach.Installed(target);
    }

    public static void Before(CpuContext c, IMemory m)
    {
        int index = Array.IndexOf(Sites, c.RA);
        if (index < 0 || !FramePacing.InMainLoopDraw) return;
        var hand = _hands[index];
        hand.Restore = false;
        if (Active) Carry(m, index, hand);
        else hand.Primed = false;
        if (_probe && index == 0) Probe(m, hand.Restore);   // with smoothing off too, to compare
    }

    static void Carry(IMemory m, int index, Hand hand)
    {
        hand.Handed = Read(m, index);
        Sample(hand);
        if (!CanCarry(hand.Prev, hand.Cur, hand.Handed)) return;
        Write(m, index, Mix(hand.Prev.Time, hand.Cur.Time, FramePacing.TickFraction));
        hand.Restore = true;
    }

    public static void After(CpuContext c, IMemory m)
    {
        int index = Array.IndexOf(Sites, c.RA);
        if (index < 0 || !_hands[index].Restore) return;
        _hands[index].Restore = false;
        Write(m, index, _hands[index].Handed.Time);
    }

    /// <summary>Once a frame a hand: the last two ticks' swings.</summary>
    static void Sample(Hand hand)
    {
        long tick = FramePacing.Ticks;
        if (!hand.Primed)
        {
            hand.Prev = hand.Cur = hand.Handed;
            hand.Primed = true;
            hand.Tick = tick;
        }
        else if (tick != hand.Tick && FramePacing.IterationTicked)
        {
            hand.Tick = tick;
            hand.Prev = hand.Cur;
            hand.Cur = hand.Handed;
            if (hand.Prev.Swinging && hand.Cur.Swinging && !CanCarry(hand.Prev, hand.Cur, hand.Cur)) _snaps++;
        }
        else if (hand.Handed != hand.Cur)
        {
            // Changed without a tick of the world: no pair to carry.
            hand.Prev = hand.Cur = hand.Handed;
        }
    }

    static bool CanCarry(in Swing prev, in Swing cur, in Swing handed) =>
        handed == cur && prev.Swinging && cur.Swinging && prev.Clip == cur.Clip &&
        prev.Time != cur.Time && Math.Abs(cur.Time - prev.Time) <= SnapTime;

    static Swing Read(IMemory m, int hand) =>
        new((short)m.ReadU16(SwingTimes + 2u * (uint)hand), m.ReadU8(Clips + (uint)hand));

    static void Write(IMemory m, int hand, short time) => m.WriteU16(SwingTimes + 2u * (uint)hand, (ushort)time);

    static short Mix(short a, short b, double t) => (short)(a + Math.Round((b - a) * t));

    // ST_SMOOTH_PROBE=1: hand 0's frames drawn mid-swing, those carried, those whose time
    // differed from the frame before, and swing steps snapped.
    static void Probe(IMemory m, bool carried)
    {
        short drawn = (short)m.ReadU16(SwingTimes);
        if (drawn != AtRest)
        {
            _swingFrames++;
            if (carried) _carried++;
            if (drawn != _lastDrawn) _distinct++;
        }
        _lastDrawn = drawn;
        double now = Environment.TickCount64 / 1000.0;
        if (_probeAt < 0.0) _probeAt = now;
        double dt = now - _probeAt;
        if (dt < 1.0) return;
        if (_swingFrames > 0)
        {
            Console.WriteLine($"[ST] arm smoothing: {_swingFrames} swing frame(s), {_carried} carried, " +
                              $"{_distinct} with a new time, {_snaps} snap(s), over {dt:0.0} s");
            Console.Out.Flush();
        }
        _probeAt = now;
        _swingFrames = _carried = _distinct = _snaps = 0;
    }
}
