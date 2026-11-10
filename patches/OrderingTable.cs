using RecompOne.Runtime.Memory;

namespace St;

/// <summary>
/// ST.EXE's ordering table, read by the probes: 0x1000 entries at the word
/// <c>0x80165174</c> points to, drawn from its last entry down (<c>ClearOTagR</c>
/// order). See "The draw path" in docs/GAME_INTERNALS.md.
/// </summary>
static class OrderingTable
{
    /// <summary>Every packet hanging from the table, from its last entry down the chain
    /// to the terminator, with its tag word. Table entries themselves are skipped.</summary>
    public static void EachPacket(IMemory m, int maxPackets, Action<uint, uint> each)
    {
        uint ot = m.ReadU32(MainLoop.OtPointer);
        if (ot < 0x80000000u || ot - 0x80000000u + MainLoop.OtWords * 4u > 0x200000u) return;
        uint lo = ot, hi = ot + MainLoop.OtWords * 4u;
        uint p = hi - 4u;
        for (int guard = 0; guard < maxPackets; guard++)
        {
            uint tag = m.ReadU32(p);
            if (p < lo || p >= hi) each(p, tag);
            uint next = tag & 0xFFFFFFu;
            if (next == 0xFFFFFFu) break;
            p = 0x80000000u | next;
        }
    }

    /// <summary>A packet's kind from its first command byte. Polygons by F/G, T, 3/4
    /// corners, S for semi-transparent textured ones.</summary>
    public static string Kind(uint code) => code switch
    {
        >= 0x20 and <= 0x3F => $"{((code & 0x10) != 0 ? "G" : "F")}{((code & 0x04) != 0 ? "T" : "")}" +
                               $"{((code & 0x08) != 0 ? "4" : "3")}{((code & 0x04) != 0 && (code & 0x02) != 0 ? "S" : "")}",
        >= 0x40 and <= 0x5F => "line",
        >= 0x60 and <= 0x7F => "sprite",
        0x02 => "fill",
        >= 0xE1 and <= 0xE6 => "E1-6",
        0x00 => "nop",
        _ => $"other {code:X2}",
    };
}
