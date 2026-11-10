using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_80023ED8</c>: func_800251C8's last call. <c>ST_MODELEXTRAS2</c>.
/// </summary>
public static class ModelExtras2
{
    public static readonly Literal R = new(typeof(ModelExtras2), "model extras 2", "MODELEXTRAS2", 0x80023ED8, Run,
        "func_800251C8's last call.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x38u;
        mem.WriteU32((c.SP + 0x2Cu), c.S7);
        c.S7 = c.A0;
        mem.WriteU32((c.SP + 0x18u), c.S2);
        c.S2 = c.A2;
        mem.WriteU32((c.SP + 0x14u), c.S1);
        c.S1 = c.A1;
        mem.WriteU32((c.SP + 0x24u), c.S5);
        c.S5 = c.A3 + c.S1;
        mem.WriteU32((c.SP + 0x20u), c.S4);
        c.S4 = 0x1F800000u;
        c.S4 = mem.ReadU32((c.S4 + 0xF0u));
        c.V0 = (int)c.S1 < (int)c.S5 ? 1u : 0u;
        mem.WriteU32((c.SP + 0x30u), c.RA);
        mem.WriteU32((c.SP + 0x28u), c.S6);
        mem.WriteU32((c.SP + 0x1Cu), c.S3);
        if (c.V0 == 0u) {
            mem.WriteU32((c.SP + 0x10u), c.S0);
            goto L80023FD4;
        }
        mem.WriteU32((c.SP + 0x10u), c.S0);
        c.V0 = 0x801C0000u;
        c.S6 = c.V0 + 0x76F8u;
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.V1 = c.S2 << 2;
        c.S3 = c.V1 + c.V0;
        L80023F38: ;
        Poll(c, mem);
        c.V1 = mem.ReadU8(c.S4);
        c.V0 = 0u | 0x834Cu;
        c.V0 = c.S3 + c.V0;
        c.S0 = mem.ReadU32(c.V0);
        if (c.V1 == 0u) {
            c.S4 = c.S4 + 0x1u;
            goto L80023F94;
        }
        c.S4 = c.S4 + 0x1u;
        if (c.S0 != 0u) {
            c.A0 = c.S7;
            goto L80023F74;
        }
        c.A0 = c.S7;
        c.A1 = c.S1 << 1;
        c.V0 = c.S2 + c.S6;
        c.A3 = mem.ReadU8((c.V0 - 0x1B8u));
        c.A2 = c.S2 & 0xFFFFu;
        c.RA = 0x80023F6Cu;
        ShadowTower_st.func_80023ABC(c, mem);
        c.S1 = c.S1 + 0x1u;
        goto L80023FC4;
        L80023F74: ;
        c.A0 = c.S0;
        c.RA = 0x80023F7Cu;
        ShadowTower_st.func_80015258(c, mem);
        c.V0 = c.V0 & 0x00FFu;
        c.V1 = 0x00000001u;
        if (c.V0 != c.V1) {
            c.A0 = c.S0;
            goto L80023FC0;
        }
        c.A0 = c.S0;
        c.A1 = 0x00000002u;
        goto L80023FB8;
        L80023F94: ;
        if (c.S0 == 0u) {
            goto L80023FC0;
        }
        c.A0 = c.S0;
        c.RA = 0x80023FA4u;
        ShadowTower_st.func_80015258(c, mem);
        c.V0 = c.V0 & 0x00FFu;
        c.V1 = 0x00000003u;
        if (c.V0 == c.V1) {
            c.A0 = c.S0;
            goto L80023FC0;
        }
        c.A0 = c.S0;
        c.A1 = 0x00000001u;
        L80023FB8: ;
        c.RA = 0x80023FC0u;
        ShadowTower_st.func_80015250(c, mem);
        L80023FC0: ;
        c.S1 = c.S1 + 0x1u;
        L80023FC4: ;
        c.S3 = c.S3 + 0x4u;
        c.V0 = (int)c.S1 < (int)c.S5 ? 1u : 0u;
        if (c.V0 != 0u) {
            c.S2 = c.S2 + 0x1u;
            goto L80023F38;
        }
        c.S2 = c.S2 + 0x1u;
        L80023FD4: ;
        c.RA = mem.ReadU32((c.SP + 0x30u));
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
