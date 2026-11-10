using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary><see cref="Draw16A14"/>'s straight-line runs, moved out whole so neither file
/// passes the length rule (scripts/split_literal.py). Each is named after the label
/// before it.</summary>
public static partial class Draw16A14
{
    static void After80016A80(CpuContext c, PSMemory mem)
    {
        c.V0 = c.V0 - 0x1u;
        c.V0 = mem.ReadWordLeft(c.V0, (c.S3 + 0x11u));
        c.V0 = mem.ReadWordRight(c.V0, (c.S3 + 0xEu));
        c.V1 = mem.ReadWordLeft(c.V1, (c.S3 + 0x15u));
        c.V1 = mem.ReadWordRight(c.V1, (c.S3 + 0x12u));
        mem.WriteWordLeft((c.SP + 0x13u), c.V0);
        mem.WriteWordRight((c.SP + 0x10u), c.V0);
        mem.WriteWordLeft((c.SP + 0x17u), c.V1);
        mem.WriteWordRight((c.SP + 0x14u), c.V1);
        c.V0 = mem.ReadU8((c.S3 - 0x1u));
        mem.WriteU8(c.S3, (byte)c.V0);
        c.V1 = mem.ReadU8(c.FP);
        c.V0 = (int)c.V1 < 21 ? 1u : 0u;
    }

    static void After80016B58(CpuContext c, PSMemory mem)
    {
        c.A0 = c.S7;
        c.A1 = c.S4;
        c.V0 = mem.ReadU16((c.S3 + 0x10u));
        c.V1 = mem.ReadU16((c.SP + 0x16u));
        c.S1 = (uint)(short)mem.ReadU16((c.S3 + 0x8u));
        c.V0 = c.V0 + c.V1;
        mem.WriteU16((c.SP + 0x12u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S3 + 0x4u));
        c.A2 = c.S1;
        mem.WriteU16((c.SP + 0x16u), (ushort)c.V0);
        c.RA = 0x80016BBCu;
        ShadowTower_st.func_80079DCC(c, mem);
        c.S3 = c.S3 + 0x18u;
    }

    static void After80016B58_2(CpuContext c, PSMemory mem)
    {
        c.A0 = c.S7;
        c.S4 = (uint)(short)mem.ReadU16((c.S3 + 0x6u));
        c.A3 = (uint)(short)mem.ReadU16((c.S3 + 0x8u));
        c.V1 = mem.ReadU16((c.S3 + 0x4u));
        c.A2 = (uint)(short)mem.ReadU16((c.S3 + 0x4u));
        c.V0 = mem.ReadU16((c.S3 + 0x14u));
        c.A1 = c.S4;
        c.A2 = c.A3 + c.A2;
        c.V0 = c.V0 - c.V1;
        mem.WriteU16((c.SP + 0x16u), (ushort)c.V0);
        c.RA = 0x80016B84u;
        ShadowTower_st.func_80079DCC(c, mem);
        c.V0 = (uint)(short)mem.ReadU16((c.S3 + 0x4u));
    }

    static void After80016C4C(CpuContext c, PSMemory mem)
    {
        c.A2 = c.S1;
        c.S0 = c.S2 << 16;
        c.A1 = (uint)(short)mem.ReadU16((c.S3 + 0x6u));
        c.V0 = mem.ReadU16((c.S3 + 0xEu));
        c.S0 = (uint)((int)c.S0 >> 16);
        mem.WriteU16((c.SP + 0x10u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S3 + 0x12u));
        c.A1 = c.A1 + c.S0;
        c.V0 = c.V0 - c.S2;
        mem.WriteU16((c.SP + 0x14u), (ushort)c.V0);
        c.RA = 0x80016C78u;
        ShadowTower_st.func_80079DCC(c, mem);
    }

    static void After80016D70(CpuContext c, PSMemory mem)
    {
        c.V1 = c.LO;
        c.A0 = c.S5 << 16;
        c.A0 = (uint)((int)c.A0 >> 16);
        c.LO = (uint)((long)(int)c.V1 * (int)c.A0); c.HI = (uint)(((long)(int)c.V1 * (int)c.A0) >> 32);
        c.A0 = c.LO;
        c.RA = 0x80016D8Cu;
        ShadowTower_st.rsin_st(c, mem);
        c.V1 = mem.ReadU16((c.S3 + 0x14u));
        c.A2 = c.V0;
        c.V1 = c.V1 << 16;
        c.A0 = (uint)((int)c.V1 >> 16);
        c.A1 = c.A0;
        c.V1 = c.V1 >> 31;
        c.V1 = c.A0 + c.V1;
    }

