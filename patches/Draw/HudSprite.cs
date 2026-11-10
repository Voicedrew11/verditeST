using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_80025C24</c>: one HUD panel record drawn as a sprite with its mode packet; called by func_80026964. <c>ST_HUDSPRITE</c>.
/// </summary>
public static class HudSprite
{
    public static readonly Literal R = new(typeof(HudSprite), "HUD sprite", "HUDSPRITE", 0x80025C24, Run,
        "one HUD panel record drawn as a sprite with its mode packet; called by func_80026964.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x28u;
        mem.WriteU32((c.SP + 0x1Cu), c.S1);
        c.S1 = c.A0;
        c.A3 = c.A1;
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.V1 = 0u | 0x8000u;
        c.A1 = c.V0 + c.V1;
        mem.WriteU32((c.SP + 0x24u), c.RA);
        mem.WriteU32((c.SP + 0x20u), c.S2);
        mem.WriteU32((c.SP + 0x18u), c.S0);
        c.V1 = mem.ReadU32((c.A1 + 0x304u));
        c.S2 = mem.ReadU32((c.V1 + 0x8u));
        c.V0 = mem.ReadU32((c.V1 + 0x4u));
        c.A0 = c.S2 + 0xCu;
        c.V0 = c.V0 < c.A0 ? 1u : 0u;
        if (c.V0 != 0u) {
            c.T0 = c.A3;
            goto L80025DE4;
        }
        c.T0 = c.A3;
        mem.WriteU32((c.V1 + 0x8u), c.A0);
        c.V0 = mem.ReadU32((c.A1 + 0x328u));
        c.V1 = mem.ReadU32((c.A1 + 0x304u));
        c.V0 = c.V0 + 0x1u;
        mem.WriteU32((c.A1 + 0x328u), c.V0);
        c.S0 = mem.ReadU32((c.V1 + 0x8u));
        c.V0 = mem.ReadU32((c.V1 + 0x4u));
        c.A0 = c.S0 + 0x14u;
        c.V0 = c.V0 < c.A0 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L80025DE4;
        }
        mem.WriteU32((c.V1 + 0x8u), c.A0);
        c.V0 = mem.ReadU32((c.A1 + 0x328u));
        c.V0 = c.V0 + 0x1u;
        mem.WriteU32((c.A1 + 0x328u), c.V0);
        c.V0 = mem.ReadU16((c.S1 + 0x6u));
        mem.WriteU16((c.S0 + 0x8u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S1 + 0x8u));
        mem.WriteU16((c.S0 + 0xAu), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S1 + 0xEu));
        mem.WriteU16((c.S0 + 0xEu), (ushort)c.V0);
        c.V0 = mem.ReadU8((c.S1 + 0x2u));
        mem.WriteU8((c.S0 + 0xCu), (byte)c.V0);
        c.V0 = mem.ReadU8((c.S1 + 0x3u));
        mem.WriteU8((c.S0 + 0xDu), (byte)c.V0);
        c.V0 = mem.ReadU8((c.S1 + 0x4u));
        mem.WriteU16((c.S0 + 0x10u), (ushort)c.V0);
        c.V0 = mem.ReadU8((c.S1 + 0x5u));
        mem.WriteU16((c.S0 + 0x12u), (ushort)c.V0);
        c.V0 = mem.ReadU8(c.A2);
        mem.WriteU8((c.S0 + 0x4u), (byte)c.V0);
        c.V0 = mem.ReadU8((c.A2 + 0x1u));
        mem.WriteU8((c.S0 + 0x5u), (byte)c.V0);
        c.V1 = mem.ReadU8((c.A2 + 0x2u));
        c.V0 = 0x00000004u;
        mem.WriteU8((c.S0 + 0x3u), (byte)c.V0);
        c.V0 = 0x00000064u;
        mem.WriteU8((c.S0 + 0x7u), (byte)c.V0);
        c.V0 = 0x000000FFu;
        mem.WriteU8((c.S0 + 0x6u), (byte)c.V1);
        c.V1 = c.T0 & 0xFFFFu;
        if (c.V1 == c.V0) {
            c.A1 = 0u;
            goto L80025D70;
        }
        c.A1 = 0u;
        c.A0 = c.S2;
        c.A2 = 0x00000001u;
        c.A3 = c.A3 & 0x0003u;
        c.V0 = mem.ReadU16((c.S1 + 0x10u));
        c.A3 = c.A3 << 5;
        mem.WriteU32((c.SP + 0x10u), 0u);
        c.A3 = c.V0 | c.A3;
        c.RA = 0x80025D64u;
        ShadowTower_st.SetDrawMode_st(c, mem);
        c.V0 = mem.ReadU8((c.S0 + 0x7u));
        c.V0 = c.V0 | 0x0002u;
        goto L80025D90;
        L80025D70: ;
        c.A0 = c.S2;
        c.A3 = mem.ReadU16((c.S1 + 0x10u));
        c.A2 = 0x00000001u;
        mem.WriteU32((c.SP + 0x10u), 0u);
        c.RA = 0x80025D84u;
        ShadowTower_st.SetDrawMode_st(c, mem);
        c.V0 = mem.ReadU8((c.S0 + 0x7u));
        c.V0 = c.V0 & 0x00FDu;
        L80025D90: ;
        mem.WriteU8((c.S0 + 0x7u), (byte)c.V0);
        c.A1 = c.S0;
        c.S0 = 0x80160000u;
        c.S0 = c.S0 - 0x2F70u;
        c.V0 = 0u | 0x8000u;
        c.S0 = c.S0 + c.V0;
        c.A0 = (uint)(short)mem.ReadU16((c.S1 + 0x12u));
        c.V0 = mem.ReadU32((c.S0 + 0xE4u));
        c.A0 = c.A0 << 2;
        c.A0 = c.V0 + c.A0;
        c.RA = 0x80025DBCu;
        ShadowTower_st.AddPrim_st(c, mem);
        c.A1 = c.S2;
        c.A0 = (uint)(short)mem.ReadU16((c.S1 + 0x12u));
        c.V0 = mem.ReadU32((c.S0 + 0xE4u));
        c.A0 = c.A0 << 2;
        c.A0 = c.V0 + c.A0;
        c.RA = 0x80025DD4u;
        ShadowTower_st.AddPrim_st(c, mem);
        c.V0 = mem.ReadU32((c.S0 + 0x324u));
        c.V0 = c.V0 + 0x2u;
        mem.WriteU32((c.S0 + 0x324u), c.V0);
        L80025DE4: ;
        c.RA = mem.ReadU32((c.SP + 0x24u));
        c.S2 = mem.ReadU32((c.SP + 0x20u));
        c.S1 = mem.ReadU32((c.SP + 0x1Cu));
        c.S0 = mem.ReadU32((c.SP + 0x18u));
        c.SP = c.SP + 0x28u;
        return;
    }
}
