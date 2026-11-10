using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary><see cref="ModelDraw"/>'s straight-line runs, moved out whole so neither file
/// passes the length rule (scripts/split_literal.py). Each is named after the label
/// before it.</summary>
public static partial class ModelDraw
{
    static void After80022764(CpuContext c, PSMemory mem)
    {
        c.A1 = 0x801A0000u;
        c.A1 = c.A1 - 0x70D8u;
        c.V0 = mem.ReadU32(c.S1);
        c.A0 = mem.ReadU32((c.A1 + 0x248u));
        c.A2 = (uint)(short)mem.ReadU16((c.A1 + 0x2CEu));
        c.V1 = mem.ReadU32((c.A1 + 0x24Cu));
        c.V0 = c.V0 - c.A0;
        c.V1 = c.V1 + c.A2;
        mem.WriteU32((c.SP + 0x18u), c.V0);
        c.A0 = (uint)(short)mem.ReadU16((c.A1 + 0x2D2u));
        c.V0 = mem.ReadU32((c.S1 + 0x4u));
        c.V1 = c.V1 + c.A0;
        c.V0 = c.V0 - c.V1;
        mem.WriteU32((c.SP + 0x1Cu), c.V0);
        c.V0 = mem.ReadU32((c.S1 + 0x8u));
        c.V1 = mem.ReadU32((c.A1 + 0x250u));
        c.V0 = c.V0 - c.V1;
        mem.WriteU32((c.SP + 0x20u), c.V0);
        c.A2 = mem.ReadU16((c.A1 + 0x2CEu));
        c.V0 = mem.ReadU16(c.S1);
        c.V1 = mem.ReadU16((c.A1 + 0x248u));
        c.A0 = mem.ReadU16((c.A1 + 0x24Cu));
        c.V0 = c.V0 - c.V1;
        c.A0 = c.A0 + c.A2;
        mem.WriteU16((c.SP + 0x10u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.A1 + 0x2D2u));
        c.V1 = mem.ReadU16((c.S1 + 0x4u));
        c.V0 = c.V0 + c.A0;
        c.V1 = c.V1 - c.V0;
        mem.WriteU16((c.SP + 0x12u), (ushort)c.V1);
        c.V0 = mem.ReadU16((c.S1 + 0x8u));
        c.V1 = mem.ReadU16((c.A1 + 0x250u));
        c.V0 = c.V0 - c.V1;
        mem.WriteU16((c.SP + 0x14u), (ushort)c.V0);
        c.V0 = c.SP + 0x10u;
        RecompOne.Runtime.Gte.Write(0, (mem.ReadU32(c.V0)));
        RecompOne.Runtime.Gte.Write(1, (mem.ReadU32((c.V0 + 0x4u))));
        RecompOne.Runtime.Gte.MvmvaOp(12, false, 0, 0, 0);
        c.T3 = 0x1F800000u;
        c.T3 = c.T3 | 0x00BCu;
        mem.WriteU32(c.T3, (RecompOne.Runtime.Gte.Read(25)));
        mem.WriteU32((c.T3 + 0x4u), (RecompOne.Runtime.Gte.Read(26)));
        mem.WriteU32((c.T3 + 0x8u), (RecompOne.Runtime.Gte.Read(27)));
        c.V0 = c.SP + 0x98u;
        c.T4 = RecompOne.Runtime.Gte.ReadControl(31);
        mem.WriteU32(c.V0, c.T4);
        c.S1 = c.S0 + 0xA8u;
    }

