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
    static void After80024784(CpuContext c, PSMemory mem)
    {
        mem.WriteU8((c.S0 + 0x7u), (byte)0u);
        c.V1 = 0x00010000u;
        c.V1 = c.V1 | 0x8000u;
        c.T0 = 0x80160000u;
        c.T0 = c.T0 - 0x2F70u;
        c.V0 = mem.ReadU8((c.S0 + 0x3u));
        c.A1 = c.T0 + c.V1;
        c.A0 = c.V0 & 0x003Fu;
        mem.WriteU8((c.S0 + 0x3u), (byte)c.A0);
        c.V0 = mem.ReadU32((c.A1 + 0x6E04u));
        c.LO = (uint)((long)(int)c.V0 * (int)c.V0); c.HI = (uint)(((long)(int)c.V0 * (int)c.V0) >> 32);
        c.V1 = mem.ReadU32((c.S0 + 0x18u));
        c.T0 = c.LO;
        c.V0 = c.T0 < c.V1 ? 1u : 0u;
    }

    static void After80024784_2(CpuContext c, PSMemory mem)
    {
        c.T0 = 0x801C0000u;
        c.T0 = c.T0 + 0x310u;
        c.V0 = c.V0 + c.T0;
        c.T0 = 0x801C0000u;
        c.V1 = mem.ReadU16((c.V0 + 0x71B4u));
        c.T0 = c.T0 + 0x2230u;
        c.V0 = c.V1 << 1;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 2;
        c.V0 = c.V0 - c.V1;
        c.V0 = c.V0 << 3;
        c.S0 = c.V0 + c.T0;
        c.V1 = mem.ReadU8(c.S0);
        c.V0 = 0x00000002u;
    }

    static void After80024D44(CpuContext c, PSMemory mem)
    {
        c.T0 = 0x80160000u;
        c.V0 = mem.ReadU16((c.S0 + 0x34u));
        mem.WriteU16((c.S4 + 0xD8u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S0 + 0x36u));
        mem.WriteU16((c.S4 + 0xDAu), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S0 + 0x38u));
        c.T0 = c.T0 + 0x5090u;
        mem.WriteU16((c.S4 + 0xDCu), (ushort)c.V0);
        c.V0 = (uint)(short)mem.ReadU16((c.S0 + 0x14u));
        c.V1 = mem.ReadU32((c.T0 + 0xE4u));
        c.V0 = c.V0 << 2;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32(c.S4, c.V1);
        c.V1 = (uint)(short)mem.ReadU16((c.S0 + 0x14u));
        c.V0 = 0x00000004u;
        c.V0 = c.V0 - c.V1;
        mem.WriteU32((c.S4 + 0x80u), c.V0);
        c.V1 = (uint)(short)mem.ReadU16((c.S0 + 0x14u));
        c.V0 = c.S0 + 0x44u;
        mem.WriteU32((c.SP + 0x14u), c.V0);
        c.V0 = 0x80180000u;
        c.V0 = c.V0 - 0x4288u;
        mem.WriteU32((c.SP + 0x18u), c.V0);
        c.V0 = 0x00001000u;
        mem.WriteU32((c.SP + 0x10u), c.S1);
        c.V0 = c.V0 - c.V1;
        mem.WriteU32((c.S4 + 0x4u), c.V0);
        c.V0 = mem.ReadU8((c.S0 + 0x2u));
        c.A0 = 0u;
        mem.WriteU32((c.SP + 0x1Cu), c.V0);
        c.V0 = mem.ReadU16((c.S0 + 0x10u));
        c.A1 = c.S2;
        mem.WriteU32((c.SP + 0x24u), 0u);
        mem.WriteU32((c.SP + 0x20u), c.V0);
        c.V0 = (uint)(short)mem.ReadU16((c.S0 + 0x16u));
        c.A2 = c.S0 + 0x24u;
        mem.WriteU32((c.SP + 0x28u), c.V0);
        c.V0 = mem.ReadU8((c.S0 + 0x3u));
        c.A3 = c.S4 + 0xD8u;
        mem.WriteU32((c.SP + 0x30u), 0u);
        mem.WriteU32((c.SP + 0x2Cu), c.V0);
    }

    static void After80024D44_2(CpuContext c, PSMemory mem)
    {
        c.V0 = mem.ReadU16((c.S0 + 0xAu));
        c.V1 = 0x00000001u;
        c.V0 = c.S4 + c.V0;
        mem.WriteU8((c.V0 + 0xF8u), (byte)c.V1);
        c.A0 = mem.ReadU16((c.S0 + 0xAu));
        c.S1 = c.S0 + 0x3Cu;
        c.RA = 0x80024D60u;
        ShadowTower_st.func_80023208(c, mem);
        c.S2 = c.V0 & 0xFFFFu;
        c.A0 = c.S2;
        c.A1 = c.SP + 0x48u;
        c.RA = 0x80024D70u;
        ShadowTower_st.func_80033DB8(c, mem);
        c.V0 = mem.ReadU32((c.SP + 0x48u));
    }

    static void After80024E2C(CpuContext c, PSMemory mem)
    {
        c.T0 = 0x80160000u;
        c.V0 = mem.ReadU16((c.S0 + 0x34u));
        mem.WriteU16((c.S4 + 0xDAu), (ushort)0u);
        mem.WriteU16((c.S4 + 0xD8u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S0 + 0x38u));
        c.T0 = c.T0 + 0x5090u;
        mem.WriteU16((c.S4 + 0xDCu), (ushort)c.V0);
        c.V0 = (uint)(short)mem.ReadU16((c.S0 + 0x14u));
        c.V1 = mem.ReadU32((c.T0 + 0xE4u));
        c.V0 = c.V0 << 2;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32(c.S4, c.V1);
        c.V1 = (uint)(short)mem.ReadU16((c.S0 + 0x14u));
        c.V0 = 0x00000004u;
        c.V0 = c.V0 - c.V1;
        mem.WriteU32((c.S4 + 0x80u), c.V0);
        c.V1 = (uint)(short)mem.ReadU16((c.S0 + 0x14u));
        c.V0 = c.S0 + 0x44u;
        mem.WriteU32((c.SP + 0x14u), c.V0);
        c.V0 = 0x80180000u;
        c.V0 = c.V0 - 0x41C8u;
        mem.WriteU32((c.SP + 0x18u), c.V0);
        c.V0 = 0x00001000u;
        mem.WriteU32((c.SP + 0x10u), c.S1);
        c.V0 = c.V0 - c.V1;
        mem.WriteU32((c.S4 + 0x4u), c.V0);
        c.V0 = mem.ReadU8((c.S0 + 0x2u));
        mem.WriteU32((c.SP + 0x1Cu), c.V0);
        c.V0 = mem.ReadU16((c.S0 + 0x10u));
        c.A0 = 0u;
        mem.WriteU32((c.SP + 0x20u), c.V0);
        c.V0 = (uint)(short)mem.ReadU16((c.S0 + 0x16u));
        c.A1 = c.S3;
        mem.WriteU32((c.SP + 0x24u), c.V0);
        c.V0 = (uint)(short)mem.ReadU16((c.S0 + 0x16u));
        c.A2 = c.S0 + 0x24u;
        mem.WriteU32((c.SP + 0x28u), c.V0);
        c.V0 = mem.ReadU8((c.S0 + 0x3u));
        c.A3 = c.S4 + 0xD8u;
        mem.WriteU32((c.SP + 0x30u), 0u);
        mem.WriteU32((c.SP + 0x2Cu), c.V0);
    }

    static void After80024E2C_2(CpuContext c, PSMemory mem)
    {
        c.V1 = mem.ReadU16((c.S0 + 0xAu));
        c.T0 = c.T0 + 0x310u;
        c.V0 = c.S4 + c.V1;
        mem.WriteU8((c.V0 + 0xF8u), (byte)c.S5);
        c.V0 = c.V1 << 1;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 3;
        c.A0 = mem.ReadU16((c.S0 + 0xAu));
        c.S2 = c.V0 + c.T0;
        c.RA = 0x80024E54u;
        ShadowTower_st.func_80023208(c, mem);
        c.S3 = c.V0 & 0xFFFFu;
        c.A0 = c.S3;
        c.A1 = c.SP + 0x48u;
        c.RA = 0x80024E64u;
        ShadowTower_st.func_80033DB8(c, mem);
        c.V0 = mem.ReadU32((c.SP + 0x48u));
    }

    static void After80024F54(CpuContext c, PSMemory mem)
    {
        c.V0 = 0x80080000u;
        c.A2 = c.V0 + 0x7F04u;
        c.V1 = mem.ReadWordLeft(c.V1, (c.A2 + 0x3u));
        c.V1 = mem.ReadWordRight(c.V1, c.A2);
        c.A0 = mem.ReadWordLeft(c.A0, (c.A2 + 0x7u));
        c.A0 = mem.ReadWordRight(c.A0, (c.A2 + 0x4u));
        mem.WriteWordLeft((c.SP + 0x63u), c.V1);
        mem.WriteWordRight((c.SP + 0x60u), c.V1);
        mem.WriteWordLeft((c.SP + 0x67u), c.A0);
        mem.WriteWordRight((c.SP + 0x64u), c.A0);
        c.V0 = (uint)(short)mem.ReadU16((c.S0 + 0x14u));
        c.T0 = c.T0 + 0x5090u;
        mem.WriteU8((c.S0 + 0x7u), (byte)c.S5);
        c.V1 = mem.ReadU32((c.T0 + 0xE4u));
        c.V0 = c.V0 << 2;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32(c.S4, c.V1);
        c.V1 = (uint)(short)mem.ReadU16((c.S0 + 0x14u));
        c.V0 = 0x00000004u;
        c.V0 = c.V0 - c.V1;
        mem.WriteU32((c.S4 + 0x80u), c.V0);
        c.V1 = (uint)(short)mem.ReadU16((c.S0 + 0x14u));
        c.V0 = c.S0 + 0x44u;
        mem.WriteU32((c.SP + 0x14u), c.V0);
        c.V0 = 0x80180000u;
        c.V0 = c.V0 - 0x4288u;
        mem.WriteU32((c.SP + 0x18u), c.V0);
        c.V0 = 0x00001000u;
        mem.WriteU32((c.SP + 0x10u), c.S1);
        c.V0 = c.V0 - c.V1;
        mem.WriteU32((c.S4 + 0x4u), c.V0);
        c.V0 = mem.ReadU8((c.S0 + 0x2u));
        mem.WriteU32((c.SP + 0x1Cu), c.V0);
        c.V0 = mem.ReadU16((c.S0 + 0x10u));
        c.A0 = 0u;
        mem.WriteU32((c.SP + 0x20u), c.V0);
        c.V0 = (uint)(short)mem.ReadU16((c.S0 + 0x16u));
        c.A1 = c.S3;
        mem.WriteU32((c.SP + 0x24u), c.V0);
        c.V0 = (uint)(short)mem.ReadU16((c.S0 + 0x16u));
        c.A2 = c.S0 + 0x24u;
        mem.WriteU32((c.SP + 0x28u), c.V0);
        c.V0 = mem.ReadU8((c.S0 + 0x3u));
        c.A3 = c.S4 + 0xD8u;
        mem.WriteU32((c.SP + 0x30u), 0u);
        mem.WriteU32((c.SP + 0x2Cu), c.V0);
        c.RA = 0x80025014u;
        ShadowTower_st.func_80022678(c, mem);
        c.V0 = mem.ReadU8((c.S0 + 0x4u));
        c.V0 = c.V0 & 0x0001u;
    }

    static void After800250A4(CpuContext c, PSMemory mem)
    {
        c.V0 = c.V0 >> 1;
        c.V1 = c.V1 + c.V0;
        mem.WriteU16((c.SP + 0x64u), (ushort)c.V1);
        c.A0 = 0u;
        c.A2 = c.SP + 0x50u;
        c.A3 = c.S4 + 0xD8u;
        c.T0 = 0x80160000u;
        c.T0 = c.T0 + 0x5090u;
        c.V1 = (uint)(short)mem.ReadU16((c.S0 + 0x14u));
        c.V0 = mem.ReadU32((c.T0 + 0xE4u));
        c.V1 = c.V1 << 2;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 - 0x14u;
        mem.WriteU32(c.S4, c.V0);
        c.V1 = (uint)(short)mem.ReadU16((c.S0 + 0x14u));
        c.V0 = 0x00000009u;
        c.V0 = c.V0 - c.V1;
        mem.WriteU32((c.S4 + 0x80u), c.V0);
        c.V1 = (uint)(short)mem.ReadU16((c.S0 + 0x14u));
        c.V0 = 0x00001005u;
        c.V0 = c.V0 - c.V1;
        mem.WriteU32((c.S4 + 0x4u), c.V0);
        c.A1 = mem.ReadU8((c.S2 + 0x3u));
        c.V0 = c.SP + 0x60u;
        mem.WriteU32((c.SP + 0x10u), c.V0);
        c.V0 = c.S0 + 0x44u;
        mem.WriteU32((c.SP + 0x14u), c.V0);
        c.V0 = 0x80180000u;
        c.V0 = c.V0 - 0x4288u;
        mem.WriteU32((c.SP + 0x18u), c.V0);
        c.V0 = 0x00000080u;
        mem.WriteU32((c.SP + 0x1Cu), c.V0);
        mem.WriteU32((c.SP + 0x20u), 0u);
        c.V1 = (uint)(short)mem.ReadU16((c.S0 + 0x16u));
        c.V0 = 0x00001000u;
        mem.WriteU32((c.SP + 0x28u), c.V0);
        c.V0 = 0x00000004u;
        mem.WriteU32((c.SP + 0x2Cu), c.V0);
        mem.WriteU32((c.SP + 0x30u), 0u);
        c.A1 = c.A1 >> 2;
        c.A1 = c.A1 & 0x0001u;
        c.A1 = c.A1 | 0x002Cu;
        mem.WriteU32((c.SP + 0x24u), c.V1);
    }

    static void After80025190(CpuContext c, PSMemory mem)
    {
        mem.WriteU32((c.S4 + 0xF0u), c.V0);
        c.RA = mem.ReadU32((c.SP + 0x8Cu));
        c.FP = mem.ReadU32((c.SP + 0x88u));
        c.S7 = mem.ReadU32((c.SP + 0x84u));
        c.S6 = mem.ReadU32((c.SP + 0x80u));
        c.S5 = mem.ReadU32((c.SP + 0x7Cu));
        c.S4 = mem.ReadU32((c.SP + 0x78u));
        c.S3 = mem.ReadU32((c.SP + 0x74u));
        c.S2 = mem.ReadU32((c.SP + 0x70u));
        c.S1 = mem.ReadU32((c.SP + 0x6Cu));
        c.S0 = mem.ReadU32((c.SP + 0x68u));
        c.SP = c.SP + 0x90u;
    }

    static void AfterEntry(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x90u;
        c.V1 = 0x1F800000u;
        c.V1 = c.V1 | 0x00F8u;
        c.V0 = 0x1F800000u;
        c.V0 = c.V0 | 0x0244u;
        c.A0 = 0x1F800000u;
        c.A0 = c.A0 | 0x00F8u;
        c.A1 = 0u;
        c.A2 = 0x00000053u;
        mem.WriteU32((c.SP + 0x78u), c.S4);
        c.S4 = 0x1F800000u;
        mem.WriteU32((c.SP + 0x8Cu), c.RA);
        mem.WriteU32((c.SP + 0x88u), c.FP);
        mem.WriteU32((c.SP + 0x84u), c.S7);
        mem.WriteU32((c.SP + 0x80u), c.S6);
        mem.WriteU32((c.SP + 0x7Cu), c.S5);
        mem.WriteU32((c.SP + 0x74u), c.S3);
        mem.WriteU32((c.SP + 0x70u), c.S2);
        mem.WriteU32((c.SP + 0x6Cu), c.S1);
        mem.WriteU32((c.SP + 0x68u), c.S0);
        mem.WriteU32((c.S4 + 0xF0u), c.V1);
        mem.WriteU32((c.S4 + 0xF4u), c.V0);
        c.RA = 0x80024740u;
        ShadowTower_st.func_80014D5C(c, mem);
        c.A0 = c.S4;
        c.A0 = c.A0 | 0x0244u;
        c.A1 = 0u;
        c.A2 = 0x0000004Bu;
        c.RA = 0x80024754u;
        ShadowTower_st.func_80014D5C(c, mem);
        c.V0 = 0x801C0000u;
        c.A0 = c.V0 + 0x310u;
        c.V0 = 0x80090000u;
        c.FP = mem.ReadU8((c.A0 + 0x7274u));
        c.V1 = 0xFFFFFFFFu;
        mem.WriteU16((c.S4 + 0x98u), (ushort)0u);
        c.S6 = mem.ReadU32((c.V0 - 0x7DFCu));
        c.FP = c.FP - 0x1u;
    }
}
