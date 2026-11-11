using System.Reflection;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Events;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// The item held up to the eye when it is picked up, carried between steps.
/// <c>func_8005335C(item)</c> runs a loop of its own: 16 steps lifting the item from where
/// it lies to in front of the eye, a hold until a button, then 16 steps to the player
/// (taken) or back (left). Each step writes the item record's position (<c>+8</c>, three
/// s32) and angles (<c>+0x18</c>, three s16) and calls stage 14 to draw it, so it moved at
/// the swap's 20 Hz whatever the drawn rate.
///
/// Here the loop's frames are presented at the drawn rate and each step is repeated
/// (its counter <c>s0</c> put back after stage 14) until the world clock ticks, so the
/// pickup lasts the console's 0.8 s each way; every frame draws the item interpolated
/// between the last two steps, and the step's pose is put back after it. The status
/// flash after each draw (<c>func_8003C24C</c>, a tick's fade) runs on a step's first
/// frame only, and a repeat is an idle frame to <see cref="DrawStageTick"/>.
///
/// On with the models (<c>ST_SMOOTH_MODELS=0</c> compares); <c>ST_SMOOTH_PROBE=1</c> adds a
/// line as each pickup ends. See "The item picked up" in docs/SMOOTHING.md.
/// </summary>
public static class PickupSmoothing
{
    const uint Stage14 = 0x80026AFC, StatusFlash = 0x8003C24C;

    /// <summary>Stage 14's return in the lift and hold loop, and in the loop away; the
    /// status flash's returns after each.</summary>
    const uint LiftDraw = 0x80053894, AwayDraw = 0x80053A04;
    const uint LiftFlash = 0x8005389C, AwayFlash = 0x80053A0C;

    const uint Position = 0x08, Angles = 0x18;

    readonly record struct Pose(int X, int Y, int Z, short A, short B, short C);

    static readonly ModInfo _self = new() { Id = "st.pickupsmoothing", Name = "Pickup smoothing", Version = "1.0" };
    static readonly bool _probe = Verdite.Core.Game.Env("SMOOTH_PROBE")?.Trim() == "1";

    static uint _item;
    static Pose _prev, _cur;
    static long _lastFrame = -1;
    static bool _repeat, _restore;
    static long _steps, _frames, _carried;

    static bool Active => ModelSmoothing.Enabled && FramePacing.Enabled;

    public static void Install()
    {
        Event.AddListener<OverlayLoadedEvent>(_ => _lastFrame = -1);
        HookAttach.OnOverlayLoad("pickup smoothing", Attach);
    }

    static bool Attach()
    {
        SymbolRegistry.Build();
        if (SymbolRegistry.Resolve(MainLoop.Overlay, null, Stage14) is not { } draw) return false;
        if (SymbolRegistry.Resolve(MainLoop.Overlay, null, StatusFlash) is not { } flash) return false;
        MethodInfo Hook(string n) => typeof(PickupSmoothing).GetMethod(n, BindingFlags.Public | BindingFlags.Static)!;
        HookManager.AddPre(_self, draw, Hook(nameof(BeforeDraw)));
        HookManager.AddPost(_self, draw, Hook(nameof(AfterDraw)));
        HookManager.AddPre(_self, flash, Hook(nameof(BeforeFlash)));
        HookManager.Commit();
        return HookAttach.Installed(draw) && HookAttach.Installed(flash);
    }

    public static void BeforeDraw(CpuContext c, IMemory m)
    {
        if (c.RA is not (LiftDraw or AwayDraw) || !Active) return;
        uint item = c.S1;
        var handed = Read(m, item);
        bool continuing = item == _item && _lastFrame >= 0 && FramePacing.Frames - _lastFrame <= 1;
        if (!continuing) { _prev = _cur = handed; _repeat = false; _steps = _frames = _carried = 0; }
        else if (!_repeat) { _prev = _cur; _cur = handed; _steps++; }
        _item = item;

        FramePacing.InSteppedLoopDraw = true;
        FramePacing.SteppedLoopRepeat = _repeat;
        _frames++;
        if (_prev == _cur) return;
        Write(m, item, Lerp(_prev, _cur, FramePacing.TickFraction));
        _restore = true;
        _carried++;
    }

    /// <summary>The frame is presented: repeat the step unless the world clock ticked.</summary>
    public static void AfterDraw(CpuContext c, IMemory m)
    {
        if (!FramePacing.InSteppedLoopDraw || c.RA is not (LiftDraw or AwayDraw)) return;
        FramePacing.InSteppedLoopDraw = false;
        FramePacing.SteppedLoopRepeat = false;
        if (_restore) { Write(m, _item, _cur); _restore = false; }
        _lastFrame = FramePacing.Frames;
        _repeat = !FramePacing.TickedAtBoundary;
        // The lift loop holds at 0 by itself; otherwise the loop takes one from s0 next.
        if (_repeat && (c.RA == AwayDraw || (int)c.S0 > 0)) c.S0++;
        if (c.RA == AwayDraw && c.S0 == 0 && !_repeat) Report();   // the loop's last step
    }

    /// <summary>The status flash fades a tick's worth a call: not on a repeat.</summary>
    public static bool BeforeFlash(CpuContext c, IMemory m) =>
        !(c.RA is LiftFlash or AwayFlash) || !_repeat || _lastFrame != FramePacing.Frames;

    static Pose Read(IMemory m, uint r) => new(
        (int)m.ReadU32(r + Position), (int)m.ReadU32(r + Position + 4), (int)m.ReadU32(r + Position + 8),
        (short)m.ReadU16(r + Angles), (short)m.ReadU16(r + Angles + 2), (short)m.ReadU16(r + Angles + 4));

    static void Write(IMemory m, uint r, in Pose p)
    {
        m.WriteU32(r + Position, (uint)p.X); m.WriteU32(r + Position + 4, (uint)p.Y); m.WriteU32(r + Position + 8, (uint)p.Z);
        m.WriteU16(r + Angles, (ushort)p.A); m.WriteU16(r + Angles + 2, (ushort)p.B); m.WriteU16(r + Angles + 4, (ushort)p.C);
    }

    // The loop's own steps are linear in its counter, so a straight mix between two is exact.
    static Pose Lerp(in Pose a, in Pose b, double t) => new(
        Mix(a.X, b.X, t), Mix(a.Y, b.Y, t), Mix(a.Z, b.Z, t), (short)Mix(a.A, b.A, t), (short)Mix(a.B, b.B, t), (short)Mix(a.C, b.C, t));

    static int Mix(int a, int b, double t) => (int)(a + Math.Round(((long)b - a) * t));

    // ST_SMOOTH_PROBE=1: the pickup's steps, frames drawn, and frames carried.
    static void Report()
    {
        if (!_probe) return;
        Console.WriteLine($"[ST] pickup smoothing: {_steps} step(s), {_frames} frame(s), {_carried} carried");
        Console.Out.Flush();
    }
}
