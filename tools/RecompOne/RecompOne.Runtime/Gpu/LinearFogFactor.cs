namespace RecompOne.Runtime;

/// <summary>0123. A fog factor linear in depth on the CPU's own integers, 4096 at the
/// far depth: <c>(SZ - near) * 4096 / (far - near)</c>, SZ read back as a signed
/// short, the quotient truncated toward zero, kept in 16 bits and read back signed,
/// then 0 when negative. Not clamped at 4096: past the far depth the colour goes past
/// the far colour and the channel's clamp stops it, as DPCS does with an IR0 the game
/// wrote itself. <see cref="Held"/> also holds it under 4066, for vertices a game
/// projects one at a time and caps. A corner's cue carries near and far as its DQA and
/// DQB. Both retained shaders compose <see cref="Glsl"/>; no game addresses or policy.</summary>
public static class LinearFogFactor
{
    /// <summary>The curve numbers in <see cref="RetainedScene.Vertex.Curve"/>'s numbering:
    /// held under 4066, and not held.</summary>
    public const uint Held = 6, Free = 7;

    public static bool Is(int curve) => curve == Held || curve == Free;

    public static float Weight(float depth, float near, float far, bool held)
    {
        int n = (int)near, d = (int)far - n;
        if (d == 0) return 0f;
        int sz = (short)(int)Math.Clamp(MathF.Floor(depth), 0f, 65535f);
        int f = (short)((sz - n) * 4096 / d);
        if (held && f >= 4066) f = 4065;
        return Math.Max(f, 0);
    }

    public const string Glsl = """
        // 0123. LinearFogFactor: integers throughout, the division truncating toward
        // zero whatever the signs (GLSL leaves a negative operand's quotient open).
        float linearFogFactor(float depth, vec2 pair, bool held) {
            int n = int(pair.x), d = int(pair.y) - n;
            if (d == 0) return 0.0;
            int sz = (int(clamp(floor(depth), 0.0, 65535.0)) << 16) >> 16;
            int num = (sz - n) * 4096;
            int q = abs(num) / abs(d); // int-ok: d != 0 is tested above
            if ((num < 0) != (d < 0)) q = -q;
            int f = (q << 16) >> 16;
            if (held && f >= 4066) f = 4065;
            return float(max(f, 0));
        }
        """;
}
