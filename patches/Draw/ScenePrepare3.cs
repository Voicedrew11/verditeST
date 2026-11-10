using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_8001CB88</c>: the scene's third preparation pass; writes no packet. <c>ST_SCENEPREPARE3</c>.
/// </summary>
public static class ScenePrepare3
{
    public static readonly Literal R = new(typeof(ScenePrepare3), "scene preparation 3", "SCENEPREPARE3", 0x8001CB88, Run,
        "the scene's third preparation pass; writes no packet.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.V0 = 0x801B0000u;
        c.V1 = c.V0 + 0x5948u;
        c.A3 = mem.ReadU16((c.V1 + 0x44CCu));
        c.V0 = 0xFFFFFFFFu;
        c.A3 = c.A3 - 0x1u;
        if (c.A3 == c.V0) {
            c.A1 = 0x1F800000u;
            goto L8001CDF0;
        }
        c.A1 = 0x1F800000u;
        c.T6 = c.V1;
        c.V0 = 0x80180000u;
        c.T5 = c.V0 - 0x4130u;
        c.V0 = 0x801A0000u;
        c.T0 = c.V0 - 0x70D8u;
        c.T2 = 0u | 0x8000u;
        c.T1 = 0u | 0xFFFFu;
        c.T3 = 0x00007FFFu;
        c.V0 = c.A3 << 4;
        c.A2 = c.V0 + c.A0;
        L8001CBCC: ;
        Poll(c, mem);
        c.V0 = c.A3 << 1;
        c.V0 = c.V0 + c.T6;
        c.V0 = mem.ReadU16((c.V0 + 0x45CEu));
        c.A0 = c.V0 << 1;
        c.A0 = c.A0 + c.V0;
        c.A0 = c.A0 << 2;
        c.A0 = c.A0 + c.T5;
        c.V0 = mem.ReadU8((c.A0 + 0x8u));
        c.V0 = c.V0 << 12;
        mem.WriteU32((c.A1 + 0x128u), c.V0);
        c.V1 = mem.ReadU8((c.A0 + 0x9u));
        c.V0 = mem.ReadU8((c.A0 + 0xBu));
        c.V1 = c.V1 << 12;
        c.V0 = c.V0 - 0x80u;
        c.V0 = c.V0 << 7;
        c.V1 = c.V1 + c.V0;
        c.V0 = mem.ReadU32((c.A1 + 0x128u));
        mem.WriteU32((c.A1 + 0x12Cu), c.V1);
        c.V1 = mem.ReadU32((c.T0 + 0x248u));
        c.A0 = mem.ReadU8((c.A0 + 0xAu));
        mem.WriteU32((c.A1 + 0x20Cu), 0u);
        c.V0 = c.V0 - c.V1;
        mem.WriteU32((c.A1 + 0x128u), c.V0);
        c.V0 = (uint)(short)mem.ReadU16((c.T0 + 0x2CEu));
        c.V1 = mem.ReadU32((c.T0 + 0x24Cu));
        c.A0 = c.A0 << 12;
        mem.WriteU32((c.A1 + 0x130u), c.A0);
        c.A0 = (uint)(short)mem.ReadU16((c.T0 + 0x2D2u));
        c.V1 = c.V1 + c.V0;
        c.V0 = mem.ReadU32((c.A1 + 0x12Cu));
        c.V1 = c.V1 + c.A0;
        c.V0 = c.V0 - c.V1;
        c.V1 = mem.ReadU32((c.A1 + 0x130u));
        mem.WriteU32((c.A1 + 0x12Cu), c.V0);
        c.A0 = mem.ReadU32((c.T0 + 0x250u));
        c.V0 = mem.ReadU32((c.A1 + 0x128u));
        c.V1 = c.V1 - c.A0;
        c.V0 = c.V0 + c.T2;
        c.V0 = c.T1 < c.V0 ? 1u : 0u;
        if (c.V0 != 0u) {
            mem.WriteU32((c.A1 + 0x130u), c.V1);
            goto L8001CCA0;
        }
        mem.WriteU32((c.A1 + 0x130u), c.V1);
        c.V1 = mem.ReadU32((c.A1 + 0x12Cu));
        c.V0 = (int)c.T3 < (int)c.V1 ? 1u : 0u;
        if (c.V0 != 0u) {
            c.V0 = (int)c.V1 < -32768 ? 1u : 0u;
            goto L8001CCA0;
        }
        c.V0 = (int)c.V1 < -32768 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L8001CCA0;
        }
        c.V1 = mem.ReadU32((c.A1 + 0x130u));
        c.V0 = (int)c.T3 < (int)c.V1 ? 1u : 0u;
        goto L8001CD08;
        L8001CCA0: ;
        Poll(c, mem);
        c.V0 = mem.ReadU32((c.A1 + 0x128u));
        c.V1 = mem.ReadU32((c.A1 + 0x130u));
        c.V0 = (uint)((int)c.V0 >> 1);
        mem.WriteU32((c.A1 + 0x128u), c.V0);
        c.V0 = mem.ReadU32((c.A1 + 0x12Cu));
        c.V1 = (uint)((int)c.V1 >> 1);
        mem.WriteU32((c.A1 + 0x130u), c.V1);
        c.V1 = mem.ReadU32((c.A1 + 0x20Cu));
        c.V0 = (uint)((int)c.V0 >> 1);
        mem.WriteU32((c.A1 + 0x12Cu), c.V0);
        c.V0 = mem.ReadU32((c.A1 + 0x128u));
        c.V1 = c.V1 + 0x1u;
        c.V0 = c.V0 + c.T2;
        c.V0 = c.T1 < c.V0 ? 1u : 0u;
        if (c.V0 != 0u) {
            mem.WriteU32((c.A1 + 0x20Cu), c.V1);
            goto L8001CCA0;
        }
        mem.WriteU32((c.A1 + 0x20Cu), c.V1);
        c.V1 = mem.ReadU32((c.A1 + 0x12Cu));
        c.A0 = 0x00007FFFu;
        c.V0 = (int)c.A0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 != 0u) {
            c.V0 = (int)c.V1 < -32768 ? 1u : 0u;
            goto L8001CCA0;
        }
        c.V0 = (int)c.V1 < -32768 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L8001CCA0;
        }
        c.V1 = mem.ReadU32((c.A1 + 0x130u));
        c.V0 = (int)c.A0 < (int)c.V1 ? 1u : 0u;
        L8001CD08: ;
        if (c.V0 != 0u) {
            c.V0 = (int)c.V1 < -32768 ? 1u : 0u;
            goto L8001CCA0;
        }
        c.V0 = (int)c.V1 < -32768 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L8001CCA0;
        }
        c.V0 = mem.ReadU16((c.A1 + 0x128u));
        c.V1 = mem.ReadU16((c.A1 + 0x12Cu));
        c.A0 = mem.ReadU16((c.A1 + 0x130u));
        mem.WriteU16((c.A1 + 0x120u), (ushort)c.V0);
        c.V0 = c.A1 + 0x120u;
        mem.WriteU16((c.A1 + 0x122u), (ushort)c.V1);
        mem.WriteU16((c.A1 + 0x124u), (ushort)c.A0);
        RecompOne.Runtime.Gte.Write(0, (mem.ReadU32(c.V0)));
        RecompOne.Runtime.Gte.Write(1, (mem.ReadU32((c.V0 + 0x4u))));
        RecompOne.Runtime.Gte.MvmvaOp(12, false, 0, 0, 0);
        c.V0 = c.A1 + 0xA8u;
        mem.WriteU32(c.V0, (RecompOne.Runtime.Gte.Read(25)));
        mem.WriteU32((c.V0 + 0x4u), (RecompOne.Runtime.Gte.Read(26)));
        mem.WriteU32((c.V0 + 0x8u), (RecompOne.Runtime.Gte.Read(27)));
        c.V0 = c.A1 + 0x58u;
        c.T4 = RecompOne.Runtime.Gte.ReadControl(31);
        mem.WriteU32(c.V0, c.T4);
        c.V0 = mem.ReadU32((c.A1 + 0x20Cu));
        mem.WriteU32((c.A2 + 0xCu), c.V0);
        c.V1 = mem.ReadU32((c.A1 + 0x20Cu));
        c.V0 = 0xFFFFFFFFu;
        c.V1 = c.V1 - 0x1u;
        if (c.V1 == c.V0) {
            mem.WriteU32((c.A1 + 0x20Cu), c.V1);
            goto L8001CDC0;
        }
        mem.WriteU32((c.A1 + 0x20Cu), c.V1);
        c.A0 = c.V0;
        L8001CD8C: ;
        Poll(c, mem);
        c.V0 = mem.ReadU32((c.A1 + 0xA8u));
        c.V1 = mem.ReadU32((c.A1 + 0xACu));
        c.V0 = c.V0 << 1;
        mem.WriteU32((c.A1 + 0xA8u), c.V0);
        c.V0 = mem.ReadU32((c.A1 + 0xB0u));
        c.V1 = c.V1 << 1;
        mem.WriteU32((c.A1 + 0xACu), c.V1);
        c.V1 = mem.ReadU32((c.A1 + 0x20Cu));
        c.V0 = c.V0 << 1;
        c.V1 = c.V1 - 0x1u;
        mem.WriteU32((c.A1 + 0xB0u), c.V0);
        if (c.V1 != c.A0) {
            mem.WriteU32((c.A1 + 0x20Cu), c.V1);
            goto L8001CD8C;
        }
        mem.WriteU32((c.A1 + 0x20Cu), c.V1);
        L8001CDC0: ;
        c.V0 = mem.ReadU32((c.A1 + 0xA8u));
        mem.WriteU32(c.A2, c.V0);
        c.V0 = mem.ReadU32((c.A1 + 0xACu));
        mem.WriteU32((c.A2 + 0x4u), c.V0);
        c.V0 = mem.ReadU32((c.A1 + 0xB0u));
        c.A3 = c.A3 - 0x1u;
        mem.WriteU32((c.A2 + 0x8u), c.V0);
        c.V0 = 0xFFFFFFFFu;
        if (c.A3 != c.V0) {
            c.A2 = c.A2 - 0x10u;
            goto L8001CBCC;
        }
        c.A2 = c.A2 - 0x10u;
        L8001CDF0: ;
        return;
    }
}