    static void After80022764_2(CpuContext c, PSMemory mem)
    {
        c.T3 = 0x80180000u;
        c.T3 = c.T3 - 0x4288u;
        c.T4 = mem.ReadU32(c.T3);
        c.T5 = mem.ReadU32((c.T3 + 0x4u));
        RecompOne.Runtime.Gte.WriteControl(0, c.T4);
        RecompOne.Runtime.Gte.WriteControl(1, c.T5);
        c.T4 = mem.ReadU32((c.T3 + 0x8u));
        c.T5 = mem.ReadU32((c.T3 + 0xCu));
        c.T6 = mem.ReadU32((c.T3 + 0x10u));
        RecompOne.Runtime.Gte.WriteControl(2, c.T4);
        RecompOne.Runtime.Gte.WriteControl(3, c.T5);
        RecompOne.Runtime.Gte.WriteControl(4, c.T6);
        c.T3 = 0x80180000u;
        c.T3 = c.T3 - 0x4288u;
        c.T4 = mem.ReadU32((c.T3 + 0x14u));
        c.T5 = mem.ReadU32((c.T3 + 0x18u));
        RecompOne.Runtime.Gte.WriteControl(5, c.T4);
        c.T6 = mem.ReadU32((c.T3 + 0x1Cu));
        RecompOne.Runtime.Gte.WriteControl(6, c.T5);
        RecompOne.Runtime.Gte.WriteControl(7, c.T6);
    }

    static void After800228B4(CpuContext c, PSMemory mem)
    {
        c.A0 = mem.ReadU32((c.SP + 0xDCu));
        c.A1 = c.S1;
        c.RA = 0x800228C0u;
        ShadowTower_st.func_80049E30(c, mem);
        c.A0 = c.S2 << 16;
        c.A0 = (uint)((int)c.A0 >> 16);
        c.S6 = c.S4 & 0x00FFu;
        c.A1 = c.S6;
        c.RA = 0x800228D4u;
        ShadowTower_st.func_80017D28(c, mem);
        c.T3 = mem.ReadU32((c.S0 + 0x394u));
        c.T4 = mem.ReadU32(c.T3);
        c.T5 = mem.ReadU32((c.T3 + 0x4u));
        RecompOne.Runtime.Gte.WriteControl(0, c.T4);
        RecompOne.Runtime.Gte.WriteControl(1, c.T5);
        c.T4 = mem.ReadU32((c.T3 + 0x8u));
        c.T5 = mem.ReadU32((c.T3 + 0xCu));
        c.T6 = mem.ReadU32((c.T3 + 0x10u));
        RecompOne.Runtime.Gte.WriteControl(2, c.T4);
        RecompOne.Runtime.Gte.WriteControl(3, c.T5);
        RecompOne.Runtime.Gte.WriteControl(4, c.T6);
        c.T4 = mem.ReadU16(c.S1);
        c.T5 = mem.ReadU16((c.S1 + 0x6u));
        c.T6 = mem.ReadU16((c.S1 + 0xCu));
        RecompOne.Runtime.Gte.Write(9, c.T4);
        RecompOne.Runtime.Gte.Write(10, c.T5);
        RecompOne.Runtime.Gte.Write(11, c.T6);
        RecompOne.Runtime.Gte.MvmvaOp(12, false, 0, 3, 3);
        c.V1 = c.SP + 0x58u;
        c.T4 = RecompOne.Runtime.Gte.Read(9);
        c.T5 = RecompOne.Runtime.Gte.Read(10);
        c.T6 = RecompOne.Runtime.Gte.Read(11);
        mem.WriteU16(c.V1, (ushort)c.T4);
        mem.WriteU16((c.V1 + 0x6u), (ushort)c.T5);
        mem.WriteU16((c.V1 + 0xCu), (ushort)c.T6);
        c.S3 = c.S0 + 0xAAu;
        c.T4 = mem.ReadU16(c.S3);
        c.T5 = mem.ReadU16((c.S3 + 0x6u));
        c.T6 = mem.ReadU16((c.S3 + 0xCu));
        RecompOne.Runtime.Gte.Write(9, c.T4);
        RecompOne.Runtime.Gte.Write(10, c.T5);
        RecompOne.Runtime.Gte.Write(11, c.T6);
        RecompOne.Runtime.Gte.MvmvaOp(12, false, 0, 3, 3);
        c.V0 = c.SP + 0x5Au;
        c.T4 = RecompOne.Runtime.Gte.Read(9);
        c.T5 = RecompOne.Runtime.Gte.Read(10);
        c.T6 = RecompOne.Runtime.Gte.Read(11);
        mem.WriteU16(c.V0, (ushort)c.T4);
        mem.WriteU16((c.V0 + 0x6u), (ushort)c.T5);
        mem.WriteU16((c.V0 + 0xCu), (ushort)c.T6);
        c.S2 = c.S0 + 0xACu;
        c.T4 = mem.ReadU16(c.S2);
        c.T5 = mem.ReadU16((c.S2 + 0x6u));
        c.T6 = mem.ReadU16((c.S2 + 0xCu));
        RecompOne.Runtime.Gte.Write(9, c.T4);
        RecompOne.Runtime.Gte.Write(10, c.T5);
        RecompOne.Runtime.Gte.Write(11, c.T6);
        RecompOne.Runtime.Gte.MvmvaOp(12, false, 0, 3, 3);
        c.V0 = c.SP + 0x5Cu;
        c.T4 = RecompOne.Runtime.Gte.Read(9);
        c.T5 = RecompOne.Runtime.Gte.Read(10);
        c.T6 = RecompOne.Runtime.Gte.Read(11);
        mem.WriteU16(c.V0, (ushort)c.T4);
        mem.WriteU16((c.V0 + 0x6u), (ushort)c.T5);
        mem.WriteU16((c.V0 + 0xCu), (ushort)c.T6);
        c.T4 = mem.ReadU32(c.V1);
        c.T5 = mem.ReadU32((c.V1 + 0x4u));
        RecompOne.Runtime.Gte.WriteControl(8, c.T4);
        RecompOne.Runtime.Gte.WriteControl(9, c.T5);
        c.T4 = mem.ReadU32((c.V1 + 0x8u));
        c.T5 = mem.ReadU32((c.V1 + 0xCu));
        c.T6 = mem.ReadU32((c.V1 + 0x10u));
        RecompOne.Runtime.Gte.WriteControl(10, c.T4);
        RecompOne.Runtime.Gte.WriteControl(11, c.T5);
        RecompOne.Runtime.Gte.WriteControl(12, c.T6);
    }

