using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_800276BC</c>: text or a number at a position; the menu; not read. <c>ST_TEXT276BC</c>.
/// </summary>
public static class Text276BC
{
    public static readonly Literal R = new(typeof(Text276BC), "text-276BC", "TEXT276BC", 0x800276BC, Run,
        "text or a number at a position; the menu; not read.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x48u;
        mem.WriteU32((c.SP + 0x38u), c.S6);
        c.S6 = 0x00000001u;
        c.T1 = 0u;
        c.V0 = c.A0 >> 24;
        mem.WriteU32((c.SP + 0x28u), c.S2);
        c.S2 = c.V0 & 0x000Fu;
        c.T0 = c.A0;
        mem.WriteU32((c.SP + 0x3Cu), c.S7);
        c.S7 = c.A3;
        mem.WriteU32((c.SP + 0x34u), c.S5);
        c.S5 = c.A1;
        mem.WriteU32((c.SP + 0x30u), c.S4);
        c.S4 = c.A2;
        mem.WriteU32((c.SP + 0x24u), c.S1);
        c.S1 = 0x00000007u;
        c.V0 = 0xF0000000u;
        c.T2 = c.A0 & c.V0;
        c.A2 = 0xFFFFFFFFu;
        c.V0 = c.A0 << 16;
        mem.WriteU32((c.SP + 0x40u), c.RA);
        mem.WriteU32((c.SP + 0x2Cu), c.S3);
        if ((int)c.V0 >= 0) {
            mem.WriteU32((c.SP + 0x20u), c.S0);
            goto L80027724;
        }
        mem.WriteU32((c.SP + 0x20u), c.S0);
        c.T0 = 0u - c.A0;
        c.T1 = c.S6;
        L80027724: ;
        c.V0 = c.T0 << 16;
        if (c.V0 != 0u) {
            c.S0 = c.S2 - 0x1u;
            goto L80027760;
        }
        c.S0 = c.S2 - 0x1u;
        c.V1 = 0x00000007u;
        c.V0 = c.S2 - 0x1u;
        c.LO = (uint)((long)(int)c.V0 * (int)c.V1); c.HI = (uint)(((long)(int)c.V0 * (int)c.V1) >> 32);
        c.A0 = 0x00000002u;
        c.A2 = c.S4 & 0xFFFFu;
        c.A3 = c.S7;
        c.T3 = c.LO;
        c.A1 = c.A1 + c.T3;
        c.A1 = c.A1 & 0xFFFFu;
        c.RA = 0x80027758u;
        ShadowTower_st.func_80026ECC(c, mem);
        goto L800278D0;
        L80027760: ;
        if (c.S0 == c.A2) {
            c.V1 = 0x00000001u;
            goto L80027780;
        }
        c.V1 = 0x00000001u;
        c.A0 = 0xFFFFFFFFu;
        L8002776C: ;
        Poll(c, mem);
        c.V0 = c.V1 << 2;
        c.V0 = c.V0 + c.V1;
        c.S0 = c.S0 - 0x1u;
        if (c.S0 != c.A0) {
            c.V1 = c.V0 << 1;
            goto L8002776C;
        }
        c.V1 = c.V0 << 1;
        L80027780: ;
        c.V0 = c.T0 << 16;
        c.V0 = (uint)((int)c.V0 >> 16);
        c.V0 = c.V0 < c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.A1 = c.V1;
            goto L8002780C;
        }
        c.A1 = c.V1;
        if (c.S2 == 0u) {
            c.S0 = c.S2 - 0x1u;
            goto L80027838;
        }
        c.S0 = c.S2 - 0x1u;
        c.A3 = 0xCCCC0000u;
        c.A3 = c.A3 | 0xCCCDu;
        c.A2 = 0xFFFFFFFFu;
        c.V1 = c.SP + 0x10u;
        c.V0 = c.S0 << 1;
        c.A0 = c.V0 + c.V1;
        L800277B4: ;
        Poll(c, mem);
        c.LO = (uint)((ulong)c.A1 * c.A3); c.HI = (uint)(((ulong)c.A1 * c.A3) >> 32);
        c.V0 = c.T0 << 16;
        c.T3 = c.HI;
        c.V0 = (uint)((int)c.V0 >> 16);
        c.A1 = c.T3 >> 3;
        if (c.A1 != 0u) { c.LO = c.V0 / c.A1; c.HI = c.V0 % c.A1; }
        if (c.A1 != 0u) {
            goto L800277D8;
        }
        Bios.Break(c, mem);
        L800277D8: ;
        c.V0 = c.LO;
        mem.WriteU16(c.A0, (ushort)c.V0);
        c.V0 = mem.ReadU16(c.A0);
        c.LO = (uint)((long)(int)c.V0 * (int)c.A1); c.HI = (uint)(((long)(int)c.V0 * (int)c.A1) >> 32);
        c.S0 = c.S0 - 0x1u;
        c.A0 = c.A0 - 0x2u;
        c.T3 = c.LO;
        if (c.S0 != c.A2) {
            c.T0 = c.T0 - c.T3;
            goto L800277B4;
        }
        c.T0 = c.T0 - c.T3;
        c.V0 = c.T1 & 0x00FFu;
        goto L8002783C;
        L8002780C: ;
        if (c.S2 == 0u) {
            c.S0 = c.S2 - 0x1u;
            goto L80027838;
        }
        c.S0 = c.S2 - 0x1u;
        c.A1 = 0x00000009u;
        c.A0 = 0xFFFFFFFFu;
        c.V1 = c.SP + 0x10u;
        c.V0 = c.S0 << 1;
        c.V0 = c.V0 + c.V1;
        L80027828: ;
        Poll(c, mem);
        mem.WriteU16(c.V0, (ushort)c.A1);
        c.S0 = c.S0 - 0x1u;
        if (c.S0 != c.A0) {
            c.V0 = c.V0 - 0x2u;
            goto L80027828;
        }
        c.V0 = c.V0 - 0x2u;
        L80027838: ;
        c.V0 = c.T1 & 0x00FFu;
        L8002783C: ;
        if (c.V0 != 0u) {
            c.A0 = 0x0000000Du;
            goto L8002784C;
        }
        c.A0 = 0x0000000Du;
        if (c.T2 == 0u) {
            c.A0 = 0x0000000Cu;
            goto L80027860;
        }
        c.A0 = 0x0000000Cu;
        L8002784C: ;
        c.A1 = c.S5 - c.S1;
        c.A1 = c.A1 & 0xFFFFu;
        c.A2 = c.S4 & 0xFFFFu;
        c.A3 = c.S7;
        c.RA = 0x80027860u;
        ShadowTower_st.func_80026ECC(c, mem);
        L80027860: ;
        if (c.S2 == 0u) {
            c.S0 = c.S2 - 0x1u;
            goto L800278D0;
        }
        c.S0 = c.S2 - 0x1u;
        c.S3 = c.S1 & 0xFFFFu;
        c.V1 = c.SP + 0x10u;
        c.V0 = c.S0 << 1;
        c.S1 = c.V0 + c.V1;
        L80027878: ;
        Poll(c, mem);
        c.A0 = mem.ReadU16(c.S1);
        if (c.A0 == 0u) {
            c.V0 = c.S6 & 0x00FFu;
            goto L80027890;
        }
        c.V0 = c.S6 & 0x00FFu;
        c.S6 = 0u;
        c.V0 = c.S6 & 0x00FFu;
        L80027890: ;
        if (c.V0 != 0u) {
            c.V0 = c.S2 - c.S0;
            goto L800278C0;
        }
        c.V0 = c.S2 - c.S0;
        c.V0 = c.V0 - 0x1u;
        c.LO = (uint)((long)(int)c.S3 * (int)c.V0); c.HI = (uint)(((long)(int)c.S3 * (int)c.V0) >> 32);
        c.A0 = c.A0 + 0x2u;
        c.A0 = c.A0 & 0xFFFFu;
        c.A2 = c.S4 & 0xFFFFu;
        c.A3 = c.S7;
        c.T3 = c.LO;
        c.A1 = c.S5 + c.T3;
        c.A1 = c.A1 & 0xFFFFu;
        c.RA = 0x800278C0u;
        ShadowTower_st.func_80026ECC(c, mem);
        L800278C0: ;
        c.S0 = c.S0 - 0x1u;
        c.V0 = 0xFFFFFFFFu;
        if (c.S0 != c.V0) {
            c.S1 = c.S1 - 0x2u;
            goto L80027878;
        }
        c.S1 = c.S1 - 0x2u;
        L800278D0: ;
        c.RA = mem.ReadU32((c.SP + 0x40u));
        c.S7 = mem.ReadU32((c.SP + 0x3Cu));
        c.S6 = mem.ReadU32((c.SP + 0x38u));
        c.S5 = mem.ReadU32((c.SP + 0x34u));
        c.S4 = mem.ReadU32((c.SP + 0x30u));
        c.S3 = mem.ReadU32((c.SP + 0x2Cu));
        c.S2 = mem.ReadU32((c.SP + 0x28u));
        c.S1 = mem.ReadU32((c.SP + 0x24u));
        c.S0 = mem.ReadU32((c.SP + 0x20u));
        c.SP = c.SP + 0x48u;
        return;
    }
}
