using System.Reflection;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// The area objects' effect timers held to the world's tick. <c>func_800246E4</c>, in
/// stage 14, fires each visible object's effect (a sound, a particle, <c>rand()</c>)
/// when the vblank count (<c>0x80088204</c>) passes the object's deadline (<c>+0x50</c>),
/// and sets the next deadline 6 vblanks a period (<c>+0x4E</c>) on, plus a random part.
/// On the console it looks once a tick, so a short period fires 20 times a second; at
/// 144 fps it would look on every frame and fire on every vblank, 60. On an idle frame
/// the routine is shown the vblank count of the tick's frame, and the vblanks that
/// pass during it are added back after. On whenever pacing is. See "Effect timers"
/// in docs/GAME_INTERNALS.md.
/// </summary>
public static class EffectTimers
{
    const uint Effects = 0x800246E4;
    const uint DrawStageSite = 0x80026D54;   // the draw stage's call, + 8
    const uint VblankCount = 0x80088204;

    static readonly ModInfo _self = new() { Id = "st.effecttimers", Name = "Effect timers held to the tick", Version = "1.0" };

    static uint _tickCount, _realCount;
    static bool _held;

    public static void Install() => HookAttach.OnOverlayLoad("effect timers", () =>
    {
        SymbolRegistry.Build();
        if (SymbolRegistry.Resolve(MainLoop.Overlay, null, Effects) is not { } t) return false;
        var self = typeof(EffectTimers);
        HookManager.AddPre(_self, t, self.GetMethod(nameof(Before), BindingFlags.Public | BindingFlags.Static)!);
        HookManager.AddPost(_self, t, self.GetMethod(nameof(After), BindingFlags.Public | BindingFlags.Static)!);
        HookManager.Commit();
        return HookAttach.Installed(t);
    });

    public static void Before(CpuContext c, IMemory m)
    {
        _held = false;
        if (c.RA != DrawStageSite) return;
        if (!FramePacing.DrawingIdleFrame) { _tickCount = m.ReadU32(VblankCount); return; }
        _realCount = m.ReadU32(VblankCount);
        m.WriteU32(VblankCount, _tickCount);
        _held = true;
    }

    public static void After(CpuContext c, IMemory m)
    {
        if (!_held) return;
        _held = false;
        m.WriteU32(VblankCount, _realCount + (m.ReadU32(VblankCount) - _tickCount));
    }
}
