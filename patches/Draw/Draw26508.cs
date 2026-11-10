using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_80026508</c>: a draw-stage call when 0x801D2D23 is set and 0x801B5929 is under 0x2C; not read. <c>ST_DRAW26508</c>.
/// </summary>
public static class Draw26508
{
    public static readonly Literal R = new(typeof(Draw26508), "draw-26508", "DRAW26508", 0x80026508, Run,
        "a draw-stage call when 0x801D2D23 is set and 0x801B5929 is under 0x2C; not read.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x18u;
        c.V0 = 0x801A0000u;
        c.A0 = c.V0 - 0x70D8u;
        mem.WriteU32((c.SP + 0x14u), c.RA);
        mem.WriteU32((c.SP + 0x10u), c.S0);
        c.A1 = mem.ReadU16((c.A0 + 0x2u));
        c.V0 = mem.ReadU16((c.A0 + 0x18Au));
        if (c.A1 != c.V0) {
            goto L80026544;
        }
        c.V1 = mem.ReadU16((c.A0 + 0x6u));
        c.V0 = mem.ReadU16((c.A0 + 0x18Cu));
        if (c.V1 == c.V0) {
            c.V0 = 0x801A0000u;
            goto L8002655C;
        }
        c.V0 = 0x801A0000u;
        L80026544: ;
        c.V1 = mem.ReadU16((c.A0 + 0x6u));
        c.V0 = 0x00000002u;
        mem.WriteU8((c.A0 + 0x186u), (byte)c.V0);
        mem.WriteU16((c.A0 + 0x18Au), (ushort)c.A1);
        mem.WriteU16((c.A0 + 0x18Cu), (ushort)c.V1);
        c.V0 = 0x801A0000u;
        L8002655C: ;
        c.S0 = c.V0 - 0x70D8u;
        c.V1 = mem.ReadU8((c.S0 + 0x186u));
        if (c.V1 == 0u) {
            c.V0 = 0x00000002u;
            goto L800265E8;
        }
        c.V0 = 0x00000002u;
        if (c.V1 != c.V0) {
            c.V0 = 0x801A0000u;
            goto L800265EC;
        }
        c.V0 = 0x801A0000u;
        c.A1 = 0x0000000Eu;
        c.A2 = 0x00000010u;
        c.T0 = 0x80080000u;
        c.A3 = c.T0 - 0xC0Cu;
        c.A0 = mem.ReadU16((c.S0 + 0x2u));
        c.V0 = mem.ReadU8((c.T0 - 0xC0Cu));
        c.V1 = 0x000000FFu;
        mem.WriteU8((c.A3 + 0x1CCu), (byte)c.V1);
        c.V0 = c.V0 | 0x0001u;
        mem.WriteU8((c.T0 - 0xC0Cu), (byte)c.V0);
        c.V0 = mem.ReadU8((c.A3 + 0x14u));
        c.V1 = mem.ReadU8((c.A3 + 0x28u));
        c.V0 = c.V0 | 0x0001u;
        mem.WriteU8((c.A3 + 0x14u), (byte)c.V0);
        c.V0 = mem.ReadU8((c.A3 + 0x3Cu));
        c.V1 = c.V1 | 0x0002u;
        mem.WriteU8((c.A3 + 0x28u), (byte)c.V1);
        c.V1 = mem.ReadU8((c.A3 + 0x104u));
        c.V0 = c.V0 | 0x0002u;
        c.V1 = c.V1 | 0x0001u;
        mem.WriteU8((c.A3 + 0x3Cu), (byte)c.V0);
        mem.WriteU8((c.A3 + 0x104u), (byte)c.V1);
        c.RA = 0x800265D4u;
        ShadowTower_st.func_80026228(c, mem);
        c.A1 = 0x00000013u;
        c.A0 = mem.ReadU16((c.S0 + 0x6u));
        c.A2 = 0x00000010u;
        c.RA = 0x800265E4u;
        ShadowTower_st.func_80026228(c, mem);
        mem.WriteU8((c.S0 + 0x186u), (byte)0u);
        L800265E8: ;
        c.V0 = 0x801A0000u;
        L800265EC: ;
        c.A3 = c.V0 - 0x70D8u;
        c.V0 = mem.ReadU8((c.A3 + 0x186u));
        if (c.V0 != 0u) {
            c.A1 = 0x66660000u;
            goto L8002666C;
        }
        c.A1 = 0x66660000u;
        c.V0 = mem.ReadU16((c.A3 + 0x17Eu));
        c.A1 = c.A1 | 0x6667u;
        c.A0 = c.V0 << 3;
        c.A0 = c.A0 - c.V0;
        c.A0 = c.A0 << 4;
        c.LO = (uint)((long)(int)c.A0 * (int)c.A1); c.HI = (uint)(((long)(int)c.A0 * (int)c.A1) >> 32);
        c.V0 = mem.ReadU16((c.A3 + 0x182u));
        c.V1 = c.V0 << 3;
        c.A2 = c.HI;
        c.V1 = c.V1 - c.V0;
        c.V1 = c.V1 << 4;
        c.LO = (uint)((long)(int)c.V1 * (int)c.A1); c.HI = (uint)(((long)(int)c.V1 * (int)c.A1) >> 32);
        c.A0 = (uint)((int)c.A0 >> 31);
        c.A1 = 0x80080000u;
        c.A1 = c.A1 - 0xC0Cu;
        c.V0 = (uint)((int)c.A2 >> 11);
        c.V0 = c.V0 - c.A0;
        c.V1 = (uint)((int)c.V1 >> 31);
        mem.WriteU16((c.A1 + 0xAu), (ushort)c.V0);
        c.V0 = 0x00000133u;
        c.T0 = c.HI;
        c.A0 = (uint)((int)c.T0 >> 11);
        c.A0 = c.A0 - c.V1;
        c.V0 = c.V0 - c.A0;
        mem.WriteU16((c.A1 + 0x1Eu), (ushort)c.A0);
        mem.WriteU16((c.A1 + 0x1Au), (ushort)c.V0);
        L8002666C: ;
        c.V1 = mem.ReadU32((c.A3 + 0x2Cu));
        c.V0 = c.V1 & 0x0007u;
        if (c.V0 == 0u) {
            c.V0 = 0x80080000u;
            goto L800266B4;
        }
        c.V0 = 0x80080000u;
        c.A0 = c.V0 - 0xC0Cu;
        c.V0 = 0x00000001u;
        mem.WriteU8((c.A0 + 0x50u), (byte)c.V0);
        c.V0 = c.V1 & 0x0004u;
        if (c.V0 == 0u) {
            c.V0 = 0x00000009u;
            goto L800266A0;
        }
        c.V0 = 0x00000009u;
        mem.WriteU8((c.A0 + 0x50u), (byte)c.V0);
        goto L800266BC;
        L800266A0: ;
        c.V0 = c.V1 & 0x0002u;
        if (c.V0 == 0u) {
            c.V0 = 0x00000005u;
            goto L800266BC;
        }
        c.V0 = 0x00000005u;
        mem.WriteU8((c.A0 + 0x50u), (byte)c.V0);
        goto L800266BC;
        L800266B4: ;
        c.V0 = 0x80080000u;
        mem.WriteU8((c.V0 - 0xBBCu), (byte)0u);
        L800266BC: ;
        c.V0 = 0x801A0000u;
        c.V1 = mem.ReadU32((c.V0 - 0x70ACu));
        c.V0 = c.V1 & 0x0018u;
        if (c.V0 == 0u) {
            c.V0 = 0x80080000u;
            goto L800266F4;
        }
        c.V0 = 0x80080000u;
        c.A0 = c.V0 - 0xC0Cu;
        c.V0 = 0x00000001u;
        mem.WriteU8((c.A0 + 0x8Cu), (byte)c.V0);
        c.V0 = c.V1 & 0x0010u;
        if (c.V0 == 0u) {
            c.V0 = 0x00000009u;
            goto L800266FC;
        }
        c.V0 = 0x00000009u;
        mem.WriteU8((c.A0 + 0x8Cu), (byte)c.V0);
        goto L800266FC;
        L800266F4: ;
        c.V0 = 0x80080000u;
        mem.WriteU8((c.V0 - 0xB80u), (byte)0u);
        L800266FC: ;
        c.V0 = 0x801A0000u;
        c.V1 = mem.ReadU32((c.V0 - 0x70ACu));
        c.V0 = c.V1 & 0x0060u;
        if (c.V0 == 0u) {
            c.V0 = 0x80080000u;
            goto L80026734;
        }
        c.V0 = 0x80080000u;
        c.A0 = c.V0 - 0xC0Cu;
        c.V0 = 0x00000001u;
        mem.WriteU8((c.A0 + 0xB4u), (byte)c.V0);
        c.V0 = c.V1 & 0x0040u;
        if (c.V0 == 0u) {
            c.V0 = 0x00000009u;
            goto L8002673C;
        }
        c.V0 = 0x00000009u;
        mem.WriteU8((c.A0 + 0xB4u), (byte)c.V0);
        goto L8002673C;
        L80026734: ;
        c.V0 = 0x80080000u;
        mem.WriteU8((c.V0 - 0xB58u), (byte)0u);
        L8002673C: ;
        c.V0 = 0x801A0000u;
        c.V0 = mem.ReadU32((c.V0 - 0x70ACu));
        c.V0 = c.V0 & 0x0080u;
        if (c.V0 == 0u) {
            c.V1 = 0x80080000u;
            goto L8002676C;
        }
        c.V1 = 0x80080000u;
        c.V1 = c.V1 - 0xC0Cu;
        c.V0 = mem.ReadU8((c.V1 + 0xDCu));
        c.V0 = c.V0 | 0x0001u;
        mem.WriteU8((c.V1 + 0xDCu), (byte)c.V0);
        goto L80026774;
        L8002676C: ;
        c.V0 = 0x80080000u;
        mem.WriteU8((c.V0 - 0xB30u), (byte)0u);
        L80026774: ;
        c.V0 = 0x801A0000u;
        c.V0 = mem.ReadU32((c.V0 - 0x70ACu));
        c.V0 = c.V0 & 0x0100u;
        if (c.V0 == 0u) {
            c.V1 = 0x80080000u;
            goto L800267A4;
        }
        c.V1 = 0x80080000u;
        c.V1 = c.V1 - 0xC0Cu;
        c.V0 = mem.ReadU8((c.V1 + 0xF0u));
        c.V0 = c.V0 | 0x0001u;
        mem.WriteU8((c.V1 + 0xF0u), (byte)c.V0);
        goto L800267AC;
        L800267A4: ;
        c.V0 = 0x80080000u;
        mem.WriteU8((c.V0 - 0xB1Cu), (byte)0u);
        L800267AC: ;
        c.V0 = 0x801A0000u;
        c.V0 = mem.ReadU32((c.V0 - 0x6F34u));
        if (c.V0 != 0u) {
            c.V1 = 0x80080000u;
            goto L800267CC;
        }
        c.V1 = 0x80080000u;
        c.V0 = 0x80080000u;
        mem.WriteU8((c.V0 - 0x928u), (byte)0u);
        goto L800267E0;
        L800267CC: ;
        c.V1 = c.V1 - 0xC0Cu;
        c.V0 = mem.ReadU8((c.V1 + 0x2E4u));
        c.V0 = c.V0 | 0x0001u;
        mem.WriteU8((c.V1 + 0x2E4u), (byte)c.V0);
        L800267E0: ;
        c.V0 = 0x801A0000u;
        c.V0 = mem.ReadU32((c.V0 - 0x6F18u));
        if (c.V0 != 0u) {
            c.V1 = 0x80080000u;
            goto L80026800;
        }
        c.V1 = 0x80080000u;
        c.V0 = 0x80080000u;
        mem.WriteU8((c.V0 - 0x914u), (byte)0u);
        goto L80026814;
        L80026800: ;
        c.V1 = c.V1 - 0xC0Cu;
        c.V0 = mem.ReadU8((c.V1 + 0x2F8u));
        c.V0 = c.V0 | 0x0001u;
        mem.WriteU8((c.V1 + 0x2F8u), (byte)c.V0);
        L80026814: ;
        c.V0 = 0x801A0000u;
        c.V0 = mem.ReadU32((c.V0 - 0x6F2Cu));
        if (c.V0 != 0u) {
            c.V1 = 0x80080000u;
            goto L80026834;
        }
        c.V1 = 0x80080000u;
        c.V0 = 0x80080000u;
        mem.WriteU8((c.V0 - 0x900u), (byte)0u);
        goto L80026848;
        L80026834: ;
        c.V1 = c.V1 - 0xC0Cu;
        c.V0 = mem.ReadU8((c.V1 + 0x30Cu));
        c.V0 = c.V0 | 0x0001u;
        mem.WriteU8((c.V1 + 0x30Cu), (byte)c.V0);
        L80026848: ;
        c.V0 = 0x801A0000u;
        c.V0 = mem.ReadU32((c.V0 - 0x6F28u));
        if (c.V0 != 0u) {
            c.V1 = 0x80080000u;
            goto L80026868;
        }
        c.V1 = 0x80080000u;
        c.V0 = 0x80080000u;
        mem.WriteU8((c.V0 - 0x8ECu), (byte)0u);
        goto L8002687C;
        L80026868: ;
        c.V1 = c.V1 - 0xC0Cu;
        c.V0 = mem.ReadU8((c.V1 + 0x320u));
        c.V0 = c.V0 | 0x0001u;
        mem.WriteU8((c.V1 + 0x320u), (byte)c.V0);
        L8002687C: ;
        c.V0 = 0x801A0000u;
        c.V0 = mem.ReadU32((c.V0 - 0x6F24u));
        if (c.V0 != 0u) {
            c.V1 = 0x80080000u;
            goto L8002689C;
        }
        c.V1 = 0x80080000u;
        c.V0 = 0x80080000u;
        mem.WriteU8((c.V0 - 0x8D8u), (byte)0u);
        goto L800268B0;
        L8002689C: ;
        c.V1 = c.V1 - 0xC0Cu;
        c.V0 = mem.ReadU8((c.V1 + 0x334u));
        c.V0 = c.V0 | 0x0001u;
        mem.WriteU8((c.V1 + 0x334u), (byte)c.V0);
        L800268B0: ;
        c.V0 = 0x801A0000u;
        c.V0 = mem.ReadU32((c.V0 - 0x6F20u));
        if (c.V0 != 0u) {
            c.V1 = 0x80080000u;
            goto L800268D0;
        }
        c.V1 = 0x80080000u;
        c.V0 = 0x80080000u;
        mem.WriteU8((c.V0 - 0x8C4u), (byte)0u);
        goto L800268E4;
        L800268D0: ;
        c.V1 = c.V1 - 0xC0Cu;
        c.V0 = mem.ReadU8((c.V1 + 0x348u));
        c.V0 = c.V0 | 0x0001u;
        mem.WriteU8((c.V1 + 0x348u), (byte)c.V0);
        L800268E4: ;
        c.V0 = 0x801A0000u;
        c.V0 = mem.ReadU32((c.V0 - 0x6F1Cu));
        if (c.V0 != 0u) {
            c.V1 = 0x80080000u;
            goto L80026904;
        }
        c.V1 = 0x80080000u;
        c.V0 = 0x80080000u;
        mem.WriteU8((c.V0 - 0x8B0u), (byte)0u);
        goto L80026918;
        L80026904: ;
        c.V1 = c.V1 - 0xC0Cu;
        c.V0 = mem.ReadU8((c.V1 + 0x35Cu));
        c.V0 = c.V0 | 0x0001u;
        mem.WriteU8((c.V1 + 0x35Cu), (byte)c.V0);
        L80026918: ;
        c.V0 = 0x801A0000u;
        c.V0 = mem.ReadU32((c.V0 - 0x70ACu));
        c.V1 = 0x01000000u;
        c.V0 = c.V0 & c.V1;
        if (c.V0 == 0u) {
            c.V1 = 0x80080000u;
            goto L80026948;
        }
        c.V1 = 0x80080000u;
        c.V1 = c.V1 - 0xC0Cu;
        c.V0 = mem.ReadU8((c.V1 + 0x370u));
        c.V0 = c.V0 | 0x0001u;
        mem.WriteU8((c.V1 + 0x370u), (byte)c.V0);
        goto L80026950;
        L80026948: ;
        c.V0 = 0x80080000u;
        mem.WriteU8((c.V0 - 0x89Cu), (byte)0u);
        L80026950: ;
        c.RA = mem.ReadU32((c.SP + 0x14u));
        c.S0 = mem.ReadU32((c.SP + 0x10u));
        c.SP = c.SP + 0x18u;
        return;
    }
}
