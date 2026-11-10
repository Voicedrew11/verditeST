using System.Reflection;
using RecompOne.Runtime;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// The map on the GPU: every visible cell is an instance of its mesh, kept in the
/// runtime's stores (<see cref="RetainedMeshes"/>), placed by the matrix and translation
/// <c>ScenePrepare4</c> projects the cell with, lit from the cell's lighting record at
/// its quarter turn and fogged by the record's near and far (runtime 0123's curve).
/// Published before the scene's first face routine runs; the four face routines then
/// run as the game wrote them, and <see cref="PacketFilter"/> takes their packets back
/// off the table, all but the blended faces, which stay packets. A frame whose cells
/// cannot all be published keeps every map packet, counted by reason. See "The
/// retained map" in docs/GPU_RENDERER.md.
/// </summary>
static class RetainedMap
{
    const uint Scratch = 0x1F800000;
    const uint VisibleCount = 0x801B9E14, VisibleList = 0x801B9F16, Cells = 0x8017BED0;
    const uint Turns = 0x8017BDB8, SceneFlags = 0x8017BE8C;

    // SceneTriangles, SceneQuads, MapTriangles, MapQuads: the scene's face routines.
    static readonly uint[] Routines = [0x8001F954, 0x8001FD00, 0x8001D2A8, 0x8001DB64];

    static readonly ModInfo _self = new() { Id = "st.retainedmap", Name = "Retained map", Version = "1.0" };
    static readonly PacketFilter _filter = new();
    static bool _published;
    static long _frames, _cells, _flat, _blendedFaces;

    public static void Install() => HookAttach.OnOverlayLoad("retained map", Attach);

    static bool Attach()
    {
        SymbolRegistry.Build();
        MethodInfo Hook(string n) => typeof(RetainedMap).GetMethod(n, BindingFlags.Public | BindingFlags.Static)!;
        var targets = new List<MethodInfo>();
        foreach (uint a in Routines)
        {
            if (SymbolRegistry.Resolve(MainLoop.Overlay, null, a) is not { } t) return false;
            HookManager.AddPre(_self, t, Hook(nameof(Before)), order: int.MinValue + 2);
            HookManager.AddPost(_self, t, Hook(nameof(After)), order: int.MaxValue - 2);
            targets.Add(t);
        }
        HookManager.Commit();
        return targets.All(HookAttach.Installed);
    }

    public static void Before(CpuContext c, IMemory m)
    {
        // The first face routine: the cells are projected, a0 is their translations.
        if (c.RA == 0x80020DACu) _published = GpuWorld.Capture && Publish(c, m);
        if (_published && GpuWorld.Drawing) _filter.Open(m);
    }

    public static void After(CpuContext c, IMemory m)
    {
        if (_published && GpuWorld.Drawing) _filter.Close(m, PacketFilter.Semi);
    }

    static bool Publish(CpuContext c, IMemory m)
    {
        if ((m.ReadU8(SceneFlags) & 6) != 0) { GpuWorld.Fallback("map", "flicker-recolour"); return false; }
        uint bank = m.ReadU32(Scratch + 0xC), record = m.ReadU32(Scratch + 0x3C4), table = c.A0;
        uint rgb = m.ReadU32(Scratch + 0x44) & 0xFFFFFF;
        int count = m.ReadU16(VisibleCount);
        if (count > 256) { GpuWorld.Fallback("map", "visible-count"); return false; }
        var instances = new RetainedScene.ModelInstance[count];
        var runs = new RetainedMeshes.Run[2];
        // The walk's order, last visible cell first, as the corner buffer is laid out.
        uint corners = m.ReadU32(Scratch + 0x34);
        for (int i = count - 1; i >= 0; i--)
        {
            uint cell = Cells + m.ReadU16(VisibleList + (uint)i * 2) * 12u;
            uint descriptor = bank + 0xC + m.ReadU8(cell) * 0x28u, base_ = bank + 0xC;
            uint faces = base_ + m.ReadU32(descriptor + 0x10);
            int triangles = m.ReadU16(descriptor + 0x18), quads = m.ReadU16(descriptor + 0x1C);
            runs[0] = new(RetainedMeshes.FaceShape.FlatTriangle, faces, triangles);
            runs[1] = new(RetainedMeshes.FaceShape.FlatQuad, faces + (uint)triangles * 0x14, quads);
            uint normals = base_ + m.ReadU32(descriptor + 8), vertices = base_ + m.ReadU32(descriptor);
            int vertexCount = (int)m.ReadU32(descriptor + 4);
            var mesh = RetainedMeshes.Get(m, descriptor, runs, normals, vertices, vertexCount, out string? reason);
            if (mesh == null) { GpuWorld.Fallback("map", reason ?? "mesh"); return false; }
            int turn = m.ReadU8(cell + 3) & 3;
            bool flat = (m.ReadU8(cell + 4) & 3) == 0;
            var inst = new RetainedScene.ModelInstance
            {
                MeshStart = mesh.Start, MeshCount = mesh.Opaque, MeshAll = mesh.Total, Pose = mesh.Pose,
                Rgbc = rgb, Tile = true, Far = 1e30f, Near = -1e30f,
                Dqa = (short)m.ReadU16(record + 0x68), Dqb = (short)m.ReadU16(record + 0x6A),
                Curve = flat ? 0 : LinearFogFactor.Free,
            };
            Place(ref inst, m, Turns + (uint)turn * 0x20, table + (uint)i * 16);
            Light(ref inst, m, record, record + (uint)turn * 0x14);
            instances[count - 1 - i] = inst;
            if (RetainedCheck.On)
                RetainedCheck.Cell(m, inst, vertices, vertexCount, corners, normals, runs, record,
                                   record + (uint)turn * 0x14, flat);
            corners += (uint)vertexCount * 8;
            _flat += flat ? 1 : 0;
            _blendedFaces += mesh.Blended;
        }
        foreach (var inst in instances) RetainedScene.AddInstance(inst);
        _frames++;
        _cells += count;
        return true;
    }

