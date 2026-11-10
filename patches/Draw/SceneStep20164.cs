using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_80020164</c>: a scene step between the scene's own quads and the map's; wrote no packet in map 0. <c>ST_SCENESTEP20164</c>.
/// </summary>
public static class SceneStep20164
{
    public static readonly Literal R = new(typeof(SceneStep20164), "scene step 20164", "SCENESTEP20164", 0x80020164, Run,
        "a scene step between the scene's own quads and the map's; wrote no packet in map 0.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x8u;
        c.V0 = 0x801B0000u;
        c.V1 = c.V0 + 0x5948u;
        mem.WriteU32((c.SP + 0x4u), c.S1);
        mem.WriteU32(c.SP, c.S0);
        c.T2 = mem.ReadU16((c.V1 + 0x44CCu));
        c.V0 = 0xFFFFFFFFu;
        c.T2 = c.T2 - 0x1u;
        if (c.T2 == c.V0) {
            c.T0 = 0x1F800000u;
            goto L80020574;
        }
        c.T0 = 0x1F800000u;
        c.T8 = c.V1;
        c.T7 = c.V0;
        c.T3 = 0x00FF0000u;
        c.T3 = c.T3 | 0xFFFFu;
        L8002019C: ;
        Poll(c, mem);
        c.V0 = c.T2 << 1;
        c.V0 = c.V0 + c.T8;
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
            goto L80020208;
        }
        c.V1 = mem.ReadU8(c.A0);
        c.V0 = c.V1 << 2;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 3;
        c.V1 = mem.ReadU32((c.T0 + 0xCu));
        c.V0 = c.V0 + 0xCu;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32((c.T0 + 0x20u), c.V1);
        c.V0 = mem.ReadU32((c.V1 + 0x4u));
        goto L8002055C;
        L80020208: ;
        c.V0 = mem.ReadU8((c.A0 + 0x3u));
        c.V0 = c.V0 & 0x0003u;
        c.V1 = c.V0 << 2;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32((c.T0 + 0x1E4u), c.V0);
        c.V0 = mem.ReadU32((c.T0 + 0x3C4u));
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
        c.V1 = mem.ReadU32((c.T0 + 0xCu));
        c.V0 = c.V0 + 0xCu;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32((c.T0 + 0x20u), c.V1);
        c.V1 = mem.ReadU32(c.V1);
        c.V0 = mem.ReadU32((c.T0 + 0xCu));
        c.A0 = mem.ReadU32((c.T0 + 0x20u));
        c.V1 = c.V1 + 0xCu;
        c.V0 = c.V0 + c.V1;
        c.A1 = c.A0;
        mem.WriteU32((c.T0 + 0x3Cu), c.V0);
        c.V1 = mem.ReadU32((c.A0 + 0x8u));
        c.V0 = mem.ReadU32((c.T0 + 0xCu));
        c.V1 = c.V1 + 0xCu;
        c.V0 = c.V0 + c.V1;
        mem.WriteU32((c.T0 + 0x24u), c.V0);
        c.V0 = mem.ReadU32((c.A1 + 0x10u));
        c.V1 = mem.ReadU32((c.T0 + 0xCu));
        c.A0 = mem.ReadU16((c.A1 + 0x18u));
        c.V0 = c.V0 + 0xCu;
        c.T1 = c.V1 + c.V0;
        c.V0 = c.A0 << 2;
        c.V0 = c.V0 + c.A0;
        c.V0 = c.V0 << 2;
        c.T1 = c.T1 + c.V0;
        c.V1 = mem.ReadU16((c.A1 + 0x1Cu));
        c.A0 = mem.ReadU16((c.A1 + 0x1Au));
        c.V0 = c.V1 << 3;
        c.V0 = c.V0 - c.V1;
        c.V0 = c.V0 << 2;
        c.T1 = c.T1 + c.V0;
        mem.WriteU32((c.T0 + 0x184u), c.A0);
        c.V1 = c.A0;
        c.V0 = mem.ReadU32((c.T0 + 0x1B4u));
        c.V1 = c.V1 - 0x1u;
        mem.WriteU32((c.T0 + 0x184u), c.V1);
        c.V0 = c.V0 + c.A0;
        if (c.V1 == c.T7) {
            mem.WriteU32((c.T0 + 0x1B4u), c.V0);
            goto L80020550;
        }
        mem.WriteU32((c.T0 + 0x1B4u), c.V0);
        c.A3 = c.T1 + 0x8u;
        L80020308: ;
        Poll(c, mem);
        c.V1 = mem.ReadU16((c.A3 + 0x6u));
        c.V0 = mem.ReadU32((c.T0 + 0x34u));
        c.A0 = mem.ReadU16((c.A3 + 0xEu));
        c.A2 = c.V0 + c.V1;
        c.A0 = c.V0 + c.A0;
        c.V1 = mem.ReadU16((c.A3 + 0xAu));
        c.S0 = mem.ReadU32(c.A2);
        c.T9 = mem.ReadU32(c.A0);
        c.A1 = c.V0 + c.V1;
        c.S1 = mem.ReadU32(c.A1);
        RecompOne.Runtime.Gte.Write(12, c.S0);
        RecompOne.Runtime.Gte.Write(14, c.T9);
        RecompOne.Runtime.Gte.Write(13, c.S1);
        RecompOne.Runtime.Gte.Nclip();
        c.V0 = mem.ReadU8((c.A3 + 0x3u));
        c.V0 = c.V0 | 0x0034u;
        mem.WriteU8((c.T0 + 0x47u), (byte)c.V0);
        c.V0 = c.T0 + 0x70u;
        mem.WriteU32(c.V0, (RecompOne.Runtime.Gte.Read(24)));
        c.V0 = mem.ReadU32((c.T0 + 0x70u));
        if ((int)c.V0 <= 0) {
            goto L80020430;
        }
        c.S0 = mem.ReadU32((c.T0 + 0x10u));
        mem.WriteU32((c.S0 + 0x8u), (RecompOne.Runtime.Gte.Read(12)));
        mem.WriteU32((c.S0 + 0x14u), (RecompOne.Runtime.Gte.Read(13)));
        mem.WriteU32((c.S0 + 0x20u), (RecompOne.Runtime.Gte.Read(14)));
        c.V0 = (uint)(short)mem.ReadU16((c.A2 + 0x4u));
        mem.WriteU32((c.T0 + 0x84u), c.V0);
        mem.WriteU32((c.T0 + 0x80u), c.V0);
        c.V1 = (uint)(short)mem.ReadU16((c.A1 + 0x4u));
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L800203B0;
        }
        mem.WriteU32((c.T0 + 0x84u), c.V1);
        c.V1 = (uint)(short)mem.ReadU16((c.A1 + 0x4u));
        L800203B0: ;
        c.V0 = mem.ReadU32((c.T0 + 0x80u));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L800203C8;
        }
        mem.WriteU32((c.T0 + 0x80u), c.V1);
        L800203C8: ;
        c.V1 = (uint)(short)mem.ReadU16((c.A0 + 0x4u));
        c.V0 = mem.ReadU32((c.T0 + 0x84u));
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L800203E8;
        }
        mem.WriteU32((c.T0 + 0x84u), c.V1);
        c.V1 = (uint)(short)mem.ReadU16((c.A0 + 0x4u));
        L800203E8: ;
        c.V0 = mem.ReadU32((c.T0 + 0x80u));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L80020400;
        }
        mem.WriteU32((c.T0 + 0x80u), c.V1);
        L80020400: ;
        c.V0 = mem.ReadU32((c.T0 + 0x84u));
        c.V1 = (uint)(sbyte)mem.ReadU8((c.A3 + 0x2u));
        c.V0 = (uint)((int)c.V0 >> 5);
        c.V1 = c.V0 + c.V1;
        c.V0 = (int)c.V1 < 4096 ? 1u : 0u;
        if (c.V0 == 0u) {
            mem.WriteU32((c.T0 + 0x88u), c.V1);
            goto L80020430;
        }
        mem.WriteU32((c.T0 + 0x88u), c.V1);
        c.V0 = mem.ReadU32((c.T0 + 0x8Cu));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8002043C;
        }
        L80020430: ;
        c.A3 = c.A3 + 0x18u;
        c.T1 = c.T1 + 0x18u;
        goto L8002053C;
        L8002043C: ;
        c.A2 = mem.ReadU32((c.T0 + 0x10u));
        c.V0 = mem.ReadU32((c.T0 + 0x14u));
        c.V1 = c.A2 + 0x28u;
        c.V0 = c.V0 < c.V1 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L80020574;
        }
        c.V0 = mem.ReadU32((c.T0 + 0x1BCu));
        mem.WriteU32((c.T0 + 0x10u), c.V1);
        c.V0 = c.V0 + 0x1u;
        mem.WriteU32((c.T0 + 0x1BCu), c.V0);
        c.V0 = mem.ReadU16((c.A3 - 0x6u));
        mem.WriteU16((c.A2 + 0xEu), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.A3 - 0x2u));
        mem.WriteU16((c.A2 + 0x1Au), (ushort)c.V0);
        c.A1 = mem.ReadU16((c.A3 + 0x4u));
        c.V0 = mem.ReadU32((c.T0 + 0x24u));
        c.V1 = mem.ReadU16((c.A3 + 0x8u));
        c.A0 = mem.ReadU16((c.A3 + 0xCu));
        c.A1 = c.V0 + c.A1;
        c.V1 = c.V0 + c.V1;
        c.V0 = c.V0 + c.A0;
        RecompOne.Runtime.Gte.Write(0, (mem.ReadU32(c.A1)));
        RecompOne.Runtime.Gte.Write(1, (mem.ReadU32((c.A1 + 0x4u))));
        RecompOne.Runtime.Gte.Write(2, (mem.ReadU32(c.V1)));
        RecompOne.Runtime.Gte.Write(3, (mem.ReadU32((c.V1 + 0x4u))));
        RecompOne.Runtime.Gte.Write(4, (mem.ReadU32(c.V0)));
        RecompOne.Runtime.Gte.Write(5, (mem.ReadU32((c.V0 + 0x4u))));
        c.V0 = c.T0 + 0x44u;
        RecompOne.Runtime.Gte.Write(6, (mem.ReadU32(c.V0)));
        RecompOne.Runtime.Gte.NcctOp(12, true);
        c.A0 = c.A2 + 0x4u;
        c.V1 = c.A2 + 0x10u;
        c.V0 = c.A2 + 0x1Cu;
        mem.WriteU32(c.A0, (RecompOne.Runtime.Gte.Read(20)));
        mem.WriteU32(c.V1, (RecompOne.Runtime.Gte.Read(21)));
        mem.WriteU32(c.V0, (RecompOne.Runtime.Gte.Read(22)));
        c.V0 = mem.ReadU32(c.T1);
        mem.WriteU32((c.A2 + 0xCu), c.V0);
        c.V0 = mem.ReadU32((c.A3 - 0x4u));
        c.T1 = c.T1 + 0x18u;
        mem.WriteU32((c.A2 + 0x18u), c.V0);
        c.V0 = mem.ReadU16(c.A3);
        c.A3 = c.A3 + 0x18u;
        mem.WriteU16((c.A2 + 0x24u), (ushort)c.V0);
        c.V0 = mem.ReadU32((c.T0 + 0x88u));
        c.V1 = mem.ReadU32(c.T0);
        c.V0 = c.V0 << 2;
        c.V0 = c.V0 + c.V1;
        c.V0 = mem.ReadU32(c.V0);
        c.V1 = 0x09000000u;
        c.V0 = c.V0 & c.T3;
        c.V0 = c.V0 | c.V1;
        mem.WriteU32(c.A2, c.V0);
        c.V0 = mem.ReadU32((c.T0 + 0x88u));
        c.V1 = mem.ReadU32(c.T0);
        c.V0 = c.V0 << 2;
        c.V0 = c.V0 + c.V1;
        c.V1 = c.A2 & c.T3;
        mem.WriteU32(c.V0, c.V1);
        L8002053C: ;
        c.V0 = mem.ReadU32((c.T0 + 0x184u));
        c.V0 = c.V0 - 0x1u;
        if (c.V0 != c.T7) {
            mem.WriteU32((c.T0 + 0x184u), c.V0);
            goto L80020308;
        }
        mem.WriteU32((c.T0 + 0x184u), c.V0);
        L80020550: ;
        c.V0 = mem.ReadU32((c.T0 + 0x20u));
        c.V0 = mem.ReadU32((c.V0 + 0x4u));
        L8002055C: ;
        c.V1 = mem.ReadU32((c.T0 + 0x34u));
        c.V0 = c.V0 << 3;
        c.V1 = c.V1 + c.V0;
        c.T2 = c.T2 - 0x1u;
        if (c.T2 != c.T7) {
            mem.WriteU32((c.T0 + 0x34u), c.V1);
            goto L8002019C;
        }
        mem.WriteU32((c.T0 + 0x34u), c.V1);
        L80020574: ;
        c.S1 = mem.ReadU32((c.SP + 0x4u));
        c.S0 = mem.ReadU32(c.SP);
        c.SP = c.SP + 0x8u;
        return;
    }
}
