using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_80025E00</c>: one HUD panel record drawn as an FT4; called by func_80026964. <c>ST_HUDQUAD</c>.
/// </summary>
public static class HudQuad
{
    public static readonly Literal R = new(typeof(HudQuad), "HUD quad", "HUDQUAD", 0x80025E00, Run,
        "one HUD panel record drawn as an FT4; called by func_80026964.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x18u;
        c.A3 = c.A0;
        c.T1 = c.A1;
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.V1 = 0u | 0x8000u;
        c.T0 = c.V0 + c.V1;
        mem.WriteU32((c.SP + 0x14u), c.RA);
        mem.WriteU32((c.SP + 0x10u), c.S0);
        c.V1 = mem.ReadU32((c.T0 + 0x304u));
        c.A1 = mem.ReadU32((c.V1 + 0x8u));
        c.V0 = mem.ReadU32((c.V1 + 0x4u));
        c.A0 = c.A1 + 0x28u;
        c.V0 = c.V0 < c.A0 ? 1u : 0u;
        if (c.V0 != 0u) {
            c.T2 = c.T1;
            goto L8002600C;
        }
        c.T2 = c.T1;
        mem.WriteU32((c.V1 + 0x8u), c.A0);
        c.V0 = mem.ReadU32((c.T0 + 0x328u));
        c.V0 = c.V0 + 0x1u;
        mem.WriteU32((c.T0 + 0x328u), c.V0);
        c.V0 = mem.ReadU16((c.A3 + 0x6u));
        mem.WriteU16((c.A1 + 0x8u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.A3 + 0x8u));
        mem.WriteU16((c.A1 + 0xAu), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.A3 + 0x6u));
        c.V1 = mem.ReadU16((c.A3 + 0xAu));
        c.V0 = c.V0 + c.V1;
        mem.WriteU16((c.A1 + 0x10u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.A3 + 0x8u));
        mem.WriteU16((c.A1 + 0x12u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.A3 + 0x6u));
        mem.WriteU16((c.A1 + 0x18u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.A3 + 0x8u));
        c.V1 = mem.ReadU16((c.A3 + 0xCu));
        c.V0 = c.V0 + c.V1;
        mem.WriteU16((c.A1 + 0x1Au), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.A3 + 0x6u));
        c.V1 = mem.ReadU16((c.A3 + 0xAu));
        c.V0 = c.V0 + c.V1;
        mem.WriteU16((c.A1 + 0x20u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.A3 + 0x8u));
        c.V1 = mem.ReadU16((c.A3 + 0xCu));
        c.V0 = c.V0 + c.V1;
        mem.WriteU16((c.A1 + 0x22u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.A3 + 0xEu));
        mem.WriteU16((c.A1 + 0xEu), (ushort)c.V0);
        c.V0 = mem.ReadU8((c.A3 + 0x2u));
        mem.WriteU8((c.A1 + 0xCu), (byte)c.V0);
        c.V0 = mem.ReadU8((c.A3 + 0x3u));
        mem.WriteU8((c.A1 + 0xDu), (byte)c.V0);
        c.V0 = mem.ReadU8((c.A3 + 0x2u));
        c.V1 = mem.ReadU8((c.A3 + 0x4u));
        c.V0 = c.V0 + c.V1;
        mem.WriteU8((c.A1 + 0x14u), (byte)c.V0);
        c.V0 = mem.ReadU8((c.A3 + 0x3u));
        mem.WriteU8((c.A1 + 0x15u), (byte)c.V0);
        c.V0 = mem.ReadU8((c.A3 + 0x2u));
        mem.WriteU8((c.A1 + 0x1Cu), (byte)c.V0);
        c.V0 = mem.ReadU8((c.A3 + 0x3u));
        c.V1 = mem.ReadU8((c.A3 + 0x5u));
        c.V0 = c.V0 + c.V1;
        mem.WriteU8((c.A1 + 0x1Du), (byte)c.V0);
        c.V0 = mem.ReadU8((c.A3 + 0x2u));
        c.V1 = mem.ReadU8((c.A3 + 0x4u));
        c.V0 = c.V0 + c.V1;
        mem.WriteU8((c.A1 + 0x24u), (byte)c.V0);
        c.V0 = mem.ReadU8((c.A3 + 0x3u));
        c.V1 = mem.ReadU8((c.A3 + 0x5u));
        c.V0 = c.V0 + c.V1;
        mem.WriteU8((c.A1 + 0x25u), (byte)c.V0);
        c.V0 = mem.ReadU8(c.A2);
        mem.WriteU8((c.A1 + 0x4u), (byte)c.V0);
        c.V0 = mem.ReadU8((c.A2 + 0x1u));
        mem.WriteU8((c.A1 + 0x5u), (byte)c.V0);
        c.V1 = mem.ReadU8((c.A2 + 0x2u));
        c.V0 = 0x00000009u;
        mem.WriteU8((c.A1 + 0x3u), (byte)c.V0);
        c.V0 = 0x0000002Cu;
        mem.WriteU8((c.A1 + 0x7u), (byte)c.V0);
        c.V0 = 0x000000FFu;
        mem.WriteU8((c.A1 + 0x6u), (byte)c.V1);
        c.V1 = c.T2 & 0xFFFFu;
        if (c.V1 == c.V0) {
            c.V0 = c.T1 & 0x0003u;
            goto L80025FC4;
        }
        c.V0 = c.T1 & 0x0003u;
        c.V0 = c.V0 << 5;
        c.V1 = mem.ReadU16((c.A3 + 0x10u));
        c.A0 = mem.ReadU8((c.A1 + 0x7u));
        c.V1 = c.V1 | c.V0;
        c.A0 = c.A0 | 0x0002u;
        mem.WriteU16((c.A1 + 0x16u), (ushort)c.V1);
        mem.WriteU8((c.A1 + 0x7u), (byte)c.A0);
        goto L80025FD8;
        L80025FC4: ;
        c.V0 = mem.ReadU8((c.A1 + 0x7u));
        c.V1 = mem.ReadU16((c.A3 + 0x10u));
        c.V0 = c.V0 & 0x00FDu;
        mem.WriteU16((c.A1 + 0x16u), (ushort)c.V1);
        mem.WriteU8((c.A1 + 0x7u), (byte)c.V0);
        L80025FD8: ;
        c.S0 = 0x80160000u;
        c.S0 = c.S0 - 0x2F70u;
        c.V0 = 0u | 0x8000u;
        c.S0 = c.S0 + c.V0;
        c.A0 = (uint)(short)mem.ReadU16((c.A3 + 0x12u));
        c.V0 = mem.ReadU32((c.S0 + 0xE4u));
        c.A0 = c.A0 << 2;
        c.A0 = c.V0 + c.A0;
        c.RA = 0x80025FFCu;
        ShadowTower_st.AddPrim_st(c, mem);
        c.V0 = mem.ReadU32((c.S0 + 0x324u));
        c.V0 = c.V0 + 0x1u;
        mem.WriteU32((c.S0 + 0x324u), c.V0);
        L8002600C: ;
        c.RA = mem.ReadU32((c.SP + 0x14u));
        c.S0 = mem.ReadU32((c.SP + 0x10u));
        c.SP = c.SP + 0x18u;
        return;
    }
}
