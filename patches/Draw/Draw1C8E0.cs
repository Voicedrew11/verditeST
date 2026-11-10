using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_8001C8E0</c>: a draw-stage call after the scene; calls func_8001C88C; not read. <c>ST_DRAW1C8E0</c>.
/// </summary>
public static class Draw1C8E0
{
    public static readonly Literal R = new(typeof(Draw1C8E0), "draw-1C8E0", "DRAW1C8E0", 0x8001C8E0, Run,
        "a draw-stage call after the scene; calls func_8001C88C; not read.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x28u;
        c.V0 = 0x801C0000u;
        c.V0 = c.V0 - 0x5EC8u;
        mem.WriteU32((c.SP + 0x14u), c.S1);
        c.S1 = 0x0000003Fu;
        mem.WriteU32((c.SP + 0x1Cu), c.S3);
        c.S3 = 0x00000001u;
        mem.WriteU32((c.SP + 0x18u), c.S2);
        c.S2 = c.V0;
        mem.WriteU32((c.SP + 0x10u), c.S0);
        c.S0 = c.S2 + 0x4u;
        mem.WriteU32((c.SP + 0x20u), c.RA);
        mem.WriteU8((c.V0 + 0x6100u), (byte)0u);
        L8001C914: ;
        Poll(c, mem);
        c.A0 = mem.ReadU8((c.S0 - 0x1u));
        c.V0 = 0x00000002u;
        if (c.A0 == c.V0) {
            goto L8001C994;
        }
        c.V1 = mem.ReadU8((c.S0 + 0x1u));
        if (c.V1 == c.S3) {
            c.V0 = 0x00000005u;
            goto L8001C93C;
        }
        c.V0 = 0x00000005u;
        if (c.V1 != c.V0) {
            goto L8001C994;
        }
        L8001C93C: ;
        if (c.A0 == c.S3) {
            goto L8001C958;
        }
        c.A0 = mem.ReadU16((c.S0 + 0x22u));
        c.RA = 0x8001C950u;
        ShadowTower_st.func_8001C88C(c, mem);
        if (c.V0 == 0u) {
            goto L8001C994;
        }
        L8001C958: ;
        c.V0 = mem.ReadU8((c.S2 + 0x6100u));
        c.V0 = c.V0 < 0x0000000Cu ? 1u : 0u;
        if (c.V0 == 0u) {
            c.V1 = 0x0000003Fu;
            goto L8001C9A8;
        }
        c.V1 = 0x0000003Fu;
        c.V0 = mem.ReadU8((c.S2 + 0x6100u));
        c.V1 = c.V1 - c.S1;
        c.V0 = c.V0 << 1;
        c.V0 = c.V0 + c.S2;
        mem.WriteU16((c.V0 + 0x6104u), (ushort)c.V1);
        c.V0 = mem.ReadU8((c.S2 + 0x6100u));
        c.V0 = c.V0 + 0x1u;
        mem.WriteU8((c.S2 + 0x6100u), (byte)c.V0);
        goto L8001C998;
        L8001C994: ;
        mem.WriteU8(c.S0, (byte)0u);
        L8001C998: ;
        c.S1 = c.S1 - 0x1u;
        c.V0 = 0xFFFFFFFFu;
        if (c.S1 != c.V0) {
            c.S0 = c.S0 + 0xD4u;
            goto L8001C914;
        }
        c.S0 = c.S0 + 0xD4u;
        L8001C9A8: ;
        c.RA = mem.ReadU32((c.SP + 0x20u));
        c.S3 = mem.ReadU32((c.SP + 0x1Cu));
        c.S2 = mem.ReadU32((c.SP + 0x18u));
        c.S1 = mem.ReadU32((c.SP + 0x14u));
        c.S0 = mem.ReadU32((c.SP + 0x10u));
        c.SP = c.SP + 0x28u;
        return;
    }
}
