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

    /// <summary>A bulk face's key and packet words, or a near face (<c>Words</c> null)
    /// whose packets the division writes and the reference steps over.</summary>
    readonly record struct Expected(int Key, uint[]? Words);

    static readonly ModInfo _self = new() { Id = "st.modelreference", Name = "Model reference", Version = "1.0" };
    static readonly List<Expected> _expected = [];
    static readonly Dictionary<int, uint> _headBefore = [];
    static ModelFaces _shape = ModelFaces.FlatTriangles;
    static uint _cursor, _ot;
    static int _flicker;
    static bool _fogged, _inside;
    static uint _splitStart;
    static readonly List<(uint Start, uint End)> _nearSpans = [];

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
            if (ModelFaces.All[i].NearSplit is not { } at) continue;
            if (SymbolRegistry.Resolve(MainLoop.Overlay, null, at) is not { } split) return false;
            HookManager.AddPre(_self, split, Hook(nameof(SplitPre)));
            HookManager.AddPost(_self, split, Hook(nameof(SplitPost)));
            targets.Add(split);
        }
        if (SymbolRegistry.Resolve(MainLoop.Overlay, null, 0x80017D28) is { } blend)
            HookManager.AddPre(_self, blend, Hook(nameof(BlendPre)));
        if (SymbolRegistry.Resolve(MainLoop.Overlay, null, 0x80022678) is { } draw)
            HookManager.AddPre(_self, draw, Hook(nameof(DrawPre)));
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
    public static void Pre8(CpuContext c, IMemory m) => Pre(ModelFaces.All[8], c, m);
    public static void Pre9(CpuContext c, IMemory m) => Pre(ModelFaces.All[9], c, m);
    public static void Pre10(CpuContext c, IMemory m) => Pre(ModelFaces.All[10], c, m);
    public static void Pre11(CpuContext c, IMemory m) => Pre(ModelFaces.All[11], c, m);

    // ModelStep17D28's mode (a1 & 0x38) and whether its weight (a0) is the whole 4096.
    public static void BlendPre(CpuContext c, IMemory m)
    {
        uint w = (uint)(short)c.A0;
        string which = (c.A1 & 0x20) != 0 ? "0x20" : (c.A1 & 0x10) != 0 ? "0x10" : (c.A1 & 8) != 0 ? "0x08" : "none";
        MapReference.Count($"light blend {which}: {(w == 0x1000 ? "weight 4096" : w == 0 ? "weight 0" : "weight between")}");
    }

    // ModelDraw's caller (by return address) and the weight and flags it passes on the
    // stack (+0x24, +0x2C), which reach ModelStep17D28 as a0 and a1 & 0xFF.
    public static void DrawPre(CpuContext c, IMemory m)
    {
        short w = (short)m.ReadU32(c.SP + 0x24);
        uint flags = m.ReadU32(c.SP + 0x2C) & 0xFF;
        string weight = w == 0x1000 ? "4096" : w == 0 ? "0" : w < 0 ? "negative" : "between";
        MapReference.Count($"model draw from {c.RA:X8}: flags 0x{flags:X2}, weight {weight}");
    }

    // A near face's packets: from the cursor at the division's call to the cursor it returns.
    public static void SplitPre(CpuContext c, IMemory m) => _splitStart = c.A0;

    public static void SplitPost(CpuContext c, IMemory m)
    {
        if (_inside) _nearSpans.Add((_splitStart, c.V0));
    }

    static MapLight.Matrix Control(int first) => MapLight.Matrix.FromWords(Gte.ReadControl(first),
        Gte.ReadControl(first + 1), Gte.ReadControl(first + 2), Gte.ReadControl(first + 3), Gte.ReadControl(first + 4));

    static void Pre(ModelFaces shape, CpuContext c, IMemory m)
    {
        _shape = shape;
        _expected.Clear();
        _headBefore.Clear();
        _nearSpans.Clear();
        _inside = true;
        _ot = c.A2;
        _cursor = m.ReadU32(Scratch + 0x10);
        var light = Control(8);
        var inputs = new MapLight.Inputs(Control(16), (int)Gte.ReadControl(13), (int)Gte.ReadControl(14),
            (int)Gte.ReadControl(15), (int)Gte.ReadControl(21), (int)Gte.ReadControl(22), (int)Gte.ReadControl(23));
        uint corners = m.ReadU32(Scratch + 0x40), normals = m.ReadU32(Scratch + 0x24);
        uint rgb = m.ReadU32(Scratch + 0x60) & 0xFFFFFF;
        int flags = m.ReadU8(0x8017BE8C);
        _fogged = shape.Fog != ModelFaces.Fogging.None && !(shape.FogsBySceneFlag && (flags & 0x10) != 0);
        // The flicker's calls rewrite a fogged face's colours after NCDS.
        _flicker = _fogged ? flags & 6 : 0;
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
        long n1 = Nclip(sxy[0], sxy[1], sxy[2]);
        long n2 = _shape.Faces == ModelFaces.Facing.BothHalves ? Nclip(sxy[3], sxy[1], sxy[2]) : 0;
        if (!(n1 > 0 || n2 < 0)) { MapReference.Count($"{_shape.Name}: turned away"); return; }
        // The mean depth, >> 5: a triangle's through a multiply by 0x55555556, a quad's by >> 7.
        int mean = n == 3 ? (sum >> 5) / 3 : sum >> 7;
        int key = (short)(mean + (sbyte)m.ReadU8(q + 0xA));
        if (key >= far || key <= near) { MapReference.Count($"{_shape.Name}: dropped by depth"); return; }
        if (_shape.NearSplit != null && (n1 > 16384 || n2 < -16384))
        {
            _expected.Add(new Expected(key, null));
            return;
        }

        uint code = (m.ReadU8(q + 0xB) | _shape.CodeBits | (mode & 2)) << 24;
        var w = new uint[_shape.PacketSize / 4];
        foreach (var (word, off, half) in _shape.Texture) w[word] = half ? m.ReadU16(q + off) : m.ReadU32(q + off);
        w[_shape.TpageWord] |= mode & 0x600000;
        for (int k = 0; k < n; k++) w[_shape.Positions[k]] = sxy[k];
        for (int k = 0; k < _shape.Colours.Length; k++)
        {
            uint at = normals + m.ReadU16(q + _shape.NormalsAt[k]);
            short nx = (short)m.ReadU16(at), ny = (short)m.ReadU16(at + 2), nz = (short)m.ReadU16(at + 4);
            w[_shape.Colours[k]] = !_fogged
                ? MapLight.Lit(light, inputs, nx, ny, nz, rgb | code)
                : MapLight.Flicker(MapLight.LitFogged(light, inputs, nx, ny, nz, rgb | code, _shape.Ir0(fog, k)), _flicker);
            if (_flicker != 0) MapReference.Count($"{_shape.Name}: colours under the flicker");
        }
        _expected.Add(new Expected(key, w));
    }

    static long Nclip(uint a, uint b, uint c)
    {
        long ax = (short)a, ay = (short)(a >> 16), bx = (short)b, by = (short)(b >> 16), cx = (short)c, cy = (short)(c >> 16);
        return (int)(ax * (by - cy) + bx * (cy - ay) + cx * (ay - by));
    }

    // A key's head as the walk leaves it: one packet, or anywhere in a split's packets.
    readonly record struct Head(uint Lo, uint Hi)
    {
        public bool Holds(uint link) => link >= (Lo & 0xFFFFFF) && link < (Hi & 0xFFFFFF);
    }

    public static void Post(CpuContext c, IMemory m)
    {
        _inside = false;
        var heads = new Dictionary<int, Head>();
        uint p = _cursor;
        int near = 0;
        foreach (var e in _expected)
        {
            if (e.Words == null)
            {
                MapReference.Count($"{_shape.Name}: near faces stepped over");
                if (near >= _nearSpans.Count) { MapReference.Agree($"{_shape.Name}: near faces against the calls", false); break; }
                var (start, end) = _nearSpans[near++];
                MapReference.Agree($"{_shape.Name}: a split at the cursor", start == p);
                if (end != start) heads[e.Key] = new Head(start, end);
                p = end;
                continue;
            }
            var head = heads.TryGetValue(e.Key, out var h) ? h : new Head(_headBefore[e.Key], _headBefore[e.Key] + 1);
            uint tag = m.ReadU32(p);
            MapReference.Agree($"{_shape.Name}: tag", tag >> 24 == _shape.Length && head.Holds(tag & 0xFFFFFF));
            MapReference.Agree($"{_shape.Name}: faces, every other word", Words(m, p, e.Words));
            heads[e.Key] = new Head(p, p + 1);
            p += _shape.PacketSize;
        }
        MapReference.Agree($"{_shape.Name}: near faces against the calls", near == _nearSpans.Count);
        foreach (var (key, head) in heads)
            MapReference.Agree($"{_shape.Name}: the key's head after", head.Holds(m.ReadU32(_ot + (uint)key * 4) & 0xFFFFFF));
        MapReference.Agree($"{_shape.Name}: the cursor after", m.ReadU32(Scratch + 0x10) == p);
    }

    static bool Words(IMemory m, uint p, uint[] words)
    {
        for (int i = 1; i < words.Length; i++)
        {
            uint mask = _shape.Texture.Any(t => t.Word == i && t.Half) ? 0xFFFFu : 0xFFFFFFFFu;
            uint game = m.ReadU32(p + (uint)i * 4);
            if ((game & mask) == words[i]) continue;
            MapReference.Sample($"{_shape.Name} word {i} at {p:X8}: game {game:X8}, reference {words[i]:X8}");
            return false;
        }
        return true;
    }
}
