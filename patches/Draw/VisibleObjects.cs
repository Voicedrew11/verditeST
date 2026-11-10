using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_8001C9C8</c>: the visible-object list: of the area's 0xF0 object records (0x58 bytes from 0x801C2230), those in a visible cell, up to 0x60, the count at + 0x7274. <c>ST_VISIBLEOBJECTS</c>.
/// </summary>
public static class VisibleObjects
{
    public static readonly Literal R = new(typeof(VisibleObjects), "visible objects", "VISIBLEOBJECTS", 0x8001C9C8, Run,
        "the visible-object list: of the area's 0xF0 object records (0x58 bytes from 0x801C2230), those in a visible cell, up to 0x60, the count at + 0x7274.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x20u;
        c.V0 = 0x801C0000u;
        c.V0 = c.V0 + 0x2230u;
        c.V1 = c.V0 - 0x1F20u;
        mem.WriteU32((c.SP + 0x14u), c.S1);
        c.S1 = 0x000000EFu;
        mem.WriteU32((c.SP + 0x18u), c.S2);
        c.S2 = c.V1;
        mem.WriteU32((c.SP + 0x10u), c.S0);
        c.S0 = c.V0 + 0x7u;
        mem.WriteU32((c.SP + 0x1Cu), c.RA);
        mem.WriteU8((c.V1 + 0x7274u), (byte)0u);
        L8001C9F8: ;
        Poll(c, mem);
        c.V0 = mem.ReadU16((c.S0 + 0x3u));
        c.V0 = c.V0 < 0x00000118u ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001CA88;
        }
        c.V0 = mem.ReadU8((c.S0 - 0x3u));
        c.A0 = mem.ReadU16((c.S0 + 0x1u));
        c.V0 = c.V0 & 0x007Fu;
        mem.WriteU8((c.S0 - 0x3u), (byte)c.V0);
        c.RA = 0x8001CA20u;
        ShadowTower_st.func_8001C88C(c, mem);
        if (c.V0 != 0u) {
            c.V0 = 0x000000ECu;
            goto L8001CA38;
        }
        c.V0 = 0x000000ECu;
        c.V1 = mem.ReadU8((c.S0 - 0x2u));
        if (c.V1 != c.V0) {
            goto L8001CA8C;
        }
        L8001CA38: ;
        c.V0 = mem.ReadU8((c.S2 + 0x7274u));
        c.V0 = c.V0 < 0x00000060u ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001CA9C;
        }
        c.V0 = mem.ReadU8((c.S0 - 0x3u));
        c.V0 = c.V0 | 0x0080u;
        mem.WriteU8((c.S0 - 0x3u), (byte)c.V0);
        c.V0 = 0x000000EFu;
        c.V1 = mem.ReadU8((c.S2 + 0x7274u));
        c.V0 = c.V0 - c.S1;
        c.V1 = c.V1 << 1;
        c.V1 = c.V1 + c.S2;
        mem.WriteU16((c.V1 + 0x71B4u), (ushort)c.V0);
        c.V0 = mem.ReadU8((c.S2 + 0x7274u));
        c.V0 = c.V0 + 0x1u;
        mem.WriteU8((c.S2 + 0x7274u), (byte)c.V0);
        goto L8001CA8C;
        L8001CA88: ;
        mem.WriteU8(c.S0, (byte)0u);
        L8001CA8C: ;
        c.S1 = c.S1 - 0x1u;
        c.V0 = 0xFFFFFFFFu;
        if (c.S1 != c.V0) {
            c.S0 = c.S0 + 0x58u;
            goto L8001C9F8;
        }
        c.S0 = c.S0 + 0x58u;
        L8001CA9C: ;
        c.RA = mem.ReadU32((c.SP + 0x1Cu));
        c.S2 = mem.ReadU32((c.SP + 0x18u));
        c.S1 = mem.ReadU32((c.SP + 0x14u));
        c.S0 = mem.ReadU32((c.SP + 0x10u));
        c.SP = c.SP + 0x20u;
        return;
    }
}
