using RecompOne.Runtime.Memory;
using Verdite.Core;

namespace St;

/// <summary>
/// Instant mouse look: the mouse leads the tick. The look routine spends the mouse
/// once a world tick, and the carried view reaches that turn only at the next tick,
/// so a turn showed up to two ticks after the hand. Each drawn frame instead drains
/// the host's motion (<c>Mouse.Poll</c>), replaces the lerp's share of the last tick's
/// mouse turn with all of it, and adds the motion not yet spent on top. A pad turn
/// keeps the carried view, since only the mouse's own share is led.
///
/// Verdite3's lead (its ViewSmoothing, from Verdite2's FrameSmoothing.MouseLead) on
/// this game's camera, whose pitch and yaw are the player's <c>+0x278</c> and
/// <c>+0x27A</c>. See "Instant mouse look" in docs/INPUT.md.
/// </summary>
public static class MouseLead
{
    static int _tickYaw, _tickPitch;

    /// <summary>The setting, under pacing; with carrying off the view still leads.</summary>
    public static bool Leading => Mouse.Lead && FramePacing.Enabled;

    /// <summary>A tick was taken into the carried pair: measure what the game turned
    /// by for the mouse on it, off the base angles, so the pitch limit is in it.</summary>
    public static void Ticked(IMemory m)
    {
        var spent = Mouse.SpentThisFrame(m);
        (_tickYaw, _tickPitch) = spent is { } t ? (t.Yaw, t.Pitch) : (0, 0);
    }

    /// <summary>The carried pair snapped (a cut): there is no lerp share to replace.</summary>
    public static void Snapped() => _tickYaw = _tickPitch = 0;

    /// <summary>The view with the mouse's lead added, or null when there is none.
    /// <paramref name="frac"/> is the lerp's phase; <paramref name="view"/> is carried
    /// when <paramref name="carried"/>, else the tick's own.</summary>
    public static Camera? Lead(IMemory m, Camera view, double frac, bool carried)
    {
        Mouse.Poll();
        var (turn, look) = MouseLook.SynthPending(frac) ?? Mouse.Pending;

        double keep = carried ? 1.0 - frac : 0.0;
        double yaw = _tickYaw * keep + turn;

        // Held inside the game's pitch limit, off the base angle the next tick adds
        // to, so looking into the limit stops at it instead of overshooting.
        int basePitch = S12(m.ReadU16(Mouse.PitchAddress));
        int ahead = Math.Clamp(basePitch + (int)Math.Round(look), -Mouse.PitchLimit, Mouse.PitchLimit) - basePitch;
        double pitch = _tickPitch * keep + ahead;

        int dy = (int)Math.Round(yaw), dp = (int)Math.Round(pitch);
        if (dy == 0 && dp == 0) return carried ? view : null;
        return view with { Yaw = Add12(view.Yaw, dy), Pitch = Add12(view.Pitch, dp) };
    }

    static int S12(ushort a) => ((a + 0x800) & 0xFFF) - 0x800;

    static short Add12(short a, int d) => (uint)a < 0x1000u ? (short)((a + d) & 0xFFF) : (short)(a + d);
}
