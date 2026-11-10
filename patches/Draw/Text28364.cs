using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_80028364</c>: text for func_80028784 and the menu; not read. <c>ST_TEXT28364</c>.
/// </summary>
public static class Text28364
{
    public static readonly Literal R = new(typeof(Text28364), "text-28364", "TEXT28364", 0x80028364, Run,
        "text for func_80028784 and the menu; not read.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x40u;
        mem.WriteU32((c.SP + 0x20u), c.S2);
        c.S2 = c.A0;
        mem.WriteU32((c.SP + 0x38u), c.FP);
        c.FP = c.A2;
        mem.WriteU32((c.SP + 0x30u), c.S6);
        c.A0 = mem.ReadU32((c.SP + 0x50u));
        c.V1 = 0u | 0x8000u;
        mem.WriteU32((c.SP + 0x3Cu), c.RA);
        mem.WriteU32((c.SP + 0x34u), c.S7);
        mem.WriteU32((c.SP + 0x2Cu), c.S5);
        mem.WriteU32((c.SP + 0x28u), c.S4);
        mem.WriteU32((c.SP + 0x24u), c.S3);
        mem.WriteU32((c.SP + 0x1Cu), c.S1);
        mem.WriteU32((c.SP + 0x18u), c.S0);
        c.T2 = c.A0 & 0x0001u;
        c.V0 = (uint)((int)c.A0 >> 1);
        c.T1 = c.V0 & 0x0003u;
        c.V0 = (uint)((int)c.A0 >> 3);
        c.T0 = c.V0 & 0x0003u;
        c.A0 = (uint)((int)c.A0 >> 5);
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.A2 = c.V0 + c.V1;
        mem.WriteU32((c.SP + 0x10u), c.T2);
        c.V1 = mem.ReadU32((c.A2 + 0x304u));
        c.S7 = c.A0 & 0x0003u;
        c.S0 = mem.ReadU32((c.V1 + 0x8u));
        c.V0 = mem.ReadU32((c.V1 + 0x4u));
        c.A0 = c.S0 + 0x28u;
        c.V0 = c.V0 < c.A0 ? 1u : 0u;
        if (c.V0 != 0u) {
            c.S6 = c.A3;
            goto L80028674;
        }
        c.S6 = c.A3;
        mem.WriteU32((c.V1 + 0x8u), c.A0);
        c.V0 = mem.ReadU32((c.A2 + 0x328u));
        c.V0 = c.V0 + 0x1u;
        mem.WriteU32((c.A2 + 0x328u), c.V0);
        c.V0 = mem.ReadU16(c.A1);
        c.V1 = mem.ReadU8(c.A1);
        c.S3 = mem.ReadU8((c.A1 + 0x2u));
        c.A2 = c.V0 & 0xFFC0u;
        c.V0 = mem.ReadU16((c.A1 + 0x2u));
        c.S1 = c.V1 & 0x003Fu;
        if (c.T0 != 0u) {
            c.A3 = c.V0 & 0xFF00u;
            goto L80028434;
        }
        c.A3 = c.V0 & 0xFF00u;
        c.V1 = c.S1 & 0x00FFu;
        c.V0 = c.V1 < 0x00000040u ? 1u : 0u;
        if (c.V0 == 0u) {
            c.S1 = c.V1 << 2;
            goto L8002844C;
        }
        c.S1 = c.V1 << 2;
        goto L80028450;
        L80028434: ;
        c.V0 = 0x00000001u;
        if (c.T0 != c.V0) {
            c.V1 = c.S1 & 0x00FFu;
            goto L80028450;
        }
        c.V1 = c.S1 & 0x00FFu;
        c.V0 = c.V1 < 0x00000080u ? 1u : 0u;
        if (c.V0 != 0u) {
            c.S1 = c.V1 << 1;
            goto L80028450;
        }
        c.S1 = c.V1 << 1;
        L8002844C: ;
        c.S1 = 0x000000FFu;
        L80028450: ;
        c.V1 = (uint)(short)mem.ReadU16((c.S2 + 0x4u));
        c.V0 = c.S1 & 0x00FFu;
        c.V0 = c.V0 + c.V1;
        c.V0 = (int)c.V0 < 256 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.S5 = 0x000000FFu;
            goto L80028474;
        }
        c.S5 = 0x000000FFu;
        c.V0 = mem.ReadU8((c.S2 + 0x4u));
        c.S5 = c.S1 + c.V0;
        L80028474: ;
        c.V0 = (uint)(short)mem.ReadU16((c.S2 + 0x6u));
        c.V0 = c.S3 + c.V0;
        c.V0 = (int)c.V0 < 256 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.S4 = 0x000000FFu;
            goto L80028498;
        }
        c.S4 = 0x000000FFu;
        c.V0 = mem.ReadU8((c.S2 + 0x6u));
        c.S4 = c.S3 + c.V0;
        L80028498: ;
        c.A0 = c.T0;
        c.A1 = c.T1;
        c.A2 = c.A2 << 16;
        c.A3 = c.A3 << 16;
        c.A2 = (uint)((int)c.A2 >> 16);
        c.A3 = (uint)((int)c.A3 >> 16);
        c.RA = 0x800284B4u;
        ShadowTower_st.GetTPage_st(c, mem);
        mem.WriteU16((c.S0 + 0x16u), (ushort)c.V0);
        c.A0 = (uint)(short)mem.ReadU16(c.FP);
        c.A1 = (uint)(short)mem.ReadU16((c.FP + 0x2u));
        c.RA = 0x800284C8u;
        ShadowTower_st.GetClut_st(c, mem);
        if (c.S6 != 0u) {
            mem.WriteU16((c.S0 + 0xEu), (ushort)c.V0);
            goto L800284E0;
        }
        mem.WriteU16((c.S0 + 0xEu), (ushort)c.V0);
        c.V0 = 0x00000080u;
        mem.WriteU8((c.S0 + 0x4u), (byte)c.V0);
        mem.WriteU8((c.S0 + 0x5u), (byte)c.V0);
        goto L800284FC;
        L800284E0: ;
        c.V0 = mem.ReadU8(c.S6);
        mem.WriteU8((c.S0 + 0x4u), (byte)c.V0);
        c.V0 = mem.ReadU8((c.S6 + 0x1u));
        mem.WriteU8((c.S0 + 0x5u), (byte)c.V0);
        c.V0 = mem.ReadU8((c.S6 + 0x2u));
        L800284FC: ;
        mem.WriteU8((c.S0 + 0x6u), (byte)c.V0);
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.V1 = 0u | 0x8000u;
        c.V0 = c.V0 + c.V1;
        c.V0 = mem.ReadU32((c.V0 + 0xE4u));
        c.V1 = 0x00FF0000u;
        c.V0 = mem.ReadU32(c.V0);
        c.V1 = c.V1 | 0xFFFFu;
        c.V0 = c.V0 & c.V1;
        c.V1 = 0x09000000u;
        c.V0 = c.V0 | c.V1;
        mem.WriteU32(c.S0, c.V0);
        c.V0 = mem.ReadU16(c.S2);
        mem.WriteU16((c.S0 + 0x8u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S2 + 0x2u));
        mem.WriteU16((c.S0 + 0xAu), (ushort)c.V0);
        c.V0 = mem.ReadU16(c.S2);
        c.V1 = mem.ReadU16((c.S2 + 0x4u));
        c.V0 = c.V0 + c.V1;
        mem.WriteU16((c.S0 + 0x10u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S2 + 0x2u));
        mem.WriteU16((c.S0 + 0x12u), (ushort)c.V0);
        c.V0 = mem.ReadU16(c.S2);
        mem.WriteU16((c.S0 + 0x18u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S2 + 0x2u));
        c.V1 = mem.ReadU16((c.S2 + 0x6u));
        c.V0 = c.V0 + c.V1;
        mem.WriteU16((c.S0 + 0x1Au), (ushort)c.V0);
        c.V0 = mem.ReadU16(c.S2);
        c.V1 = mem.ReadU16((c.S2 + 0x4u));
        c.V0 = c.V0 + c.V1;
        mem.WriteU16((c.S0 + 0x20u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S2 + 0x2u));
        c.V1 = mem.ReadU16((c.S2 + 0x6u));
        c.V0 = c.V0 + c.V1;
        mem.WriteU16((c.S0 + 0x22u), (ushort)c.V0);
        c.V0 = 0x00000001u;
        if (c.S7 != c.V0) {
            c.V0 = 0x00000002u;
            goto L800285E8;
        }
        c.V0 = 0x00000002u;
        c.V1 = c.S5 - 0x1u;
        c.V0 = c.S1 - 0x1u;
        mem.WriteU8((c.S0 + 0xCu), (byte)c.V1);
        mem.WriteU8((c.S0 + 0xDu), (byte)c.S3);
        mem.WriteU8((c.S0 + 0x14u), (byte)c.V0);
        mem.WriteU8((c.S0 + 0x15u), (byte)c.S3);
        mem.WriteU8((c.S0 + 0x1Cu), (byte)c.V1);
        mem.WriteU8((c.S0 + 0x1Du), (byte)c.S4);
        mem.WriteU8((c.S0 + 0x24u), (byte)c.V0);
        goto L80028634;
        L800285E8: ;
        if (c.S7 != c.V0) {
            c.V0 = c.S4 - 0x1u;
            goto L80028618;
        }
        c.V0 = c.S4 - 0x1u;
        mem.WriteU8((c.S0 + 0xDu), (byte)c.V0);
        mem.WriteU8((c.S0 + 0x15u), (byte)c.V0);
        c.V0 = c.S3 - 0x1u;
        mem.WriteU8((c.S0 + 0xCu), (byte)c.S1);
        mem.WriteU8((c.S0 + 0x14u), (byte)c.S5);
        mem.WriteU8((c.S0 + 0x1Cu), (byte)c.S1);
        mem.WriteU8((c.S0 + 0x1Du), (byte)c.V0);
        mem.WriteU8((c.S0 + 0x24u), (byte)c.S5);
        mem.WriteU8((c.S0 + 0x25u), (byte)c.V0);
        goto L80028638;
        L80028618: ;
        mem.WriteU8((c.S0 + 0xCu), (byte)c.S1);
        mem.WriteU8((c.S0 + 0xDu), (byte)c.S3);
        mem.WriteU8((c.S0 + 0x14u), (byte)c.S5);
        mem.WriteU8((c.S0 + 0x15u), (byte)c.S3);
        mem.WriteU8((c.S0 + 0x1Cu), (byte)c.S1);
        mem.WriteU8((c.S0 + 0x1Du), (byte)c.S4);
        mem.WriteU8((c.S0 + 0x24u), (byte)c.S5);
        L80028634: ;
        mem.WriteU8((c.S0 + 0x25u), (byte)c.S4);
        L80028638: ;
        c.T2 = mem.ReadU32((c.SP + 0x10u));
        if (c.T2 != 0u) {
            c.V0 = 0x0000002Eu;
            goto L8002864C;
        }
        c.V0 = 0x0000002Eu;
        c.V0 = 0x0000002Cu;
        L8002864C: ;
        mem.WriteU8((c.S0 + 0x7u), (byte)c.V0);
        c.A0 = 0x00FF0000u;
        c.A0 = c.A0 | 0xFFFFu;
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.V1 = 0u | 0x8000u;
        c.V0 = c.V0 + c.V1;
        c.V0 = mem.ReadU32((c.V0 + 0xE4u));
        c.A0 = c.S0 & c.A0;
        mem.WriteU32(c.V0, c.A0);
        L80028674: ;
        c.RA = mem.ReadU32((c.SP + 0x3Cu));
        c.FP = mem.ReadU32((c.SP + 0x38u));
        c.S7 = mem.ReadU32((c.SP + 0x34u));
        c.S6 = mem.ReadU32((c.SP + 0x30u));
        c.S5 = mem.ReadU32((c.SP + 0x2Cu));
        c.S4 = mem.ReadU32((c.SP + 0x28u));
        c.S3 = mem.ReadU32((c.SP + 0x24u));
        c.S2 = mem.ReadU32((c.SP + 0x20u));
        c.S1 = mem.ReadU32((c.SP + 0x1Cu));
        c.S0 = mem.ReadU32((c.SP + 0x18u));
        c.SP = c.SP + 0x40u;
        return;
    }
}
