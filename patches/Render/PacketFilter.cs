using RecompOne.Runtime.Memory;

namespace St;

/// <summary>
/// Takes the packets one call of a writer linked into the ordering table back off it,
/// keeping those a test still wants drawn, in the order they were linked. The writer
/// runs as the game wrote it, so every other effect (the scratchpad, the GTE, its
/// counters) is the game's; only what the table walk will draw changes. Each table
/// entry the call changed is walked from its new head to the head it had before: the
/// packets between are the call's, newest first. A filter is opened on entry and
/// closed after; filters may not nest. The table is the scratchpad's <c>+0</c>, 0x1000
/// entries, the packet cursor its <c>+0x10</c>. See "The retained map" in
/// docs/GPU_RENDERER.md.
/// </summary>
sealed class PacketFilter
{
    const uint Scratch = 0x1F800000;
    const int Entries = 0x1000;

    readonly uint[] _heads = new uint[Entries];
    readonly List<uint> _call = [];
    uint _table, _cursor;
    bool _open;

    /// <summary>Packets taken off and kept, over every call.</summary>
    public long Removed, Kept;

    public void Open(IMemory m)
    {
        _table = m.ReadU32(Scratch);
        _cursor = m.ReadU32(Scratch + 0x10);
        for (int i = 0; i < Entries; i++) _heads[i] = m.ReadU32(_table + (uint)i * 4) & 0xFFFFFF;
        _open = true;
    }

    /// <summary>Takes the call's packets off, but those <paramref name="keep"/> wants; the
    /// cursor goes back to where it was when nothing is kept.</summary>
    public void Close(IMemory m, Func<IMemory, uint, bool>? keep = null)
    {
        if (!_open) return;
        _open = false;
        if (m.ReadU32(Scratch) != _table) return;
        int kept = 0;
        for (int i = 0; i < Entries; i++)
        {
            uint entry = _table + (uint)i * 4, word = m.ReadU32(entry);
            if ((word & 0xFFFFFF) == _heads[i]) continue;
            _call.Clear();
            for (uint p = word & 0xFFFFFF, guard = 0; p != _heads[i] && guard < 0x10000; guard++)
            {
                uint at = 0x80000000u | p;
                _call.Add(at);
                p = m.ReadU32(at) & 0xFFFFFF;
            }
            uint head = (word & 0xFF000000u) | _heads[i];
            // Linked oldest first, so the newest ends at the head as before.
            for (int k = _call.Count - 1; k >= 0; k--)
            {
                uint p = _call[k];
                if (keep == null || !keep(m, p)) { Removed++; continue; }
                m.WriteU32(p, (m.ReadU32(p) & 0xFF000000u) | (head & 0xFFFFFF));
                head = (head & 0xFF000000u) | (p & 0xFFFFFF);
                kept++;
            }
            m.WriteU32(entry, head);
        }
        Kept += kept;
        if (kept == 0) m.WriteU32(Scratch + 0x10, _cursor);
    }

    /// <summary>A packet's GP0 code: the first command byte after the tag.</summary>
    public static byte Code(IMemory m, uint packet) => (byte)(m.ReadU32(packet + 4) >> 24);

    /// <summary>The semi-transparency bit of a polygon's code.</summary>
    public static bool Semi(IMemory m, uint packet) => (Code(m, packet) & 0xE2) == 0x22;
}
