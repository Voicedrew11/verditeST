using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary><see cref="Effect246E4"/>'s straight-line runs, moved out whole so neither file
/// passes the length rule (scripts/split_literal.py). Each is named after the label
/// before it.</summary>
public static partial class Effect246E4
{
    static void After800248A8(CpuContext c, PSMemory mem)
    {
        c.RA = 0x80024904u;
        ShadowTower_st.func_80078AC4(c, mem);
        c.V1 = mem.ReadU16((c.S0 + 0x4Eu));
        c.V0 = (uint)((int)c.V0 >> 8);
        c.A0 = c.V1 << 1;
        c.A0 = c.A0 + c.V1;
        c.A0 = c.A0 << 1;
        c.A0 = c.A0 + c.V0;
        c.A0 = c.S6 + c.A0;
        mem.WriteU32((c.S0 + 0x50u), c.A0);
    }

    static void After800248A8_2(CpuContext c, PSMemory mem)
    {
        c.A0 = mem.ReadU32((c.S0 + 0x24u));
        c.A1 = mem.ReadU32((c.S0 + 0x2Cu));
        c.A2 = mem.ReadU8((c.S0 + 0x48u));
        c.A3 = mem.ReadU8((c.S0 + 0x49u));
        c.T0 = 0u | 0x8000u;
        mem.WriteU32((c.SP + 0x10u), c.T0);
        c.A0 = (uint)((int)c.A0 >> 12);
        c.A1 = (uint)((int)c.A1 >> 12);
        c.RA = 0x800248CCu;
        ShadowTower_st.func_8004FC94(c, mem);
    }

    static void After80024924(CpuContext c, PSMemory mem)
    {
        c.RA = 0x80024940u;
        ShadowTower_st.func_80078AC4(c, mem);
        c.V1 = mem.ReadU16((c.S0 + 0x4Eu));
        c.V0 = (uint)((int)c.V0 >> 6);
        c.A0 = c.V1 << 1;
        c.A0 = c.A0 + c.V1;
        c.A0 = c.A0 << 1;
        c.A0 = c.A0 + c.V0;
        c.A0 = c.S6 + c.A0;
        mem.WriteU32((c.S0 + 0x50u), c.A0);
    }

    static void After80024964(CpuContext c, PSMemory mem)
    {
        c.V1 = mem.ReadU16((c.S0 + 0x4Eu));
        c.V0 = c.V1 << 1;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 1;
        c.V0 = c.S6 + c.V0;
        mem.WriteU32((c.S0 + 0x50u), c.V0);
    }

    static void After80024980(CpuContext c, PSMemory mem)
    {
        c.T0 = 0x801A0000u;
        c.V0 = mem.ReadU8((c.S0 + 0x48u));
        c.T0 = c.T0 - 0x70D8u;
        c.A1 = c.V0 << 11;
        c.V0 = mem.ReadU32((c.S0 + 0x24u));
        c.V1 = mem.ReadU32((c.T0 + 0x248u));
        c.V0 = c.V0 + c.A1;
        c.A0 = c.V1 - c.V0;
    }

    static void After800249AC(CpuContext c, PSMemory mem)
    {
        c.A0 = c.A1 - c.A0;
        c.T0 = 0x801A0000u;
        c.V0 = mem.ReadU8((c.S0 + 0x49u));
        c.T0 = c.T0 - 0x70D8u;
        c.A1 = c.V0 << 11;
        c.V0 = mem.ReadU32((c.S0 + 0x2Cu));
        c.V1 = mem.ReadU32((c.T0 + 0x250u));
        c.V0 = c.V0 + c.A1;
        c.V1 = c.V1 - c.V0;
    }

    static void After80024A60(CpuContext c, PSMemory mem)
    {
        c.V0 = (int)c.S1 < 20 ? 1u : 0u;
        c.T0 = 0x801A0000u;
        c.T0 = c.T0 - 0x70D8u;
        c.V1 = mem.ReadU32((c.T0 + 0x24Cu));
        c.V0 = mem.ReadU32((c.S0 + 0x28u));
        c.V1 = c.V1 - c.V0;
    }

