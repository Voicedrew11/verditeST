using System.Reflection;
using RecompOne.Runtime;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// ST_MAP_REFERENCE=1, the models' half: before each face writer in
/// <see cref="ModelFaces"/> runs, its faces are worked out in plain C# from its
/// arguments (a0 the count, a1 the records, a3 the caller's blend and semi bits), the
/// scratchpad's buffers and the GTE's light inputs as the caller left them, with
/// <see cref="MapLight"/>'s arithmetic; after it, the packets are compared word by word
/// with each tag's link, each key's head and the cursor. So this checks the writers'
/// arithmetic, not how the inputs were made (that is "The light inputs" in
/// docs/GPU_MODELS.md). Counts go out with <see cref="MapReference"/>'s report.
/// </summary>
public static class ModelReference
{
    const uint Scratch = 0x1F800000;

    readonly record struct Expected(int Key, uint[] Words);

    static readonly ModInfo _self = new() { Id = "st.modelreference", Name = "Model reference", Version = "1.0" };
    static readonly List<Expected> _expected = [];
    static readonly Dictionary<int, uint> _headBefore = [];
    static ModelFaces _shape = ModelFaces.FlatTriangles;
    static uint _cursor, _ot;
    static bool _colours;

    public static void Install(string? spec)
    {
        if (spec?.Trim() != "1") return;
        HookAttach.OnOverlayLoad("model reference", Attach);
    }

    static bool Attach()
    {
        SymbolRegistry.Build();
        MethodInfo Hook(string n) => typeof(ModelReference).GetMethod(n, BindingFlags.Public | BindingFlags.Static)!;
        var targets = new List<MethodInfo>();
        for (int i = 0; i < ModelFaces.All.Length; i++)
        {
            if (SymbolRegistry.Resolve(MainLoop.Overlay, null, ModelFaces.All[i].Writer) is not { } t) return false;
            HookManager.AddPre(_self, t, Hook("Pre" + i));
            HookManager.AddPost(_self, t, Hook(nameof(Post)));
            targets.Add(t);
        }
        HookManager.Commit();
        bool on = targets.All(HookAttach.Installed);
        Console.WriteLine($"[ST] model reference: {(on ? "on" : "not attached")}, {ModelFaces.All.Length} writers");
        return on;
    }

    // One per entry of ModelFaces.All, by its index.
    public static void Pre0(CpuContext c, IMemory m) => Pre(ModelFaces.All[0], c, m);
    public static void Pre1(CpuContext c, IMemory m) => Pre(ModelFaces.All[1], c, m);
    public static void Pre2(CpuContext c, IMemory m) => Pre(ModelFaces.All[2], c, m);
    public static void Pre3(CpuContext c, IMemory m) => Pre(ModelFaces.All[3], c, m);
    public static void Pre4(CpuContext c, IMemory m) => Pre(ModelFaces.All[4], c, m);
    public static void Pre5(CpuContext c, IMemory m) => Pre(ModelFaces.All[5], c, m);
    public static void Pre6(CpuContext c, IMemory m) => Pre(ModelFaces.All[6], c, m);
    public static void Pre7(CpuContext c, IMemory m) => Pre(ModelFaces.All[7], c, m);

    static MapLight.Matrix Control(int first) => MapLight.Matrix.FromWords(Gte.ReadControl(first),
        Gte.ReadControl(first + 1), Gte.ReadControl(first + 2), Gte.ReadControl(first + 3), Gte.ReadControl(first + 4));

    static void Pre(ModelFaces shape, CpuContext c, IMemory m)
    {
        _shape = shape;
        _expected.Clear();
        _headBefore.Clear();
        _ot = c.A2;
        _cursor = m.ReadU32(Scratch + 0x10);
        var light = Control(8);
        var inputs = new MapLight.Inputs(Control(16), (int)Gte.ReadControl(13), (int)Gte.ReadControl(14),
            (int)Gte.ReadControl(15), (int)Gte.ReadControl(21), (int)Gte.ReadControl(22), (int)Gte.ReadControl(23));
        uint corners = m.ReadU32(Scratch + 0x40), normals = m.ReadU32(Scratch + 0x24);
        uint rgb = m.ReadU32(Scratch + 0x60) & 0xFFFFFF;
        // The flicker's calls rewrite a fogged face's colour after NCDS; not modelled.
        _colours = shape.Fog == ModelFaces.Fogging.None || (m.ReadU8(0x8017BE8C) & 6) == 0;
        int far = (short)m.ReadU16(Scratch + 4), near = (int)m.ReadU32(Scratch + 0x80);
        for (uint i = 0, q = c.A1; i < (c.A0 & 0xFFFF); i++, q += shape.RecordSize)
            Face(m, q, corners, normals, light, inputs, rgb, c.A3, near, far);
        foreach (var e in _expected) _headBefore.TryAdd(e.Key, m.ReadU32(_ot + (uint)e.Key * 4) & 0xFFFFFF);
    }

