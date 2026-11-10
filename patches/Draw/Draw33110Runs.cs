using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary><see cref="Draw33110"/>'s straight-line runs, moved out whole so neither file
/// passes the length rule (scripts/split_literal.py). Each is named after the label
/// before it.</summary>
public static partial class Draw33110
{
    static void After800333F0(CpuContext c, PSMemory mem)
    {
        c.V0 = 0x00010000u;
        c.T0 = mem.ReadU32((c.A1 + 0x6E08u));
        c.V1 = mem.ReadU8((c.A1 + 0x6DFDu));
        mem.WriteU8((c.A1 + 0x6DFEu), (byte)0u);
        c.V0 = c.V1 << 3;
        c.V0 = c.V0 - c.V1;
        c.V0 = c.V0 << 2;
        c.V0 = c.V0 - c.V1;
        c.V0 = c.V0 << 2;
        c.A0 = c.V0 + c.A0;
        c.A3 = c.A0 + 0x6Cu;
        c.V0 = c.A3 | c.T0;
        c.V1 = mem.ReadU8((c.A1 + 0x6E14u));
        c.V0 = c.V0 & 0x0003u;
    }

    static void After8003346C(CpuContext c, PSMemory mem)
    {
        c.V0 = mem.ReadWordLeft(c.V0, (c.A3 + 0x3u));
        c.V0 = mem.ReadWordRight(c.V0, c.A3);
        c.V1 = mem.ReadWordLeft(c.V1, (c.A3 + 0x7u));
        c.V1 = mem.ReadWordRight(c.V1, (c.A3 + 0x4u));
        c.A0 = mem.ReadWordLeft(c.A0, (c.A3 + 0xBu));
        c.A0 = mem.ReadWordRight(c.A0, (c.A3 + 0x8u));
        c.A1 = mem.ReadWordLeft(c.A1, (c.A3 + 0xFu));
        c.A1 = mem.ReadWordRight(c.A1, (c.A3 + 0xCu));
        mem.WriteWordLeft((c.T0 + 0x3u), c.V0);
        mem.WriteWordRight(c.T0, c.V0);
        mem.WriteWordLeft((c.T0 + 0x7u), c.V1);
        mem.WriteWordRight((c.T0 + 0x4u), c.V1);
        mem.WriteWordLeft((c.T0 + 0xBu), c.A0);
        mem.WriteWordRight((c.T0 + 0x8u), c.A0);
        mem.WriteWordLeft((c.T0 + 0xFu), c.A1);
        mem.WriteWordRight((c.T0 + 0xCu), c.A1);
        c.A3 = c.A3 + 0x10u;
    }

    static void After800334F0(CpuContext c, PSMemory mem)
    {
        c.V0 = mem.ReadWordLeft(c.V0, (c.A3 + 0x3u));
        c.V0 = mem.ReadWordRight(c.V0, c.A3);
        c.V1 = mem.ReadWordLeft(c.V1, (c.A3 + 0x7u));
        c.V1 = mem.ReadWordRight(c.V1, (c.A3 + 0x4u));
        c.A0 = mem.ReadWordLeft(c.A0, (c.A3 + 0xBu));
        c.A0 = mem.ReadWordRight(c.A0, (c.A3 + 0x8u));
        mem.WriteWordLeft((c.T0 + 0x3u), c.V0);
        mem.WriteWordRight(c.T0, c.V0);
        mem.WriteWordLeft((c.T0 + 0x7u), c.V1);
        mem.WriteWordRight((c.T0 + 0x4u), c.V1);
        mem.WriteWordLeft((c.T0 + 0xBu), c.A0);
        mem.WriteWordRight((c.T0 + 0x8u), c.A0);
        c.V0 = 0x00010000u;
    }

