using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_8001E5A0</c>: a scene step after the map's quads; wrote no packet in map 0. <c>ST_SCENESTEP1E5A0</c>.
/// </summary>
public static partial class SceneStep1E5A0
{
    public static readonly Literal R = new(typeof(SceneStep1E5A0), "scene step 1E5A0", "SCENESTEP1E5A0", 0x8001E5A0, Run,
        "a scene step after the map's quads; wrote no packet in map 0.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x260u;
        mem.WriteU32((c.SP + 0x254u), c.S7);
        c.S7 = 0u;
        c.V1 = 0x00010000u;
        c.V1 = c.V1 | 0x8000u;
        c.V0 = 0x801C0000u;
        mem.WriteU32((c.SP + 0x258u), c.FP);
        c.FP = mem.ReadU16((c.V0 - 0x61ECu));
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.V0 = c.V0 + c.V1;
        mem.WriteU32((c.SP + 0x25Cu), c.RA);
        mem.WriteU32((c.SP + 0x250u), c.S6);
        mem.WriteU32((c.SP + 0x24Cu), c.S5);
        mem.WriteU32((c.SP + 0x248u), c.S4);
        mem.WriteU32((c.SP + 0x244u), c.S3);
        mem.WriteU32((c.SP + 0x240u), c.S2);
        mem.WriteU32((c.SP + 0x23Cu), c.S1);
        mem.WriteU32((c.SP + 0x238u), c.S0);
        mem.WriteU32((c.SP + 0x260u), c.A0);
        c.V1 = mem.ReadU8((c.V0 + 0x6DFCu));
        c.V0 = c.V1 & 0x0002u;
        if (c.V0 == 0u) {
            c.S2 = 0x1F800000u;
            goto L8001E610;
        }
        c.S2 = 0x1F800000u;
        c.V0 = 0x80020000u;
        c.S7 = c.V0 - 0x1AE4u;
        goto L8001E620;
        L8001E610: ;
        c.V0 = c.V1 & 0x0004u;
        if (c.V0 == 0u) {
            c.V0 = 0x80020000u;
            goto L8001E620;
        }
        c.V0 = 0x80020000u;
        c.S7 = c.V0 - 0x1AA4u;
        L8001E620: ;
        c.FP = c.FP - 0x1u;
        c.V0 = 0xFFFFFFFFu;
        if (c.FP == c.V0) {
            c.A3 = c.SP + 0x34u;
            goto L8001EDF4;
        }
        c.A3 = c.SP + 0x34u;
        c.T0 = c.SP + 0x4Cu;
        c.T1 = c.SP + 0x64u;
        mem.WriteU32((c.SP + 0x228u), c.A3);
        mem.WriteU32((c.SP + 0x22Cu), c.T0);
        mem.WriteU32((c.SP + 0x230u), c.T1);
        L8001E644: ;
        Poll(c, mem);
        c.V1 = 0x801B0000u;
        c.V1 = c.V1 + 0x5948u;
        c.V0 = c.FP << 1;
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
            goto L8001E6B8;
        }
        c.V1 = mem.ReadU8(c.A0);
        c.V0 = c.V1 << 2;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 3;
        c.V1 = mem.ReadU32((c.S2 + 0xCu));
        c.V0 = c.V0 + 0xCu;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32((c.S2 + 0x20u), c.V1);
        c.V0 = mem.ReadU32((c.V1 + 0x4u));
        goto L8001EDD8;
        L8001E6B8: ;
        After8001E6B8(c, mem);
        if (c.V1 == c.V0) {
            goto L8001EDCC;
        }
        c.S1 = c.S4 + 0x14u;
        L8001E7C0: ;
        Poll(c, mem);
        c.V1 = mem.ReadU16((c.S1 - 0x6u));
        c.V0 = mem.ReadU32((c.S2 + 0x34u));
        c.A0 = mem.ReadU16((c.S1 + 0x2u));
        c.S5 = c.V0 + c.V1;
        c.S6 = c.V0 + c.A0;
        c.V1 = mem.ReadU16((c.S1 - 0x2u));
        c.A3 = mem.ReadU32(c.S5);
        c.T1 = mem.ReadU32(c.S6);
        c.S3 = c.V0 + c.V1;
        c.T0 = mem.ReadU32(c.S3);
        RecompOne.Runtime.Gte.Write(12, c.A3);
        RecompOne.Runtime.Gte.Write(14, c.T1);
        RecompOne.Runtime.Gte.Write(13, c.T0);
        RecompOne.Runtime.Gte.Nclip();
        c.V0 = mem.ReadU8((c.S1 - 0x9u));
        c.V0 = c.V0 | 0x0034u;
        mem.WriteU8((c.S2 + 0x47u), (byte)c.V0);
        c.V0 = c.S2 + 0x70u;
        mem.WriteU32(c.V0, (RecompOne.Runtime.Gte.Read(24)));
        c.V0 = mem.ReadU32((c.S2 + 0x70u));
        if ((int)c.V0 <= 0) {
            goto L8001EDB0;
        }
        c.A3 = mem.ReadU32((c.S2 + 0x10u));
        mem.WriteU32((c.A3 + 0x8u), (RecompOne.Runtime.Gte.Read(12)));
        mem.WriteU32((c.A3 + 0x14u), (RecompOne.Runtime.Gte.Read(13)));
        mem.WriteU32((c.A3 + 0x20u), (RecompOne.Runtime.Gte.Read(14)));
        c.V0 = (uint)(short)mem.ReadU16((c.S5 + 0x4u));
        mem.WriteU32((c.S2 + 0x84u), c.V0);
        mem.WriteU32((c.S2 + 0x80u), c.V0);
        c.V1 = (uint)(short)mem.ReadU16((c.S3 + 0x4u));
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001E868;
        }
        mem.WriteU32((c.S2 + 0x84u), c.V1);
        c.V1 = (uint)(short)mem.ReadU16((c.S3 + 0x4u));
        L8001E868: ;
        c.V0 = mem.ReadU32((c.S2 + 0x80u));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001E880;
        }
        mem.WriteU32((c.S2 + 0x80u), c.V1);
        L8001E880: ;
        c.V1 = (uint)(short)mem.ReadU16((c.S6 + 0x4u));
        c.V0 = mem.ReadU32((c.S2 + 0x84u));
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001E8A0;
        }
        mem.WriteU32((c.S2 + 0x84u), c.V1);
        c.V1 = (uint)(short)mem.ReadU16((c.S6 + 0x4u));
        L8001E8A0: ;
        c.V0 = mem.ReadU32((c.S2 + 0x80u));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001E8B8;
        }
        mem.WriteU32((c.S2 + 0x80u), c.V1);
        L8001E8B8: ;
        c.V0 = mem.ReadU32((c.S2 + 0x84u));
        c.V1 = (uint)(sbyte)mem.ReadU8((c.S1 - 0xAu));
        c.V0 = (uint)((int)c.V0 >> 5);
        c.V1 = c.V0 + c.V1;
        c.V0 = (int)c.V1 < 4096 ? 1u : 0u;
        if (c.V0 == 0u) {
            mem.WriteU32((c.S2 + 0x88u), c.V1);
            goto L8001EDB0;
        }
        mem.WriteU32((c.S2 + 0x88u), c.V1);
        c.V0 = mem.ReadU32((c.S2 + 0x8Cu));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L8001EDB0;
        }
        c.V0 = mem.ReadU32((c.S2 + 0x70u));
        c.V0 = (int)c.V0 < 16385 ? 1u : 0u;
        if (c.V0 != 0u) {
            c.V0 = 0x00000001u;
            goto L8001EC3C;
        }
        c.V0 = 0x00000001u;
        mem.WriteU32((c.SP + 0x10u), c.V0);
        c.V1 = mem.ReadU32((c.S2 + 0x70u));
        c.V0 = 0u | 0x8000u;
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.V0 = 0x00000002u;
            goto L8001E930;
        }
        c.V0 = 0x00000002u;
        mem.WriteU32((c.SP + 0x10u), c.V0);
        c.V1 = mem.ReadU32((c.S2 + 0x70u));
        c.V0 = 0x00010000u;
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.V0 = 0x00000003u;
            goto L8001E930;
        }
        c.V0 = 0x00000003u;
        mem.WriteU32((c.SP + 0x10u), c.V0);
        L8001E930: ;
        After8001E930(c, mem);
        if (c.V0 == 0u) {
            goto L8001EA60;
        }
        c.A0 = mem.ReadU32((c.SP + 0x228u));
        c.RA = 0x8001EA48u;
        Dispatcher.Call(c, mem, c.S7);
        c.A0 = mem.ReadU32((c.SP + 0x22Cu));
        c.RA = 0x8001EA54u;
        Dispatcher.Call(c, mem, c.S7);
        c.A0 = mem.ReadU32((c.SP + 0x230u));
        c.RA = 0x8001EA60u;
        Dispatcher.Call(c, mem, c.S7);
        L8001EA60: ;
        After8001EA60(c, mem);
        goto L8001EDB8;
        L8001EC3C: ;
        c.A1 = mem.ReadU32((c.S2 + 0x10u));
        c.V0 = mem.ReadU32((c.S2 + 0x14u));
        c.V1 = c.A1 + 0x28u;
        c.V0 = c.V0 < c.V1 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L8001EDF4;
        }
        After8001EC3C(c, mem);
        if (c.V0 == 0u) {
            goto L8001EDB0;
        }
        c.A0 = c.A2;
        c.RA = 0x8001EDA0u;
        Dispatcher.Call(c, mem, c.S7);
        c.A0 = c.S3;
        c.RA = 0x8001EDA8u;
        Dispatcher.Call(c, mem, c.S7);
        c.A0 = c.S0;
        c.RA = 0x8001EDB0u;
        Dispatcher.Call(c, mem, c.S7);
        L8001EDB0: ;
        c.S1 = c.S1 + 0x18u;
        c.S4 = c.S4 + 0x18u;
        L8001EDB8: ;
        c.V1 = mem.ReadU32((c.S2 + 0x184u));
        c.V0 = 0xFFFFFFFFu;
        c.V1 = c.V1 - 0x1u;
        if (c.V1 != c.V0) {
            mem.WriteU32((c.S2 + 0x184u), c.V1);
            goto L8001E7C0;
        }
        mem.WriteU32((c.S2 + 0x184u), c.V1);
        L8001EDCC: ;
        c.V0 = mem.ReadU32((c.S2 + 0x20u));
        c.V0 = mem.ReadU32((c.V0 + 0x4u));
        L8001EDD8: ;
        c.V1 = mem.ReadU32((c.S2 + 0x34u));
        c.V0 = c.V0 << 3;
        c.V1 = c.V1 + c.V0;
        c.FP = c.FP - 0x1u;
        c.V0 = 0xFFFFFFFFu;
        if (c.FP != c.V0) {
            mem.WriteU32((c.S2 + 0x34u), c.V1);
            goto L8001E644;
        }
        mem.WriteU32((c.S2 + 0x34u), c.V1);
        L8001EDF4: ;
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
