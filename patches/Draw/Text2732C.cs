using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_8002732C</c>: a line of text at a position; the menu and the prompt; not read. <c>ST_TEXT2732C</c>.
/// </summary>
public static class Text2732C
{
    public static readonly Literal R = new(typeof(Text2732C), "text-2732C", "TEXT2732C", 0x8002732C, Run,
        "a line of text at a position; the menu and the prompt; not read.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x38u;
        mem.WriteU32((c.SP + 0x10u), c.S0);
        c.S0 = c.A0;
        mem.WriteU32((c.SP + 0x14u), c.S1);
        c.S1 = c.A1;
        mem.WriteU32((c.SP + 0x2Cu), c.S7);
        c.S7 = c.A3;
        mem.WriteU32((c.SP + 0x34u), c.RA);
        mem.WriteU32((c.SP + 0x30u), c.FP);
        mem.WriteU32((c.SP + 0x28u), c.S6);
        mem.WriteU32((c.SP + 0x24u), c.S5);
        mem.WriteU32((c.SP + 0x20u), c.S4);
        mem.WriteU32((c.SP + 0x1Cu), c.S3);
        mem.WriteU32((c.SP + 0x18u), c.S2);
        c.V0 = mem.ReadU8((c.S7 + 0x3u));
        c.S2 = c.A2;
        c.V0 = c.V0 & 0x007Cu;
        c.S4 = c.V0 >> 2;
        c.V0 = 0x1F800000u;
        c.V0 = mem.ReadU8((c.V0 + 0x2E5u));
        if (c.V0 != 0u) {
            c.FP = c.A1;
            goto L80027394;
        }
        c.FP = c.A1;
        c.S5 = 0x0000000Eu;
        c.S3 = 0x00000040u;
        goto L800273A0;
        L80027394: ;
        c.S4 = 0x0000000Bu;
        c.S5 = 0x0000000Au;
        c.S3 = 0x00000004u;
        L800273A0: ;
        c.V1 = mem.ReadU16(c.S0);
        c.V0 = 0u | 0xFFFFu;
        if (c.V1 == c.V0) {
            c.S6 = c.S4 >> 1;
            goto L8002740C;
        }
        c.S6 = c.S4 >> 1;
        L800273B0: ;
        Poll(c, mem);
        if (c.V1 != 0u) {
            c.A1 = c.S1 & 0xFFFFu;
            goto L800273C8;
        }
        c.A1 = c.S1 & 0xFFFFu;
        c.S1 = c.FP;
        c.S2 = c.S2 + c.S5;
        c.S0 = c.S0 + 0x4u;
        goto L800273FC;
        L800273C8: ;
        c.A2 = c.S2 & 0xFFFFu;
        c.A0 = mem.ReadU16(c.S0);
        c.A3 = c.S7;
        c.RA = 0x800273D8u;
        ShadowTower_st.func_80026ECC(c, mem);
        c.V0 = mem.ReadU16(c.S0);
        c.V0 = c.V0 < c.S3 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L800273F4;
        }
        c.S1 = c.S1 + c.S6;
        goto L800273F8;
        L800273F4: ;
        c.S1 = c.S1 + c.S4;
        L800273F8: ;
        c.S0 = c.S0 + 0x2u;
        L800273FC: ;
        c.V1 = mem.ReadU16(c.S0);
        c.V0 = 0u | 0xFFFFu;
        if (c.V1 != c.V0) {
            goto L800273B0;
        }
        L8002740C: ;
        c.RA = mem.ReadU32((c.SP + 0x34u));
        c.FP = mem.ReadU32((c.SP + 0x30u));
        c.S7 = mem.ReadU32((c.SP + 0x2Cu));
        c.S6 = mem.ReadU32((c.SP + 0x28u));
        c.S5 = mem.ReadU32((c.SP + 0x24u));
        c.S4 = mem.ReadU32((c.SP + 0x20u));
        c.S3 = mem.ReadU32((c.SP + 0x1Cu));
        c.S2 = mem.ReadU32((c.SP + 0x18u));
        c.S1 = mem.ReadU32((c.SP + 0x14u));
        c.S0 = mem.ReadU32((c.SP + 0x10u));
        c.SP = c.SP + 0x38u;
        return;
    }
}
