using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_80029618</c>: a window or panel, x y w h; the menu and the prompt. <c>ST_WINDOW29618</c>.
/// </summary>
public static class Window29618
{
    public static readonly Literal R = new(typeof(Window29618), "window-29618", "WINDOW29618", 0x80029618, Run,
        "a window or panel, x y w h; the menu and the prompt.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x68u;
        mem.WriteU32((c.SP + 0x54u), c.S5);
        c.S5 = c.A0;
        mem.WriteU32((c.SP + 0x50u), c.S4);
        c.S4 = c.A1;
        mem.WriteU32((c.SP + 0x5Cu), c.S7);
        c.S7 = c.A2;
        mem.WriteU32((c.SP + 0x58u), c.S6);
        c.S6 = c.A3;
        c.V0 = 0x80080000u;
        mem.WriteU32((c.SP + 0x64u), c.RA);
        mem.WriteU32((c.SP + 0x60u), c.FP);
        mem.WriteU32((c.SP + 0x4Cu), c.S3);
        mem.WriteU32((c.SP + 0x48u), c.S2);
        mem.WriteU32((c.SP + 0x44u), c.S1);
        mem.WriteU32((c.SP + 0x40u), c.S0);
        c.A2 = c.V0 + 0x7F30u;
        c.V1 = mem.ReadWordLeft(c.V1, (c.A2 + 0x3u));
        c.V1 = mem.ReadWordRight(c.V1, c.A2);
        mem.WriteWordLeft((c.SP + 0x23u), c.V1);
        mem.WriteWordRight((c.SP + 0x20u), c.V1);
        c.A0 = 0x1F800000u;
        c.A0 = c.A0 | 0x033Cu;
        c.A1 = 0x1F800000u;
        c.A1 = c.A1 | 0x0358u;
        c.A2 = 0x1F800000u;
        c.A2 = c.A2 | 0x035Cu;
        c.A3 = 0u;
        c.FP = 0x1F800000u;
        c.V0 = 0x00000010u;
        mem.WriteU16((c.FP + 0x340u), (ushort)c.V0);
        mem.WriteU16((c.FP + 0x342u), (ushort)c.V0);
        c.V0 = 0x00000220u;
        mem.WriteU16((c.FP + 0x35Cu), (ushort)c.V0);
        c.V0 = 0x000001F8u;
        mem.WriteU16((c.FP + 0x35Eu), (ushort)c.V0);
        c.V0 = 0x00000264u;
        mem.WriteU16((c.FP + 0x358u), (ushort)c.V0);
        c.V0 = 0x00000190u;
        c.S2 = 0x00000001u;
        mem.WriteU16((c.FP + 0x33Cu), (ushort)c.S5);
        mem.WriteU16((c.FP + 0x33Eu), (ushort)c.S4);
        mem.WriteU16((c.FP + 0x35Au), (ushort)c.V0);
        mem.WriteU32((c.SP + 0x10u), c.S2);
        c.RA = 0x800296D0u;
        ShadowTower_st.func_80028364(c, mem);
        c.A0 = c.FP;
        c.A0 = c.A0 | 0x033Cu;
        c.A1 = c.FP;
        c.A1 = c.A1 | 0x0358u;
        c.A2 = c.FP;
        c.A2 = c.A2 | 0x035Cu;
        c.A3 = 0u;
        c.S0 = c.S4 + c.S6;
        c.S3 = c.S0 - 0x10u;
        c.S1 = 0x00000041u;
        mem.WriteU16((c.FP + 0x33Eu), (ushort)c.S3);
        mem.WriteU32((c.SP + 0x10u), c.S1);
        c.RA = 0x80029704u;
        ShadowTower_st.func_80028364(c, mem);
        c.A0 = c.FP;
        c.A0 = c.A0 | 0x033Cu;
        c.A1 = c.FP;
        c.A1 = c.A1 | 0x0358u;
        c.A2 = c.FP;
        c.A2 = c.A2 | 0x035Cu;
        c.A3 = 0u;
        c.V0 = c.S5 + c.S7;
        c.V0 = c.V0 - 0x10u;
        mem.WriteU32((c.SP + 0x38u), c.V0);
        c.T0 = mem.ReadU16((c.SP + 0x38u));
        c.V0 = 0x0000026Cu;
        mem.WriteU16((c.FP + 0x33Eu), (ushort)c.S4);
        mem.WriteU16((c.FP + 0x358u), (ushort)c.V0);
        mem.WriteU16((c.FP + 0x33Cu), (ushort)c.T0);
        mem.WriteU32((c.SP + 0x10u), c.S2);
        c.RA = 0x80029748u;
        ShadowTower_st.func_80028364(c, mem);
        c.A0 = c.FP;
        c.A0 = c.A0 | 0x033Cu;
        c.A1 = c.FP;
        c.A1 = c.A1 | 0x0358u;
        c.A2 = c.FP;
        c.A2 = c.A2 | 0x035Cu;
        c.A3 = 0u;
        mem.WriteU16((c.FP + 0x33Eu), (ushort)c.S3);
        mem.WriteU32((c.SP + 0x10u), c.S1);
        c.RA = 0x80029770u;
        ShadowTower_st.func_80028364(c, mem);
        c.A0 = c.FP;
        c.A0 = c.A0 | 0x033Cu;
        c.A1 = c.FP;
        c.A1 = c.A1 | 0x0358u;
        c.A2 = c.FP;
        c.A2 = c.A2 | 0x035Cu;
        c.A3 = 0u;
        c.S3 = c.S4 + 0x10u;
        c.V0 = 0x0000025Cu;
        mem.WriteU16((c.FP + 0x33Cu), (ushort)c.S5);
        mem.WriteU16((c.FP + 0x33Eu), (ushort)c.S3);
        mem.WriteU16((c.FP + 0x358u), (ushort)c.V0);
        mem.WriteU32((c.SP + 0x10u), c.S2);
        c.RA = 0x800297A8u;
        ShadowTower_st.func_80028364(c, mem);
        c.A0 = c.FP;
        c.A0 = c.A0 | 0x033Cu;
        c.A1 = c.FP;
        c.A1 = c.A1 | 0x0358u;
        c.A2 = c.FP;
        c.A2 = c.A2 | 0x035Cu;
        c.A3 = 0u;
        c.S0 = c.S0 - 0x20u;
        mem.WriteU16((c.FP + 0x33Eu), (ushort)c.S0);
        mem.WriteU32((c.SP + 0x10u), c.S1);
        c.RA = 0x800297D4u;
        ShadowTower_st.func_80028364(c, mem);
        c.A0 = c.FP;
        c.A0 = c.A0 | 0x033Cu;
        c.A1 = c.FP;
        c.A1 = c.A1 | 0x0358u;
        c.A2 = c.FP;
        c.A2 = c.A2 | 0x035Cu;
        c.A3 = 0u;
        c.T0 = mem.ReadU16((c.SP + 0x38u));
        c.V0 = 0x00000254u;
        mem.WriteU16((c.FP + 0x33Eu), (ushort)c.S3);
        mem.WriteU16((c.FP + 0x358u), (ushort)c.V0);
        mem.WriteU16((c.FP + 0x33Cu), (ushort)c.T0);
        mem.WriteU32((c.SP + 0x10u), c.S2);
        c.RA = 0x8002980Cu;
        ShadowTower_st.func_80028364(c, mem);
        c.A0 = c.FP;
        c.A0 = c.A0 | 0x033Cu;
        c.A1 = c.FP;
        c.A1 = c.A1 | 0x0358u;
        c.A2 = c.FP;
        c.A2 = c.A2 | 0x035Cu;
        c.A3 = 0u;
        mem.WriteU16((c.FP + 0x33Eu), (ushort)c.S0);
        mem.WriteU32((c.SP + 0x10u), c.S1);
        c.RA = 0x80029834u;
        ShadowTower_st.func_80028364(c, mem);
        c.V0 = c.S6 << 16;
        c.V1 = (uint)((int)c.V0 >> 16);
        c.V0 = c.S4 + 0x20u;
        mem.WriteU16((c.FP + 0x33Eu), (ushort)c.V0);
        c.V0 = 0x00000270u;
        mem.WriteU16((c.FP + 0x33Cu), (ushort)c.S5);
        mem.WriteU16((c.FP + 0x358u), (ushort)c.V0);
        mem.WriteU16((c.SP + 0x28u), (ushort)c.S7);
        if ((int)c.V1 >= 0) {
            mem.WriteU16((c.SP + 0x30u), (ushort)c.S6);
            goto L80029860;
        }
        mem.WriteU16((c.SP + 0x30u), (ushort)c.S6);
        c.V1 = c.V1 + 0xFu;
        L80029860: ;
        c.V0 = (uint)((int)c.V1 >> 4);
        c.S1 = c.V0 - 0x5u;
        c.V0 = 0xFFFFFFFFu;
        if (c.S1 == c.V0) {
            c.S0 = 0x1F800000u;
            goto L800298E8;
        }
        c.S0 = 0x1F800000u;
        c.S3 = 0x1F800000u;
        c.S3 = c.S3 | 0x033Cu;
        c.S2 = 0x1F800000u;
        c.S2 = c.S2 | 0x0358u;
        c.S0 = c.S0 | 0x035Cu;
        c.S6 = mem.ReadU32((c.SP + 0x38u));
        c.A0 = c.S3;
        L80029890: ;
        Poll(c, mem);
        c.A1 = c.S2;
        c.A2 = c.S0;
        c.A3 = 0u;
        c.V0 = 0x00000021u;
        mem.WriteU16((c.FP + 0x33Cu), (ushort)c.S5);
        mem.WriteU32((c.SP + 0x10u), c.V0);
        c.RA = 0x800298ACu;
        ShadowTower_st.func_80028364(c, mem);
        c.A0 = c.S3;
        c.A1 = c.S2;
        c.A2 = c.S0;
        c.A3 = 0u;
        c.V0 = 0x00000001u;
        mem.WriteU16((c.FP + 0x33Cu), (ushort)c.S6);
        mem.WriteU32((c.SP + 0x10u), c.V0);
        c.RA = 0x800298CCu;
        ShadowTower_st.func_80028364(c, mem);
        c.V0 = mem.ReadU16((c.FP + 0x33Eu));
        c.S1 = c.S1 - 0x1u;
        c.V0 = c.V0 + 0x10u;
        mem.WriteU16((c.FP + 0x33Eu), (ushort)c.V0);
        c.V0 = 0xFFFFFFFFu;
        if (c.S1 != c.V0) {
            c.A0 = c.S3;
            goto L80029890;
        }
        c.A0 = c.S3;
        L800298E8: ;
        c.T0 = mem.ReadU16((c.SP + 0x28u));
        c.V0 = c.T0 << 16;
        c.V1 = (uint)((int)c.V0 >> 16);
        c.V0 = c.S5 + 0x10u;
        mem.WriteU16((c.FP + 0x33Cu), (ushort)c.V0);
        c.V0 = 0x00000268u;
        if ((int)c.V1 >= 0) {
            mem.WriteU16((c.FP + 0x358u), (ushort)c.V0);
            goto L80029910;
        }
        mem.WriteU16((c.FP + 0x358u), (ushort)c.V0);
        c.V1 = c.V1 + 0xFu;
        L80029910: ;
        c.V0 = (uint)((int)c.V1 >> 4);
        c.S0 = c.V0 - 0x3u;
        c.V0 = 0xFFFFFFFFu;
        if (c.S0 == c.V0) {
            goto L80029998;
        }
        c.S3 = c.FP + 0x33Cu;
        c.S2 = c.FP + 0x358u;
        c.T0 = mem.ReadU16((c.SP + 0x30u));
        c.S1 = c.FP + 0x35Cu;
        c.V0 = c.S4 + c.T0;
        c.S6 = c.V0 - 0x10u;
        c.A0 = c.S3;
        L80029940: ;
        Poll(c, mem);
        c.A1 = c.S2;
        c.A2 = c.S1;
        c.A3 = 0u;
        c.V0 = 0x00000001u;
        mem.WriteU16((c.FP + 0x33Eu), (ushort)c.S4);
        mem.WriteU32((c.SP + 0x10u), c.V0);
        c.RA = 0x8002995Cu;
        ShadowTower_st.func_80028364(c, mem);
        c.A0 = c.S3;
        c.A1 = c.S2;
        c.A2 = c.S1;
        c.A3 = 0u;
        c.V0 = 0x00000041u;
        mem.WriteU16((c.FP + 0x33Eu), (ushort)c.S6);
        mem.WriteU32((c.SP + 0x10u), c.V0);
        c.RA = 0x8002997Cu;
        ShadowTower_st.func_80028364(c, mem);
        c.V0 = mem.ReadU16((c.FP + 0x33Cu));
        c.S0 = c.S0 - 0x1u;
        c.V0 = c.V0 + 0x10u;
        mem.WriteU16((c.FP + 0x33Cu), (ushort)c.V0);
        c.V0 = 0xFFFFFFFFu;
        if (c.S0 != c.V0) {
            c.A0 = c.S3;
            goto L80029940;
        }
        c.A0 = c.S3;
        L80029998: ;
        c.T0 = mem.ReadU16((c.SP + 0x28u));
        c.V0 = c.T0 << 16;
        c.V1 = (uint)((int)c.V0 >> 16);
        c.V0 = c.S5 + 0x10u;
        mem.WriteU16((c.FP + 0x33Cu), (ushort)c.V0);
        c.V0 = 0x00000260u;
        if ((int)c.V1 >= 0) {
            mem.WriteU16((c.FP + 0x358u), (ushort)c.V0);
            goto L800299C0;
        }
        mem.WriteU16((c.FP + 0x358u), (ushort)c.V0);
        c.V1 = c.V1 + 0xFu;
        L800299C0: ;
        c.V0 = (uint)((int)c.V1 >> 4);
        c.S0 = c.V0 - 0x3u;
        c.V0 = 0xFFFFFFFFu;
        if (c.S0 == c.V0) {
            c.S1 = c.FP + 0x35Cu;
            goto L80029A4C;
        }
        c.S1 = c.FP + 0x35Cu;
        c.S7 = c.S4 + 0x10u;
        c.S3 = c.FP + 0x33Cu;
        c.S2 = c.FP + 0x358u;
        c.T0 = mem.ReadU16((c.SP + 0x30u));
        c.V0 = c.S4 + c.T0;
        c.S6 = c.V0 - 0x20u;
        c.A0 = c.S3;
        L800299F4: ;
        Poll(c, mem);
        c.A1 = c.S2;
        c.A2 = c.S1;
        c.A3 = 0u;
        c.V0 = 0x00000001u;
        mem.WriteU16((c.FP + 0x33Eu), (ushort)c.S7);
        mem.WriteU32((c.SP + 0x10u), c.V0);
        c.RA = 0x80029A10u;
        ShadowTower_st.func_80028364(c, mem);
        c.A0 = c.S3;
        c.A1 = c.S2;
        c.A2 = c.S1;
        c.A3 = 0u;
        c.V0 = 0x00000041u;
        mem.WriteU16((c.FP + 0x33Eu), (ushort)c.S6);
        mem.WriteU32((c.SP + 0x10u), c.V0);
        c.RA = 0x80029A30u;
        ShadowTower_st.func_80028364(c, mem);
        c.V0 = mem.ReadU16((c.FP + 0x33Cu));
        c.S0 = c.S0 - 0x1u;
        c.V0 = c.V0 + 0x10u;
        mem.WriteU16((c.FP + 0x33Cu), (ushort)c.V0);
        c.V0 = 0xFFFFFFFFu;
        if (c.S0 != c.V0) {
            c.A0 = c.S3;
            goto L800299F4;
        }
        c.A0 = c.S3;
        L80029A4C: ;
        c.T0 = mem.ReadU16((c.SP + 0x30u));
        c.V0 = c.T0 << 16;
        c.V1 = (uint)((int)c.V0 >> 16);
        c.V0 = c.S4 + 0x20u;
        mem.WriteU16((c.FP + 0x33Eu), (ushort)c.V0);
        c.V0 = 0x00000258u;
        if ((int)c.V1 >= 0) {
            mem.WriteU16((c.FP + 0x358u), (ushort)c.V0);
            goto L80029A74;
        }
        mem.WriteU16((c.FP + 0x358u), (ushort)c.V0);
        c.V1 = c.V1 + 0xFu;
        L80029A74: ;
        c.V0 = (uint)((int)c.V1 >> 4);
        c.S1 = c.V0 - 0x5u;
        c.V0 = 0xFFFFFFFFu;
        if (c.S1 == c.V0) {
            c.A0 = c.SP + 0x18u;
            goto L80029B0C;
        }
        c.A0 = c.SP + 0x18u;
        c.S3 = c.V0;
        c.S6 = 0x00000001u;
        L80029A90: ;
        Poll(c, mem);
        c.V0 = c.S5 + 0x10u;
        mem.WriteU16((c.FP + 0x33Cu), (ushort)c.V0);
        c.T0 = mem.ReadU16((c.SP + 0x28u));
        c.V0 = c.T0 << 16;
        c.V0 = (uint)((int)c.V0 >> 16);
        if ((int)c.V0 >= 0) {
            goto L80029AB4;
        }
        c.V0 = c.V0 + 0xFu;
        L80029AB4: ;
        c.V0 = (uint)((int)c.V0 >> 4);
        c.S0 = c.V0 - 0x3u;
        if (c.S0 == c.S3) {
            goto L80029AF4;
        }
        c.S2 = 0xFFFFFFFFu;
        L80029AC8: ;
        Poll(c, mem);
        c.A0 = c.FP + 0x33Cu;
        c.A1 = c.FP + 0x358u;
        c.A2 = c.FP + 0x35Cu;
        c.A3 = 0u;
        mem.WriteU32((c.SP + 0x10u), c.S6);
        c.RA = 0x80029AE0u;
        ShadowTower_st.func_80028364(c, mem);
        c.V0 = mem.ReadU16((c.FP + 0x33Cu));
        c.S0 = c.S0 - 0x1u;
        c.V0 = c.V0 + 0x10u;
        if (c.S0 != c.S2) {
            mem.WriteU16((c.FP + 0x33Cu), (ushort)c.V0);
            goto L80029AC8;
        }
        mem.WriteU16((c.FP + 0x33Cu), (ushort)c.V0);
        L80029AF4: ;
        c.V0 = mem.ReadU16((c.FP + 0x33Eu));
        c.S1 = c.S1 - 0x1u;
        c.V0 = c.V0 + 0x10u;
        if (c.S1 != c.S3) {
            mem.WriteU16((c.FP + 0x33Eu), (ushort)c.V0);
            goto L80029A90;
        }
        mem.WriteU16((c.FP + 0x33Eu), (ushort)c.V0);
        c.A0 = c.SP + 0x18u;
        L80029B0C: ;
        c.T0 = mem.ReadU16((c.SP + 0x28u));
        c.A1 = c.SP + 0x20u;
        mem.WriteU16((c.SP + 0x1Cu), (ushort)c.T0);
        c.T0 = mem.ReadU16((c.SP + 0x30u));
        c.A2 = 0x00000001u;
        mem.WriteU16((c.SP + 0x18u), (ushort)c.S5);
        mem.WriteU16((c.SP + 0x1Au), (ushort)c.S4);
        mem.WriteU16((c.SP + 0x1Eu), (ushort)c.T0);
        c.RA = 0x80029B30u;
        ShadowTower_st.func_80028220(c, mem);
        c.A0 = 0x00000002u;
        c.A1 = 0u;
        c.A2 = c.A1;
        c.RA = 0x80029B40u;
        ShadowTower_st.func_800286A8(c, mem);
        c.RA = mem.ReadU32((c.SP + 0x64u));
        c.FP = mem.ReadU32((c.SP + 0x60u));
        c.S7 = mem.ReadU32((c.SP + 0x5Cu));
        c.S6 = mem.ReadU32((c.SP + 0x58u));
        c.S5 = mem.ReadU32((c.SP + 0x54u));
        c.S4 = mem.ReadU32((c.SP + 0x50u));
        c.S3 = mem.ReadU32((c.SP + 0x4Cu));
        c.S2 = mem.ReadU32((c.SP + 0x48u));
        c.S1 = mem.ReadU32((c.SP + 0x44u));
        c.S0 = mem.ReadU32((c.SP + 0x40u));
        c.SP = c.SP + 0x68u;
        return;
    }
}
