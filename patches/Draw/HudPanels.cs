using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_80026964</c>: the HUD's panel list: 0x14-byte records from 0x8007F3F4 + 0x14 * a0, to a 0xFF, each drawn as a sprite, an FT4 or an F4. <c>ST_HUDPANELS</c>.
/// </summary>
public static class HudPanels
{
    public static readonly Literal R = new(typeof(HudPanels), "HUD panels", "HUDPANELS", 0x80026964, Run,
        "the HUD's panel list: 0x14-byte records from 0x8007F3F4 + 0x14 * a0, to a 0xFF, each drawn as a sprite, an FT4 or an F4.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x30u;
        c.V0 = 0x80080000u;
        mem.WriteU32((c.SP + 0x28u), c.RA);
        mem.WriteU32((c.SP + 0x24u), c.S3);
        mem.WriteU32((c.SP + 0x20u), c.S2);
        mem.WriteU32((c.SP + 0x1Cu), c.S1);
        mem.WriteU32((c.SP + 0x18u), c.S0);
        c.A3 = c.V0 + 0x7F14u;
        c.V1 = mem.ReadWordLeft(c.V1, (c.A3 + 0x3u));
        c.V1 = mem.ReadWordRight(c.V1, c.A3);
        mem.WriteWordLeft((c.SP + 0x13u), c.V1);
        mem.WriteWordRight((c.SP + 0x10u), c.V1);
        c.A0 = c.A0 & 0x00FFu;
        c.V1 = c.A0 << 2;
        c.V1 = c.V1 + c.A0;
        c.V1 = c.V1 << 2;
        c.V0 = 0x80080000u;
        c.V0 = c.V0 - 0xC0Cu;
        c.S1 = c.V1 + c.V0;
        c.V1 = mem.ReadU8(c.S1);
        c.V0 = 0x000000FFu;
        if (c.V1 == c.V0) {
            c.S3 = 0x00000060u;
            goto L80026A68;
        }
        c.S3 = 0x00000060u;
        c.S2 = c.V0;
        c.S0 = c.S1 + 0x1u;
        L800269CC: ;
        Poll(c, mem);
        c.V0 = c.V1 & 0x0001u;
        if (c.V0 == 0u) {
            c.V0 = c.V1 & 0x0004u;
            goto L80026A3C;
        }
        c.V0 = c.V1 & 0x0004u;
        if (c.V0 == 0u) {
            c.V0 = c.V1 & 0x0008u;
            goto L800269E8;
        }
        c.V0 = c.V1 & 0x0008u;
        mem.WriteU8((c.SP + 0x12u), (byte)0u);
        goto L80026A04;
        L800269E8: ;
        if (c.V0 == 0u) {
            goto L800269FC;
        }
        mem.WriteU8((c.SP + 0x11u), (byte)0u);
        mem.WriteU8((c.SP + 0x12u), (byte)0u);
        goto L80026A04;
        L800269FC: ;
        mem.WriteU8((c.SP + 0x11u), (byte)c.S3);
        mem.WriteU8((c.SP + 0x12u), (byte)c.S3);
        L80026A04: ;
        c.V0 = mem.ReadU16((c.S0 + 0x9u));
        if (c.V0 == c.S2) {
            c.A0 = c.S1;
            goto L80026A28;
        }
        c.A0 = c.S1;
        c.A1 = mem.ReadU8(c.S0);
        c.A2 = c.SP + 0x10u;
        c.RA = 0x80026A20u;
        ShadowTower_st.func_80025E00(c, mem);
        c.S1 = c.S1 + 0x14u;
        goto L80026A58;
        L80026A28: ;
        c.A1 = mem.ReadU8(c.S0);
        c.A2 = c.SP + 0x10u;
        c.RA = 0x80026A34u;
        ShadowTower_st.func_80025C24(c, mem);
        c.S1 = c.S1 + 0x14u;
        goto L80026A58;
        L80026A3C: ;
        c.V0 = c.V1 & 0x0002u;
        if (c.V0 == 0u) {
            c.A0 = c.S1;
            goto L80026A54;
        }
        c.A0 = c.S1;
        c.A1 = mem.ReadU8(c.S0);
        c.A2 = c.SP + 0x10u;
        c.RA = 0x80026A54u;
        ShadowTower_st.func_80026020(c, mem);
        L80026A54: ;
        c.S1 = c.S1 + 0x14u;
        L80026A58: ;
        c.V1 = mem.ReadU8(c.S1);
        if (c.V1 != c.S2) {
            c.S0 = c.S0 + 0x14u;
            goto L800269CC;
        }
        c.S0 = c.S0 + 0x14u;
        L80026A68: ;
        c.RA = mem.ReadU32((c.SP + 0x28u));
        c.S3 = mem.ReadU32((c.SP + 0x24u));
        c.S2 = mem.ReadU32((c.SP + 0x20u));
        c.S1 = mem.ReadU32((c.SP + 0x1Cu));
        c.S0 = mem.ReadU32((c.SP + 0x18u));
        c.SP = c.SP + 0x30u;
        return;
    }
}
