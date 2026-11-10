using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_80035AA8</c>: a model step in func_80022678 and func_80021AC4; not read. <c>ST_MODELSTEP35AA8</c>.
/// </summary>
public static class ModelStep35AA8
{
    public static readonly Literal R = new(typeof(ModelStep35AA8), "model-step-35AA8", "MODELSTEP35AA8", 0x80035AA8, Run,
        "a model step in func_80022678 and func_80021AC4; not read.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x20u;
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.V1 = c.A1 << 2;
        c.V1 = c.V1 + c.V0;
        c.V0 = 0u | 0x834Cu;
        mem.WriteU32((c.SP + 0x14u), c.S1);
        c.S1 = c.V1 + c.V0;
        mem.WriteU32((c.SP + 0x1Cu), c.RA);
        mem.WriteU32((c.SP + 0x18u), c.S2);
        mem.WriteU32((c.SP + 0x10u), c.S0);
        c.V1 = mem.ReadU32(c.S1);
        c.V0 = mem.ReadU16((c.V1 + 0x4u));
        c.S0 = mem.ReadU32(c.A0);
        if (c.V0 != 0u) {
            c.S2 = 0x1F800000u;
            goto L80035B3C;
        }
        c.S2 = 0x1F800000u;
        if (c.S0 == 0u) {
            goto L80035B08;
        }
        c.A0 = c.S0;
        c.RA = 0x80035AFCu;
        ShadowTower_st.func_80035008(c, mem);
        c.A0 = c.S0;
        c.RA = 0x80035B04u;
        ShadowTower_st.func_8003413C(c, mem);
        c.V1 = mem.ReadU32(c.S1);
        L80035B08: ;
        c.V0 = mem.ReadU32((c.V1 + 0x8u));
        c.V1 = c.V1 + c.V0;
        c.V0 = c.V1 + 0xCu;
        mem.WriteU32((c.S2 + 0xCu), c.V1);
        mem.WriteU32((c.S2 + 0x20u), c.V0);
        c.A0 = mem.ReadU32((c.V1 + 0xCu));
        c.V0 = 0x00000001u;
        c.A0 = c.A0 + 0xCu;
        c.V1 = c.V1 + c.A0;
        mem.WriteU32((c.S2 + 0x48u), c.V1);
        goto L80035B64;
        L80035B3C: ;
        c.V0 = mem.ReadU16((c.V1 + 0x6u));
        if (c.V0 == 0u) {
            goto L80035B5C;
        }
        c.RA = 0x80035B54u;
        ShadowTower_st.func_8003569C(c, mem);
        goto L80035B64;
        L80035B5C: ;
        c.RA = 0x80035B64u;
        ShadowTower_st.func_80034364(c, mem);
        L80035B64: ;
        c.RA = mem.ReadU32((c.SP + 0x1Cu));
        c.S2 = mem.ReadU32((c.SP + 0x18u));
        c.S1 = mem.ReadU32((c.SP + 0x14u));
        c.S0 = mem.ReadU32((c.SP + 0x10u));
        c.SP = c.SP + 0x20u;
        return;
    }
}
