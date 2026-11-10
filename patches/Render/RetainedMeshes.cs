using System.Runtime.InteropServices;
using RecompOne.Runtime;
using RecompOne.Runtime.Memory;

namespace St;

/// <summary>
/// The meshes the retained renderer draws from, kept on the GPU (the runtime's mesh
/// and pose stores) and built once per content: a mesh's faces as corners in its own
/// space, and, for vertices that hold still, a rigid pose (<see cref="Rigid"/>). The
/// map's cells and the models share the face records (<see cref="FaceShape"/>), so one
/// decoder serves both. A mesh is keyed by its descriptor's address and checked against
/// a hash of its face and normal bytes every time it is asked for: a mesh rewritten in
/// place is built again. The stores are emptied
/// (<see cref="Clear"/>) on an area change and when they pass their bounds.
/// See "The retained map" in docs/GPU_RENDERER.md.
/// </summary>
static class RetainedMeshes
{
    /// <summary>A face record's layout: its size, the u16 byte offsets of its normals
    /// (one, or one a corner) and of its corners' vertices (each a multiple of 8).</summary>
    public sealed record FaceShape(string Name, uint Size, uint[] NormalsAt, uint[] CornersAt)
    {
        public int Corners => CornersAt.Length;

        public static readonly FaceShape FlatTriangle = new("FT3", 0x14, [0xC], [0xE, 0x10, 0x12]);
        public static readonly FaceShape FlatQuad = new("FT4", 0x1C, [0x10], [0x12, 0x14, 0x16, 0x18]);
        public static readonly FaceShape SmoothTriangle = new("GT3", 0x18, [0xC, 0x10, 0x14], [0xE, 0x12, 0x16]);
        public static readonly FaceShape SmoothQuad = new("GT4", 0x20, [0x10, 0x14, 0x18, 0x1C], [0x12, 0x16, 0x1A, 0x1E]);
    }

    /// <summary>A run of face records of one shape.</summary>
    public readonly record struct Run(FaceShape Shape, uint First, int Count);

    /// <summary>One face of a built mesh: its first corner in the store and how many (3,
    /// or 6 for a quad), whether its record is blended, and its record (the run and
    /// the index in it), in the records' order.</summary>
    public readonly record struct Face(int Corner, int Corners, bool Semi, int Run, int Index);

    /// <summary>A built mesh: its corners in the store (opaque first, then blended), and
    /// how many faces it has and how many of them are blended.</summary>
    public sealed class Mesh
    {
        public int Start, Opaque, Total, Vertices, Faces, Blended;
        public Face[] FaceList = [];
        public uint NormalBytes;
        public ulong Hash;
    }

    static readonly Dictionary<uint, Mesh> _meshes = [];
    static readonly Dictionary<(uint, int, ulong), int> _rigid = [];
    static int _generation = -1;
    public static long Builds, Hits, Rebuilds, Clears, RigidBuilds, RigidHits;

    /// <summary>The mesh at <paramref name="key"/> (its descriptor), from its runs of
    /// faces and its normals, for <paramref name="count"/> vertices; null when a record
    /// points outside RAM or past the vertices.</summary>
    public static Mesh? Get(IMemory m, uint key, ReadOnlySpan<Run> runs, uint normals, int count, out string? reason)
    {
        reason = null;
        CheckGeneration();
        if (count <= 0 || count > 4096) { reason = "mesh-vertex-range"; return null; }
        ulong hash = 14695981039346656037ul;
        foreach (var r in runs)
        {
            if (r.Count < 0 || r.Count > 4096 || !InRam(r.First, (uint)r.Count * r.Shape.Size)) { reason = "mesh-face-range"; return null; }
            hash = Fnv(m, r.First, (uint)r.Count * r.Shape.Size, hash);
        }
        if (_meshes.TryGetValue(key, out var mesh) && InRam(normals, mesh.NormalBytes)
            && Fnv(m, normals, mesh.NormalBytes, hash) == mesh.Hash) { Hits++; return mesh; }
        if (mesh != null) Rebuilds++;
        mesh = Build(m, runs, normals, count, out reason);
        if (mesh == null) return null;
        mesh.Hash = Fnv(m, normals, mesh.NormalBytes, hash);
        _meshes[key] = mesh;
        Builds++;
        return mesh;
    }

    /// <summary>Vertices that hold still, as a pose in the store (one past its first
    /// texel), kept by address and content; 0 when they are not in RAM.</summary>
    public static int Rigid(IMemory m, uint vertices, int count)
    {
        CheckGeneration();
        if (count <= 0 || count > 4096 || !InRam(vertices, (uint)count * 8)) return 0;
        var key = (vertices, count, Fnv(m, vertices, (uint)count * 8));
        if (_rigid.TryGetValue(key, out int pose)) { RigidHits++; return pose; }
        pose = RetainedScene.AddPose(Shorts(m, vertices, count)) + 1;
        _rigid[key] = pose;
        RigidBuilds++;
        return pose;
    }

    /// <summary>A mesh's vertices as the stores take them: four shorts each.</summary>
    public static ReadOnlySpan<short> Shorts(IMemory m, uint vertices, int count) =>
        MemoryMarshal.Cast<byte, short>(((PSMemory)m).Ram.Slice((int)(vertices & 0x1FFFFF), count * 8));

    public static void Clear()
    {
        RetainedScene.ClearMeshes();
        CheckGeneration();
        Clears++;
    }

    static void CheckGeneration()
    {
        if (_generation == RetainedScene.MeshGeneration) return;
        _meshes.Clear();
        _rigid.Clear();
        _generation = RetainedScene.MeshGeneration;
    }

