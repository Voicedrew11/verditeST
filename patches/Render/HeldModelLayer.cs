using System.Reflection;
using RecompOne.Runtime;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// The held models drawn over the world, as the console draws them. <c>func_80021AC4(hand)</c>
/// draws the arm while that hand's swing time (<c>player+0x130 + 2 * hand</c>) is not -1,
/// into the 0x40-entry front table, which <c>func_80017634</c> links ahead of the main
/// table's entry 0: drawn last, with no depth test. Here its corners would otherwise have
/// the address map's view depths, so the Z-buffer let nearer floor and walls cover it, and
/// the retained world drew it as an instance in the world's own depth.
///
/// So while the routine runs, its parts are not published to the retained world
/// (<see cref="Drawing"/>, read by <see cref="RetainedModels"/>) and keep their packets; and
/// after it, the words of the packets it wrote are dropped from the address map, so they
/// keep painter's order. See "The arm" in docs/SMOOTHING.md and "The held models" in
/// docs/GPU_RETAINED.md.
/// </summary>
static class HeldModelLayer
{
    const uint Routine = 0x80021AC4;
    const uint PacketRecord = 0x80165394;   // -> the frame's packet buffer { start, end, cursor }

    static readonly ModInfo _self = new() { Id = "st.heldmodellayer", Name = "Held model layer", Version = "1.0" };
    static uint _cursorBefore;

    /// <summary>True inside <c>func_80021AC4</c>: its parts stay packets.</summary>
    public static bool Drawing { get; private set; }

    /// <summary>Packet bytes the held models wrote and the address map forgot, ever.</summary>
    public static long BytesForgotten { get; private set; }

    /// <summary><c>ST_HELDLAYER=0</c> leaves the arm to the depth test, for comparison.</summary>
    public static void Install(string? spec)
    {
        if (spec?.Trim() == "0") return;
        HookAttach.OnOverlayLoad("held model layer", Attach);
    }

    static bool Attach()
    {
        SymbolRegistry.Build();
        if (SymbolRegistry.Resolve(MainLoop.Overlay, null, Routine) is not { } target) return false;
        MethodInfo Hook(string n) => typeof(HeldModelLayer).GetMethod(n, BindingFlags.Public | BindingFlags.Static)!;
        HookManager.AddPre(_self, target, Hook(nameof(Before)), order: int.MinValue);
        HookManager.AddPost(_self, target, Hook(nameof(After)), order: int.MaxValue);
        HookManager.Commit();
        return HookAttach.Installed(target);
    }

    public static void Before(CpuContext c, IMemory m)
    {
        Drawing = true;
        _cursorBefore = Cursor(m);
    }

    public static void After(CpuContext c, IMemory m)
    {
        Drawing = false;
        uint cursorAfter = Cursor(m);
        if (cursorAfter <= _cursorBefore || !GteVertexMap.Active) return;
        GteVertexMap.NoteStores(_cursorBefore, cursorAfter - 4, 0);
        BytesForgotten += cursorAfter - _cursorBefore;
    }

    static uint Cursor(IMemory m) => m.ReadU32(m.ReadU32(PacketRecord) + 8);
}
