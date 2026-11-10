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
    int TpageWord)                       // the texture word that takes the caller's blend bits
{
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

    public static readonly ModelFaces[] All = [FlatTriangles, FlatQuads, SmoothTriangles, SmoothQuads];
}
