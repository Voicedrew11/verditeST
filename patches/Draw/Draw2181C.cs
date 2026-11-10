using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_8002181C</c>: a draw-stage call; calls func_80021518 with 320 by 240; not read. <c>ST_DRAW2181C</c>.
/// </summary>
public static class Draw2181C
{
    public static readonly Literal R = new(typeof(Draw2181C), "draw-2181C", "DRAW2181C", 0x8002181C, Run,
        "a draw-stage call; calls func_80021518 with 320 by 240; not read.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x48u;
        c.V0 = 0x00010000u;
        c.V0 = c.V0 | 0x8000u;
        c.V1 = 0x80160000u;
        c.V1 = c.V1 - 0x2F70u;
        mem.WriteU32((c.SP + 0x40u), c.S0);
        c.S0 = c.V1 + c.V0;
        mem.WriteU32((c.SP + 0x44u), c.RA);
        c.V0 = mem.ReadU8((c.S0 + 0x6CD6u));
        if (c.V0 == 0u) {
            c.V0 = 0x000000E0u;
            goto L800218C4;
        }
        c.V0 = 0x000000E0u;
        mem.WriteU32((c.SP + 0x14u), c.V0);
        c.V0 = 0x0000000Fu;
        mem.WriteU32((c.SP + 0x18u), c.V0);
        mem.WriteU32((c.SP + 0x1Cu), c.V0);
        c.V0 = 0x00000001u;
        c.V1 = 0x00007C23u;
        c.A2 = 0x00000140u;
        mem.WriteU32((c.SP + 0x10u), 0u);
        mem.WriteU32((c.SP + 0x20u), c.V0);
        c.V0 = mem.ReadU8((c.S0 + 0x6CD8u));
        c.A3 = 0x000000F0u;
        mem.WriteU32((c.SP + 0x28u), c.V1);
        mem.WriteU32((c.SP + 0x38u), 0u);
        c.V1 = mem.ReadU8((c.S0 + 0x6CDAu));
        c.A0 = mem.ReadU8((c.S0 + 0x6CDEu));
        c.A1 = mem.ReadU8((c.S0 + 0x6CE2u));
        c.V0 = c.V0 & 0x0003u;
        c.V0 = c.V0 << 5;
        c.V0 = c.V0 | 0x001Eu;
        mem.WriteU32((c.SP + 0x30u), c.A0);
        c.A0 = 0u;
        mem.WriteU32((c.SP + 0x34u), c.A1);
        c.A1 = c.A0;
        mem.WriteU32((c.SP + 0x24u), c.V0);
        mem.WriteU32((c.SP + 0x2Cu), c.V1);
        c.RA = 0x800218B4u;
        ShadowTower_st.func_80021518(c, mem);
        mem.WriteU16((c.S0 + 0x6CE2u), (ushort)0u);
        mem.WriteU16((c.S0 + 0x6CDEu), (ushort)0u);
        mem.WriteU16((c.S0 + 0x6CDAu), (ushort)0u);
        mem.WriteU8((c.S0 + 0x6CD6u), (byte)0u);
        L800218C4: ;
        c.RA = mem.ReadU32((c.SP + 0x44u));
        c.S0 = mem.ReadU32((c.SP + 0x40u));
        c.SP = c.SP + 0x48u;
        return;
    }
}
