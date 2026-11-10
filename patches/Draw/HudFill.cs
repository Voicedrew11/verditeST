using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_80026020</c>: one HUD panel record drawn as an F4 with its mode packet; called by func_80026964. <c>ST_HUDFILL</c>.
/// </summary>
public static class HudFill
{
    public static readonly Literal R = new(typeof(HudFill), "HUD fill", "HUDFILL", 0x80026020, Run,
        "one HUD panel record drawn as an F4 with its mode packet; called by func_80026964.");

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
            goto L8002620C;
        }
        c.T0 = c.A3;
        mem.WriteU32((c.V1 + 0x8u), c.A0);
        c.V0 = mem.ReadU32((c.A1 + 0x328u));
        c.V1 = mem.ReadU32((c.A1 + 0x304u));
        c.V0 = c.V0 + 0x1u;
        mem.WriteU32((c.A1 + 0x328u), c.V0);
        c.S0 = mem.ReadU32((c.V1 + 0x8u));
        c.V0 = mem.ReadU32((c.V1 + 0x4u));
        c.A0 = c.S0 + 0x18u;
        c.V0 = c.V0 < c.A0 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L8002620C;
        }
        mem.WriteU32((c.V1 + 0x8u), c.A0);
        c.V0 = mem.ReadU32((c.A1 + 0x328u));
        c.V0 = c.V0 + 0x1u;
        mem.WriteU32((c.A1 + 0x328u), c.V0);
        c.V0 = mem.ReadU16((c.S1 + 0x6u));
        mem.WriteU16((c.S0 + 0x8u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S1 + 0x8u));
        mem.WriteU16((c.S0 + 0xAu), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S1 + 0x6u));
        c.V1 = mem.ReadU16((c.S1 + 0xAu));
        c.V0 = c.V0 + c.V1;
        mem.WriteU16((c.S0 + 0xCu), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S1 + 0x8u));
        mem.WriteU16((c.S0 + 0xEu), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S1 + 0x6u));
        mem.WriteU16((c.S0 + 0x10u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S1 + 0x8u));
        c.V1 = mem.ReadU16((c.S1 + 0xCu));
        c.V0 = c.V0 + c.V1;
        mem.WriteU16((c.S0 + 0x12u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S1 + 0x6u));
        c.V1 = mem.ReadU16((c.S1 + 0xAu));
        c.V0 = c.V0 + c.V1;
        mem.WriteU16((c.S0 + 0x14u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S1 + 0x8u));
        c.V1 = mem.ReadU16((c.S1 + 0xCu));
        c.V0 = c.V0 + c.V1;
        mem.WriteU16((c.S0 + 0x16u), (ushort)c.V0);
        c.V0 = mem.ReadU8(c.A2);
        mem.WriteU8((c.S0 + 0x4u), (byte)c.V0);
        c.V0 = mem.ReadU8((c.A2 + 0x1u));
        mem.WriteU8((c.S0 + 0x5u), (byte)c.V0);
        c.V1 = mem.ReadU8((c.A2 + 0x2u));
        c.V0 = 0x00000005u;
        mem.WriteU8((c.S0 + 0x3u), (byte)c.V0);
        c.V0 = 0x00000028u;
        mem.WriteU8((c.S0 + 0x7u), (byte)c.V0);
        c.V0 = 0x000000FFu;
        mem.WriteU8((c.S0 + 0x6u), (byte)c.V1);
        c.V1 = c.T0 & 0xFFFFu;
        if (c.V1 == c.V0) {
            c.A1 = 0u;
            goto L80026198;
        }
        c.A1 = 0u;
        c.A0 = c.S2;
        c.A2 = 0x00000001u;
        c.A3 = c.A3 & 0x0003u;
        c.V0 = mem.ReadU16((c.S1 + 0x10u));
        c.A3 = c.A3 << 5;
        mem.WriteU32((c.SP + 0x10u), 0u);
        c.A3 = c.V0 | c.A3;
        c.RA = 0x8002618Cu;
        ShadowTower_st.SetDrawMode_st(c, mem);
        c.V0 = mem.ReadU8((c.S0 + 0x7u));
        c.V0 = c.V0 | 0x0002u;
        goto L800261B8;
        L80026198: ;
        c.A0 = c.S2;
        c.A3 = mem.ReadU16((c.S1 + 0x10u));
        c.A2 = 0x00000001u;
        mem.WriteU32((c.SP + 0x10u), 0u);
        c.RA = 0x800261ACu;
        ShadowTower_st.SetDrawMode_st(c, mem);
        c.V0 = mem.ReadU8((c.S0 + 0x7u));
        c.V0 = c.V0 & 0x00FDu;
        L800261B8: ;
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
        c.RA = 0x800261E4u;
        ShadowTower_st.AddPrim_st(c, mem);
        c.A1 = c.S2;
        c.A0 = (uint)(short)mem.ReadU16((c.S1 + 0x12u));
        c.V0 = mem.ReadU32((c.S0 + 0xE4u));
        c.A0 = c.A0 << 2;
        c.A0 = c.V0 + c.A0;
        c.RA = 0x800261FCu;
        ShadowTower_st.AddPrim_st(c, mem);
        c.V0 = mem.ReadU32((c.S0 + 0x324u));
        c.V0 = c.V0 + 0x2u;
        mem.WriteU32((c.S0 + 0x324u), c.V0);
        L8002620C: ;
        c.RA = mem.ReadU32((c.SP + 0x24u));
        c.S2 = mem.ReadU32((c.SP + 0x20u));
        c.S1 = mem.ReadU32((c.SP + 0x1Cu));
        c.S0 = mem.ReadU32((c.SP + 0x18u));
        c.SP = c.SP + 0x28u;
        return;
    }
}
