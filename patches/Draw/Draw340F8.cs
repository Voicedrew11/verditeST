using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_800340F8</c>: a draw-stage call; a leaf; not read. <c>ST_DRAW340F8</c>.
/// </summary>
public static class Draw340F8
{
    public static readonly Literal R = new(typeof(Draw340F8), "draw-340F8", "DRAW340F8", 0x800340F8, Run,
        "a draw-stage call; a leaf; not read.");

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
