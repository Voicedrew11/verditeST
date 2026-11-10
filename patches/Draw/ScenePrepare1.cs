using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_8001C264</c>: the scene's first preparation pass after the cell list; writes no packet. <c>ST_SCENEPREPARE1</c>.
/// </summary>
public static class ScenePrepare1
{
    public static readonly Literal R = new(typeof(ScenePrepare1), "scene preparation 1", "SCENEPREPARE1", 0x8001C264, Run,
        "the scene's first preparation pass after the cell list; writes no packet.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x38u;
        mem.WriteU32((c.SP + 0x30u), c.FP);
        c.FP = 0x1F800000u;
        c.V0 = 0x801A0000u;
        c.V0 = c.V0 - 0x70D8u;
        mem.WriteU32((c.SP + 0x34u), c.RA);
        mem.WriteU32((c.SP + 0x2Cu), c.S7);
        mem.WriteU32((c.SP + 0x28u), c.S6);
        mem.WriteU32((c.SP + 0x24u), c.S5);
        mem.WriteU32((c.SP + 0x20u), c.S4);
        mem.WriteU32((c.SP + 0x1Cu), c.S3);
        mem.WriteU32((c.SP + 0x18u), c.S2);
        mem.WriteU32((c.SP + 0x14u), c.S1);
        mem.WriteU32((c.SP + 0x10u), c.S0);
        c.V1 = (uint)(short)mem.ReadU16((c.V0 + 0x26Au));
        c.S7 = (uint)(short)mem.ReadU16((c.V0 + 0x268u));
        c.V0 = (int)c.S7 < 2049 ? 1u : 0u;
        if (c.V0 != 0u) {
            c.S2 = c.V1 + 0x400u;
            goto L8001C2C0;
        }
        c.S2 = c.V1 + 0x400u;
        c.V0 = 0x00001000u;
        c.S1 = c.V0 - c.S7;
        goto L8001C2C4;
        L8001C2C0: ;
        c.S1 = c.S7;
        L8001C2C4: ;
        c.V0 = c.S1 << 3;
        c.V0 = c.V0 + c.S1;
        c.V0 = c.V0 << 3;
        c.V0 = c.V0 + c.S1;
        c.V0 = c.V0 << 3;
        if ((int)c.V0 >= 0) {
            goto L8001C2E4;
        }
        c.V0 = c.V0 + 0x3FFu;
        L8001C2E4: ;
        c.V0 = (uint)((int)c.V0 >> 10);
        c.S1 = c.V0 + 0x200u;
        c.A0 = c.S2;
        c.RA = 0x8001C2F4u;
        ShadowTower_st.rcos_st(c, mem);
        c.A0 = c.S2;
        c.S6 = c.V0;
        c.RA = 0x8001C300u;
        ShadowTower_st.rsin_st(c, mem);
        c.S0 = c.S2 - c.S1;
        c.A0 = c.S0;
        c.S4 = c.V0;
        c.RA = 0x8001C310u;
        ShadowTower_st.rcos_st(c, mem);
        c.A0 = c.S0;
        c.S5 = c.V0;
        c.RA = 0x8001C31Cu;
        ShadowTower_st.rsin_st(c, mem);
        c.S0 = c.S2 + c.S1;
        c.A0 = c.S0;
        c.S3 = c.V0;
        c.RA = 0x8001C32Cu;
        ShadowTower_st.rcos_st(c, mem);
        c.A0 = c.S0;
        c.S2 = c.V0;
        c.RA = 0x8001C338u;
        ShadowTower_st.rsin_st(c, mem);
        c.A0 = c.S7;
        c.S1 = c.V0;
        c.RA = 0x8001C344u;
        ShadowTower_st.rcos_st(c, mem);
        c.A0 = c.S7;
        c.S0 = c.V0;
        c.RA = 0x8001C350u;
        ShadowTower_st.rsin_st(c, mem);
        c.A0 = c.S0 << 1;
        c.A0 = c.A0 + c.S0;
        c.A0 = c.A0 << 2;
        c.A0 = c.S0 - c.A0;
        c.A0 = (uint)((int)c.A0 >> 12);
        c.LO = (uint)((long)(int)c.A0 * (int)c.S6); c.HI = (uint)(((long)(int)c.A0 * (int)c.S6) >> 32);
        c.A1 = c.LO;
        c.LO = (uint)((long)(int)c.A0 * (int)c.S4); c.HI = (uint)(((long)(int)c.A0 * (int)c.S4) >> 32);
        c.A0 = c.LO;
        c.V1 = c.S0 << 4;
        c.V1 = (uint)((int)c.V1 >> 12);
        c.LO = (uint)((long)(int)c.V1 * (int)c.S5); c.HI = (uint)(((long)(int)c.V1 * (int)c.S5) >> 32);
        c.T0 = c.LO;
        c.LO = (uint)((long)(int)c.V1 * (int)c.S3); c.HI = (uint)(((long)(int)c.V1 * (int)c.S3) >> 32);
        c.A2 = c.LO;
        c.LO = (uint)((long)(int)c.V1 * (int)c.S2); c.HI = (uint)(((long)(int)c.V1 * (int)c.S2) >> 32);
        c.T3 = c.LO;
        c.LO = (uint)((long)(int)c.V1 * (int)c.S1); c.HI = (uint)(((long)(int)c.V1 * (int)c.S1) >> 32);
        c.S0 = c.S0 << 3;
        c.T1 = c.LO;
        c.S0 = 0u - c.S0;
        c.S0 = (uint)((int)c.S0 >> 12);
        c.LO = (uint)((long)(int)c.S0 * (int)c.S6); c.HI = (uint)(((long)(int)c.S0 * (int)c.S6) >> 32);
        c.T4 = c.LO;
        c.LO = (uint)((long)(int)c.S0 * (int)c.S4); c.HI = (uint)(((long)(int)c.S0 * (int)c.S4) >> 32);
        c.A3 = c.A0 << 4;
        c.A3 = (uint)((int)c.A3 >> 16);
        c.V1 = c.A2 + c.A0;
        c.V1 = c.V1 << 4;
        c.V1 = (uint)((int)c.V1 >> 16);
        c.V1 = c.V1 - c.A3;
        c.A2 = c.A1 << 4;
        c.A2 = (uint)((int)c.A2 >> 16);
        c.T5 = c.LO;
        mem.WriteU32((c.FP + 0x1F0u), c.V1);
        c.V1 = 0u - c.V1;
        c.LO = (uint)((long)(int)c.V1 * (int)c.A2); c.HI = (uint)(((long)(int)c.V1 * (int)c.A2) >> 32);
        c.V1 = c.T0 + c.A1;
        c.V1 = c.V1 << 4;
        c.T2 = c.LO;
        c.V1 = (uint)((int)c.V1 >> 16);
        c.V1 = c.V1 - c.A2;
        c.LO = (uint)((long)(int)c.V1 * (int)c.A3); c.HI = (uint)(((long)(int)c.V1 * (int)c.A3) >> 32);
        c.A0 = c.T1 + c.A0;
        c.A0 = c.A0 << 4;
        c.A0 = (uint)((int)c.A0 >> 16);
        c.A0 = c.A0 - c.A3;
        mem.WriteU32((c.FP + 0x1F4u), c.A0);
        mem.WriteU32((c.FP + 0x1F8u), c.V1);
        c.T0 = c.LO;
        c.V1 = c.A0;
        c.V1 = 0u - c.V1;
        c.LO = (uint)((long)(int)c.V1 * (int)c.A2); c.HI = (uint)(((long)(int)c.V1 * (int)c.A2) >> 32);
        c.A1 = c.T3 + c.A1;
        c.A1 = c.A1 << 4;
        c.A1 = (uint)((int)c.A1 >> 16);
        c.A1 = c.A1 - c.A2;
        c.A0 = c.LO;
        mem.WriteU32((c.FP + 0x1FCu), c.A1);
        c.V1 = c.A1;
        c.LO = (uint)((long)(int)c.V1 * (int)c.A3); c.HI = (uint)(((long)(int)c.V1 * (int)c.A3) >> 32);
        mem.WriteU32((c.FP + 0x208u), c.V0);
        c.V0 = (uint)((int)c.T4 >> 12);
        mem.WriteU16((c.FP + 0x1EAu), (ushort)c.V0);
        c.V0 = (uint)((int)c.T5 >> 12);
        mem.WriteU16((c.FP + 0x1EEu), (ushort)c.V0);
        c.V0 = c.T2 + c.T0;
        mem.WriteU32((c.FP + 0x200u), c.V0);
        c.V1 = c.LO;
        c.V0 = c.A0 + c.V1;
        mem.WriteU32((c.FP + 0x204u), c.V0);
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
