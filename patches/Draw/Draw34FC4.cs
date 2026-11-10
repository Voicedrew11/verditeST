using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_80034FC4</c>: a draw-stage call; a leaf; not read. <c>ST_DRAW34FC4</c>.
/// </summary>
public static class Draw34FC4
{
    public static readonly Literal R = new(typeof(Draw34FC4), "draw-34FC4", "DRAW34FC4", 0x80034FC4, Run,
        "a draw-stage call; a leaf; not read.");

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