    static void After80022A24(CpuContext c, PSMemory mem)
    {
        c.T4 = mem.ReadU32(c.FP);
        c.T5 = mem.ReadU32((c.FP + 0x4u));
        RecompOne.Runtime.Gte.WriteControl(0, c.T4);
        RecompOne.Runtime.Gte.WriteControl(1, c.T5);
        c.T4 = mem.ReadU32((c.FP + 0x8u));
        c.T5 = mem.ReadU32((c.FP + 0xCu));
        c.T6 = mem.ReadU32((c.FP + 0x10u));
        RecompOne.Runtime.Gte.WriteControl(2, c.T4);
        RecompOne.Runtime.Gte.WriteControl(3, c.T5);
        RecompOne.Runtime.Gte.WriteControl(4, c.T6);
        c.T4 = mem.ReadU16(c.S1);
        c.T5 = mem.ReadU16((c.S1 + 0x6u));
        c.T6 = mem.ReadU16((c.S1 + 0xCu));
        RecompOne.Runtime.Gte.Write(9, c.T4);
        RecompOne.Runtime.Gte.Write(10, c.T5);
        RecompOne.Runtime.Gte.Write(11, c.T6);
        RecompOne.Runtime.Gte.MvmvaOp(12, false, 0, 3, 3);
        c.T4 = RecompOne.Runtime.Gte.Read(9);
        c.T5 = RecompOne.Runtime.Gte.Read(10);
        c.T6 = RecompOne.Runtime.Gte.Read(11);
        mem.WriteU16(c.S1, (ushort)c.T4);
        mem.WriteU16((c.S1 + 0x6u), (ushort)c.T5);
        mem.WriteU16((c.S1 + 0xCu), (ushort)c.T6);
        c.T4 = mem.ReadU16(c.S3);
        c.T5 = mem.ReadU16((c.S3 + 0x6u));
        c.T6 = mem.ReadU16((c.S3 + 0xCu));
        RecompOne.Runtime.Gte.Write(9, c.T4);
        RecompOne.Runtime.Gte.Write(10, c.T5);
        RecompOne.Runtime.Gte.Write(11, c.T6);
        RecompOne.Runtime.Gte.MvmvaOp(12, false, 0, 3, 3);
        c.T4 = RecompOne.Runtime.Gte.Read(9);
        c.T5 = RecompOne.Runtime.Gte.Read(10);
        c.T6 = RecompOne.Runtime.Gte.Read(11);
        mem.WriteU16(c.S3, (ushort)c.T4);
        mem.WriteU16((c.S3 + 0x6u), (ushort)c.T5);
        mem.WriteU16((c.S3 + 0xCu), (ushort)c.T6);
        c.T4 = mem.ReadU16(c.S2);
        c.T5 = mem.ReadU16((c.S2 + 0x6u));
        c.T6 = mem.ReadU16((c.S2 + 0xCu));
        RecompOne.Runtime.Gte.Write(9, c.T4);
        RecompOne.Runtime.Gte.Write(10, c.T5);
        RecompOne.Runtime.Gte.Write(11, c.T6);
        RecompOne.Runtime.Gte.MvmvaOp(12, false, 0, 3, 3);
        c.T4 = RecompOne.Runtime.Gte.Read(9);
        c.T5 = RecompOne.Runtime.Gte.Read(10);
        c.T6 = RecompOne.Runtime.Gte.Read(11);
        mem.WriteU16(c.S2, (ushort)c.T4);
        mem.WriteU16((c.S2 + 0x6u), (ushort)c.T5);
        mem.WriteU16((c.S2 + 0xCu), (ushort)c.T6);
    }

