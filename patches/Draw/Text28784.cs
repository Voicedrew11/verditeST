using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_80028784</c>: the draw stage's text when 0x8017BD66 is set; not read. <c>ST_TEXT28784</c>.
/// </summary>
public static class Text28784
{
    public static readonly Literal R = new(typeof(Text28784), "text-28784", "TEXT28784", 0x80028784, Run,
        "the draw stage's text when 0x8017BD66 is set; not read.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x58u;
        c.V0 = 0x80080000u;
        mem.WriteU32((c.SP + 0x50u), c.RA);
        mem.WriteU32((c.SP + 0x4Cu), c.S1);
        mem.WriteU32((c.SP + 0x48u), c.S0);
        c.A2 = c.V0 + 0x7F1Cu;
        c.V1 = mem.ReadWordLeft(c.V1, (c.A2 + 0x3u));
        c.V1 = mem.ReadWordRight(c.V1, c.A2);
        c.A0 = mem.ReadWordLeft(c.A0, (c.A2 + 0x7u));
        c.A0 = mem.ReadWordRight(c.A0, (c.A2 + 0x4u));
        mem.WriteWordLeft((c.SP + 0x1Bu), c.V1);
        mem.WriteWordRight((c.SP + 0x18u), c.V1);
        mem.WriteWordLeft((c.SP + 0x1Fu), c.A0);
        mem.WriteWordRight((c.SP + 0x1Cu), c.A0);
        c.V0 = 0x80080000u;
        c.A2 = c.V0 + 0x7F24u;
        c.V1 = mem.ReadWordLeft(c.V1, (c.A2 + 0x3u));
        c.V1 = mem.ReadWordRight(c.V1, c.A2);
        c.A0 = mem.ReadWordLeft(c.A0, (c.A2 + 0x7u));
        c.A0 = mem.ReadWordRight(c.A0, (c.A2 + 0x4u));
        mem.WriteWordLeft((c.SP + 0x23u), c.V1);
        mem.WriteWordRight((c.SP + 0x20u), c.V1);
        mem.WriteWordLeft((c.SP + 0x27u), c.A0);
        mem.WriteWordRight((c.SP + 0x24u), c.A0);
        c.V0 = 0x000003C0u;
        mem.WriteU16((c.SP + 0x38u), (ushort)c.V0);
        c.V0 = 0x00000100u;
        mem.WriteU16((c.SP + 0x3Au), (ushort)c.V0);
        c.V1 = 0x00010000u;
        c.V1 = c.V1 | 0x8000u;
        c.V0 = 0x00000230u;
        mem.WriteU16((c.SP + 0x40u), (ushort)c.V0);
        c.V0 = 0x000001EFu;
        mem.WriteU16((c.SP + 0x42u), (ushort)c.V0);
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.V1 = c.V0 + c.V1;
        c.V0 = mem.ReadU8((c.V1 + 0x6E17u));
        c.A0 = 0x00000001u;
        if (c.V0 != c.A0) {
            c.V0 = 0x000001FFu;
            goto L80028834;
        }
        c.V0 = 0x000001FFu;
        mem.WriteU16((c.SP + 0x3Au), (ushort)0u);
        mem.WriteU16((c.SP + 0x40u), (ushort)0u);
        mem.WriteU16((c.SP + 0x42u), (ushort)c.V0);
        L80028834: ;
        c.V0 = mem.ReadU8((c.V1 + 0x6E18u));
        if (c.V0 != 0u) {
            goto L80028848;
        }
        mem.WriteU8((c.V1 + 0x6E18u), (byte)c.A0);
        L80028848: ;
        c.V0 = mem.ReadU8((c.V1 + 0x6E16u));
        c.V1 = c.V0 - 0x1u;
        c.V0 = c.V1 < 0x00000006u ? 1u : 0u;
        if (c.V0 == 0u) {
            c.V0 = 0x80010000u;
            goto L80028C14;
        }
        c.V0 = 0x80010000u;
        c.V0 = c.V0 + 0x1490u;
        c.V1 = c.V1 << 2;
        c.V1 = c.V1 + c.V0;
        c.V0 = mem.ReadU32(c.V1);
        switch (c.V0)
        {
            case 0x8002887Cu: goto L8002887C;
            case 0x800288E0u: goto L800288E0;
            case 0x8002897Cu: goto L8002897C;
            case 0x80028A08u: goto L80028A08;
            case 0x80028C14u: goto L80028C14;
            case 0x80028AB0u: goto L80028AB0;
            default: Dispatcher.Call(c, mem, c.V0); return;
        }
        L8002887C: ;
        c.V1 = 0x00010000u;
        c.V1 = c.V1 | 0x8000u;
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.A0 = c.V0 + c.V1;
        c.V1 = mem.ReadU8((c.A0 + 0x6E17u));
        c.V0 = 0x00000003u;
        if (c.V1 == c.V0) {
            c.V0 = 0x000000FFu;
            goto L80028B1C;
        }
        c.V0 = 0x000000FFu;
        c.V0 = mem.ReadU8((c.A0 + 0x6E19u));
        c.V1 = mem.ReadU8((c.A0 + 0x6E18u));
        c.V0 = c.V0 << 4;
        if (c.V1 != 0u) { if ((int)c.V0 == int.MinValue && (int)c.V1 == -1) { c.LO = 0x80000000u; c.HI = 0u; } else { c.LO = (uint)((int)c.V0 / (int)c.V1); c.HI = (uint)((int)c.V0 % (int)c.V1); } }
        if (c.V1 != 0u) {
            goto L800288BC;
        }
        Bios.Break(c, mem);
        L800288BC: ;
        c.At = 0xFFFFFFFFu;
        if (c.V1 != c.At) {
            c.At = 0x80000000u;
            goto L800288D4;
        }
        c.At = 0x80000000u;
        if (c.V0 != c.At) {
            goto L800288D4;
        }
        Bios.Break(c, mem);
        L800288D4: ;
        c.V0 = c.LO;
        mem.WriteU8((c.SP + 0x2Au), (byte)c.V0);
        goto L80028B20;
        L800288E0: ;
        c.V0 = 0x801D0000u;
        c.V1 = mem.ReadU8((c.V0 - 0x2EDCu));
        c.V0 = 0x00000001u;
        if (c.V1 == c.V0) {
            c.V1 = 0x00010000u;
            goto L80028914;
        }
        c.V1 = 0x00010000u;
        c.V1 = c.V1 | 0x8000u;
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.A0 = c.V0 + c.V1;
        c.V1 = mem.ReadU8((c.A0 + 0x6E17u));
        c.V0 = 0x00000002u;
        if (c.V1 != c.V0) {
            c.V0 = 0x00000003u;
            goto L80028974;
        }
        c.V0 = 0x00000003u;
        L80028914: ;
        c.V1 = 0x00010000u;
        c.V1 = c.V1 | 0x8000u;
        c.A0 = c.SP + 0x20u;
        c.A1 = c.SP + 0x38u;
        c.A2 = c.SP + 0x40u;
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.S0 = c.V0 + c.V1;
        c.A3 = c.SP + 0x30u;
        c.V0 = mem.ReadU8((c.S0 + 0x6E19u));
        c.S1 = 0x00000003u;
        c.V0 = c.V0 << 3;
        mem.WriteU8((c.SP + 0x32u), (byte)c.V0);
        mem.WriteU8((c.SP + 0x31u), (byte)c.V0);
        mem.WriteU8((c.SP + 0x30u), (byte)c.V0);
        mem.WriteU32((c.SP + 0x10u), c.S1);
        c.RA = 0x80028958u;
        ShadowTower_st.func_80028364(c, mem);
        c.V0 = mem.ReadU8((c.S0 + 0x6E19u));
        c.V0 = c.V0 < 0x00000010u ? 1u : 0u;
        if (c.V0 != 0u) {
            c.V0 = 0x00010000u;
            goto L80028B4C;
        }
        c.V0 = 0x00010000u;
        mem.WriteU8((c.S0 + 0x6E16u), (byte)c.S1);
        goto L80028B4C;
        L80028974: ;
        mem.WriteU8((c.A0 + 0x6E16u), (byte)c.V0);
        goto L80028B48;
        L8002897C: ;
        c.V0 = 0x801D0000u;
        c.V1 = mem.ReadU8((c.V0 - 0x2EDCu));
        c.V0 = 0x00000001u;
        if (c.V1 == c.V0) {
            c.V1 = 0x00010000u;
            goto L800289B0;
        }
        c.V1 = 0x00010000u;
        c.V1 = c.V1 | 0x8000u;
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.V0 = c.V0 + c.V1;
        c.V1 = mem.ReadU8((c.V0 + 0x6E17u));
        c.V0 = 0x00000002u;
        if (c.V1 != c.V0) {
            c.V0 = 0x00010000u;
            goto L80028B4C;
        }
        c.V0 = 0x00010000u;
        L800289B0: ;
        c.V1 = 0x80090000u;
        c.V1 = mem.ReadU16((c.V1 - 0x7E70u));
        c.V0 = 0x00000080u;
        mem.WriteU8((c.SP + 0x32u), (byte)c.V0);
        mem.WriteU8((c.SP + 0x31u), (byte)c.V0);
        mem.WriteU8((c.SP + 0x30u), (byte)c.V0);
        c.V0 = 0x00000019u;
        if (c.V1 == c.V0) {
            c.V0 = 0x0000001Eu;
            goto L800289E4;
        }
        c.V0 = 0x0000001Eu;
        if (c.V1 == c.V0) {
            c.V0 = 0x00000005u;
            goto L800289E4;
        }
        c.V0 = 0x00000005u;
        if (c.V1 != c.V0) {
            c.V0 = 0x00000003u;
            goto L800289E8;
        }
        c.V0 = 0x00000003u;
        L800289E4: ;
        c.V0 = 0x00000002u;
        L800289E8: ;
        mem.WriteU32((c.SP + 0x10u), c.V0);
        c.A0 = c.SP + 0x20u;
        c.A1 = c.SP + 0x38u;
        c.A2 = c.SP + 0x40u;
        c.A3 = c.SP + 0x30u;
        c.RA = 0x80028A00u;
        ShadowTower_st.func_80028364(c, mem);
        c.V0 = 0x00010000u;
        goto L80028B4C;
        L80028A08: ;
        c.V0 = 0x801D0000u;
        c.V1 = mem.ReadU8((c.V0 - 0x2EDCu));
        c.V0 = 0x00000001u;
        if (c.V1 == c.V0) {
            c.V1 = 0x00010000u;
            goto L80028A3C;
        }
        c.V1 = 0x00010000u;
        c.V1 = c.V1 | 0x8000u;
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.A0 = c.V0 + c.V1;
        c.V1 = mem.ReadU8((c.A0 + 0x6E17u));
        c.V0 = 0x00000002u;
        if (c.V1 != c.V0) {
            c.V0 = 0x00000005u;
            goto L80028AA8;
        }
        c.V0 = 0x00000005u;
        L80028A3C: ;
        c.V1 = 0x00010000u;
        c.V1 = c.V1 | 0x8000u;
        c.A0 = c.SP + 0x20u;
        c.A1 = c.SP + 0x38u;
        c.A2 = c.SP + 0x40u;
        c.A3 = c.SP + 0x30u;
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.S0 = c.V0 + c.V1;
        c.V1 = mem.ReadU8((c.S0 + 0x6E19u));
        c.V0 = 0x00000010u;
        c.V0 = c.V0 - c.V1;
        c.V0 = c.V0 << 3;
        mem.WriteU8((c.SP + 0x32u), (byte)c.V0);
        mem.WriteU8((c.SP + 0x31u), (byte)c.V0);
        mem.WriteU8((c.SP + 0x30u), (byte)c.V0);
        c.V0 = 0x00000003u;
        mem.WriteU32((c.SP + 0x10u), c.V0);
        c.RA = 0x80028A88u;
        ShadowTower_st.func_80028364(c, mem);
        c.V0 = mem.ReadU8((c.S0 + 0x6E19u));
        c.V0 = c.V0 < 0x00000010u ? 1u : 0u;
        if (c.V0 != 0u) {
            c.V0 = 0x00010000u;
            goto L80028B4C;
        }
        c.V0 = 0x00010000u;
        c.V0 = 0x00000005u;
        mem.WriteU8((c.S0 + 0x6E16u), (byte)c.V0);
        goto L80028B48;
        L80028AA8: ;
        mem.WriteU8((c.A0 + 0x6E16u), (byte)c.V0);
        goto L80028B48;
        L80028AB0: ;
        c.V1 = 0x00010000u;
        c.V1 = c.V1 | 0x8000u;
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.A0 = c.V0 + c.V1;
        c.V1 = mem.ReadU8((c.A0 + 0x6E17u));
        c.V0 = 0x00000003u;
        if (c.V1 == c.V0) {
            c.V0 = 0x000000FFu;
            goto L80028B1C;
        }
        c.V0 = 0x000000FFu;
        c.V1 = mem.ReadU8((c.A0 + 0x6E18u));
        c.V0 = mem.ReadU8((c.A0 + 0x6E19u));
        c.V0 = c.V1 - c.V0;
        c.V0 = c.V0 << 4;
        if (c.V1 != 0u) { if ((int)c.V0 == int.MinValue && (int)c.V1 == -1) { c.LO = 0x80000000u; c.HI = 0u; } else { c.LO = (uint)((int)c.V0 / (int)c.V1); c.HI = (uint)((int)c.V0 % (int)c.V1); } }
        if (c.V1 != 0u) {
            goto L80028AF8;
        }
        Bios.Break(c, mem);
        L80028AF8: ;
        c.At = 0xFFFFFFFFu;
        if (c.V1 != c.At) {
            c.At = 0x80000000u;
            goto L80028B10;
        }
        c.At = 0x80000000u;
        if (c.V0 != c.At) {
            goto L80028B10;
        }
        Bios.Break(c, mem);
        L80028B10: ;
        c.V0 = c.LO;
        mem.WriteU8((c.SP + 0x2Au), (byte)c.V0);
        goto L80028B20;
        L80028B1C: ;
        mem.WriteU8((c.SP + 0x2Au), (byte)c.V0);
        L80028B20: ;
        mem.WriteU8((c.SP + 0x29u), (byte)c.V0);
        mem.WriteU8((c.SP + 0x28u), (byte)c.V0);
        c.A0 = c.SP + 0x18u;
        c.A1 = c.SP + 0x28u;
        c.A2 = 0x00000001u;
        c.RA = 0x80028B38u;
        ShadowTower_st.func_80028220(c, mem);
        c.A0 = 0x00000002u;
        c.A1 = 0u;
        c.A2 = c.A1;
        c.RA = 0x80028B48u;
        ShadowTower_st.func_800286A8(c, mem);
        L80028B48: ;
        c.V0 = 0x00010000u;
        L80028B4C: ;
        c.V0 = c.V0 | 0x8000u;
        c.V1 = 0x80160000u;
        c.V1 = c.V1 - 0x2F70u;
        c.V1 = c.V1 + c.V0;
        c.V0 = mem.ReadU8((c.V1 + 0x6E17u));
        if (c.V0 != 0u) {
            goto L80028BB4;
        }
        c.V1 = mem.ReadU8((c.V1 + 0x6E16u));
        c.V0 = c.V1 < 0x00000002u ? 1u : 0u;
        if (c.V0 != 0u) {
            c.V0 = c.V1 < 0x00000006u ? 1u : 0u;
            goto L80028BB4;
        }
        c.V0 = c.V1 < 0x00000006u ? 1u : 0u;
        if (c.V0 == 0u) {
            c.A0 = c.SP + 0x18u;
            goto L80028BB4;
        }
        c.A0 = c.SP + 0x18u;
        c.A1 = c.SP + 0x28u;
        c.A2 = 0x00000001u;
        c.V0 = 0x00000010u;
        mem.WriteU8((c.SP + 0x2Au), (byte)c.V0);
        mem.WriteU8((c.SP + 0x29u), (byte)c.V0);
        mem.WriteU8((c.SP + 0x28u), (byte)c.V0);
        c.RA = 0x80028BA4u;
        ShadowTower_st.func_80028220(c, mem);
        c.A0 = 0x00000002u;
        c.A1 = 0u;
        c.A2 = c.A1;
        c.RA = 0x80028BB4u;
        ShadowTower_st.func_800286A8(c, mem);
        L80028BB4: ;
        c.V1 = 0x00010000u;
        c.V1 = c.V1 | 0x8000u;
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.S0 = c.V0 + c.V1;
        c.V1 = mem.ReadU8((c.S0 + 0x6E17u));
        c.V0 = 0x00000003u;
        if (c.V1 != c.V0) {
            c.A0 = c.SP + 0x18u;
            goto L80028C04;
        }
        c.A0 = c.SP + 0x18u;
        c.A1 = c.SP + 0x28u;
        c.A2 = 0x00000001u;
        c.V0 = 0x000000FFu;
        mem.WriteU8((c.SP + 0x2Au), (byte)c.V0);
        mem.WriteU8((c.SP + 0x29u), (byte)c.V0);
        mem.WriteU8((c.SP + 0x28u), (byte)c.V0);
        c.RA = 0x80028BF4u;
        ShadowTower_st.func_80028220(c, mem);
        c.A0 = 0x00000002u;
        c.A1 = 0u;
        c.A2 = c.A1;
        c.RA = 0x80028C04u;
        ShadowTower_st.func_800286A8(c, mem);
        L80028C04: ;
        c.V0 = mem.ReadU8((c.S0 + 0x6E19u));
        c.V0 = c.V0 + 0x1u;
        mem.WriteU8((c.S0 + 0x6E19u), (byte)c.V0);
        L80028C14: ;
        c.RA = mem.ReadU32((c.SP + 0x50u));
        c.S1 = mem.ReadU32((c.SP + 0x4Cu));
        c.S0 = mem.ReadU32((c.SP + 0x48u));
        c.SP = c.SP + 0x58u;
        return;
    }
}
