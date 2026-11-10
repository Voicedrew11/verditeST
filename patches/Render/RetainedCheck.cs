using RecompOne.Runtime;
using RecompOne.Runtime.Memory;

namespace St;

/// <summary>
/// ST_GPU_WORLD_CHECK=1: the retained map against the game's own numbers, every frame
/// it is published. For each visible cell, each vertex is placed as the world shader
/// places it (the instance's float matrix and translation, then H x / z about the
/// screen centre) and compared with the screen XY and depth <c>ScenePrepare4</c> wrote
/// to the corner buffer; its fog factor (runtime 0123's curve, from that depth) with
/// the factor written beside it; and each face's colour as the shader lights it (float
/// LLM, LCM, BK) with <see cref="MapLight.Lit"/>'s integers. Counts and worst cases
/// every five seconds. A diagnostic; inspection of the GPU's own output is not this.
/// </summary>
static class RetainedCheck
{
    public static bool On = Environment.GetEnvironmentVariable("ST_GPU_WORLD_CHECK") == "1";

    /// <summary>One family's placement and fog counts.</summary>
    sealed class Tally(string name)
    {
        public readonly string Name = name;
        public long Vertices, XyOff, XySaturated, XyNear, ZOff, FogSame, FogHeld, FogOff;
        public double WorstXy, WorstZ;
        public int WorstFog;
        public override string ToString() =>
            $"{Name}: {Vertices} vertices, screen off by more than a pixel and two units {XyOff} " +
            $"(worst {WorstXy:F2} past that; {XyNear} within two units, {XySaturated} where the GTE saturates), " +
            $"depth >1 {ZOff} (worst {WorstZ:F2}); fog same {FogSame}, held 4065 {FogHeld}, off {FogOff} (worst {WorstFog})";
    }

    static readonly Tally _map = new("map"), _models = new("models");
    static readonly List<string> _samples = [];
    static long _droppedFar, _droppedNear, _keyAgree, _keyGameOnly, _keyGpuOnly;
    static int _nearLimit;
    static long _faces, _lightOff, _lightOff2;
    static int _worstLight;
    static long _at, _zSamples;

    /// <summary>One published model part, at its step's entry: the corner buffer at
    /// the scratchpad's <c>+0x40</c> holds its vertices' screen XY, depth and fog.</summary>
    public static void Model(IMemory m, in RetainedScene.ModelInstance i, uint vertices, int count, uint corners, bool fog,
                             string who)
    {
        long before = _models.ZOff;
        Vertices(_models, m, i, vertices, count, corners, (int)i.Dqa, (int)i.Dqb, fog, held: true);
        if (_models.ZOff > before && _samples.Count < 12 && _zSamples++ % 200 == 0)
            _samples.Add($"models depth off in {AgentBeacon.Map}: {who}, {_models.ZOff - before} of {count} vertices, " +
                         $"t ({i.Vtx},{i.Vty},{i.Vtz}), corners 0x{corners:X8}, vertices 0x{vertices:X8}");
        // The table's range on the faces' mean depth: lo < (mean >> 5) + bias < hi,
        // against the instance's Near <= mean of (SZ >> 2) < Far, both from the same corners.
        uint bank = m.ReadU32(0x1F80000C), descriptor = m.ReadU32(0x1F800020);
        uint faces = bank + 0xC + m.ReadU32(descriptor + 0x10);
        int lo = (int)m.ReadU32(0x1F800080), hi = (int)m.ReadU32(0x1F800004);
        var shapes = new (RetainedMeshes.FaceShape Shape, int Count)[]
        {
            (RetainedMeshes.FaceShape.FlatTriangle, m.ReadU16(descriptor + 0x18)), (RetainedMeshes.FaceShape.FlatQuad, m.ReadU16(descriptor + 0x1C)),
            (RetainedMeshes.FaceShape.SmoothTriangle, m.ReadU16(descriptor + 0x1A)), (RetainedMeshes.FaceShape.SmoothQuad, m.ReadU16(descriptor + 0x1E)),
        };
        foreach (var (shape, n) in shapes)
            for (int f = 0; f < n; f++, faces += shape.Size)
            {
                int sum = 0, quarters = 0;
                foreach (uint at in shape.CornersAt)
                {
                    int sz = m.ReadU16(corners + m.ReadU16(faces + at) + 4);
                    sum += sz; quarters += sz >> 2;
                }
                int key = (shape.Corners == 4 ? sum >> 7 : (sum / 3) >> 5) + (sbyte)m.ReadU8(faces + 0xA);
                int z = shape.Corners == 4 ? quarters >> 2 : quarters / 3;
                bool game = key > lo && key < hi, gpu = z > 0 && z >= i.Near && z < i.Far;
                if (game == gpu) _keyAgree++;
                else if (game) _keyGameOnly++;
                else _keyGpuOnly++;
            }
    }

