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
        var (i1, i2, i3) = Light(light, a, nx, ny, nz);
        return Pack(Times(rgbc, 0, i1) >> 4, Times(rgbc, 8, i2) >> 4, Times(rgbc, 16, i3) >> 4, rgbc);
    }

    /// <summary>NCDS: <see cref="Lit"/>'s light and colour, then moved toward the far
    /// colour by <paramref name="ir0"/> of 4096 in one step, as DPCS does.</summary>
    public static uint LitFogged(in Matrix light, in Inputs a, short nx, short ny, short nz, uint rgbc, short ir0)
    {
        var (i1, i2, i3) = Light(light, a, nx, ny, nz);
        return Pack(Toward(Raw(rgbc, 0, i1), a.FarR, ir0), Toward(Raw(rgbc, 8, i2), a.FarG, ir0),
                    Toward(Raw(rgbc, 16, i3), a.FarB, ir0), rgbc);
    }

    // The colour times the light, unshifted: what NCDS interpolates from.
    static long Raw(uint rgbc, int shift, int ir) => ((long)((rgbc >> shift) & 0xFF) * ir) << 4;

    // The same, >> 12: the GTE's MAC, a byte << 4.
    static int Times(uint rgbc, int shift, int ir) => (int)(Raw(rgbc, shift, ir) >> 12);

    static (int, int, int) Light(in Matrix light, in Inputs a, short nx, short ny, short nz)
    {
        int l1 = Ir(((long)light.M0 * nx + (long)light.M1 * ny + (long)light.M2 * nz) >> 12, true);
        int l2 = Ir(((long)light.M3 * nx + (long)light.M4 * ny + (long)light.M5 * nz) >> 12, true);
        int l3 = Ir(((long)light.M6 * nx + (long)light.M7 * ny + (long)light.M8 * nz) >> 12, true);
        var c = a.Colour;
        return (Ir((((long)a.BackR << 12) + (long)c.M0 * l1 + (long)c.M1 * l2 + (long)c.M2 * l3) >> 12, true),
                Ir((((long)a.BackG << 12) + (long)c.M3 * l1 + (long)c.M4 * l2 + (long)c.M5 * l3) >> 12, true),
                Ir((((long)a.BackB << 12) + (long)c.M6 * l1 + (long)c.M7 * l2 + (long)c.M8 * l3) >> 12, true));
    }

    static uint Pack(int r, int g, int b, uint code) =>
        (uint)(Byte(r) | (Byte(g) << 8) | (Byte(b) << 16)) | (code & 0xFF000000u);

    /// <summary>DPCS: the colour moved toward the far colour by <paramref name="ir0"/> of
    /// 4096. IR0 is used as written, so a corner past the far depth goes past the far
    /// colour and is clamped per channel.</summary>
    public static uint Fogged(uint rgbc, in Inputs a, short ir0) => Pack(
        Toward((long)(rgbc & 0xFF) << 16, a.FarR, ir0), Toward((long)((rgbc >> 8) & 0xFF) << 16, a.FarG, ir0),
        Toward((long)((rgbc >> 16) & 0xFF) << 16, a.FarB, ir0), rgbc);

    // The GTE's interpolation from near (MAC before the shift, 4.12 of a byte << 4)
    // toward far by ir0, then to a byte.
    static int Toward(long near, int far, short ir0)
    {
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

    /// <summary>The flickering light's rewrite of a fogged corner colour, after DPCS or
    /// NCDS (<c>func_8001DAE0</c>/<c>8001DB20</c> for the map, <c>func_80018440</c>/
    /// <c>80018480</c> for the models): with <c>0x8017BE8C &amp; 2</c> the blue held under
    /// 0x56 and the colour made (3b, 2b, b), warm; else with <c>&amp; 4</c> the red held
    /// and the colour made (r, 3r, 3r), cold. The code byte is kept.</summary>
    public static uint Flicker(uint rgbc, int sceneFlags)
    {
        uint code = rgbc & 0xFF000000u;
        if ((sceneFlags & 2) != 0)
        {
            uint b = Math.Min((rgbc >> 16) & 0xFF, 0x55u);
            return code | (b << 16) | ((2 * b) << 8) | (3 * b);
        }
        if ((sceneFlags & 4) != 0)
        {
            uint r = Math.Min(rgbc & 0xFF, 0x55u);
            return code | ((3 * r) << 16) | ((3 * r) << 8) | r;
        }
        return rgbc;
    }
}
