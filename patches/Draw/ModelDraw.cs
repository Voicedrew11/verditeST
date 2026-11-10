using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_80022678</c>: one model: its matrix, light and pose, then func_80022010 for its polygons. <c>ST_MODELDRAW</c>.
/// </summary>
public static class ModelDraw
{
    public static readonly Literal R = new(typeof(ModelDraw), "model draw", "MODELDRAW", 0x80022678, Run,
        "one model: its matrix, light and pose, then func_80022010 for its polygons.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0xD0u;
        mem.WriteU32((c.SP + 0xACu), c.S1);
        c.S1 = c.A2;
        mem.WriteU32((c.SP + 0xA8u), c.S0);
        c.S0 = 0x1F800000u;
        c.A0 = 0x80160000u;
        mem.WriteU32((c.SP + 0xC4u), c.S7);
        c.S7 = mem.ReadU32((c.SP + 0xE0u));
        c.V1 = 0x80160000u;
        mem.WriteU32((c.SP + 0xC8u), c.FP);
        c.FP = mem.ReadU32((c.SP + 0xE8u));
        c.T2 = mem.ReadU32((c.SP + 0xFCu));
        c.V1 = c.V1 - 0x2F70u;
        mem.WriteU32((c.SP + 0xBCu), c.S5);
        c.S5 = mem.ReadU16((c.SP + 0xECu));
        c.T3 = mem.ReadU16((c.SP + 0xF0u));
        c.V0 = 0u | 0x8000u;
        mem.WriteU32((c.SP + 0xB0u), c.S2);
        c.S2 = mem.ReadU16((c.SP + 0xF4u));
        c.V1 = c.V1 + c.V0;
        mem.WriteU32((c.SP + 0xCCu), c.RA);
        mem.WriteU32((c.SP + 0xC0u), c.S6);
        mem.WriteU32((c.SP + 0xB8u), c.S4);
        mem.WriteU32((c.SP + 0xB4u), c.S3);
        mem.WriteU32((c.SP + 0xDCu), c.A3);
        mem.WriteU16((c.SP + 0xA0u), (ushort)c.T3);
        c.A2 = mem.ReadU32((c.V1 + 0x31Cu));
        c.A3 = mem.ReadU32((c.V1 + 0x320u));
        c.T0 = mem.ReadU32((c.V1 + 0x324u));
        c.T1 = mem.ReadU32((c.V1 + 0x328u));
        c.V0 = mem.ReadU32((c.V1 + 0x304u));
        c.A0 = c.A0 + 0x7BCCu;
        mem.WriteU32((c.S0 + 0x40u), c.A0);
        mem.WriteU32((c.S0 + 0x44u), c.A0);
        mem.WriteU32((c.S0 + 0x88u), c.A2);
        mem.WriteU32((c.S0 + 0x8Cu), c.A3);
        mem.WriteU32((c.S0 + 0x90u), c.T0);
        mem.WriteU32((c.S0 + 0x94u), c.T1);
        c.A0 = mem.ReadU32((c.V0 + 0x8u));
        mem.WriteU32((c.S0 + 0x10u), c.A0);
        c.V1 = mem.ReadU32((c.V0 + 0x4u));
        c.S4 = c.T2;
        c.V0 = c.V1 - 0x7800u;
        c.V0 = c.V0 < c.A0 ? 1u : 0u;
        mem.WriteU32((c.S0 + 0x14u), c.V1);
        if (c.V0 == 0u) {
            mem.WriteU16((c.SP + 0x9Cu), (ushort)c.A1);
            goto L8002273C;
        }
        mem.WriteU16((c.SP + 0x9Cu), (ushort)c.A1);
        c.S4 = c.T2 & 0x00BFu;
        L8002273C: ;
        c.V0 = c.V1 < c.A0 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.A0 = 0u | 0xFFFFu;
            goto L80022764;
        }
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
        L80022764: ;
        c.T3 = 0x80180000u;
        c.T3 = c.T3 - 0x4288u;
        c.T4 = mem.ReadU32(c.T3);
        c.T5 = mem.ReadU32((c.T3 + 0x4u));
        RecompOne.Runtime.Gte.WriteControl(0, c.T4);
        RecompOne.Runtime.Gte.WriteControl(1, c.T5);
        c.T4 = mem.ReadU32((c.T3 + 0x8u));
        c.T5 = mem.ReadU32((c.T3 + 0xCu));
        c.T6 = mem.ReadU32((c.T3 + 0x10u));
        RecompOne.Runtime.Gte.WriteControl(2, c.T4);
        RecompOne.Runtime.Gte.WriteControl(3, c.T5);
        RecompOne.Runtime.Gte.WriteControl(4, c.T6);
        c.T3 = 0x80180000u;
        c.T3 = c.T3 - 0x4288u;
        c.T4 = mem.ReadU32((c.T3 + 0x14u));
        c.T5 = mem.ReadU32((c.T3 + 0x18u));
        RecompOne.Runtime.Gte.WriteControl(5, c.T4);
        c.T6 = mem.ReadU32((c.T3 + 0x1Cu));
        RecompOne.Runtime.Gte.WriteControl(6, c.T5);
        RecompOne.Runtime.Gte.WriteControl(7, c.T6);
        if (c.FP == 0u) {
            c.A1 = 0x801A0000u;
            goto L8002288C;
        }
        c.A1 = 0x801A0000u;
        c.A1 = c.A1 - 0x70D8u;
        c.V0 = mem.ReadU32(c.S1);
        c.A0 = mem.ReadU32((c.A1 + 0x248u));
        c.A2 = (uint)(short)mem.ReadU16((c.A1 + 0x2CEu));
        c.V1 = mem.ReadU32((c.A1 + 0x24Cu));
        c.V0 = c.V0 - c.A0;
        c.V1 = c.V1 + c.A2;
        mem.WriteU32((c.SP + 0x18u), c.V0);
        c.A0 = (uint)(short)mem.ReadU16((c.A1 + 0x2D2u));
        c.V0 = mem.ReadU32((c.S1 + 0x4u));
        c.V1 = c.V1 + c.A0;
        c.V0 = c.V0 - c.V1;
        mem.WriteU32((c.SP + 0x1Cu), c.V0);
        c.V0 = mem.ReadU32((c.S1 + 0x8u));
        c.V1 = mem.ReadU32((c.A1 + 0x250u));
        c.V0 = c.V0 - c.V1;
        mem.WriteU32((c.SP + 0x20u), c.V0);
        c.A2 = mem.ReadU16((c.A1 + 0x2CEu));
        c.V0 = mem.ReadU16(c.S1);
        c.V1 = mem.ReadU16((c.A1 + 0x248u));
        c.A0 = mem.ReadU16((c.A1 + 0x24Cu));
        c.V0 = c.V0 - c.V1;
        c.A0 = c.A0 + c.A2;
        mem.WriteU16((c.SP + 0x10u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.A1 + 0x2D2u));
        c.V1 = mem.ReadU16((c.S1 + 0x4u));
        c.V0 = c.V0 + c.A0;
        c.V1 = c.V1 - c.V0;
        mem.WriteU16((c.SP + 0x12u), (ushort)c.V1);
        c.V0 = mem.ReadU16((c.S1 + 0x8u));
        c.V1 = mem.ReadU16((c.A1 + 0x250u));
        c.V0 = c.V0 - c.V1;
        mem.WriteU16((c.SP + 0x14u), (ushort)c.V0);
        c.V0 = c.SP + 0x10u;
        RecompOne.Runtime.Gte.Write(0, (mem.ReadU32(c.V0)));
        RecompOne.Runtime.Gte.Write(1, (mem.ReadU32((c.V0 + 0x4u))));
        RecompOne.Runtime.Gte.MvmvaOp(12, false, 0, 0, 0);
        c.T3 = 0x1F800000u;
        c.T3 = c.T3 | 0x00BCu;
        mem.WriteU32(c.T3, (RecompOne.Runtime.Gte.Read(25)));
        mem.WriteU32((c.T3 + 0x4u), (RecompOne.Runtime.Gte.Read(26)));
        mem.WriteU32((c.T3 + 0x8u), (RecompOne.Runtime.Gte.Read(27)));
        c.V0 = c.SP + 0x98u;
        c.T4 = RecompOne.Runtime.Gte.ReadControl(31);
        mem.WriteU32(c.V0, c.T4);
        c.S1 = c.S0 + 0xA8u;
        goto L800228B4;
        L8002288C: ;
        c.V0 = mem.ReadU32(c.S1);
        mem.WriteU32((c.S0 + 0xBCu), c.V0);
        c.V0 = mem.ReadU32((c.S1 + 0x4u));
        mem.WriteU32((c.S0 + 0xC0u), c.V0);
        c.V0 = mem.ReadU32((c.S1 + 0x8u));
        mem.WriteU32((c.S0 + 0xC4u), c.V0);
        c.S1 = c.S0 + 0xA8u;
        L800228B4: ;
        c.A0 = mem.ReadU32((c.SP + 0xDCu));
        c.A1 = c.S1;
        c.RA = 0x800228C0u;
        ShadowTower_st.func_80049E30(c, mem);
        c.A0 = c.S2 << 16;
        c.A0 = (uint)((int)c.A0 >> 16);
        c.S6 = c.S4 & 0x00FFu;
        c.A1 = c.S6;
        c.RA = 0x800228D4u;
        ShadowTower_st.func_80017D28(c, mem);
        c.T3 = mem.ReadU32((c.S0 + 0x394u));
        c.T4 = mem.ReadU32(c.T3);
        c.T5 = mem.ReadU32((c.T3 + 0x4u));
        RecompOne.Runtime.Gte.WriteControl(0, c.T4);
        RecompOne.Runtime.Gte.WriteControl(1, c.T5);
        c.T4 = mem.ReadU32((c.T3 + 0x8u));
        c.T5 = mem.ReadU32((c.T3 + 0xCu));
        c.T6 = mem.ReadU32((c.T3 + 0x10u));
        RecompOne.Runtime.Gte.WriteControl(2, c.T4);
        RecompOne.Runtime.Gte.WriteControl(3, c.T5);
        RecompOne.Runtime.Gte.WriteControl(4, c.T6);
        c.T4 = mem.ReadU16(c.S1);
        c.T5 = mem.ReadU16((c.S1 + 0x6u));
        c.T6 = mem.ReadU16((c.S1 + 0xCu));
        RecompOne.Runtime.Gte.Write(9, c.T4);
        RecompOne.Runtime.Gte.Write(10, c.T5);
        RecompOne.Runtime.Gte.Write(11, c.T6);
        RecompOne.Runtime.Gte.MvmvaOp(12, false, 0, 3, 3);
        c.V1 = c.SP + 0x58u;
        c.T4 = RecompOne.Runtime.Gte.Read(9);
        c.T5 = RecompOne.Runtime.Gte.Read(10);
        c.T6 = RecompOne.Runtime.Gte.Read(11);
        mem.WriteU16(c.V1, (ushort)c.T4);
        mem.WriteU16((c.V1 + 0x6u), (ushort)c.T5);
        mem.WriteU16((c.V1 + 0xCu), (ushort)c.T6);
        c.S3 = c.S0 + 0xAAu;
        c.T4 = mem.ReadU16(c.S3);
        c.T5 = mem.ReadU16((c.S3 + 0x6u));
        c.T6 = mem.ReadU16((c.S3 + 0xCu));
        RecompOne.Runtime.Gte.Write(9, c.T4);
        RecompOne.Runtime.Gte.Write(10, c.T5);
        RecompOne.Runtime.Gte.Write(11, c.T6);
        RecompOne.Runtime.Gte.MvmvaOp(12, false, 0, 3, 3);
        c.V0 = c.SP + 0x5Au;
        c.T4 = RecompOne.Runtime.Gte.Read(9);
        c.T5 = RecompOne.Runtime.Gte.Read(10);
        c.T6 = RecompOne.Runtime.Gte.Read(11);
        mem.WriteU16(c.V0, (ushort)c.T4);
        mem.WriteU16((c.V0 + 0x6u), (ushort)c.T5);
        mem.WriteU16((c.V0 + 0xCu), (ushort)c.T6);
        c.S2 = c.S0 + 0xACu;
        c.T4 = mem.ReadU16(c.S2);
        c.T5 = mem.ReadU16((c.S2 + 0x6u));
        c.T6 = mem.ReadU16((c.S2 + 0xCu));
        RecompOne.Runtime.Gte.Write(9, c.T4);
        RecompOne.Runtime.Gte.Write(10, c.T5);
        RecompOne.Runtime.Gte.Write(11, c.T6);
        RecompOne.Runtime.Gte.MvmvaOp(12, false, 0, 3, 3);
        c.V0 = c.SP + 0x5Cu;
        c.T4 = RecompOne.Runtime.Gte.Read(9);
        c.T5 = RecompOne.Runtime.Gte.Read(10);
        c.T6 = RecompOne.Runtime.Gte.Read(11);
        mem.WriteU16(c.V0, (ushort)c.T4);
        mem.WriteU16((c.V0 + 0x6u), (ushort)c.T5);
        mem.WriteU16((c.V0 + 0xCu), (ushort)c.T6);
        c.T4 = mem.ReadU32(c.V1);
        c.T5 = mem.ReadU32((c.V1 + 0x4u));
        RecompOne.Runtime.Gte.WriteControl(8, c.T4);
        RecompOne.Runtime.Gte.WriteControl(9, c.T5);
        c.T4 = mem.ReadU32((c.V1 + 0x8u));
        c.T5 = mem.ReadU32((c.V1 + 0xCu));
        c.T6 = mem.ReadU32((c.V1 + 0x10u));
        RecompOne.Runtime.Gte.WriteControl(10, c.T4);
        RecompOne.Runtime.Gte.WriteControl(11, c.T5);
        RecompOne.Runtime.Gte.WriteControl(12, c.T6);
        if (c.S7 == 0u) {
            c.A0 = c.S1;
            goto L80022A24;
        }
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
        L80022A24: ;
        if (c.FP == 0u) {
            goto L80022B08;
        }
        c.T4 = mem.ReadU32(c.FP);
        c.T5 = mem.ReadU32((c.FP + 0x4u));
        RecompOne.Runtime.Gte.WriteControl(0, c.T4);
        RecompOne.Runtime.Gte.WriteControl(1, c.T5);
        c.T4 = mem.ReadU32((c.FP + 0x8u));
        c.T5 = mem.ReadU32((c.FP + 0xCu));
        c.T6 = mem.ReadU32((c.FP + 0x10u));
        RecompOne.Runtime.Gte.WriteControl(2, c.T4);
        RecompOne.Runtime.Gte.WriteControl(3, c.T5);
        RecompOne.Runtime.Gte.WriteControl(4, c.T6);
        c.T4 = mem.ReadU16(c.S1);
        c.T5 = mem.ReadU16((c.S1 + 0x6u));
        c.T6 = mem.ReadU16((c.S1 + 0xCu));
        RecompOne.Runtime.Gte.Write(9, c.T4);
        RecompOne.Runtime.Gte.Write(10, c.T5);
        RecompOne.Runtime.Gte.Write(11, c.T6);
        RecompOne.Runtime.Gte.MvmvaOp(12, false, 0, 3, 3);
        c.T4 = RecompOne.Runtime.Gte.Read(9);
        c.T5 = RecompOne.Runtime.Gte.Read(10);
        c.T6 = RecompOne.Runtime.Gte.Read(11);
        mem.WriteU16(c.S1, (ushort)c.T4);
        mem.WriteU16((c.S1 + 0x6u), (ushort)c.T5);
        mem.WriteU16((c.S1 + 0xCu), (ushort)c.T6);
        c.T4 = mem.ReadU16(c.S3);
        c.T5 = mem.ReadU16((c.S3 + 0x6u));
        c.T6 = mem.ReadU16((c.S3 + 0xCu));
        RecompOne.Runtime.Gte.Write(9, c.T4);
        RecompOne.Runtime.Gte.Write(10, c.T5);
        RecompOne.Runtime.Gte.Write(11, c.T6);
        RecompOne.Runtime.Gte.MvmvaOp(12, false, 0, 3, 3);
        c.T4 = RecompOne.Runtime.Gte.Read(9);
        c.T5 = RecompOne.Runtime.Gte.Read(10);
        c.T6 = RecompOne.Runtime.Gte.Read(11);
        mem.WriteU16(c.S3, (ushort)c.T4);
        mem.WriteU16((c.S3 + 0x6u), (ushort)c.T5);
        mem.WriteU16((c.S3 + 0xCu), (ushort)c.T6);
        c.T4 = mem.ReadU16(c.S2);
        c.T5 = mem.ReadU16((c.S2 + 0x6u));
        c.T6 = mem.ReadU16((c.S2 + 0xCu));
        RecompOne.Runtime.Gte.Write(9, c.T4);
        RecompOne.Runtime.Gte.Write(10, c.T5);
        RecompOne.Runtime.Gte.Write(11, c.T6);
        RecompOne.Runtime.Gte.MvmvaOp(12, false, 0, 3, 3);
        c.T4 = RecompOne.Runtime.Gte.Read(9);
        c.T5 = RecompOne.Runtime.Gte.Read(10);
        c.T6 = RecompOne.Runtime.Gte.Read(11);
        mem.WriteU16(c.S2, (ushort)c.T4);
        mem.WriteU16((c.S2 + 0x6u), (ushort)c.T5);
        mem.WriteU16((c.S2 + 0xCu), (ushort)c.T6);
        L80022B08: ;
        c.T4 = mem.ReadU32(c.S1);
        c.T5 = mem.ReadU32((c.S1 + 0x4u));
        RecompOne.Runtime.Gte.WriteControl(0, c.T4);
        RecompOne.Runtime.Gte.WriteControl(1, c.T5);
        c.T4 = mem.ReadU32((c.S1 + 0x8u));
        c.T5 = mem.ReadU32((c.S1 + 0xCu));
        c.T6 = mem.ReadU32((c.S1 + 0x10u));
        RecompOne.Runtime.Gte.WriteControl(2, c.T4);
        RecompOne.Runtime.Gte.WriteControl(3, c.T5);
        RecompOne.Runtime.Gte.WriteControl(4, c.T6);
        c.T4 = mem.ReadU32((c.S1 + 0x14u));
        c.T5 = mem.ReadU32((c.S1 + 0x18u));
        RecompOne.Runtime.Gte.WriteControl(5, c.T4);
        c.T6 = mem.ReadU32((c.S1 + 0x1Cu));
        RecompOne.Runtime.Gte.WriteControl(6, c.T5);
        RecompOne.Runtime.Gte.WriteControl(7, c.T6);
        c.V1 = 0x80160000u;
        c.A1 = mem.ReadU16((c.SP + 0x9Cu));
        c.V1 = c.V1 - 0x2F70u;
        c.V0 = c.A1 << 2;
        c.V0 = c.V0 + c.V1;
        c.V1 = 0u | 0x834Cu;
        c.V0 = c.V0 + c.V1;
        c.V1 = mem.ReadU32(c.V0);
        c.V0 = mem.ReadU16((c.V1 + 0x6u));
        if (c.V0 != 0u) {
            c.A2 = c.S5 & 0xFFFFu;
            goto L80022BD4;
        }
        c.A2 = c.S5 & 0xFFFFu;
        c.V0 = mem.ReadU32((c.V1 + 0x8u));
        c.V1 = c.V1 + c.V0;
        c.V0 = c.A2 < 0x00000080u ? 1u : 0u;
        if (c.V0 == 0u) {
            mem.WriteU32((c.S0 + 0xCu), c.V1);
            goto L80022C40;
        }
        mem.WriteU32((c.S0 + 0xCu), c.V1);
        c.S1 = 0u;
        c.V0 = c.V1 + 0xCu;
        mem.WriteU32((c.S0 + 0x20u), c.V0);
        c.T3 = mem.ReadU16((c.SP + 0xA0u));
        c.A0 = mem.ReadU32((c.SP + 0xE4u));
        c.A3 = c.T3;
        c.RA = 0x80022BB0u;
        ShadowTower_st.func_80034604(c, mem);
        if (c.V0 != 0u) {
            c.V0 = c.S4 & 0x0040u;
            goto L80022C74;
        }
        c.V0 = c.S4 & 0x0040u;
        c.V1 = mem.ReadU32((c.S0 + 0xCu));
        c.V0 = c.V1 + 0xCu;
        mem.WriteU32((c.S0 + 0x20u), c.V0);
        c.V0 = mem.ReadU32((c.V1 + 0xCu));
        c.V0 = c.V0 + 0xCu;
        goto L80022C68;
        L80022BD4: ;
        c.V0 = mem.ReadU32((c.V1 + 0x8u));
        c.V1 = c.V1 + c.V0;
        c.V0 = c.A2 < 0x00000080u ? 1u : 0u;
        if (c.V0 == 0u) {
            mem.WriteU32((c.S0 + 0xCu), c.V1);
            goto L80022C40;
        }
        mem.WriteU32((c.S0 + 0xCu), c.V1);
        c.T3 = mem.ReadU16((c.SP + 0xA0u));
        c.A0 = mem.ReadU32((c.SP + 0xE4u));
        c.A3 = c.T3;
        c.RA = 0x80022BFCu;
        ShadowTower_st.func_80035AA8(c, mem);
        if (c.V0 != 0u) {
            goto L80022C2C;
        }
        c.V1 = mem.ReadU32((c.S0 + 0xCu));
        c.V0 = c.V1 + 0xCu;
        mem.WriteU32((c.S0 + 0x20u), c.V0);
        c.V0 = mem.ReadU32((c.V1 + 0xCu));
        c.V0 = c.V0 + 0xCu;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32((c.S0 + 0x48u), c.V1);
        goto L80023184;
        L80022C2C: ;
        c.A1 = mem.ReadU32((c.SP + 0xDCu));
        c.A0 = c.S6;
        c.RA = 0x80022C38u;
        ShadowTower_st.func_80022010(c, mem);
        c.V1 = 0x80160000u;
        goto L80023188;
        L80022C40: ;
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
        L80022C68: ;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32((c.S0 + 0x48u), c.V1);
        c.V0 = c.S4 & 0x0040u;
        L80022C74: ;
        if (c.V0 == 0u) {
            c.V0 = c.S4 & 0x0080u;
            goto L80022E28;
        }
        c.V0 = c.S4 & 0x0080u;
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
        if (c.A2 == c.V0) {
            c.A1 = c.A3 + 0x6u;
            goto L80022D74;
        }
        c.A1 = c.A3 + 0x6u;
        c.T2 = 0x00000FE1u;
        c.T1 = c.V0;
        L80022CB8: ;
        Poll(c, mem);
        RecompOne.Runtime.Gte.Write(0, (mem.ReadU32(c.T0)));
        RecompOne.Runtime.Gte.Write(1, (mem.ReadU32((c.T0 + 0x4u))));
        RecompOne.Runtime.Gte.Rtps(12, false);
        mem.WriteU32(c.A3, (RecompOne.Runtime.Gte.Read(14)));
        c.V0 = c.A3 + 0x4u;
        mem.WriteU32(c.V0, (RecompOne.Runtime.Gte.Read(19)));
        c.A0 = mem.ReadU32((c.S0 + 0x64u));
        c.V1 = mem.ReadU32((c.S0 + 0x68u));
        if (c.A0 == c.V1) {
            c.V1 = c.V1 - c.A0;
            goto L80022D30;
        }
        c.V1 = c.V1 - c.A0;
        c.V0 = (uint)(short)mem.ReadU16((c.A1 - 0x2u));
        c.V0 = c.V0 - c.A0;
        c.V0 = c.V0 << 12;
        if (c.V1 != 0u) { if ((int)c.V0 == int.MinValue && (int)c.V1 == -1) { c.LO = 0x80000000u; c.HI = 0u; } else { c.LO = (uint)((int)c.V0 / (int)c.V1); c.HI = (uint)((int)c.V0 % (int)c.V1); } }
        if (c.V1 != 0u) {
            goto L80022D0C;
        }
        Bios.Break(c, mem);
        L80022D0C: ;
        c.At = 0xFFFFFFFFu;
        if (c.V1 != c.At) {
            c.At = 0x80000000u;
            goto L80022D24;
        }
        c.At = 0x80000000u;
        if (c.V0 != c.At) {
            goto L80022D24;
        }
        Bios.Break(c, mem);
        L80022D24: ;
        c.V0 = c.LO;
        mem.WriteU16(c.A1, (ushort)c.V0);
        goto L80022D34;
        L80022D30: ;
        mem.WriteU16(c.A1, (ushort)0u);
        L80022D34: ;
        c.V0 = (uint)(short)mem.ReadU16(c.A1);
        c.V0 = (int)c.V0 < 4066 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L80022D4C;
        }
        mem.WriteU16(c.A1, (ushort)c.T2);
        L80022D4C: ;
        c.V0 = (uint)(short)mem.ReadU16(c.A1);
        if ((int)c.V0 >= 0) {
            goto L80022D60;
        }
        mem.WriteU16(c.A1, (ushort)0u);
        L80022D60: ;
        c.A1 = c.A1 + 0x8u;
        c.A3 = c.A3 + 0x8u;
        c.A2 = c.A2 - 0x1u;
        if (c.A2 != c.T1) {
            c.T0 = c.T0 + 0x8u;
            goto L80022CB8;
        }
        c.T0 = c.T0 + 0x8u;
        L80022D74: ;
        if (c.FP == 0u) {
            c.A0 = c.S1;
            goto L80022D8C;
        }
        c.A0 = c.S1;
        c.A1 = c.S4 & 0x00FFu;
        c.RA = 0x80022D84u;
        ShadowTower_st.func_8001BF10(c, mem);
        c.V1 = 0x80160000u;
        goto L80023188;
        L80022D8C: ;
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
        if (c.A0 == c.V0) {
            c.A3 = c.S0 + 0x50u;
            goto L80023178;
        }
        c.A3 = c.S0 + 0x50u;
        c.T0 = c.V0;
        c.V1 = c.A1 + 0x4u;
        L80022DC8: ;
        Poll(c, mem);
        RecompOne.Runtime.Gte.Write(0, (mem.ReadU32(c.A2)));
        RecompOne.Runtime.Gte.Write(1, (mem.ReadU32((c.A2 + 0x4u))));
        RecompOne.Runtime.Gte.MvmvaOp(12, false, 0, 0, 0);
        mem.WriteU32(c.A3, (RecompOne.Runtime.Gte.Read(25)));
        mem.WriteU32((c.A3 + 0x4u), (RecompOne.Runtime.Gte.Read(26)));
        mem.WriteU32((c.A3 + 0x8u), (RecompOne.Runtime.Gte.Read(27)));
        c.A2 = c.A2 + 0x8u;
        c.V0 = mem.ReadU16((c.S0 + 0x50u));
        c.A0 = c.A0 - 0x1u;
        mem.WriteU16(c.A1, (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S0 + 0x54u));
        c.A1 = c.A1 + 0x8u;
        mem.WriteU16((c.V1 - 0x2u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S0 + 0x58u));
        c.T3 = mem.ReadU16((c.SP + 0x100u));
        mem.WriteU16(c.V1, (ushort)c.T3);
        mem.WriteU16((c.V1 + 0x2u), (ushort)c.V0);
        if (c.A0 != c.T0) {
            c.V1 = c.V1 + 0x8u;
            goto L80022DC8;
        }
        c.V1 = c.V1 + 0x8u;
        c.A0 = c.S1;
        goto L8002317C;
        L80022E28: ;
        if (c.V0 == 0u) {
            goto L80022FDC;
        }
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
        if (c.A2 == c.V0) {
            c.A1 = c.A3 + 0x6u;
            goto L80022F28;
        }
        c.A1 = c.A3 + 0x6u;
        c.T2 = 0x00000FE1u;
        c.T1 = c.V0;
        L80022E6C: ;
        Poll(c, mem);
        RecompOne.Runtime.Gte.Write(0, (mem.ReadU32(c.T0)));
        RecompOne.Runtime.Gte.Write(1, (mem.ReadU32((c.T0 + 0x4u))));
        RecompOne.Runtime.Gte.Rtps(12, false);
        mem.WriteU32(c.A3, (RecompOne.Runtime.Gte.Read(14)));
        c.V0 = c.A3 + 0x4u;
        mem.WriteU32(c.V0, (RecompOne.Runtime.Gte.Read(19)));
        c.A0 = mem.ReadU32((c.S0 + 0x64u));
        c.V1 = mem.ReadU32((c.S0 + 0x68u));
        if (c.A0 == c.V1) {
            c.V1 = c.V1 - c.A0;
            goto L80022EE4;
        }
        c.V1 = c.V1 - c.A0;
        c.V0 = (uint)(short)mem.ReadU16((c.A1 - 0x2u));
        c.V0 = c.V0 - c.A0;
        c.V0 = c.V0 << 12;
        if (c.V1 != 0u) { if ((int)c.V0 == int.MinValue && (int)c.V1 == -1) { c.LO = 0x80000000u; c.HI = 0u; } else { c.LO = (uint)((int)c.V0 / (int)c.V1); c.HI = (uint)((int)c.V0 % (int)c.V1); } }
        if (c.V1 != 0u) {
            goto L80022EC0;
        }
        Bios.Break(c, mem);
        L80022EC0: ;
        c.At = 0xFFFFFFFFu;
        if (c.V1 != c.At) {
            c.At = 0x80000000u;
            goto L80022ED8;
        }
        c.At = 0x80000000u;
        if (c.V0 != c.At) {
            goto L80022ED8;
        }
        Bios.Break(c, mem);
        L80022ED8: ;
        c.V0 = c.LO;
        mem.WriteU16(c.A1, (ushort)c.V0);
        goto L80022EE8;
        L80022EE4: ;
        mem.WriteU16(c.A1, (ushort)0u);
        L80022EE8: ;
        c.V0 = (uint)(short)mem.ReadU16(c.A1);
        c.V0 = (int)c.V0 < 4066 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L80022F00;
        }
        mem.WriteU16(c.A1, (ushort)c.T2);
        L80022F00: ;
        c.V0 = (uint)(short)mem.ReadU16(c.A1);
        if ((int)c.V0 >= 0) {
            goto L80022F14;
        }
        mem.WriteU16(c.A1, (ushort)0u);
        L80022F14: ;
        c.A1 = c.A1 + 0x8u;
        c.A3 = c.A3 + 0x8u;
        c.A2 = c.A2 - 0x1u;
        if (c.A2 != c.T1) {
            c.T0 = c.T0 + 0x8u;
            goto L80022E6C;
        }
        c.T0 = c.T0 + 0x8u;
        L80022F28: ;
        if (c.FP == 0u) {
            c.A0 = c.S1;
            goto L80022F40;
        }
        c.A0 = c.S1;
        c.A1 = c.S4 & 0x00FFu;
        c.RA = 0x80022F38u;
        ShadowTower_st.func_800192D4(c, mem);
        c.V1 = 0x80160000u;
        goto L80023188;
        L80022F40: ;
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
        if (c.A0 == c.V0) {
            c.A3 = c.S0 + 0x50u;
            goto L80023178;
        }
        c.A3 = c.S0 + 0x50u;
        c.T0 = c.V0;
        c.V1 = c.A1 + 0x4u;
        L80022F7C: ;
        Poll(c, mem);
        RecompOne.Runtime.Gte.Write(0, (mem.ReadU32(c.A2)));
        RecompOne.Runtime.Gte.Write(1, (mem.ReadU32((c.A2 + 0x4u))));
        RecompOne.Runtime.Gte.MvmvaOp(12, false, 0, 0, 0);
        mem.WriteU32(c.A3, (RecompOne.Runtime.Gte.Read(25)));
        mem.WriteU32((c.A3 + 0x4u), (RecompOne.Runtime.Gte.Read(26)));
        mem.WriteU32((c.A3 + 0x8u), (RecompOne.Runtime.Gte.Read(27)));
        c.A2 = c.A2 + 0x8u;
        c.V0 = mem.ReadU16((c.S0 + 0x50u));
        c.A0 = c.A0 - 0x1u;
        mem.WriteU16(c.A1, (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S0 + 0x54u));
        c.A1 = c.A1 + 0x8u;
        mem.WriteU16((c.V1 - 0x2u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S0 + 0x58u));
        c.T3 = mem.ReadU16((c.SP + 0x100u));
        mem.WriteU16(c.V1, (ushort)c.T3);
        mem.WriteU16((c.V1 + 0x2u), (ushort)c.V0);
        if (c.A0 != c.T0) {
            c.V1 = c.V1 + 0x8u;
            goto L80022F7C;
        }
        c.V1 = c.V1 + 0x8u;
        c.A0 = c.S1;
        goto L8002317C;
        L80022FDC: ;
        if (c.FP == 0u) {
            goto L800230E4;
        }
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
        if (c.A2 == c.V0) {
            c.T2 = 0x00000FE1u;
            goto L80023178;
        }
        c.T2 = 0x00000FE1u;
        c.T1 = c.V0;
        c.A1 = c.A3 + 0x6u;
        L80023020: ;
        Poll(c, mem);
        RecompOne.Runtime.Gte.Write(0, (mem.ReadU32(c.T0)));
        RecompOne.Runtime.Gte.Write(1, (mem.ReadU32((c.T0 + 0x4u))));
        RecompOne.Runtime.Gte.Rtps(12, false);
        mem.WriteU32(c.A3, (RecompOne.Runtime.Gte.Read(14)));
        c.V0 = c.A3 + 0x4u;
        mem.WriteU32(c.V0, (RecompOne.Runtime.Gte.Read(19)));
        c.A0 = mem.ReadU32((c.S0 + 0x64u));
        c.V1 = mem.ReadU32((c.S0 + 0x68u));
        if (c.A0 == c.V1) {
            c.V1 = c.V1 - c.A0;
            goto L80023098;
        }
        c.V1 = c.V1 - c.A0;
        c.V0 = (uint)(short)mem.ReadU16((c.A1 - 0x2u));
        c.V0 = c.V0 - c.A0;
        c.V0 = c.V0 << 12;
        if (c.V1 != 0u) { if ((int)c.V0 == int.MinValue && (int)c.V1 == -1) { c.LO = 0x80000000u; c.HI = 0u; } else { c.LO = (uint)((int)c.V0 / (int)c.V1); c.HI = (uint)((int)c.V0 % (int)c.V1); } }
        if (c.V1 != 0u) {
            goto L80023074;
        }
        Bios.Break(c, mem);
        L80023074: ;
        c.At = 0xFFFFFFFFu;
        if (c.V1 != c.At) {
            c.At = 0x80000000u;
            goto L8002308C;
        }
        c.At = 0x80000000u;
        if (c.V0 != c.At) {
            goto L8002308C;
        }
        Bios.Break(c, mem);
        L8002308C: ;
        c.V0 = c.LO;
        mem.WriteU16(c.A1, (ushort)c.V0);
        goto L8002309C;
        L80023098: ;
        mem.WriteU16(c.A1, (ushort)0u);
        L8002309C: ;
        c.V0 = (uint)(short)mem.ReadU16(c.A1);
        c.V0 = (int)c.V0 < 4066 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L800230B4;
        }
        mem.WriteU16(c.A1, (ushort)c.T2);
        L800230B4: ;
        c.V0 = (uint)(short)mem.ReadU16(c.A1);
        if ((int)c.V0 >= 0) {
            goto L800230C8;
        }
        mem.WriteU16(c.A1, (ushort)0u);
        L800230C8: ;
        c.A1 = c.A1 + 0x8u;
        c.A3 = c.A3 + 0x8u;
        c.A2 = c.A2 - 0x1u;
        if (c.A2 != c.T1) {
            c.T0 = c.T0 + 0x8u;
            goto L80023020;
        }
        c.T0 = c.T0 + 0x8u;
        c.A0 = c.S1;
        goto L8002317C;
        L800230E4: ;
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
        if (c.A0 == c.V0) {
            c.A3 = c.S0 + 0x50u;
            goto L80023178;
        }
        c.A3 = c.S0 + 0x50u;
        c.T0 = c.V0;
        c.V1 = c.A1 + 0x4u;
        L80023120: ;
        Poll(c, mem);
        RecompOne.Runtime.Gte.Write(0, (mem.ReadU32(c.A2)));
        RecompOne.Runtime.Gte.Write(1, (mem.ReadU32((c.A2 + 0x4u))));
        RecompOne.Runtime.Gte.MvmvaOp(12, false, 0, 0, 0);
        mem.WriteU32(c.A3, (RecompOne.Runtime.Gte.Read(25)));
        mem.WriteU32((c.A3 + 0x4u), (RecompOne.Runtime.Gte.Read(26)));
        mem.WriteU32((c.A3 + 0x8u), (RecompOne.Runtime.Gte.Read(27)));
        c.A2 = c.A2 + 0x8u;
        c.V0 = mem.ReadU16((c.S0 + 0x50u));
        c.A0 = c.A0 - 0x1u;
        mem.WriteU16(c.A1, (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S0 + 0x54u));
        c.A1 = c.A1 + 0x8u;
        mem.WriteU16((c.V1 - 0x2u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S0 + 0x58u));
        c.T3 = mem.ReadU16((c.SP + 0x100u));
        mem.WriteU16(c.V1, (ushort)c.T3);
        mem.WriteU16((c.V1 + 0x2u), (ushort)c.V0);
        if (c.A0 != c.T0) {
            c.V1 = c.V1 + 0x8u;
            goto L80023120;
        }
        c.V1 = c.V1 + 0x8u;
        L80023178: ;
        c.A0 = c.S1;
        L8002317C: ;
        c.A1 = c.S4 & 0x00FFu;
        c.RA = 0x80023184u;
        ShadowTower_st.func_80019DC8(c, mem);
        L80023184: ;
        c.V1 = 0x80160000u;
        L80023188: ;
        c.V1 = c.V1 - 0x2F70u;
        c.V0 = 0u | 0x8000u;
        c.V1 = c.V1 + c.V0;
        c.A0 = mem.ReadU32((c.V1 + 0x304u));
        c.V0 = mem.ReadU32((c.S0 + 0x10u));
        mem.WriteU32((c.A0 + 0x8u), c.V0);
        c.A0 = mem.ReadU32((c.V1 + 0x304u));
        c.V0 = mem.ReadU32((c.S0 + 0x14u));
        mem.WriteU32((c.A0 + 0x4u), c.V0);
        c.V0 = mem.ReadU32((c.S0 + 0x88u));
        c.A0 = mem.ReadU32((c.S0 + 0x8Cu));
        c.A1 = mem.ReadU32((c.S0 + 0x90u));
        c.A2 = mem.ReadU32((c.S0 + 0x94u));
        mem.WriteU32((c.V1 + 0x31Cu), c.V0);
        mem.WriteU32((c.V1 + 0x320u), c.A0);
        mem.WriteU32((c.V1 + 0x324u), c.A1);
        mem.WriteU32((c.V1 + 0x328u), c.A2);
        c.RA = mem.ReadU32((c.SP + 0xCCu));
        c.FP = mem.ReadU32((c.SP + 0xC8u));
        c.S7 = mem.ReadU32((c.SP + 0xC4u));
        c.S6 = mem.ReadU32((c.SP + 0xC0u));
        c.S5 = mem.ReadU32((c.SP + 0xBCu));
        c.S4 = mem.ReadU32((c.SP + 0xB8u));
        c.S3 = mem.ReadU32((c.SP + 0xB4u));
        c.S2 = mem.ReadU32((c.SP + 0xB0u));
        c.S1 = mem.ReadU32((c.SP + 0xACu));
        c.S0 = mem.ReadU32((c.SP + 0xA8u));
        c.SP = c.SP + 0xD0u;
        return;
    }
}
