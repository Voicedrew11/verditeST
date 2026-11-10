using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_800246E4</c>: an effect drawn when a timer kept in vblanks runs out (calls rand()); FT3 and FT4; not read. <c>ST_EFFECT246E4</c>.
/// </summary>
public static partial class Effect246E4
{
    public static readonly Literal R = new(typeof(Effect246E4), "effect-246E4", "EFFECT246E4", 0x800246E4, Run,
        "an effect drawn when a timer kept in vblanks runs out (calls rand()); FT3 and FT4; not read.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        AfterEntry(c, mem);
        if (c.FP == c.V1) {
            c.V0 = 0x801B0000u;
            goto L80025168;
        }
        c.V0 = 0x801B0000u;
        c.S7 = c.V1;
        c.V0 = c.FP << 1;
        L80024784: ;
        Poll(c, mem);
        After80024784_2(c, mem);
        if (c.V1 == c.V0) {
            mem.WriteU8((c.S0 + 0x7u), (byte)0u);
            goto L80025158;
        }
        After80024784(c, mem);
        if (c.V0 == 0u) {
            c.V0 = c.A0 | 0x0040u;
            goto L80024820;
        }
        c.V0 = c.A0 | 0x0040u;
        c.V0 = mem.ReadU32((c.A1 + 0x6E00u));
        c.LO = (uint)((long)(int)c.V0 * (int)c.V0); c.HI = (uint)(((long)(int)c.V0 * (int)c.V0) >> 32);
        c.T0 = c.LO;
        c.V0 = c.V1 < c.T0 ? 1u : 0u;
        if (c.V0 != 0u) {
            c.V0 = c.A0 | 0x0080u;
            goto L80024824;
        }
        c.V0 = c.A0 | 0x0080u;
        L80024820: ;
        mem.WriteU8((c.S0 + 0x3u), (byte)c.V0);
        L80024824: ;
        c.V1 = mem.ReadU8((c.S0 + 0x5u));
        c.V0 = 0x000000E2u;
        if (c.V1 == c.V0) {
            c.V0 = (int)c.V1 < 227 ? 1u : 0u;
            goto L80025158;
        }
        c.V0 = (int)c.V1 < 227 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.V0 = 0x00000025u;
            goto L80024870;
        }
        c.V0 = 0x00000025u;
        if (c.V1 == c.V0) {
            c.V0 = (int)c.V1 < 38 ? 1u : 0u;
            goto L80025158;
        }
        c.V0 = (int)c.V1 < 38 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.V0 = 0x0000001Fu;
            goto L8002485C;
        }
        c.V0 = 0x0000001Fu;
        if (c.V1 == c.V0) {
            c.S5 = 0x00000001u;
            goto L800248A8;
        }
        c.S5 = 0x00000001u;
        c.T0 = 0x801C0000u;
        goto L80024E2C;
        L8002485C: ;
        c.V0 = 0x00000026u;
        if (c.V1 == c.V0) {
            c.S5 = 0x00000001u;
            goto L80024D44;
        }
        c.S5 = 0x00000001u;
        c.T0 = 0x801C0000u;
        goto L80024E2C;
        L80024870: ;
        c.V0 = 0x000000E9u;
        if (c.V1 == c.V0) {
            c.V0 = (int)c.V1 < 234 ? 1u : 0u;
            goto L80025158;
        }
        c.V0 = (int)c.V1 < 234 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.V0 = 0x000000E5u;
            goto L80024894;
        }
        c.V0 = 0x000000E5u;
        if (c.V1 == c.V0) {
            c.S5 = 0x00000001u;
            goto L80025158;
        }
        c.S5 = 0x00000001u;
        c.T0 = 0x801C0000u;
        goto L80024E2C;
        L80024894: ;
        c.V0 = 0x000000ECu;
        if (c.V1 == c.V0) {
            c.S5 = 0x00000001u;
            goto L80024B18;
        }
        c.S5 = 0x00000001u;
        c.T0 = 0x801C0000u;
        goto L80024E2C;
        L800248A8: ;
        After800248A8_2(c, mem);
        if (c.V0 == 0u) {
            goto L80024AEC;
        }
        c.V0 = mem.ReadU32((c.S0 + 0x50u));
        c.V0 = c.V0 - c.S6;
        if ((int)c.V0 >= 0) {
            goto L80025158;
        }
        c.V0 = mem.ReadU8((c.S0 + 0x4Du));
        c.V0 = c.V0 & 0x0002u;
        if (c.V0 == 0u) {
            goto L80024924;
        }
        After800248A8(c, mem);
        L80024924: ;
        c.V0 = mem.ReadU8((c.S0 + 0x4Du));
        c.V0 = c.V0 & 0x0004u;
        if (c.V0 == 0u) {
            goto L80024964;
        }
        After80024924(c, mem);
        goto L80024980;
        L80024964: ;
        After80024964(c, mem);
        L80024980: ;
        After80024980(c, mem);
        if ((int)c.A0 >= 0) {
            goto L800249AC;
        }
        c.A0 = 0u - c.A0;
        L800249AC: ;
        After800249AC(c, mem);
        if ((int)c.V1 >= 0) {
            goto L800249DC;
        }
        c.V1 = 0u - c.V1;
        L800249DC: ;
        c.V1 = c.A1 - c.V1;
        c.V0 = (int)c.V1 < (int)c.A0 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L800249F0;
        }
        c.A0 = c.V1;
        L800249F0: ;
        c.V0 = mem.ReadU8((c.S0 + 0x4Cu));
        c.S1 = c.V0 << 12;
        c.V0 = (int)c.A0 < (int)c.S1 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L80024A14;
        }
        c.S1 = mem.ReadU8((c.S0 + 0x4Bu));
        goto L80024A60;
        L80024A14: ;
        if (c.S1 == 0u) {
            goto L80025158;
        }
        c.V0 = mem.ReadU8((c.S0 + 0x4Bu));
        c.LO = (uint)((long)(int)c.V0 * (int)c.A0); c.HI = (uint)(((long)(int)c.V0 * (int)c.A0) >> 32);
        c.V0 = c.LO;
        if (c.S1 != 0u) { if ((int)c.V0 == int.MinValue && (int)c.S1 == -1) { c.LO = 0x80000000u; c.HI = 0u; } else { c.LO = (uint)((int)c.V0 / (int)c.S1); c.HI = (uint)((int)c.V0 % (int)c.S1); } }
        if (c.S1 != 0u) {
            goto L80024A44;
        }
        Bios.Break(c, mem);
        L80024A44: ;
        c.At = 0xFFFFFFFFu;
        if (c.S1 != c.At) {
            c.At = 0x80000000u;
            goto L80024A5C;
        }
        c.At = 0x80000000u;
        if (c.V0 != c.At) {
            goto L80024A5C;
        }
        Bios.Break(c, mem);
        L80024A5C: ;
        c.S1 = c.LO;
        L80024A60: ;
        c.V0 = mem.ReadU8((c.S0 + 0x4Du));
        c.V0 = c.V0 & 0x0001u;
        if (c.V0 == 0u) {
            c.V0 = (int)c.S1 < 20 ? 1u : 0u;
            goto L80024AB4;
        }
        After80024A60(c, mem);
        if ((int)c.V1 >= 0) {
            goto L80024A98;
        }
        c.V1 = 0u - c.V1;
        L80024A98: ;
        After80024A98(c, mem);
        L80024AB4: ;
        if (c.V0 != 0u) {
            goto L80025158;
        }
        c.A0 = (uint)(short)mem.ReadU16((c.S0 + 0x54u));
        if (c.A0 == c.S7) {
            goto L80024ADC;
        }
        c.A1 = mem.ReadU8((c.S0 + 0x4Au));
        c.A1 = c.A1 + 0xD0u;
        c.RA = 0x80024AD8u;
        ShadowTower_st.func_8005850C(c, mem);
        mem.WriteU16((c.S0 + 0x54u), (ushort)c.S7);
        L80024ADC: ;
        c.A0 = mem.ReadU8((c.S0 + 0x4Au));
        c.A1 = c.S1;
        c.A0 = c.A0 + 0xD0u;
        goto L80024CF8;
        L80024AEC: ;
        After80024AEC(c, mem);
        if (c.A0 == c.S7) {
            mem.WriteU32((c.S0 + 0x50u), c.V0);
            goto L80025158;
        }
        mem.WriteU32((c.S0 + 0x50u), c.V0);
        c.A1 = mem.ReadU8((c.S0 + 0x4Au));
        c.A1 = c.A1 + 0xD0u;
        goto L80024D34;
        L80024B18: ;
        After80024B18_2(c, mem);
        if (c.V0 == 0u) {
            goto L80024D08;
        }
        c.V0 = mem.ReadU32((c.S0 + 0x50u));
        c.V0 = c.V0 - c.S6;
        if ((int)c.V0 >= 0) {
            goto L80025158;
        }
        c.V0 = mem.ReadU8((c.S0 + 0x4Du));
        c.V0 = c.V0 & 0x0002u;
        if (c.V0 == 0u) {
            goto L80024B94;
        }
        After80024B18(c, mem);
        L80024B94: ;
        c.V0 = mem.ReadU8((c.S0 + 0x4Du));
        c.V0 = c.V0 & 0x0004u;
        if (c.V0 == 0u) {
            goto L80024BD4;
        }
        After80024B94(c, mem);
        goto L80024BF0;
        L80024BD4: ;
        After80024BD4(c, mem);
        L80024BF0: ;
        After80024BF0(c, mem);
        if (c.V0 != 0u) {
            goto L80024C6C;
        }
        if (c.A1 == c.A0) {
            c.V0 = c.A1 - c.A2;
            goto L80024C6C;
        }
        c.V0 = c.A1 - c.A2;
        c.V1 = mem.ReadU8((c.S0 + 0x4Bu));
        c.LO = (uint)((long)(int)c.V1 * (int)c.V0); c.HI = (uint)(((long)(int)c.V1 * (int)c.V0) >> 32);
        c.V1 = c.LO;
        c.V0 = c.A1 - c.A0;
        if (c.V0 != 0u) { if ((int)c.V1 == int.MinValue && (int)c.V0 == -1) { c.LO = 0x80000000u; c.HI = 0u; } else { c.LO = (uint)((int)c.V1 / (int)c.V0); c.HI = (uint)((int)c.V1 % (int)c.V0); } }
        if (c.V0 != 0u) {
            goto L80024C48;
        }
        Bios.Break(c, mem);
        L80024C48: ;
        c.At = 0xFFFFFFFFu;
        if (c.V0 != c.At) {
            c.At = 0x80000000u;
            goto L80024C60;
        }
        c.At = 0x80000000u;
        if (c.V1 != c.At) {
            goto L80024C60;
        }
        Bios.Break(c, mem);
        L80024C60: ;
        c.S1 = c.LO;
        goto L80024C70;
        L80024C6C: ;
        c.S1 = mem.ReadU8((c.S0 + 0x4Bu));
        L80024C70: ;
        c.V0 = mem.ReadU8((c.S0 + 0x4Du));
        c.V0 = c.V0 & 0x0001u;
        if (c.V0 == 0u) {
            c.V0 = (int)c.S1 < 20 ? 1u : 0u;
            goto L80024CC4;
        }
        After80024C70(c, mem);
        if ((int)c.V1 >= 0) {
            goto L80024CA8;
        }
        c.V1 = 0u - c.V1;
        L80024CA8: ;
        After80024CA8(c, mem);
        L80024CC4: ;
        if (c.V0 != 0u) {
            goto L80025158;
        }
        c.A0 = (uint)(short)mem.ReadU16((c.S0 + 0x54u));
        if (c.A0 == c.S7) {
            goto L80024CEC;
        }
        c.A1 = mem.ReadU8((c.S0 + 0x4Au));
        c.A1 = c.A1 + 0x454u;
        c.RA = 0x80024CE8u;
        ShadowTower_st.func_8005850C(c, mem);
        mem.WriteU16((c.S0 + 0x54u), (ushort)c.S7);
        L80024CEC: ;
        c.A0 = mem.ReadU8((c.S0 + 0x4Au));
        c.A1 = c.S1;
        c.A0 = c.A0 + 0x454u;
        L80024CF8: ;
        c.FP = c.FP - 0x1u;
        c.RA = 0x80024D00u;
        ShadowTower_st.func_800582F0(c, mem);
        mem.WriteU16((c.S0 + 0x54u), (ushort)c.V0);
        goto L8002515C;
        L80024D08: ;
        c.V1 = mem.ReadU16((c.S0 + 0x4Eu));
        c.A0 = (uint)(short)mem.ReadU16((c.S0 + 0x54u));
        c.V0 = c.V1 << 4;
        c.V0 = c.V0 - c.V1;
        c.V0 = c.V0 << 2;
        c.V0 = c.S6 + c.V0;
        if (c.A0 == c.S7) {
            mem.WriteU32((c.S0 + 0x50u), c.V0);
            goto L80025158;
        }
        mem.WriteU32((c.S0 + 0x50u), c.V0);
        c.A1 = mem.ReadU8((c.S0 + 0x4Au));
        c.A1 = c.A1 + 0x454u;
        L80024D34: ;
        c.FP = c.FP - 0x1u;
        c.RA = 0x80024D3Cu;
        ShadowTower_st.func_8005850C(c, mem);
        mem.WriteU16((c.S0 + 0x54u), (ushort)c.S7);
        goto L8002515C;
        L80024D44: ;
        After80024D44_2(c, mem);
        if (c.V0 == 0u) {
            c.T0 = 0x80160000u;
            goto L80025158;
        }
        After80024D44(c, mem);
        goto L80025150;
        L80024E2C: ;
        After80024E2C_2(c, mem);
        if (c.V0 == 0u) {
            c.S1 = c.S0 + 0x3Cu;
            goto L80025158;
        }
        After80024E2C_3(c, mem);
        if (c.V0 == 0u) {
            c.T0 = 0x80160000u;
            goto L80024F54;
        }
        After80024E2C(c, mem);
        goto L80025150;
        L80024F54: ;
        After80024F54(c, mem);
        if (c.V0 != 0u) {
            goto L80025158;
        }
        After80024F54_3(c, mem);
        if (c.V0 == 0u) {
            goto L80025080;
        }
        After80024F54_2(c, mem);
        goto L800250A4;
        L80025080: ;
        After80025080(c, mem);
        L800250A4: ;
        After800250A4(c, mem);
        L80025150: ;
        c.RA = 0x80025158u;
        ShadowTower_st.func_80022678(c, mem);
        L80025158: ;
        c.FP = c.FP - 0x1u;
        L8002515C: ;
        if (c.FP != c.S7) {
            c.V0 = c.FP << 1;
            goto L80024784;
        }
        c.V0 = c.FP << 1;
        c.V0 = 0x801B0000u;
        L80025168: ;
        c.V0 = mem.ReadU8((c.V0 + 0x593Fu));
        if (c.V0 != 0u) {
            c.V0 = c.S4 + 0x224u;
            goto L80025190;
        }
        After80025168(c, mem);
        L80025190: ;
        After80025190(c, mem);
        return;
    }
}
