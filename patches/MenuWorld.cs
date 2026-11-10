using System.Reflection;
using RecompOne.Runtime;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;
using Recompiled;

namespace St;

/// <summary>
/// The world drawn live behind the in-game menu, instead of the 320-wide still of it.
/// As the menu opens, <c>func_80026E04</c> shrinks the packet buffers to <c>0xD400</c>
/// each and stores the frame (320 by 240) after them, at <c>0x80136C88</c>; every
/// menu frame <c>MenuBackdrop</c> (<c>func_8002948C</c>) puts it back with
/// <c>LoadImage</c>, after the swap's <c>VSync</c> and before its <c>DrawOTag</c>.
/// That paste is 320 wide and so never reaches the widescreen margin.
///
/// Here the paste is skipped and, in its place, stage 14's world half (the camera,
/// the slot marks, the scene, the visibility lists, the models) runs into an
/// ordering table of its own in the still's store, which nothing reads once the
/// paste is gone. Its front table is linked in as the swap does, its end is linked
/// to the head of the menu's table, and the menu's <c>DrawOTag</c> starts at it, so
/// one walk draws the world, then the menu over it. Skipped, as they move the
/// game's state: the texture animations, the effects (<c>rand</c>), the cels, the
/// tints and the HUD. The pass runs with interrupts held off, and every byte of game
/// memory it changed outside its own store is put back after it, as are the
/// scratchpad, the GTE and the registers, so the menu's world is drawn and the
/// game's state is the menu's. Verdite3's MenuWorld on this game's menu.
///
///     ST_MENUWORLD=0          the game's still
///     ST_MENUWORLD_PROBE=1    a line a second: passes, packet bytes, the bytes of
///                             game memory put back after a pass; the first pass's listed
///
/// See "The menus" in docs/WIDESCREEN.md.
/// </summary>
public static class MenuWorld
{
    const uint Stage14 = 0x80026AFC;
    const uint LoadImage = 0x80079D0C, DrawOTag = 0x8007A054;
    const uint BackdropPaste = 0x80029518;     // MenuBackdrop's LoadImage returns here
    const uint SwapDraw = 0x8001766C;          // func_80017634's DrawOTag returns here

    const uint Base = 0x80165090;
    const uint OtPointer = Base + 0xE4, FrontPointer = Base + 0x2E8, ActiveDescriptor = Base + 0x304;
    const uint Counters = Base + 0x31C, CounterWords = 12;   // 0x31C..0x348, zeroed by the frame head
    const uint Store = 0x80136C88, StoreEnd = 0x8015C488;
    const uint OtEntries = 0x1000, FrontEntries = 0x40;
    const uint Table = Store + 0x10, Front = Table + OtEntries * 4u, Arena = Front + FrontEntries * 4u;

    const uint Scratchpad = 0x1F800000, ScratchpadWords = 0x100;

    public static bool Enabled { get; set; } = true;

    /// <summary>A pass is running: what holds an idle frame's draw to the tick (the
    /// light's flicker) holds it too.</summary>
    public static bool Drawing { get; private set; }
    static bool _probe, _queued;

    // The world as the last main-loop stage 14 found it.
    static uint _eye, _angles;
    static bool _worldSeen;
    static readonly uint[] _worldPad = new uint[ScratchpadWords];
    static readonly Gte.State _worldGte = new();

    static uint _head;
    static long _passes, _windowPasses;
    static uint _peak;
    static double _windowAt = -1.0;

    static readonly ModInfo _self = new() { Id = "st.menuworld", Name = "Menu world", Version = "1.0" };

    public static void Configure(string? on, string? probe)
    {
        Enabled = on?.Trim() is not ("0" or "off");
        _probe = probe?.Trim() == "1";
    }

    public static void Install() => HookAttach.OnOverlayLoad("menu world", Attach);

