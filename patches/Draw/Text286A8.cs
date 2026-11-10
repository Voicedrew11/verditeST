using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_800286A8</c>: text for func_80028784, the menu and the prompt; not read. <c>ST_TEXT286A8</c>.
/// </summary>
public static class Text286A8
{
    public static readonly Literal R = new(typeof(Text286A8), "text-286A8", "TEXT286A8", 0x800286A8, Run,
        "text for func_80028784, the menu and the prompt; not read.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x28u;
        c.T1 = c.A0;
        c.T2 = c.A1;
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.V1 = 0u | 0x8000u;
        mem.WriteU32((c.SP + 0x1Cu), c.S1);
        c.S1 = c.V0 + c.V1;
        mem.WriteU32((c.SP + 0x20u), c.RA);
        mem.WriteU32((c.SP + 0x18u), c.S0);
        c.V1 = mem.ReadU32((c.S1 + 0x304u));
        c.S0 = mem.ReadU32((c.V1 + 0x8u));
        c.V0 = mem.ReadU32((c.V1 + 0x4u));
        c.T0 = c.S0 + 0xCu;
        c.V0 = c.V0 < c.T0 ? 1u : 0u;
        if (c.V0 != 0u) {
            c.A3 = c.A2;
            goto L8002876C;
        }
        c.A3 = c.A2;
        c.A0 = 0u;
        c.A1 = c.T1;
        mem.WriteU32((c.V1 + 0x8u), c.T0);
        c.V0 = mem.ReadU32((c.S1 + 0x328u));
        c.A2 = c.T2;
        c.V0 = c.V0 + 0x1u;
        mem.WriteU32((c.S1 + 0x328u), c.V0);
        c.RA = 0x80028710u;
        ShadowTower_st.GetTPage_st(c, mem);
        c.A0 = c.S0;
        c.A1 = 0u;
        c.A2 = c.A1;
        c.A3 = c.V0 & 0xFFFFu;
        mem.WriteU32((c.SP + 0x10u), 0u);
        c.RA = 0x80028728u;
        ShadowTower_st.SetDrawMode_st(c, mem);
        c.A0 = 0x00FF0000u;
        c.A0 = c.A0 | 0xFFFFu;
        c.A1 = 0xFF000000u;
        c.V0 = mem.ReadU32((c.S1 + 0xE4u));
        c.V1 = mem.ReadU32(c.S0);
        c.V0 = mem.ReadU32(c.V0);
        c.V1 = c.V1 & c.A1;
        c.V0 = c.V0 & c.A0;
        c.V1 = c.V1 | c.V0;
        mem.WriteU32(c.S0, c.V1);
        c.V1 = mem.ReadU32((c.S1 + 0xE4u));
        c.V0 = mem.ReadU32(c.V1);
        c.A0 = c.S0 & c.A0;
        c.V0 = c.V0 & c.A1;
        c.V0 = c.V0 | c.A0;
        mem.WriteU32(c.V1, c.V0);
        L8002876C: ;
        c.RA = mem.ReadU32((c.SP + 0x20u));
        c.S1 = mem.ReadU32((c.SP + 0x1Cu));
        c.S0 = mem.ReadU32((c.SP + 0x18u));
        c.SP = c.SP + 0x28u;
        return;
    }
}
