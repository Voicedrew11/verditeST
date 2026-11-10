using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_8001DB64</c>: the map's GT4s, every visible cell's quads. <c>ST_MAPQUADS</c>.
/// </summary>
public static partial class MapQuads
{
    public static readonly Literal R = new(typeof(MapQuads), "map quads", "MAPQUADS", 0x8001DB64, Run,
        "the map's GT4s, every visible cell's quads.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x388u;
        mem.WriteU32((c.SP + 0x380u), c.FP);
        c.FP = 0u;
        c.V1 = 0x00010000u;
        c.V0 = 0x801C0000u;
        c.V0 = mem.ReadU16((c.V0 - 0x61ECu));
        c.V1 = c.V1 | 0x8000u;
        mem.WriteU32((c.SP + 0x384u), c.RA);
        mem.WriteU32((c.SP + 0x37Cu), c.S7);
        mem.WriteU32((c.SP + 0x378u), c.S6);
        mem.WriteU32((c.SP + 0x374u), c.S5);
        mem.WriteU32((c.SP + 0x370u), c.S4);
        mem.WriteU32((c.SP + 0x36Cu), c.S3);
        mem.WriteU32((c.SP + 0x368u), c.S2);
        mem.WriteU32((c.SP + 0x364u), c.S1);
        mem.WriteU32((c.SP + 0x360u), c.S0);
        mem.WriteU32((c.SP + 0x388u), c.A0);
        mem.WriteU32((c.SP + 0x348u), c.V0);
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.V0 = c.V0 + c.V1;
        c.V1 = mem.ReadU8((c.V0 + 0x6DFCu));
        c.V0 = c.V1 & 0x0002u;
        if (c.V0 == 0u) {
            c.S0 = 0x1F800000u;
            goto L8001DBD8;
        }
        c.S0 = 0x1F800000u;
        c.V0 = 0x80020000u;
        c.FP = c.V0 - 0x2520u;
        goto L8001DBE8;
        L8001DBD8: ;
        c.V0 = c.V1 & 0x0004u;
        if (c.V0 == 0u) {
            c.V0 = 0x80020000u;
            goto L8001DBE8;
        }
        c.V0 = 0x80020000u;
        c.FP = c.V0 - 0x24E0u;
        L8001DBE8: ;
        c.A3 = mem.ReadU32((c.SP + 0x348u));
        c.V0 = 0xFFFFFFFFu;
        c.A3 = c.A3 - 0x1u;
        if (c.A3 == c.V0) {
            mem.WriteU32((c.SP + 0x348u), c.A3);
            goto L8001E4E8;
        }
        mem.WriteU32((c.SP + 0x348u), c.A3);
        c.T0 = c.SP + 0x34u;
        c.T1 = c.SP + 0x4Cu;
        c.A3 = c.SP + 0x64u;
        mem.WriteU32((c.SP + 0x350u), c.T0);
        c.T0 = c.SP + 0x7Cu;
        mem.WriteU32((c.SP + 0x354u), c.T1);
        mem.WriteU32((c.SP + 0x358u), c.A3);
        mem.WriteU32((c.SP + 0x35Cu), c.T0);
        L8001DC1C: ;
        Poll(c, mem);
        c.V1 = 0x801B0000u;
        c.T1 = mem.ReadU32((c.SP + 0x348u));
        c.V1 = c.V1 + 0x5948u;
        c.V0 = c.T1 << 1;
        c.V0 = c.V0 + c.V1;
        c.V0 = mem.ReadU16((c.V0 + 0x45CEu));
        c.V1 = c.V0 << 1;
        c.V1 = c.V1 + c.V0;
        c.V1 = c.V1 << 2;
        c.V0 = 0x80180000u;
        c.V0 = c.V0 - 0x4130u;
        c.A0 = c.V1 + c.V0;
        c.V0 = mem.ReadU8((c.A0 + 0x4u));
        c.V0 = c.V0 & 0x0003u;
        if (c.V0 != 0u) {
            goto L8001DC94;
        }
        c.V1 = mem.ReadU8(c.A0);
        c.V0 = c.V1 << 2;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 3;
        c.V1 = mem.ReadU32((c.S0 + 0xCu));
        c.V0 = c.V0 + 0xCu;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32((c.S0 + 0x20u), c.V1);
        c.V0 = mem.ReadU32((c.V1 + 0x4u));
        goto L8001E4C4;
        L8001DC94: ;
        c.V0 = mem.ReadU8((c.A0 + 0x3u));
        c.V0 = c.V0 & 0x0003u;
        c.V1 = c.V0 << 2;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32((c.S0 + 0x1E4u), c.V0);
        c.V0 = mem.ReadU32((c.S0 + 0x3C4u));
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
        c.V1 = mem.ReadU32((c.S0 + 0xCu));
        c.V0 = c.V0 + 0xCu;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32((c.S0 + 0x20u), c.V1);
        c.V1 = mem.ReadU32(c.V1);
        c.V0 = mem.ReadU32((c.S0 + 0xCu));
        c.A0 = mem.ReadU32((c.S0 + 0x20u));
        c.V1 = c.V1 + 0xCu;
        c.V0 = c.V0 + c.V1;
        mem.WriteU32((c.S0 + 0x3Cu), c.V0);
        c.V1 = mem.ReadU32((c.A0 + 0x8u));
        c.V0 = mem.ReadU32((c.S0 + 0xCu));
        c.V1 = c.V1 + 0xCu;
        c.V0 = c.V0 + c.V1;
        mem.WriteU32((c.S0 + 0x24u), c.V0);
        c.V0 = mem.ReadU32((c.A0 + 0x10u));
        c.A3 = mem.ReadU16((c.A0 + 0x1Cu));
        c.V0 = c.V0 + 0xCu;
        mem.WriteU32((c.SP + 0x34Cu), c.A3);
        c.V1 = mem.ReadU32((c.S0 + 0xCu));
        c.A0 = mem.ReadU16((c.A0 + 0x18u));
        c.S5 = c.V1 + c.V0;
        c.V0 = c.A0 << 2;
        c.V0 = c.V0 + c.A0;
        c.V0 = c.V0 << 2;
        c.S5 = c.S5 + c.V0;
        c.V1 = mem.ReadU32((c.S0 + 0x1B4u));
        c.V0 = c.A3;
        c.V1 = c.V1 + c.A3;
        c.A3 = c.A3 - 0x1u;
        mem.WriteU32((c.SP + 0x34Cu), c.A3);
        if (c.V0 == 0u) {
            mem.WriteU32((c.S0 + 0x1B4u), c.V1);
            goto L8001E4B8;
        }
        mem.WriteU32((c.S0 + 0x1B4u), c.V1);
        c.S2 = c.S5 + 0xCu;
        L8001DD7C: ;
        Poll(c, mem);
        c.V1 = mem.ReadU16((c.S2 + 0x6u));
        c.V0 = mem.ReadU32((c.S0 + 0x34u));
        c.A0 = mem.ReadU16((c.S2 + 0xAu));
        c.S4 = c.V0 + c.V1;
        c.S6 = c.V0 + c.A0;
        c.V1 = mem.ReadU16((c.S2 + 0x8u));
        c.T0 = mem.ReadU32(c.S4);
        c.A3 = mem.ReadU32(c.S6);
        c.S3 = c.V0 + c.V1;
        c.T1 = mem.ReadU32(c.S3);
        RecompOne.Runtime.Gte.Write(12, c.T0);
        RecompOne.Runtime.Gte.Write(14, c.A3);
        RecompOne.Runtime.Gte.Write(13, c.T1);
        RecompOne.Runtime.Gte.Nclip();
        c.V0 = mem.ReadU8((c.S2 - 0x1u));
        c.V0 = c.V0 | 0x003Cu;
        mem.WriteU8((c.S0 + 0x47u), (byte)c.V0);
        c.V0 = c.S0 + 0x70u;
        mem.WriteU32(c.V0, (RecompOne.Runtime.Gte.Read(24)));
        c.V1 = mem.ReadU16((c.S2 + 0xCu));
        c.V0 = mem.ReadU32((c.S0 + 0x34u));
        c.T0 = mem.ReadU32((c.S0 + 0x10u));
        c.S7 = c.V0 + c.V1;
        mem.WriteU32((c.T0 + 0x8u), (RecompOne.Runtime.Gte.Read(12)));
        mem.WriteU32((c.T0 + 0x14u), (RecompOne.Runtime.Gte.Read(13)));
        mem.WriteU32((c.T0 + 0x20u), (RecompOne.Runtime.Gte.Read(14)));
        c.T1 = mem.ReadU32(c.S7);
        RecompOne.Runtime.Gte.Write(12, c.T1);
        RecompOne.Runtime.Gte.Nclip();
        c.V0 = c.S0 + 0x74u;
        mem.WriteU32(c.V0, (RecompOne.Runtime.Gte.Read(24)));
        c.V0 = mem.ReadU32((c.S0 + 0x70u));
        if ((int)c.V0 > 0) {
            goto L8001DE30;
        }
        c.V0 = mem.ReadU32((c.S0 + 0x74u));
        if ((int)c.V0 >= 0) {
            goto L8001E498;
        }
        L8001DE30: ;
        c.V0 = mem.ReadU32((c.S0 + 0x10u));
        c.V0 = c.V0 + 0x2Cu;
        mem.WriteU32(c.V0, (RecompOne.Runtime.Gte.Read(12)));
        c.V0 = (uint)(short)mem.ReadU16((c.S4 + 0x4u));
        mem.WriteU32((c.S0 + 0x84u), c.V0);
        mem.WriteU32((c.S0 + 0x80u), c.V0);
        c.V1 = (uint)(short)mem.ReadU16((c.S3 + 0x4u));
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001DE6C;
        }
        mem.WriteU32((c.S0 + 0x84u), c.V1);
        c.V1 = (uint)(short)mem.ReadU16((c.S3 + 0x4u));
        L8001DE6C: ;
        c.V0 = mem.ReadU32((c.S0 + 0x80u));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001DE84;
        }
        mem.WriteU32((c.S0 + 0x80u), c.V1);
        L8001DE84: ;
        c.V1 = (uint)(short)mem.ReadU16((c.S6 + 0x4u));
        c.V0 = mem.ReadU32((c.S0 + 0x84u));
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001DEA4;
        }
        mem.WriteU32((c.S0 + 0x84u), c.V1);
        c.V1 = (uint)(short)mem.ReadU16((c.S6 + 0x4u));
        L8001DEA4: ;
        c.V0 = mem.ReadU32((c.S0 + 0x80u));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001DEBC;
        }
        mem.WriteU32((c.S0 + 0x80u), c.V1);
        L8001DEBC: ;
        c.V1 = (uint)(short)mem.ReadU16((c.S7 + 0x4u));
        c.V0 = mem.ReadU32((c.S0 + 0x84u));
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001DEDC;
        }
        mem.WriteU32((c.S0 + 0x84u), c.V1);
        c.V1 = (uint)(short)mem.ReadU16((c.S7 + 0x4u));
        L8001DEDC: ;
        c.V0 = mem.ReadU32((c.S0 + 0x80u));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001DEF4;
        }
        mem.WriteU32((c.S0 + 0x80u), c.V1);
        L8001DEF4: ;
        c.V0 = mem.ReadU32((c.S0 + 0x84u));
        c.V1 = (uint)(sbyte)mem.ReadU8((c.S2 - 0x2u));
        c.V0 = (uint)((int)c.V0 >> 5);
        c.V1 = c.V0 + c.V1;
        c.V0 = (int)c.V1 < 4096 ? 1u : 0u;
        if (c.V0 == 0u) {
            mem.WriteU32((c.S0 + 0x88u), c.V1);
            goto L8001E498;
        }
        mem.WriteU32((c.S0 + 0x88u), c.V1);
        c.V0 = mem.ReadU32((c.S0 + 0x8Cu));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L8001E498;
        }
        c.V0 = mem.ReadU32((c.S0 + 0x70u));
        c.V0 = (int)c.V0 < 16385 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.V0 = 0x00000001u;
            goto L8001DF4C;
        }
        c.V0 = 0x00000001u;
        c.V0 = mem.ReadU32((c.S0 + 0x74u));
        c.V0 = (int)c.V0 < -16384 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.V0 = 0x00000001u;
            goto L8001E304;
        }
        c.V0 = 0x00000001u;
        L8001DF4C: ;
        mem.WriteU32((c.SP + 0x10u), c.V0);
        c.V1 = mem.ReadU32((c.S0 + 0x70u));
        c.V0 = 0u | 0x8000u;
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 != 0u) {
            c.V0 = 0x00000002u;
            goto L8001DF78;
        }
        c.V0 = 0x00000002u;
        c.V0 = mem.ReadU32((c.S0 + 0x74u));
        c.V0 = (int)c.V0 < -32768 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.V0 = 0x00000002u;
            goto L8001DF7C;
        }
        c.V0 = 0x00000002u;
        L8001DF78: ;
        mem.WriteU32((c.SP + 0x10u), c.V0);
        L8001DF7C: ;
        NearQuad(c, mem);
        goto L8001E4A0;
        L8001E304: ;
        c.A1 = mem.ReadU32((c.S0 + 0x10u));
        c.V0 = mem.ReadU32((c.S0 + 0x14u));
        c.V1 = c.A1 + 0x34u;
        c.V0 = c.V0 < c.V1 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L8001E4E8;
        }
        Quad(c, mem);
        L8001E498: ;
        c.S2 = c.S2 + 0x1Cu;
        c.S5 = c.S5 + 0x1Cu;
        L8001E4A0: ;
        c.V0 = mem.ReadU32((c.SP + 0x34Cu));
        c.T0 = c.V0;
        c.T0 = c.T0 - 0x1u;
        if (c.V0 != 0u) {
            mem.WriteU32((c.SP + 0x34Cu), c.T0);
            goto L8001DD7C;
        }
        mem.WriteU32((c.SP + 0x34Cu), c.T0);
        L8001E4B8: ;
        c.V0 = mem.ReadU32((c.S0 + 0x20u));
        c.V0 = mem.ReadU32((c.V0 + 0x4u));
        L8001E4C4: ;
        c.V1 = mem.ReadU32((c.S0 + 0x34u));
        c.V0 = c.V0 << 3;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32((c.S0 + 0x34u), c.V1);
        c.T1 = mem.ReadU32((c.SP + 0x348u));
        c.V0 = 0xFFFFFFFFu;
        c.T1 = c.T1 - 0x1u;
        if (c.T1 != c.V0) {
            mem.WriteU32((c.SP + 0x348u), c.T1);
            goto L8001DC1C;
        }
        mem.WriteU32((c.SP + 0x348u), c.T1);
        L8001E4E8: ;
        c.RA = mem.ReadU32((c.SP + 0x384u));
        c.FP = mem.ReadU32((c.SP + 0x380u));
        c.S7 = mem.ReadU32((c.SP + 0x37Cu));
        c.S6 = mem.ReadU32((c.SP + 0x378u));
        c.S5 = mem.ReadU32((c.SP + 0x374u));
        c.S4 = mem.ReadU32((c.SP + 0x370u));
        c.S3 = mem.ReadU32((c.SP + 0x36Cu));
        c.S2 = mem.ReadU32((c.SP + 0x368u));
        c.S1 = mem.ReadU32((c.SP + 0x364u));
        c.S0 = mem.ReadU32((c.SP + 0x360u));
        c.SP = c.SP + 0x388u;
        return;
    }

}
