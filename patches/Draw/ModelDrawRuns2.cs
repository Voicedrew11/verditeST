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
    static void After8002273C(CpuContext c, PSMemory mem)
    {
        c.A0 = 0u | 0xFFFFu;
        c.A1 = 0u;
        c.A2 = c.A1;
        c.RA = 0x80022754u;
        ShadowTower_st.func_8003F12C(c, mem);
        c.A0 = 0u | 0xFFFFu;
        c.A1 = 0x00000001u;
        c.A2 = 0u;
        c.RA = 0x80022764u;
        ShadowTower_st.func_8003F12C(c, mem);
    }

    static void After800228B4_2(CpuContext c, PSMemory mem)
    {
        c.A0 = c.S1;
        c.V0 = (uint)(short)mem.ReadU16(c.S7);
        mem.WriteU32((c.SP + 0x28u), c.V0);
        c.V0 = (uint)(short)mem.ReadU16((c.S7 + 0x2u));
        mem.WriteU32((c.SP + 0x2Cu), c.V0);
        c.V0 = (uint)(short)mem.ReadU16((c.S7 + 0x4u));
        c.A1 = c.SP + 0x28u;
        mem.WriteU32((c.SP + 0x30u), c.V0);
        c.RA = 0x80022A24u;
        ShadowTower_st.ScaleMatrix(c, mem);
    }

    static void After80022C40(CpuContext c, PSMemory mem)
    {
        c.S1 = c.S5 & 0x007Fu;
        c.V0 = c.S1 << 2;
        c.V0 = c.V0 + c.S1;
        c.V0 = c.V0 << 3;
        c.V0 = c.V0 + 0xCu;
        c.V0 = c.V1 + c.V0;
        mem.WriteU32((c.S0 + 0x20u), c.V0);
        c.V0 = mem.ReadU32(c.V0);
        c.V1 = mem.ReadU32((c.S0 + 0xCu));
        c.V0 = c.V0 + 0xCu;
    }

    static void After80022D8C(CpuContext c, PSMemory mem)
    {
        c.A1 = mem.ReadU32((c.S0 + 0x40u));
        c.V1 = mem.ReadU32((c.S0 + 0x20u));
        c.V0 = mem.ReadU32((c.S0 + 0x88u));
        c.A0 = mem.ReadU32((c.S0 + 0x20u));
        c.V1 = mem.ReadU32((c.V1 + 0x4u));
        c.A2 = mem.ReadU32((c.S0 + 0x48u));
        c.V0 = c.V0 + c.V1;
        mem.WriteU32((c.S0 + 0x88u), c.V0);
        c.A0 = mem.ReadU32((c.A0 + 0x4u));
        c.V0 = 0xFFFFFFFFu;
        c.A0 = c.A0 - 0x1u;
    }

    static void After80022E28(CpuContext c, PSMemory mem)
    {
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

    static void After80022F40(CpuContext c, PSMemory mem)
    {
        c.A1 = mem.ReadU32((c.S0 + 0x40u));
        c.V1 = mem.ReadU32((c.S0 + 0x20u));
        c.V0 = mem.ReadU32((c.S0 + 0x88u));
        c.A0 = mem.ReadU32((c.S0 + 0x20u));
        c.V1 = mem.ReadU32((c.V1 + 0x4u));
        c.A2 = mem.ReadU32((c.S0 + 0x48u));
        c.V0 = c.V0 + c.V1;
        mem.WriteU32((c.S0 + 0x88u), c.V0);
        c.A0 = mem.ReadU32((c.A0 + 0x4u));
        c.V0 = 0xFFFFFFFFu;
        c.A0 = c.A0 - 0x1u;
    }

    static void After80022FDC(CpuContext c, PSMemory mem)
    {
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

    static void After800230E4(CpuContext c, PSMemory mem)
    {
        c.A1 = mem.ReadU32((c.S0 + 0x40u));
        c.V1 = mem.ReadU32((c.S0 + 0x20u));
        c.V0 = mem.ReadU32((c.S0 + 0x88u));
        c.A0 = mem.ReadU32((c.S0 + 0x20u));
        c.V1 = mem.ReadU32((c.V1 + 0x4u));
        c.A2 = mem.ReadU32((c.S0 + 0x48u));
        c.V0 = c.V0 + c.V1;
        mem.WriteU32((c.S0 + 0x88u), c.V0);
        c.A0 = mem.ReadU32((c.A0 + 0x4u));
        c.V0 = 0xFFFFFFFFu;
        c.A0 = c.A0 - 0x1u;
    }
}