    static bool Attach()
    {
        SymbolRegistry.Build();
        var fns = new[] { Stage14, LoadImage, DrawOTag }.Select(a => SymbolRegistry.Resolve(MainLoop.Overlay, null, a)).ToArray();
        if (fns.Any(f => f == null)) return false;
        MethodInfo M(string n) => typeof(MenuWorld).GetMethod(n, BindingFlags.Public | BindingFlags.Static)!;
        if (!_queued)
        {
            HookManager.AddPre(_self, fns[0]!, M(nameof(BeforeStage14)));
            HookManager.AddPre(_self, fns[1]!, M(nameof(BeforeLoadImage)));
            HookManager.AddPre(_self, fns[2]!, M(nameof(BeforeDrawOTag)));
            _queued = true;
        }
        HookManager.Commit();
        bool ok = fns.All(f => HookAttach.Installed(f!));
        Console.WriteLine($"[ST] menu world: {(ok ? Enabled ? "on" : "off (ST_MENUWORLD=0)" : "not installed")}");
        return ok;
    }

    /// <summary>The main loop's stage 14: what a pass needs of the world.</summary>
    public static void BeforeStage14(CpuContext c, IMemory m)
    {
        if (c.RA != MainLoop.Draw.ReturnAddress) return;
        _eye = c.A0;
        _angles = c.A1;
        for (uint i = 0; i < ScratchpadWords; i++) _worldPad[i] = m.ReadU32(Scratchpad + i * 4u);
        Gte.Save(_worldGte);
        _worldSeen = true;
    }

    /// <summary>The backdrop's paste: the world drawn instead, and the paste skipped.</summary>
    public static bool BeforeLoadImage(CpuContext c, IMemory m)
    {
        if (c.RA != BackdropPaste || !Enabled || !_worldSeen) return true;
        if (m is not PSMemory ram) return true;
        _head = DrawWorld(c, ram);
        c.V0 = 0;
        return false;
    }

    /// <summary>The menu's walk starts at the world's table, which ends in the menu's.</summary>
    public static bool BeforeDrawOTag(CpuContext c, IMemory m)
    {
        if (_head == 0 || c.RA != SwapDraw) return true;
        Terminate(m, Table, c.A0);
        c.A0 = _head;
        _head = 0;
        return true;
    }

    static uint DrawWorld(CpuContext c, PSMemory m)
    {
        var regs = c.Snapshot();
        var menuGte = new Gte.State();
        Gte.Save(menuGte);
        var pad = new uint[ScratchpadWords];
        for (uint i = 0; i < ScratchpadWords; i++) pad[i] = m.ReadU32(Scratchpad + i * 4u);

        // Interrupts held off for the pass, as EnterCriticalSection does, so every
        // byte that changes in it is the pass's; what came due runs after.
        c.A0 = 1;
        Interrupts.Syscall(c, m);
        bool wasEnabled = c.V0 != 0;
        m.Ram.CopyTo(_before);

        ClearTable(m, Table, OtEntries);
        ClearTable(m, Front, FrontEntries);
        m.WriteU32(Store, Arena);
        m.WriteU32(Store + 4u, StoreEnd);
        m.WriteU32(Store + 8u, Arena);
        m.WriteU32(ActiveDescriptor, Store);
        m.WriteU32(OtPointer, Table);
        m.WriteU32(FrontPointer, Front);
        for (uint i = 0; i < CounterWords; i++) m.WriteU32(Counters + i * 4u, 0u);
        for (uint i = 0; i < ScratchpadWords; i++) m.WriteU32(Scratchpad + i * 4u, _worldPad[i]);
        Gte.Load(_worldGte);
        Drawing = true;
        try
        {
            c.A0 = _eye; c.A1 = _angles;
            ShadowTower_st.func_800176A8(c, m);     // the camera
            ShadowTower_st.func_800340F8(c, m);     // MarkClipSlots
            ShadowTower_st.func_80034FC4(c, m);     // MarkPoseSlots
            c.A0 = 0; ShadowTower_st.func_80021AC4(c, m);
            c.A0 = 1; ShadowTower_st.func_80021AC4(c, m);
            ShadowTower_st.func_80020A80(c, m);     // the scene
            ShadowTower_st.func_8001C8E0(c, m);     // VisibleEntities
            ShadowTower_st.func_8001C9C8(c, m);     // VisibleObjects
            ShadowTower_st.func_8001CAB8(c, m);     // VisibleAreaRecords
            ShadowTower_st.func_800241D0(c, m);     // the models
            ShadowTower_st.func_800251C8(c, m);     // the second model list
            // The front table into entry 0 of the main one: AddPrims(table, front's
            // last entry, front), as func_80017634 does.
            uint first = Front + (FrontEntries - 1u) * 4u, entry = m.ReadU32(Table), tail = m.ReadU32(Front);
            m.WriteU32(Front, (tail & 0xFF000000u) | (entry & 0x00FFFFFFu));
            m.WriteU32(Table, (entry & 0xFF000000u) | (first & 0x00FFFFFFu));
        }
        finally
        {
            Drawing = false;
            uint used = m.ReadU32(Store + 8u) - Arena;
            if (used > _peak) _peak = used;
            _restored = PutBack(m, _before);
            for (uint i = 0; i < ScratchpadWords; i++) m.WriteU32(Scratchpad + i * 4u, pad[i]);
            Gte.Load(menuGte);
            if (wasEnabled) { c.A0 = 2; Interrupts.Syscall(c, m); }
            c.Restore(regs);
            _passes++;
            _windowPasses++;
            if (_probe) Report();
        }
        return Table + (OtEntries - 1u) * 4u;
    }

