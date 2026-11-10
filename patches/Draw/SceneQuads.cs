using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_8001FD00</c>: the scene's FT4s: a list of 12-byte records at 0x8017BED0, indexed from 0x801B9F16, counted at 0x801B9E14. A leaf. <c>ST_SCENEQUADS</c>.
/// </summary>
public static class SceneQuads
{
    public static readonly Literal R = new(typeof(SceneQuads), "scene quads", "SCENEQUADS", 0x8001FD00, Run,
        "the scene's FT4s: a list of 12-byte records at 0x8017BED0, indexed from 0x801B9F16, counted at 0x801B9E14. A leaf.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x10u;
        c.V0 = 0x801B0000u;
        c.V1 = c.V0 + 0x5948u;
        mem.WriteU32((c.SP + 0x8u), c.S2);
        mem.WriteU32((c.SP + 0x4u), c.S1);
        mem.WriteU32(c.SP, c.S0);
        c.T7 = mem.ReadU16((c.V1 + 0x44CCu));
        c.V0 = 0xFFFFFFFFu;
        c.T7 = c.T7 - 0x1u;
        if (c.T7 == c.V0) {
            c.A1 = 0x1F800000u;
            goto L8002014C;
        }
        c.A1 = 0x1F800000u;
        c.T9 = c.V1;
        c.T8 = 0x00FF0000u;
        c.T8 = c.T8 | 0xFFFFu;
        L8001FD38: ;
        Poll(c, mem);
        c.V0 = c.T7 << 1;
        c.V0 = c.V0 + c.T9;
        c.V0 = mem.ReadU16((c.V0 + 0x45CEu));
        c.V1 = c.V0 << 1;
        c.V1 = c.V1 + c.V0;
        c.V1 = c.V1 << 2;
        c.V0 = 0x80180000u;
        c.V0 = c.V0 - 0x4130u;
        c.A0 = c.V1 + c.V0;
        c.V0 = mem.ReadU8((c.A0 + 0x4u));
        c.V0 = c.V0 & 0x0003u;
        if (c.V0 == 0u) {
            goto L8001FDA4;
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
        goto L80020130;
        L8001FDA4: ;
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
        c.T3 = mem.ReadU16((c.A0 + 0x1Cu));
        c.V1 = mem.ReadU32((c.A1 + 0xCu));
        c.A0 = mem.ReadU16((c.A0 + 0x18u));
        c.V0 = c.V0 + 0xCu;
        c.T2 = c.V1 + c.V0;
        c.V0 = c.A0 << 2;
        c.V0 = c.V0 + c.A0;
        c.V0 = c.V0 << 2;
        c.T2 = c.T2 + c.V0;
        c.V1 = mem.ReadU32((c.A1 + 0x1B4u));
        c.V0 = c.T3;
        c.V1 = c.V1 + c.T3;
        c.T3 = c.T3 - 0x1u;
        if (c.V0 == 0u) {
            mem.WriteU32((c.A1 + 0x1B4u), c.V1);
            goto L80020124;
        }
        mem.WriteU32((c.A1 + 0x1B4u), c.V1);
        c.A2 = c.T2 + 0xCu;
        L8001FE84: ;
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
        c.V0 = mem.ReadU8((c.A2 - 0x1u));
        c.V0 = c.V0 | 0x002Cu;
        mem.WriteU8((c.A1 + 0x47u), (byte)c.V0);
        c.V0 = c.A1 + 0x70u;
        mem.WriteU32(c.V0, (RecompOne.Runtime.Gte.Read(24)));
        c.V1 = mem.ReadU16((c.A2 + 0xCu));
        c.V0 = mem.ReadU32((c.A1 + 0x34u));
        c.S0 = mem.ReadU32((c.A1 + 0x10u));
        c.T1 = c.V0 + c.V1;
        mem.WriteU32((c.S0 + 0x8u), (RecompOne.Runtime.Gte.Read(12)));
        mem.WriteU32((c.S0 + 0x10u), (RecompOne.Runtime.Gte.Read(13)));
        mem.WriteU32((c.S0 + 0x18u), (RecompOne.Runtime.Gte.Read(14)));
        c.S1 = mem.ReadU32(c.T1);
        RecompOne.Runtime.Gte.Write(12, c.S1);
        RecompOne.Runtime.Gte.Nclip();
        c.V0 = c.A1 + 0x74u;
        mem.WriteU32(c.V0, (RecompOne.Runtime.Gte.Read(24)));
        c.V0 = mem.ReadU32((c.A1 + 0x70u));
        if ((int)c.V0 > 0) {
            goto L8001FF38;
        }
        c.V0 = mem.ReadU32((c.A1 + 0x74u));
        if ((int)c.V0 >= 0) {
            goto L8002002C;
        }
        L8001FF38: ;
        c.V0 = mem.ReadU32((c.A1 + 0x10u));
        c.V0 = c.V0 + 0x20u;
        mem.WriteU32(c.V0, (RecompOne.Runtime.Gte.Read(12)));
        c.V0 = (uint)(short)mem.ReadU16((c.T0 + 0x4u));
        mem.WriteU32((c.A1 + 0x84u), c.V0);
        mem.WriteU32((c.A1 + 0x80u), c.V0);
        c.V1 = (uint)(short)mem.ReadU16((c.A3 + 0x4u));
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001FF74;
        }
        mem.WriteU32((c.A1 + 0x84u), c.V1);
        c.V1 = (uint)(short)mem.ReadU16((c.A3 + 0x4u));
        L8001FF74: ;
        c.V0 = mem.ReadU32((c.A1 + 0x80u));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001FF8C;
        }
        mem.WriteU32((c.A1 + 0x80u), c.V1);
        L8001FF8C: ;
        c.V1 = (uint)(short)mem.ReadU16((c.A0 + 0x4u));
        c.V0 = mem.ReadU32((c.A1 + 0x84u));
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001FFAC;
        }
        mem.WriteU32((c.A1 + 0x84u), c.V1);
        c.V1 = (uint)(short)mem.ReadU16((c.A0 + 0x4u));
        L8001FFAC: ;
        c.V0 = mem.ReadU32((c.A1 + 0x80u));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001FFC4;
        }
        mem.WriteU32((c.A1 + 0x80u), c.V1);
        L8001FFC4: ;
        c.V1 = (uint)(short)mem.ReadU16((c.T1 + 0x4u));
        c.V0 = mem.ReadU32((c.A1 + 0x84u));
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001FFE4;
        }
        mem.WriteU32((c.A1 + 0x84u), c.V1);
        c.V1 = (uint)(short)mem.ReadU16((c.T1 + 0x4u));
        L8001FFE4: ;
        c.V0 = mem.ReadU32((c.A1 + 0x80u));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001FFFC;
        }
        mem.WriteU32((c.A1 + 0x80u), c.V1);
        L8001FFFC: ;
        c.V0 = mem.ReadU32((c.A1 + 0x84u));
        c.V1 = (uint)(sbyte)mem.ReadU8((c.A2 - 0x2u));
        c.V0 = (uint)((int)c.V0 >> 5);
        c.V1 = c.V0 + c.V1;
        c.V0 = (int)c.V1 < 4096 ? 1u : 0u;
        if (c.V0 == 0u) {
            mem.WriteU32((c.A1 + 0x88u), c.V1);
            goto L8002002C;
        }
        mem.WriteU32((c.A1 + 0x88u), c.V1);
        c.V0 = mem.ReadU32((c.A1 + 0x8Cu));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L80020038;
        }
        L8002002C: ;
        c.A2 = c.A2 + 0x1Cu;
        c.T2 = c.T2 + 0x1Cu;
        goto L80020118;
        L80020038: ;
        c.A0 = mem.ReadU32((c.A1 + 0x10u));
        c.V0 = mem.ReadU32((c.A1 + 0x14u));
        c.V1 = c.A0 + 0x28u;
        c.V0 = c.V0 < c.V1 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L8002014C;
        }
        c.V0 = mem.ReadU32((c.A1 + 0x1BCu));
        mem.WriteU32((c.A1 + 0x10u), c.V1);
        c.V0 = c.V0 + 0x1u;
        mem.WriteU32((c.A1 + 0x1BCu), c.V0);
        c.V0 = mem.ReadU16((c.A2 - 0xAu));
        mem.WriteU16((c.A0 + 0xEu), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.A2 - 0x6u));
        mem.WriteU16((c.A0 + 0x16u), (ushort)c.V0);
        c.V1 = mem.ReadU16((c.A2 + 0x4u));
        c.V0 = mem.ReadU32((c.A1 + 0x24u));
        c.V0 = c.V0 + c.V1;
        RecompOne.Runtime.Gte.Write(0, (mem.ReadU32(c.V0)));
        RecompOne.Runtime.Gte.Write(1, (mem.ReadU32((c.V0 + 0x4u))));
        c.V0 = c.A1 + 0x44u;
        RecompOne.Runtime.Gte.Write(6, (mem.ReadU32(c.V0)));
        c.V0 = mem.ReadU32(c.T2);
        mem.WriteU32((c.A0 + 0xCu), c.V0);
        c.V0 = mem.ReadU32((c.A2 - 0x8u));
        mem.WriteU32((c.A0 + 0x14u), c.V0);
        RecompOne.Runtime.Gte.NccsOp(12, true);
        c.V0 = mem.ReadU16((c.A2 - 0x4u));
        mem.WriteU16((c.A0 + 0x1Cu), (ushort)c.V0);
        c.V0 = mem.ReadU16(c.A2);
        mem.WriteU16((c.A0 + 0x24u), (ushort)c.V0);
        c.V0 = c.A0 + 0x4u;
        mem.WriteU32(c.V0, (RecompOne.Runtime.Gte.Read(22)));
        c.A2 = c.A2 + 0x1Cu;
        c.T2 = c.T2 + 0x1Cu;
        c.V0 = mem.ReadU32((c.A1 + 0x88u));
        c.V1 = mem.ReadU32(c.A1);
        c.V0 = c.V0 << 2;
        c.V0 = c.V0 + c.V1;
        c.V0 = mem.ReadU32(c.V0);
        c.V1 = 0x09000000u;
        c.V0 = c.V0 & c.T8;
        c.V0 = c.V0 | c.V1;
        mem.WriteU32(c.A0, c.V0);
        c.V0 = mem.ReadU32((c.A1 + 0x88u));
        c.V1 = mem.ReadU32(c.A1);
        c.V0 = c.V0 << 2;
        c.V0 = c.V0 + c.V1;
        c.V1 = c.A0 & c.T8;
        mem.WriteU32(c.V0, c.V1);
        L80020118: ;
        c.V0 = c.T3;
        if (c.V0 != 0u) {
            c.T3 = c.T3 - 0x1u;
            goto L8001FE84;
        }
        c.T3 = c.T3 - 0x1u;
        L80020124: ;
        c.V0 = mem.ReadU32((c.A1 + 0x20u));
        c.V0 = mem.ReadU32((c.V0 + 0x4u));
        L80020130: ;
        c.V1 = mem.ReadU32((c.A1 + 0x34u));
        c.V0 = c.V0 << 3;
        c.V1 = c.V1 + c.V0;
        c.T7 = c.T7 - 0x1u;
        c.V0 = 0xFFFFFFFFu;
        if (c.T7 != c.V0) {
            mem.WriteU32((c.A1 + 0x34u), c.V1);
            goto L8001FD38;
        }
        mem.WriteU32((c.A1 + 0x34u), c.V1);
        L8002014C: ;
        c.S2 = mem.ReadU32((c.SP + 0x8u));
        c.S1 = mem.ReadU32((c.SP + 0x4u));
        c.S0 = mem.ReadU32(c.SP);
        c.SP = c.SP + 0x10u;
        return;
    }
}
