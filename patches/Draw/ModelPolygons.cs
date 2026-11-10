using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_80022010</c>: every model polygon: FT3 to GT4, semi-transparent or not; called by func_80022678 and func_80021AC4. <c>ST_MODELPOLYGONS</c>.
/// </summary>
public static class ModelPolygons
{
    public static readonly Literal R = new(typeof(ModelPolygons), "model polygons", "MODELPOLYGONS", 0x80022010, Run,
        "every model polygon: FT3 to GT4, semi-transparent or not; called by func_80022678 and func_80021AC4.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0xA0u;
        mem.WriteU32((c.SP + 0x84u), c.S3);
        c.S3 = 0x1F800000u;
        mem.WriteU32((c.SP + 0x9Cu), c.RA);
        mem.WriteU32((c.SP + 0x98u), c.FP);
        mem.WriteU32((c.SP + 0x94u), c.S7);
        mem.WriteU32((c.SP + 0x90u), c.S6);
        mem.WriteU32((c.SP + 0x8Cu), c.S5);
        mem.WriteU32((c.SP + 0x88u), c.S4);
        mem.WriteU32((c.SP + 0x80u), c.S2);
        mem.WriteU32((c.SP + 0x7Cu), c.S1);
        mem.WriteU32((c.SP + 0x78u), c.S0);
        mem.WriteU32((c.SP + 0xA4u), c.A1);
        c.V0 = mem.ReadU32((c.S3 + 0xA4u));
        c.S4 = c.A0;
        c.A3 = mem.ReadU32(c.V0);
        c.S2 = mem.ReadU32((c.V0 + 0x4u));
        c.V0 = 0x80010000u;
        c.A2 = c.V0 + 0x1010u;
        c.V1 = mem.ReadU32(c.A2);
        c.A0 = mem.ReadU32((c.A2 + 0x4u));
        c.A1 = mem.ReadU32((c.A2 + 0x8u));
        mem.WriteU32((c.SP + 0x30u), c.V1);
        mem.WriteU32((c.SP + 0x34u), c.A0);
        mem.WriteU32((c.SP + 0x38u), c.A1);
        c.V1 = mem.ReadU32((c.A2 + 0xCu));
        c.A0 = mem.ReadU32((c.A2 + 0x10u));
        c.A1 = mem.ReadU32((c.A2 + 0x14u));
        mem.WriteU32((c.SP + 0x3Cu), c.V1);
        mem.WriteU32((c.SP + 0x40u), c.A0);
        mem.WriteU32((c.SP + 0x44u), c.A1);
        c.V1 = mem.ReadU32((c.A2 + 0x18u));
        c.A0 = mem.ReadU32((c.A2 + 0x1Cu));
        mem.WriteU32((c.SP + 0x48u), c.V1);
        mem.WriteU32((c.SP + 0x4Cu), c.A0);
        c.V0 = 0xFFFFFFFFu;
        c.S2 = c.S2 - 0x1u;
        if (c.S2 == c.V0) {
            goto L800220C4;
        }
        c.V1 = c.V0;
        L800220B0: ;
        Poll(c, mem);
        c.V0 = mem.ReadU32((c.A3 + 0x4u));
        c.A3 = c.A3 + 0x10u;
        c.S2 = c.S2 - 0x1u;
        if (c.S2 != c.V1) {
            mem.WriteU16(c.V0, (ushort)0u);
            goto L800220B0;
        }
        mem.WriteU16(c.V0, (ushort)0u);
        L800220C4: ;
        c.V0 = mem.ReadU32((c.S3 + 0xA4u));
        c.S2 = mem.ReadU32((c.V0 + 0x4u));
        c.A3 = mem.ReadU32(c.V0);
        c.V0 = 0xFFFFFFFFu;
        c.S2 = c.S2 - 0x1u;
        if (c.S2 == c.V0) {
            c.FP = c.V0;
            goto L80022644;
        }
        c.FP = c.V0;
        c.S5 = c.SP + 0x50u;
        c.V0 = 0x801A0000u;
        c.S7 = c.V0 - 0x70D8u;
        c.V0 = 0x80170000u;
        c.S6 = c.V0 + 0x5090u;
        c.S1 = c.A3 + 0x8u;
        L800220FC: ;
        Poll(c, mem);
        c.V0 = mem.ReadU32((c.S1 - 0x4u));
        mem.WriteU16(c.V0, (ushort)0u);
        c.V0 = mem.ReadU32((c.S1 + 0x4u));
        if (c.V0 == c.FP) {
            c.V0 = 0u | 0xFFFFu;
            goto L80022634;
        }
        c.V0 = 0u | 0xFFFFu;
        c.V1 = mem.ReadU16(c.S1);
        if (c.V1 == c.V0) {
            c.V0 = 0x00000002u;
            goto L80022634;
        }
        c.V0 = 0x00000002u;
        c.V1 = mem.ReadU8((c.S1 + 0x2u));
        if (c.V1 != c.V0) {
            c.V0 = 0x80010000u;
            goto L80022338;
        }
        c.V0 = 0x80010000u;
        c.A2 = c.V0 + 0x1010u;
        c.V1 = mem.ReadU32(c.A2);
        c.A0 = mem.ReadU32((c.A2 + 0x4u));
        c.A1 = mem.ReadU32((c.A2 + 0x8u));
        mem.WriteU32((c.SP + 0x50u), c.V1);
        mem.WriteU32((c.SP + 0x54u), c.A0);
        mem.WriteU32((c.SP + 0x58u), c.A1);
        c.V1 = mem.ReadU32((c.A2 + 0xCu));
        c.A0 = mem.ReadU32((c.A2 + 0x10u));
        c.A1 = mem.ReadU32((c.A2 + 0x14u));
        mem.WriteU32((c.SP + 0x5Cu), c.V1);
        mem.WriteU32((c.SP + 0x60u), c.A0);
        mem.WriteU32((c.SP + 0x64u), c.A1);
        c.V1 = mem.ReadU32((c.A2 + 0x18u));
        c.A0 = mem.ReadU32((c.A2 + 0x1Cu));
        mem.WriteU32((c.SP + 0x68u), c.V1);
        mem.WriteU32((c.SP + 0x6Cu), c.A0);
        c.V0 = mem.ReadU16((c.S7 + 0x268u));
        c.A1 = c.S5;
        c.V0 = 0u - c.V0;
        c.A0 = c.V0 << 16;
        c.A0 = (uint)((int)c.A0 >> 16);
        mem.WriteU16((c.SP + 0x70u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S7 + 0x26Au));
        c.V1 = mem.ReadU16((c.S7 + 0x26Cu));
        c.V0 = 0u - c.V0;
        c.V1 = 0u - c.V1;
        mem.WriteU16((c.SP + 0x72u), (ushort)c.V0);
        mem.WriteU16((c.SP + 0x74u), (ushort)c.V1);
        c.RA = 0x800221B0u;
        ShadowTower_st.RotMatrixX_st(c, mem);
        c.T3 = mem.ReadU32((c.SP + 0xA4u));
        c.V0 = (uint)(short)mem.ReadU16((c.SP + 0x72u));
        c.A0 = (uint)(short)mem.ReadU16((c.T3 + 0x2u));
        c.A1 = c.S5;
        c.A0 = c.V0 + c.A0;
        c.RA = 0x800221C8u;
        ShadowTower_st.RotMatrixY_st(c, mem);
        c.A0 = (uint)(short)mem.ReadU16((c.SP + 0x74u));
        c.A1 = c.S5;
        c.RA = 0x800221D4u;
        ShadowTower_st.RotMatrixZ_st(c, mem);
        c.V0 = mem.ReadU32((c.S1 - 0x4u));
        c.A1 = mem.ReadU32((c.V0 + 0x48u));
        c.V0 = mem.ReadU16((c.A1 + 0x2u));
        if (c.V0 != 0u) {
            goto L80022204;
        }
        c.A0 = mem.ReadU32((c.A1 + 0x44u));
        c.A1 = c.A1 + 0x4u;
        c.A0 = c.A0 + 0x10u;
        c.RA = 0x80022204u;
        ShadowTower_st.RotMatrix_st(c, mem);
        L80022204: ;
        c.V0 = mem.ReadU32((c.S1 - 0x4u));
        c.V0 = mem.ReadU32((c.V0 + 0x48u));
        c.V0 = c.V0 + 0x4u;
        c.T4 = mem.ReadU32(c.V0);
        c.T5 = mem.ReadU32((c.V0 + 0x4u));
        RecompOne.Runtime.Gte.WriteControl(0, c.T4);
        RecompOne.Runtime.Gte.WriteControl(1, c.T5);
        c.T4 = mem.ReadU32((c.V0 + 0x8u));
        c.T5 = mem.ReadU32((c.V0 + 0xCu));
        c.T6 = mem.ReadU32((c.V0 + 0x10u));
        RecompOne.Runtime.Gte.WriteControl(2, c.T4);
        RecompOne.Runtime.Gte.WriteControl(3, c.T5);
        RecompOne.Runtime.Gte.WriteControl(4, c.T6);
        c.T4 = mem.ReadU16(c.S5);
        c.T5 = mem.ReadU16((c.S5 + 0x6u));
        c.T6 = mem.ReadU16((c.S5 + 0xCu));
        RecompOne.Runtime.Gte.Write(9, c.T4);
        RecompOne.Runtime.Gte.Write(10, c.T5);
        RecompOne.Runtime.Gte.Write(11, c.T6);
        RecompOne.Runtime.Gte.MvmvaOp(12, false, 0, 3, 3);
        c.T4 = RecompOne.Runtime.Gte.Read(9);
        c.T5 = RecompOne.Runtime.Gte.Read(10);
        c.T6 = RecompOne.Runtime.Gte.Read(11);
        mem.WriteU16(c.V0, (ushort)c.T4);
        mem.WriteU16((c.V0 + 0x6u), (ushort)c.T5);
        mem.WriteU16((c.V0 + 0xCu), (ushort)c.T6);
        c.V0 = c.SP + 0x52u;
        c.T4 = mem.ReadU16(c.V0);
        c.T5 = mem.ReadU16((c.V0 + 0x6u));
        c.T6 = mem.ReadU16((c.V0 + 0xCu));
        RecompOne.Runtime.Gte.Write(9, c.T4);
        RecompOne.Runtime.Gte.Write(10, c.T5);
        RecompOne.Runtime.Gte.Write(11, c.T6);
        RecompOne.Runtime.Gte.MvmvaOp(12, false, 0, 3, 3);
        c.V0 = mem.ReadU32((c.S1 - 0x4u));
        c.V0 = mem.ReadU32((c.V0 + 0x48u));
        c.V0 = c.V0 + 0x6u;
        c.T4 = RecompOne.Runtime.Gte.Read(9);
        c.T5 = RecompOne.Runtime.Gte.Read(10);
        c.T6 = RecompOne.Runtime.Gte.Read(11);
        mem.WriteU16(c.V0, (ushort)c.T4);
        mem.WriteU16((c.V0 + 0x6u), (ushort)c.T5);
        mem.WriteU16((c.V0 + 0xCu), (ushort)c.T6);
        c.V0 = c.SP + 0x54u;
        c.T4 = mem.ReadU16(c.V0);
        c.T5 = mem.ReadU16((c.V0 + 0x6u));
        c.T6 = mem.ReadU16((c.V0 + 0xCu));
        RecompOne.Runtime.Gte.Write(9, c.T4);
        RecompOne.Runtime.Gte.Write(10, c.T5);
        RecompOne.Runtime.Gte.Write(11, c.T6);
        RecompOne.Runtime.Gte.MvmvaOp(12, false, 0, 3, 3);
        c.V0 = mem.ReadU32((c.S1 - 0x4u));
        c.V0 = mem.ReadU32((c.V0 + 0x48u));
        c.V0 = c.V0 + 0x8u;
        c.T4 = RecompOne.Runtime.Gte.Read(9);
        c.T5 = RecompOne.Runtime.Gte.Read(10);
        c.T6 = RecompOne.Runtime.Gte.Read(11);
        mem.WriteU16(c.V0, (ushort)c.T4);
        mem.WriteU16((c.V0 + 0x6u), (ushort)c.T5);
        mem.WriteU16((c.V0 + 0xCu), (ushort)c.T6);
        c.V0 = mem.ReadU32((c.S1 - 0x4u));
        c.V0 = mem.ReadU32((c.V0 + 0x48u));
        mem.WriteU16((c.V0 + 0x2u), (ushort)0u);
        L80022338: ;
        c.A0 = mem.ReadU32((c.S1 - 0x4u));
        c.RA = 0x80022344u;
        ShadowTower_st.func_80035260(c, mem);
        c.V1 = mem.ReadU8((c.S1 + 0x2u));
        c.V0 = 0x00000001u;
        if (c.V1 != c.V0) {
            c.V0 = 0x00000003u;
            goto L800223F4;
        }
        c.V0 = 0x00000003u;
        c.V0 = 0x2AAA0000u;
        c.V0 = c.V0 | 0xAAABu;
        c.LO = (uint)((long)(int)c.S2 * (int)c.V0); c.HI = (uint)(((long)(int)c.S2 * (int)c.V0) >> 32);
        c.V1 = c.HI;
        c.A1 = mem.ReadU8((c.S6 + 0x6CC0u));
        c.V0 = 0xAAAA0000u;
        c.V0 = c.V0 | 0xAAABu;
        c.LO = (uint)((ulong)c.A1 * c.V0); c.HI = (uint)(((ulong)c.A1 * c.V0) >> 32);
        c.V0 = (uint)((int)c.S2 >> 31);
        c.V0 = c.V1 - c.V0;
        c.V1 = c.V0 << 1;
        c.V1 = c.V1 + c.V0;
        c.V1 = c.V1 << 1;
        c.V1 = c.S2 - c.V1;
        c.A0 = c.HI;
        c.A0 = c.A0 >> 2;
        c.V0 = c.A0 << 1;
        c.V0 = c.V0 + c.A0;
        c.V0 = c.V0 << 1;
        c.A1 = c.A1 - c.V0;
        c.A1 = c.A1 & 0x00FFu;
        if (c.V1 != c.A1) {
            c.S0 = c.SP + 0x30u;
            goto L80022634;
        }
        c.S0 = c.SP + 0x30u;
        c.V0 = mem.ReadU32((c.S1 - 0x4u));
        c.A1 = mem.ReadU32((c.V0 + 0x44u));
        c.A0 = c.S0;
        c.RA = 0x800223C4u;
        ShadowTower_st.ScaleMatrix(c, mem);
        c.T4 = mem.ReadU32(c.S0);
        c.T5 = mem.ReadU32((c.S0 + 0x4u));
        RecompOne.Runtime.Gte.WriteControl(0, c.T4);
        RecompOne.Runtime.Gte.WriteControl(1, c.T5);
        c.T4 = mem.ReadU32((c.S0 + 0x8u));
        c.T5 = mem.ReadU32((c.S0 + 0xCu));
        c.T6 = mem.ReadU32((c.S0 + 0x10u));
        RecompOne.Runtime.Gte.WriteControl(2, c.T4);
        RecompOne.Runtime.Gte.WriteControl(3, c.T5);
        RecompOne.Runtime.Gte.WriteControl(4, c.T6);
        goto L800224D4;
        L800223F4: ;
        if (c.V1 != c.V0) {
            c.V0 = 0x00000004u;
            goto L80022468;
        }
        c.V0 = 0x00000004u;
        if ((int)c.S2 >= 0) {
            c.V1 = c.S2;
            goto L80022408;
        }
        c.V1 = c.S2;
        c.V1 = c.S2 + 0x3u;
        L80022408: ;
        c.V1 = (uint)((int)c.V1 >> 2);
        c.V1 = c.V1 << 2;
        c.V0 = mem.ReadU8((c.S6 + 0x6CC0u));
        c.V1 = c.S2 - c.V1;
        c.V0 = c.V0 & 0x0003u;
        if (c.V1 != c.V0) {
            c.S0 = c.SP + 0x30u;
            goto L80022634;
        }
        c.S0 = c.SP + 0x30u;
        c.V0 = mem.ReadU32((c.S1 - 0x4u));
        c.A1 = mem.ReadU32((c.V0 + 0x44u));
        c.A0 = c.S0;
        c.RA = 0x80022438u;
        ShadowTower_st.ScaleMatrix(c, mem);
        c.T4 = mem.ReadU32(c.S0);
        c.T5 = mem.ReadU32((c.S0 + 0x4u));
        RecompOne.Runtime.Gte.WriteControl(0, c.T4);
        RecompOne.Runtime.Gte.WriteControl(1, c.T5);
        c.T4 = mem.ReadU32((c.S0 + 0x8u));
        c.T5 = mem.ReadU32((c.S0 + 0xCu));
        c.T6 = mem.ReadU32((c.S0 + 0x10u));
        RecompOne.Runtime.Gte.WriteControl(2, c.T4);
        RecompOne.Runtime.Gte.WriteControl(3, c.T5);
        RecompOne.Runtime.Gte.WriteControl(4, c.T6);
        goto L800224D4;
        L80022468: ;
        if (c.V1 != c.V0) {
            goto L800224D4;
        }
        if ((int)c.S2 >= 0) {
            c.V1 = c.S2;
            goto L8002247C;
        }
        c.V1 = c.S2;
        c.V1 = c.S2 + 0x7u;
        L8002247C: ;
        c.V1 = (uint)((int)c.V1 >> 3);
        c.V1 = c.V1 << 3;
        c.V0 = mem.ReadU8((c.S6 + 0x6CC0u));
        c.V1 = c.S2 - c.V1;
        c.V0 = c.V0 & 0x0007u;
        if (c.V1 != c.V0) {
            c.S0 = c.SP + 0x30u;
            goto L80022634;
        }
        c.S0 = c.SP + 0x30u;
        c.V0 = mem.ReadU32((c.S1 - 0x4u));
        c.A1 = mem.ReadU32((c.V0 + 0x44u));
        c.A0 = c.S0;
        c.RA = 0x800224ACu;
        ShadowTower_st.ScaleMatrix(c, mem);
        c.T4 = mem.ReadU32(c.S0);
        c.T5 = mem.ReadU32((c.S0 + 0x4u));
        RecompOne.Runtime.Gte.WriteControl(0, c.T4);
        RecompOne.Runtime.Gte.WriteControl(1, c.T5);
        c.T4 = mem.ReadU32((c.S0 + 0x8u));
        c.T5 = mem.ReadU32((c.S0 + 0xCu));
        c.T6 = mem.ReadU32((c.S0 + 0x10u));
        RecompOne.Runtime.Gte.WriteControl(2, c.T4);
        RecompOne.Runtime.Gte.WriteControl(3, c.T5);
        RecompOne.Runtime.Gte.WriteControl(4, c.T6);
        L800224D4: ;
        c.V1 = mem.ReadU16(c.S1);
        c.A3 = mem.ReadU32((c.S3 + 0x40u));
        c.V0 = c.V1 << 2;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 3;
        c.V1 = mem.ReadU32((c.S3 + 0xCu));
        c.V0 = c.V0 + 0xCu;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32((c.S3 + 0x20u), c.V1);
        c.V1 = mem.ReadU32(c.V1);
        c.V0 = mem.ReadU32((c.S3 + 0xCu));
        c.A0 = mem.ReadU32((c.S3 + 0x20u));
        c.V1 = c.V1 + 0xCu;
        c.V0 = c.V0 + c.V1;
        mem.WriteU32((c.S3 + 0x48u), c.V0);
        c.A2 = mem.ReadU32((c.A0 + 0x4u));
        c.A2 = c.A2 - 0x1u;
        if (c.A2 == c.FP) {
            c.T0 = c.V0;
            goto L800225EC;
        }
        c.T0 = c.V0;
        c.T2 = 0x00000FE1u;
        c.T1 = 0xFFFFFFFFu;
        c.A1 = c.A3 + 0x6u;
        L80022530: ;
        Poll(c, mem);
        RecompOne.Runtime.Gte.Write(0, (mem.ReadU32(c.T0)));
        RecompOne.Runtime.Gte.Write(1, (mem.ReadU32((c.T0 + 0x4u))));
        RecompOne.Runtime.Gte.Rtps(12, false);
        mem.WriteU32(c.A3, (RecompOne.Runtime.Gte.Read(14)));
        c.V0 = c.A3 + 0x4u;
        mem.WriteU32(c.V0, (RecompOne.Runtime.Gte.Read(19)));
        c.A0 = mem.ReadU32((c.S3 + 0x64u));
        c.V0 = mem.ReadU32((c.S3 + 0x68u));
        if (c.A0 == c.V0) {
            c.V0 = c.V0 - c.A0;
            goto L800225A8;
        }
        c.V0 = c.V0 - c.A0;
        c.V1 = (uint)(short)mem.ReadU16((c.A1 - 0x2u));
        c.V1 = c.V1 - c.A0;
        c.V1 = c.V1 << 12;
        if (c.V0 != 0u) { if ((int)c.V1 == int.MinValue && (int)c.V0 == -1) { c.LO = 0x80000000u; c.HI = 0u; } else { c.LO = (uint)((int)c.V1 / (int)c.V0); c.HI = (uint)((int)c.V1 % (int)c.V0); } }
        if (c.V0 != 0u) {
            goto L80022584;
        }
        Bios.Break(c, mem);
        L80022584: ;
        c.At = 0xFFFFFFFFu;
        if (c.V0 != c.At) {
            c.At = 0x80000000u;
            goto L8002259C;
        }
        c.At = 0x80000000u;
        if (c.V1 != c.At) {
            goto L8002259C;
        }
        Bios.Break(c, mem);
        L8002259C: ;
        c.V1 = c.LO;
        mem.WriteU16(c.A1, (ushort)c.V1);
        goto L800225AC;
        L800225A8: ;
        mem.WriteU16(c.A1, (ushort)0u);
        L800225AC: ;
        c.V0 = (uint)(short)mem.ReadU16(c.A1);
        c.V0 = (int)c.V0 < 4066 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L800225C4;
        }
        mem.WriteU16(c.A1, (ushort)c.T2);
        L800225C4: ;
        c.V0 = (uint)(short)mem.ReadU16(c.A1);
        if ((int)c.V0 >= 0) {
            goto L800225D8;
        }
        mem.WriteU16(c.A1, (ushort)0u);
        L800225D8: ;
        c.A1 = c.A1 + 0x8u;
        c.A3 = c.A3 + 0x8u;
        c.A2 = c.A2 - 0x1u;
        if (c.A2 != c.T1) {
            c.T0 = c.T0 + 0x8u;
            goto L80022530;
        }
        c.T0 = c.T0 + 0x8u;
        L800225EC: ;
        c.V0 = c.S4 & 0x0040u;
        if (c.V0 == 0u) {
            c.V0 = c.S4 & 0x0080u;
            goto L8002260C;
        }
        c.V0 = c.S4 & 0x0080u;
        c.A0 = mem.ReadU16(c.S1);
        c.A1 = c.S4 & 0x00FFu;
        c.RA = 0x80022604u;
        ShadowTower_st.func_8001BF10(c, mem);
        c.S1 = c.S1 + 0x10u;
        goto L80022638;
        L8002260C: ;
        if (c.V0 == 0u) {
            goto L80022628;
        }
        c.A0 = mem.ReadU16(c.S1);
        c.A1 = c.S4 & 0x00FFu;
        c.RA = 0x80022620u;
        ShadowTower_st.func_800192D4(c, mem);
        c.S1 = c.S1 + 0x10u;
        goto L80022638;
        L80022628: ;
        c.A0 = mem.ReadU16(c.S1);
        c.A1 = c.S4 & 0x00FFu;
        c.RA = 0x80022634u;
        ShadowTower_st.func_80019DC8(c, mem);
        L80022634: ;
        c.S1 = c.S1 + 0x10u;
        L80022638: ;
        c.S2 = c.S2 - 0x1u;
        if (c.S2 != c.FP) {
            goto L800220FC;
        }
        L80022644: ;
        c.RA = mem.ReadU32((c.SP + 0x9Cu));
        c.FP = mem.ReadU32((c.SP + 0x98u));
        c.S7 = mem.ReadU32((c.SP + 0x94u));
        c.S6 = mem.ReadU32((c.SP + 0x90u));
        c.S5 = mem.ReadU32((c.SP + 0x8Cu));
        c.S4 = mem.ReadU32((c.SP + 0x88u));
        c.S3 = mem.ReadU32((c.SP + 0x84u));
        c.S2 = mem.ReadU32((c.SP + 0x80u));
        c.S1 = mem.ReadU32((c.SP + 0x7Cu));
        c.S0 = mem.ReadU32((c.SP + 0x78u));
        c.SP = c.SP + 0xA0u;
        return;
    }
}
