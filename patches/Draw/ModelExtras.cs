using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_80023C6C</c>: func_800241D0's last call, after the models. <c>ST_MODELEXTRAS</c>.
/// </summary>
public static class ModelExtras
{
    public static readonly Literal R = new(typeof(ModelExtras), "model extras", "MODELEXTRAS", 0x80023C6C, Run,
        "func_800241D0's last call, after the models.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x40u;
        mem.WriteU32((c.SP + 0x28u), c.S4);
        c.S4 = c.A2;
        mem.WriteU32((c.SP + 0x34u), c.S7);
        c.S7 = 0x1F800000u;
        mem.WriteU32((c.SP + 0x30u), c.S6);
        c.S6 = c.A1;
        c.A3 = c.A3 + c.S6;
        mem.WriteU32((c.SP + 0x3Cu), c.RA);
        mem.WriteU32((c.SP + 0x38u), c.FP);
        mem.WriteU32((c.SP + 0x2Cu), c.S5);
        mem.WriteU32((c.SP + 0x24u), c.S3);
        mem.WriteU32((c.SP + 0x20u), c.S2);
        mem.WriteU32((c.SP + 0x1Cu), c.S1);
        mem.WriteU32((c.SP + 0x18u), c.S0);
        mem.WriteU32((c.SP + 0x40u), c.A0);
        c.FP = mem.ReadU32((c.S7 + 0xF0u));
        c.V0 = (int)c.S6 < (int)c.A3 ? 1u : 0u;
        if (c.V0 == 0u) {
            mem.WriteU32((c.SP + 0x10u), c.A3);
            goto L80023EA4;
        }
        mem.WriteU32((c.SP + 0x10u), c.A3);
        c.V0 = c.S6 << 2;
        c.S5 = c.V0 + c.S6;
        L80023CC4: ;
        Poll(c, mem);
        c.A1 = mem.ReadU8(c.FP);
        c.S3 = c.S4 & 0xFFFFu;
        c.V1 = c.S3 << 2;
        c.T0 = 0x80160000u;
        c.T0 = c.T0 - 0x2F70u;
        c.V1 = c.V1 + c.T0;
        c.A0 = 0u | 0x834Cu;
        c.A0 = c.V1 + c.A0;
        c.V0 = 0u | 0x830Cu;
        c.V1 = c.V1 + c.V0;
        c.S0 = mem.ReadU32(c.A0);
        c.S2 = mem.ReadU32(c.V1);
        if (c.A1 == 0u) {
            c.FP = c.FP + 0x1u;
            goto L80023E04;
        }
        c.FP = c.FP + 0x1u;
        if (c.S0 != 0u) {
            c.V0 = c.S4 & 0x000Fu;
            goto L80023D18;
        }
        c.V0 = c.S4 & 0x000Fu;
        c.V0 = c.S7 + c.V0;
        c.A0 = mem.ReadU32((c.SP + 0x40u));
        c.A3 = mem.ReadU8((c.V0 + 0xC8u));
        c.A1 = c.S5;
        goto L80023D64;
        L80023D18: ;
        c.A0 = c.S0;
        c.RA = 0x80023D20u;
        ShadowTower_st.func_80015258(c, mem);
        c.V0 = c.V0 & 0x00FFu;
        c.T0 = 0x00000001u;
        if (c.V0 != c.T0) {
            goto L80023D7C;
        }
        c.A0 = c.S0;
        c.RA = 0x80023D38u;
        ShadowTower_st.func_8001526C(c, mem);
        c.V1 = c.S4 & 0x000Fu;
        c.S1 = c.S7 + c.V1;
        c.V1 = mem.ReadU8((c.S1 + 0xC8u));
        c.V0 = c.V0 & 0x00FFu;
        if (c.V0 == c.V1) {
            c.A0 = c.S0;
            goto L80023D74;
        }
        c.A0 = c.S0;
        c.A0 = c.S0;
        c.RA = 0x80023D58u;
        ShadowTower_st.func_80015230(c, mem);
        c.A1 = c.S5;
        c.A0 = mem.ReadU32((c.SP + 0x40u));
        c.A3 = mem.ReadU8((c.S1 + 0xC8u));
        L80023D64: ;
        c.A2 = c.S3;
        c.RA = 0x80023D6Cu;
        ShadowTower_st.func_800237A8(c, mem);
        goto L80023D7C;
        L80023D74: ;
        c.A1 = 0x00000002u;
        c.RA = 0x80023D7Cu;
        ShadowTower_st.func_80015250(c, mem);
        L80023D7C: ;
        if (c.S2 != 0u) {
            c.V0 = c.S4 & 0x000Fu;
            goto L80023D98;
        }
        c.V0 = c.S4 & 0x000Fu;
        c.V0 = c.S7 + c.V0;
        c.A0 = mem.ReadU32((c.SP + 0x40u));
        c.A3 = mem.ReadU8((c.V0 + 0xC8u));
        c.A1 = c.S5;
        goto L80023DE4;
        L80023D98: ;
        c.A0 = c.S2;
        c.RA = 0x80023DA0u;
        ShadowTower_st.func_80015258(c, mem);
        c.V0 = c.V0 & 0x00FFu;
        c.T0 = 0x00000001u;
        if (c.V0 != c.T0) {
            goto L80023E88;
        }
        c.A0 = c.S2;
        c.RA = 0x80023DB8u;
        ShadowTower_st.func_8001526C(c, mem);
        c.V1 = c.S4 & 0x000Fu;
        c.S0 = c.S7 + c.V1;
        c.V1 = mem.ReadU8((c.S0 + 0xC8u));
        c.V0 = c.V0 & 0x00FFu;
        if (c.V0 == c.V1) {
            c.A0 = c.S2;
            goto L80023DF4;
        }
        c.A0 = c.S2;
        c.A0 = c.S2;
        c.RA = 0x80023DD8u;
        ShadowTower_st.func_80015230(c, mem);
        c.A1 = c.S5;
        c.A0 = mem.ReadU32((c.SP + 0x40u));
        c.A3 = mem.ReadU8((c.S0 + 0xC8u));
        L80023DE4: ;
        c.A2 = c.S4 & 0xFFFFu;
        c.RA = 0x80023DECu;
        ShadowTower_st.func_800239EC(c, mem);
        c.S5 = c.S5 + 0x5u;
        goto L80023E8C;
        L80023DF4: ;
        c.A1 = 0x00000002u;
        c.RA = 0x80023DFCu;
        ShadowTower_st.func_80015250(c, mem);
        c.S5 = c.S5 + 0x5u;
        goto L80023E8C;
        L80023E04: ;
        if (c.S0 == 0u) {
            goto L80023E2C;
        }
        c.A0 = c.S0;
        c.RA = 0x80023E14u;
        ShadowTower_st.func_80015258(c, mem);
        c.V0 = c.V0 & 0x00FFu;
        c.V1 = 0x00000003u;
        if (c.V0 == c.V1) {
            c.A0 = c.S0;
            goto L80023E2C;
        }
        c.A0 = c.S0;
        c.A1 = 0x00000001u;
        c.RA = 0x80023E2Cu;
        ShadowTower_st.func_80015250(c, mem);
        L80023E2C: ;
        if (c.S2 == 0u) {
            c.V1 = c.S6 << 3;
            goto L80023E58;
        }
        c.V1 = c.S6 << 3;
        c.A0 = c.S2;
        c.RA = 0x80023E3Cu;
        ShadowTower_st.func_80015258(c, mem);
        c.V0 = c.V0 & 0x00FFu;
        c.V1 = 0x00000003u;
        if (c.V0 == c.V1) {
            c.A0 = c.S2;
            goto L80023E54;
        }
        c.A0 = c.S2;
        c.A1 = 0x00000001u;
        c.RA = 0x80023E54u;
        ShadowTower_st.func_80015250(c, mem);
        L80023E54: ;
        c.V1 = c.S6 << 3;
        L80023E58: ;
        c.V0 = 0x801E0000u;
        c.V0 = c.V0 - 0x2DD8u;
        c.V1 = c.V1 + c.V0;
        c.V1 = mem.ReadU32(c.V1);
        if (c.V1 == 0u) {
            c.T0 = 0x00000001u;
            goto L80023E88;
        }
        c.T0 = 0x00000001u;
        c.V0 = (uint)(short)mem.ReadU16((c.V1 + 0x4u));
        if (c.V0 != c.T0) {
            c.V0 = 0x00000002u;
            goto L80023E88;
        }
        c.V0 = 0x00000002u;
        mem.WriteU16((c.V1 + 0x4u), (ushort)c.V0);
        L80023E88: ;
        c.S5 = c.S5 + 0x5u;
        L80023E8C: ;
        c.S6 = c.S6 + 0x1u;
        c.T0 = mem.ReadU32((c.SP + 0x10u));
        c.V0 = (int)c.S6 < (int)c.T0 ? 1u : 0u;
        if (c.V0 != 0u) {
            c.S4 = c.S4 + 0x1u;
            goto L80023CC4;
        }
        c.S4 = c.S4 + 0x1u;
        L80023EA4: ;
        c.RA = mem.ReadU32((c.SP + 0x3Cu));
        c.FP = mem.ReadU32((c.SP + 0x38u));
        c.S7 = mem.ReadU32((c.SP + 0x34u));
        c.S6 = mem.ReadU32((c.SP + 0x30u));
        c.S5 = mem.ReadU32((c.SP + 0x2Cu));
        c.S4 = mem.ReadU32((c.SP + 0x28u));
        c.S3 = mem.ReadU32((c.SP + 0x24u));
        c.S2 = mem.ReadU32((c.SP + 0x20u));
        c.S1 = mem.ReadU32((c.SP + 0x1Cu));
        c.S0 = mem.ReadU32((c.SP + 0x18u));
        c.SP = c.SP + 0x40u;
        return;
    }
}