    static void Face(IMemory m, uint q, uint corners, uint normals, in MapLight.Matrix light,
                     in MapLight.Inputs inputs, uint rgb, uint mode, int near, int far)
    {
        int n = _shape.Corners;
        Span<uint> sxy = stackalloc uint[n];
        int sum = 0;
        Span<short> fog = stackalloc short[n];
        for (int k = 0; k < n; k++)
        {
            uint at = corners + m.ReadU16(q + _shape.CornersAt[k]);
            sxy[k] = m.ReadU32(at);
            sum += (short)m.ReadU16(at + 4);
            fog[k] = (short)m.ReadU16(at + 6);
        }
        if (Nclip(sxy[0], sxy[1], sxy[2]) <= 0) { MapReference.Count($"{_shape.Name}: turned away"); return; }
        // The mean depth, >> 5: a triangle's through a multiply by 0x55555556, a quad's by >> 7.
        int mean = n == 3 ? (sum >> 5) / 3 : sum >> 7;
        int key = (short)(mean + (sbyte)m.ReadU8(q + 0xA));
        if (key >= far || key <= near) { MapReference.Count($"{_shape.Name}: dropped by depth"); return; }

        uint code = (m.ReadU8(q + 0xB) | _shape.CodeBits | (mode & 2)) << 24;
        var w = new uint[_shape.PacketSize / 4];
        foreach (var (word, off, half) in _shape.Texture) w[word] = half ? m.ReadU16(q + off) : m.ReadU32(q + off);
        w[_shape.TpageWord] |= mode & 0x600000;
        for (int k = 0; k < n; k++) w[_shape.Positions[k]] = sxy[k];
        for (int k = 0; k < _shape.Colours.Length; k++)
        {
            uint at = normals + m.ReadU16(q + _shape.NormalsAt[k]);
            short nx = (short)m.ReadU16(at), ny = (short)m.ReadU16(at + 2), nz = (short)m.ReadU16(at + 4);
            w[_shape.Colours[k]] = _shape.Fog == ModelFaces.Fogging.None
                ? MapLight.Lit(light, inputs, nx, ny, nz, rgb | code)
                : MapLight.LitFogged(light, inputs, nx, ny, nz, rgb | code, _shape.Ir0(fog, k));
        }
        _expected.Add(new Expected(key, w));
    }

    static long Nclip(uint a, uint b, uint c)
    {
        long ax = (short)a, ay = (short)(a >> 16), bx = (short)b, by = (short)(b >> 16), cx = (short)c, cy = (short)(c >> 16);
        return (int)(ax * (by - cy) + bx * (cy - ay) + cx * (ay - by));
    }

    public static void Post(CpuContext c, IMemory m)
    {
        var last = new Dictionary<int, uint>();
        uint p = _cursor;
        foreach (var e in _expected)
        {
            uint link = last.TryGetValue(e.Key, out var prev) ? prev & 0xFFFFFF : _headBefore[e.Key];
            MapReference.Agree($"{_shape.Name}: tag", m.ReadU32(p) == ((_shape.Length << 24) | link));
            MapReference.Agree($"{_shape.Name}: faces, every other word", Words(m, p, e.Words));
            last[e.Key] = p;
            p += _shape.PacketSize;
        }
        foreach (var (key, at) in last)
            MapReference.Agree($"{_shape.Name}: the key's head after", (m.ReadU32(_ot + (uint)key * 4) & 0xFFFFFF) == (at & 0xFFFFFF));
        MapReference.Agree($"{_shape.Name}: the cursor after", m.ReadU32(Scratch + 0x10) == p);
    }

    static bool Words(IMemory m, uint p, uint[] words)
    {
        for (int i = 1; i < words.Length; i++)
        {
            if (!_colours && _shape.Colours.Contains(i)) continue;
            uint mask = _shape.Texture.Any(t => t.Word == i && t.Half) ? 0xFFFFu : 0xFFFFFFFFu;
            uint game = m.ReadU32(p + (uint)i * 4);
            if ((game & mask) == words[i]) continue;
            MapReference.Sample($"{_shape.Name} word {i} at {p:X8}: game {game:X8}, reference {words[i]:X8}");
            return false;
        }
        return true;
    }
}
