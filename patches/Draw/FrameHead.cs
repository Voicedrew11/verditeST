using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_800174C4</c>: the frame head: flips the buffer, rewinds the packet cursor, clears both ordering tables. <c>ST_FRAMEHEAD</c>.
/// </summary>
public static class FrameHead
{
    public static readonly Literal R = new(typeof(FrameHead), "frame head", "FRAMEHEAD", 0x800174C4, Run,
        "the frame head: flips the buffer, rewinds the packet cursor, clears both ordering tables.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x20u;
        mem.WriteU32((c.SP + 0x14u), c.S1);
        c.S1 = 0x00010000u;
        c.S1 = c.S1 | 0x8000u;
        c.A2 = 0x80160000u;
        c.A2 = c.A2 - 0x2F70u;
        c.S1 = c.A2 + c.S1;
        mem.WriteU32((c.SP + 0x18u), c.RA);
        mem.WriteU32((c.SP + 0x10u), c.S0);
        c.V0 = mem.ReadU8((c.S1 + 0x6CC2u));
        c.S0 = 0u | 0x8000u;
        c.V0 = c.V0 < 0x00000001u ? 1u : 0u;
        mem.WriteU8((c.S1 + 0x6CC2u), (byte)c.V0);
        c.V0 = mem.ReadU8((c.S1 + 0x6CC2u));
        c.S0 = c.A2 + c.S0;
        c.V1 = c.V0 << 1;
        c.V1 = c.V1 + c.V0;
        c.V1 = c.V1 << 2;
        c.V0 = 0x80160000u;
        c.V0 = c.V0 + 0x537Cu;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32((c.S0 + 0x304u), c.V1);
        c.V0 = mem.ReadU32(c.V1);
        c.A1 = 0x00001000u;
        mem.WriteU32((c.V1 + 0x8u), c.V0);
        c.A0 = mem.ReadU8((c.S1 + 0x6CC2u));
        c.A2 = c.A2 + 0xE4u;
        c.A0 = c.A0 << 14;
        c.A0 = c.A0 + c.A2;
        mem.WriteU32((c.S0 + 0xE4u), c.A0);
        c.RA = 0x80017540u;
        ShadowTower_st.ClearOTagR_st(c, mem);
        c.A1 = 0x00000040u;
        c.V0 = 0x80160000u;
        c.A0 = mem.ReadU8((c.S1 + 0x6CC2u));
        c.V0 = c.V0 + 0x5178u;
        c.A0 = c.A0 << 8;
        c.A0 = c.A0 + c.V0;
        mem.WriteU32((c.S0 + 0x2E8u), c.A0);
        c.RA = 0x80017560u;
        ShadowTower_st.ClearOTagR_st(c, mem);
        mem.WriteU32((c.S0 + 0x328u), 0u);
        mem.WriteU32((c.S0 + 0x324u), 0u);
        mem.WriteU32((c.S0 + 0x320u), 0u);
        mem.WriteU32((c.S0 + 0x31Cu), 0u);
        mem.WriteU32((c.S0 + 0x32Cu), 0u);
        mem.WriteU32((c.S0 + 0x330u), 0u);
        mem.WriteU32((c.S0 + 0x338u), 0u);
        mem.WriteU32((c.S0 + 0x33Cu), 0u);
        mem.WriteU32((c.S0 + 0x340u), 0u);
        mem.WriteU32((c.S0 + 0x344u), 0u);
        mem.WriteU32((c.S0 + 0x348u), 0u);
        c.RA = mem.ReadU32((c.SP + 0x18u));
        c.S1 = mem.ReadU32((c.SP + 0x14u));
        c.S0 = mem.ReadU32((c.SP + 0x10u));
        c.SP = c.SP + 0x20u;
        return;
    }
}
