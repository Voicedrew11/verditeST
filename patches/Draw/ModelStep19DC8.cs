using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_80019DC8</c>: a model step in func_80022678 and func_80021AC4; not read. <c>ST_MODELSTEP19DC8</c>.
/// </summary>
public static class ModelStep19DC8
{
    public static readonly Literal R = new(typeof(ModelStep19DC8), "model-step-19DC8", "MODELSTEP19DC8", 0x80019DC8, Run,
        "a model step in func_80022678 and func_80021AC4; not read.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x20u;
        c.V0 = c.A1 >> 1;
        mem.WriteU32((c.SP + 0x14u), c.S1);
        c.S1 = c.V0 & 0x0002u;
        c.A1 = c.A1 & 0x0003u;
        c.A1 = c.A1 << 21;
        c.S1 = c.S1 | c.A1;
        mem.WriteU32((c.SP + 0x10u), c.S0);
        c.S0 = 0x1F800000u;
        c.A0 = c.A0 & 0xFFFFu;
        c.V0 = c.A0 << 2;
        c.V0 = c.V0 + c.A0;
        c.V0 = c.V0 << 3;
        c.V0 = c.V0 + 0xCu;
        mem.WriteU32((c.SP + 0x18u), c.RA);
        c.A0 = mem.ReadU32((c.S0 + 0xCu));
        c.A1 = 0x00010000u;
        c.V0 = c.A0 + c.V0;
        mem.WriteU32((c.S0 + 0x20u), c.V0);
        c.V1 = mem.ReadU32((c.V0 + 0x8u));
        c.A1 = c.A1 | 0x8000u;
        c.V1 = c.V1 + 0xCu;
        c.V1 = c.A0 + c.V1;
        mem.WriteU32((c.S0 + 0x24u), c.V1);
        c.V1 = mem.ReadU32((c.V0 + 0x10u));
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.A2 = c.V0 + c.A1;
        c.A1 = mem.ReadU8((c.A2 + 0x6DFCu));
        c.V1 = c.V1 + 0xCu;
        c.A0 = c.A0 + c.V1;
        c.V0 = c.A1 & 0x0002u;
        if (c.V0 == 0u) {
            mem.WriteU32((c.S0 + 0x1Cu), c.A0);
            goto L80019E84;
        }
        mem.WriteU32((c.S0 + 0x1Cu), c.A0);
        c.V0 = c.A1 & 0x0010u;
        if (c.V0 == 0u) {
            goto L80019E70;
        }
        c.V1 = mem.ReadU32((c.A2 + 0x6DF8u));
        c.V0 = 0xFFFFFF88u;
        c.V1 = (uint)((int)c.V1 >> 13);
        c.V0 = c.V0 - c.V1;
        goto L80019E88;
        L80019E70: ;
        c.V1 = mem.ReadU32((c.A2 + 0x6DF8u));
        c.V0 = 0xFFFFFFA0u;
        c.V1 = (uint)((int)c.V1 >> 11);
        c.V0 = c.V0 - c.V1;
        goto L80019E88;
        L80019E84: ;
        c.V0 = 0x00000080u;
        L80019E88: ;
        mem.WriteU8((c.S0 + 0x62u), (byte)c.V0);
        mem.WriteU8((c.S0 + 0x61u), (byte)c.V0);
        mem.WriteU8((c.S0 + 0x60u), (byte)c.V0);
        c.V0 = 0x00010000u;
        c.V0 = c.V0 | 0x8000u;
        c.V1 = 0x80160000u;
        c.V1 = c.V1 - 0x2F70u;
        c.V1 = c.V1 + c.V0;
        c.V1 = mem.ReadU8((c.V1 + 0x6DFCu));
        c.V0 = c.V1 & 0x0002u;
        if (c.V0 == 0u) {
            c.T0 = 0x00000006u;
            goto L80019EE4;
        }
        c.T0 = 0x00000006u;
        c.T1 = 0x00000004u;
        c.T2 = 0x00000002u;
        c.T4 = c.T0 << 4;
        c.T5 = c.T1 << 4;
        c.T6 = c.T2 << 4;
        RecompOne.Runtime.Gte.WriteControl(21, c.T4);
        RecompOne.Runtime.Gte.WriteControl(22, c.T5);
        RecompOne.Runtime.Gte.WriteControl(23, c.T6);
        goto L80019F3C;
        L80019EE4: ;
        c.V0 = c.V1 & 0x0004u;
        if (c.V0 == 0u) {
            c.T0 = 0x00000002u;
            goto L80019F18;
        }
        c.T0 = 0x00000002u;
        c.T1 = 0x00000006u;
        c.T2 = c.T1;
        c.T4 = c.T0 << 4;
        c.T5 = c.T1 << 4;
        c.T6 = c.T2 << 4;
        RecompOne.Runtime.Gte.WriteControl(21, c.T4);
        RecompOne.Runtime.Gte.WriteControl(22, c.T5);
        RecompOne.Runtime.Gte.WriteControl(23, c.T6);
        goto L80019F3C;
        L80019F18: ;
        c.T0 = 0x00000005u;
        c.T1 = c.T0;
        c.T2 = c.T0;
        c.T4 = c.T0 << 4;
        c.T5 = c.T1 << 4;
        c.T6 = c.T2 << 4;
        RecompOne.Runtime.Gte.WriteControl(21, c.T4);
        RecompOne.Runtime.Gte.WriteControl(22, c.T5);
        RecompOne.Runtime.Gte.WriteControl(23, c.T6);
        L80019F3C: ;
        c.A1 = mem.ReadU32((c.S0 + 0x1Cu));
        c.V0 = mem.ReadU32((c.S0 + 0x20u));
        c.A2 = mem.ReadU32(c.S0);
        c.A0 = mem.ReadU16((c.V0 + 0x18u));
        c.A3 = c.S1;
        c.RA = 0x80019F54u;
        ShadowTower_st.func_800194C4(c, mem);
        c.A1 = mem.ReadU32((c.S0 + 0x1Cu));
        c.V0 = mem.ReadU32((c.S0 + 0x20u));
        c.A2 = mem.ReadU32(c.S0);
        c.A0 = mem.ReadU16((c.V0 + 0x1Cu));
        c.A3 = c.S1;
        c.RA = 0x80019F6Cu;
        ShadowTower_st.func_800196C0(c, mem);
        c.A1 = mem.ReadU32((c.S0 + 0x1Cu));
        c.V0 = mem.ReadU32((c.S0 + 0x20u));
        c.A2 = mem.ReadU32(c.S0);
        c.A0 = mem.ReadU16((c.V0 + 0x1Au));
        c.A3 = c.S1;
        c.RA = 0x80019F84u;
        ShadowTower_st.func_800198F4(c, mem);
        c.A1 = mem.ReadU32((c.S0 + 0x1Cu));
        c.V0 = mem.ReadU32((c.S0 + 0x20u));
        c.A2 = mem.ReadU32(c.S0);
        c.A0 = mem.ReadU16((c.V0 + 0x1Eu));
        c.A3 = c.S1;
        c.RA = 0x80019F9Cu;
        ShadowTower_st.func_80019B34(c, mem);
        c.RA = mem.ReadU32((c.SP + 0x18u));
        c.S1 = mem.ReadU32((c.SP + 0x14u));
        c.S0 = mem.ReadU32((c.SP + 0x10u));
        c.SP = c.SP + 0x20u;
        return;
    }
}
