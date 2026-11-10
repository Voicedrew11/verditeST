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
/// Published before the scene's first face routine runs; the six face routines then
/// run as the game wrote them, and <see cref="PacketFilter"/> takes their packets back
/// off the table (the blended faces too, drawn by <see cref="RetainedBlend"/>). A frame
/// whose cells cannot all be published keeps every map packet, counted by reason. See
/// "The map" in docs/GPU_RETAINED.md.
/// </summary>
static class RetainedMap
{
    const uint Scratch = 0x1F800000;
    const uint VisibleCount = 0x801B9E14, VisibleList = 0x801B9F16, Cells = 0x8017BED0;
    const uint Turns = 0x8017BDB8, SceneFlags = 0x8017BE8C;

    // SceneTriangles, SceneQuads, MapTriangles, MapQuads, then the smooth faces'
    // SceneStep1E5A0 (GT3 records) and SceneStep1EEAC (GT4): the scene's face routines.
    static readonly uint[] Routines = [0x8001F954, 0x8001FD00, 0x8001D2A8, 0x8001DB64, 0x8001E5A0, 0x8001EEAC];

    static readonly ModInfo _self = new() { Id = "st.retainedmap", Name = "Retained map", Version = "1.0" };
    static readonly PacketFilter _filter = new();
    static bool _published, _open;
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
        _open = _published && GpuWorld.Drawing;
        if (_open) _filter.Open(m);
    }

    public static void After(CpuContext c, IMemory m)
    {
        if (!_open) return;
        _open = false;
        _filter.Close(m, RetainedBlend.Enabled ? null : PacketFilter.Semi);
    }

    static bool Publish(CpuContext c, IMemory m)
    {
        uint bank = m.ReadU32(Scratch + 0xC), record = m.ReadU32(Scratch + 0x3C4), table = c.A0;
        uint rgb = m.ReadU32(Scratch + 0x44) & 0xFFFFFF;
        int count = m.ReadU16(VisibleCount);
        if (count > 256) { GpuWorld.Fallback("map", "visible-count"); return false; }
        var instances = new (RetainedScene.ModelInstance Inst, RetainedMeshes.Mesh Mesh, RetainedMeshes.Run[] Runs, uint Corners)[count];
        int nearLimit = (int)m.ReadU32(Scratch + 0x8C);
        // The walk's order, last visible cell first, as the corner buffer is laid out.
        uint corners = m.ReadU32(Scratch + 0x34);
        for (int i = count - 1; i >= 0; i--)
        {
            uint cell = Cells + m.ReadU16(VisibleList + (uint)i * 2) * 12u;
            uint descriptor = bank + 0xC + m.ReadU8(cell) * 0x28u, base_ = bank + 0xC;
            uint faces = base_ + m.ReadU32(descriptor + 0x10);
            bool flat = (m.ReadU8(cell + 4) & 3) == 0;
            var runs = Runs(m, descriptor, faces, flat);
            uint normals = base_ + m.ReadU32(descriptor + 8), vertices = base_ + m.ReadU32(descriptor);
            int vertexCount = (int)m.ReadU32(descriptor + 4);
            // A flat cell's mesh is its flat runs only: a key of its own.
            var mesh = RetainedMeshes.Get(m, flat ? descriptor | 1 : descriptor, runs, normals, vertexCount, out string? reason);
            if (mesh == null) { GpuWorld.Fallback("map", reason ?? "mesh"); return false; }
            int pose = RetainedMeshes.Rigid(m, vertices, vertexCount);
            if (pose == 0) { GpuWorld.Fallback("map", "vertex-range"); return false; }
            int turn = m.ReadU8(cell + 3) & 3;
            var inst = new RetainedScene.ModelInstance
            {
                MeshStart = mesh.Start, MeshCount = mesh.Opaque, MeshAll = mesh.Total, Pose = pose,
                Rgbc = rgb, Tile = true, Far = 1e30f, Near = -1e30f,
                Dqa = (short)m.ReadU16(record + 0x68), Dqb = (short)m.ReadU16(record + 0x6A),
                Curve = flat ? 0 : LinearFogFactor.Free,
            };
            Place(ref inst, m, Turns + (uint)turn * 0x20, table + (uint)i * 16);
            Light(ref inst, m, record, record + (uint)turn * 0x14);
            if (!flat) Flicker.Recolour(ref inst, m.ReadU8(SceneFlags));
            instances[count - 1 - i] = (inst, mesh, runs, corners);
            if (RetainedCheck.On)
                RetainedCheck.Cell(m, inst, vertices, vertexCount, corners, normals, runs, record,
                                   record + (uint)turn * 0x14, flat);
            corners += (uint)vertexCount * 8;
            _flat += flat ? 1 : 0;
            _blendedFaces += mesh.Blended;
        }
        foreach (var (inst, mesh, runs, at) in instances)
        {
            RetainedScene.AddInstance(inst);
            // The map's blended faces at the key of their deepest corner, dropped at
            // 4096 or under the near limit, as the face routines link them.
            if (RetainedBlend.Enabled && mesh.Blended > 0)
                RetainedBlend.Add(m, mesh, runs, at, RetainedBlend.Keying.Deepest, nearLimit - 1, 4096, false, 0);
            else if (mesh.Blended > 0) GpuWorld.Fallback("map", "blended-faces");
        }
        _frames++;
        _cells += count;
        return true;
    }

    /// <summary>A cell's face runs from its descriptor: FT3 and FT4 (<c>+0x18</c>,
    /// <c>+0x1C</c>), then GT3 and GT4 (<c>+0x1A</c>, <c>+0x1E</c>), one after another
    /// from <paramref name="faces"/>. The scene's own flat cells (<c>+4 &amp; 3 == 0</c>)
    /// draw only the first two; the smooth ones are drawn for the others alone.</summary>
    static RetainedMeshes.Run[] Runs(IMemory m, uint descriptor, uint faces, bool flat)
    {
        int ft3 = m.ReadU16(descriptor + 0x18), ft4 = m.ReadU16(descriptor + 0x1C);
        uint gt3At = faces + (uint)ft3 * 0x14 + (uint)ft4 * 0x1C;
        int gt3 = flat ? 0 : m.ReadU16(descriptor + 0x1A), gt4 = flat ? 0 : m.ReadU16(descriptor + 0x1E);
        return
        [
            new(RetainedMeshes.FaceShape.FlatTriangle, faces, ft3),
            new(RetainedMeshes.FaceShape.FlatQuad, faces + (uint)ft3 * 0x14, ft4),
            new(RetainedMeshes.FaceShape.SmoothTriangle, gt3At, gt3),
            new(RetainedMeshes.FaceShape.SmoothQuad, gt3At + (uint)gt3 * 0x18, gt4),
        ];
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