    static void After80022B08(CpuContext c, PSMemory mem)
    {
        c.T4 = mem.ReadU32(c.S1);
        c.T5 = mem.ReadU32((c.S1 + 0x4u));
        RecompOne.Runtime.Gte.WriteControl(0, c.T4);
        RecompOne.Runtime.Gte.WriteControl(1, c.T5);
        c.T4 = mem.ReadU32((c.S1 + 0x8u));
        c.T5 = mem.ReadU32((c.S1 + 0xCu));
        c.T6 = mem.ReadU32((c.S1 + 0x10u));
        RecompOne.Runtime.Gte.WriteControl(2, c.T4);
        RecompOne.Runtime.Gte.WriteControl(3, c.T5);
        RecompOne.Runtime.Gte.WriteControl(4, c.T6);
        c.T4 = mem.ReadU32((c.S1 + 0x14u));
        c.T5 = mem.ReadU32((c.S1 + 0x18u));
        RecompOne.Runtime.Gte.WriteControl(5, c.T4);
        c.T6 = mem.ReadU32((c.S1 + 0x1Cu));
        RecompOne.Runtime.Gte.WriteControl(6, c.T5);
        RecompOne.Runtime.Gte.WriteControl(7, c.T6);
        c.V1 = 0x80160000u;
        c.A1 = mem.ReadU16((c.SP + 0x9Cu));
        c.V1 = c.V1 - 0x2F70u;
        c.V0 = c.A1 << 2;
        c.V0 = c.V0 + c.V1;
        c.V1 = 0u | 0x834Cu;
        c.V0 = c.V0 + c.V1;
        c.V1 = mem.ReadU32(c.V0);
        c.V0 = mem.ReadU16((c.V1 + 0x6u));
    }

    static void After80022C74(CpuContext c, PSMemory mem)
    {
        c.V0 = c.S4 & 0x0080u;
        c.A3 = mem.ReadU32((c.S0 + 0x40u));
        c.V1 = mem.ReadU32((c.S0 + 0x20u));
        c.V0 = mem.ReadU32((c.S0 + 0x88u));
        c.A0 = mem.ReadU32((c.S0 + 0x20u));
        c.V1 = mem.ReadU32((c.V1 + 0x4u));
        c.T0 = mem.ReadU32((c.S0 + 0x48u));
        c.V0 = c.V0 + c.V1;
        mem.WriteU32((c.S0 + 0x88u), c.V0);
        c.A2 = mem.ReadU32((c.A0 + 0x4u));
        c.V0 = 0xFFFFFFFFu;
        c.A2 = c.A2 - 0x1u;
    }

