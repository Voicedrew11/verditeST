using System.Reflection;
using RecompOne.Runtime;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// ST_MAP_REFERENCE=1: Phase 11 Step 2's reference for the map's quads. Before
/// <c>MapQuads</c> (<c>func_8001DB64</c>) runs, every visible cell's quads are worked
/// out in plain C# from the records alone (<see cref="MapLight"/> for the colours):
/// facing, the near path, the drop, the order key, the four corners' fog and colours
/// and the whole GT4. After it, the packets it wrote are compared with that, packet by
/// packet, and so is each key's head in the ordering table. A diagnostic: counts and
/// the first differences, every five seconds. See "The map reference" in
/// docs/GPU_RENDERER.md.
/// </summary>
public static class MapReference
{
    const uint Routine = 0x8001DB64, Scratch = 0x1F800000;
    const uint VisibleCount = 0x801B9E14, VisibleList = 0x801B9F16, Cells = 0x8017BED0;
    const uint SceneFlags = 0x8017BE8C;
    const uint NearSplit = 0x80076884;
    const int GT4 = 0x34;

    /// <summary>One quad as the reference has it: a bulk quad's key and every word of
    /// its GT4 but the tag's link, or a near face (<c>Words</c> null), whose packets the
    /// split writes and the reference steps over.</summary>
    readonly record struct Expected(int Key, uint[]? Words);

