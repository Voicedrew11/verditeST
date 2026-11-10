using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_8001CDF8</c>: the scene's fourth preparation pass; writes no packet. <c>ST_SCENEPREPARE4</c>.
/// </summary>
public static class ScenePrepare4
{
    public static readonly Literal R = new(typeof(ScenePrepare4), "scene preparation 4", "SCENEPREPARE4", 0x8001CDF8, Run,
        "the scene's fourth preparation pass; writes no packet.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x10u;
        mem.WriteU32((c.SP + 0x8u), c.S2);
        c.S2 = c.A0;
        c.V0 = 0x801B0000u;
        c.V1 = c.V0 + 0x5948u;
        mem.WriteU32((c.SP + 0xCu), c.S3);
        mem.WriteU32((c.SP + 0x4u), c.S1);
        mem.WriteU32(c.SP, c.S0);
        c.T2 = mem.ReadU16((c.V1 + 0x44CCu));
        c.V0 = 0xFFFFFFFFu;
        c.T2 = c.T2 - 0x1u;
        if (c.T2 == c.V0) {
            c.T0 = 0x1F800000u;
            goto L8001D208;
        }
        c.T0 = 0x1F800000u;
        c.S1 = c.V1;
        c.V0 = 0x80180000u;
        c.S0 = c.V0 - 0x4130u;
        c.V0 = 0x80180000u;
        c.T9 = c.V0 - 0x4248u;
        c.T7 = 0xFFFFFFFFu;
        c.T8 = 0x00000FE1u;
        L8001CE48: ;
        Poll(c, mem);
        c.V0 = c.T2 << 1;
        c.V0 = c.V0 + c.S1;
        c.V0 = mem.ReadU16((c.V0 + 0x45CEu));
        c.V1 = c.V0 << 1;
        c.V1 = c.V1 + c.V0;
        c.V1 = c.V1 << 2;
        c.V1 = c.V1 + c.S0;
        c.V0 = mem.ReadU8((c.V1 + 0x3u));
        c.V0 = c.V0 & 0x0003u;
        mem.WriteU32((c.T0 + 0x1E4u), c.V0);
        c.V0 = c.V0 << 5;
        c.V0 = c.V0 + c.T9;
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
        c.V0 = c.T2 << 4;
        c.V0 = c.S2 + c.V0;
        c.T4 = mem.ReadU32(c.V0);
        c.T5 = mem.ReadU32((c.V0 + 0x4u));
        c.T6 = mem.ReadU32((c.V0 + 0x8u));
        RecompOne.Runtime.Gte.WriteControl(5, c.T4);
        RecompOne.Runtime.Gte.WriteControl(6, c.T5);
        RecompOne.Runtime.Gte.WriteControl(7, c.T6);
        c.V1 = mem.ReadU8(c.V1);
        c.V0 = c.V1 << 2;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 3;
        c.V1 = mem.ReadU32((c.T0 + 0xCu));
        c.V0 = c.V0 + 0xCu;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32((c.T0 + 0x20u), c.V1);
        c.V0 = mem.ReadU32(c.V1);
        c.V1 = mem.ReadU32((c.T0 + 0xCu));
        c.A0 = mem.ReadU32((c.T0 + 0x20u));
        c.V0 = c.V0 + 0xCu;
        c.V1 = c.V1 + c.V0;
        c.V0 = mem.ReadU32((c.T0 + 0x34u));
        mem.WriteU32((c.T0 + 0x3Cu), c.V1);
        mem.WriteU32((c.T0 + 0x40u), c.V1);
        mem.WriteU32((c.T0 + 0x38u), c.V0);
        c.V1 = mem.ReadU32((c.A0 + 0x4u));
        c.V0 = 0xAAAA0000u;
        c.V0 = c.V0 | 0xAAABu;
        c.LO = (uint)((ulong)c.V1 * c.V0); c.HI = (uint)(((ulong)c.V1 * c.V0) >> 32);
        c.A3 = mem.ReadU32((c.T0 + 0x34u));
        c.T1 = mem.ReadU32((c.T0 + 0x3Cu));
        c.S3 = c.HI;
        c.A2 = c.S3 >> 1;
        c.A2 = c.A2 - 0x1u;
        if (c.A2 == c.T7) {
            goto L8001D0D8;
        }
        c.A1 = c.A3 + 0x16u;
        L8001CF40: ;
        Poll(c, mem);
        RecompOne.Runtime.Gte.Write(0, (mem.ReadU32(c.T1)));
        RecompOne.Runtime.Gte.Write(1, (mem.ReadU32((c.T1 + 0x4u))));
        RecompOne.Runtime.Gte.Write(2, (mem.ReadU32((c.T1 + 0x8u))));
        RecompOne.Runtime.Gte.Write(3, (mem.ReadU32((c.T1 + 0xCu))));
        RecompOne.Runtime.Gte.Write(4, (mem.ReadU32((c.T1 + 0x10u))));
        RecompOne.Runtime.Gte.Write(5, (mem.ReadU32((c.T1 + 0x14u))));
        RecompOne.Runtime.Gte.Rtpt(12, false);
        c.V1 = c.A3 + 0x8u;
        c.V0 = c.A3 + 0x10u;
        mem.WriteU32(c.A3, (RecompOne.Runtime.Gte.Read(12)));
        mem.WriteU32(c.V1, (RecompOne.Runtime.Gte.Read(13)));
        mem.WriteU32(c.V0, (RecompOne.Runtime.Gte.Read(14)));
        c.A0 = c.A3 + 0x4u;
        c.V1 = c.A3 + 0xCu;
        c.V0 = c.A3 + 0x14u;
        mem.WriteU32(c.A0, (RecompOne.Runtime.Gte.Read(17)));
        mem.WriteU32(c.V1, (RecompOne.Runtime.Gte.Read(18)));
        mem.WriteU32(c.V0, (RecompOne.Runtime.Gte.Read(19)));
        c.A0 = mem.ReadU32((c.T0 + 0x64u));
        c.V0 = mem.ReadU32((c.T0 + 0x68u));
        if (c.A0 == c.V0) {
            c.V0 = c.V0 - c.A0;
            goto L8001D07C;
        }
        c.V0 = c.V0 - c.A0;
        c.V1 = (uint)(short)mem.ReadU16((c.A1 - 0x12u));
        c.V1 = c.V1 - c.A0;
        c.V1 = c.V1 << 12;
        if (c.V0 != 0u) { if ((int)c.V1 == int.MinValue && (int)c.V0 == -1) { c.LO = 0x80000000u; c.HI = 0u; } else { c.LO = (uint)((int)c.V1 / (int)c.V0); c.HI = (uint)((int)c.V1 % (int)c.V0); } }
        if (c.V0 != 0u) {
            goto L8001CFC4;
        }
        Bios.Break(c, mem);
        L8001CFC4: ;
        c.At = 0xFFFFFFFFu;
        if (c.V0 != c.At) {
            c.At = 0x80000000u;
            goto L8001CFDC;
        }
        c.At = 0x80000000u;
        if (c.V1 != c.At) {
            goto L8001CFDC;
        }
        Bios.Break(c, mem);
        L8001CFDC: ;
        c.V1 = c.LO;
        c.V0 = (uint)(short)mem.ReadU16((c.A1 - 0xAu));
        mem.WriteU16((c.A1 - 0x10u), (ushort)c.V1);
        c.A0 = mem.ReadU32((c.T0 + 0x64u));
        c.V1 = mem.ReadU32((c.T0 + 0x68u));
        c.V0 = c.V0 - c.A0;
        c.V0 = c.V0 << 12;
        c.V1 = c.V1 - c.A0;
        if (c.V1 != 0u) { if ((int)c.V0 == int.MinValue && (int)c.V1 == -1) { c.LO = 0x80000000u; c.HI = 0u; } else { c.LO = (uint)((int)c.V0 / (int)c.V1); c.HI = (uint)((int)c.V0 % (int)c.V1); } }
        if (c.V1 != 0u) {
            goto L8001D00C;
        }
        Bios.Break(c, mem);
        L8001D00C: ;
        c.At = 0xFFFFFFFFu;
        if (c.V1 != c.At) {
            c.At = 0x80000000u;
            goto L8001D024;
        }
        c.At = 0x80000000u;
        if (c.V0 != c.At) {
            goto L8001D024;
        }
        Bios.Break(c, mem);
        L8001D024: ;
        c.V0 = c.LO;
        mem.WriteU16((c.A1 - 0x8u), (ushort)c.V0);
        c.V0 = (uint)(short)mem.ReadU16((c.A1 - 0x2u));
        c.A0 = mem.ReadU32((c.T0 + 0x64u));
        c.V1 = mem.ReadU32((c.T0 + 0x68u));
        c.V0 = c.V0 - c.A0;
        c.V0 = c.V0 << 12;
        c.V1 = c.V1 - c.A0;
        if (c.V1 != 0u) { if ((int)c.V0 == int.MinValue && (int)c.V1 == -1) { c.LO = 0x80000000u; c.HI = 0u; } else { c.LO = (uint)((int)c.V0 / (int)c.V1); c.HI = (uint)((int)c.V0 % (int)c.V1); } }
        if (c.V1 != 0u) {
            goto L8001D058;
        }
        Bios.Break(c, mem);
        L8001D058: ;
        c.At = 0xFFFFFFFFu;
        if (c.V1 != c.At) {
            c.At = 0x80000000u;
            goto L8001D070;
        }
        c.At = 0x80000000u;
        if (c.V0 != c.At) {
            goto L8001D070;
        }
        Bios.Break(c, mem);
        L8001D070: ;
        c.V0 = c.LO;
        mem.WriteU16(c.A1, (ushort)c.V0);
        goto L8001D088;
        L8001D07C: ;
        mem.WriteU16((c.A1 - 0x10u), (ushort)0u);
        mem.WriteU16((c.A1 - 0x8u), (ushort)0u);
        mem.WriteU16(c.A1, (ushort)0u);
        L8001D088: ;
        c.V0 = (uint)(short)mem.ReadU16((c.A1 - 0x10u));
        if ((int)c.V0 >= 0) {
            goto L8001D09C;
        }
        mem.WriteU16((c.A1 - 0x10u), (ushort)0u);
        L8001D09C: ;
        c.V0 = (uint)(short)mem.ReadU16((c.A1 - 0x8u));
        if ((int)c.V0 >= 0) {
            goto L8001D0B0;
        }
        mem.WriteU16((c.A1 - 0x8u), (ushort)0u);
        L8001D0B0: ;
        c.V0 = (uint)(short)mem.ReadU16(c.A1);
        if ((int)c.V0 >= 0) {
            goto L8001D0C4;
        }
        mem.WriteU16(c.A1, (ushort)0u);
        L8001D0C4: ;
        c.A1 = c.A1 + 0x18u;
        c.A3 = c.A3 + 0x18u;
        c.A2 = c.A2 - 0x1u;
        if (c.A2 != c.T7) {
            c.T1 = c.T1 + 0x18u;
            goto L8001CF40;
        }
        c.T1 = c.T1 + 0x18u;
        L8001D0D8: ;
        c.V0 = mem.ReadU32((c.T0 + 0x20u));
        c.V1 = mem.ReadU32((c.V0 + 0x4u));
        c.V0 = 0xAAAA0000u;
        c.V0 = c.V0 | 0xAAABu;
        c.LO = (uint)((ulong)c.V1 * c.V0); c.HI = (uint)(((ulong)c.V1 * c.V0) >> 32);
        c.S3 = c.HI;
        c.A2 = c.S3 >> 1;
        c.V0 = c.A2 << 1;
        c.V0 = c.V0 + c.A2;
        c.A2 = c.V1 - c.V0;
        c.A2 = c.A2 - 0x1u;
        if (c.A2 == c.T7) {
            goto L8001D1D4;
        }
        c.T3 = 0xFFFFFFFFu;
        c.A1 = c.A3 + 0x6u;
        L8001D118: ;
        Poll(c, mem);
        RecompOne.Runtime.Gte.Write(0, (mem.ReadU32(c.T1)));
        RecompOne.Runtime.Gte.Write(1, (mem.ReadU32((c.T1 + 0x4u))));
        RecompOne.Runtime.Gte.Rtps(12, false);
        mem.WriteU32(c.A3, (RecompOne.Runtime.Gte.Read(14)));
        c.V0 = c.A3 + 0x4u;
        mem.WriteU32(c.V0, (RecompOne.Runtime.Gte.Read(19)));
        c.A0 = mem.ReadU32((c.T0 + 0x64u));
        c.V0 = mem.ReadU32((c.T0 + 0x68u));
        if (c.A0 == c.V0) {
            c.V0 = c.V0 - c.A0;
            goto L8001D190;
        }
        c.V0 = c.V0 - c.A0;
        c.V1 = (uint)(short)mem.ReadU16((c.A1 - 0x2u));
        c.V1 = c.V1 - c.A0;
        c.V1 = c.V1 << 12;
        if (c.V0 != 0u) { if ((int)c.V1 == int.MinValue && (int)c.V0 == -1) { c.LO = 0x80000000u; c.HI = 0u; } else { c.LO = (uint)((int)c.V1 / (int)c.V0); c.HI = (uint)((int)c.V1 % (int)c.V0); } }
        if (c.V0 != 0u) {
            goto L8001D16C;
        }
        Bios.Break(c, mem);
        L8001D16C: ;
        c.At = 0xFFFFFFFFu;
        if (c.V0 != c.At) {
            c.At = 0x80000000u;
            goto L8001D184;
        }
        c.At = 0x80000000u;
        if (c.V1 != c.At) {
            goto L8001D184;
        }
        Bios.Break(c, mem);
        L8001D184: ;
        c.V1 = c.LO;
        mem.WriteU16(c.A1, (ushort)c.V1);
        goto L8001D194;
        L8001D190: ;
        mem.WriteU16(c.A1, (ushort)0u);
        L8001D194: ;
        c.V0 = (uint)(short)mem.ReadU16(c.A1);
        c.V0 = (int)c.V0 < 4066 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L8001D1AC;
        }
        mem.WriteU16(c.A1, (ushort)c.T8);
        L8001D1AC: ;
        c.V0 = (uint)(short)mem.ReadU16(c.A1);
        if ((int)c.V0 >= 0) {
            goto L8001D1C0;
        }
        mem.WriteU16(c.A1, (ushort)0u);
        L8001D1C0: ;
        c.A1 = c.A1 + 0x8u;
        c.A3 = c.A3 + 0x8u;
        c.A2 = c.A2 - 0x1u;
        if (c.A2 != c.T3) {
            c.T1 = c.T1 + 0x8u;
            goto L8001D118;
        }
        c.T1 = c.T1 + 0x8u;
        L8001D1D4: ;
        c.T2 = c.T2 - 0x1u;
        c.V1 = mem.ReadU32((c.T0 + 0x20u));
        c.V0 = mem.ReadU32((c.T0 + 0x1B0u));
        c.V1 = mem.ReadU32((c.V1 + 0x4u));
        c.A0 = mem.ReadU32((c.T0 + 0x20u));
        c.V0 = c.V0 + c.V1;
        mem.WriteU32((c.T0 + 0x1B0u), c.V0);
        c.V0 = mem.ReadU32((c.A0 + 0x4u));
        c.V1 = mem.ReadU32((c.T0 + 0x34u));
        c.V0 = c.V0 << 3;
        c.V1 = c.V1 + c.V0;
        if (c.T2 != c.T7) {
            mem.WriteU32((c.T0 + 0x34u), c.V1);
            goto L8001CE48;
        }
        mem.WriteU32((c.T0 + 0x34u), c.V1);
        L8001D208: ;
        c.S3 = mem.ReadU32((c.SP + 0xCu));
        c.S2 = mem.ReadU32((c.SP + 0x8u));
        c.S1 = mem.ReadU32((c.SP + 0x4u));
        c.S0 = mem.ReadU32(c.SP);
        c.SP = c.SP + 0x10u;
        return;
    }
}
