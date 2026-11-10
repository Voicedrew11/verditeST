using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_80016A14</c>: the draw stage's second call; calls func_80079DCC and func_80079D0C (libgpu); not read. <c>ST_DRAW16A14</c>.
/// </summary>
public static partial class Draw16A14
{
    public static readonly Literal R = new(typeof(Draw16A14), "draw-16A14", "DRAW16A14", 0x80016A14, Run,
        "the draw stage's second call; calls func_80079DCC and func_80079D0C (libgpu); not read.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        AfterEntry(c, mem);
        L80016A60: ;
        Poll(c, mem);
        c.V0 = 0x801B0000u;
        c.V1 = (uint)(short)mem.ReadU16((c.S3 + 0x2u));
        c.V0 = mem.ReadU8((c.V0 + 0x5929u));
        if (c.V1 == c.V0) {
            c.V0 = 0x000000FFu;
            goto L80016A80;
        }
        c.V0 = 0x000000FFu;
        if (c.V1 != c.V0) {
            goto L80016F68;
        }
        L80016A80: ;
        c.V0 = mem.ReadU8(c.S3);
        if (c.V0 != 0u) {
            c.V0 = c.V0 - 0x1u;
            goto L80016F64;
        }
        After80016A80(c, mem);
        if (c.V0 == 0u) {
            c.V0 = (int)c.V1 < 17 ? 1u : 0u;
            goto L80016AF8;
        }
        c.V0 = (int)c.V1 < 17 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L80016E6C;
        }
        if (c.V1 == 0u) {
            c.V0 = (int)c.V1 < 6 ? 1u : 0u;
            goto L80016F68;
        }
        c.V0 = (int)c.V1 < 6 ? 1u : 0u;
        if (c.V0 != 0u) {
            c.V0 = (int)c.V1 < 10 ? 1u : 0u;
            goto L80016B24;
        }
        c.V0 = (int)c.V1 < 10 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L80016F68;
        }
        goto L80016BC4;
        L80016AF8: ;
        c.V0 = 0x00000031u;
        if (c.V1 == c.V0) {
            c.V0 = (int)c.V1 < 50 ? 1u : 0u;
            goto L80016EDC;
        }
        c.V0 = (int)c.V1 < 50 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.V0 = (int)c.V1 < 38 ? 1u : 0u;
            goto L80016F68;
        }
        c.V0 = (int)c.V1 < 38 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.V0 = (int)c.V1 < 33 ? 1u : 0u;
            goto L80016F68;
        }
        c.V0 = (int)c.V1 < 33 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L80016CD8;
        }
        c.S3 = c.S3 + 0x18u;
        goto L80016F6C;
        L80016B24: ;
        c.V1 = mem.ReadU8((c.S3 + 0x1u));
        c.V0 = mem.ReadU16((c.S3 + 0x4u));
        c.A0 = c.V0 + c.V1;
        c.V0 = c.A0 << 16;
        c.V1 = (uint)(short)mem.ReadU16((c.S3 + 0x14u));
        c.V0 = (uint)((int)c.V0 >> 16);
        mem.WriteU16((c.S3 + 0x4u), (ushort)c.A0);
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        c.V1 = mem.ReadU16((c.S3 + 0x14u));
        if (c.V0 != 0u) {
            c.V0 = c.A0 - c.V1;
            goto L80016B58;
        }
        c.V0 = c.A0 - c.V1;
        mem.WriteU16((c.S3 + 0x4u), (ushort)c.V0);
        L80016B58: ;
        After80016B58_2(c, mem);
        if (c.V0 == 0u) {
            c.A0 = c.S7;
            goto L80016F68;
        }
        After80016B58(c, mem);
        goto L80016F6C;
        L80016BC4: ;
        c.V1 = mem.ReadU8((c.S3 + 0x1u));
        c.V0 = mem.ReadU16((c.S3 + 0x4u));
        c.A0 = c.V0 + c.V1;
        c.V0 = c.A0 << 16;
        c.V1 = (uint)(short)mem.ReadU16((c.S3 + 0x12u));
        c.V0 = (uint)((int)c.V0 >> 16);
        mem.WriteU16((c.S3 + 0x4u), (ushort)c.A0);
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        c.V1 = mem.ReadU16((c.S3 + 0x12u));
        if (c.V0 != 0u) {
            c.V0 = 0x00000001u;
            goto L80016C00;
        }
        c.V0 = 0x00000001u;
        c.V0 = c.A0 - c.V1;
        mem.WriteU16((c.S3 + 0x4u), (ushort)c.V0);
        c.V0 = 0x00000001u;
        L80016C00: ;
        c.S5 = mem.ReadU16((c.S3 + 0x14u));
        c.T9 = 0xFFFFFFFFu;
        mem.WriteU16((c.SP + 0x16u), (ushort)c.V0);
        c.S2 = mem.ReadU16((c.S3 + 0x4u));
        c.S1 = (uint)(short)mem.ReadU16((c.S3 + 0x8u));
        c.V0 = c.S5 - 0x1u;
        c.S5 = c.V0;
        c.V0 = c.V0 << 16;
        c.V0 = (uint)((int)c.V0 >> 16);
        if (c.V0 == c.T9) {
            goto L80016F68;
        }
        L80016C2C: ;
        Poll(c, mem);
        c.V0 = c.S2 << 16;
        c.V1 = (uint)(short)mem.ReadU16((c.S3 + 0x12u));
        c.V0 = (uint)((int)c.V0 >> 16);
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        c.V1 = mem.ReadU16((c.S3 + 0x12u));
        if (c.V0 != 0u) {
            c.A0 = c.SP + 0x10u;
            goto L80016C4C;
        }
        c.A0 = c.SP + 0x10u;
        c.S2 = c.S2 - c.V1;
        L80016C4C: ;
        After80016C4C(c, mem);
        if (c.S0 == 0u) {
            c.A0 = c.SP + 0x10u;
            goto L80016CA4;
        }
        c.A0 = c.SP + 0x10u;
        c.S4 = (uint)(short)mem.ReadU16((c.S3 + 0x6u));
        c.V0 = mem.ReadU16((c.S3 + 0xEu));
        c.V1 = mem.ReadU16((c.SP + 0x14u));
        c.A2 = c.S1;
        mem.WriteU16((c.SP + 0x14u), (ushort)c.S2);
        c.A1 = c.S4;
        c.V0 = c.V0 + c.V1;
        mem.WriteU16((c.SP + 0x10u), (ushort)c.V0);
        c.RA = 0x80016CA4u;
        ShadowTower_st.func_80079DCC(c, mem);
        L80016CA4: ;
        c.S1 = c.S1 + 0x1u;
        c.S2 = c.S2 + 0x1u;
        c.V0 = c.S5 - 0x1u;
        c.S5 = c.V0;
        c.V0 = c.V0 << 16;
        c.V0 = (uint)((int)c.V0 >> 16);
        c.V1 = mem.ReadU16((c.SP + 0x12u));
        c.T8 = 0xFFFFFFFFu;
        c.V1 = c.V1 + 0x1u;
        if (c.V0 != c.T8) {
            mem.WriteU16((c.SP + 0x12u), (ushort)c.V1);
            goto L80016C2C;
        }
        mem.WriteU16((c.SP + 0x12u), (ushort)c.V1);
        c.S3 = c.S3 + 0x18u;
        goto L80016F6C;
        L80016CD8: ;
        c.V1 = mem.ReadU8((c.S3 + 0x1u));
        c.V0 = mem.ReadU16((c.S3 + 0x4u));
        c.A0 = c.V0 + c.V1;
        c.V0 = c.A0 << 16;
        c.V1 = (uint)(short)mem.ReadU16((c.S3 + 0x14u));
        c.V0 = (uint)((int)c.V0 >> 16);
        mem.WriteU16((c.S3 + 0x4u), (ushort)c.A0);
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        c.V1 = mem.ReadU16((c.S3 + 0x14u));
        if (c.V0 != 0u) {
            c.V0 = 0x00000001u;
            goto L80016D14;
        }
        c.V0 = 0x00000001u;
        c.V0 = c.A0 - c.V1;
        mem.WriteU16((c.S3 + 0x4u), (ushort)c.V0);
        c.V0 = 0x00000001u;
        L80016D14: ;
        c.S2 = mem.ReadU16((c.S3 + 0x12u));
        c.T9 = 0xFFFFFFFFu;
        mem.WriteU16((c.SP + 0x14u), (ushort)c.V0);
        c.S5 = mem.ReadU16((c.S3 + 0x4u));
        c.S4 = (uint)(short)mem.ReadU16((c.S3 + 0x6u));
        c.V0 = c.S2 - 0x1u;
        c.S2 = c.V0;
        c.V0 = c.V0 << 16;
        c.V0 = (uint)((int)c.V0 >> 16);
        if (c.V0 == c.T9) {
            goto L80016F68;
        }
        L80016D40: ;
        Poll(c, mem);
        c.V0 = (uint)(short)mem.ReadU16((c.S3 + 0x12u));
        c.V1 = 0x00001000u;
        if (c.V0 != 0u) { if ((int)c.V1 == int.MinValue && (int)c.V0 == -1) { c.LO = 0x80000000u; c.HI = 0u; } else { c.LO = (uint)((int)c.V1 / (int)c.V0); c.HI = (uint)((int)c.V1 % (int)c.V0); } }
        if (c.V0 != 0u) {
            goto L80016D58;
        }
        Bios.Break(c, mem);
        L80016D58: ;
        c.At = 0xFFFFFFFFu;
        if (c.V0 != c.At) {
            c.At = 0x80000000u;
            goto L80016D70;
        }
        c.At = 0x80000000u;
        if (c.V1 != c.At) {
            goto L80016D70;
        }
        Bios.Break(c, mem);
        L80016D70: ;
        After80016D70(c, mem);
        if ((int)c.A0 >= 0) {
            c.V1 = (uint)((int)c.V1 >> 1);
            goto L80016DB4;
        }
        c.V1 = (uint)((int)c.V1 >> 1);
        c.A1 = c.A0 + 0x3u;
        L80016DB4: ;
        c.V0 = (uint)((int)c.A1 >> 2);
        c.LO = (uint)((long)(int)c.V0 * (int)c.A2); c.HI = (uint)(((long)(int)c.V0 * (int)c.A2) >> 32);
        c.V0 = c.LO;
        if ((int)c.V0 >= 0) {
            goto L80016DCC;
        }
        c.V0 = c.V0 + 0xFFFu;
        L80016DCC: ;
        c.V0 = (uint)((int)c.V0 >> 12);
        c.S0 = c.V1 + c.V0;
        c.V0 = (int)c.S0 < (int)c.A0 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L80016DE4;
        }
        c.S0 = c.S0 - c.A0;
        L80016DE4: ;
        c.A0 = c.SP + 0x10u;
        c.A2 = (uint)(short)mem.ReadU16((c.S3 + 0x8u));
        c.V0 = mem.ReadU16((c.S3 + 0x10u));
        c.A1 = c.S4;
        mem.WriteU16((c.SP + 0x12u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S3 + 0x14u));
        c.A2 = c.A2 + c.S0;
        c.V0 = c.V0 - c.S0;
        mem.WriteU16((c.SP + 0x16u), (ushort)c.V0);
        c.RA = 0x80016E0Cu;
        ShadowTower_st.func_80079DCC(c, mem);
        if (c.S0 == 0u) {
            c.A0 = c.SP + 0x10u;
            goto L80016E38;
        }
        c.A0 = c.SP + 0x10u;
        c.S1 = (uint)(short)mem.ReadU16((c.S3 + 0x8u));
        c.V0 = mem.ReadU16((c.S3 + 0x10u));
        c.V1 = mem.ReadU16((c.SP + 0x16u));
        c.A1 = c.S4;
        mem.WriteU16((c.SP + 0x16u), (ushort)c.S0);
        c.A2 = c.S1;
        c.V0 = c.V0 + c.V1;
        mem.WriteU16((c.SP + 0x12u), (ushort)c.V0);
        c.RA = 0x80016E38u;
        ShadowTower_st.func_80079DCC(c, mem);
        L80016E38: ;
        c.S4 = c.S4 + 0x1u;
        c.S5 = c.S5 + 0x1u;
        c.V0 = c.S2 - 0x1u;
        c.S2 = c.V0;
        c.V0 = c.V0 << 16;
        c.V0 = (uint)((int)c.V0 >> 16);
        c.V1 = mem.ReadU16((c.SP + 0x10u));
        c.T8 = 0xFFFFFFFFu;
        c.V1 = c.V1 + 0x1u;
        if (c.V0 != c.T8) {
            mem.WriteU16((c.SP + 0x10u), (ushort)c.V1);
            goto L80016D40;
        }
        mem.WriteU16((c.SP + 0x10u), (ushort)c.V1);
        c.S3 = c.S3 + 0x18u;
        goto L80016F6C;
        L80016E6C: ;
        c.V0 = mem.ReadU16((c.S3 + 0x4u));
        c.V1 = mem.ReadU8((c.S3 + 0x1u));
        c.A0 = c.V0 + 0x1u;
        c.V0 = c.A0 << 16;
        c.V0 = (uint)((int)c.V0 >> 16);
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 != 0u) {
            mem.WriteU16((c.S3 + 0x4u), (ushort)c.A0);
            goto L80016EA8;
        }
        mem.WriteU16((c.S3 + 0x4u), (ushort)c.A0);
        mem.WriteU16((c.S3 + 0x4u), (ushort)0u);
        c.A0 = c.S7;
        c.A1 = 0x800A0000u;
        c.A1 = c.A1 - 0x5378u;
        c.RA = 0x80016EA0u;
        ShadowTower_st.func_80079D0C(c, mem);
        c.S3 = c.S3 + 0x18u;
        goto L80016F6C;
        L80016EA8: ;
        After80016EA8(c, mem);
        goto L80016F6C;
        L80016EDC: ;
        After80016EDC(c, mem);
        goto L80016F6C;
        L80016F64: ;
        mem.WriteU8(c.S3, (byte)c.V0);
        L80016F68: ;
        c.S3 = c.S3 + 0x18u;
        L80016F6C: ;
        c.FP = c.FP + 0x18u;
        c.T8 = mem.ReadU32((c.SP + 0x18u));
        c.T9 = 0xFFFFFFFFu;
        c.T8 = c.T8 - 0x1u;
        if (c.T8 != c.T9) {
            mem.WriteU32((c.SP + 0x18u), c.T8);
            goto L80016A60;
        }
        After80016F6C(c, mem);
        return;
    }
}
