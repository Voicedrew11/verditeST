using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_8001C60C</c>: the scene's second preparation pass; writes no packet. <c>ST_SCENEPREPARE2</c>.
/// </summary>
public static class ScenePrepare2
{
    public static readonly Literal R = new(typeof(ScenePrepare2), "scene preparation 2", "SCENEPREPARE2", 0x8001C60C, Run,
        "the scene's second preparation pass; writes no packet.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x38u;
        c.A0 = 0x00010000u;
        c.A0 = c.A0 | 0x8000u;
        c.V0 = 0x80160000u;
        c.A3 = c.V0 - 0x2F70u;
        c.A0 = c.A3 + c.A0;
        mem.WriteU32((c.SP + 0x34u), c.RA);
        mem.WriteU32((c.SP + 0x30u), c.FP);
        mem.WriteU32((c.SP + 0x2Cu), c.S7);
        mem.WriteU32((c.SP + 0x28u), c.S6);
        mem.WriteU32((c.SP + 0x24u), c.S5);
        mem.WriteU32((c.SP + 0x20u), c.S4);
        mem.WriteU32((c.SP + 0x1Cu), c.S3);
        mem.WriteU32((c.SP + 0x18u), c.S2);
        mem.WriteU32((c.SP + 0x14u), c.S1);
        mem.WriteU32((c.SP + 0x10u), c.S0);
        c.V0 = mem.ReadU32((c.A0 + 0x6E00u));
        c.V0 = c.V0 >> 11;
        c.LO = (uint)((long)(int)c.V0 * (int)c.V0); c.HI = (uint)(((long)(int)c.V0 * (int)c.V0) >> 32);
        c.V0 = 0x801B0000u;
        c.A2 = c.V0 + 0x5948u;
        c.V0 = mem.ReadU32((c.A0 + 0x6E04u));
        c.S7 = c.LO;
        c.V0 = c.V0 >> 11;
        c.LO = (uint)((long)(int)c.V0 * (int)c.V0); c.HI = (uint)(((long)(int)c.V0 * (int)c.V0) >> 32);
        c.V1 = mem.ReadU32((c.A0 + 0x6E08u));
        c.S4 = 0x1F800000u;
        c.V1 = (uint)(short)mem.ReadU16((c.V1 + 0x6Au));
        c.S1 = mem.ReadU16((c.A2 + 0x44CCu));
        c.S6 = c.LO;
        if ((int)c.V1 >= 0) {
            c.A1 = c.V1;
            goto L8001C698;
        }
        c.A1 = c.V1;
        c.A1 = c.V1 + 0x7FFu;
        L8001C698: ;
        c.V0 = (uint)((int)c.A1 >> 11);
        c.A0 = c.V0 + 0x2u;
        c.LO = (uint)((long)(int)c.A0 * (int)c.A0); c.HI = (uint)(((long)(int)c.A0 * (int)c.A0) >> 32);
        c.S1 = c.S1 - 0x1u;
        c.V0 = 0xFFFFFFFFu;
        c.S5 = c.LO;
        if (c.S1 == c.V0) {
            c.S3 = c.A2;
            goto L8001C7CC;
        }
        c.S3 = c.A2;
        c.FP = c.A3;
        c.V0 = c.S1 << 1;
        L8001C6C0: ;
        Poll(c, mem);
        c.S2 = c.V0 + c.S3;
        c.A0 = 0x00010000u;
        c.T0 = 0x80180000u;
        c.T0 = c.T0 - 0x4130u;
        c.V1 = mem.ReadU16((c.S2 + 0x45CEu));
        c.A0 = c.A0 | 0x8000u;
        c.V0 = c.V1 << 1;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 2;
        c.S0 = c.V0 + c.T0;
        c.V0 = mem.ReadU8((c.S0 + 0x8u));
        c.A0 = c.FP + c.A0;
        c.V0 = c.V0 << 12;
        mem.WriteU32((c.S4 + 0x128u), c.V0);
        c.V1 = mem.ReadU8((c.S0 + 0x9u));
        c.V0 = mem.ReadU8((c.S0 + 0xBu));
        c.V1 = c.V1 << 12;
        c.V0 = c.V0 - 0x80u;
        c.V0 = c.V0 << 7;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32((c.S4 + 0x12Cu), c.V1);
        c.V0 = mem.ReadU8((c.S0 + 0xAu));
        c.V1 = mem.ReadU8((c.A0 + 0x6E15u));
        c.V0 = c.V0 << 12;
        mem.WriteU8((c.S4 + 0x1E8u), (byte)c.V1);
        mem.WriteU32((c.S4 + 0x130u), c.V0);
        c.V0 = mem.ReadU8((c.S0 + 0x4u));
        c.V0 = c.V0 & 0x0001u;
        mem.WriteU8((c.S0 + 0x4u), (byte)c.V0);
        c.RA = 0x8001C73Cu;
        ShadowTower_st.func_8001C4C8(c, mem);
        c.V1 = c.V0;
        c.V0 = (int)c.S5 < (int)c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.V0 = (int)c.V1 < (int)c.S7 ? 1u : 0u;
            goto L8001C79C;
        }
        c.V0 = (int)c.V1 < (int)c.S7 ? 1u : 0u;
        c.V0 = mem.ReadU16((c.S3 + 0x44CCu));
        c.V0 = c.V0 - 0x1u;
        mem.WriteU16((c.S3 + 0x44CCu), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S3 + 0x44CCu));
        c.V0 = (int)c.S1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.A0 = c.S1;
            goto L8001C7BC;
        }
        c.A0 = c.S1;
        c.V1 = c.S2;
        L8001C774: ;
        Poll(c, mem);
        c.V0 = mem.ReadU16((c.V1 + 0x45D0u));
        c.A0 = c.A0 + 0x1u;
        mem.WriteU16((c.V1 + 0x45CEu), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.S3 + 0x44CCu));
        c.V0 = (int)c.A0 < (int)c.V0 ? 1u : 0u;
        if (c.V0 != 0u) {
            c.V1 = c.V1 + 0x2u;
            goto L8001C774;
        }
        c.V1 = c.V1 + 0x2u;
        c.S1 = c.S1 - 0x1u;
        goto L8001C7C0;
        L8001C79C: ;
        if (c.V0 == 0u) {
            c.V0 = (int)c.S6 < (int)c.V1 ? 1u : 0u;
            goto L8001C7AC;
        }
        c.V0 = (int)c.S6 < (int)c.V1 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L8001C7BC;
        }
        L8001C7AC: ;
        c.V0 = mem.ReadU8((c.S0 + 0x4u));
        c.V0 = c.V0 | 0x0002u;
        mem.WriteU8((c.S0 + 0x4u), (byte)c.V0);
        L8001C7BC: ;
        c.S1 = c.S1 - 0x1u;
        L8001C7C0: ;
        c.V0 = 0xFFFFFFFFu;
        if (c.S1 != c.V0) {
            c.V0 = c.S1 << 1;
            goto L8001C6C0;
        }
        c.V0 = c.S1 << 1;
        L8001C7CC: ;
        c.RA = mem.ReadU32((c.SP + 0x34u));
        c.FP = mem.ReadU32((c.SP + 0x30u));
        c.S7 = mem.ReadU32((c.SP + 0x2Cu));
        c.S6 = mem.ReadU32((c.SP + 0x28u));
        c.S5 = mem.ReadU32((c.SP + 0x24u));
        c.S4 = mem.ReadU32((c.SP + 0x20u));
        c.S3 = mem.ReadU32((c.SP + 0x1Cu));
        c.S2 = mem.ReadU32((c.SP + 0x18u));
        c.S1 = mem.ReadU32((c.SP + 0x14u));
        c.S0 = mem.ReadU32((c.SP + 0x10u));
        c.SP = c.SP + 0x38u;
        return;
    }
}