    static void After80033524(CpuContext c, PSMemory mem)
    {
        c.V0 = c.V0 | 0x8000u;
        c.V1 = 0x80160000u;
        c.V1 = c.V1 - 0x2F70u;
        c.V1 = c.V1 + c.V0;
        c.A2 = c.A2 & 0x00FFu;
        c.V0 = mem.ReadU32((c.V1 + 0x6E0Cu));
        c.A1 = 0x00000040u;
        c.V0 = (uint)(short)mem.ReadU16((c.V0 + 0x68u));
        c.A1 = c.A1 - c.A2;
        c.LO = (uint)((long)(int)c.V0 * (int)c.A1); c.HI = (uint)(((long)(int)c.V0 * (int)c.A1) >> 32);
        c.V0 = mem.ReadU32((c.V1 + 0x6E10u));
        c.A0 = c.LO;
        c.V0 = (uint)(short)mem.ReadU16((c.V0 + 0x68u));
        c.LO = (uint)((long)(int)c.V0 * (int)c.A2); c.HI = (uint)(((long)(int)c.V0 * (int)c.A2) >> 32);
        c.V0 = c.LO;
        c.V0 = c.A0 + c.V0;
        c.A0 = mem.ReadU32((c.V1 + 0x6E08u));
        c.V0 = (uint)((int)c.V0 >> 6);
        mem.WriteU16((c.A0 + 0x68u), (ushort)c.V0);
        c.V0 = mem.ReadU32((c.V1 + 0x6E0Cu));
        c.V0 = (uint)(short)mem.ReadU16((c.V0 + 0x6Au));
        c.LO = (uint)((long)(int)c.V0 * (int)c.A1); c.HI = (uint)(((long)(int)c.V0 * (int)c.A1) >> 32);
        c.V0 = mem.ReadU32((c.V1 + 0x6E10u));
        c.A0 = c.LO;
        c.V0 = (uint)(short)mem.ReadU16((c.V0 + 0x6Au));
        c.LO = (uint)((long)(int)c.V0 * (int)c.A2); c.HI = (uint)(((long)(int)c.V0 * (int)c.A2) >> 32);
        c.V0 = c.LO;
        c.V0 = c.A0 + c.V0;
        c.A0 = mem.ReadU32((c.V1 + 0x6E08u));
        c.V0 = (uint)((int)c.V0 >> 6);
        mem.WriteU16((c.A0 + 0x6Au), (ushort)c.V0);
        c.V0 = mem.ReadU32((c.V1 + 0x6E0Cu));
        c.V0 = (uint)(short)mem.ReadU16((c.V0 + 0x50u));
        c.LO = (uint)((long)(int)c.V0 * (int)c.A1); c.HI = (uint)(((long)(int)c.V0 * (int)c.A1) >> 32);
        c.V0 = mem.ReadU32((c.V1 + 0x6E10u));
        c.A0 = c.LO;
        c.V0 = (uint)(short)mem.ReadU16((c.V0 + 0x50u));
        c.LO = (uint)((long)(int)c.V0 * (int)c.A2); c.HI = (uint)(((long)(int)c.V0 * (int)c.A2) >> 32);
        c.V0 = c.LO;
        c.V0 = c.A0 + c.V0;
        c.A0 = mem.ReadU32((c.V1 + 0x6E08u));
        c.V0 = c.V0 >> 6;
        mem.WriteU16((c.A0 + 0x50u), (ushort)c.V0);
        c.V0 = mem.ReadU32((c.V1 + 0x6E0Cu));
        c.V0 = (uint)(short)mem.ReadU16((c.V0 + 0x52u));
        c.LO = (uint)((long)(int)c.V0 * (int)c.A1); c.HI = (uint)(((long)(int)c.V0 * (int)c.A1) >> 32);
        c.V0 = mem.ReadU32((c.V1 + 0x6E10u));
        c.A0 = c.LO;
        c.V0 = (uint)(short)mem.ReadU16((c.V0 + 0x52u));
        c.LO = (uint)((long)(int)c.V0 * (int)c.A2); c.HI = (uint)(((long)(int)c.V0 * (int)c.A2) >> 32);
        c.V0 = c.LO;
        c.V0 = c.A0 + c.V0;
        c.A0 = mem.ReadU32((c.V1 + 0x6E08u));
        c.V0 = c.V0 >> 6;
        mem.WriteU16((c.A0 + 0x52u), (ushort)c.V0);
        c.V0 = mem.ReadU32((c.V1 + 0x6E0Cu));
        c.V0 = (uint)(short)mem.ReadU16((c.V0 + 0x54u));
        c.LO = (uint)((long)(int)c.V0 * (int)c.A1); c.HI = (uint)(((long)(int)c.V0 * (int)c.A1) >> 32);
        c.V0 = mem.ReadU32((c.V1 + 0x6E10u));
        c.A0 = c.LO;
        c.V0 = (uint)(short)mem.ReadU16((c.V0 + 0x54u));
        c.LO = (uint)((long)(int)c.V0 * (int)c.A2); c.HI = (uint)(((long)(int)c.V0 * (int)c.A2) >> 32);
        c.V0 = c.LO;
        c.V0 = c.A0 + c.V0;
        c.A0 = mem.ReadU32((c.V1 + 0x6E08u));
        c.V0 = c.V0 >> 6;
        mem.WriteU16((c.A0 + 0x54u), (ushort)c.V0);
        c.V0 = mem.ReadU32((c.V1 + 0x6E0Cu));
        c.V0 = (uint)(short)mem.ReadU16((c.V0 + 0x56u));
        c.LO = (uint)((long)(int)c.V0 * (int)c.A1); c.HI = (uint)(((long)(int)c.V0 * (int)c.A1) >> 32);
        c.V0 = mem.ReadU32((c.V1 + 0x6E10u));
        c.A0 = c.LO;
        c.V0 = (uint)(short)mem.ReadU16((c.V0 + 0x56u));
        c.LO = (uint)((long)(int)c.V0 * (int)c.A2); c.HI = (uint)(((long)(int)c.V0 * (int)c.A2) >> 32);
        c.V0 = c.LO;
        c.V0 = c.A0 + c.V0;
        c.A0 = mem.ReadU32((c.V1 + 0x6E08u));
        c.V0 = c.V0 >> 6;
        mem.WriteU16((c.A0 + 0x56u), (ushort)c.V0);
        c.V0 = mem.ReadU32((c.V1 + 0x6E0Cu));
        c.V0 = (uint)(short)mem.ReadU16((c.V0 + 0x58u));
        c.LO = (uint)((long)(int)c.V0 * (int)c.A1); c.HI = (uint)(((long)(int)c.V0 * (int)c.A1) >> 32);
        c.V0 = mem.ReadU32((c.V1 + 0x6E10u));
        c.A0 = c.LO;
        c.V0 = (uint)(short)mem.ReadU16((c.V0 + 0x58u));
        c.LO = (uint)((long)(int)c.V0 * (int)c.A2); c.HI = (uint)(((long)(int)c.V0 * (int)c.A2) >> 32);
        c.V0 = c.LO;
        c.V0 = c.A0 + c.V0;
        c.A0 = mem.ReadU32((c.V1 + 0x6E08u));
        c.V0 = c.V0 >> 6;
        mem.WriteU16((c.A0 + 0x58u), (ushort)c.V0);
        c.V0 = mem.ReadU32((c.V1 + 0x6E0Cu));
        c.V0 = (uint)(short)mem.ReadU16((c.V0 + 0x5Au));
        c.LO = (uint)((long)(int)c.V0 * (int)c.A1); c.HI = (uint)(((long)(int)c.V0 * (int)c.A1) >> 32);
        c.V0 = mem.ReadU32((c.V1 + 0x6E10u));
        c.A0 = c.LO;
        c.V0 = (uint)(short)mem.ReadU16((c.V0 + 0x5Au));
        c.LO = (uint)((long)(int)c.V0 * (int)c.A2); c.HI = (uint)(((long)(int)c.V0 * (int)c.A2) >> 32);
        c.V0 = c.LO;
        c.V0 = c.A0 + c.V0;
        c.A0 = mem.ReadU32((c.V1 + 0x6E08u));
        c.V0 = c.V0 >> 6;
        mem.WriteU16((c.A0 + 0x5Au), (ushort)c.V0);
        c.V0 = mem.ReadU32((c.V1 + 0x6E0Cu));
        c.V0 = (uint)(short)mem.ReadU16((c.V0 + 0x5Cu));
        c.LO = (uint)((long)(int)c.V0 * (int)c.A1); c.HI = (uint)(((long)(int)c.V0 * (int)c.A1) >> 32);
        c.V0 = mem.ReadU32((c.V1 + 0x6E10u));
        c.A0 = c.LO;
        c.V0 = (uint)(short)mem.ReadU16((c.V0 + 0x5Cu));
        c.LO = (uint)((long)(int)c.V0 * (int)c.A2); c.HI = (uint)(((long)(int)c.V0 * (int)c.A2) >> 32);
        c.V0 = c.LO;
        c.V0 = c.A0 + c.V0;
        c.A0 = mem.ReadU32((c.V1 + 0x6E08u));
        c.V0 = c.V0 >> 6;
        mem.WriteU16((c.A0 + 0x5Cu), (ushort)c.V0);
        c.V0 = mem.ReadU32((c.V1 + 0x6E0Cu));
        c.V0 = (uint)(short)mem.ReadU16((c.V0 + 0x5Eu));
        c.LO = (uint)((long)(int)c.V0 * (int)c.A1); c.HI = (uint)(((long)(int)c.V0 * (int)c.A1) >> 32);
        c.V0 = mem.ReadU32((c.V1 + 0x6E10u));
        c.A0 = c.LO;
        c.V0 = (uint)(short)mem.ReadU16((c.V0 + 0x5Eu));
        c.LO = (uint)((long)(int)c.V0 * (int)c.A2); c.HI = (uint)(((long)(int)c.V0 * (int)c.A2) >> 32);
        c.V0 = c.LO;
        c.V0 = c.A0 + c.V0;
        c.A0 = mem.ReadU32((c.V1 + 0x6E08u));
        c.V0 = c.V0 >> 6;
        mem.WriteU16((c.A0 + 0x5Eu), (ushort)c.V0);
        c.V0 = mem.ReadU32((c.V1 + 0x6E0Cu));
        c.V0 = (uint)(short)mem.ReadU16((c.V0 + 0x60u));
        c.LO = (uint)((long)(int)c.V0 * (int)c.A1); c.HI = (uint)(((long)(int)c.V0 * (int)c.A1) >> 32);
        c.V0 = mem.ReadU32((c.V1 + 0x6E10u));
        c.A0 = c.LO;
        c.V0 = (uint)(short)mem.ReadU16((c.V0 + 0x60u));
        c.LO = (uint)((long)(int)c.V0 * (int)c.A2); c.HI = (uint)(((long)(int)c.V0 * (int)c.A2) >> 32);
        c.V0 = c.LO;
        c.V0 = c.A0 + c.V0;
        c.A0 = mem.ReadU32((c.V1 + 0x6E08u));
        c.V0 = c.V0 >> 6;
        mem.WriteU16((c.A0 + 0x60u), (ushort)c.V0);
        c.V0 = mem.ReadU32((c.V1 + 0x6E0Cu));
        c.V0 = mem.ReadU8((c.V0 + 0x64u));
        c.LO = (uint)((long)(int)c.V0 * (int)c.A1); c.HI = (uint)(((long)(int)c.V0 * (int)c.A1) >> 32);
        c.V0 = mem.ReadU32((c.V1 + 0x6E10u));
        c.A0 = c.LO;
        c.V0 = mem.ReadU8((c.V0 + 0x64u));
        c.LO = (uint)((long)(int)c.V0 * (int)c.A2); c.HI = (uint)(((long)(int)c.V0 * (int)c.A2) >> 32);
        c.V0 = c.LO;
        c.V0 = c.A0 + c.V0;
        c.A0 = mem.ReadU32((c.V1 + 0x6E08u));
        c.V0 = c.V0 >> 6;
        mem.WriteU8((c.A0 + 0x64u), (byte)c.V0);
        c.V0 = mem.ReadU32((c.V1 + 0x6E0Cu));
        c.V0 = mem.ReadU8((c.V0 + 0x65u));
        c.LO = (uint)((long)(int)c.V0 * (int)c.A1); c.HI = (uint)(((long)(int)c.V0 * (int)c.A1) >> 32);
        c.V0 = mem.ReadU32((c.V1 + 0x6E10u));
        c.A0 = c.LO;
        c.V0 = mem.ReadU8((c.V0 + 0x65u));
        c.LO = (uint)((long)(int)c.V0 * (int)c.A2); c.HI = (uint)(((long)(int)c.V0 * (int)c.A2) >> 32);
        c.V0 = c.LO;
        c.V0 = c.A0 + c.V0;
        c.A0 = mem.ReadU32((c.V1 + 0x6E08u));
        c.V0 = c.V0 >> 6;
        mem.WriteU8((c.A0 + 0x65u), (byte)c.V0);
        c.V0 = mem.ReadU32((c.V1 + 0x6E0Cu));
        c.V0 = mem.ReadU8((c.V0 + 0x66u));
        c.LO = (uint)((long)(int)c.V0 * (int)c.A1); c.HI = (uint)(((long)(int)c.V0 * (int)c.A1) >> 32);
        c.V0 = mem.ReadU32((c.V1 + 0x6E10u));
        c.A0 = c.LO;
        c.V0 = mem.ReadU8((c.V0 + 0x66u));
        c.LO = (uint)((long)(int)c.V0 * (int)c.A2); c.HI = (uint)(((long)(int)c.V0 * (int)c.A2) >> 32);
        c.V0 = c.LO;
        c.V0 = c.A0 + c.V0;
        c.A0 = mem.ReadU32((c.V1 + 0x6E08u));
        c.V0 = c.V0 >> 6;
        mem.WriteU8((c.A0 + 0x66u), (byte)c.V0);
        c.V0 = mem.ReadU8((c.V1 + 0x6DFEu));
        c.V0 = c.V0 + 0x1u;
        mem.WriteU8((c.V1 + 0x6DFEu), (byte)c.V0);
    }

