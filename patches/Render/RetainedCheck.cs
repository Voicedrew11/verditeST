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

    static readonly List<string> _samples = [];
    static long _droppedFar, _droppedNear;
    static int _nearLimit;
    static long _vertices, _xyOff, _xySaturated, _xyNear, _zOff, _fogSame, _fogHeld, _fogOff, _faces, _lightOff, _lightOff2;
    static double _worstXy, _worstZ;
    static int _worstFog, _worstLight;
    static long _at;

    /// <summary>One published cell: <paramref name="corners"/> is its run of the
    /// corner buffer, 8 bytes a vertex.</summary>
    public static void Cell(IMemory m, in RetainedScene.ModelInstance i, uint vertices, int count, uint corners,
                            uint normals, ReadOnlySpan<RetainedMeshes.Run> runs, uint record, uint llm, bool flat)
    {
        float h = GteDepth.ProjH, cx = GteDepth.ProjCx, cy = GteDepth.ProjCy;
        int near = (short)m.ReadU16(record + 0x68), far = (short)m.ReadU16(record + 0x6A);
        for (int v = 0; v < count; v++)
        {
            uint p = vertices + (uint)v * 8, q = corners + (uint)v * 8;
            float x = (short)m.ReadU16(p), y = (short)m.ReadU16(p + 2), z = (short)m.ReadU16(p + 4);
            float ex = i.R00 * x + i.R01 * y + i.R02 * z + i.Tx;
            float ey = i.R10 * x + i.R11 * y + i.R12 * z + i.Ty;
            float ez = i.R20 * x + i.R21 * y + i.R22 * z + i.Tz;
            int gx = (short)m.ReadU16(q), gy = (short)m.ReadU16(q + 2), gz = m.ReadU16(q + 4);
            _vertices++;
            double dz = Math.Abs(Math.Clamp(ez, 0, 65535) - gz);
            if (dz > 1) _zOff++;
            _worstZ = Math.Max(_worstZ, dz);
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
                    if (saturated) { if (dxy > 1) _xySaturated++; }
                    // The GTE truncates the view position to whole units before the
                    // divide, a unit being H / z pixels: near the eye that is more than one.
                    else if (dxy > 1 + 2 * h / ez)
                    {
                        _xyOff++;
                        if (_samples.Count < 12 && _xyOff % 500 == 1)
                            _samples.Add($"v{v}/{count} game ({gx},{gy},{gz}) gpu ({sx:F1},{sy:F1},{ez:F1}) " +
                                         $"t ({i.Vtx},{i.Vty},{i.Vtz}) v ({x},{y},{z})");
                    }
                    else if (dxy > 1) _xyNear++;
                    if (!saturated) _worstXy = Math.Max(_worstXy, dxy - 2 * h / ez);
                }
            }
            if (flat) continue;
            int game = (short)m.ReadU16(q + 6), gpu = (int)LinearFogFactor.Weight(gz, near, far, held: false);
            if (game == gpu) _fogSame++;
            else if (game == 4065 && gpu >= 4066) _fogHeld++;
            else { _fogOff++; _worstFog = Math.Max(_worstFog, Math.Abs(game - gpu)); }
        }
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
                uint n = normals + m.ReadU16(r.First + (uint)f * r.Shape.Size + r.Shape.NormalsAt[0]);
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
        Console.WriteLine($"[ST] retained check: {_vertices} vertices, screen off by more than a pixel and two units {_xyOff} " +
                          $"(worst {_worstXy:F2} past that; {_xyNear} within two units, {_xySaturated} where the GTE saturates), " +
                          $"depth >1 {_zOff} (worst {_worstZ:F2}); fog same {_fogSame}, held 4065 {_fogHeld}, " +
                          $"off {_fogOff} (worst {_worstFog}); dropped by key far {_droppedFar}, near with a corner in front {_droppedNear} " +
                          $"(near limit {_nearLimit}); {_faces} faces lit, off {_lightOff} (>1: {_lightOff2}, worst {_worstLight})");
    }
}
