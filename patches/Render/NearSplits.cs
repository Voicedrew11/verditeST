using RecompOne.Runtime.Memory;

namespace St;

/// <summary>
/// What libgte's division (<c>func_80075EC4</c>, <c>func_80076884</c>) makes of a near
/// face, measured from outside for <see cref="MapReference"/>: the division level the
/// writer hands it, the packets it writes and their kinds, whether they lie end to end
/// from the cursor, and whether every one is linked into the face's own key. The
/// division's arithmetic is not modelled; this is what a replacement must reproduce or
/// knowingly change. See "The near splits" in docs/GPU_RENDERER.md.
/// </summary>
static class NearSplits
{
    // The block the writer passes in a1: +0 the division level, +0x14 the face's entry
    // in the ordering table. The cursor comes in a0 and goes back in v0.
    static uint _level, _entry, _headBefore;

    public static void Before(IMemory m, uint block)
    {
        _level = m.ReadU32(block);
        _entry = m.ReadU32(block + 0x14);
        _headBefore = m.ReadU32(_entry) & 0xFFFFFF;
    }

    public static void After(IMemory m, string shape, uint start, uint end)
    {
        string at = $"{shape} level {_level}";
        int packets = 0;
        uint p = start;
        while (p < end && packets < 4096)
        {
            uint len = m.ReadU32(p) >> 24;
            MapReference.Count($"{at}: {OrderingTable.Kind(m.ReadU32(p + 4) >> 24)}");
            p += (len + 1) * 4;
            packets++;
        }
        MapReference.Count($"{at}: calls");
        MapReference.Count(packets == 0 ? $"{at}: clipped away whole" : $"{at}: {packets} pieces");
        MapReference.Agree("near splits: packets end to end from the cursor", p == end);

        // Every packet of the split, and nothing else, between the key's head and its old head.
        int chained = 0;
        uint link = m.ReadU32(_entry) & 0xFFFFFF;
        while (link != _headBefore && link >= (start & 0xFFFFFF) && link < (end & 0xFFFFFF) && chained <= packets)
        {
            chained++;
            link = m.ReadU32(0x80000000u | link) & 0xFFFFFF;
        }
        MapReference.Agree("near splits: every packet in the face's own key", link == _headBefore && chained == packets);
    }
}