    static void After800338C8(CpuContext c, PSMemory mem)
    {
        c.V0 = mem.ReadWordLeft(c.V0, (c.A2 + 0x3u));
        c.V0 = mem.ReadWordRight(c.V0, c.A2);
        c.V1 = mem.ReadWordLeft(c.V1, (c.A2 + 0x7u));
        c.V1 = mem.ReadWordRight(c.V1, (c.A2 + 0x4u));
        c.A0 = mem.ReadWordLeft(c.A0, (c.A2 + 0xBu));
        c.A0 = mem.ReadWordRight(c.A0, (c.A2 + 0x8u));
        c.A1 = mem.ReadWordLeft(c.A1, (c.A2 + 0xFu));
        c.A1 = mem.ReadWordRight(c.A1, (c.A2 + 0xCu));
        mem.WriteWordLeft((c.A3 + 0x3u), c.V0);
        mem.WriteWordRight(c.A3, c.V0);
        mem.WriteWordLeft((c.A3 + 0x7u), c.V1);
        mem.WriteWordRight((c.A3 + 0x4u), c.V1);
        mem.WriteWordLeft((c.A3 + 0xBu), c.A0);
        mem.WriteWordRight((c.A3 + 0x8u), c.A0);
        mem.WriteWordLeft((c.A3 + 0xFu), c.A1);
        mem.WriteWordRight((c.A3 + 0xCu), c.A1);
        c.A2 = c.A2 + 0x10u;
    }