    static void After80016EA8(CpuContext c, PSMemory mem)
    {
        c.V0 = mem.ReadU16((c.S3 + 0x14u));
        c.LO = (uint)((long)(int)c.A0 * (int)c.V0); c.HI = (uint)(((long)(int)c.A0 * (int)c.V0) >> 32);
        c.V0 = mem.ReadU16((c.SP + 0x12u));
        c.T9 = c.LO;
        c.V0 = c.V0 + c.T9;
        mem.WriteU16((c.SP + 0x12u), (ushort)c.V0);
        c.A1 = (uint)(short)mem.ReadU16((c.S3 + 0x6u));
        c.A2 = (uint)(short)mem.ReadU16((c.S3 + 0x8u));
        c.A0 = c.S7;
        c.RA = 0x80016ED4u;
        ShadowTower_st.func_80079DCC(c, mem);
        c.S3 = c.S3 + 0x18u;
    }

    static void After80016EDC(CpuContext c, PSMemory mem)
    {
        c.A0 = c.S7;
        c.V0 = 0x800A0000u;
        c.S2 = mem.ReadU16((c.V0 - 0x5376u));
        c.V1 = mem.ReadU16((c.S6 + 0x2u));
        c.A2 = mem.ReadU16((c.S6 + 0x4u));
        c.A3 = mem.ReadU16((c.S6 + 0x6u));
        c.T0 = mem.ReadU16((c.S6 + 0x8u));
        c.T1 = mem.ReadU16((c.S6 + 0xAu));
        c.T2 = mem.ReadU16((c.S6 + 0xCu));
        c.T3 = mem.ReadU16((c.S6 + 0xEu));
        c.T4 = mem.ReadU16((c.S6 + 0x10u));
        c.T5 = mem.ReadU16((c.S6 + 0x12u));
        c.T6 = mem.ReadU16((c.S6 + 0x14u));
        c.T7 = mem.ReadU16((c.S6 + 0x16u));
        c.S0 = mem.ReadU16((c.S6 + 0x18u));
        c.S1 = mem.ReadU16((c.S6 + 0x1Au));
        c.A1 = c.S6 - 0x2u;
        mem.WriteU16((c.V0 - 0x5376u), (ushort)c.V1);
        mem.WriteU16((c.S6 + 0x2u), (ushort)c.A2);
        mem.WriteU16((c.S6 + 0x4u), (ushort)c.A3);
        mem.WriteU16((c.S6 + 0x6u), (ushort)c.T0);
        mem.WriteU16((c.S6 + 0x8u), (ushort)c.T1);
        mem.WriteU16((c.S6 + 0xAu), (ushort)c.T2);
        mem.WriteU16((c.S6 + 0xCu), (ushort)c.T3);
        mem.WriteU16((c.S6 + 0xEu), (ushort)c.T4);
        mem.WriteU16((c.S6 + 0x10u), (ushort)c.T5);
        mem.WriteU16((c.S6 + 0x12u), (ushort)c.T6);
        mem.WriteU16((c.S6 + 0x14u), (ushort)c.T7);
        mem.WriteU16((c.S6 + 0x16u), (ushort)c.S0);
        mem.WriteU16((c.S6 + 0x18u), (ushort)c.S1);
        mem.WriteU16((c.S6 + 0x1Au), (ushort)c.S2);
        c.RA = 0x80016F5Cu;
        ShadowTower_st.func_80079D0C(c, mem);
        c.S3 = c.S3 + 0x18u;
    }

    static void After80016F6C(CpuContext c, PSMemory mem)
    {
        mem.WriteU32((c.SP + 0x18u), c.T8);
        c.RA = mem.ReadU32((c.SP + 0x44u));
        c.FP = mem.ReadU32((c.SP + 0x40u));
        c.S7 = mem.ReadU32((c.SP + 0x3Cu));
        c.S6 = mem.ReadU32((c.SP + 0x38u));
        c.S5 = mem.ReadU32((c.SP + 0x34u));
        c.S4 = mem.ReadU32((c.SP + 0x30u));
        c.S3 = mem.ReadU32((c.SP + 0x2Cu));
        c.S2 = mem.ReadU32((c.SP + 0x28u));
        c.S1 = mem.ReadU32((c.SP + 0x24u));
        c.S0 = mem.ReadU32((c.SP + 0x20u));
        c.SP = c.SP + 0x48u;
    }

    static void AfterEntry(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x48u;
        c.V0 = 0x80180000u;
        mem.WriteU32((c.SP + 0x40u), c.FP);
        c.FP = c.V0 - 0x4430u;
        c.T8 = 0x0000000Fu;
        mem.WriteU32((c.SP + 0x3Cu), c.S7);
        c.S7 = c.SP + 0x10u;
        c.V0 = 0x800A0000u;
        mem.WriteU32((c.SP + 0x38u), c.S6);
        c.S6 = c.V0 - 0x5376u;
        mem.WriteU32((c.SP + 0x2Cu), c.S3);
        c.S3 = c.FP + 0x2u;
        mem.WriteU32((c.SP + 0x44u), c.RA);
        mem.WriteU32((c.SP + 0x34u), c.S5);
        mem.WriteU32((c.SP + 0x30u), c.S4);
        mem.WriteU32((c.SP + 0x28u), c.S2);
        mem.WriteU32((c.SP + 0x24u), c.S1);
        mem.WriteU32((c.SP + 0x20u), c.S0);
        mem.WriteU32((c.SP + 0x18u), c.T8);
    }
}
