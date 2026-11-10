using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_80021AC4</c>: a model drawn with func_80022010 from the draw stage, called twice (0 and 1): a hand each, probably; drew nothing in map 0. <c>ST_HELDMODEL</c>.
/// </summary>
public static class HeldModel
{
    public static readonly Literal R = new(typeof(HeldModel), "held model", "HELDMODEL", 0x80021AC4, Run,
        "a model drawn with func_80022010 from the draw stage, called twice (0 and 1): a hand each, probably; drew nothing in map 0.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x50u;
        mem.WriteU32((c.SP + 0x44u), c.S3);
        c.S3 = c.A0;
        c.V0 = 0x801A0000u;
        mem.WriteU32((c.SP + 0x40u), c.S2);
        c.S2 = c.V0 - 0x70D8u;
        mem.WriteU32((c.SP + 0x3Cu), c.S1);
        c.S1 = c.S3 & 0x00FFu;
        c.V0 = c.S1 << 1;
        c.V0 = c.V0 + c.S2;
        mem.WriteU32((c.SP + 0x48u), c.RA);
        mem.WriteU32((c.SP + 0x38u), c.S0);
        c.V1 = (uint)(short)mem.ReadU16((c.V0 + 0x130u));
        c.V0 = 0xFFFFFFFFu;
        if (c.V1 == c.V0) {
            c.A0 = 0u;
            goto L80021FF0;
        }
        c.A0 = 0u;
        c.A1 = c.A0;
        c.RA = 0x80021B0Cu;
        ShadowTower_st.func_80017D28(c, mem);
        c.S0 = 0x1F800000u;
        c.V0 = mem.ReadU32((c.S0 + 0x394u));
        c.V0 = c.V0 + 0x3Cu;
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
        c.T3 = 0x80180000u;
        c.T3 = c.T3 - 0x4288u;
        c.T4 = mem.ReadU16(c.T3);
        c.T5 = mem.ReadU16((c.T3 + 0x6u));
        c.T6 = mem.ReadU16((c.T3 + 0xCu));
        RecompOne.Runtime.Gte.Write(9, c.T4);
        RecompOne.Runtime.Gte.Write(10, c.T5);
        RecompOne.Runtime.Gte.Write(11, c.T6);
        RecompOne.Runtime.Gte.MvmvaOp(12, false, 0, 3, 3);
        c.V1 = c.SP + 0x10u;
        c.T4 = RecompOne.Runtime.Gte.Read(9);
        c.T5 = RecompOne.Runtime.Gte.Read(10);
        c.T6 = RecompOne.Runtime.Gte.Read(11);
        mem.WriteU16(c.V1, (ushort)c.T4);
        mem.WriteU16((c.V1 + 0x6u), (ushort)c.T5);
        mem.WriteU16((c.V1 + 0xCu), (ushort)c.T6);
        c.T3 = 0x80180000u;
        c.T3 = c.T3 - 0x4286u;
        c.T4 = mem.ReadU16(c.T3);
        c.T5 = mem.ReadU16((c.T3 + 0x6u));
        c.T6 = mem.ReadU16((c.T3 + 0xCu));
        RecompOne.Runtime.Gte.Write(9, c.T4);
        RecompOne.Runtime.Gte.Write(10, c.T5);
        RecompOne.Runtime.Gte.Write(11, c.T6);
        RecompOne.Runtime.Gte.MvmvaOp(12, false, 0, 3, 3);
        c.V0 = c.SP + 0x12u;
        c.T4 = RecompOne.Runtime.Gte.Read(9);
        c.T5 = RecompOne.Runtime.Gte.Read(10);
        c.T6 = RecompOne.Runtime.Gte.Read(11);
        mem.WriteU16(c.V0, (ushort)c.T4);
        mem.WriteU16((c.V0 + 0x6u), (ushort)c.T5);
        mem.WriteU16((c.V0 + 0xCu), (ushort)c.T6);
        c.T3 = 0x80180000u;
        c.T3 = c.T3 - 0x4284u;
        c.T4 = mem.ReadU16(c.T3);
        c.T5 = mem.ReadU16((c.T3 + 0x6u));
        c.T6 = mem.ReadU16((c.T3 + 0xCu));
        RecompOne.Runtime.Gte.Write(9, c.T4);
        RecompOne.Runtime.Gte.Write(10, c.T5);
        RecompOne.Runtime.Gte.Write(11, c.T6);
        RecompOne.Runtime.Gte.MvmvaOp(12, false, 0, 3, 3);
        c.V0 = c.SP + 0x14u;
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
        c.V0 = c.S1 << 2;
        c.A1 = c.V0 + c.S2;
        c.V0 = mem.ReadU32((c.A1 + 0xE8u));
        c.V0 = (uint)(short)mem.ReadU16((c.V0 + 0xCu));
        mem.WriteU32((c.S0 + 0xBCu), c.V0);
        c.V0 = mem.ReadU32((c.A1 + 0xE8u));
        c.V0 = (uint)(short)mem.ReadU16((c.V0 + 0xEu));
        mem.WriteU32((c.S0 + 0xC0u), c.V0);
        c.V0 = mem.ReadU32((c.A1 + 0xE8u));
        c.V0 = (uint)(short)mem.ReadU16((c.V0 + 0x10u));
        mem.WriteU32((c.S0 + 0xC4u), c.V0);
        c.V1 = mem.ReadU16((c.S2 + 0x58u));
        c.V0 = 0u | 0xFFFFu;
        if (c.V1 == c.V0) {
            c.V0 = 0x801A0000u;
            goto L80021CF0;
        }
        c.V0 = 0x801A0000u;
        if (c.S1 == 0u) {
            c.A0 = c.SP + 0x30u;
            goto L80021CF0;
        }
        c.A0 = c.SP + 0x30u;
        c.V0 = mem.ReadU32((c.A1 + 0xE8u));
        c.V1 = mem.ReadU16((c.S2 + 0x2DEu));
        c.V0 = mem.ReadU16((c.V0 + 0x14u));
        c.V0 = c.V0 + c.V1;
        mem.WriteU16((c.SP + 0x30u), (ushort)c.V0);
        c.V0 = mem.ReadU32((c.A1 + 0xE8u));
        c.V1 = mem.ReadU16((c.S2 + 0x2E0u));
        c.V0 = mem.ReadU16((c.V0 + 0x16u));
        c.V0 = c.V0 + c.V1;
        mem.WriteU16((c.SP + 0x32u), (ushort)c.V0);
        c.V0 = mem.ReadU32((c.A1 + 0xE8u));
        c.A1 = c.S0;
        c.V1 = mem.ReadU16((c.S2 + 0x2E2u));
        c.V0 = mem.ReadU16((c.V0 + 0x18u));
        c.A1 = c.A1 | 0x00A8u;
        c.V0 = c.V0 + c.V1;
        mem.WriteU16((c.SP + 0x34u), (ushort)c.V0);
        goto L80021D0C;
        L80021CF0: ;
        c.V0 = c.V0 - 0x70D8u;
        c.V1 = c.S3 & 0x00FFu;
        c.V1 = c.V1 << 2;
        c.V1 = c.V1 + c.V0;
        c.A0 = mem.ReadU32((c.V1 + 0xE8u));
        c.A1 = c.S0 | 0x00A8u;
        c.A0 = c.A0 + 0x14u;
        L80021D0C: ;
        c.RA = 0x80021D14u;
        ShadowTower_st.RotMatrix_st(c, mem);
        c.V0 = 0x00000040u;
        mem.WriteU32((c.S0 + 0x4u), c.V0);
        c.V0 = 0x80160000u;
        c.T2 = c.V0 - 0x2F70u;
        c.V1 = 0u | 0x8000u;
        c.V1 = c.T2 + c.V1;
        c.A0 = mem.ReadU32((c.V1 + 0x2E8u));
        c.A1 = mem.ReadU32((c.V1 + 0x31Cu));
        c.A2 = mem.ReadU32((c.V1 + 0x320u));
        c.A3 = mem.ReadU32((c.V1 + 0x324u));
        c.T0 = mem.ReadU32((c.V1 + 0x328u));
        c.T1 = mem.ReadU32((c.V1 + 0x304u));
        c.V0 = 0x00000004u;
        mem.WriteU32((c.S0 + 0x80u), c.V0);
        mem.WriteU32((c.S0 + 0x88u), c.A1);
        c.A1 = c.S3 & 0x00FFu;
        mem.WriteU32(c.S0, c.A0);
        mem.WriteU32((c.S0 + 0x8Cu), c.A2);
        mem.WriteU32((c.S0 + 0x90u), c.A3);
        mem.WriteU32((c.S0 + 0x94u), c.T0);
        c.V0 = mem.ReadU32((c.T1 + 0x8u));
        c.A2 = c.A1 << 2;
        mem.WriteU32((c.S0 + 0x10u), c.V0);
        c.V1 = mem.ReadU32((c.V1 + 0x304u));
        c.V0 = 0x80160000u;
        c.V1 = mem.ReadU32((c.V1 + 0x4u));
        c.V0 = c.V0 + 0x7BCCu;
        mem.WriteU32((c.S0 + 0x40u), c.V0);
        c.V0 = 0x801A0000u;
        c.A3 = c.V0 - 0x70D8u;
        c.V0 = c.A2 + c.A3;
        mem.WriteU16((c.S0 + 0x98u), (ushort)0u);
        mem.WriteU32((c.S0 + 0x14u), c.V1);
        c.V0 = mem.ReadU32((c.V0 + 0xF8u));
        c.V1 = mem.ReadU32((c.GP + 0x28u));
        c.V0 = mem.ReadU16((c.V0 + 0x6u));
        if (c.V0 != 0u) {
            mem.WriteU32((c.S0 + 0x60u), c.V1);
            goto L80021F50;
        }
        mem.WriteU32((c.S0 + 0x60u), c.V1);
        c.V0 = c.S0 + 0xA8u;
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
        c.T4 = mem.ReadU32((c.V0 + 0x14u));
        c.T5 = mem.ReadU32((c.V0 + 0x18u));
        RecompOne.Runtime.Gte.WriteControl(5, c.T4);
        c.T6 = mem.ReadU32((c.V0 + 0x1Cu));
        RecompOne.Runtime.Gte.WriteControl(6, c.T5);
        RecompOne.Runtime.Gte.WriteControl(7, c.T6);
        c.V0 = c.A2 + c.T2;
        c.V1 = 0u | 0x83CCu;
        c.V0 = c.V0 + c.V1;
        c.V0 = mem.ReadU32(c.V0);
        c.A0 = c.A3 + 0x108u;
        c.V1 = mem.ReadU32((c.V0 + 0x8u));
        c.A0 = c.A2 + c.A0;
        c.V0 = c.V0 + c.V1;
        mem.WriteU32((c.S0 + 0xCu), c.V0);
        c.V0 = c.V0 + 0xCu;
        mem.WriteU32((c.S0 + 0x20u), c.V0);
        c.V0 = c.A1 + c.A3;
        c.A2 = mem.ReadU8((c.V0 + 0x150u));
        c.V0 = c.A1 << 1;
        c.V0 = c.V0 + c.A3;
        c.A3 = (uint)(short)mem.ReadU16((c.V0 + 0x130u));
        c.A1 = c.A1 + 0x20u;
        c.RA = 0x80021E3Cu;
        ShadowTower_st.func_80034604(c, mem);
        if (c.V0 == 0u) {
            c.V1 = 0x80160000u;
            goto L80021FA4;
        }
        c.V1 = 0x80160000u;
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
            goto L80021F3C;
        }
        c.T2 = 0x00000FE1u;
        c.T1 = c.V0;
        c.A1 = c.A3 + 0x6u;
        L80021E80: ;
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
            goto L80021EF8;
        }
        c.V1 = c.V1 - c.A0;
        c.V0 = (uint)(short)mem.ReadU16((c.A1 - 0x2u));
        c.V0 = c.V0 - c.A0;
        c.V0 = c.V0 << 12;
        if (c.V1 != 0u) { if ((int)c.V0 == int.MinValue && (int)c.V1 == -1) { c.LO = 0x80000000u; c.HI = 0u; } else { c.LO = (uint)((int)c.V0 / (int)c.V1); c.HI = (uint)((int)c.V0 % (int)c.V1); } }
        if (c.V1 != 0u) {
            goto L80021ED4;
        }
        Bios.Break(c, mem);
        L80021ED4: ;
        c.At = 0xFFFFFFFFu;
        if (c.V1 != c.At) {
            c.At = 0x80000000u;
            goto L80021EEC;
        }
        c.At = 0x80000000u;
        if (c.V0 != c.At) {
            goto L80021EEC;
        }
        Bios.Break(c, mem);
        L80021EEC: ;
        c.V0 = c.LO;
        mem.WriteU16(c.A1, (ushort)c.V0);
        goto L80021EFC;
        L80021EF8: ;
        mem.WriteU16(c.A1, (ushort)0u);
        L80021EFC: ;
        c.V0 = (uint)(short)mem.ReadU16(c.A1);
        c.V0 = (int)c.V0 < 4066 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L80021F14;
        }
        mem.WriteU16(c.A1, (ushort)c.T2);
        L80021F14: ;
        c.V0 = (uint)(short)mem.ReadU16(c.A1);
        if ((int)c.V0 >= 0) {
            goto L80021F28;
        }
        mem.WriteU16(c.A1, (ushort)0u);
        L80021F28: ;
        c.A1 = c.A1 + 0x8u;
        c.A3 = c.A3 + 0x8u;
        c.A2 = c.A2 - 0x1u;
        if (c.A2 != c.T1) {
            c.T0 = c.T0 + 0x8u;
            goto L80021E80;
        }
        c.T0 = c.T0 + 0x8u;
        L80021F3C: ;
        c.A0 = 0u;
        c.A1 = 0x00000080u;
        c.RA = 0x80021F48u;
        ShadowTower_st.func_80019DC8(c, mem);
        c.V1 = 0x80160000u;
        goto L80021FA4;
        L80021F50: ;
        c.V0 = c.A2 + c.T2;
        c.V1 = 0u | 0x83CCu;
        c.V0 = c.V0 + c.V1;
        c.V0 = mem.ReadU32(c.V0);
        c.A0 = c.A3 + 0x108u;
        c.V1 = mem.ReadU32((c.V0 + 0x8u));
        c.A0 = c.A2 + c.A0;
        c.V0 = c.V0 + c.V1;
        mem.WriteU32((c.S0 + 0xCu), c.V0);
        c.V0 = c.A1 + c.A3;
        c.A2 = mem.ReadU8((c.V0 + 0x150u));
        c.V0 = c.A1 << 1;
        c.V0 = c.V0 + c.A3;
        c.A3 = (uint)(short)mem.ReadU16((c.V0 + 0x130u));
        c.A1 = c.A1 + 0x20u;
        c.RA = 0x80021F90u;
        ShadowTower_st.func_80035AA8(c, mem);
        if (c.V0 == 0u) {
            c.A0 = 0x00000080u;
            goto L80021FA0;
        }
        c.A0 = 0x00000080u;
        c.A1 = 0u;
        c.RA = 0x80021FA0u;
        ShadowTower_st.func_80022010(c, mem);
        L80021FA0: ;
        c.V1 = 0x80160000u;
        L80021FA4: ;
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
        c.A1 = mem.ReadU32((c.S0 + 0x94u));
        c.A2 = mem.ReadU32((c.S0 + 0x90u));
        mem.WriteU32((c.V1 + 0x31Cu), c.V0);
        mem.WriteU32((c.V1 + 0x320u), c.A0);
        mem.WriteU32((c.V1 + 0x328u), c.A1);
        mem.WriteU32((c.V1 + 0x324u), c.A2);
        L80021FF0: ;
        c.RA = mem.ReadU32((c.SP + 0x48u));
        c.S3 = mem.ReadU32((c.SP + 0x44u));
        c.S2 = mem.ReadU32((c.SP + 0x40u));
        c.S1 = mem.ReadU32((c.SP + 0x3Cu));
        c.S0 = mem.ReadU32((c.SP + 0x38u));
        c.SP = c.SP + 0x50u;
        return;
    }
}