    static readonly byte[] _before = new byte[0x200000];
    static long _restored;
    static bool _listed;

    /// <summary>
    /// Every byte of game memory the pass changed, put back, but for its own table
    /// and packets in the store and the stack it ran on: the camera block, the clip
    /// and pose slots (the menu frees the pose slots; a pass claimed slot 0 and a
    /// creature's record pointed at it), the draw state, a saved stack pointer.
    /// </summary>
    static long PutBack(PSMemory m, byte[] before)
    {
        var now = m.Ram;
        long n = 0;
        for (int at = 0; at < before.Length; at += 8)
        {
            if (now.Slice(at, 8).SequenceEqual(before.AsSpan(at, 8))) continue;
            for (int i = at; i < at + 8; i++)
            {
                uint addr = 0x80000000u + (uint)i;
                if (now[i] == before[i] || Owned(addr)) continue;
                if (_probe && !_listed && n < 24) Console.WriteLine($"[ST] menu world: put back 0x{addr:X8} ({now[i]:X2} -> {before[i]:X2})");
                m.WriteU8(addr, before[i]);
                n++;
            }
        }
        _listed = true;
        return n;
    }

    static bool Owned(uint addr) =>
        addr < 0x80010000u || addr >= Store && addr < StoreEnd || addr >= 0x801F0000u;

    /// <summary><c>ClearOTagR</c>: each entry links to the one before it, entry 0 ends.</summary>
    static void ClearTable(IMemory m, uint table, uint entries)
    {
        m.WriteU32(table, 0x00FFFFFFu);
        for (uint i = 1; i < entries; i++) m.WriteU32(table + i * 4u, (table + (i - 1u) * 4u) & 0x00FFFFFFu);
    }

    /// <summary>The end of the walk from entry 0 (entry 0, or the last packet added
    /// there) pointed at <paramref name="next"/>.</summary>
    static void Terminate(IMemory m, uint table, uint next)
    {
        uint at = table;
        for (int guard = 0; guard < 0x10000; guard++)
        {
            uint tag = m.ReadU32(at);
            if ((tag & 0x00FFFFFFu) == 0x00FFFFFFu)
            {
                m.WriteU32(at, (tag & 0xFF000000u) | (next & 0x00FFFFFFu));
                return;
            }
            at = 0x80000000u | (tag & 0x00FFFFFFu);
        }
    }

    // ---- The probe ----

    static void Report()
    {
        double now = Environment.TickCount64 / 1000.0;
        if (_windowAt < 0.0) { _windowAt = now; return; }
        if (now - _windowAt < 1.0) return;
        Console.WriteLine($"[ST] menu world: {_windowPasses / (now - _windowAt):0.0} pass(es)/s, {_passes} in all, " +
                          $"packets peak 0x{_peak:X} of 0x{StoreEnd - Arena:X}, {_restored} byte(s) of game memory put back after the last");
        _windowAt = now;
        _windowPasses = 0;
    }
}
