using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_800241D0</c>: the model list: the entity records (0xD4 bytes from 0x801BA138) named by the u16 list at 0x801C023C, counted at 0x801C0238, each through func_80022678. <c>ST_MODELS</c>.
/// </summary>
public static class Models
{
    public static readonly Literal R = new(typeof(Models), "models", "MODELS", 0x800241D0, Run,
        "the model list: the entity records (0xD4 bytes from 0x801BA138) named by the u16 list at 0x801C023C, counted at 0x801C0238, each through func_80022678.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x90u;
        c.V1 = 0x1F800000u;
        c.V1 = c.V1 | 0x00F8u;
        c.V0 = 0x1F800000u;
        c.V0 = c.V0 | 0x0244u;
        c.A0 = 0x1F800000u;
        c.A0 = c.A0 | 0x00F8u;
        c.A1 = 0u;
        c.A2 = 0x00000020u;
        mem.WriteU32((c.SP + 0x74u), c.S3);
        c.S3 = 0x1F800000u;
        mem.WriteU32((c.SP + 0x8Cu), c.RA);
        mem.WriteU32((c.SP + 0x88u), c.FP);
        mem.WriteU32((c.SP + 0x84u), c.S7);
        mem.WriteU32((c.SP + 0x80u), c.S6);
        mem.WriteU32((c.SP + 0x7Cu), c.S5);
        mem.WriteU32((c.SP + 0x78u), c.S4);
        mem.WriteU32((c.SP + 0x70u), c.S2);
        mem.WriteU32((c.SP + 0x6Cu), c.S1);
        mem.WriteU32((c.SP + 0x68u), c.S0);
        mem.WriteU32((c.S3 + 0xF0u), c.V1);
        mem.WriteU32((c.S3 + 0xF4u), c.V0);
        c.RA = 0x8002422Cu;
        ShadowTower_st.func_80014D5C(c, mem);
        c.A0 = c.S3;
        c.A0 = c.A0 | 0x0244u;
        c.A1 = 0u;
        c.A2 = 0x00000004u;
        c.RA = 0x80024240u;
        ShadowTower_st.func_80014D5C(c, mem);
        c.V0 = 0x801C0000u;
        c.V1 = c.V0 - 0x5EC8u;
        c.S4 = mem.ReadU8((c.V1 + 0x6100u));
        c.V0 = 0xFFFFFFFFu;
        c.S4 = c.S4 - 0x1u;
        if (c.S4 == c.V0) {
            mem.WriteU16((c.S3 + 0x98u), (ushort)0u);
            goto L8002468C;
        }
        mem.WriteU16((c.S3 + 0x98u), (ushort)0u);
        c.S6 = c.V1;
        c.FP = c.S6 + 0x3500u;
        c.V0 = 0x80170000u;
        c.S7 = c.V0 + 0x5090u;
        c.V0 = 0x80160000u;
        c.S5 = c.V0 + 0x5090u;
        c.V0 = c.S4 << 1;
        L80024278: ;
        Poll(c, mem);
        c.V0 = c.V0 + c.S6;
        c.V1 = mem.ReadU16((c.V0 + 0x6104u));
        c.A1 = c.SP + 0x48u;
        c.V0 = c.V1 << 1;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 2;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 2;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 2;
        c.S1 = c.V0 + c.S6;
        c.A0 = mem.ReadU8((c.S1 + 0xBu));
        c.V1 = mem.ReadU8((c.S1 + 0x2u));
        c.A0 = c.A0 + 0x80u;
        c.V0 = c.V1 << 1;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 6;
        c.S0 = c.V0 + c.FP;
        c.RA = 0x800242C4u;
        ShadowTower_st.func_80033DB8(c, mem);
        c.V0 = mem.ReadU8((c.S1 + 0x1Eu));
        mem.WriteU8((c.S1 + 0x4u), (byte)0u);
        c.A0 = c.V0 & 0x003Fu;
        mem.WriteU8((c.S1 + 0x1Eu), (byte)c.A0);
        c.V0 = mem.ReadU32((c.S7 + 0x6E04u));
        c.LO = (uint)((long)(int)c.V0 * (int)c.V0); c.HI = (uint)(((long)(int)c.V0 * (int)c.V0) >> 32);
        c.V1 = mem.ReadU32((c.S1 + 0x3Cu));
        c.T1 = c.LO;
        c.V0 = c.T1 < c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.V0 = c.A0 | 0x0040u;
            goto L80024310;
        }
        c.V0 = c.A0 | 0x0040u;
        c.V0 = mem.ReadU32((c.S7 + 0x6E00u));
        c.LO = (uint)((long)(int)c.V0 * (int)c.V0); c.HI = (uint)(((long)(int)c.V0 * (int)c.V0) >> 32);
        c.T1 = c.LO;
        c.V0 = c.V1 < c.T1 ? 1u : 0u;
        if (c.V0 != 0u) {
            c.V0 = c.A0 | 0x0080u;
            goto L80024314;
        }
        c.V0 = c.A0 | 0x0080u;
        L80024310: ;
        mem.WriteU8((c.S1 + 0x1Eu), (byte)c.V0);
        L80024314: ;
        c.V0 = mem.ReadU32((c.S1 + 0x1Cu));
        c.V1 = 0x00300000u;
        c.V0 = c.V0 & c.V1;
        if (c.V0 == 0u) {
            goto L80024334;
        }
        c.A3 = (uint)(short)mem.ReadU16((c.S1 + 0x34u));
        goto L80024338;
        L80024334: ;
        c.A3 = (uint)(short)mem.ReadU16((c.S1 + 0x36u));
        L80024338: ;
        c.V1 = mem.ReadU16((c.S0 + 0x10u));
        c.V0 = 0u | 0xFFFFu;
        if (c.V1 == c.V0) {
            c.V0 = 0x801D0000u;
            goto L80024374;
        }
        c.V0 = 0x801D0000u;
        c.A1 = c.V1;
        c.V0 = c.V0 - 0x45F8u;
        c.A0 = c.A1 >> 5;
        c.A0 = c.A0 << 2;
        c.A0 = c.A0 + c.V0;
        c.A1 = c.A1 & 0x001Fu;
        c.V0 = 0x00000001u;
        c.V1 = mem.ReadU32((c.A0 + 0x14A8u));
        c.V0 = c.V0 << (int)(c.A1 & 31u);
        c.V1 = c.V1 | c.V0;
        mem.WriteU32((c.A0 + 0x14A8u), c.V1);
        L80024374: ;
        c.V0 = mem.ReadU32((c.SP + 0x48u));
        if (c.V0 == 0u) {
            c.V0 = 0x00000002u;
            goto L8002461C;
        }
        c.V0 = 0x00000002u;
        c.V0 = mem.ReadU32((c.S1 + 0x70u));
        c.V0 = c.V0 & 0x0020u;
        if (c.V0 == 0u) {
            c.T0 = c.S1 + 0x74u;
            goto L80024448;
        }
        c.T0 = c.S1 + 0x74u;
        c.A0 = 0u;
        c.A2 = c.T0;
        mem.WriteU16((c.S3 + 0xDCu), (ushort)0u);
        mem.WriteU16((c.S3 + 0xDAu), (ushort)0u);
        mem.WriteU16((c.S3 + 0xD8u), (ushort)0u);
        c.V0 = (uint)(sbyte)mem.ReadU8((c.S1 + 0x21u));
        c.V1 = mem.ReadU32((c.S5 + 0xE4u));
        c.V0 = c.V0 << 2;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32(c.S3, c.V1);
        c.V1 = (uint)(sbyte)mem.ReadU8((c.S1 + 0x21u));
        c.V0 = 0x00000004u;
        c.V0 = c.V0 - c.V1;
        mem.WriteU32((c.S3 + 0x80u), c.V0);
        c.V0 = (uint)(sbyte)mem.ReadU8((c.S1 + 0x21u));
        c.V1 = 0x00001000u;
        c.V0 = c.V1 - c.V0;
        mem.WriteU32((c.S3 + 0x4u), c.V0);
        c.A1 = mem.ReadU8((c.S1 + 0xBu));
        c.V0 = c.S1 + 0x94u;
        mem.WriteU32((c.SP + 0x10u), c.V0);
        c.V0 = c.S1 + 0xA4u;
        mem.WriteU32((c.SP + 0x14u), c.V0);
        c.V0 = 0x80080000u;
        c.V0 = c.V0 + 0x40Cu;
        c.V1 = c.V1 - c.A3;
        mem.WriteU32((c.SP + 0x18u), c.V0);
        c.V0 = mem.ReadU8((c.S1 + 0x8u));
        c.V1 = c.V1 << 16;
        mem.WriteU32((c.SP + 0x1Cu), c.V0);
        c.V0 = mem.ReadU16((c.S1 + 0x2Au));
        c.V1 = (uint)((int)c.V1 >> 16);
        mem.WriteU32((c.SP + 0x24u), c.V1);
        mem.WriteU32((c.SP + 0x20u), c.V0);
        c.V0 = (uint)(short)mem.ReadU16((c.S1 + 0x24u));
        c.A3 = c.S3 + 0xD8u;
        mem.WriteU32((c.SP + 0x28u), c.V0);
        c.V0 = mem.ReadU8((c.S1 + 0x1Eu));
        c.A1 = c.A1 + 0x80u;
        mem.WriteU32((c.SP + 0x30u), 0u);
        mem.WriteU32((c.SP + 0x2Cu), c.V0);
        c.RA = 0x80024440u;
        ShadowTower_st.func_80022678(c, mem);
        goto L80024620;
        L80024448: ;
        c.V0 = 0x80080000u;
        c.A2 = c.V0 + 0x7F04u;
        c.V1 = mem.ReadWordLeft(c.V1, (c.A2 + 0x3u));
        c.V1 = mem.ReadWordRight(c.V1, c.A2);
        c.A0 = mem.ReadWordLeft(c.A0, (c.A2 + 0x7u));
        c.A0 = mem.ReadWordRight(c.A0, (c.A2 + 0x4u));
        mem.WriteWordLeft((c.SP + 0x63u), c.V1);
        mem.WriteWordRight((c.SP + 0x60u), c.V1);
        mem.WriteWordLeft((c.SP + 0x67u), c.A0);
        mem.WriteWordRight((c.SP + 0x64u), c.A0);
        c.V1 = mem.ReadU16((c.S1 + 0x8Cu));
        c.V0 = 0x00000001u;
        mem.WriteU8((c.S1 + 0x4u), (byte)c.V0);
        mem.WriteU16((c.S3 + 0xD8u), (ushort)c.V1);
        c.V0 = mem.ReadU16((c.S1 + 0x8Eu));
        mem.WriteU16((c.S3 + 0xDAu), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S1 + 0x90u));
        mem.WriteU16((c.S3 + 0xDCu), (ushort)c.V0);
        c.V1 = mem.ReadU8((c.S1 + 0x3u));
        c.V0 = 0x00000003u;
        if (c.V1 == c.V0) {
            c.S0 = c.SP + 0x60u;
            goto L80024544;
        }
        c.S0 = c.SP + 0x60u;
        c.A0 = 0u;
        c.A2 = c.T0;
        c.V0 = (uint)(sbyte)mem.ReadU8((c.S1 + 0x21u));
        c.V1 = mem.ReadU32((c.S5 + 0xE4u));
        c.V0 = c.V0 << 2;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32(c.S3, c.V1);
        c.V1 = (uint)(sbyte)mem.ReadU8((c.S1 + 0x21u));
        c.V0 = 0x00000004u;
        c.V0 = c.V0 - c.V1;
        mem.WriteU32((c.S3 + 0x80u), c.V0);
        c.V0 = (uint)(sbyte)mem.ReadU8((c.S1 + 0x21u));
        c.V1 = 0x00001000u;
        c.V0 = c.V1 - c.V0;
        mem.WriteU32((c.S3 + 0x4u), c.V0);
        c.A1 = mem.ReadU8((c.S1 + 0xBu));
        c.V0 = c.S1 + 0x94u;
        mem.WriteU32((c.SP + 0x10u), c.V0);
        c.V0 = c.S1 + 0xA4u;
        mem.WriteU32((c.SP + 0x14u), c.V0);
        c.V0 = 0x80180000u;
        c.V0 = c.V0 - 0x4288u;
        c.V1 = c.V1 - c.A3;
        mem.WriteU32((c.SP + 0x18u), c.V0);
        c.V0 = mem.ReadU8((c.S1 + 0x8u));
        c.V1 = c.V1 << 16;
        mem.WriteU32((c.SP + 0x1Cu), c.V0);
        c.V0 = mem.ReadU16((c.S1 + 0x2Au));
        c.V1 = (uint)((int)c.V1 >> 16);
        mem.WriteU32((c.SP + 0x24u), c.V1);
        mem.WriteU32((c.SP + 0x20u), c.V0);
        c.V0 = (uint)(short)mem.ReadU16((c.S1 + 0x24u));
        c.A3 = c.S3 + 0xD8u;
        mem.WriteU32((c.SP + 0x28u), c.V0);
        c.V0 = mem.ReadU8((c.S1 + 0x1Eu));
        c.A1 = c.A1 + 0x80u;
        mem.WriteU32((c.SP + 0x30u), 0u);
        mem.WriteU32((c.SP + 0x2Cu), c.V0);
        c.RA = 0x80024544u;
        ShadowTower_st.func_80022678(c, mem);
        L80024544: ;
        c.V0 = mem.ReadU16((c.S1 + 0x32u));
        if (c.V0 == 0u) {
            c.A2 = c.SP + 0x50u;
            goto L80024620;
        }
        c.A2 = c.SP + 0x50u;
        c.V0 = mem.ReadU32((c.S1 + 0x74u));
        c.V1 = mem.ReadU32((c.S1 + 0x78u));
        c.A0 = mem.ReadU32((c.S1 + 0x7Cu));
        c.A1 = mem.ReadU32((c.S1 + 0x80u));
        mem.WriteU32((c.SP + 0x50u), c.V0);
        mem.WriteU32((c.SP + 0x54u), c.V1);
        mem.WriteU32((c.SP + 0x58u), c.A0);
        mem.WriteU32((c.SP + 0x5Cu), c.A1);
        c.A0 = 0u;
        c.A1 = 0x0000002Cu;
        c.V0 = mem.ReadU16((c.S1 + 0x32u));
        c.V0 = c.V0 << 1;
        c.V0 = c.V0 + 0x400u;
        mem.WriteU16((c.SP + 0x60u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S1 + 0x32u));
        c.A3 = c.S3 + 0xD8u;
        c.V0 = c.V0 << 1;
        c.V0 = c.V0 + 0x400u;
        mem.WriteU16((c.SP + 0x64u), (ushort)c.V0);
        c.V1 = (uint)(sbyte)mem.ReadU8((c.S1 + 0x21u));
        c.V0 = mem.ReadU32((c.S5 + 0xE4u));
        c.V1 = c.V1 << 2;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 - 0x10u;
        mem.WriteU32(c.S3, c.V0);
        c.V1 = (uint)(sbyte)mem.ReadU8((c.S1 + 0x21u));
        c.V0 = 0x00000008u;
        c.V0 = c.V0 - c.V1;
        mem.WriteU32((c.S3 + 0x80u), c.V0);
        c.V0 = 0x80180000u;
        c.V1 = (uint)(sbyte)mem.ReadU8((c.S1 + 0x21u));
        c.V0 = c.V0 - 0x4288u;
        mem.WriteU32((c.SP + 0x18u), c.V0);
        c.V0 = 0x00000080u;
        mem.WriteU32((c.SP + 0x1Cu), c.V0);
        c.V0 = 0x00000004u;
        mem.WriteU32((c.SP + 0x2Cu), c.V0);
        c.V0 = 0x00001004u;
        mem.WriteU32((c.SP + 0x10u), c.S0);
        mem.WriteU32((c.SP + 0x14u), 0u);
        mem.WriteU32((c.SP + 0x20u), 0u);
        mem.WriteU32((c.SP + 0x24u), 0u);
        mem.WriteU32((c.SP + 0x28u), 0u);
        mem.WriteU32((c.SP + 0x30u), 0u);
        c.V0 = c.V0 - c.V1;
        mem.WriteU32((c.S3 + 0x4u), c.V0);
        c.RA = 0x80024614u;
        ShadowTower_st.func_80022678(c, mem);
        goto L80024620;
        L8002461C: ;
        mem.WriteU8((c.S1 + 0x4u), (byte)c.V0);
        L80024620: ;
        c.V0 = (uint)(sbyte)mem.ReadU8((c.S1 + 0x22u));
        c.S2 = 0xFFFFFFFFu;
        if (c.V0 == c.S2) {
            c.S0 = 0x801E0000u;
            goto L8002465C;
        }
        c.S0 = 0x801E0000u;
        c.S0 = c.S0 - 0x2408u;
        c.A0 = c.S0;
        c.RA = 0x8002463Cu;
        ShadowTower_st.SpuGetAllKeysStatus_st(c, mem);
        c.V0 = (uint)(sbyte)mem.ReadU8((c.S1 + 0x22u));
        c.S0 = c.S0 - 0xA38u;
        c.V0 = c.V0 + c.S0;
        c.V0 = mem.ReadU8((c.V0 + 0xA38u));
        if (c.V0 != 0u) {
            goto L8002465C;
        }
        mem.WriteU8((c.S1 + 0x22u), (byte)c.S2);
        L8002465C: ;
        c.S4 = c.S4 - 0x1u;
        c.V1 = mem.ReadU8((c.S1 + 0xBu));
        c.V0 = 0x00000001u;
        c.V1 = c.S3 + c.V1;
        mem.WriteU8((c.V1 + 0xF8u), (byte)c.V0);
        c.V0 = mem.ReadU8((c.S1 + 0xBu));
        c.V1 = mem.ReadU8((c.S1 + 0xCu));
        c.V0 = c.S3 + c.V0;
        mem.WriteU8((c.V0 + 0xC8u), (byte)c.V1);
        c.V0 = 0xFFFFFFFFu;
        if (c.S4 != c.V0) {
            c.V0 = c.S4 << 1;
            goto L80024278;
        }
        c.V0 = c.S4 << 1;
        L8002468C: ;
        c.V0 = 0x801B0000u;
        c.V0 = mem.ReadU8((c.V0 + 0x593Fu));
        if (c.V0 != 0u) {
            c.A0 = 0x00000002u;
            goto L800246B0;
        }
        c.A0 = 0x00000002u;
        c.A1 = 0u;
        c.A2 = 0x00000080u;
        c.A3 = 0x00000010u;
        c.RA = 0x800246B0u;
        ShadowTower_st.func_80023C6C(c, mem);
        L800246B0: ;
        c.RA = mem.ReadU32((c.SP + 0x8Cu));
        c.FP = mem.ReadU32((c.SP + 0x88u));
        c.S7 = mem.ReadU32((c.SP + 0x84u));
        c.S6 = mem.ReadU32((c.SP + 0x80u));
        c.S5 = mem.ReadU32((c.SP + 0x7Cu));
        c.S4 = mem.ReadU32((c.SP + 0x78u));
        c.S3 = mem.ReadU32((c.SP + 0x74u));
        c.S2 = mem.ReadU32((c.SP + 0x70u));
        c.S1 = mem.ReadU32((c.SP + 0x6Cu));
        c.S0 = mem.ReadU32((c.SP + 0x68u));
        c.SP = c.SP + 0x90u;
        return;
    }
}
