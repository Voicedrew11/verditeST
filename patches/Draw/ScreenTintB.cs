using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_800218D8</c>: the second full-screen tint, from the bytes after ScreenTintA's (flag 0x8017BD67, colour 0x8017BD6C, 70, 74, blend 0x8017BD69). <c>ST_SCREENTINTB</c>.
/// </summary>
public static class ScreenTintB
{
    public static readonly Literal R = new(typeof(ScreenTintB), "screen tint b", "SCREENTINTB", 0x800218D8, Run,
        "the second full-screen tint, from the bytes after ScreenTintA's (flag 0x8017BD67, colour 0x8017BD6C, 70, 74, blend 0x8017BD69).");

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
        c.V0 = mem.ReadU8((c.S0 + 0x6CD7u));
        if (c.V0 == 0u) {
            c.V0 = 0x000000E0u;
            goto L80021980;
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
        c.V0 = mem.ReadU8((c.S0 + 0x6CD9u));
        c.A3 = 0x000000F0u;
        mem.WriteU32((c.SP + 0x28u), c.V1);
        mem.WriteU32((c.SP + 0x38u), 0u);
        c.V1 = mem.ReadU8((c.S0 + 0x6CDCu));
        c.A0 = mem.ReadU8((c.S0 + 0x6CE0u));
        c.A1 = mem.ReadU8((c.S0 + 0x6CE4u));
        c.V0 = c.V0 & 0x0003u;
        c.V0 = c.V0 << 5;
        c.V0 = c.V0 | 0x001Eu;
        mem.WriteU32((c.SP + 0x30u), c.A0);
        c.A0 = 0u;
        mem.WriteU32((c.SP + 0x34u), c.A1);
        c.A1 = c.A0;
        mem.WriteU32((c.SP + 0x24u), c.V0);
        mem.WriteU32((c.SP + 0x2Cu), c.V1);
        c.RA = 0x80021970u;
        ShadowTower_st.func_80021518(c, mem);
        mem.WriteU16((c.S0 + 0x6CE4u), (ushort)0u);
        mem.WriteU16((c.S0 + 0x6CE0u), (ushort)0u);
        mem.WriteU16((c.S0 + 0x6CDCu), (ushort)0u);
        mem.WriteU8((c.S0 + 0x6CD7u), (byte)0u);
        L80021980: ;
        c.RA = mem.ReadU32((c.SP + 0x44u));
        c.S0 = mem.ReadU32((c.SP + 0x40u));
        c.SP = c.SP + 0x48u;
        return;
    }
}
