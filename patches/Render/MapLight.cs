namespace St;

/// <summary>
/// The map's colour arithmetic in plain integers, as the GTE does it for the map's quads
/// (<c>docs/GPU_RENDERER.md</c>, "The map's records and colours"): <c>NCCS</c> once a
/// face, then <c>DPCS</c> once a corner toward the far colour. Written from the GTE's
/// definition, not by calling it, so a shader can do the same sums and the reference
/// can show they are the game's. Every value is the GTE's own width and clamp.
/// </summary>
static class MapLight
{
    /// <summary>A 3x3 matrix of 4.12 fixed point, row major, as the GTE holds one.</summary>
    public readonly record struct Matrix(short M0, short M1, short M2, short M3, short M4, short M5,
                                         short M6, short M7, short M8)
    {
        /// <summary>The five words a light or colour matrix occupies in the lighting record.</summary>
        public static Matrix FromWords(uint w0, uint w1, uint w2, uint w3, uint w4) =>
            new((short)w0, (short)(w0 >> 16), (short)w1, (short)(w1 >> 16), (short)w2,
                (short)(w2 >> 16), (short)w3, (short)(w3 >> 16), (short)w4);
    }

    /// <summary>The inputs of one area's light, as the scene loads them into the GTE.</summary>
    public readonly record struct Inputs(Matrix Colour, int BackR, int BackG, int BackB,
                                         int FarR, int FarG, int FarB);

    static int Ir(long v, bool positive) => (int)Math.Clamp(v, positive ? 0 : -0x8000, 0x7FFF);

    static int Byte(int v) => Math.Clamp(v, 0, 0xFF);

    /// <summary>NCCS: the face's normal through the light matrix, then the colour
    /// matrix plus the back colour, then times the face's RGB. Returns the colour word
    /// with <paramref name="rgbc"/>'s code byte on top.</summary>
    public static uint Lit(in Matrix light, in Inputs a, short nx, short ny, short nz, uint rgbc)
    {
        int l1 = Ir(((long)light.M0 * nx + (long)light.M1 * ny + (long)light.M2 * nz) >> 12, true);
        int l2 = Ir(((long)light.M3 * nx + (long)light.M4 * ny + (long)light.M5 * nz) >> 12, true);
        int l3 = Ir(((long)light.M6 * nx + (long)light.M7 * ny + (long)light.M8 * nz) >> 12, true);
        var c = a.Colour;
        int i1 = Ir((((long)a.BackR << 12) + (long)c.M0 * l1 + (long)c.M1 * l2 + (long)c.M2 * l3) >> 12, true);
        int i2 = Ir((((long)a.BackG << 12) + (long)c.M3 * l1 + (long)c.M4 * l2 + (long)c.M5 * l3) >> 12, true);
        int i3 = Ir((((long)a.BackB << 12) + (long)c.M6 * l1 + (long)c.M7 * l2 + (long)c.M8 * l3) >> 12, true);
        int r = (int)((((long)(rgbc & 0xFF) * i1) << 4) >> 12);
        int g = (int)((((long)((rgbc >> 8) & 0xFF) * i2) << 4) >> 12);
        int b = (int)((((long)((rgbc >> 16) & 0xFF) * i3) << 4) >> 12);
        return Pack(r >> 4, g >> 4, b >> 4, rgbc);
    }

    static uint Pack(int r, int g, int b, uint code) =>
        (uint)(Byte(r) | (Byte(g) << 8) | (Byte(b) << 16)) | (code & 0xFF000000u);

    /// <summary>DPCS: the colour moved toward the far colour by <paramref name="ir0"/> of
    /// 4096. IR0 is used as written, so a corner past the far depth goes past the far
    /// colour and is clamped per channel.</summary>
    public static uint Fogged(uint rgbc, in Inputs a, short ir0) => Pack(
        Toward(rgbc & 0xFF, a.FarR, ir0), Toward((rgbc >> 8) & 0xFF, a.FarG, ir0),
        Toward((rgbc >> 16) & 0xFF, a.FarB, ir0), rgbc);

    static int Toward(uint channel, int far, short ir0)
    {
        long near = (long)channel << 16;
        int step = Ir((((long)far << 12) - near) >> 12, false);
        return (int)(((long)step * ir0 + near) >> 12) >> 4;
    }

    /// <summary>The fog factor a corner's depth gives (<c>ScenePrepare4</c>): linear from
    /// the near depth to the far, truncating, kept in 16 bits and read back signed, 0 when
    /// negative. So a corner far enough past the far depth to pass 32767 wraps and
    /// takes no fog. A mesh's vertices are projected three at a time; the last one or two,
    /// projected singly (<paramref name="single"/>), are also held under 4066.</summary>
    public static int FogFactor(int sz, int near, int far, bool single)
    {
        if (near == far) return 0;
        int f = (short)((sz - near) * 4096 / (far - near));
        if (single && f >= 4066) f = 4065;
        return f < 0 ? 0 : f;
    }
}
