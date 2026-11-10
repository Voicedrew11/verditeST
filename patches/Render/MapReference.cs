using System.Reflection;
using RecompOne.Runtime;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// ST_MAP_REFERENCE=1: Phase 11 Step 2's reference for the map's faces, each shape in
/// <see cref="MapFaces"/>. Before the shape's routine runs, every visible cell's faces
/// of it are worked out in plain C# from the records alone (<see cref="MapLight"/> for
/// the colours): facing, the near path, the drop, the order key, each corner's fog and
/// colour and the whole packet. After it, the packets it wrote are compared with that,
/// packet by packet, and so is each key's head in the ordering table. A diagnostic: counts and
/// the first differences, every five seconds. See "The map reference" in
/// docs/GPU_RENDERER.md.
/// </summary>
public static class MapReference
{
    const uint Scratch = 0x1F800000;
    const uint VisibleCount = 0x801B9E14, VisibleList = 0x801B9F16, Cells = 0x8017BED0;
    const uint SceneFlags = 0x8017BE8C;

    /// <summary>One face as the reference has it: a bulk face's key and every word of
    /// its packet but the tag's link, or a near face (<c>Words</c> null), whose packets the
    /// split writes and the reference steps over.</summary>
    readonly record struct Expected(int Key, uint[]? Words);

    static readonly ModInfo _self = new() { Id = "st.mapreference", Name = "Map reference", Version = "1.0" };
    static readonly List<Expected> _expected = [];
    static readonly Dictionary<int, uint> _headBefore = [];
    static readonly SortedDictionary<string, long> _counts = [];
    static readonly List<string> _samples = [];
    static readonly List<(uint Start, uint End)> _nearSpans = [];
    static MapFaces _shape = MapFaces.Quads;
    static uint _cursor, _ot;
    static bool _colours, _inside;
    static long _windowStart = -1;

    public static void Install(string? spec)
    {
        if (spec?.Trim() != "1") return;
        HookAttach.OnOverlayLoad("map reference", Attach);
    }

    static bool Attach()
    {
        SymbolRegistry.Build();
        MethodInfo Hook(string n) => typeof(MapReference).GetMethod(n, BindingFlags.Public | BindingFlags.Static)!;
        var targets = new List<MethodInfo>();
        for (int i = 0; i < MapFaces.All.Length; i++)
        {
            var shape = MapFaces.All[i];
            if (SymbolRegistry.Resolve(MainLoop.Overlay, null, shape.Routine) is not { } t) return false;
            HookManager.AddPre(_self, t, Hook("Pre" + i));
            HookManager.AddPost(_self, t, Hook(nameof(Post)));
            targets.Add(t);
            if (shape.NearSplit is not { } at) continue;
            if (SymbolRegistry.Resolve(MainLoop.Overlay, null, at) is not { } split) return false;
            HookManager.AddPre(_self, split, Hook(nameof(SplitPre)));
            HookManager.AddPost(_self, split, Hook(nameof(SplitPost)));
            targets.Add(split);
        }
        HookManager.Commit();
        bool on = targets.All(HookAttach.Installed);
        Console.WriteLine($"[ST] map reference: {(on ? "on" : "not attached")}, {MapFaces.All.Length} face shapes");
        return on;
    }

    // One per entry of MapFaces.All, by its index: a hook knows its target only so.
    public static void Pre0(CpuContext c, IMemory m) => Pre(MapFaces.All[0], m);
    public static void Pre1(CpuContext c, IMemory m) => Pre(MapFaces.All[1], m);
    public static void Pre2(CpuContext c, IMemory m) => Pre(MapFaces.All[2], m);
    public static void Pre3(CpuContext c, IMemory m) => Pre(MapFaces.All[3], m);

    static void Pre(MapFaces shape, IMemory m)
    {
        _shape = shape;
        _expected.Clear();
        _headBefore.Clear();
        _ot = m.ReadU32(Scratch);
        _cursor = m.ReadU32(Scratch + 0x10);
        // The flicker's calls rewrite each corner's colour after DPCS; not modelled.
        _colours = (m.ReadU8(SceneFlags) & 6) == 0;
        _nearSpans.Clear();
        _inside = true;
        Walk(m);
        foreach (var e in _expected) _headBefore.TryAdd(e.Key, m.ReadU32(_ot + (uint)e.Key * 4) & 0xFFFFFF);
    }

    // The near split's packets: from the cursor at its call to the cursor it returns.
    static uint _splitStart;
    public static void SplitPre(CpuContext c, IMemory m)
    {
        _splitStart = m.ReadU32(Scratch + 0x10);
        if (_inside) NearSplits.Before(m, c.A1);
    }

    public static void SplitPost(CpuContext c, IMemory m)
    {
        if (!_inside) return;
        _nearSpans.Add((_splitStart, c.V0));
        NearSplits.After(m, _shape.Name, _splitStart, c.V0);
    }

