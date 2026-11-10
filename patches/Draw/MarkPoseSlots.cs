using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_80034FC4</c>: marks the frame start for 16 slots of 0x3C bytes at 0x8016780C: a slot with any flag but bit 3 set gets flags 9; the pose state func_80035008 frees (flag 8), probably. <c>ST_MARKPOSESLOTS</c>.
/// </summary>
public static class MarkPoseSlots
{
    public static readonly Literal R = new(typeof(MarkPoseSlots), "mark pose slots", "MARKPOSESLOTS", 0x80034FC4, Run,
        "marks the frame start for 16 slots of 0x3C bytes at 0x8016780C: a slot with any flag but bit 3 set gets flags 9; the pose state func_80035008 frees (flag 8), probably.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.V0 = 0x80160000u;
        c.V1 = c.V0 + 0x780Cu;
        c.A0 = 0x00000010u;
        c.A2 = 0xFFFFFFF7u;
        c.A1 = 0x00000009u;
        L80034FD8: ;
        Poll(c, mem);
        c.V0 = mem.ReadU16(c.V1);
        c.V0 = c.V0 & c.A2;
        if (c.V0 == 0u) {
            c.V0 = c.A0 - 0x1u;
            goto L80034FF0;
        }
        c.V0 = c.A0 - 0x1u;
        mem.WriteU16(c.V1, (ushort)c.A1);
        L80034FF0: ;
        c.A0 = c.V0;
        c.V0 = c.V0 & 0xFFFFu;
        if (c.V0 != 0u) {
            c.V1 = c.V1 + 0x3Cu;
            goto L80034FD8;
        }
        c.V1 = c.V1 + 0x3Cu;
        return;
    }
}