    static void After80024A98(CpuContext c, PSMemory mem)
    {
        c.V0 = mem.ReadU8((c.S0 + 0x4Bu));
        c.LO = (uint)((long)(int)c.V0 * (int)c.V1); c.HI = (uint)(((long)(int)c.V0 * (int)c.V1) >> 32);
        c.T0 = c.LO;
        c.V1 = (uint)((int)c.T0 >> 13);
        c.S1 = c.S1 - c.V1;
        c.V0 = (int)c.S1 < 20 ? 1u : 0u;
    }

    static void After80024AEC(CpuContext c, PSMemory mem)
    {
        c.V1 = mem.ReadU16((c.S0 + 0x4Eu));
        c.A0 = (uint)(short)mem.ReadU16((c.S0 + 0x54u));
        c.V0 = c.V1 << 1;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 1;
        c.V0 = c.S6 + c.V0;
    }

    static void After80024B18(CpuContext c, PSMemory mem)
    {
        c.RA = 0x80024B74u;
        ShadowTower_st.func_80078AC4(c, mem);
        c.V1 = mem.ReadU16((c.S0 + 0x4Eu));
        c.V0 = (uint)((int)c.V0 >> 11);
        c.A0 = c.V1 << 4;
        c.A0 = c.A0 - c.V1;
        c.A0 = c.A0 << 2;
        c.A0 = c.A0 + c.V0;
        c.A0 = c.S6 + c.A0;
        mem.WriteU32((c.S0 + 0x50u), c.A0);
    }

    static void After80024B18_2(CpuContext c, PSMemory mem)
    {
        c.A2 = mem.ReadU8((c.S0 + 0x48u));
        c.A3 = mem.ReadU8((c.S0 + 0x49u));
        c.T0 = 0u | 0x8000u;
        mem.WriteU32((c.SP + 0x10u), c.T0);
        c.A0 = mem.ReadU32((c.S0 + 0x24u));
        c.A1 = mem.ReadU32((c.S0 + 0x2Cu));
        c.A2 = c.A2 << 8;
        c.A3 = c.A3 << 8;
        c.RA = 0x80024B3Cu;
        ShadowTower_st.func_8004FD28(c, mem);
    }

    static void After80024B94(CpuContext c, PSMemory mem)
    {
        c.RA = 0x80024BB0u;
        ShadowTower_st.func_80078AC4(c, mem);
        c.V1 = mem.ReadU16((c.S0 + 0x4Eu));
        c.V0 = (uint)((int)c.V0 >> 9);
        c.A0 = c.V1 << 4;
        c.A0 = c.A0 - c.V1;
        c.A0 = c.A0 << 2;
        c.A0 = c.A0 + c.V0;
        c.A0 = c.S6 + c.A0;
        mem.WriteU32((c.S0 + 0x50u), c.A0);
    }

    static void After80024BD4(CpuContext c, PSMemory mem)
    {
        c.V1 = mem.ReadU16((c.S0 + 0x4Eu));
        c.V0 = c.V1 << 4;
        c.V0 = c.V0 - c.V1;
        c.V0 = c.V0 << 2;
        c.V0 = c.S6 + c.V0;
        mem.WriteU32((c.S0 + 0x50u), c.V0);
    }

    static void After80024BF0(CpuContext c, PSMemory mem)
    {
        c.V0 = mem.ReadU8((c.S0 + 0x48u));
        c.V1 = mem.ReadU8((c.S0 + 0x4Cu));
        c.A2 = mem.ReadU32((c.S0 + 0x1Cu));
        c.V0 = c.V0 << 7;
        c.A1 = c.V0;
        c.V1 = c.V1 << 7;
        c.A0 = c.A1 - c.V1;
        c.V0 = (int)c.A2 < (int)c.A0 ? 1u : 0u;
    }

