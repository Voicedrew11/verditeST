using System.Reflection;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// The near polygons' screen widened to the widescreen aspect. A polygon large on
/// screen is subdivided by libgte's division, <c>func_80075EC4</c> (triangles, from
/// <c>MapTriangles</c> and <c>SceneStep1E5A0</c>) or <c>func_80076884</c> (quads, from
/// <c>MapQuads</c> and <c>SceneStep1EEAC</c>), which drops the pieces past
/// <c>OFX ± w/2</c>, <c>w</c> the u32 at <c>a1 + 4</c> of a block the writer fills on its
/// stack for every face with an immediate 320. A pre-hook on each entry rewrites that
/// word to the widened width, so the margin gets the near pieces too.
///
///     ST_NEARSCREEN=0        leave the block at 320
///
/// Nothing is written at 4:3, and the block is rebuilt for the next face, so there is
/// nothing to put back. Verdite3's NearScreen on this disc's two entries. See "The
/// near polygons" in docs/WIDESCREEN.md.
/// </summary>
public static class NearScreen
{
    static readonly uint[] Entries = [0x80075EC4, 0x80076884];
    const uint StockWidth = 320u;

    public static bool Enabled { get; private set; } = true;

    /// <summary>Faces handed to a division, and those whose width was widened.</summary>
    public static long Faces, Widened;

    static readonly ModInfo _self = new() { Id = "st.nearscreen", Name = "Near screen", Version = "1.0" };
    static bool _queued;

    public static void Configure(string? on) => Enabled = on?.Trim() is not ("0" or "off");

    public static void Install() => HookAttach.OnOverlayLoad("near screen", () =>
    {
        SymbolRegistry.Build();
        var targets = Entries.Select(a => SymbolRegistry.Resolve(MainLoop.Overlay, null, a)).ToArray();
        if (targets.Any(t => t == null)) return false;
        if (!_queued)
        {
            var pre = typeof(NearScreen).GetMethod(nameof(Before), BindingFlags.Public | BindingFlags.Static)!;
            foreach (var t in targets) HookManager.AddPre(_self, t!, pre);
            _queued = true;
        }
        HookManager.Commit();
        return targets.All(t => HookAttach.Installed(t!));
    });

    public static void Before(CpuContext c, IMemory m)
    {
        Faces++;
        if (!Enabled || !Widescreen.On || c.A1 < 0x80000000u) return;
        uint at = c.A1 + 4u;
        if (m.ReadU32(at) != StockWidth) return;
        m.WriteU32(at, StockWidth + 2u * (uint)Widescreen.Margin);
        Widened++;
    }
}
