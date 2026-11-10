using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_80033DB8</c>: per entity before its model is drawn; calls func_80015258. <c>ST_MODELLIGHT</c>.
/// </summary>
public static class ModelLight
{
    public static readonly Literal R = new(typeof(ModelLight), "model light", "MODELLIGHT", 0x80033DB8, Run,
        "per entity before its model is drawn; calls func_80015258.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x18u;
        mem.WriteU32((c.SP + 0x10u), c.S0);
        c.S0 = c.A1;
        c.V1 = 0x80160000u;
        c.V1 = c.V1 - 0x2F70u;
        c.A0 = c.A0 & 0xFFFFu;
        c.V0 = c.A0 << 2;
        c.V0 = c.V0 + c.V1;
        c.V1 = 0u | 0x834Cu;
        c.V0 = c.V0 + c.V1;
        mem.WriteU32((c.SP + 0x14u), c.RA);
        c.V0 = mem.ReadU32(c.V0);
        c.A0 = c.A0 < 0x00000070u ? 1u : 0u;
        if (c.A0 != 0u) {
            mem.WriteU32(c.S0, c.V0);
            goto L80033E1C;
        }
        mem.WriteU32(c.S0, c.V0);
        if (c.V0 == 0u) {
            goto L80033E18;
        }
        c.A0 = c.V0;
        c.RA = 0x80033E04u;
        ShadowTower_st.func_80015258(c, mem);
        c.V0 = c.V0 - 0x1u;
        c.V0 = c.V0 & 0x00FFu;
        c.V0 = c.V0 < 0x00000002u ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L80033E1C;
        }
        L80033E18: ;
        mem.WriteU32(c.S0, 0u);
        L80033E1C: ;
        c.RA = mem.ReadU32((c.SP + 0x14u));
        c.S0 = mem.ReadU32((c.SP + 0x10u));
        c.SP = c.SP + 0x18u;
        return;
    }
}
