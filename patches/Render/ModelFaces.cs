namespace St;

/// <summary>
/// The models' face writers as <see cref="ModelReference"/> checks them: the record's
/// layout, the order key's divisor, and where each value goes in the packet. One entry
/// a writer; the unfogged four (<c>ModelStep19DC8</c>'s) are in. See "The face
/// writers" in docs/GPU_MODELS.md.
/// </summary>
sealed record ModelFaces(
    string Name,
    uint Writer,
    uint RecordSize,
    uint[] NormalsAt,                    // u16 byte offsets into the normals (scratchpad +0x24): one, or one a corner
    uint[] CornersAt,                    // u16 byte offsets into the projected corners (scratchpad +0x40)
    uint CodeBits,                       // or-ed with the record's +0xB and the caller's semi bit
    uint Length,
    int[] Colours,                       // packet words: one (flat) or one a corner (Gouraud)
    int[] Positions,
    (int Word, uint At, bool Half)[] Texture,
    int TpageWord,                       // the texture word that takes the caller's blend bits
    ModelFaces.Fogging Fog = ModelFaces.Fogging.None,
    uint? NearSplit = null,               // libgte's division for a face whose NCLIP passes 16384; none: no near path
    ModelFaces.Facing Faces = ModelFaces.Facing.FirstHalf)
{
    /// <summary>How a fogged writer forms IR0 from the corners' fog factors f0..f3.
    /// None: NCCS/NCCT, no fog. Mean: (f0 + f1 + f2) / 3 or (f0 + .. + f3) >> 2, one for
    /// the face. FirstTwice: (2 f0 + f1 + f2) >> 2, a smooth triangle's. MeanThenFirst: a
    /// smooth quad's, the four's mean for the NCDT of corners 0 to 2, then f0 alone for
    /// corner 3's own NCDS.</summary>
    public enum Fogging { None, Mean, FirstTwice, MeanThenFirst }

    /// <summary>FirstHalf: kept when NCLIP of corners 0-1-2 is positive. BothHalves, a
    /// mixed quad's, as the map's: or when NCLIP of 3-1-2 is negative; and near when
    /// either passes 16384 the other way.</summary>
    public enum Facing { FirstHalf, BothHalves }

    /// <summary>The mixed writers fog by Mean unless <c>0x8017BE8C &amp; 0x10</c>, which
    /// takes them to NCCS, unfogged.</summary>
    public bool FogsBySceneFlag => NearSplit != null;

    /// <summary>IR0 for the colour of corner <paramref name="k"/>.</summary>
    public short Ir0(ReadOnlySpan<short> f, int k) => Fog switch
    {
        Fogging.Mean => (short)(f.Length == 3 ? (f[0] + f[1] + f[2]) / 3 : (f[0] + f[1] + f[2] + f[3]) >> 2),
        Fogging.FirstTwice => (short)((2 * f[0] + f[1] + f[2]) >> 2),
        Fogging.MeanThenFirst => k == 3 ? f[0] : (short)((f[0] + f[1] + f[2] + f[3]) >> 2),
        _ => 0,
    };

    public uint PacketSize => (Length + 1) * 4;

    public int Corners => CornersAt.Length;

    public static readonly ModelFaces FlatTriangles = new("model FT3", 0x800194C4, 0x14,
        [0xC], [0xE, 0x10, 0x12], 0x24, 7, [1], [2, 4, 6],
        [(3, 0x0, false), (5, 0x4, false), (7, 0x8, true)], 5);

    public static readonly ModelFaces FlatQuads = new("model FT4", 0x800196C0, 0x1C,
        [0x10], [0x12, 0x14, 0x16, 0x18], 0x2C, 9, [1], [2, 4, 6, 8],
        [(3, 0x0, false), (5, 0x4, false), (7, 0x8, true), (9, 0xC, true)], 5);

    public static readonly ModelFaces SmoothTriangles = new("model GT3", 0x800198F4, 0x18,
        [0xC, 0x10, 0x14], [0xE, 0x12, 0x16], 0x34, 9, [1, 4, 7], [2, 5, 8],
        [(3, 0x0, false), (6, 0x4, false), (9, 0x8, true)], 6);

    public static readonly ModelFaces SmoothQuads = new("model GT4", 0x80019B34, 0x20,
        [0x10, 0x14, 0x18, 0x1C], [0x12, 0x16, 0x1A, 0x1E], 0x3C, 0xC, [1, 4, 7, 10], [2, 5, 8, 11],
        [(3, 0x0, false), (6, 0x4, false), (9, 0x8, true), (12, 0xC, true)], 6);

    // ModelStep192D4's: the same records and packets, fogged.
    public static readonly ModelFaces FoggedFlatTriangles = FlatTriangles with
        { Name = "fogged model FT3", Writer = 0x800184C4, Fog = Fogging.Mean };
    public static readonly ModelFaces FoggedFlatQuads = FlatQuads with
        { Name = "fogged model FT4", Writer = 0x80018828, Fog = Fogging.Mean };
    public static readonly ModelFaces FoggedSmoothTriangles = SmoothTriangles with
        { Name = "fogged model GT3", Writer = 0x80018BA0, Fog = Fogging.FirstTwice };
    public static readonly ModelFaces FoggedSmoothQuads = SmoothQuads with
        { Name = "fogged model GT4", Writer = 0x80018F48, Fog = Fogging.MeanThenFirst };

    // ModelStep1BF10's: fogged unless the scene flag says not, with a near path, and the
    // smooth faces' IR0 a plain mean (no corner counted twice, no fourth corner apart).
    public static readonly ModelFaces MixedFlatTriangles = FoggedFlatTriangles with
        { Name = "mixed model FT3", Writer = 0x8001A038, NearSplit = 0x80075AC4 };
    public static readonly ModelFaces MixedFlatQuads = FoggedFlatQuads with
        { Name = "mixed model FT4", Writer = 0x8001A6EC, NearSplit = 0x80076364, Faces = Facing.BothHalves };
    public static readonly ModelFaces MixedSmoothTriangles = FoggedSmoothTriangles with
        { Name = "mixed model GT3", Writer = 0x8001AE7C, NearSplit = 0x80075EC4, Fog = Fogging.Mean };
    public static readonly ModelFaces MixedSmoothQuads = FoggedSmoothQuads with
        { Name = "mixed model GT4", Writer = 0x8001B5DC, NearSplit = 0x80076884, Faces = Facing.BothHalves,
          Fog = Fogging.Mean };

    public static readonly ModelFaces[] All =
    [
        FlatTriangles, FlatQuads, SmoothTriangles, SmoothQuads,
        FoggedFlatTriangles, FoggedFlatQuads, FoggedSmoothTriangles, FoggedSmoothQuads,
        MixedFlatTriangles, MixedFlatQuads, MixedSmoothTriangles, MixedSmoothQuads,
    ];
}
