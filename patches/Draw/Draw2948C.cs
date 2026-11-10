using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_8002948C</c>: the menu's last call before presenting; not read. <c>ST_DRAW2948C</c>.
/// </summary>
public static class Draw2948C
{
    public static readonly Literal R = new(typeof(Draw2948C), "draw-2948C", "DRAW2948C", 0x8002948C, Run,
        "the menu's last call before presenting; not read.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x28u;
        c.V0 = 0x80080000u;
        mem.WriteU32((c.SP + 0x20u), c.RA);
        c.A2 = c.V0 + 0x7F1Cu;
        c.V1 = mem.ReadWordLeft(c.V1, (c.A2 + 0x3u));
        c.V1 = mem.ReadWordRight(c.V1, c.A2);
        c.A0 = mem.ReadWordLeft(c.A0, (c.A2 + 0x7u));
        c.A0 = mem.ReadWordRight(c.A0, (c.A2 + 0x4u));
        mem.WriteWordLeft((c.SP + 0x13u), c.V1);
        mem.WriteWordRight((c.SP + 0x10u), c.V1);
        mem.WriteWordLeft((c.SP + 0x17u), c.A0);
        mem.WriteWordRight((c.SP + 0x14u), c.A0);
        c.V0 = 0x80080000u;
        c.A2 = c.V0 + 0x7F2Cu;
        c.V1 = mem.ReadWordLeft(c.V1, (c.A2 + 0x3u));
        c.V1 = mem.ReadWordRight(c.V1, c.A2);
        mem.WriteWordLeft((c.SP + 0x1Bu), c.V1);
        mem.WriteWordRight((c.SP + 0x18u), c.V1);
        c.V1 = 0x00010000u;
        c.V1 = c.V1 | 0x8000u;
        c.A2 = 0x80160000u;
        c.A2 = c.A2 - 0x2F70u;
        c.V1 = c.A2 + c.V1;
        c.V0 = 0u | 0x8000u;
        c.V0 = c.A2 + c.V0;
        c.V1 = mem.ReadU8((c.V1 + 0x6CC2u));
        c.A1 = mem.ReadU32((c.V0 + 0x2FCu));
        c.A0 = c.V1 << 1;
        c.A0 = c.A0 + c.V1;
        c.A0 = c.A0 << 3;
        c.A0 = c.A0 - c.V1;
        c.A0 = c.A0 << 2;
        c.A0 = c.A0 + c.A2;
        c.RA = 0x80029518u;
        ShadowTower_st.func_80079D0C(c, mem);
        c.A0 = c.SP + 0x10u;
        c.A1 = c.SP + 0x18u;
        c.A2 = 0x00000001u;
        c.RA = 0x80029528u;
        ShadowTower_st.func_80028220(c, mem);
        c.A0 = 0x00000002u;
        c.A1 = 0u;
        c.A2 = c.A1;
        c.RA = 0x80029538u;
        ShadowTower_st.func_800286A8(c, mem);
        c.RA = mem.ReadU32((c.SP + 0x20u));
        c.SP = c.SP + 0x28u;
        return;
    }
}
