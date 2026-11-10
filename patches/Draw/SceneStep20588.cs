using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_80020588</c>: a scene step before the map's triangles; wrote no packet in map 0. <c>ST_SCENESTEP20588</c>.
/// </summary>
public static class SceneStep20588
{
    public static readonly Literal R = new(typeof(SceneStep20588), "scene step 20588", "SCENESTEP20588", 0x80020588, Run,
        "a scene step before the map's triangles; wrote no packet in map 0.");

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
            c.T0 = 0x1F800000u;
            goto L80020A68;
        }
        c.T0 = 0x1F800000u;
        c.T9 = c.V1;
        c.T8 = c.V0;
        c.T7 = 0x00FF0000u;
        c.T7 = c.T7 | 0xFFFFu;
        L800205C4: ;
        Poll(c, mem);
        c.V0 = c.T3 << 1;
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
            goto L80020630;
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
        goto L80020A50;
        L80020630: ;
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
        c.A0 = mem.ReadU16((c.A1 + 0x1Eu));
        c.V0 = c.V1 << 3;
        c.V0 = c.V0 - c.V1;
        c.V0 = c.V0 << 2;
        c.V1 = mem.ReadU16((c.A1 + 0x1Au));
        c.T1 = c.T1 + c.V0;
        mem.WriteU32((c.T0 + 0x184u), c.A0);
        c.V0 = c.V1 << 1;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 3;
        c.T1 = c.T1 + c.V0;
        c.V1 = c.A0;
        c.V0 = mem.ReadU32((c.T0 + 0x1B4u));
        c.V1 = c.V1 - 0x1u;
        mem.WriteU32((c.T0 + 0x184u), c.V1);
        c.V0 = c.V0 + c.A0;
        if (c.V1 == c.T8) {
            mem.WriteU32((c.T0 + 0x1B4u), c.V0);
            goto L80020A44;
        }
        mem.WriteU32((c.T0 + 0x1B4u), c.V0);
        c.A3 = c.T1 + 0xCu;
        L80020744: ;
        Poll(c, mem);
        c.V1 = mem.ReadU16((c.A3 + 0x6u));
        c.V0 = mem.ReadU32((c.T0 + 0x34u));
        c.A0 = mem.ReadU16((c.A3 + 0xEu));
        c.A2 = c.V0 + c.V1;
        c.A0 = c.V0 + c.A0;
        c.V1 = mem.ReadU16((c.A3 + 0xAu));
        c.S0 = mem.ReadU32(c.A2);
        c.S2 = mem.ReadU32(c.A0);
        c.A1 = c.V0 + c.V1;
        c.S1 = mem.ReadU32(c.A1);
        RecompOne.Runtime.Gte.Write(12, c.S0);
        RecompOne.Runtime.Gte.Write(14, c.S2);
        RecompOne.Runtime.Gte.Write(13, c.S1);
        RecompOne.Runtime.Gte.Nclip();
        c.V0 = mem.ReadU8((c.A3 - 0x1u));
        c.V0 = c.V0 | 0x003Cu;
        mem.WriteU8((c.T0 + 0x47u), (byte)c.V0);
        c.V0 = c.T0 + 0x70u;
        mem.WriteU32(c.V0, (RecompOne.Runtime.Gte.Read(24)));
        c.V1 = mem.ReadU16((c.A3 + 0x12u));
        c.V0 = mem.ReadU32((c.T0 + 0x34u));
        c.S0 = mem.ReadU32((c.T0 + 0x10u));
        c.T2 = c.V0 + c.V1;
        mem.WriteU32((c.S0 + 0x8u), (RecompOne.Runtime.Gte.Read(12)));
        mem.WriteU32((c.S0 + 0x14u), (RecompOne.Runtime.Gte.Read(13)));
        mem.WriteU32((c.S0 + 0x20u), (RecompOne.Runtime.Gte.Read(14)));
        c.S1 = mem.ReadU32(c.T2);
        RecompOne.Runtime.Gte.Write(12, c.S1);
        RecompOne.Runtime.Gte.Nclip();
        c.V0 = c.T0 + 0x74u;
        mem.WriteU32(c.V0, (RecompOne.Runtime.Gte.Read(24)));
        c.V0 = mem.ReadU32((c.T0 + 0x70u));
        if ((int)c.V0 > 0) {
            goto L800207F8;
        }
        c.V0 = mem.ReadU32((c.T0 + 0x74u));
        if ((int)c.V0 >= 0) {
            goto L800208EC;
        }
        L800207F8: ;
        c.V0 = mem.ReadU32((c.T0 + 0x10u));
        c.V0 = c.V0 + 0x2Cu;
        mem.WriteU32(c.V0, (RecompOne.Runtime.Gte.Read(12)));
        c.V0 = (uint)(short)mem.ReadU16((c.A2 + 0x4u));
        mem.WriteU32((c.T0 + 0x84u), c.V0);
        mem.WriteU32((c.T0 + 0x80u), c.V0);
        c.V1 = (uint)(short)mem.ReadU16((c.A1 + 0x4u));
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L80020834;
        }
        mem.WriteU32((c.T0 + 0x84u), c.V1);
        c.V1 = (uint)(short)mem.ReadU16((c.A1 + 0x4u));
        L80020834: ;
        c.V0 = mem.ReadU32((c.T0 + 0x80u));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8002084C;
        }
        mem.WriteU32((c.T0 + 0x80u), c.V1);
        L8002084C: ;
        c.V1 = (uint)(short)mem.ReadU16((c.A0 + 0x4u));
        c.V0 = mem.ReadU32((c.T0 + 0x84u));
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8002086C;
        }
        mem.WriteU32((c.T0 + 0x84u), c.V1);
        c.V1 = (uint)(short)mem.ReadU16((c.A0 + 0x4u));
        L8002086C: ;
        c.V0 = mem.ReadU32((c.T0 + 0x80u));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L80020884;
        }
        mem.WriteU32((c.T0 + 0x80u), c.V1);
        L80020884: ;
        c.V1 = (uint)(short)mem.ReadU16((c.T2 + 0x4u));
        c.V0 = mem.ReadU32((c.T0 + 0x84u));
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L800208A4;
        }
        mem.WriteU32((c.T0 + 0x84u), c.V1);
        c.V1 = (uint)(short)mem.ReadU16((c.T2 + 0x4u));
        L800208A4: ;
        c.V0 = mem.ReadU32((c.T0 + 0x80u));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L800208BC;
        }
        mem.WriteU32((c.T0 + 0x80u), c.V1);
        L800208BC: ;
        c.V0 = mem.ReadU32((c.T0 + 0x84u));
        c.V1 = (uint)(sbyte)mem.ReadU8((c.A3 - 0x2u));
        c.V0 = (uint)((int)c.V0 >> 5);
        c.V1 = c.V0 + c.V1;
        c.V0 = (int)c.V1 < 4096 ? 1u : 0u;
        if (c.V0 == 0u) {
            mem.WriteU32((c.T0 + 0x88u), c.V1);
            goto L800208EC;
        }
        mem.WriteU32((c.T0 + 0x88u), c.V1);
        c.V0 = mem.ReadU32((c.T0 + 0x8Cu));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L800208F8;
        }
        L800208EC: ;
        c.A3 = c.A3 + 0x20u;
        c.T1 = c.T1 + 0x20u;
        goto L80020A30;
        L800208F8: ;
        c.A2 = mem.ReadU32((c.T0 + 0x10u));
        c.V0 = mem.ReadU32((c.T0 + 0x14u));
        c.V1 = c.A2 + 0x34u;
        c.V0 = c.V0 < c.V1 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L80020A68;
        }
        c.V0 = mem.ReadU32((c.T0 + 0x1BCu));
        mem.WriteU32((c.T0 + 0x10u), c.V1);
        c.V0 = c.V0 + 0x1u;
        mem.WriteU32((c.T0 + 0x1BCu), c.V0);
        c.V0 = mem.ReadU16((c.A3 - 0xAu));
        mem.WriteU16((c.A2 + 0xEu), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.A3 - 0x6u));
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
        c.V1 = mem.ReadU16((c.A3 + 0x10u));
        c.V0 = mem.ReadU32((c.T0 + 0x24u));
        c.V0 = c.V0 + c.V1;
        RecompOne.Runtime.Gte.Write(0, (mem.ReadU32(c.V0)));
        RecompOne.Runtime.Gte.Write(1, (mem.ReadU32((c.V0 + 0x4u))));
        RecompOne.Runtime.Gte.NccsOp(12, true);
        c.V0 = c.A2 + 0x28u;
        mem.WriteU32(c.V0, (RecompOne.Runtime.Gte.Read(22)));
        c.V0 = mem.ReadU32(c.T1);
        mem.WriteU32((c.A2 + 0xCu), c.V0);
        c.V0 = mem.ReadU32((c.A3 - 0x8u));
        mem.WriteU32((c.A2 + 0x18u), c.V0);
        c.V0 = mem.ReadU16((c.A3 - 0x4u));
        c.T1 = c.T1 + 0x20u;
        mem.WriteU16((c.A2 + 0x24u), (ushort)c.V0);
        c.V0 = mem.ReadU16(c.A3);
        c.A3 = c.A3 + 0x20u;
        mem.WriteU16((c.A2 + 0x30u), (ushort)c.V0);
        c.V0 = mem.ReadU32((c.T0 + 0x88u));
        c.V1 = mem.ReadU32(c.T0);
        c.V0 = c.V0 << 2;
        c.V0 = c.V0 + c.V1;
        c.V0 = mem.ReadU32(c.V0);
        c.V1 = 0x0C000000u;
        c.V0 = c.V0 & c.T7;
        c.V0 = c.V0 | c.V1;
        mem.WriteU32(c.A2, c.V0);
        c.V0 = mem.ReadU32((c.T0 + 0x88u));
        c.V1 = mem.ReadU32(c.T0);
        c.V0 = c.V0 << 2;
        c.V0 = c.V0 + c.V1;
        c.V1 = c.A2 & c.T7;
        mem.WriteU32(c.V0, c.V1);
        L80020A30: ;
        c.V0 = mem.ReadU32((c.T0 + 0x184u));
        c.V0 = c.V0 - 0x1u;
        if (c.V0 != c.T8) {
            mem.WriteU32((c.T0 + 0x184u), c.V0);
            goto L80020744;
        }
        mem.WriteU32((c.T0 + 0x184u), c.V0);
        L80020A44: ;
        c.V0 = mem.ReadU32((c.T0 + 0x20u));
        c.V0 = mem.ReadU32((c.V0 + 0x4u));
        L80020A50: ;
        c.V1 = mem.ReadU32((c.T0 + 0x34u));
        c.V0 = c.V0 << 3;
        c.V1 = c.V1 + c.V0;
        c.T3 = c.T3 - 0x1u;
        if (c.T3 != c.T8) {
            mem.WriteU32((c.T0 + 0x34u), c.V1);
            goto L800205C4;
        }
        mem.WriteU32((c.T0 + 0x34u), c.V1);
        L80020A68: ;
        c.S2 = mem.ReadU32((c.SP + 0x8u));
        c.S1 = mem.ReadU32((c.SP + 0x4u));
        c.S0 = mem.ReadU32(c.SP);
        c.SP = c.SP + 0x10u;
        return;
    }
}