    static void After80022DC8(CpuContext c, PSMemory mem)
    {
        RecompOne.Runtime.Gte.Write(0, (mem.ReadU32(c.A2)));
        RecompOne.Runtime.Gte.Write(1, (mem.ReadU32((c.A2 + 0x4u))));
        RecompOne.Runtime.Gte.MvmvaOp(12, false, 0, 0, 0);
        mem.WriteU32(c.A3, (RecompOne.Runtime.Gte.Read(25)));
        mem.WriteU32((c.A3 + 0x4u), (RecompOne.Runtime.Gte.Read(26)));
        mem.WriteU32((c.A3 + 0x8u), (RecompOne.Runtime.Gte.Read(27)));
        c.A2 = c.A2 + 0x8u;
        c.V0 = mem.ReadU16((c.S0 + 0x50u));
        c.A0 = c.A0 - 0x1u;
        mem.WriteU16(c.A1, (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S0 + 0x54u));
        c.A1 = c.A1 + 0x8u;
        mem.WriteU16((c.V1 - 0x2u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S0 + 0x58u));
        c.T3 = mem.ReadU16((c.SP + 0x100u));
        mem.WriteU16(c.V1, (ushort)c.T3);
        mem.WriteU16((c.V1 + 0x2u), (ushort)c.V0);
    }

    static void After80022F7C(CpuContext c, PSMemory mem)
    {
        RecompOne.Runtime.Gte.Write(0, (mem.ReadU32(c.A2)));
        RecompOne.Runtime.Gte.Write(1, (mem.ReadU32((c.A2 + 0x4u))));
        RecompOne.Runtime.Gte.MvmvaOp(12, false, 0, 0, 0);
        mem.WriteU32(c.A3, (RecompOne.Runtime.Gte.Read(25)));
        mem.WriteU32((c.A3 + 0x4u), (RecompOne.Runtime.Gte.Read(26)));
        mem.WriteU32((c.A3 + 0x8u), (RecompOne.Runtime.Gte.Read(27)));
        c.A2 = c.A2 + 0x8u;
        c.V0 = mem.ReadU16((c.S0 + 0x50u));
        c.A0 = c.A0 - 0x1u;
        mem.WriteU16(c.A1, (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S0 + 0x54u));
        c.A1 = c.A1 + 0x8u;
        mem.WriteU16((c.V1 - 0x2u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S0 + 0x58u));
        c.T3 = mem.ReadU16((c.SP + 0x100u));
        mem.WriteU16(c.V1, (ushort)c.T3);
        mem.WriteU16((c.V1 + 0x2u), (ushort)c.V0);
    }

    static void After80023120(CpuContext c, PSMemory mem)
    {
        RecompOne.Runtime.Gte.Write(0, (mem.ReadU32(c.A2)));
        RecompOne.Runtime.Gte.Write(1, (mem.ReadU32((c.A2 + 0x4u))));
        RecompOne.Runtime.Gte.MvmvaOp(12, false, 0, 0, 0);
        mem.WriteU32(c.A3, (RecompOne.Runtime.Gte.Read(25)));
        mem.WriteU32((c.A3 + 0x4u), (RecompOne.Runtime.Gte.Read(26)));
        mem.WriteU32((c.A3 + 0x8u), (RecompOne.Runtime.Gte.Read(27)));
        c.A2 = c.A2 + 0x8u;
        c.V0 = mem.ReadU16((c.S0 + 0x50u));
        c.A0 = c.A0 - 0x1u;
        mem.WriteU16(c.A1, (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S0 + 0x54u));
        c.A1 = c.A1 + 0x8u;
        mem.WriteU16((c.V1 - 0x2u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S0 + 0x58u));
        c.T3 = mem.ReadU16((c.SP + 0x100u));
        mem.WriteU16(c.V1, (ushort)c.T3);
        mem.WriteU16((c.V1 + 0x2u), (ushort)c.V0);
    }

