using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_80033110</c>: called from the draw stage when the byte at 0x80199140 is 0, 5 or 6; not read. <c>ST_DRAW33110</c>.
/// </summary>
public static partial class Draw33110
{
    public static readonly Literal R = new(typeof(Draw33110), "draw-33110", "DRAW33110", 0x80033110, Run,
        "called from the draw stage when the byte at 0x80199140 is 0, 5 or 6; not read.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        AfterEntry(c, mem);
        if (c.V1 != 0u) {
            c.V1 = 0x80190000u;
            goto L8003317C;
        }
        c.V1 = 0x80190000u;
        c.V1 = c.V0 & 0x00FFu;
        c.V0 = 0x0000000Eu;
        if (c.V1 == c.V0) {
            c.V0 = 0x00000017u;
            goto L80033178;
        }
        c.V0 = 0x00000017u;
        if (c.V1 != c.V0) {
            c.V0 = 0x801A0000u;
            goto L80033194;
        }
        c.V0 = 0x801A0000u;
        L80033178: ;
        c.V1 = 0x80190000u;
        L8003317C: ;
        c.V1 = c.V1 - 0x4930u;
        c.V0 = mem.ReadU8((c.V1 + 0x4u));
        mem.WriteU8((c.V1 + 0x9u), (byte)0u);
        c.V0 = c.V0 & 0x00DFu;
        mem.WriteU8((c.V1 + 0x4u), (byte)c.V0);
        c.V0 = 0x801A0000u;
        L80033194: ;
        c.V0 = (uint)(sbyte)mem.ReadU8((c.V0 - 0x6F11u));
        if ((int)c.V0 >= 0) {
            goto L800331C0;
        }
        c.V0 = 0x80190000u;
        c.V0 = c.V0 - 0x4930u;
        c.V1 = mem.ReadU8((c.V0 + 0x3u));
        c.A0 = mem.ReadU8((c.V0 + 0x4u));
        c.V1 = c.V1 | 0x0001u;
        c.A0 = c.A0 | 0x0004u;
        goto L800331DC;
        L800331C0: ;
        if ((int)c.V0 <= 0) {
            c.V0 = 0x80190000u;
            goto L800331E4;
        }
        c.V0 = 0x80190000u;
        c.V0 = c.V0 - 0x4930u;
        c.V1 = mem.ReadU8((c.V0 + 0x3u));
        c.A0 = mem.ReadU8((c.V0 + 0x4u));
        c.V1 = c.V1 | 0x0001u;
        c.A0 = c.A0 | 0x0010u;
        L800331DC: ;
        mem.WriteU8((c.V0 + 0x3u), (byte)c.V1);
        mem.WriteU8((c.V0 + 0x4u), (byte)c.A0);
        L800331E4: ;
        c.V0 = 0x80190000u;
        c.A0 = c.V0 - 0x4930u;
        c.V0 = mem.ReadU8((c.A0 + 0xAu));
        c.A2 = 0x00000001u;
        if (c.V0 == c.A2) {
            c.A3 = 0x00000002u;
            goto L800339A8;
        }
        c.A3 = 0x00000002u;
        if (c.V0 != c.A3) {
            c.V0 = 0x801B0000u;
            goto L8003320C;
        }
        c.V0 = 0x801B0000u;
        mem.WriteU8((c.A0 + 0xAu), (byte)0u);
        goto L800339A8;
        L8003320C: ;
        c.A1 = mem.ReadU8((c.V0 + 0x5929u));
        c.V0 = mem.ReadU8((c.A0 + 0x7u));
        if (c.A1 == c.V0) {
            c.V1 = 0x00010000u;
            goto L8003323C;
        }
        c.V1 = 0x00010000u;
        c.V1 = c.V1 | 0x8000u;
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.V0 = c.V0 + c.V1;
        mem.WriteU8((c.A0 + 0x8u), (byte)c.A2);
        mem.WriteU8((c.A0 + 0x7u), (byte)c.A1);
        mem.WriteU8((c.V0 + 0x6DFEu), (byte)0u);
        L8003323C: ;
        c.V0 = mem.ReadU8((c.A0 + 0x8u));
        if (c.V0 != c.A3) {
            c.V0 = 0x80190000u;
            goto L80033268;
        }
        c.V0 = 0x80190000u;
        c.V0 = 0x801A0000u;
        c.V1 = mem.ReadU8((c.V0 - 0x6EC0u));
        c.V0 = 0x00000006u;
        if (c.V1 != c.V0) {
            c.V0 = 0x80190000u;
            goto L80033268;
        }
        c.V0 = 0x80190000u;
        mem.WriteU8((c.A0 + 0x3u), (byte)0u);
        mem.WriteU8((c.A0 + 0x4u), (byte)0u);
        L80033268: ;
        c.A0 = c.V0 - 0x4930u;
        c.V0 = mem.ReadU8((c.A0 + 0x9u));
        if (c.V0 == 0u) {
            c.V0 = 0x801B0000u;
            goto L80033288;
        }
        c.V0 = 0x801B0000u;
        c.S2 = 0x00000001u;
        mem.WriteU8((c.A0 + 0x9u), (byte)0u);
        goto L800332D8;
        L80033288: ;
        c.V0 = mem.ReadU8((c.V0 + 0x5929u));
        c.V0 = c.V0 << 1;
        c.V0 = c.V0 + c.A0;
        c.V1 = (uint)(short)mem.ReadU16((c.V0 + 0xCu));
        c.V0 = 0x00000011u;
        if (c.V1 != c.V0) {
            goto L800332B0;
        }
        c.S2 = 0x00000001u;
        goto L800332D8;
        L800332B0: ;
        c.V0 = mem.ReadU8((c.A0 + 0x3u));
        c.V1 = c.S2 | c.V0;
        c.V0 = c.V1 & 0x0004u;
        if (c.V0 != 0u) {
            c.S2 = 0x00000004u;
            goto L800332D8;
        }
        c.S2 = 0x00000004u;
        c.V0 = c.V1 & 0x0002u;
        if (c.V0 == 0u) {
            c.S2 = 0x00000001u;
            goto L800332D8;
        }
        c.S2 = 0x00000001u;
        c.S2 = 0x00000002u;
        L800332D8: ;
        c.V0 = 0x80190000u;
        c.A0 = c.V0 - 0x4930u;
        c.V1 = mem.ReadU8((c.A0 + 0x4u));
        if (c.V1 == 0u) {
            c.V0 = c.V1 & 0x00E0u;
            goto L8003332C;
        }
        c.V0 = c.V1 & 0x00E0u;
        if (c.V0 != 0u) {
            c.V0 = c.V1 & 0x000Fu;
            goto L80033328;
        }
        c.V0 = c.V1 & 0x000Fu;
        if (c.V0 != 0u) {
            c.S1 = 0x00000040u;
            goto L8003332C;
        }
        c.S1 = 0x00000040u;
        c.V0 = 0x801B0000u;
        c.V0 = mem.ReadU8((c.V0 + 0x5929u));
        c.V0 = c.V0 << 1;
        c.V0 = c.V0 + c.A0;
        c.V0 = mem.ReadU16((c.V0 + 0xCu));
        c.V0 = c.V0 & 0x0010u;
        if (c.V0 == 0u) {
            c.S1 = 0x00000020u;
            goto L8003332C;
        }
        c.S1 = 0x00000020u;
        L80033328: ;
        c.S1 = 0x00000010u;
        L8003332C: ;
        c.V0 = 0x80190000u;
        c.S0 = c.V0 - 0x4930u;
        c.V0 = mem.ReadU8((c.S0 + 0x5u));
        c.A1 = c.S2 & 0x00FFu;
        if (c.A1 != c.V0) {
            goto L80033354;
        }
        c.V0 = mem.ReadU8((c.S0 + 0x6u));
        if (c.S1 == c.V0) {
            c.V0 = 0x80190000u;
            goto L80033390;
        }
        c.V0 = 0x80190000u;
        L80033354: ;
        c.V1 = mem.ReadU8((c.S0 + 0x8u));
        c.V0 = 0x00000002u;
        if (c.V1 != c.V0) {
            c.A0 = c.S1;
            goto L8003336C;
        }
        c.A0 = c.S1;
        c.A2 = 0u;
        goto L8003337C;
        L8003336C: ;
        if (c.V1 != 0u) {
            c.V0 = 0x80190000u;
            goto L80033390;
        }
        c.V0 = 0x80190000u;
        c.A0 = c.S1;
        c.A2 = 0x00000001u;
        L8003337C: ;
        c.RA = 0x80033384u;
        ShadowTower_st.func_80032800(c, mem);
        mem.WriteU8((c.S0 + 0x5u), (byte)c.S2);
        mem.WriteU8((c.S0 + 0x6u), (byte)c.S1);
        c.V0 = 0x80190000u;
        L80033390: ;
        c.A0 = c.V0 - 0x4930u;
        c.V1 = mem.ReadU8((c.A0 + 0x8u));
        c.V0 = 0x00000001u;
        if (c.V1 != c.V0) {
            c.V0 = 0x80190000u;
            goto L800333D4;
        }
        c.V0 = 0x80190000u;
        c.V0 = 0x801B0000u;
        c.V0 = c.V0 + 0x5920u;
        c.V0 = mem.ReadU16((c.V0 + 0x2u));
        c.V0 = c.V0 << 16;
        c.V0 = (uint)((int)c.V0 >> 16);
        c.V0 = (int)c.V0 < 7 ? 1u : 0u;
        if (c.V0 != 0u) {
            c.V0 = 0x80190000u;
            goto L800333D4;
        }
        c.V0 = 0x80190000u;
        c.V0 = 0x00000002u;
        mem.WriteU8((c.A0 + 0x8u), (byte)c.V0);
        goto L800333EC;
        L800333D4: ;
        c.A0 = c.V0 - 0x4930u;
        c.V1 = mem.ReadU8((c.A0 + 0x8u));
        c.V0 = 0x00000002u;
        if (c.V1 != c.V0) {
            c.V1 = 0x00010000u;
            goto L800333F0;
        }
        c.V1 = 0x00010000u;
        mem.WriteU8((c.A0 + 0x8u), (byte)0u);
        L800333EC: ;
        c.V1 = 0x00010000u;
        L800333F0: ;
        c.V1 = c.V1 | 0x8000u;
        c.A0 = 0x80190000u;
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.A1 = c.V0 + c.V1;
        c.V1 = mem.ReadU8((c.A1 + 0x6DFEu));
        c.A0 = c.A0 - 0x4930u;
        mem.WriteU8((c.A0 + 0x4u), (byte)0u);
        c.V0 = c.V1 & 0x0080u;
        if (c.V0 == 0u) {
            mem.WriteU8((c.A0 + 0x3u), (byte)0u);
            goto L80033894;
        }
        mem.WriteU8((c.A0 + 0x3u), (byte)0u);
        c.A2 = c.V1 & 0x007Fu;
        c.V0 = c.A2 < 0x00000040u ? 1u : 0u;
        if (c.V0 != 0u) {
            c.V0 = 0x00010000u;
            goto L80033524;
        }
        After800333F0(c, mem);
        if (c.V0 == 0u) {
            mem.WriteU8((c.A1 + 0x6DFCu), (byte)c.V1);
            goto L800334C0;
        }
        mem.WriteU8((c.A1 + 0x6DFCu), (byte)c.V1);
        c.T1 = c.A0 + 0xCCu;
        L8003346C: ;
        Poll(c, mem);
        After8003346C(c, mem);
        if (c.A3 != c.T1) {
            c.T0 = c.T0 + 0x10u;
            goto L8003346C;
        }
        c.T0 = c.T0 + 0x10u;
        goto L800334F0;
        L800334C0: ;
        c.T1 = c.A0 + 0xCCu;
        L800334C4: ;
        Poll(c, mem);
        c.V0 = mem.ReadU32(c.A3);
        c.V1 = mem.ReadU32((c.A3 + 0x4u));
        c.A0 = mem.ReadU32((c.A3 + 0x8u));
        c.A1 = mem.ReadU32((c.A3 + 0xCu));
        mem.WriteU32(c.T0, c.V0);
        mem.WriteU32((c.T0 + 0x4u), c.V1);
        mem.WriteU32((c.T0 + 0x8u), c.A0);
        mem.WriteU32((c.T0 + 0xCu), c.A1);
        c.A3 = c.A3 + 0x10u;
        if (c.A3 != c.T1) {
            c.T0 = c.T0 + 0x10u;
            goto L800334C4;
        }
        c.T0 = c.T0 + 0x10u;
        L800334F0: ;
        After800334F0(c, mem);
        L80033524: ;
        After80033524(c, mem);
        goto L80033978;
        L80033894: ;
        c.V0 = mem.ReadU8((c.A1 + 0x6DFDu));
        c.A3 = mem.ReadU32((c.A1 + 0x6E08u));
        c.V1 = c.V0 << 3;
        c.V1 = c.V1 - c.V0;
        c.V1 = c.V1 << 2;
        c.V1 = c.V1 - c.V0;
        c.V1 = c.V1 << 2;
        c.V1 = c.V1 + c.A0;
        c.A2 = c.V1 + 0x6Cu;
        c.V0 = c.A2 | c.A3;
        c.V0 = c.V0 & 0x0003u;
        if (c.V0 == 0u) {
            c.T0 = c.V1 + 0xCCu;
            goto L8003391C;
        }
        c.T0 = c.V1 + 0xCCu;
        L800338C8: ;
        Poll(c, mem);
        After800338C8(c, mem);
        if (c.A2 != c.T0) {
            c.A3 = c.A3 + 0x10u;
            goto L800338C8;
        }
        c.A3 = c.A3 + 0x10u;
        goto L80033948;
        L8003391C: ;
        Poll(c, mem);
        c.V0 = mem.ReadU32(c.A2);
        c.V1 = mem.ReadU32((c.A2 + 0x4u));
        c.A0 = mem.ReadU32((c.A2 + 0x8u));
        c.A1 = mem.ReadU32((c.A2 + 0xCu));
        mem.WriteU32(c.A3, c.V0);
        mem.WriteU32((c.A3 + 0x4u), c.V1);
        mem.WriteU32((c.A3 + 0x8u), c.A0);
        mem.WriteU32((c.A3 + 0xCu), c.A1);
        c.A2 = c.A2 + 0x10u;
        if (c.A2 != c.T0) {
            c.A3 = c.A3 + 0x10u;
            goto L8003391C;
        }
        c.A3 = c.A3 + 0x10u;
        L80033948: ;
        After80033948(c, mem);
        L80033978: ;
        c.V1 = 0x00010000u;
        c.V1 = c.V1 | 0x8000u;
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.V0 = c.V0 + c.V1;
        c.V1 = mem.ReadU32((c.V0 + 0x6E08u));
        c.V1 = (uint)(short)mem.ReadU16((c.V1 + 0x68u));
        c.A0 = 0x00001000u;
        mem.WriteU32((c.V0 + 0x6E04u), c.A0);
        c.V1 = c.V1 - 0x400u;
        mem.WriteU32((c.V0 + 0x6E00u), c.V1);
        L800339A8: ;
        c.RA = mem.ReadU32((c.SP + 0x1Cu));
        c.S2 = mem.ReadU32((c.SP + 0x18u));
        c.S1 = mem.ReadU32((c.SP + 0x14u));
        c.S0 = mem.ReadU32((c.SP + 0x10u));
        c.SP = c.SP + 0x20u;
        return;
    }
}