    static void Vertices(Tally t, IMemory m, in RetainedScene.ModelInstance i, uint vertices, int count, uint corners,
                         int near, int far, bool fog, bool held)
    {
        float h = GteDepth.ProjH, cx = GteDepth.ProjCx, cy = GteDepth.ProjCy;
        for (int v = 0; v < count; v++)
        {
            uint p = vertices + (uint)v * 8, q = corners + (uint)v * 8;
            float x = (short)m.ReadU16(p), y = (short)m.ReadU16(p + 2), z = (short)m.ReadU16(p + 4);
            float ex = i.R00 * x + i.R01 * y + i.R02 * z + i.Tx;
            float ey = i.R10 * x + i.R11 * y + i.R12 * z + i.Ty;
            float ez = i.R20 * x + i.R21 * y + i.R22 * z + i.Tz;
            int gx = (short)m.ReadU16(q), gy = (short)m.ReadU16(q + 2), gz = m.ReadU16(q + 4);
            t.Vertices++;
            double dz = Math.Abs(Math.Clamp(ez, 0, 65535) - gz);
            if (dz > 1) t.ZOff++;
            t.WorstZ = Math.Max(t.WorstZ, dz);
            // Only where the GTE's divide does not saturate is the screen position the
            // projection's; elsewhere the GPU clips at its near plane instead.
            if (ez > h / 2 && gz > h / 2)
            {
                double sx = cx + h * ex / ez, sy = cy + h * ey / ez;
                // Where the screen can show it, even at 16:9: further out the GTE's
                // reciprocal, good to about one part in 2^15, is a pixel or two off.
                if (Math.Abs(sx - cx) <= 320 && Math.Abs(sy - cy) <= 240)
                {
                    double dxy = Math.Max(Math.Abs(sx - gx), Math.Abs(sy - gy));
                    // The GTE saturates the view-space X and Y at 16 bits before the
                    // divide; the GPU does not, so there the game's place is the wrong one.
                    bool saturated = Math.Abs(ex) > 32767 || Math.Abs(ey) > 32767;
                    if (saturated) { if (dxy > 1) t.XySaturated++; }
                    // The GTE truncates the view position to whole units before the
                    // divide, a unit being H / z pixels: near the eye that is more than one.
                    else if (dxy > 1 + 2 * h / ez)
                    {
                        t.XyOff++;
                        if (_samples.Count < 12 && t.XyOff % 500 == 1)
                            _samples.Add($"{t.Name} v{v}/{count} game ({gx},{gy},{gz}) gpu ({sx:F1},{sy:F1},{ez:F1}) " +
                                         $"t ({i.Vtx},{i.Vty},{i.Vtz}) v ({x},{y},{z})");
                    }
                    else if (dxy > 1) t.XyNear++;
                    if (!saturated) t.WorstXy = Math.Max(t.WorstXy, dxy - 2 * h / ez);
                }
            }
            if (!fog) continue;
            int game = (short)m.ReadU16(q + 6), gpu = (int)LinearFogFactor.Weight(gz, near, far, held);
            if (game == gpu) t.FogSame++;
            else if (game == 4065 && gpu >= 4066) t.FogHeld++;
            else { t.FogOff++; t.WorstFog = Math.Max(t.WorstFog, Math.Abs(game - gpu)); }
        }
    }

