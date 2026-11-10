using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_8001D2A8</c>: the map's GT3s, every visible cell's triangles. <c>ST_MAPTRIANGLES</c>.
/// </summary>
public static partial class MapTriangles
{
    public static readonly Literal R = new(typeof(MapTriangles), "map triangles", "MAPTRIANGLES", 0x8001D2A8, Run,
        "the map's GT3s, every visible cell's triangles.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x260u;
        mem.WriteU32((c.SP + 0x258u), c.FP);
        c.FP = 0u;
        c.V1 = 0x00010000u;
        c.V0 = 0x801C0000u;
        c.V0 = mem.ReadU16((c.V0 - 0x61ECu));
        c.V1 = c.V1 | 0x8000u;
        mem.WriteU32((c.SP + 0x25Cu), c.RA);
        mem.WriteU32((c.SP + 0x254u), c.S7);
        mem.WriteU32((c.SP + 0x250u), c.S6);
        mem.WriteU32((c.SP + 0x24Cu), c.S5);
        mem.WriteU32((c.SP + 0x248u), c.S4);
        mem.WriteU32((c.SP + 0x244u), c.S3);
        mem.WriteU32((c.SP + 0x240u), c.S2);
        mem.WriteU32((c.SP + 0x23Cu), c.S1);
        mem.WriteU32((c.SP + 0x238u), c.S0);
        mem.WriteU32((c.SP + 0x260u), c.A0);
        mem.WriteU32((c.SP + 0x228u), c.V0);
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.V0 = c.V0 + c.V1;
        c.V1 = mem.ReadU8((c.V0 + 0x6DFCu));
        c.V0 = c.V1 & 0x0002u;
        if (c.V0 == 0u) {
            c.S1 = 0x1F800000u;
            goto L8001D31C;
        }
        c.S1 = 0x1F800000u;
        c.V0 = 0x80020000u;
        c.FP = c.V0 - 0x2DDCu;
        goto L8001D32C;
        L8001D31C: ;
        c.V0 = c.V1 & 0x0004u;
        if (c.V0 == 0u) {
            c.V0 = 0x80020000u;
            goto L8001D32C;
        }
        c.V0 = 0x80020000u;
        c.FP = c.V0 - 0x2D9Cu;
        L8001D32C: ;
        c.A3 = mem.ReadU32((c.SP + 0x228u));
        c.V0 = 0xFFFFFFFFu;
        c.A3 = c.A3 - 0x1u;
        if (c.A3 == c.V0) {
            mem.WriteU32((c.SP + 0x228u), c.A3);
            goto L8001DAAC;
        }
        mem.WriteU32((c.SP + 0x228u), c.A3);
        c.T0 = c.SP + 0x34u;
        c.T1 = c.SP + 0x4Cu;
        c.A3 = c.SP + 0x64u;
        mem.WriteU32((c.SP + 0x22Cu), c.T0);
        mem.WriteU32((c.SP + 0x230u), c.T1);
        mem.WriteU32((c.SP + 0x234u), c.A3);
        L8001D358: ;
        Poll(c, mem);
        c.V1 = 0x801B0000u;
        c.T0 = mem.ReadU32((c.SP + 0x228u));
        c.V1 = c.V1 + 0x5948u;
        c.V0 = c.T0 << 1;
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
            goto L8001D3D0;
        }
        c.V1 = mem.ReadU8(c.A0);
        c.V0 = c.V1 << 2;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 3;
        c.V1 = mem.ReadU32((c.S1 + 0xCu));
        c.V0 = c.V0 + 0xCu;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32((c.S1 + 0x20u), c.V1);
        c.V0 = mem.ReadU32((c.V1 + 0x4u));
        goto L8001DA88;
        L8001D3D0: ;
        c.V0 = mem.ReadU8((c.A0 + 0x3u));
        c.V0 = c.V0 & 0x0003u;
        c.V1 = c.V0 << 2;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32((c.S1 + 0x1E4u), c.V0);
        c.V0 = mem.ReadU32((c.S1 + 0x3C4u));
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
        c.V1 = mem.ReadU32((c.S1 + 0xCu));
        c.V0 = c.V0 + 0xCu;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32((c.S1 + 0x20u), c.V1);
        c.V1 = mem.ReadU32(c.V1);
        c.V0 = mem.ReadU32((c.S1 + 0xCu));
        c.A0 = mem.ReadU32((c.S1 + 0x20u));
        c.V1 = c.V1 + 0xCu;
        c.V0 = c.V0 + c.V1;
        mem.WriteU32((c.S1 + 0x3Cu), c.V0);
        c.V1 = mem.ReadU32((c.A0 + 0x8u));
        c.V0 = mem.ReadU32((c.S1 + 0xCu));
        c.V1 = c.V1 + 0xCu;
        c.V0 = c.V0 + c.V1;
        mem.WriteU32((c.S1 + 0x24u), c.V0);
        c.V0 = mem.ReadU32((c.A0 + 0x10u));
        c.S7 = mem.ReadU16((c.A0 + 0x18u));
        c.A0 = mem.ReadU32((c.S1 + 0xCu));
        c.V1 = mem.ReadU32((c.S1 + 0x1B4u));
        c.V0 = c.V0 + 0xCu;
        c.S4 = c.A0 + c.V0;
        c.V1 = c.V1 + c.S7;
        c.V0 = c.S7;
        c.S7 = c.S7 - 0x1u;
        if (c.V0 == 0u) {
            mem.WriteU32((c.S1 + 0x1B4u), c.V1);
            goto L8001DA7C;
        }
        mem.WriteU32((c.S1 + 0x1B4u), c.V1);
        c.S2 = c.S4 + 0x8u;
        L8001D49C: ;
        Poll(c, mem);
        c.V1 = mem.ReadU16((c.S2 + 0x6u));
        c.V0 = mem.ReadU32((c.S1 + 0x34u));
        c.A0 = mem.ReadU16((c.S2 + 0xAu));
        c.S6 = c.V0 + c.V1;
        c.S5 = c.V0 + c.A0;
        c.V1 = mem.ReadU16((c.S2 + 0x8u));
        c.T1 = mem.ReadU32(c.S6);
        c.T0 = mem.ReadU32(c.S5);
        c.S3 = c.V0 + c.V1;
        c.A3 = mem.ReadU32(c.S3);
        RecompOne.Runtime.Gte.Write(12, c.T1);
        RecompOne.Runtime.Gte.Write(14, c.T0);
        RecompOne.Runtime.Gte.Write(13, c.A3);
        RecompOne.Runtime.Gte.Nclip();
        c.V0 = mem.ReadU8((c.S2 + 0x3u));
        c.V0 = c.V0 | 0x0034u;
        mem.WriteU8((c.S1 + 0x47u), (byte)c.V0);
        c.V0 = c.S1 + 0x70u;
        mem.WriteU32(c.V0, (RecompOne.Runtime.Gte.Read(24)));
        c.V0 = mem.ReadU32((c.S1 + 0x70u));
        if ((int)c.V0 <= 0) {
            goto L8001DA68;
        }
        c.T1 = mem.ReadU32((c.S1 + 0x10u));
        mem.WriteU32((c.T1 + 0x8u), (RecompOne.Runtime.Gte.Read(12)));
        mem.WriteU32((c.T1 + 0x14u), (RecompOne.Runtime.Gte.Read(13)));
        mem.WriteU32((c.T1 + 0x20u), (RecompOne.Runtime.Gte.Read(14)));
        c.V0 = (uint)(short)mem.ReadU16((c.S6 + 0x4u));
        mem.WriteU32((c.S1 + 0x84u), c.V0);
        mem.WriteU32((c.S1 + 0x80u), c.V0);
        c.V1 = (uint)(short)mem.ReadU16((c.S3 + 0x4u));
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001D544;
        }
        mem.WriteU32((c.S1 + 0x84u), c.V1);
        c.V1 = (uint)(short)mem.ReadU16((c.S3 + 0x4u));
        L8001D544: ;
        c.V0 = mem.ReadU32((c.S1 + 0x80u));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001D55C;
        }
        mem.WriteU32((c.S1 + 0x80u), c.V1);
        L8001D55C: ;
        c.V1 = (uint)(short)mem.ReadU16((c.S5 + 0x4u));
        c.V0 = mem.ReadU32((c.S1 + 0x84u));
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001D57C;
        }
        mem.WriteU32((c.S1 + 0x84u), c.V1);
        c.V1 = (uint)(short)mem.ReadU16((c.S5 + 0x4u));
        L8001D57C: ;
        c.V0 = mem.ReadU32((c.S1 + 0x80u));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001D594;
        }
        mem.WriteU32((c.S1 + 0x80u), c.V1);
        L8001D594: ;
        c.V0 = mem.ReadU32((c.S1 + 0x84u));
        c.V1 = (uint)(sbyte)mem.ReadU8((c.S2 + 0x2u));
        c.V0 = (uint)((int)c.V0 >> 5);
        c.V1 = c.V0 + c.V1;
        c.V0 = (int)c.V1 < 4096 ? 1u : 0u;
        if (c.V0 == 0u) {
            mem.WriteU32((c.S1 + 0x88u), c.V1);
            goto L8001DA68;
        }
        mem.WriteU32((c.S1 + 0x88u), c.V1);
        c.V0 = mem.ReadU32((c.S1 + 0x8Cu));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L8001DA68;
        }
        c.V0 = mem.ReadU32((c.S1 + 0x70u));
        c.V0 = (int)c.V0 < 16385 ? 1u : 0u;
        if (c.V0 != 0u) {
            c.V0 = 0x00000001u;
            goto L8001D904;
        }
        c.V0 = 0x00000001u;
        mem.WriteU32((c.SP + 0x10u), c.V0);
        c.V1 = mem.ReadU32((c.S1 + 0x70u));
        c.V0 = 0u | 0x8000u;
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.V0 = 0x00000002u;
            goto L8001D60C;
        }
        c.V0 = 0x00000002u;
        mem.WriteU32((c.SP + 0x10u), c.V0);
        c.V1 = mem.ReadU32((c.S1 + 0x70u));
        c.V0 = 0x00010000u;
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.V0 = 0x00000003u;
            goto L8001D60C;
        }
        c.V0 = 0x00000003u;
        mem.WriteU32((c.SP + 0x10u), c.V0);
        L8001D60C: ;
        NearTriangle(c, mem);
        goto L8001DA70;
        L8001D904: ;
        c.A1 = mem.ReadU32((c.S1 + 0x10u));
        c.V0 = mem.ReadU32((c.S1 + 0x14u));
        c.V1 = c.A1 + 0x28u;
        c.V0 = c.V0 < c.V1 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L8001DAAC;
        }
        Triangle(c, mem);
        L8001DA68: ;
        c.S2 = c.S2 + 0x14u;
        c.S4 = c.S4 + 0x14u;
        L8001DA70: ;
        c.V0 = c.S7;
        if (c.V0 != 0u) {
            c.S7 = c.S7 - 0x1u;
            goto L8001D49C;
        }
        c.S7 = c.S7 - 0x1u;
        L8001DA7C: ;
        c.V0 = mem.ReadU32((c.S1 + 0x20u));
        c.V0 = mem.ReadU32((c.V0 + 0x4u));
        L8001DA88: ;
        c.V1 = mem.ReadU32((c.S1 + 0x34u));
        c.V0 = c.V0 << 3;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32((c.S1 + 0x34u), c.V1);
        c.T0 = mem.ReadU32((c.SP + 0x228u));
        c.V0 = 0xFFFFFFFFu;
        c.T0 = c.T0 - 0x1u;
        if (c.T0 != c.V0) {
            mem.WriteU32((c.SP + 0x228u), c.T0);
            goto L8001D358;
        }
        mem.WriteU32((c.SP + 0x228u), c.T0);
        L8001DAAC: ;
        c.RA = mem.ReadU32((c.SP + 0x25Cu));
        c.FP = mem.ReadU32((c.SP + 0x258u));
        c.S7 = mem.ReadU32((c.SP + 0x254u));
        c.S6 = mem.ReadU32((c.SP + 0x250u));
        c.S5 = mem.ReadU32((c.SP + 0x24Cu));
        c.S4 = mem.ReadU32((c.SP + 0x248u));
        c.S3 = mem.ReadU32((c.SP + 0x244u));
        c.S2 = mem.ReadU32((c.SP + 0x240u));
        c.S1 = mem.ReadU32((c.SP + 0x23Cu));
        c.S0 = mem.ReadU32((c.SP + 0x238u));
        c.SP = c.SP + 0x260u;
        return;
    }

}