    static Mesh? Build(IMemory m, ReadOnlySpan<Run> runs, uint normals, int count, out string? reason)
    {
        reason = null;
        var faces = new List<RetainedScene.Vertex[]>();
        uint normalBytes = 0;
        var semi = new List<bool>();
        var where = new List<(int Run, int Index)>();
        Span<int> vi = stackalloc int[4];
        for (int run = 0; run < runs.Length; run++)
        {
            var r = runs[run];
            var s = r.Shape;
            for (int f = 0; f < r.Count; f++)
            {
                uint rec = r.First + (uint)(f * s.Size);
                for (int k = 0; k < s.Corners; k++)
                {
                    uint offset = m.ReadU16(rec + s.CornersAt[k]);
                    vi[k] = (int)(offset / 8);
                    if ((offset & 7) != 0 || vi[k] >= count) { reason = "mesh-corner-range"; return null; }
                }
                faces.Add(Corners(m, rec, s, normals, vi));
                semi.Add((m.ReadU8(rec + 0xB) & 2) != 0);
                where.Add((run, f));
                foreach (uint at in s.NormalsAt) normalBytes = Math.Max(normalBytes, m.ReadU16(rec + at) + 8u);
            }
        }
        // A writer links each face at the head of its slot, so the walk draws a slot's
        // last-built face first and its first on top: the store holds them last first.
        var opaque = new List<RetainedScene.Vertex>();
        var blended = new List<RetainedScene.Vertex>();
        var placed = new int[faces.Count];
        for (int i = faces.Count - 1; i >= 0; i--)
        {
            var list = semi[i] ? blended : opaque;
            placed[i] = list.Count;
            list.AddRange(faces[i]);
        }
        var mesh = new Mesh
        {
            Opaque = opaque.Count, Total = opaque.Count + blended.Count, Vertices = count,
            Faces = faces.Count, Blended = semi.Count(b => b), NormalBytes = normalBytes,
        };
        if (!InRam(normals, normalBytes)) { reason = "mesh-normal-range"; return null; }
        opaque.AddRange(blended);
        mesh.Start = RetainedScene.AddMesh(CollectionsMarshal.AsSpan(opaque));
        mesh.FaceList = new Face[faces.Count];
        for (int i = 0; i < faces.Count; i++)
            mesh.FaceList[i] = new(mesh.Start + placed[i] + (semi[i] ? mesh.Opaque : 0), faces[i].Length, semi[i],
                                   where[i].Run, where[i].Index);
        return mesh;
    }

    // One face's corners as the store keeps them (RetainedScene.MeshCorners): two
    // triangles a quad, 0-1-2 and 1-3-2, as the GPU splits a quad packet.
    static RetainedScene.Vertex[] Corners(IMemory m, uint rec, FaceShape s, uint normals, ReadOnlySpan<int> vi)
    {
        uint clutUv = m.ReadU32(rec), pageUv = m.ReadU32(rec + 4);
        Span<uint> uv = stackalloc uint[4];
        uv[0] = clutUv & 0xFFFF; uv[1] = pageUv & 0xFFFF; uv[2] = m.ReadU16(rec + 8);
        if (s.Corners == 4) uv[3] = m.ReadU16(rec + 0xC);
        int u0 = 255, v0 = 255, u1 = 0, v1 = 0;
        for (int k = 0; k < s.Corners; k++)
        {
            int u = (int)(uv[k] & 255), v = (int)(uv[k] >> 8);
            u0 = Math.Min(u0, u); u1 = Math.Max(u1, u); v0 = Math.Min(v0, v); v1 = Math.Max(v1, v);
        }
        uint page = (pageUv >> 16) & 0x1FF;
        bool semi = (m.ReadU8(rec + 0xB) & 2) != 0;
        var template = new RetainedScene.Vertex
        {
            Clut = clutUv >> 16 & 0x7FFF, Texpage = page,
            Dqa = vi[0], Dqb = vi[1], Curve = vi[2], Rgbc = s.Corners == 4 ? (uint)vi[3] : uint.MaxValue,
            Rect = (uint)(u0 | v0 << 8 | u1 << 16 | v1 << 24),
            Flags = RetainedScene.FlagDots | RetainedScene.FlagRect
                    | (semi ? RetainedScene.FlagSemi | (page >> 5 & 3) << 8 : 0),
        };
        ReadOnlySpan<int> order = s.Corners == 4 ? [0, 1, 2, 1, 3, 2] : [0, 1, 2];
        var corners = new RetainedScene.Vertex[order.Length];
        for (int j = 0; j < order.Length; j++)
        {
            int k = order[j];
            uint n = normals + m.ReadU16(rec + s.NormalsAt[s.NormalsAt.Length == 1 ? 0 : k]);
            var v = template;
            v.X = vi[k];
            v.R = (short)m.ReadU16(n); v.G = (short)m.ReadU16(n + 2); v.B = (short)m.ReadU16(n + 4);
            v.U = uv[k] & 255; v.V = uv[k] >> 8;
            if (j >= 3) v.Flags |= RetainedScene.FlagQuadTail;
            corners[j] = v;
        }
        return corners;
    }

    public static bool InRam(uint a, uint bytes) =>
        (a & 0xFF000000u) is 0x80000000u or 0 && (a & 0x1FFFFF) + bytes <= 0x200000u;

    static ulong Fnv(IMemory m, uint a, uint bytes, ulong h = 14695981039346656037ul)
    {
        var ram = ((PSMemory)m).Ram.Slice((int)(a & 0x1FFFFF), (int)bytes);
        var words = MemoryMarshal.Cast<byte, ulong>(ram);
        foreach (ulong w in words) h = (h ^ w) * 1099511628211ul;
        for (int i = words.Length * 8; i < ram.Length; i++) h = (h ^ ram[i]) * 1099511628211ul;
        return h;
    }
}