    static void Walk(IMemory m)
    {
        uint bank = m.ReadU32(Scratch + 0xC), record = m.ReadU32(Scratch + 0x3C4);
        uint corners = m.ReadU32(Scratch + 0x34);
        int nearLimit = (int)m.ReadU32(Scratch + 0x8C);
        uint rgb = m.ReadU32(Scratch + 0x44);
        var light = Inputs(m, record);
        int fogNear = (short)m.ReadU16(record + 0x68), fogFar = (short)m.ReadU16(record + 0x6A);
        int cells = m.ReadU16(VisibleCount);
        for (int i = cells - 1; i >= 0; i--)
        {
            uint cell = Cells + m.ReadU16(VisibleList + (uint)i * 2) * 12u;
            uint mesh = bank + 0xC + m.ReadU8(cell) * 0x28u;
            if (_shape.DrawsCell(m.ReadU8(cell + 4) & 3))
            {
                int turn = m.ReadU8(cell + 3) & 3;
                uint l = record + (uint)turn * 0x14;
                var lightMatrix = MapLight.Matrix.FromWords(m.ReadU32(l), m.ReadU32(l + 4), m.ReadU32(l + 8),
                                                            m.ReadU32(l + 12), m.ReadU32(l + 16));
                uint normals = bank + 0xC + m.ReadU32(mesh + 8);
                uint faces = bank + 0xC + _shape.First(m, mesh);
                int count = m.ReadU16(mesh + _shape.CountAt);
                for (int q = 0; q < count; q++)
                    Face(m, faces + (uint)q * _shape.RecordSize, corners, normals, lightMatrix, light, rgb, nearLimit, fogNear, fogFar,
                         (int)m.ReadU32(mesh + 4));
            }
            corners += m.ReadU32(mesh + 4) * 8u;
        }
    }

    static MapLight.Inputs Inputs(IMemory m, uint record)
    {
        uint w = record + 0x50;
        var colour = MapLight.Matrix.FromWords(m.ReadU32(w), m.ReadU32(w + 4), m.ReadU32(w + 8),
                                               m.ReadU32(w + 12), m.ReadU32(w + 16));
        // The record's back colour is a byte a channel; the scene loads it << 4.
        int br = m.ReadU8(record + 0x64) << 4, bg = m.ReadU8(record + 0x65) << 4, bb = m.ReadU8(record + 0x66) << 4;
        Agree("back colour from the record", (int)Gte.ReadControl(13) == br && (int)Gte.ReadControl(14) == bg
                                             && (int)Gte.ReadControl(15) == bb);
        // The record's fifth word holds the matrix's last element in its low half only.
        bool lcm = Gte.ReadControl(16) == m.ReadU32(w) && Gte.ReadControl(20) == (m.ReadU32(w + 16) & 0xFFFF);
        Agree("colour matrix from the record", lcm);
        if (!lcm)
            Sample($"LCM {string.Join(" ", Enumerable.Range(16, 5).Select(r => Gte.ReadControl(r).ToString("X8")))}, " +
                   $"record {string.Join(" ", Enumerable.Range(0, 5).Select(k => m.ReadU32(w + (uint)k * 4).ToString("X8")))}");
        return new MapLight.Inputs(colour, br, bg, bb,
            (int)Gte.ReadControl(21), (int)Gte.ReadControl(22), (int)Gte.ReadControl(23));
    }

    static void Face(IMemory m, uint q, uint corners, uint normals, in MapLight.Matrix lightMatrix,
                     in MapLight.Inputs light, uint rgb, int nearLimit, int fogNear, int fogFar, int vertices)
    {
        int singles = vertices / 3 * 3;
        int n = _shape.Corners;
        Span<uint> at = stackalloc uint[n];
        Span<uint> sxy = stackalloc uint[n];
        Span<short> sz = stackalloc short[n];
        Span<short> fog = stackalloc short[n];
        for (int k = 0; k < n; k++)
        {
            at[k] = corners + m.ReadU16(q + _shape.CornersAt + (uint)k * 2);
            sxy[k] = m.ReadU32(at[k]);
            sz[k] = (short)m.ReadU16(at[k] + 4);
            fog[k] = (short)m.ReadU16(at[k] + 6);
        }
        // A quad is also kept when its other half (3-1-2) faces; a triangle has none.
        long n1 = Nclip(sxy[0], sxy[1], sxy[2]), n2 = n == 4 ? Nclip(sxy[3], sxy[1], sxy[2]) : 0;
        if (!(n1 > 0 || n2 < 0)) { Count("faces turned away"); return; }
        int key = (Max(sz) >> 5) + (sbyte)m.ReadU8(q + 0xA);
        if (key >= 4096 || key < nearLimit) { Count("faces dropped by depth"); return; }
        if (_shape.NearSplit != null && (n1 > 16384 || n2 < -16384))
        {
            // The near path splits the face through libgte; its packets are not modelled
            // here, only stepped over.
            _expected.Add(new Expected(key, null));
            return;
        }
        for (int k = 0; k < n; k++)
        {
            int want = MapLight.FogFactor(sz[k], fogNear, fogFar, m.ReadU16(q + _shape.CornersAt + (uint)k * 2) / 8 >= singles);
            Agree("fog factor from depth", fog[k] == want);
            if (fog[k] != want) Sample($"fog: sz {sz[k]} near {fogNear} far {fogFar}: game {fog[k]}, reference {want}");
        }

        uint code = (m.ReadU8(q + 0xB) | _shape.CodeBits) << 24;
        uint normal = normals + m.ReadU16(q + _shape.NormalAt);
        uint lit = MapLight.Lit(lightMatrix, light, (short)m.ReadU16(normal), (short)m.ReadU16(normal + 2),
                                (short)m.ReadU16(normal + 4), (rgb & 0xFFFFFF) | code);
        var w = new uint[_shape.PacketSize / 4];
        foreach (var (word, off, half) in _shape.Texture) w[word] = half ? m.ReadU16(q + off) : m.ReadU32(q + off);
        for (int k = 0; k < n; k++) w[_shape.Positions[k]] = sxy[k];
        if (_shape.Colouring == MapFaces.Shading.Flat) w[_shape.Colours[0]] = lit;
        else for (int k = 0; k < n; k++) w[_shape.Colours[k]] = MapLight.Fogged(lit, light, fog[k]);
        _expected.Add(new Expected(key, w));
    }

