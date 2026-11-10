using System.Reflection;
using ImGuiNET;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;
using Verdite.Core;

namespace St;

/// <summary>
/// The mouse spent through the game's own turn and look routine, <c>func_80041D4C</c>
/// (once a world tick). Each axis has three branches: a held button adds an
/// acceleration to a velocity and clamps it, no button decays it by the same
/// acceleration toward 0 and applies what is left, unclamped; then the base angle
/// takes <c>(angle + vel) &amp; 0xFFF</c>. So writing <c>step ± accel</c> into the
/// velocity with the axis's buttons masked out lands on <c>step</c> exactly, through
/// the game's own pitch limit.
///
///     yaw    vel player+0x2CA, accel (s32 player+0x2E8) >> 2, base player+0x27A
///     pitch  vel player+0x2CC, accel 3, limit 0x2BC / 0xD44, base player+0x278
///
/// The buttons are the pad block's table entries (Left, Right for yaw; L2, R2 for
/// pitch, both masked since the pair recentres), read each tick because the options
/// rewrite the table. The held word is put back after the call, and a tick the mouse
/// drove ends with that axis's velocity at 0, so the view does not coast on the
/// game's decay when the hand stops. Verdite3's MouseLook on this game's routine.
///
///     ST_MOUSE_SYNTH=yaw,pitch   a synthetic hand: that step a tick, 40 ticks of every 60
///     ST_MOUSE_PROBE=1           a line a second: ticks driven, landed on the step, coasting
///
/// See "Mouse look" in docs/INPUT.md.
/// </summary>
public static class MouseLook
{
    const uint Player = 0x80198F28;
    const uint Yaw = Player + 0x27A, Pitch = Player + 0x278;
    const uint YawVel = Player + 0x2CA, PitchVel = Player + 0x2CC, TurnRate = Player + 0x2E8;
    const int PitchAccel = 3;

    const uint PadBlock = 0x801CD180;
    const uint Held = PadBlock + 0x02;
    const uint LeftEntry = PadBlock + 0x26, RightEntry = PadBlock + 0x28;
    const uint L2Entry = PadBlock + 0x2E, R2Entry = PadBlock + 0x30;

    const uint Routine = 0x80041D4C;

    /// <summary>This game's values for Verdite.Core.Mouse.</summary>
    public static readonly MouseGame Game = new(
        UnitsPerDegree: 4096f / 360f,   // 12 bits to a turn
        DegreesPerPixel: 0.15f,         // a quarter turn is about 600 px at sensitivity 1
        StepCap: 1024,                  // the most one tick may turn, a quarter
        PitchLimit: 0x2BC,              // func_80041D4C's, 0x2BC and 0xD44 at 12 bits
        YawAddress: Yaw,
        PitchAddress: Pitch,
        DefaultLeftButton: 3,           // Square: a hand (docs/INPUT.md)
        DefaultRightButton: 4,          // Triangle: the other hand
        DefaultMiddleButton: 1,         // Cross: the action button
        TextEditing: () => ImGui.GetCurrentContext() != nint.Zero && ImGui.GetIO().WantTextInput,
        Frames: () => FramePacing.Frames,
        LogicHz: () => FramePacing.LogicHz,
        DefaultCaptureKey: Silk.NET.Input.Key.Unknown);   // a click captures

    static readonly ModInfo _self = new() { Id = "st.mouselook", Name = "Mouse look", Version = "1.0" };

    static bool _queued;
    static float _fracTurn, _fracPitch;
    static int _drovePitch, _droveYaw;
    static (int Yaw, int Pitch)? _synth;
    static bool _probe;

    public static void Configure(string? synth, string? probe)
    {
        _probe = probe?.Trim() == "1";
        var parts = synth?.Split(',', StringSplitOptions.TrimEntries);
        if (parts is { Length: 2 } && int.TryParse(parts[0], out int y) && int.TryParse(parts[1], out int p))
            _synth = (y, p);
    }

    public static void Install() => HookAttach.OnOverlayLoad("mouse look", Attach);

