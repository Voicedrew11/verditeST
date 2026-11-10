using RecompOne.Runtime.Memory;

namespace St;

/// <summary>
/// The map's face shapes as <see cref="MapReference"/> walks them: which cells the
/// routine draws, where a mesh's faces of the shape start and how many there are, the
/// record's layout, the packet it becomes and where each value goes in it, and the
/// routine and near split that write it. A new shape is a new entry. The records are
/// in <c>docs/GPU_RENDERER.md</c>, "The map's records and colours".
/// </summary>
sealed record MapFaces(
    string Name,
    uint Routine,
    uint? NearSplit,                     // none: a face of any size takes the bulk path
    Func<int, bool> DrawsCell,           // by the cell record's +4 & 3
    Func<IMemory, uint, uint> First,     // (memory, the mesh's descriptor) -> the first record, from bank + 0xC
    uint CountAt,                        // u16 in the descriptor
    uint RecordSize,
    int Corners,
    uint NormalAt,                       // u16 byte offset into the normals
    uint CornersAt,                      // u16 byte offsets into the projected corners, one a corner
    uint CodeBits,                       // or-ed into the record's +0xB for the GP0 code
    uint Length,                         // the tag's length, in words after the tag
    MapFaces.Shading Colouring,
    int[] Colours,                       // packet words: one a corner (Fogged) or one (Flat)
    int[] Positions,                     // packet words: the screen XY, one a corner
    (int Word, uint At, bool Half)[] Texture)
{
    /// <summary>Fogged: NCCS once, then DPCS a corner by its fog factor (GT3, GT4).
    /// Flat: the NCCS colour alone, no fog (FT3, FT4).</summary>
    public enum Shading { Fogged, Flat }

    public uint PacketSize => (Length + 1) * 4;

    // The descriptor's faces offset, from bank + 0xC; the bank's base is added by the walk.
    static uint Faces(IMemory m, uint mesh) => m.ReadU32(mesh + 0x10);
    static uint AfterTriangles(IMemory m, uint mesh) => Faces(m, mesh) + m.ReadU16(mesh + 0x18) * 0x14u;
    static bool Lit(int flags) => flags != 0;
    static bool Plain(int flags) => flags == 0;

    static readonly (int, uint, bool)[] TriangleUv = [(3, 0x0, false), (6, 0x4, false), (9, 0x8, true)];
    static readonly (int, uint, bool)[] QuadUv = [.. TriangleUv, (12, 0xC, true)];

    /// <summary>GT3s, <c>MapTriangles</c> (<c>func_8001D2A8</c>): a mesh's faces start with them.</summary>
    public static readonly MapFaces Triangles = new("triangles", 0x8001D2A8, 0x80075EC4, Lit,
        Faces, 0x18, 0x14, 3, 0xC, 0xE, 0x34, 9, Shading.Fogged, [1, 4, 7], [2, 5, 8], TriangleUv);

    /// <summary>GT4s, <c>MapQuads</c> (<c>func_8001DB64</c>): after the triangles.</summary>
    public static readonly MapFaces Quads = new("quads", 0x8001DB64, 0x80076884, Lit,
        AfterTriangles, 0x1C, 0x1C, 4, 0x10, 0x12, 0x3C, 0xC, Shading.Fogged, [1, 4, 7, 10], [2, 5, 8, 11], QuadUv);

    /// <summary>FT3s, <c>SceneTriangles</c> (<c>func_8001F954</c>): the same records, in
    /// the cells the map routines skip.</summary>
    public static readonly MapFaces FlatTriangles = new("flat triangles", 0x8001F954, null, Plain,
        Faces, 0x18, 0x14, 3, 0xC, 0xE, 0x24, 7, Shading.Flat, [1], [2, 4, 6],
        [(3, 0x0, false), (5, 0x4, false), (7, 0x8, true)]);

    /// <summary>FT4s, <c>SceneQuads</c> (<c>func_8001FD00</c>).</summary>
    public static readonly MapFaces FlatQuads = new("flat quads", 0x8001FD00, null, Plain,
        AfterTriangles, 0x1C, 0x1C, 4, 0x10, 0x12, 0x2C, 9, Shading.Flat, [1], [2, 4, 6, 8],
        [(3, 0x0, false), (5, 0x4, false), (7, 0x8, true), (9, 0xC, true)]);

    public static readonly MapFaces[] All = [Triangles, Quads, FlatTriangles, FlatQuads];
}
