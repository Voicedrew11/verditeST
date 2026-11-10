using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_8001F954</c>: the scene's FT3s, beside func_8001FD00's FT4s. A leaf. <c>ST_SCENETRIANGLES</c>.
/// </summary>
public static class SceneTriangles
{
    public static readonly Literal R = new(typeof(SceneTriangles), "scene triangles", "SCENETRIANGLES", 0x8001F954, Run,
        "the scene's FT3s, beside func_8001FD00's FT4s. A leaf.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x10u;
        c.V0 = 0x801B0000u;
        c.V1 = c.V0 + 0x5948u;
        mem.WriteU32((c.SP + 0x8u), c.S2);
        mem.WriteU32((c.SP + 0x4u), c.S1);
        mem.WriteU32(c.SP, c.S0);
        c.T3 = mem.ReadU16((c.V1 + 0x44CCu));
        c.V0 = 0xFFFFFFFFu;
        c.T3 = c.T3 - 0x1u;
        if (c.T3 == c.V0) {
            c.A1 = 0x1F800000u;
            goto L8001FCE8;
        }
        c.A1 = 0x1F800000u;
        c.T9 = c.V1;
        c.V0 = 0x80180000u;
        c.T8 = c.V0 - 0x4130u;
        c.T7 = 0x00FF0000u;
        c.T7 = c.T7 | 0xFFFFu;
        L8001F994: ;
        Poll(c, mem);
        c.V0 = c.T3 << 1;
        c.V0 = c.V0 + c.T9;
        c.V1 = mem.ReadU16((c.V0 + 0x45CEu));
        c.V0 = c.V1 << 1;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 2;
        c.A0 = c.V0 + c.T8;
        c.V0 = mem.ReadU8((c.A0 + 0x4u));
        c.V0 = c.V0 & 0x0003u;
        if (c.V0 == 0u) {
            goto L8001F9F8;
        }
        c.V1 = mem.ReadU8(c.A0);
        c.V0 = c.V1 << 2;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 3;
        c.V1 = mem.ReadU32((c.A1 + 0xCu));
        c.V0 = c.V0 + 0xCu;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32((c.A1 + 0x20u), c.V1);
        c.V0 = mem.ReadU32((c.V1 + 0x4u));
        goto L8001FCCC;
        L8001F9F8: ;
        c.V0 = mem.ReadU8((c.A0 + 0x3u));
        c.V0 = c.V0 & 0x0003u;
        c.V1 = c.V0 << 2;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32((c.A1 + 0x1E4u), c.V0);
        c.V0 = mem.ReadU32((c.A1 + 0x3C4u));
        c.V1 = c.V1 << 2;
        c.V0 = c.V0 + c.V1;
        c.T4 = mem.ReadU32(c.V0);
        c.T5 = mem.ReadU32((c.V0 + 0x4u));
        RecompOne.Runtime.Gte.WriteControl(8, c.T4);
        RecompOne.Runtime.Gte.WriteControl(9, c.T5);
        c.T4 = mem.ReadU32((c.V0 + 0x8u));
        c.T5 = mem.ReadU32((c.V0 + 0xCu));
        c.T6 = mem.ReadU32((c.V0 + 0x10u));
        RecompOne.Runtime.Gte.WriteControl(10, c.T4);
        RecompOne.Runtime.Gte.WriteControl(11, c.T5);
        RecompOne.Runtime.Gte.WriteControl(12, c.T6);
        c.V1 = mem.ReadU8(c.A0);
        c.V0 = c.V1 << 2;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 3;
        c.V1 = mem.ReadU32((c.A1 + 0xCu));
        c.V0 = c.V0 + 0xCu;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32((c.A1 + 0x20u), c.V1);
        c.V1 = mem.ReadU32(c.V1);
        c.V0 = mem.ReadU32((c.A1 + 0xCu));
        c.A0 = mem.ReadU32((c.A1 + 0x20u));
        c.V1 = c.V1 + 0xCu;
        c.V0 = c.V0 + c.V1;
        mem.WriteU32((c.A1 + 0x3Cu), c.V0);
        c.V1 = mem.ReadU32((c.A0 + 0x8u));
        c.V0 = mem.ReadU32((c.A1 + 0xCu));
        c.V1 = c.V1 + 0xCu;
        c.V0 = c.V0 + c.V1;
        mem.WriteU32((c.A1 + 0x24u), c.V0);
        c.V0 = mem.ReadU32((c.A0 + 0x10u));
        c.T2 = mem.ReadU16((c.A0 + 0x18u));
        c.A0 = mem.ReadU32((c.A1 + 0xCu));
        c.V1 = mem.ReadU32((c.A1 + 0x1B4u));
        c.V0 = c.V0 + 0xCu;
        c.T1 = c.A0 + c.V0;
        c.V1 = c.V1 + c.T2;
        c.V0 = c.T2;
        c.T2 = c.T2 - 0x1u;
        if (c.V0 == 0u) {
            mem.WriteU32((c.A1 + 0x1B4u), c.V1);
            goto L8001FCC0;
        }
        mem.WriteU32((c.A1 + 0x1B4u), c.V1);
        c.A2 = c.T1 + 0x8u;
        L8001FAC4: ;
        Poll(c, mem);
        c.V1 = mem.ReadU16((c.A2 + 0x6u));
        c.V0 = mem.ReadU32((c.A1 + 0x34u));
        c.A0 = mem.ReadU16((c.A2 + 0xAu));
        c.T0 = c.V0 + c.V1;
        c.A0 = c.V0 + c.A0;
        c.V1 = mem.ReadU16((c.A2 + 0x8u));
        c.S0 = mem.ReadU32(c.T0);
        c.S2 = mem.ReadU32(c.A0);
        c.A3 = c.V0 + c.V1;
        c.S1 = mem.ReadU32(c.A3);
        RecompOne.Runtime.Gte.Write(12, c.S0);
        RecompOne.Runtime.Gte.Write(14, c.S2);
        RecompOne.Runtime.Gte.Write(13, c.S1);
        RecompOne.Runtime.Gte.Nclip();
        c.V0 = mem.ReadU8((c.A2 + 0x3u));
        c.V0 = c.V0 | 0x0024u;
        mem.WriteU8((c.A1 + 0x47u), (byte)c.V0);
        c.V0 = c.A1 + 0x70u;
        mem.WriteU32(c.V0, (RecompOne.Runtime.Gte.Read(24)));
        c.V0 = mem.ReadU32((c.A1 + 0x70u));
        if ((int)c.V0 <= 0) {
            goto L8001FBEC;
        }
        c.S0 = mem.ReadU32((c.A1 + 0x10u));
        mem.WriteU32((c.S0 + 0x8u), (RecompOne.Runtime.Gte.Read(12)));
        mem.WriteU32((c.S0 + 0x10u), (RecompOne.Runtime.Gte.Read(13)));
        mem.WriteU32((c.S0 + 0x18u), (RecompOne.Runtime.Gte.Read(14)));
        c.V0 = (uint)(short)mem.ReadU16((c.T0 + 0x4u));
        mem.WriteU32((c.A1 + 0x84u), c.V0);
        mem.WriteU32((c.A1 + 0x80u), c.V0);
        c.V1 = (uint)(short)mem.ReadU16((c.A3 + 0x4u));
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001FB6C;
        }
        mem.WriteU32((c.A1 + 0x84u), c.V1);
        c.V1 = (uint)(short)mem.ReadU16((c.A3 + 0x4u));
        L8001FB6C: ;
        c.V0 = mem.ReadU32((c.A1 + 0x80u));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001FB84;
        }
        mem.WriteU32((c.A1 + 0x80u), c.V1);
        L8001FB84: ;
        c.V1 = (uint)(short)mem.ReadU16((c.A0 + 0x4u));
        c.V0 = mem.ReadU32((c.A1 + 0x84u));
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001FBA4;
        }
        mem.WriteU32((c.A1 + 0x84u), c.V1);
        c.V1 = (uint)(short)mem.ReadU16((c.A0 + 0x4u));
        L8001FBA4: ;
        c.V0 = mem.ReadU32((c.A1 + 0x80u));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001FBBC;
        }
        mem.WriteU32((c.A1 + 0x80u), c.V1);
        L8001FBBC: ;
        c.V0 = mem.ReadU32((c.A1 + 0x84u));
        c.V1 = (uint)(sbyte)mem.ReadU8((c.A2 + 0x2u));
        c.V0 = (uint)((int)c.V0 >> 5);
        c.V1 = c.V0 + c.V1;
        c.V0 = (int)c.V1 < 4096 ? 1u : 0u;
        if (c.V0 == 0u) {
            mem.WriteU32((c.A1 + 0x88u), c.V1);
            goto L8001FBEC;
        }
        mem.WriteU32((c.A1 + 0x88u), c.V1);
        c.V0 = mem.ReadU32((c.A1 + 0x8Cu));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001FBF8;
        }
        L8001FBEC: ;
        c.A2 = c.A2 + 0x14u;
        c.T1 = c.T1 + 0x14u;
        goto L8001FCB4;
        L8001FBF8: ;
        c.A0 = mem.ReadU32((c.A1 + 0x10u));
        c.V0 = mem.ReadU32((c.A1 + 0x14u));
        c.V1 = c.A0 + 0x20u;
        c.V0 = c.V0 < c.V1 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L8001FCE8;
        }
        c.V0 = mem.ReadU32((c.A1 + 0x1BCu));
        mem.WriteU32((c.A1 + 0x10u), c.V1);
        c.V0 = c.V0 + 0x1u;
        mem.WriteU32((c.A1 + 0x1BCu), c.V0);
        c.V1 = mem.ReadU16((c.A2 + 0x4u));
        c.V0 = mem.ReadU32((c.A1 + 0x24u));
        c.V0 = c.V0 + c.V1;
        RecompOne.Runtime.Gte.Write(0, (mem.ReadU32(c.V0)));
        RecompOne.Runtime.Gte.Write(1, (mem.ReadU32((c.V0 + 0x4u))));
        c.V0 = c.A1 + 0x44u;
        RecompOne.Runtime.Gte.Write(6, (mem.ReadU32(c.V0)));
        c.V0 = mem.ReadU32(c.T1);
        mem.WriteU32((c.A0 + 0xCu), c.V0);
        c.V0 = mem.ReadU32((c.A2 - 0x4u));
        mem.WriteU32((c.A0 + 0x14u), c.V0);
        RecompOne.Runtime.Gte.NccsOp(12, true);
        c.V0 = mem.ReadU16(c.A2);
        mem.WriteU16((c.A0 + 0x1Cu), (ushort)c.V0);
        c.V0 = mem.ReadU32((c.A1 + 0x88u));
        c.V1 = mem.ReadU32(c.A1);
        c.V0 = c.V0 << 2;
        c.V0 = c.V0 + c.V1;
        c.V0 = mem.ReadU32(c.V0);
        c.V1 = 0x07000000u;
        c.V0 = c.V0 & c.T7;
        c.V0 = c.V0 | c.V1;
        mem.WriteU32(c.A0, c.V0);
        c.V0 = c.A0 + 0x4u;
        mem.WriteU32(c.V0, (RecompOne.Runtime.Gte.Read(22)));
        c.A2 = c.A2 + 0x14u;
        c.T1 = c.T1 + 0x14u;
        c.V0 = mem.ReadU32((c.A1 + 0x88u));
        c.V1 = mem.ReadU32(c.A1);
        c.V0 = c.V0 << 2;
        c.V0 = c.V0 + c.V1;
        c.V1 = c.A0 & c.T7;
        mem.WriteU32(c.V0, c.V1);
        L8001FCB4: ;
        c.V0 = c.T2;
        if (c.V0 != 0u) {
            c.T2 = c.T2 - 0x1u;
            goto L8001FAC4;
        }
        c.T2 = c.T2 - 0x1u;
        L8001FCC0: ;
        c.V0 = mem.ReadU32((c.A1 + 0x20u));
        c.V0 = mem.ReadU32((c.V0 + 0x4u));
        L8001FCCC: ;
        c.V1 = mem.ReadU32((c.A1 + 0x34u));
        c.V0 = c.V0 << 3;
        c.V1 = c.V1 + c.V0;
        c.T3 = c.T3 - 0x1u;
        c.V0 = 0xFFFFFFFFu;
        if (c.T3 != c.V0) {
            mem.WriteU32((c.A1 + 0x34u), c.V1);
            goto L8001F994;
        }
        mem.WriteU32((c.A1 + 0x34u), c.V1);
        L8001FCE8: ;
        c.S2 = mem.ReadU32((c.SP + 0x8u));
        c.S1 = mem.ReadU32((c.SP + 0x4u));
        c.S0 = mem.ReadU32(c.SP);
        c.SP = c.SP + 0x10u;
        return;
    }
}
