using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_800340F8</c>: marks the frame start for 16 slots of 0x18 bytes at 0x8016768C: a slot with any flag but bit 2 set gets flags 5; the pose code's, probably. <c>ST_MARKCLIPSLOTS</c>.
/// </summary>
public static class MarkClipSlots
{
    public static readonly Literal R = new(typeof(MarkClipSlots), "mark clip slots", "MARKCLIPSLOTS", 0x800340F8, Run,
        "marks the frame start for 16 slots of 0x18 bytes at 0x8016768C: a slot with any flag but bit 2 set gets flags 5; the pose code's, probably.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.V0 = 0x80160000u;
        c.V1 = c.V0 + 0x768Cu;
        c.A0 = 0x00000010u;
        c.A2 = 0xFFFFFFFBu;
        c.A1 = 0x00000005u;
        L8003410C: ;
        Poll(c, mem);
        c.V0 = mem.ReadU16(c.V1);
        c.V0 = c.V0 & c.A2;
        if (c.V0 == 0u) {
            c.V0 = c.A0 - 0x1u;
            goto L80034124;
        }
        c.V0 = c.A0 - 0x1u;
        mem.WriteU16(c.V1, (ushort)c.A1);
        L80034124: ;
        c.A0 = c.V0;
        c.V0 = c.V0 & 0xFFFFu;
        if (c.V0 != 0u) {
            c.V1 = c.V1 + 0x18u;
            goto L8003410C;
        }
        c.V1 = c.V1 + 0x18u;
        return;
    }
}
