using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_8001C100</c>: the visible-cell list, from the eye's cell outwards. <c>ST_VISIBLECELLS</c>.
/// </summary>
public static class VisibleCells
{
    public static readonly Literal R = new(typeof(VisibleCells), "visible cells", "VISIBLECELLS", 0x8001C100, Run,
        "the visible-cell list, from the eye's cell outwards.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x68u;
        c.A1 = 0u;
        c.A2 = c.A1;
        c.V0 = 0x801B0000u;
        mem.WriteU32((c.SP + 0x60u), c.S0);
        c.S0 = c.V0 + 0x5948u;
        mem.WriteU32((c.SP + 0x64u), c.RA);
        mem.WriteU16((c.S0 + 0x44CAu), (ushort)0u);
        c.RA = 0x8001C124u;
        ShadowTower_st.func_80046270(c, mem);
        c.A0 = c.SP + 0x10u;
        c.A1 = 0u;
        c.A2 = 0x00000010u;
        c.RA = 0x8001C134u;
        ShadowTower_st.func_80014D5C(c, mem);
        c.A3 = mem.ReadU16((c.S0 + 0x44CAu));
        c.V0 = 0xFFFFFFFFu;
        c.A3 = c.A3 - 0x1u;
        if (c.A3 == c.V0) {
            c.V0 = 0x80180000u;
            goto L8001C1A0;
        }
        c.V0 = 0x80180000u;
        c.T2 = c.V0 - 0x2930u;
        c.T1 = c.SP + 0x10u;
        c.V0 = c.A3 << 1;
        c.T0 = c.V0 + c.S0;
        L8001C158: ;
        Poll(c, mem);
        c.A2 = 0u;
        c.V0 = mem.ReadU16((c.T0 + 0x44CEu));
        c.A0 = c.T1;
        c.V0 = c.V0 << 6;
        c.A1 = c.V0 + c.T2;
        L8001C16C: ;
        Poll(c, mem);
        c.V0 = mem.ReadU32(c.A1);
        c.A1 = c.A1 + 0x4u;
        c.V1 = mem.ReadU32(c.A0);
        c.A2 = c.A2 + 0x1u;
        c.V1 = c.V1 | c.V0;
        mem.WriteU32(c.A0, c.V1);
        c.V0 = (int)c.A2 < 16 ? 1u : 0u;
        if (c.V0 != 0u) {
            c.A0 = c.A0 + 0x4u;
            goto L8001C16C;
        }
        c.A0 = c.A0 + 0x4u;
        c.A3 = c.A3 - 0x1u;
        c.V0 = 0xFFFFFFFFu;
        if (c.A3 != c.V0) {
            c.T0 = c.T0 - 0x2u;
            goto L8001C158;
        }
        c.T0 = c.T0 - 0x2u;
        L8001C1A0: ;
        c.V0 = 0x801C0000u;
        mem.WriteU16((c.V0 - 0x61ECu), (ushort)0u);
        c.A2 = 0u;
        c.A3 = c.A2;
        c.T0 = 0x801B0000u;
        c.T2 = c.T0 + 0x5920u;
        c.T1 = 0x00000001u;
        c.V1 = c.SP + 0x10u;
        L8001C1C0: ;
        Poll(c, mem);
        c.A0 = 0u;
        c.V0 = 0x801B0000u;
        c.A1 = c.V0 + 0x5948u;
        L8001C1CC: ;
        Poll(c, mem);
        c.V0 = mem.ReadU32(c.V1);
        c.V0 = c.V0 & 0x0001u;
        if (c.V0 == 0u) {
            goto L8001C224;
        }
        c.V0 = (uint)(short)mem.ReadU16((c.T0 + 0x5920u));
        if (c.V0 != c.T1) {
            goto L8001C200;
        }
        c.V0 = mem.ReadU8((c.T2 + 0x1Du));
        if (c.V0 != 0u) {
            goto L8001C224;
        }
        L8001C200: ;
        c.V0 = mem.ReadU16((c.A1 + 0x44CCu));
        c.V0 = c.V0 << 1;
        c.V0 = c.V0 + c.A1;
        mem.WriteU16((c.V0 + 0x45CEu), (ushort)c.A3);
        c.V0 = mem.ReadU16((c.A1 + 0x44CCu));
        c.V0 = c.V0 + 0x1u;
        mem.WriteU16((c.A1 + 0x44CCu), (ushort)c.V0);
        L8001C224: ;
        c.V0 = mem.ReadU32(c.V1);
        c.A0 = c.A0 + 0x1u;
        c.V0 = c.V0 >> 1;
        mem.WriteU32(c.V1, c.V0);
        c.V0 = (int)c.A0 < 32 ? 1u : 0u;
        if (c.V0 != 0u) {
            c.A3 = c.A3 + 0x1u;
            goto L8001C1CC;
        }
        c.A3 = c.A3 + 0x1u;
        c.A2 = c.A2 + 0x1u;
        c.V0 = (int)c.A2 < 16 ? 1u : 0u;
        if (c.V0 != 0u) {
            c.V1 = c.V1 + 0x4u;
            goto L8001C1C0;
        }
        c.V1 = c.V1 + 0x4u;
        c.RA = mem.ReadU32((c.SP + 0x64u));
        c.S0 = mem.ReadU32((c.SP + 0x60u));
        c.SP = c.SP + 0x68u;
        return;
    }
}