    static void After80024C70(CpuContext c, PSMemory mem)
    {
        c.V0 = (int)c.S1 < 20 ? 1u : 0u;
        c.T0 = 0x801A0000u;
        c.T0 = c.T0 - 0x70D8u;
        c.V1 = mem.ReadU32((c.T0 + 0x24Cu));
        c.V0 = mem.ReadU32((c.S0 + 0x28u));
        c.V1 = c.V1 - c.V0;
    }

    static void After80024CA8(CpuContext c, PSMemory mem)
    {
        c.V0 = mem.ReadU8((c.S0 + 0x4Bu));
        c.LO = (uint)((long)(int)c.V0 * (int)c.V1); c.HI = (uint)(((long)(int)c.V0 * (int)c.V1) >> 32);
        c.T0 = c.LO;
        c.V1 = (uint)((int)c.T0 >> 13);
        c.S1 = c.S1 - c.V1;
        c.V0 = (int)c.S1 < 20 ? 1u : 0u;
    }

    static void After80024E2C_3(CpuContext c, PSMemory mem)
    {
        c.S1 = c.S0 + 0x3Cu;
        c.V0 = mem.ReadU16((c.S0 + 0x34u));
        mem.WriteU16((c.S4 + 0xD8u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S0 + 0x36u));
        mem.WriteU16((c.S4 + 0xDAu), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S0 + 0x38u));
        mem.WriteU16((c.S4 + 0xDCu), (ushort)c.V0);
        c.V0 = mem.ReadU8((c.S0 + 0x4u));
        c.V0 = c.V0 & 0x0040u;
    }

    static void After80024F54_2(CpuContext c, PSMemory mem)
    {
        c.V0 = mem.ReadU16((c.S2 + 0xCu));
        c.V1 = c.V0 << 1;
        c.V0 = c.V0 >> 1;
        c.V1 = c.V1 + c.V0;
        mem.WriteU16((c.SP + 0x60u), (ushort)c.V1);
        c.V0 = mem.ReadU16((c.S2 + 0xEu));
        c.V1 = c.V0 << 1;
    }

    static void After80024F54_3(CpuContext c, PSMemory mem)
    {
        c.V0 = mem.ReadU32((c.S0 + 0x24u));
        c.V1 = mem.ReadU32((c.S0 + 0x28u));
        c.A0 = mem.ReadU32((c.S0 + 0x2Cu));
        c.A1 = mem.ReadU32((c.S0 + 0x30u));
        mem.WriteU32((c.SP + 0x50u), c.V0);
        mem.WriteU32((c.SP + 0x54u), c.V1);
        mem.WriteU32((c.SP + 0x58u), c.A0);
        mem.WriteU32((c.SP + 0x5Cu), c.A1);
        c.V0 = mem.ReadU8((c.S0 + 0x4u));
        c.V0 = c.V0 & 0x0004u;
    }

    static void After80025080(CpuContext c, PSMemory mem)
    {
        c.V0 = mem.ReadU16((c.S2 + 0x4u));
        c.V1 = c.V0 << 1;
        c.V0 = c.V0 >> 1;
        c.V1 = c.V1 + c.V0;
        mem.WriteU16((c.SP + 0x60u), (ushort)c.V1);
        c.V0 = mem.ReadU16((c.S2 + 0x4u));
        c.V1 = c.V0 << 1;
    }

    static void After80025168(CpuContext c, PSMemory mem)
    {
        c.V0 = c.S4 + 0x224u;
        c.A0 = 0u;
        c.A1 = c.A0;
        c.A2 = 0x00000100u;
        c.A3 = 0x000000F9u;
        c.RA = 0x8002518Cu;
        ShadowTower_st.func_80024004(c, mem);
        c.V0 = c.S4 + 0x224u;
    }
}