    static void After80023188(CpuContext c, PSMemory mem)
    {
        c.V1 = c.V1 - 0x2F70u;
        c.V0 = 0u | 0x8000u;
        c.V1 = c.V1 + c.V0;
        c.A0 = mem.ReadU32((c.V1 + 0x304u));
        c.V0 = mem.ReadU32((c.S0 + 0x10u));
        mem.WriteU32((c.A0 + 0x8u), c.V0);
        c.A0 = mem.ReadU32((c.V1 + 0x304u));
        c.V0 = mem.ReadU32((c.S0 + 0x14u));
        mem.WriteU32((c.A0 + 0x4u), c.V0);
        c.V0 = mem.ReadU32((c.S0 + 0x88u));
        c.A0 = mem.ReadU32((c.S0 + 0x8Cu));
        c.A1 = mem.ReadU32((c.S0 + 0x90u));
        c.A2 = mem.ReadU32((c.S0 + 0x94u));
        mem.WriteU32((c.V1 + 0x31Cu), c.V0);
        mem.WriteU32((c.V1 + 0x320u), c.A0);
        mem.WriteU32((c.V1 + 0x324u), c.A1);
        mem.WriteU32((c.V1 + 0x328u), c.A2);
        c.RA = mem.ReadU32((c.SP + 0xCCu));
        c.FP = mem.ReadU32((c.SP + 0xC8u));
        c.S7 = mem.ReadU32((c.SP + 0xC4u));
        c.S6 = mem.ReadU32((c.SP + 0xC0u));
        c.S5 = mem.ReadU32((c.SP + 0xBCu));
        c.S4 = mem.ReadU32((c.SP + 0xB8u));
        c.S3 = mem.ReadU32((c.SP + 0xB4u));
        c.S2 = mem.ReadU32((c.SP + 0xB0u));
        c.S1 = mem.ReadU32((c.SP + 0xACu));
        c.S0 = mem.ReadU32((c.SP + 0xA8u));
        c.SP = c.SP + 0xD0u;
    }

    static void AfterEntry(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0xD0u;
        mem.WriteU32((c.SP + 0xACu), c.S1);
        c.S1 = c.A2;
        mem.WriteU32((c.SP + 0xA8u), c.S0);
        c.S0 = 0x1F800000u;
        c.A0 = 0x80160000u;
        mem.WriteU32((c.SP + 0xC4u), c.S7);
        c.S7 = mem.ReadU32((c.SP + 0xE0u));
        c.V1 = 0x80160000u;
        mem.WriteU32((c.SP + 0xC8u), c.FP);
        c.FP = mem.ReadU32((c.SP + 0xE8u));
        c.T2 = mem.ReadU32((c.SP + 0xFCu));
        c.V1 = c.V1 - 0x2F70u;
        mem.WriteU32((c.SP + 0xBCu), c.S5);
        c.S5 = mem.ReadU16((c.SP + 0xECu));
        c.T3 = mem.ReadU16((c.SP + 0xF0u));
        c.V0 = 0u | 0x8000u;
        mem.WriteU32((c.SP + 0xB0u), c.S2);
        c.S2 = mem.ReadU16((c.SP + 0xF4u));
        c.V1 = c.V1 + c.V0;
        mem.WriteU32((c.SP + 0xCCu), c.RA);
        mem.WriteU32((c.SP + 0xC0u), c.S6);
        mem.WriteU32((c.SP + 0xB8u), c.S4);
        mem.WriteU32((c.SP + 0xB4u), c.S3);
        mem.WriteU32((c.SP + 0xDCu), c.A3);
        mem.WriteU16((c.SP + 0xA0u), (ushort)c.T3);
        c.A2 = mem.ReadU32((c.V1 + 0x31Cu));
        c.A3 = mem.ReadU32((c.V1 + 0x320u));
        c.T0 = mem.ReadU32((c.V1 + 0x324u));
        c.T1 = mem.ReadU32((c.V1 + 0x328u));
        c.V0 = mem.ReadU32((c.V1 + 0x304u));
        c.A0 = c.A0 + 0x7BCCu;
        mem.WriteU32((c.S0 + 0x40u), c.A0);
        mem.WriteU32((c.S0 + 0x44u), c.A0);
        mem.WriteU32((c.S0 + 0x88u), c.A2);
        mem.WriteU32((c.S0 + 0x8Cu), c.A3);
        mem.WriteU32((c.S0 + 0x90u), c.T0);
        mem.WriteU32((c.S0 + 0x94u), c.T1);
        c.A0 = mem.ReadU32((c.V0 + 0x8u));
        mem.WriteU32((c.S0 + 0x10u), c.A0);
        c.V1 = mem.ReadU32((c.V0 + 0x4u));
        c.S4 = c.T2;
        c.V0 = c.V1 - 0x7800u;
        c.V0 = c.V0 < c.A0 ? 1u : 0u;
        mem.WriteU32((c.S0 + 0x14u), c.V1);
    }
}
