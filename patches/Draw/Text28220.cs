using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_80028220</c>: text for func_80028784; not read. <c>ST_TEXT28220</c>.
/// </summary>
public static class Text28220
{
    public static readonly Literal R = new(typeof(Text28220), "text-28220", "TEXT28220", 0x80028220, Run,
        "text for func_80028784; not read.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.T0 = c.A0;
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.V1 = 0u | 0x8000u;
        c.T1 = c.V0 + c.V1;
        c.A0 = mem.ReadU32((c.T1 + 0x304u));
        c.T2 = c.A1;
        c.A3 = mem.ReadU32((c.A0 + 0x8u));
        c.V0 = mem.ReadU32((c.A0 + 0x4u));
        c.A1 = c.A3 + 0x18u;
        c.V0 = c.V0 < c.A1 ? 1u : 0u;
        if (c.V0 != 0u) {
            c.V1 = c.A2 & 0xFFFFu;
            goto L8002835C;
        }
        c.V1 = c.A2 & 0xFFFFu;
        mem.WriteU32((c.A0 + 0x8u), c.A1);
        c.V0 = mem.ReadU32((c.T1 + 0x328u));
        c.V0 = c.V0 + 0x1u;
        mem.WriteU32((c.T1 + 0x328u), c.V0);
        c.V0 = 0x00000001u;
        if (c.V1 != c.V0) {
            c.V0 = 0x00000028u;
            goto L80028278;
        }
        c.V0 = 0x00000028u;
        c.V0 = 0x0000002Au;
        L80028278: ;
        mem.WriteU8((c.A3 + 0x7u), (byte)c.V0);
        c.A0 = 0x80160000u;
        c.A0 = c.A0 - 0x2F70u;
        c.V0 = 0u | 0x8000u;
        c.A0 = c.A0 + c.V0;
        c.A1 = 0x00FF0000u;
        c.V0 = mem.ReadU32((c.A0 + 0xE4u));
        c.A1 = c.A1 | 0xFFFFu;
        c.V0 = mem.ReadU32(c.V0);
        c.V1 = 0x05000000u;
        c.V0 = c.V0 & c.A1;
        c.V0 = c.V0 | c.V1;
        mem.WriteU32(c.A3, c.V0);
        c.V0 = mem.ReadU16(c.T0);
        mem.WriteU16((c.A3 + 0x8u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.T0 + 0x2u));
        mem.WriteU16((c.A3 + 0xAu), (ushort)c.V0);
        c.V0 = mem.ReadU16(c.T0);
        c.V1 = mem.ReadU16((c.T0 + 0x4u));
        c.V0 = c.V0 + c.V1;
        mem.WriteU16((c.A3 + 0xCu), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.T0 + 0x2u));
        mem.WriteU16((c.A3 + 0xEu), (ushort)c.V0);
        c.V0 = mem.ReadU16(c.T0);
        mem.WriteU16((c.A3 + 0x10u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.T0 + 0x2u));
        c.V1 = mem.ReadU16((c.T0 + 0x6u));
        c.V0 = c.V0 + c.V1;
        mem.WriteU16((c.A3 + 0x12u), (ushort)c.V0);
        c.V0 = mem.ReadU16(c.T0);
        c.V1 = mem.ReadU16((c.T0 + 0x4u));
        c.V0 = c.V0 + c.V1;
        mem.WriteU16((c.A3 + 0x14u), (ushort)c.V0);
        c.V0 = mem.ReadU16((c.T0 + 0x2u));
        c.V1 = mem.ReadU16((c.T0 + 0x6u));
        c.V0 = c.V0 + c.V1;
        mem.WriteU16((c.A3 + 0x16u), (ushort)c.V0);
        c.V0 = mem.ReadU8(c.T2);
        mem.WriteU8((c.A3 + 0x4u), (byte)c.V0);
        c.V0 = mem.ReadU8((c.T2 + 0x1u));
        mem.WriteU8((c.A3 + 0x5u), (byte)c.V0);
        c.V0 = mem.ReadU8((c.T2 + 0x2u));
        mem.WriteU8((c.A3 + 0x6u), (byte)c.V0);
        c.V0 = mem.ReadU32((c.A0 + 0xE4u));
        c.A1 = c.A3 & c.A1;
        mem.WriteU32(c.V0, c.A1);
        L8002835C: ;
        return;
    }
}
