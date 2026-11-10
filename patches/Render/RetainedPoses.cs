using System.Reflection;
using RecompOne.Runtime;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// The models' poses on the GPU. A pose is a key's vertices plus the fraction of the
/// way to the next: libgte's <c>gteMIMefunc</c> (<c>func_80074A04</c>, called from
/// <c>func_80033CD4</c>) adds <c>(delta * fraction) &gt;&gt; 12</c> to each vertex in
/// place, in 16 bits, which is the runtime's pose blend to the bit (<c>ModelGlsl</c>'s
/// <c>modelPosed</c>). At its entry the vertices still hold the key; the key and the
/// deltas go into the pose store once (kept by their contents), and a part drawn from
/// those vertices afterwards is an instance with that pose and the fraction as its
/// weight: a new pose uploads nothing. A part whose vertices are not exactly what the
/// blend made (another write since, or no blend this frame) uploads its vertices for
/// the frame, counted. See "The retained models" in docs/GPU_RENDERER.md.
/// </summary>
static class RetainedPoses
{
    const uint Blend = 0x80074A04;

    sealed record Pending(uint Vertices, int Count, uint Deltas, int Weight, ulong KeyHash, int Serial, short[] Key);

    static readonly ModInfo _self = new() { Id = "st.retainedposes", Name = "Retained poses", Version = "1.0" };
    static readonly Dictionary<uint, Pending> _pending = [];
    static readonly Dictionary<(ulong, uint, ulong), int> _poses = [];
    static int _generation = -1;
    public static long Blends, Builds, Hits, Uploaded, Stale, Rigid;

    public static void Install() => HookAttach.OnOverlayLoad("retained poses", Attach);

    static bool Attach()
    {
        SymbolRegistry.Build();
        if (SymbolRegistry.Resolve(MainLoop.Overlay, null, Blend) is not { } t) return false;
        HookManager.AddPre(_self, t, typeof(RetainedPoses).GetMethod(nameof(Before), BindingFlags.Public | BindingFlags.Static)!);
        HookManager.Commit();
        return HookAttach.Installed(t);
    }

    /// <summary>gteMIMefunc(vertices a0, deltas a1, count a2, fraction a3).</summary>
    public static void Before(CpuContext c, IMemory m)
    {
        if (!GpuWorld.Capture) return;
        int count = (int)c.A2;
        if (count <= 0 || count > 4096 || !RetainedMeshes.InRam(c.A0, (uint)count * 8)
            || !RetainedMeshes.InRam(c.A1, (uint)count * 8)) return;
        var key = RetainedMeshes.Shorts(m, c.A0, count).ToArray();
        _pending[c.A0] = new(c.A0, count, c.A1, (int)c.A3, Hash(key), RetainedScene.Serial, key);
        Blends++;
    }

    /// <summary>The pose store's entry (one past its first texel) and the weight for a
    /// part drawn from <paramref name="vertices"/>, when they are exactly a blend made
    /// this frame; 0 otherwise.</summary>
    public static int Find(IMemory m, uint vertices, int count, out int weight)
    {
        weight = 0;
        if (_generation != RetainedScene.MeshGeneration)
        {
            _poses.Clear(); _pending.Clear(); _seen.Clear();
            _generation = RetainedScene.MeshGeneration;
        }
        if (!_pending.TryGetValue(vertices, out var p) || p.Serial != RetainedScene.Serial || p.Count != count)
            return 0;
        var now = RetainedMeshes.Shorts(m, vertices, count);
        var deltas = RetainedMeshes.Shorts(m, p.Deltas, count);
        for (int i = 0; i < count * 4; i++)
        {
            if ((i & 3) == 3) continue;
            short want = (short)(p.Key[i] + (short)((deltas[i] * p.Weight) >> 12));
            if (now[i] != want) { Stale++; return 0; }
        }
        var id = (p.KeyHash, p.Deltas, Hash(deltas));
        if (!_poses.TryGetValue(id, out int pose))
        {
            var texels = new short[count * 8];
            for (int v = 0; v < count; v++)
            {
                for (int k = 0; k < 3; k++)
                {
                    texels[v * 8 + k] = p.Key[v * 4 + k];
                    texels[v * 8 + 4 + k] = deltas[v * 4 + k];
                }
            }
            pose = RetainedScene.AddPose(texels) + 1;
            _poses[id] = pose;
            Builds++;
        }
        else Hits++;
        weight = p.Weight;
        return pose;
    }

    static readonly Dictionary<uint, (ulong Hash, int Changes)> _seen = [];

    /// <summary>A rigid pose for vertices that have not changed more than twice at this
    /// address (one past its first texel), or 0 for vertices that keep changing, which
    /// would otherwise add a pose to the store every frame.</summary>
    public static int Still(IMemory m, uint vertices, int count)
    {
        ulong hash = Hash(RetainedMeshes.Shorts(m, vertices, count));
        _seen.TryGetValue(vertices, out var seen);
        if (seen.Hash != hash) seen = (hash, seen.Hash == 0 ? 0 : seen.Changes + 1);
        _seen[vertices] = seen;
        if (seen.Changes > 2) { Uploaded++; return 0; }
        Rigid++;
        return RetainedMeshes.Rigid(m, vertices, count);
    }

    static ulong Hash(ReadOnlySpan<short> s)
    {
        ulong h = 14695981039346656037ul;
        foreach (short v in s) h = (h ^ (ushort)v) * 1099511628211ul;
        return h;
    }

    public static string Report() =>
        $"poses {Blends} blends, {Builds} built/{Hits} kept, {Rigid} rigid, {Uploaded} parts uploaded, {Stale} rewritten after the blend";
}