    static bool Attach()
    {
        SymbolRegistry.Build();
        if (SymbolRegistry.Resolve(MainLoop.Overlay, null, Routine) is not { } target) return false;
        if (!_queued)
        {
            var impl = typeof(MouseLook).GetMethod(nameof(Replace), BindingFlags.NonPublic | BindingFlags.Static)!;
            _queued = HookManager.AddReplace(_self, target, impl);
            if (!_queued) return false;
        }
        HookManager.Commit();
        bool ok = HookAttach.Installed(target);
        Console.WriteLine(ok ? $"[ST] mouse look: attached{(_synth is { } s ? $", synthetic hand {s.Yaw},{s.Pitch} a tick" : "")}"
                             : "[ST] mouse look: not installed");
        return ok;
    }

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m)
    {
        var (turn, pitch) = Mouse.TakeLook();
        bool driven = Mouse.Enabled && Mouse.Captured;
        if (_synth is { } s) { (turn, pitch, driven) = SynthStep(s); _synthTicks++; }
        if (!driven) _fracTurn = _fracPitch = 0f;

        int yaw = driven ? Whole(turn, ref _fracTurn) : 0;
        int look = driven ? Whole(pitch, ref _fracPitch) : 0;
        Mouse.NoteSpent(m, yaw, 0f, look, 0f);

        ushort pad = m.ReadU16(Held), held = pad;
        if (yaw != 0)
        {
            held &= (ushort)~(m.ReadU16(LeftEntry) | m.ReadU16(RightEntry));
            int accel = (int)m.ReadU32(TurnRate) >> 2;
            m.WriteU16(YawVel, (ushort)(yaw + Math.Sign(yaw) * accel));
        }
        if (look != 0)
        {
            held &= (ushort)~(m.ReadU16(L2Entry) | m.ReadU16(R2Entry));
            m.WriteU16(PitchVel, (ushort)(look + Math.Sign(look) * PitchAccel));
        }

        ushort yawBefore = m.ReadU16(Yaw), pitchBefore = m.ReadU16(Pitch);
        if (held != pad) m.WriteU16(Held, held);
        orig(c, m);
        if (held != pad) m.WriteU16(Held, pad);
        if (yaw != 0) m.WriteU16(YawVel, 0);
        if (look != 0) m.WriteU16(PitchVel, 0);
        _droveYaw = yaw;
        _drovePitch = look;

        if (_probe) MouseLookProbe.Tick(yaw, look, Wrap(m.ReadU16(Yaw) - yawBefore), Wrap(m.ReadU16(Pitch) - pitchBefore),
                                        m.ReadU16(Pitch));
    }

    /// <summary>
    /// Instant mouse look: the drawn view's yaw and pitch are the tick's (the camera's
    /// angles are the player's <c>+0x27A</c> and <c>+0x278</c>, read so on every tick) plus
    /// what the hand has moved since, so a turn shows on the next frame, not the next
    /// tick. Only on an axis the mouse is driving (it drove the last tick, or has moved
    /// since); a pad turn keeps the carried view. Pitch is held to the game's limit.
    /// The synthetic hand leads by its step times the tick's fraction.
    /// </summary>
    public static Camera Lead(in Camera carried, in Camera tick)
    {
        if (!Mouse.Lead) return carried;
        float turn, pitch;
        if (_synth is { } s) (turn, pitch) = SynthStep(s) is (var t, var p, _) && (t != 0f || p != 0f)
            ? (t * (float)FramePacing.TickFraction, p * (float)FramePacing.TickFraction) : (0f, 0f);
        else if (Mouse.Enabled && Mouse.Captured) (turn, pitch) = Mouse.Pending;
        else return carried;

        short yaw = carried.Yaw, look = carried.Pitch;
        if (_droveYaw != 0 || turn != 0f) yaw = (short)((tick.Yaw + (int)MathF.Round(turn)) & 0xFFF);
        if (_drovePitch != 0 || pitch != 0f)
            look = (short)(Math.Clamp(Wrap(tick.Pitch) + (int)MathF.Round(pitch), -PitchLimit, PitchLimit) & 0xFFF);
        return carried with { Yaw = yaw, Pitch = look };
    }

    const int PitchLimit = 0x2BC;

    /// <summary>The synthetic hand: its step for 40 ticks of every 60, nothing for 20.</summary>
    static (float, float, bool) SynthStep((int Yaw, int Pitch) s) =>
        _synthTicks % 60 >= 40 ? (0f, 0f, true) : (s.Yaw, s.Pitch, true);

    /// <summary>Ticks the synthetic hand has run, which schedules it (40 on, 20 off),
    /// so the lead between two ticks knows what the next one will apply.</summary>
    static long _synthTicks;

    /// <summary>The whole units of a step, carrying the fraction to the next tick,
    /// so a slow hand still turns.</summary>
    static int Whole(float step, ref float carry)
    {
        float v = step + carry;
        int whole = (int)MathF.Truncate(v);
        int capped = Math.Clamp(whole, -Mouse.StepCap, Mouse.StepCap);
        carry = capped == whole ? v - whole : 0f;
        return capped;
    }

    static int Wrap(int d) => ((d + 0x800) & 0xFFF) - 0x800;
}
