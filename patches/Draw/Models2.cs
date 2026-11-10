using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_800251C8</c>: a second model list, the same shape as func_800241D0's; drew nothing in map 0. <c>ST_MODELS2</c>.
/// </summary>
public static class Models2
{
    public static readonly Literal R = new(typeof(Models2), "models 2", "MODELS2", 0x800251C8, Run,
        "a second model list, the same shape as func_800241D0's; drew nothing in map 0.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0xA8u;
        c.V1 = 0x1F800000u;
        c.V1 = c.V1 | 0x00F8u;
        c.V0 = 0x1F800000u;
        c.V0 = c.V0 | 0x0244u;
        c.A0 = 0x1F800000u;
        c.A0 = c.A0 | 0x00F8u;
        c.A1 = 0u;
        c.A2 = 0x00000020u;
        mem.WriteU32((c.SP + 0x88u), c.S2);
        c.S2 = 0x1F800000u;
        mem.WriteU32((c.SP + 0xA4u), c.RA);
        mem.WriteU32((c.SP + 0xA0u), c.FP);
        mem.WriteU32((c.SP + 0x9Cu), c.S7);
        mem.WriteU32((c.SP + 0x98u), c.S6);
        mem.WriteU32((c.SP + 0x94u), c.S5);
        mem.WriteU32((c.SP + 0x90u), c.S4);
        mem.WriteU32((c.SP + 0x8Cu), c.S3);
        mem.WriteU32((c.SP + 0x84u), c.S1);
        mem.WriteU32((c.SP + 0x80u), c.S0);
        mem.WriteU32((c.S2 + 0xF0u), c.V1);
        mem.WriteU32((c.S2 + 0xF4u), c.V0);
        c.RA = 0x80025224u;
        ShadowTower_st.func_80014D5C(c, mem);
        c.V0 = 0x801C0000u;
        c.V1 = c.V0 + 0x76F8u;
        c.FP = mem.ReadU8((c.V1 + 0x6u));
        c.V0 = 0xFFFFFFFFu;
        c.FP = c.FP - 0x1u;
        if (c.FP == c.V0) {
            mem.WriteU16((c.S2 + 0x98u), (ushort)0u);
            goto L8002558C;
        }
        mem.WriteU16((c.S2 + 0x98u), (ushort)0u);
        c.T0 = c.SP + 0x48u;
        mem.WriteU32((c.SP + 0x78u), c.T0);
        c.T0 = 0x801C0000u;
        L8002524C: ;
        Poll(c, mem);
        c.T0 = c.T0 + 0x76F8u;
        c.V0 = c.FP + c.T0;
        c.T0 = 0x801C0000u;
        c.V1 = mem.ReadU8((c.V0 + 0x7u));
        c.T0 = c.T0 + 0x76F8u;
        c.V0 = c.V1 << 1;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 4;
        c.V1 = c.T0 + 0x3710u;
        c.S1 = c.V0 + c.V1;
        c.T0 = 0x80170000u;
        c.T0 = c.T0 + 0x5090u;
        c.A0 = mem.ReadU32((c.T0 + 0x6E00u));
        c.T0 = 0x801C0000u;
        c.V1 = mem.ReadU16(c.S1);
        c.T0 = c.T0 + 0x7810u;
        c.V0 = c.V1 << 1;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 2;
        c.V0 = c.V0 - c.V1;
        c.V0 = c.V0 << 2;
        c.V1 = mem.ReadU32((c.S1 + 0x28u));
        c.V1 = (int)c.V1 < (int)c.A0 ? 1u : 0u;
        if (c.V1 != 0u) {
            c.S3 = c.V0 + c.T0;
            goto L800252C4;
        }
        c.S3 = c.V0 + c.T0;
        c.V0 = mem.ReadU8((c.S1 + 0x24u));
        c.V0 = c.V0 | 0x0080u;
        mem.WriteU8((c.S1 + 0x24u), (byte)c.V0);
        L800252C4: ;
        c.V1 = mem.ReadU8((c.S1 + 0x22u));
        c.V0 = c.V1 < 0x00000006u ? 1u : 0u;
        if (c.V0 == 0u) {
            c.V0 = 0x80010000u;
            goto L800252F4;
        }
        c.V0 = 0x80010000u;
        c.V0 = c.V0 + 0x1478u;
        c.V1 = c.V1 << 2;
        c.V1 = c.V1 + c.V0;
        c.V0 = mem.ReadU32(c.V1);
        switch (c.V0)
        {
            case 0x8002557Cu: goto L8002557C;
            case 0x800252F4u: goto L800252F4;
            default: Dispatcher.Call(c, mem, c.V0); return;
        }
        L800252F4: ;
        c.V0 = 0x80080000u;
        c.A2 = c.V0 + 0x7F0Cu;
        c.V1 = mem.ReadWordLeft(c.V1, (c.A2 + 0x3u));
        c.V1 = mem.ReadWordRight(c.V1, c.A2);
        c.A0 = mem.ReadWordLeft(c.A0, (c.A2 + 0x7u));
        c.A0 = mem.ReadWordRight(c.A0, (c.A2 + 0x4u));
        mem.WriteWordLeft((c.SP + 0x4Bu), c.V1);
        mem.WriteWordRight((c.SP + 0x48u), c.V1);
        mem.WriteWordLeft((c.SP + 0x4Fu), c.A0);
        mem.WriteWordRight((c.SP + 0x4Cu), c.A0);
        c.V0 = mem.ReadU8((c.S3 + 0x4u));
        c.V1 = 0x00000001u;
        c.V0 = c.S2 + c.V0;
        mem.WriteU8((c.V0 + 0xF8u), (byte)c.V1);
        c.A0 = mem.ReadU8((c.S3 + 0x4u));
        c.A1 = c.SP + 0x50u;
        c.A0 = c.A0 + 0x24Cu;
        c.RA = 0x8002533Cu;
        ShadowTower_st.func_80033DB8(c, mem);
        c.V0 = mem.ReadU32((c.SP + 0x50u));
        if (c.V0 == 0u) {
            goto L8002557C;
        }
        c.V0 = mem.ReadU16((c.S1 + 0x18u));
        mem.WriteU16((c.S2 + 0xD8u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S1 + 0x1Au));
        mem.WriteU16((c.S2 + 0xDAu), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S1 + 0x1Cu));
        mem.WriteU16((c.S2 + 0xDCu), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S3 + 0x1Eu));
        c.V0 = c.V0 & 0x0040u;
        if (c.V0 == 0u) {
            c.A0 = 0u;
            goto L800254F0;
        }
        c.A0 = 0u;
        c.T0 = 0x80160000u;
        c.T0 = c.T0 + 0x5090u;
        c.S0 = 0x00000004u;
        c.S6 = 0x00001000u;
        c.S7 = c.S1 + 0x8u;
        c.A2 = c.S7;
        c.S5 = c.S1 + 0x4u;
        c.V0 = (uint)(sbyte)mem.ReadU8((c.S1 + 0x2Du));
        c.V1 = mem.ReadU32((c.T0 + 0xE4u));
        c.V0 = c.V0 << 2;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32(c.S2, c.V1);
        c.V0 = (uint)(sbyte)mem.ReadU8((c.S1 + 0x2Du));
        c.S4 = c.S2 + 0xD8u;
        c.V0 = c.S0 - c.V0;
        mem.WriteU32((c.S2 + 0x80u), c.V0);
        c.V0 = (uint)(sbyte)mem.ReadU8((c.S1 + 0x2Du));
        c.A3 = c.S4;
        c.V0 = c.S6 - c.V0;
        mem.WriteU32((c.S2 + 0x4u), c.V0);
        c.V0 = 0x80180000u;
        c.A1 = mem.ReadU8((c.S3 + 0x4u));
        c.T0 = mem.ReadU32((c.SP + 0x78u));
        c.V0 = c.V0 - 0x4288u;
        mem.WriteU32((c.SP + 0x18u), c.V0);
        c.V0 = 0x00000080u;
        mem.WriteU32((c.SP + 0x14u), c.S5);
        mem.WriteU32((c.SP + 0x1Cu), c.V0);
        mem.WriteU32((c.SP + 0x20u), 0u);
        mem.WriteU32((c.SP + 0x24u), 0u);
        mem.WriteU32((c.SP + 0x28u), c.S6);
        mem.WriteU32((c.SP + 0x10u), c.T0);
        c.V0 = mem.ReadU8((c.S1 + 0x24u));
        c.A1 = c.A1 + 0x24Cu;
        mem.WriteU32((c.SP + 0x30u), 0u);
        mem.WriteU32((c.SP + 0x2Cu), c.V0);
        c.RA = 0x80025418u;
        ShadowTower_st.func_80022678(c, mem);
        c.T0 = 0x80160000u;
        c.T0 = c.T0 + 0x5090u;
        mem.WriteU16((c.S2 + 0xDCu), (ushort)0u);
        mem.WriteU16((c.S2 + 0xDAu), (ushort)0u);
        mem.WriteU16((c.S2 + 0xD8u), (ushort)0u);
        c.V0 = (uint)(sbyte)mem.ReadU8((c.S1 + 0x2Du));
        c.V1 = mem.ReadU32((c.T0 + 0xE4u));
        c.T0 = 0x80170000u;
        c.V0 = c.V0 << 2;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32(c.S2, c.V1);
        c.V0 = (uint)(sbyte)mem.ReadU8((c.S1 + 0x2Du));
        c.T0 = c.T0 + 0x5090u;
        c.S0 = c.S0 - c.V0;
        mem.WriteU32((c.S2 + 0x80u), c.S0);
        c.V0 = (uint)(sbyte)mem.ReadU8((c.S1 + 0x2Du));
        c.V1 = mem.ReadU32((c.T0 + 0x6DA8u));
        c.A0 = mem.ReadU32((c.T0 + 0x6DACu));
        c.A1 = mem.ReadU32((c.T0 + 0x6DB0u));
        c.A2 = mem.ReadU32((c.T0 + 0x6DB4u));
        mem.WriteU32((c.SP + 0x58u), c.V1);
        mem.WriteU32((c.SP + 0x5Cu), c.A0);
        mem.WriteU32((c.SP + 0x60u), c.A1);
        mem.WriteU32((c.SP + 0x64u), c.A2);
        c.V1 = mem.ReadU32((c.T0 + 0x6DB8u));
        c.A0 = mem.ReadU32((c.T0 + 0x6DBCu));
        c.A1 = mem.ReadU32((c.T0 + 0x6DC0u));
        c.A2 = mem.ReadU32((c.T0 + 0x6DC4u));
        mem.WriteU32((c.SP + 0x68u), c.V1);
        mem.WriteU32((c.SP + 0x6Cu), c.A0);
        mem.WriteU32((c.SP + 0x70u), c.A1);
        mem.WriteU32((c.SP + 0x74u), c.A2);
        c.V0 = c.S6 - c.V0;
        mem.WriteU32((c.S2 + 0x4u), c.V0);
        c.V1 = mem.ReadU8((c.S1 + 0x22u));
        c.V0 = 0x00000002u;
        if (c.V1 == c.V0) {
            c.S0 = c.SP + 0x58u;
            goto L800254BC;
        }
        c.S0 = c.SP + 0x58u;
        c.A0 = (uint)(short)mem.ReadU16((c.S1 + 0x18u));
        c.A1 = c.S0;
        c.RA = 0x800254BCu;
        ShadowTower_st.RotMatrixX_st(c, mem);
        L800254BC: ;
        c.A0 = 0u;
        c.A2 = c.S7;
        c.V0 = 0x00000081u;
        c.A1 = mem.ReadU8((c.S3 + 0x4u));
        c.T0 = mem.ReadU32((c.SP + 0x78u));
        c.A3 = c.S4;
        mem.WriteU32((c.SP + 0x14u), c.S5);
        mem.WriteU32((c.SP + 0x18u), c.S0);
        mem.WriteU32((c.SP + 0x1Cu), c.V0);
        mem.WriteU32((c.SP + 0x20u), 0u);
        mem.WriteU32((c.SP + 0x24u), 0u);
        mem.WriteU32((c.SP + 0x28u), c.S6);
        goto L80025564;
        L800254F0: ;
        c.T0 = 0x80160000u;
        c.T0 = c.T0 + 0x5090u;
        c.A2 = c.S1 + 0x8u;
        c.A3 = c.S2 + 0xD8u;
        c.V0 = (uint)(sbyte)mem.ReadU8((c.S1 + 0x2Du));
        c.V1 = mem.ReadU32((c.T0 + 0xE4u));
        c.V0 = c.V0 << 2;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32(c.S2, c.V1);
        c.V1 = (uint)(sbyte)mem.ReadU8((c.S1 + 0x2Du));
        c.V0 = 0x00000004u;
        c.V0 = c.V0 - c.V1;
        mem.WriteU32((c.S2 + 0x80u), c.V0);
        c.V0 = (uint)(sbyte)mem.ReadU8((c.S1 + 0x2Du));
        c.V1 = 0x00001000u;
        c.V0 = c.V1 - c.V0;
        mem.WriteU32((c.S2 + 0x4u), c.V0);
        c.A1 = mem.ReadU8((c.S3 + 0x4u));
        c.V0 = c.S1 + 0x4u;
        mem.WriteU32((c.SP + 0x14u), c.V0);
        c.V0 = 0x80180000u;
        c.T0 = mem.ReadU32((c.SP + 0x78u));
        c.V0 = c.V0 - 0x4288u;
        mem.WriteU32((c.SP + 0x18u), c.V0);
        c.V0 = 0x00000080u;
        mem.WriteU32((c.SP + 0x1Cu), c.V0);
        mem.WriteU32((c.SP + 0x20u), 0u);
        mem.WriteU32((c.SP + 0x24u), 0u);
        mem.WriteU32((c.SP + 0x28u), c.V1);
        L80025564: ;
        mem.WriteU32((c.SP + 0x10u), c.T0);
        c.V0 = mem.ReadU8((c.S1 + 0x24u));
        c.A1 = c.A1 + 0x24Cu;
        mem.WriteU32((c.SP + 0x30u), 0u);
        mem.WriteU32((c.SP + 0x2Cu), c.V0);
        c.RA = 0x8002557Cu;
        ShadowTower_st.func_80022678(c, mem);
        L8002557C: ;
        c.FP = c.FP - 0x1u;
        c.V0 = 0xFFFFFFFFu;
        if (c.FP != c.V0) {
            c.T0 = 0x801C0000u;
            goto L8002524C;
        }
        c.T0 = 0x801C0000u;
        L8002558C: ;
        c.V0 = 0x801B0000u;
        c.V0 = mem.ReadU8((c.V0 + 0x593Fu));
        if (c.V0 != 0u) {
            c.V0 = c.S2 + 0x178u;
            goto L800255B8;
        }
        c.V0 = c.S2 + 0x178u;
        c.A0 = 0x00000005u;
        c.A1 = 0u;
        c.A2 = 0x0000024Cu;
        c.A3 = 0x00000080u;
        c.RA = 0x800255B4u;
        ShadowTower_st.func_80023ED8(c, mem);
        c.V0 = c.S2 + 0x178u;
        L800255B8: ;
        mem.WriteU32((c.S2 + 0xF0u), c.V0);
        c.RA = mem.ReadU32((c.SP + 0xA4u));
        c.FP = mem.ReadU32((c.SP + 0xA0u));
        c.S7 = mem.ReadU32((c.SP + 0x9Cu));
        c.S6 = mem.ReadU32((c.SP + 0x98u));
        c.S5 = mem.ReadU32((c.SP + 0x94u));
        c.S4 = mem.ReadU32((c.SP + 0x90u));
        c.S3 = mem.ReadU32((c.SP + 0x8Cu));
        c.S2 = mem.ReadU32((c.SP + 0x88u));
        c.S1 = mem.ReadU32((c.SP + 0x84u));
        c.S0 = mem.ReadU32((c.SP + 0x80u));
        c.SP = c.SP + 0xA8u;
        return;
    }
}
