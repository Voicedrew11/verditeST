using RecompOne.Runtime;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;

namespace St;

/// <summary>The instructions the transcriptions spell as calls, exactly as the
/// recompiler emits them.</summary>
public static class Mips
{
    /// <summary><c>div</c> with the compiler's zero and overflow checks after it: LO
    /// and HI left alone by a zero divisor (its <c>break</c> is a no-op), and
    /// <c>at</c> left at 0x80000000 by the overflow check.</summary>
    public static void Div(CpuContext c, uint s, uint t)
    {
        if (t != 0u)
        {
            if ((int)s == int.MinValue && (int)t == -1) { c.LO = 0x80000000u; c.HI = 0u; }
            else { c.LO = (uint)((int)s / (int)t); c.HI = (uint)((int)s % (int)t); }
        }
        c.At = 0x80000000u;
    }

    /// <summary><c>divu</c>, with its zero check (a no-op <c>break</c>).</summary>
    public static void DivU(CpuContext c, uint s, uint t)
    {
        if (t != 0u) { c.LO = s / t; c.HI = s % t; }
    }

    public static void Mult(CpuContext c, uint s, uint t)
    {
        long r = (long)(int)s * (int)t;
        c.LO = (uint)r;
        c.HI = (uint)(r >> 32);
    }

    public static void MultU(CpuContext c, uint s, uint t)
    {
        ulong r = (ulong)s * t;
        c.LO = (uint)r;
        c.HI = (uint)(r >> 32);
    }

    /// <summary>The interrupt poll the recompiler puts at every loop head.</summary>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void Poll(CpuContext c, PSMemory mem) => Interrupts.Poll(c, mem);
}
