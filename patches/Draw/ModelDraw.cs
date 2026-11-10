using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_80022678</c>: one model: its matrix, light and pose, then func_80022010 for its polygons. <c>ST_MODELDRAW</c>.
/// </summary>
public static partial class ModelDraw
{
    public static readonly Literal R = new(typeof(ModelDraw), "model draw", "MODELDRAW", 0x80022678, Run,
        "one model: its matrix, light and pose, then func_80022010 for its polygons.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        AfterEntry(c, mem);
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
        After8002273C(c, mem);
        L80022764: ;
        After80022764_2(c, mem);
        if (c.FP == 0u) {
            c.A1 = 0x801A0000u;
            goto L8002288C;
        }
        After80022764(c, mem);
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
        After800228B4(c, mem);
        if (c.S7 == 0u) {
            c.A0 = c.S1;
            goto L80022A24;
        }
        After800228B4_2(c, mem);
        L80022A24: ;
        if (c.FP == 0u) {
            goto L80022B08;
        }
        After80022A24(c, mem);
        L80022B08: ;
        After80022B08(c, mem);
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
        After80022C40(c, mem);
        L80022C68: ;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32((c.S0 + 0x48u), c.V1);
        c.V0 = c.S4 & 0x0040u;
        L80022C74: ;
        if (c.V0 == 0u) {
            c.V0 = c.S4 & 0x0080u;
            goto L80022E28;
        }
        After80022C74(c, mem);
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
        After80022D8C(c, mem);
        if (c.A0 == c.V0) {
            c.A3 = c.S0 + 0x50u;
            goto L80023178;
        }
        c.A3 = c.S0 + 0x50u;
        c.T0 = c.V0;
        c.V1 = c.A1 + 0x4u;
        L80022DC8: ;
        Poll(c, mem);
        After80022DC8(c, mem);
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
        After80022E28(c, mem);
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
        After80022F40(c, mem);
        if (c.A0 == c.V0) {
            c.A3 = c.S0 + 0x50u;
            goto L80023178;
        }
        c.A3 = c.S0 + 0x50u;
        c.T0 = c.V0;
        c.V1 = c.A1 + 0x4u;
        L80022F7C: ;
        Poll(c, mem);
        After80022F7C(c, mem);
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
        After80022FDC(c, mem);
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
        After800230E4(c, mem);
        if (c.A0 == c.V0) {
            c.A3 = c.S0 + 0x50u;
            goto L80023178;
        }
        c.A3 = c.S0 + 0x50u;
        c.T0 = c.V0;
        c.V1 = c.A1 + 0x4u;
        L80023120: ;
        Poll(c, mem);
        After80023120(c, mem);
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
        After80023188(c, mem);
        return;
    }
}
