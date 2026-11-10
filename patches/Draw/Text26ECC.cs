using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_80026ECC</c>: drawn by the menu at (0x6A, y); not read. <c>ST_TEXT26ECC</c>.
/// </summary>
public static class Text26ECC
{
    public static readonly Literal R = new(typeof(Text26ECC), "text-26ECC", "TEXT26ECC", 0x80026ECC, Run,
        "drawn by the menu at (0x6A, y); not read.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x20u;
        c.V0 = 0x1F800000u;
        c.V0 = mem.ReadU8((c.V0 + 0x2E5u));
        c.V1 = c.A0;
        mem.WriteU32((c.SP + 0x1Cu), c.RA);
        if (c.V0 != 0u) {
            mem.WriteU32((c.SP + 0x18u), c.S0);
            goto L80026F08;
        }
        mem.WriteU32((c.SP + 0x18u), c.S0);
        c.T5 = 0x0000000Cu;
        c.T3 = c.T5;
        c.T6 = c.T3;
        c.T7 = c.T3;
        c.T4 = 0x00000006u;
        c.T0 = 0x00000015u;
        c.A0 = 0x00000040u;
        goto L80026F24;
        L80026F08: ;
        c.T4 = 0x00000007u;
        c.T0 = 0x00000012u;
        c.A0 = 0x00000004u;
        c.T7 = 0x0000000Cu;
        c.T6 = 0x0000000Au;
        c.T3 = c.T7;
        c.T5 = c.T6;
        L80026F24: ;
        c.V1 = c.V1 & 0xFFFFu;
        c.A0 = c.A0 & 0xFFFFu;
        c.V0 = c.V1 < c.A0 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.A0 = c.A0 >> 1;
            goto L80027084;
        }
        c.A0 = c.A0 >> 1;
        c.A0 = c.T0 << 1;
        if (c.A0 != 0u) { if ((int)c.V1 == int.MinValue && (int)c.A0 == -1) { c.LO = 0x80000000u; c.HI = 0u; } else { c.LO = (uint)((int)c.V1 / (int)c.A0); c.HI = (uint)((int)c.V1 % (int)c.A0); } }
        if (c.A0 != 0u) {
            goto L80026F4C;
        }
        Bios.Break(c, mem);
        L80026F4C: ;
        c.At = 0xFFFFFFFFu;
        if (c.A0 != c.At) {
            c.At = 0x80000000u;
            goto L80026F64;
        }
        c.At = 0x80000000u;
        if (c.V1 != c.At) {
            goto L80026F64;
        }
        Bios.Break(c, mem);
        L80026F64: ;
        c.A0 = c.LO;
        c.V0 = c.HI;
        c.T3 = c.T4 & 0xFFFFu;
        c.LO = (uint)((long)(int)c.V0 * (int)c.T3); c.HI = (uint)(((long)(int)c.V0 * (int)c.T3) >> 32);
        c.T1 = c.LO;
        c.LO = (uint)((long)(int)c.T5 * (int)c.A0); c.HI = (uint)(((long)(int)c.T5 * (int)c.A0) >> 32);
        c.V1 = 0u | 0x8000u;
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.T2 = c.V0 + c.V1;
        c.V1 = mem.ReadU32((c.T2 + 0x304u));
        c.S0 = mem.ReadU32((c.V1 + 0x8u));
        c.V0 = mem.ReadU32((c.V1 + 0x4u));
        c.T0 = c.S0 + 0x28u;
        c.V0 = c.V0 < c.T0 ? 1u : 0u;
        c.A0 = c.LO;
        if (c.V0 != 0u) {
            goto L80027318;
        }
        mem.WriteU32((c.V1 + 0x8u), c.T0);
        c.V0 = mem.ReadU32((c.T2 + 0x328u));
        c.V1 = c.T4 + c.T1;
        c.V0 = c.V0 + 0x1u;
        mem.WriteU32((c.T2 + 0x328u), c.V0);
        c.V0 = c.T5 + c.A0;
        mem.WriteU8((c.S0 + 0xCu), (byte)c.T1);
        mem.WriteU8((c.S0 + 0xDu), (byte)c.A0);
        mem.WriteU8((c.S0 + 0x14u), (byte)c.V1);
        mem.WriteU8((c.S0 + 0x15u), (byte)c.A0);
        mem.WriteU8((c.S0 + 0x1Cu), (byte)c.T1);
        mem.WriteU8((c.S0 + 0x1Du), (byte)c.V0);
        mem.WriteU8((c.S0 + 0x24u), (byte)c.V1);
        mem.WriteU8((c.S0 + 0x25u), (byte)c.V0);
        c.V0 = mem.ReadU32((c.GP + 0x304u));
        if (c.V0 == 0u) {
            c.V0 = 0x00000001u;
            goto L8002707C;
        }
        c.V0 = 0x00000001u;
        c.V1 = 0x80090000u;
        c.V1 = mem.ReadU16((c.V1 - 0x7E54u));
        if (c.V1 != c.V0) {
            c.V0 = c.T3 + 0x1u;
            goto L80027028;
        }
        c.V0 = c.T3 + 0x1u;
        c.V1 = 0x80090000u;
        c.V1 = mem.ReadU16((c.V1 - 0x7E60u));
        c.LO = (uint)((long)(int)c.V1 * (int)c.V0); c.HI = (uint)(((long)(int)c.V1 * (int)c.V0) >> 32);
        goto L80027038;
        L80027028: ;
        c.V1 = 0x80090000u;
        c.V1 = mem.ReadU16((c.V1 - 0x7E5Eu));
        c.LO = (uint)((long)(int)c.V1 * (int)c.V0); c.HI = (uint)(((long)(int)c.V1 * (int)c.V0) >> 32);
        L80027038: ;
        c.T8 = c.LO;
        c.A1 = c.A1 - c.T8;
        c.V0 = mem.ReadU32((c.GP + 0x300u));
        c.V0 = c.V0 + 0x1u;
        c.V0 = c.T4 + c.V0;
        mem.WriteU32((c.GP + 0x300u), c.V0);
        c.V0 = (uint)(short)mem.ReadU16((c.GP + 0x2FCu));
        c.V1 = c.A1 & 0xFFFFu;
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L800271B0;
        }
        c.V0 = (uint)(short)mem.ReadU16((c.GP + 0x2FAu));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L80027318;
        }
        L8002707C: ;
        c.V0 = c.A1 + c.T4;
        goto L800271E0;
        L80027084: ;
        c.A0 = c.V1 - c.A0;
        if (c.T0 != 0u) { if ((int)c.A0 == int.MinValue && (int)c.T0 == -1) { c.LO = 0x80000000u; c.HI = 0u; } else { c.LO = (uint)((int)c.A0 / (int)c.T0); c.HI = (uint)((int)c.A0 % (int)c.T0); } }
        if (c.T0 != 0u) {
            goto L80027098;
        }
        Bios.Break(c, mem);
        L80027098: ;
        c.At = 0xFFFFFFFFu;
        if (c.T0 != c.At) {
            c.At = 0x80000000u;
            goto L800270B0;
        }
        c.At = 0x80000000u;
        if (c.A0 != c.At) {
            goto L800270B0;
        }
        Bios.Break(c, mem);
        L800270B0: ;
        c.A0 = c.LO;
        c.V0 = c.HI;
        c.V0 = c.V0 & 0xFFFFu;
        c.LO = (uint)((long)(int)c.T3 * (int)c.V0); c.HI = (uint)(((long)(int)c.T3 * (int)c.V0) >> 32);
        c.T1 = c.LO;
        c.A0 = c.A0 & 0xFFFFu;
        c.LO = (uint)((long)(int)c.T5 * (int)c.A0); c.HI = (uint)(((long)(int)c.T5 * (int)c.A0) >> 32);
        c.V1 = 0u | 0x8000u;
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.T2 = c.V0 + c.V1;
        c.V1 = mem.ReadU32((c.T2 + 0x304u));
        c.S0 = mem.ReadU32((c.V1 + 0x8u));
        c.V0 = mem.ReadU32((c.V1 + 0x4u));
        c.T0 = c.S0 + 0x28u;
        c.V0 = c.V0 < c.T0 ? 1u : 0u;
        c.A0 = c.LO;
        if (c.V0 != 0u) {
            goto L80027318;
        }
        mem.WriteU32((c.V1 + 0x8u), c.T0);
        c.V0 = mem.ReadU32((c.T2 + 0x328u));
        c.V1 = c.T3 + c.T1;
        c.V0 = c.V0 + 0x1u;
        mem.WriteU32((c.T2 + 0x328u), c.V0);
        c.V0 = c.T5 + c.A0;
        mem.WriteU8((c.S0 + 0xCu), (byte)c.T1);
        mem.WriteU8((c.S0 + 0xDu), (byte)c.A0);
        mem.WriteU8((c.S0 + 0x14u), (byte)c.V1);
        mem.WriteU8((c.S0 + 0x15u), (byte)c.A0);
        mem.WriteU8((c.S0 + 0x1Cu), (byte)c.T1);
        mem.WriteU8((c.S0 + 0x1Du), (byte)c.V0);
        mem.WriteU8((c.S0 + 0x24u), (byte)c.V1);
        mem.WriteU8((c.S0 + 0x25u), (byte)c.V0);
        c.V0 = mem.ReadU32((c.GP + 0x304u));
        if (c.V0 == 0u) {
            c.V0 = 0x00000001u;
            goto L800271DC;
        }
        c.V0 = 0x00000001u;
        c.V1 = 0x80090000u;
        c.V1 = mem.ReadU16((c.V1 - 0x7E54u));
        if (c.V1 != c.V0) {
            c.V0 = c.T4 + 0x1u;
            goto L80027174;
        }
        c.V0 = c.T4 + 0x1u;
        c.V1 = 0x80090000u;
        c.V1 = mem.ReadU16((c.V1 - 0x7E60u));
        c.LO = (uint)((long)(int)c.V1 * (int)c.V0); c.HI = (uint)(((long)(int)c.V1 * (int)c.V0) >> 32);
        goto L80027184;
        L80027174: ;
        c.V1 = 0x80090000u;
        c.V1 = mem.ReadU16((c.V1 - 0x7E5Eu));
        c.LO = (uint)((long)(int)c.V1 * (int)c.V0); c.HI = (uint)(((long)(int)c.V1 * (int)c.V0) >> 32);
        L80027184: ;
        c.T8 = c.LO;
        c.A1 = c.A1 - c.T8;
        c.V0 = mem.ReadU32((c.GP + 0x300u));
        c.V0 = c.T3 + c.V0;
        mem.WriteU32((c.GP + 0x300u), c.V0);
        c.V0 = (uint)(short)mem.ReadU16((c.GP + 0x2FCu));
        c.V1 = c.A1 & 0xFFFFu;
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L800271C8;
        }
        L800271B0: ;
        c.V0 = mem.ReadU32((c.GP + 0x308u));
        c.V0 = c.V0 + 0x1u;
        mem.WriteU32((c.GP + 0x308u), c.V0);
        goto L80027318;
        L800271C8: ;
        c.V0 = (uint)(short)mem.ReadU16((c.GP + 0x2FAu));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L80027318;
        }
        L800271DC: ;
        c.V0 = c.A1 + c.T7;
        L800271E0: ;
        mem.WriteU16((c.S0 + 0x20u), (ushort)c.V0);
        mem.WriteU16((c.S0 + 0x10u), (ushort)c.V0);
        c.V0 = c.A2 + c.T6;
        mem.WriteU16((c.S0 + 0x18u), (ushort)c.A1);
        mem.WriteU16((c.S0 + 0x8u), (ushort)c.A1);
        mem.WriteU16((c.S0 + 0x12u), (ushort)c.A2);
        mem.WriteU16((c.S0 + 0xAu), (ushort)c.A2);
        mem.WriteU16((c.S0 + 0x22u), (ushort)c.V0);
        mem.WriteU16((c.S0 + 0x1Au), (ushort)c.V0);
        c.V0 = mem.ReadWordLeft(c.V0, (c.A3 + 0x3u));
        c.V0 = mem.ReadWordRight(c.V0, c.A3);
        mem.WriteWordLeft((c.SP + 0x13u), c.V0);
        mem.WriteWordRight((c.SP + 0x10u), c.V0);
        c.V0 = mem.ReadU8((c.SP + 0x10u));
        c.V1 = mem.ReadU8((c.SP + 0x12u));
        c.V0 = c.V0 + 0x8u;
        mem.WriteU8((c.SP + 0x10u), (byte)c.V0);
        c.V0 = mem.ReadU8((c.SP + 0x11u));
        c.V1 = c.V1 + 0x8u;
        mem.WriteU8((c.SP + 0x12u), (byte)c.V1);
        c.V0 = c.V0 + 0x8u;
        mem.WriteU8((c.SP + 0x11u), (byte)c.V0);
        c.V0 = mem.ReadU8((c.SP + 0x10u));
        mem.WriteU8((c.S0 + 0x4u), (byte)c.V0);
        c.V0 = mem.ReadU8((c.SP + 0x11u));
        mem.WriteU8((c.S0 + 0x5u), (byte)c.V0);
        c.V0 = mem.ReadU8((c.SP + 0x12u));
        mem.WriteU8((c.S0 + 0x6u), (byte)c.V0);
        c.A1 = mem.ReadU8((c.A3 + 0x3u));
        c.V0 = c.A1 & 0x0080u;
        if (c.V0 == 0u) {
            c.A0 = 0u;
            goto L80027290;
        }
        c.A0 = 0u;
        c.A1 = c.A1 & 0x0003u;
        c.A2 = 0x00000380u;
        c.A3 = 0x00000100u;
        c.RA = 0x80027284u;
        ShadowTower_st.GetTPage_st(c, mem);
        c.A0 = 0x00000230u;
        c.A1 = 0x000001F1u;
        goto L800272A8;
        L80027290: ;
        c.A1 = c.A1 & 0x0003u;
        c.A2 = 0x000003C0u;
        c.A3 = 0x00000100u;
        c.RA = 0x800272A0u;
        ShadowTower_st.GetTPage_st(c, mem);
        c.A0 = 0x00000230u;
        c.A1 = 0x000001EFu;
        L800272A8: ;
        mem.WriteU16((c.S0 + 0x16u), (ushort)c.V0);
        c.RA = 0x800272B0u;
        ShadowTower_st.GetClut_st(c, mem);
        mem.WriteU16((c.S0 + 0xEu), (ushort)c.V0);
        c.A1 = 0x00FF0000u;
        c.A1 = c.A1 | 0xFFFFu;
        c.V0 = 0x00000009u;
        mem.WriteU8((c.S0 + 0x3u), (byte)c.V0);
        c.V0 = 0x0000002Eu;
        c.A0 = 0x80160000u;
        c.A0 = c.A0 - 0x2F70u;
        mem.WriteU8((c.S0 + 0x7u), (byte)c.V0);
        c.V0 = 0u | 0x8000u;
        c.A0 = c.A0 + c.V0;
        c.A2 = 0xFF000000u;
        c.V0 = mem.ReadU32((c.A0 + 0xE4u));
        c.V1 = mem.ReadU32(c.S0);
        c.V0 = mem.ReadU32(c.V0);
        c.V1 = c.V1 & c.A2;
        c.V0 = c.V0 & c.A1;
        c.V1 = c.V1 | c.V0;
        mem.WriteU32(c.S0, c.V1);
        c.V1 = mem.ReadU32((c.A0 + 0xE4u));
        c.V0 = mem.ReadU32(c.V1);
        c.A1 = c.S0 & c.A1;
        c.V0 = c.V0 & c.A2;
        c.V0 = c.V0 | c.A1;
        mem.WriteU32(c.V1, c.V0);
        L80027318: ;
        c.RA = mem.ReadU32((c.SP + 0x1Cu));
        c.S0 = mem.ReadU32((c.SP + 0x18u));
        c.SP = c.SP + 0x20u;
        return;
    }
}
