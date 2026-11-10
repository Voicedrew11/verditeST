using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_8001CAB8</c>: a draw-stage call after the scene; calls func_8001C88C; not read. <c>ST_DRAW1CAB8</c>.
/// </summary>
public static class Draw1CAB8
{
    public static readonly Literal R = new(typeof(Draw1CAB8), "draw-1CAB8", "DRAW1CAB8", 0x8001CAB8, Run,
        "a draw-stage call after the scene; calls func_8001C88C; not read.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x28u;
        c.V0 = 0x801D0000u;
        mem.WriteU32((c.SP + 0x18u), c.S2);
        c.S2 = c.V0 - 0x51F8u;
        c.V1 = c.S2 - 0x3710u;
        mem.WriteU32((c.SP + 0x14u), c.S1);
        c.S1 = 0x0000003Fu;
        c.V0 = 0x801B0000u;
        mem.WriteU32((c.SP + 0x20u), c.S4);
        c.S4 = c.V0 + 0x5920u;
        mem.WriteU32((c.SP + 0x1Cu), c.S3);
        c.S3 = c.V1;
        mem.WriteU32((c.SP + 0x10u), c.S0);
        c.S0 = c.S2 + 0x2u;
        mem.WriteU32((c.SP + 0x24u), c.RA);
        mem.WriteU8((c.V1 + 0x6u), (byte)0u);
        L8001CAF8: ;
        Poll(c, mem);
        c.V1 = mem.ReadU16(c.S2);
        c.V0 = 0u | 0xFFFFu;
        if (c.V1 == c.V0) {
            goto L8001CB50;
        }
        c.V1 = mem.ReadU8((c.S0 + 0x2Au));
        c.V0 = mem.ReadU8((c.S4 + 0x9u));
        if (c.V1 != c.V0) {
            goto L8001CB50;
        }
        c.A0 = mem.ReadU16(c.S0);
        c.RA = 0x8001CB28u;
        ShadowTower_st.func_8001C88C(c, mem);
        if (c.V0 == 0u) {
            c.V1 = 0x0000003Fu;
            goto L8001CB50;
        }
        c.V1 = 0x0000003Fu;
        c.V0 = mem.ReadU8((c.S3 + 0x6u));
        c.V1 = c.V1 - c.S1;
        c.V0 = c.V0 + c.S3;
        mem.WriteU8((c.V0 + 0x7u), (byte)c.V1);
        c.V0 = mem.ReadU8((c.S3 + 0x6u));
        c.V0 = c.V0 + 0x1u;
        mem.WriteU8((c.S3 + 0x6u), (byte)c.V0);
        L8001CB50: ;
        c.S0 = c.S0 + 0x30u;
        c.S1 = c.S1 - 0x1u;
        c.V0 = 0xFFFFFFFFu;
        if (c.S1 != c.V0) {
            c.S2 = c.S2 + 0x30u;
            goto L8001CAF8;
        }
        c.S2 = c.S2 + 0x30u;
        c.RA = mem.ReadU32((c.SP + 0x24u));
        c.S4 = mem.ReadU32((c.SP + 0x20u));
        c.S3 = mem.ReadU32((c.SP + 0x1Cu));
        c.S2 = mem.ReadU32((c.SP + 0x18u));
        c.S1 = mem.ReadU32((c.SP + 0x14u));
        c.S0 = mem.ReadU32((c.SP + 0x10u));
        c.SP = c.SP + 0x28u;
        return;
    }
}