    static void After80033948(CpuContext c, PSMemory mem)
    {
        c.V0 = mem.ReadWordLeft(c.V0, (c.A2 + 0x3u));
        c.V0 = mem.ReadWordRight(c.V0, c.A2);
        c.V1 = mem.ReadWordLeft(c.V1, (c.A2 + 0x7u));
        c.V1 = mem.ReadWordRight(c.V1, (c.A2 + 0x4u));
        c.A0 = mem.ReadWordLeft(c.A0, (c.A2 + 0xBu));
        c.A0 = mem.ReadWordRight(c.A0, (c.A2 + 0x8u));
        mem.WriteWordLeft((c.A3 + 0x3u), c.V0);
        mem.WriteWordRight(c.A3, c.V0);
        mem.WriteWordLeft((c.A3 + 0x7u), c.V1);
        mem.WriteWordRight((c.A3 + 0x4u), c.V1);
        mem.WriteWordLeft((c.A3 + 0xBu), c.A0);
        mem.WriteWordRight((c.A3 + 0x8u), c.A0);
    }

    static void AfterEntry(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x20u;
        c.A0 = 0x80190000u;
        c.V0 = 0x801B0000u;
        c.V1 = mem.ReadU8((c.V0 + 0x5929u));
        c.A0 = c.A0 - 0x4930u;
        mem.WriteU32((c.SP + 0x1Cu), c.RA);
        mem.WriteU32((c.SP + 0x18u), c.S2);
        mem.WriteU32((c.SP + 0x14u), c.S1);
        mem.WriteU32((c.SP + 0x10u), c.S0);
        c.V0 = c.V1 << 1;
        c.V0 = c.V0 + c.A0;
        c.V0 = mem.ReadU8((c.V0 + 0xCu));
        c.S2 = c.V0 & 0x000Fu;
        c.S1 = c.V0 & 0x00F0u;
        c.V0 = c.V1;
        c.V1 = c.V1 - 0x19u;
        c.V1 = c.V1 < 0x00000002u ? 1u : 0u;
    }
}