    /// <summary>The GTE's rotation (4.12) from <paramref name="matrix"/> and its
    /// translation (three s32) from <paramref name="translation"/>, kept as integers for
    /// the depth checks and as floats for the draw; the frame's view is the identity.</summary>
    public static void Place(ref RetainedScene.ModelInstance i, IMemory m, uint matrix, uint translation)
    {
        short S(uint k) => (short)m.ReadU16(matrix + k * 2);
        i.V00 = S(0); i.V01 = S(1); i.V02 = S(2);
        i.V10 = S(3); i.V11 = S(4); i.V12 = S(5);
        i.V20 = S(6); i.V21 = S(7); i.V22 = S(8);
        i.Vtx = (int)m.ReadU32(translation); i.Vty = (int)m.ReadU32(translation + 4); i.Vtz = (int)m.ReadU32(translation + 8);
        i.R00 = i.V00 / 4096f; i.R01 = i.V01 / 4096f; i.R02 = i.V02 / 4096f;
        i.R10 = i.V10 / 4096f; i.R11 = i.V11 / 4096f; i.R12 = i.V12 / 4096f;
        i.R20 = i.V20 / 4096f; i.R21 = i.V21 / 4096f; i.R22 = i.V22 / 4096f;
        i.Tx = i.Vtx; i.Ty = i.Vty; i.Tz = i.Vtz;
    }

    /// <summary>The light matrix at <paramref name="llm"/> (five words), and the colour
    /// matrix and back colour of the lighting <paramref name="record"/>, as the GTE takes them.</summary>
    static void Light(ref RetainedScene.ModelInstance i, IMemory m, uint record, uint llm)
    {
        float L(uint at, uint k) => (short)m.ReadU16(at + k * 2);
        i.Llm0 = L(llm, 0) / 4096f; i.Llm1 = L(llm, 1) / 4096f; i.Llm2 = L(llm, 2) / 4096f;
        i.Llm3 = L(llm, 3) / 4096f; i.Llm4 = L(llm, 4) / 4096f; i.Llm5 = L(llm, 5) / 4096f;
        i.Llm6 = L(llm, 6) / 4096f; i.Llm7 = L(llm, 7) / 4096f; i.Llm8 = L(llm, 8) / 4096f;
        uint lcm = record + 0x50;
        i.L0 = L(lcm, 0); i.L1 = L(lcm, 1); i.L2 = L(lcm, 2); i.L3 = L(lcm, 3); i.L4 = L(lcm, 4);
        i.L5 = L(lcm, 5); i.L6 = L(lcm, 6); i.L7 = L(lcm, 7); i.L8 = L(lcm, 8);
        i.Bk0 = m.ReadU8(record + 0x64) << 4; i.Bk1 = m.ReadU8(record + 0x65) << 4; i.Bk2 = m.ReadU8(record + 0x66) << 4;
    }

    public static string Report() =>
        $"{_frames} frames/{_cells} cells ({_flat} flat), packets off {_filter.Removed}/kept {_filter.Kept}";
}
