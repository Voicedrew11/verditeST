using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_80026228</c>: a number on the HUD, split into digits by 1000, 100 and 10; one call per active entry of the panel table at 0x8007F3F4, and two from func_80026508. <c>ST_HUDNUMBER</c>.
/// </summary>
public static class HudNumber
{
    public static readonly Literal R = new(typeof(HudNumber), "HUD number", "HUDNUMBER", 0x80026228, Run,
        "a number on the HUD, split into digits by 1000, 100 and 10; one call per active entry of the panel table at 0x8007F3F4, and two from func_80026508.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.T1 = c.A1;
        c.T2 = 0x0000270Fu;
        c.V0 = (int)c.A0 < 10000 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.T0 = c.A2;
            goto L80026240;
        }
        c.T0 = c.A2;
        c.T2 = c.A0;
        L80026240: ;
        c.V0 = c.T2 << 16;
        c.V1 = (uint)((int)c.V0 >> 16);
        c.V0 = (int)c.V1 < 10 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.A0 = 0x80080000u;
            goto L800262A4;
        }
        c.A0 = 0x80080000u;
        c.A0 = c.A0 - 0xC0Cu;
        c.V0 = c.T1 << 2;
        c.V0 = c.V0 + c.T1;
        c.V0 = c.V0 << 2;
        c.V0 = c.V0 + c.A0;
        c.V1 = c.T1 + 0x1u;
        mem.WriteU8(c.V0, (byte)0u);
        c.V0 = c.V1 << 2;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 2;
        c.V0 = c.V0 + c.A0;
        c.V1 = c.T1 + 0x2u;
        mem.WriteU8(c.V0, (byte)0u);
        c.V0 = c.V1 << 2;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 2;
        c.V0 = c.V0 + c.A0;
        c.V1 = c.T1 + 0x3u;
        mem.WriteU8(c.V0, (byte)0u);
        goto L80026380;
        L800262A4: ;
        c.V0 = (int)c.V1 < 100 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.A0 = c.A0 - 0xC0Cu;
            goto L800262E4;
        }
        c.A0 = c.A0 - 0xC0Cu;
        c.V0 = c.T1 << 2;
        c.V0 = c.V0 + c.T1;
        c.V0 = c.V0 << 2;
        c.V0 = c.V0 + c.A0;
        c.V1 = c.T1 + 0x1u;
        mem.WriteU8(c.V0, (byte)0u);
        c.V0 = c.V1 << 2;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 2;
        c.V0 = c.V0 + c.A0;
        c.V1 = c.T1 + 0x2u;
        mem.WriteU8(c.V0, (byte)0u);
        goto L8002635C;
        L800262E4: ;
        c.V0 = (int)c.V1 < 1000 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.A0 = 0x80080000u;
            goto L80026310;
        }
        c.A0 = 0x80080000u;
        c.A0 = c.A0 - 0xC0Cu;
        c.V0 = c.T1 << 2;
        c.V0 = c.V0 + c.T1;
        c.V0 = c.V0 << 2;
        c.V0 = c.V0 + c.A0;
        c.V1 = c.T1 + 0x1u;
        mem.WriteU8(c.V0, (byte)0u);
        goto L80026338;
        L80026310: ;
        c.A0 = c.A0 - 0xC0Cu;
        c.V0 = c.T1 << 2;
        c.V0 = c.V0 + c.T1;
        c.V0 = c.V0 << 2;
        c.V0 = c.V0 + c.A0;
        c.V1 = mem.ReadU8(c.V0);
        c.V1 = c.V1 | 0x0001u;
        mem.WriteU8(c.V0, (byte)c.V1);
        c.V1 = c.T1 + 0x1u;
        L80026338: ;
        c.V0 = c.V1 << 2;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 2;
        c.V0 = c.V0 + c.A0;
        c.V1 = mem.ReadU8(c.V0);
        c.V1 = c.V1 | 0x0001u;
        mem.WriteU8(c.V0, (byte)c.V1);
        c.V1 = c.T1 + 0x2u;
        L8002635C: ;
        c.V0 = c.V1 << 2;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 2;
        c.V0 = c.V0 + c.A0;
        c.V1 = mem.ReadU8(c.V0);
        c.V1 = c.V1 | 0x0001u;
        mem.WriteU8(c.V0, (byte)c.V1);
        c.V1 = c.T1 + 0x3u;
        L80026380: ;
        c.V0 = c.V1 << 2;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 2;
        c.V0 = c.V0 + c.A0;
        c.V1 = mem.ReadU8(c.V0);
        c.V1 = c.V1 | 0x0001u;
        mem.WriteU8(c.V0, (byte)c.V1);
        c.V1 = 0x10620000u;
        c.V1 = c.V1 | 0x4DD3u;
        c.A0 = c.T2 << 16;
        c.V0 = (uint)((int)c.A0 >> 16);
        c.LO = (uint)((long)(int)c.V0 * (int)c.V1); c.HI = (uint)(((long)(int)c.V0 * (int)c.V1) >> 32);
        c.A3 = 0x51EB0000u;
        c.A3 = c.A3 | 0x851Fu;
        c.A0 = (uint)((int)c.A0 >> 31);
        c.V0 = c.HI;
        c.V1 = (uint)((int)c.V0 >> 6);
        c.V1 = c.V1 - c.A0;
        c.V1 = c.V1 << 16;
        c.V1 = (uint)((int)c.V1 >> 16);
        c.A1 = c.V1 << 5;
        c.A1 = c.A1 - c.V1;
        c.A1 = c.A1 << 2;
        c.A1 = c.A1 + c.V1;
        c.A1 = c.A1 << 3;
        c.A1 = c.T2 - c.A1;
        c.A2 = c.A1 << 16;
        c.V0 = (uint)((int)c.A2 >> 16);
        c.LO = (uint)((long)(int)c.V0 * (int)c.A3); c.HI = (uint)(((long)(int)c.V0 * (int)c.A3) >> 32);
        c.T0 = c.T0 & 0x00FFu;
        c.V0 = c.HI;
        c.A3 = 0x66660000u;
        c.A3 = c.A3 | 0x6667u;
        c.LO = (uint)((long)(int)c.V1 * (int)c.T0); c.HI = (uint)(((long)(int)c.V1 * (int)c.T0) >> 32);
        c.A2 = (uint)((int)c.A2 >> 31);
        c.A0 = (uint)((int)c.V0 >> 5);
        c.A0 = c.A0 - c.A2;
        c.A0 = c.A0 << 16;
        c.A0 = (uint)((int)c.A0 >> 16);
        c.V0 = c.A0 << 1;
        c.V0 = c.V0 + c.A0;
        c.V0 = c.V0 << 3;
        c.V0 = c.V0 + c.A0;
        c.V0 = c.V0 << 2;
        c.A1 = c.A1 - c.V0;
        c.V1 = c.LO;
        c.A2 = c.A1 << 16;
        c.V0 = (uint)((int)c.A2 >> 16);
        c.LO = (uint)((long)(int)c.V0 * (int)c.A3); c.HI = (uint)(((long)(int)c.V0 * (int)c.A3) >> 32);
        c.A2 = (uint)((int)c.A2 >> 31);
        c.T2 = c.HI;
        c.A3 = 0x80080000u;
        c.A3 = c.A3 - 0xC0Cu;
        c.LO = (uint)((long)(int)c.A0 * (int)c.T0); c.HI = (uint)(((long)(int)c.A0 * (int)c.T0) >> 32);
        c.V0 = c.T1 << 2;
        c.V0 = c.V0 + c.T1;
        c.V0 = c.V0 << 2;
        c.V0 = c.V0 + c.A3;
        c.V1 = c.V1 + 0x60u;
        mem.WriteU8((c.V0 + 0x2u), (byte)c.V1);
        c.V1 = c.T1 + 0x1u;
        c.V0 = c.V1 << 2;
        c.V0 = c.V0 + c.V1;
        c.V1 = (uint)((int)c.T2 >> 2);
        c.V1 = c.V1 - c.A2;
        c.T4 = c.LO;
        c.V1 = c.V1 << 16;
        c.V1 = (uint)((int)c.V1 >> 16);
        c.LO = (uint)((long)(int)c.V1 * (int)c.T0); c.HI = (uint)(((long)(int)c.V1 * (int)c.T0) >> 32);
        c.V0 = c.V0 << 2;
        c.V0 = c.V0 + c.A3;
        c.A0 = c.T4 + 0x60u;
        mem.WriteU8((c.V0 + 0x2u), (byte)c.A0);
        c.A0 = c.T1 + 0x2u;
        c.V0 = c.V1 << 2;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 1;
        c.A1 = c.A1 - c.V0;
        c.A2 = c.LO;
        c.A1 = c.A1 << 16;
        c.A1 = (uint)((int)c.A1 >> 16);
        c.LO = (uint)((long)(int)c.A1 * (int)c.T0); c.HI = (uint)(((long)(int)c.A1 * (int)c.T0) >> 32);
        c.V0 = c.A0 << 2;
        c.V0 = c.V0 + c.A0;
        c.V0 = c.V0 << 2;
        c.V0 = c.V0 + c.A3;
        c.V1 = c.A2 + 0x60u;
        mem.WriteU8((c.V0 + 0x2u), (byte)c.V1);
        c.V1 = c.T1 + 0x3u;
        c.V0 = c.V1 << 2;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 2;
        c.V0 = c.V0 + c.A3;
        c.T0 = c.LO;
        c.V1 = c.T0 + 0x60u;
        mem.WriteU8((c.V0 + 0x2u), (byte)c.V1);
        return;
    }
}