    static readonly ModInfo _self = new() { Id = "st.mapreference", Name = "Map reference", Version = "1.0" };
    static readonly List<Expected> _expected = [];
    static readonly Dictionary<int, uint> _headBefore = [];
    static readonly SortedDictionary<string, long> _counts = [];
    static readonly List<string> _samples = [];
    static readonly List<(uint Start, uint End)> _nearSpans = [];
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
        if (SymbolRegistry.Resolve(MainLoop.Overlay, null, Routine) is not { } t) return false;
        MethodInfo Hook(string n) => typeof(MapReference).GetMethod(n, BindingFlags.Public | BindingFlags.Static)!;
        if (SymbolRegistry.Resolve(MainLoop.Overlay, null, NearSplit) is not { } split) return false;
        HookManager.AddPre(_self, t, Hook(nameof(Pre)));
        HookManager.AddPost(_self, t, Hook(nameof(Post)));
        HookManager.AddPre(_self, split, Hook(nameof(SplitPre)));
        HookManager.AddPost(_self, split, Hook(nameof(SplitPost)));
        HookManager.Commit();
        bool on = HookAttach.Installed(t) && HookAttach.Installed(split);
        Console.WriteLine($"[ST] map reference: {(on ? "on" : "not attached")}");
        return on;
    }

    public static void Pre(CpuContext c, IMemory m)
    {
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
    public static void SplitPre(CpuContext c, IMemory m) => _splitStart = m.ReadU32(Scratch + 0x10);
    public static void SplitPost(CpuContext c, IMemory m)
    {
        if (_inside) _nearSpans.Add((_splitStart, c.V0));
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
            if ((m.ReadU8(cell + 4) & 3) != 0)
            {
                int turn = m.ReadU8(cell + 3) & 3;
                uint l = record + (uint)turn * 0x14;
                var lightMatrix = MapLight.Matrix.FromWords(m.ReadU32(l), m.ReadU32(l + 4), m.ReadU32(l + 8),
                                                            m.ReadU32(l + 12), m.ReadU32(l + 16));
                uint normals = bank + 0xC + m.ReadU32(mesh + 8);
                uint quads = bank + 0xC + m.ReadU32(mesh + 0x10) + m.ReadU16(mesh + 0x18) * 0x14u;
                int count = m.ReadU16(mesh + 0x1C);
                for (int q = 0; q < count; q++)
                    Quad(m, quads + (uint)q * 0x1C, corners, normals, lightMatrix, light, rgb, nearLimit, fogNear, fogFar,
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

    static void Quad(IMemory m, uint q, uint corners, uint normals, in MapLight.Matrix lightMatrix,
                     in MapLight.Inputs light, uint rgb, int nearLimit, int fogNear, int fogFar, int vertices)
    {
        int singles = vertices / 3 * 3;
        Span<uint> at = stackalloc uint[4];
        Span<uint> sxy = stackalloc uint[4];
        Span<short> sz = stackalloc short[4];
        Span<short> fog = stackalloc short[4];
        for (int k = 0; k < 4; k++)
        {
            at[k] = corners + m.ReadU16(q + 0x12 + (uint)k * 2);
            sxy[k] = m.ReadU32(at[k]);
            sz[k] = (short)m.ReadU16(at[k] + 4);
            fog[k] = (short)m.ReadU16(at[k] + 6);
        }
        long n1 = Nclip(sxy[0], sxy[1], sxy[2]), n2 = Nclip(sxy[3], sxy[1], sxy[2]);
        if (!(n1 > 0 || n2 < 0)) { Count("faces turned away"); return; }
        int key = (Max(sz) >> 5) + (sbyte)m.ReadU8(q + 0xA);
        if (key >= 4096 || key < nearLimit) { Count("faces dropped by depth"); return; }
        if (n1 > 16384 || n2 < -16384)
        {
            // The near path splits the face through libgte; its packets are not modelled
            // here, only stepped over.
            _expected.Add(new Expected(key, null));
            return;
        }
        for (int k = 0; k < 4; k++)
        {
            int want = MapLight.FogFactor(sz[k], fogNear, fogFar, m.ReadU16(q + 0x12 + (uint)k * 2) / 8 >= singles);
            Agree("fog factor from depth", fog[k] == want);
            if (fog[k] != want) Sample($"fog: sz {sz[k]} near {fogNear} far {fogFar}: game {fog[k]}, reference {want}");
        }

        uint code = (uint)(m.ReadU8(q + 0xB) | 0x3C) << 24;
        uint n = normals + m.ReadU16(q + 0x10);
        uint lit = MapLight.Lit(lightMatrix, light, (short)m.ReadU16(n), (short)m.ReadU16(n + 2),
                                (short)m.ReadU16(n + 4), (rgb & 0xFFFFFF) | code);
        var w = new uint[GT4 / 4];
        w[3] = m.ReadU32(q);
        w[6] = m.ReadU32(q + 4);
        w[9] = m.ReadU16(q + 8);
        w[12] = m.ReadU16(q + 0xC);
        for (int k = 0; k < 4; k++)
        {
            w[1 + k * 3] = MapLight.Fogged(lit, light, fog[k]);
            w[2 + k * 3] = sxy[k];
        }
        _expected.Add(new Expected(key, w));
    }

    static long Nclip(uint a, uint b, uint c)
    {
        long ax = (short)a, ay = (short)(a >> 16), bx = (short)b, by = (short)(b >> 16), cx = (short)c, cy = (short)(c >> 16);
        return (int)(ax * (by - cy) + bx * (cy - ay) + cx * (ay - by));
    }

    static int Max(ReadOnlySpan<short> v) => Math.Max(Math.Max(v[0], v[1]), Math.Max(v[2], v[3]));

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
                Count("near faces stepped over");
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
            bool linked = tag >> 24 == 0x0C && head.Holds(tag & 0xFFFFFF);
            Agree("tag: length and the key's previous head", linked);
            if (!linked)
                Sample($"tag {tag:X8} at {p:X8}, key {e.Key}: head {head.Lo:X8}..{head.Hi:X8}; in a near span: " +
                       _nearSpans.Any(n => (tag & 0xFFFFFF) >= (n.Start & 0xFFFFFF) && (tag & 0xFFFFFF) < (n.End & 0xFFFFFF)));
            Agree("bulk quads, every other word", Words(m, p, e.Words));
            if (!_colours) Count("bulk quads with colours not checked (the flicker)");
            heads[e.Key] = new Head(p, p + 1);
            p += GT4;
        }
        Agree("near faces against the split's calls", near == _nearSpans.Count);
        foreach (var (key, head) in heads)
            Agree("the key's head after", head.Holds(m.ReadU32(_ot + (uint)key * 4) & 0xFFFFFF));
        Agree("the cursor after", m.ReadU32(Scratch + 0x10) == p);
        Report();
    }

    static bool Words(IMemory m, uint p, uint[] words)
    {
        for (int i = 1; i < words.Length; i++)
        {
            bool colour = i % 3 == 1 && i < 12;
            if (colour && !_colours) continue;
            // The last two UVs are halfwords; the packet's other halves are not the quad's.
            uint mask = i is 9 or 12 ? 0xFFFFu : 0xFFFFFFFFu;
            uint game = m.ReadU32(p + (uint)i * 4);
            if ((game & mask) == words[i]) continue;
            Sample($"{(colour ? "colour" : "word")} {i} of the GT4 at {p:X8}: game {game:X8}, reference {words[i]:X8}");
            return false;
        }
        return true;
    }

    static void Agree(string what, bool ok) => Count(ok ? what + ": agree" : what + ": DIFFER");

    static void Count(string what) => _counts[what] = _counts.GetValueOrDefault(what) + 1;

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
