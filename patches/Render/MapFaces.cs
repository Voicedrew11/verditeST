using RecompOne.Runtime.Memory;

namespace St;

/// <summary>
/// The map's two face shapes as <see cref="MapReference"/> walks them: where a mesh's
/// faces of the shape start and how many there are, the record's layout, the packet it
/// becomes, and the routine and near split that write it. A new shape is a new entry.
/// The records are in <c>docs/GPU_RENDERER.md</c>, "The map's records and colours".
/// </summary>
sealed record MapFaces(
    string Name,
    uint Routine,
    uint NearSplit,
    Func<IMemory, uint, uint> First,     // (memory, the mesh's descriptor) -> the first record, from bank + 0xC
    uint CountAt,                        // u16 in the descriptor
    uint RecordSize,
    int Corners,
    uint NormalAt,                       // u16 byte offset into the normals
    uint CornersAt,                      // u16 byte offsets into the projected corners, one a corner
    uint CodeBits,                       // or-ed into the record's +0xB for the GP0 code
    uint Length,                         // the tag's length, in words after the tag
    (int Word, uint At, bool Half)[] Texture)
{
    public uint PacketSize => (Length + 1) * 4;

    // The descriptor's faces offset, from bank + 0xC; the bank's base is added by the walk.
    static uint Faces(IMemory m, uint mesh) => m.ReadU32(mesh + 0x10);

    /// <summary>GT3s, <c>MapTriangles</c> (<c>func_8001D2A8</c>): the mesh's faces start
    /// with them.</summary>
    public static readonly MapFaces Triangles = new("triangles", 0x8001D2A8, 0x80075EC4,
        Faces, 0x18, 0x14, 3, 0xC, 0xE, 0x34, 9,
        [(3, 0x0, false), (6, 0x4, false), (9, 0x8, true)]);

    /// <summary>GT4s, <c>MapQuads</c> (<c>func_8001DB64</c>): after the triangles.</summary>
    public static readonly MapFaces Quads = new("quads", 0x8001DB64, 0x80076884,
        (m, mesh) => Faces(m, mesh) + m.ReadU16(mesh + 0x18) * 0x14u, 0x1C, 0x1C, 4, 0x10, 0x12, 0x3C, 0xC,
        [(3, 0x0, false), (6, 0x4, false), (9, 0x8, true), (12, 0xC, true)]);

    public static readonly MapFaces[] All = [Triangles, Quads];
}
