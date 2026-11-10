using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_80021654</c>: the area load's fade: while the loader (0x801B5920) runs, a full-screen FT4S whose shade follows the vblanks since 0x801B5938, over 0x78 of them. <c>ST_LOADFADE</c>.
/// </summary>
public static class LoadFade
{
    public static readonly Literal R = new(typeof(LoadFade), "load fade", "LOADFADE", 0x80021654, Run,
        "the area load's fade: while the loader (0x801B5920) runs, a full-screen FT4S whose shade follows the vblanks since 0x801B5938, over 0x78 of them.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x50u;
        c.V0 = 0x80090000u;
        mem.WriteU32((c.SP + 0x48u), c.S2);
        c.S2 = c.V0 - 0x7E00u;
        c.V0 = 0x801B0000u;
        mem.WriteU32((c.SP + 0x40u), c.S0);
        c.S0 = c.V0 + 0x5920u;
        mem.WriteU32((c.SP + 0x4Cu), c.RA);
        mem.WriteU32((c.SP + 0x44u), c.S1);
        c.A1 = mem.ReadU32((c.S2 + 0x4u));
        c.V1 = mem.ReadU32((c.S0 + 0x18u));
        c.A2 = (uint)(short)mem.ReadU16((c.V0 + 0x5920u));
        if (c.A2 != 0u) {
            c.A0 = c.A1 - c.V1;
            goto L8002169C;
        }
        c.A0 = c.A1 - c.V1;
        c.V0 = 0xFFFFFFFFu;
        if (c.V1 == c.V0) {
            goto L80021800;
        }
        L8002169C: ;
        c.V0 = mem.ReadU16((c.S0 + 0x2u));
        c.V0 = c.V0 << 16;
        c.V0 = (uint)((int)c.V0 >> 16);
        c.V0 = (int)c.V0 < 2 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.V0 = (int)c.A0 < 120 ? 1u : 0u;
            goto L800216C0;
        }
        c.V0 = (int)c.A0 < 120 ? 1u : 0u;
        mem.WriteU32((c.S0 + 0x18u), c.A1);
        goto L80021800;
        L800216C0: ;
        if (c.V0 == 0u) {
            c.V1 = c.A0 << 1;
            goto L8002170C;
        }
        c.V1 = c.A0 << 1;
        c.A0 = 0u;
        c.A1 = c.A0;
        c.A2 = 0x00000140u;
        c.A3 = 0x000000F0u;
        c.V0 = 0x000000E0u;
        mem.WriteU32((c.SP + 0x14u), c.V0);
        c.V0 = 0x0000000Fu;
        mem.WriteU32((c.SP + 0x18u), c.V0);
        mem.WriteU32((c.SP + 0x1Cu), c.V0);
        c.V0 = 0x00000001u;
        mem.WriteU32((c.SP + 0x20u), c.V0);
        c.V0 = 0x0000005Eu;
        mem.WriteU32((c.SP + 0x24u), c.V0);
        c.V0 = 0x00007C23u;
        c.V1 = c.V1 & 0x00FFu;
        mem.WriteU32((c.SP + 0x10u), 0u);
        goto L800217D8;
        L8002170C: ;
        c.S1 = (int)c.A0 < 240 ? 1u : 0u;
        if (c.S1 != 0u) {
            c.V1 = c.A0 - 0x78u;
            goto L80021720;
        }
        c.V1 = c.A0 - 0x78u;
        if (c.A2 == 0u) {
            c.V0 = (int)c.A0 < 360 ? 1u : 0u;
            goto L8002178C;
        }
        c.V0 = (int)c.A0 < 360 ? 1u : 0u;
        L80021720: ;
        c.V1 = c.V1 << 1;
        c.A0 = 0x00000020u;
        c.A1 = 0u;
        c.A2 = 0x00000120u;
        c.A3 = 0x00000100u;
        c.V0 = 0x000000FFu;
        mem.WriteU32((c.SP + 0x18u), c.V0);
        mem.WriteU32((c.SP + 0x1Cu), c.V0);
        c.V0 = 0x00000001u;
        mem.WriteU32((c.SP + 0x20u), c.V0);
        c.V0 = 0x0000003Fu;
        mem.WriteU32((c.SP + 0x24u), c.V0);
        c.V0 = 0x00007BE3u;
        c.V1 = c.V1 & 0x00FFu;
        mem.WriteU32((c.SP + 0x10u), 0u);
        mem.WriteU32((c.SP + 0x14u), 0u);
        mem.WriteU32((c.SP + 0x28u), c.V0);
        mem.WriteU32((c.SP + 0x2Cu), c.V1);
        mem.WriteU32((c.SP + 0x30u), c.V1);
        mem.WriteU32((c.SP + 0x34u), c.V1);
        mem.WriteU32((c.SP + 0x38u), 0u);
        c.RA = 0x80021778u;
        ShadowTower_st.func_80021518(c, mem);
        if (c.S1 != 0u) {
            goto L80021800;
        }
        c.V0 = mem.ReadU32((c.S2 + 0x4u));
        c.V0 = c.V0 - 0xF0u;
        goto L800217FC;
        L8002178C: ;
        if (c.V0 == 0u) {
            c.V1 = 0x00000168u;
            goto L800217F8;
        }
        c.V1 = 0x00000168u;
        c.V1 = c.V1 - c.A0;
        c.V1 = c.V1 << 1;
        c.A0 = 0x00000020u;
        c.A1 = 0u;
        c.A2 = 0x00000120u;
        c.A3 = 0x00000100u;
        c.V0 = 0x000000FFu;
        mem.WriteU32((c.SP + 0x18u), c.V0);
        mem.WriteU32((c.SP + 0x1Cu), c.V0);
        c.V0 = 0x00000001u;
        mem.WriteU32((c.SP + 0x20u), c.V0);
        c.V0 = 0x0000003Fu;
        mem.WriteU32((c.SP + 0x24u), c.V0);
        c.V0 = 0x00007BE3u;
        c.V1 = c.V1 & 0x00FFu;
        mem.WriteU32((c.SP + 0x10u), 0u);
        mem.WriteU32((c.SP + 0x14u), 0u);
        L800217D8: ;
        mem.WriteU32((c.SP + 0x28u), c.V0);
        mem.WriteU32((c.SP + 0x2Cu), c.V1);
        mem.WriteU32((c.SP + 0x30u), c.V1);
        mem.WriteU32((c.SP + 0x34u), c.V1);
        mem.WriteU32((c.SP + 0x38u), 0u);
        c.RA = 0x800217F0u;
        ShadowTower_st.func_80021518(c, mem);
        goto L80021800;
        L800217F8: ;
        c.V0 = 0xFFFFFFFFu;
        L800217FC: ;
        mem.WriteU32((c.S0 + 0x18u), c.V0);
        L80021800: ;
        c.RA = mem.ReadU32((c.SP + 0x4Cu));
        c.S2 = mem.ReadU32((c.SP + 0x48u));
        c.S1 = mem.ReadU32((c.SP + 0x44u));
        c.S0 = mem.ReadU32((c.SP + 0x40u));
        c.SP = c.SP + 0x50u;
        return;
    }
}