    /// <summary>One published cell: <paramref name="corners"/> is its run of the
    /// corner buffer, 8 bytes a vertex.</summary>
    public static void Cell(IMemory m, in RetainedScene.ModelInstance i, uint vertices, int count, uint corners,
                            uint normals, ReadOnlySpan<RetainedMeshes.Run> runs, uint record, uint llm, bool flat)
    {
        int near = (short)m.ReadU16(record + 0x68), far = (short)m.ReadU16(record + 0x6A);
        Vertices(_map, m, i, vertices, count, corners, near, far, !flat, held: false);
        var light = MapLight.Matrix.FromWords(m.ReadU32(llm), m.ReadU32(llm + 4), m.ReadU32(llm + 8),
                                               m.ReadU32(llm + 12), m.ReadU32(llm + 16));
        uint w = record + 0x50;
        var inputs = new MapLight.Inputs(MapLight.Matrix.FromWords(m.ReadU32(w), m.ReadU32(w + 4), m.ReadU32(w + 8),
            m.ReadU32(w + 12), m.ReadU32(w + 16)), m.ReadU8(record + 0x64) << 4, m.ReadU8(record + 0x65) << 4,
            m.ReadU8(record + 0x66) << 4, 0, 0, 0);
        int nearLimit = (int)m.ReadU32(0x1F80008C);
        _nearLimit = nearLimit;
        foreach (var r in runs)
            for (int f = 0; f < r.Count; f++)
            {
                // The table's drop: the largest corner depth >> 5 plus the face's bias,
                // 4096 or more, or under the scratchpad's near limit. The GPU keeps such
                // a face (its near plane and the screen clip it instead).
                uint rec = r.First + (uint)f * r.Shape.Size;
                int deepest = 0;
                foreach (uint at in r.Shape.CornersAt)
                    deepest = Math.Max(deepest, m.ReadU16(corners + m.ReadU16(rec + at) + 4));
                int key = (deepest >> 5) + (sbyte)m.ReadU8(rec + 0xA);
                if (key >= 4096) _droppedFar++;
                else if (key < nearLimit && deepest > 0) _droppedNear++;
                // One normal a flat face, one a corner a smooth one.
                foreach (uint normalAt in r.Shape.NormalsAt)
                {
                    uint n = normals + m.ReadU16(rec + normalAt);
                    short nx = (short)m.ReadU16(n), ny = (short)m.ReadU16(n + 2), nz = (short)m.ReadU16(n + 4);
                    uint want = MapLight.Lit(light, inputs, nx, ny, nz, i.Rgbc);
                    int off = 0;
                    for (int ch = 0; ch < 3; ch++)
                        off = Math.Max(off, Math.Abs((int)((want >> (8 * ch)) & 0xFF) - Shader(i, nx, ny, nz, ch)));
                    _faces++;
                    if (off > 0) _lightOff++;
                    if (off > 1) _lightOff2++;
                    _worstLight = Math.Max(_worstLight, off);
                }
            }
    }

    // The world shader's model light (GlShaders WorldVs, FlagDots), in floats.
    static int Shader(in RetainedScene.ModelInstance i, float nx, float ny, float nz, int ch)
    {
        float a0 = Math.Clamp(i.Llm0 * nx + i.Llm1 * ny + i.Llm2 * nz, 0, 32767);
        float a1 = Math.Clamp(i.Llm3 * nx + i.Llm4 * ny + i.Llm5 * nz, 0, 32767);
        float a2 = Math.Clamp(i.Llm6 * nx + i.Llm7 * ny + i.Llm8 * nz, 0, 32767);
        float bk = ch == 0 ? i.Bk0 : ch == 1 ? i.Bk1 : i.Bk2;
        float l = ch == 0 ? i.L0 * a0 + i.L1 * a1 + i.L2 * a2 : ch == 1 ? i.L3 * a0 + i.L4 * a1 + i.L5 * a2
                : i.L6 * a0 + i.L7 * a1 + i.L8 * a2;
        float ir = Math.Clamp(bk + l / 4096f, 0, 32767);
        float c = (i.Rgbc >> (8 * ch)) & 0xFF;
        return (int)Math.Clamp(MathF.Floor(c * ir / 4096f), 0, 255);
    }

    public static void Report()
    {
        if (!On || Environment.TickCount64 < _at) return;
        _at = Environment.TickCount64 + 5000;
        foreach (var line in _samples) Console.WriteLine($"[ST] retained check sample: {line}");
        _samples.Clear();
        Console.WriteLine($"[ST] retained check {_map}; dropped by key far {_droppedFar}, near with a corner in front " +
                          $"{_droppedNear} (near limit {_nearLimit}); {_faces} faces lit, off {_lightOff} (>1: {_lightOff2}, worst {_worstLight})");
        Console.WriteLine($"[ST] retained check {_models}; faces kept alike {_keyAgree}, by the game only {_keyGameOnly}, " +
                          $"by the GPU only {_keyGpuOnly}");
    }
}