    static long Nclip(uint a, uint b, uint c)
    {
        long ax = (short)a, ay = (short)(a >> 16), bx = (short)b, by = (short)(b >> 16), cx = (short)c, cy = (short)(c >> 16);
        return (int)(ax * (by - cy) + bx * (cy - ay) + cx * (ay - by));
    }

    static int Max(ReadOnlySpan<short> v)
    {
        int max = v[0];
        foreach (short x in v) max = Math.Max(max, x);
        return max;
    }

    // Each key's head as the walk leaves it: one packet, or anywhere in a near split's
    // packets, whose order inside the split is not modelled.
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
                Count($"{_shape.Name}: near faces stepped over");
                if (near >= _nearSpans.Count) { Agree("near faces against the split's calls", false); break; }
                var (start, end) = _nearSpans[near++];
                Agree("a near split starting at the cursor", start == p);
                // A split that clips the whole face away writes nothing and leaves the head.
                if (end != start) heads[e.Key] = new Head(start, end);
                p = end;
                continue;
            }
            var head = heads.TryGetValue(e.Key, out var h) ? h : new Head(_headBefore[e.Key], _headBefore[e.Key] + 1);
            uint tag = m.ReadU32(p);
            bool linked = tag >> 24 == _shape.Length && head.Holds(tag & 0xFFFFFF);
            Agree("tag: length and the key's previous head", linked);
            if (!linked)
                Sample($"tag {tag:X8} at {p:X8}, key {e.Key}: head {head.Lo:X8}..{head.Hi:X8}; in a near span: " +
                       _nearSpans.Any(n => (tag & 0xFFFFFF) >= (n.Start & 0xFFFFFF) && (tag & 0xFFFFFF) < (n.End & 0xFFFFFF)));
            Agree($"{_shape.Name}: bulk faces, every other word", Words(m, p, e.Words));
            if (!_colours) Count($"{_shape.Name}: colours not checked (the flicker)");
            heads[e.Key] = new Head(p, p + 1);
            p += _shape.PacketSize;
        }
        Agree("near faces against the split's calls", near == _nearSpans.Count);
        foreach (var (key, head) in heads)
            Agree("the key's head after", head.Holds(m.ReadU32(_ot + (uint)key * 4) & 0xFFFFFF));
        Agree($"{_shape.Name}: the cursor after", m.ReadU32(Scratch + 0x10) == p);
        Report();
    }

    static bool Words(IMemory m, uint p, uint[] words)
    {
        for (int i = 1; i < words.Length; i++)
        {
            bool colour = _shape.Colours.Contains(i);
            if (colour && !_colours) continue;
            // A halfword UV shares its word with something that is not the face's.
            uint mask = _shape.Texture.Any(t => t.Word == i && t.Half) ? 0xFFFFu : 0xFFFFFFFFu;
            uint game = m.ReadU32(p + (uint)i * 4);
            if ((game & mask) == words[i]) continue;
            Sample($"{(colour ? "colour" : "word")} {i} of the {_shape.Name} packet at {p:X8}: game {game:X8}, reference {words[i]:X8}");
            return false;
        }
        return true;
    }

    internal static void Agree(string what, bool ok) => Count(ok ? what + ": agree" : what + ": DIFFER");

    internal static void Count(string what) => _counts[what] = _counts.GetValueOrDefault(what) + 1;

    static void Sample(string s)
    {
        if (_samples.Count < 12) _samples.Add(s);
    }

    static void Report()
    {
        long now = Environment.TickCount64;
        if (_windowStart < 0) _windowStart = now;
        if (now - _windowStart < 5000) return;
        _windowStart = now;
        Console.WriteLine($"[ST] map reference (5 s) in {AgentBeacon.Map}: " +
                          string.Join(", ", _counts.Select(kv => $"{kv.Key} {kv.Value}")));
        foreach (var s in _samples) Console.WriteLine($"[ST]   {s}");
        _counts.Clear();
        _samples.Clear();
        Console.Out.Flush();
    }
}
