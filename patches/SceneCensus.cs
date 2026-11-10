using System.Reflection;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// ST_SCENE_CENSUS=1: every packet of a main-loop frame, by the domain that wrote it
/// (Phase 11's Step 0, Verdite3's scene census). Each call in
/// <see cref="GeometryProbe.Sites"/> made inside stage 14 records the span of the packet
/// buffer it moved the cursor over, as <see cref="MapCoverage"/> does; after stage 14
/// the ordering table is walked once and each packet goes to the innermost span
/// holding it, then to that call's domain. A packet in no span is counted as
/// unattributed, never dropped. One block every five seconds: packets and words a
/// frame per domain, and their kinds. A diagnostic, not a configuration to play in.
/// See "The scene census" in docs/GPU_RENDERER.md.
/// </summary>
public static class SceneCensus
{
    const int MaxPackets = 1 << 17;
    const string Unattributed = "(unattributed)";

    /// <summary>The domain of each call site, by its label in GeometryProbe.Sites;
    /// a nested call's own entry wins over its caller's.</summary>
    static readonly Dictionary<string, string> Domains = new()
    {
        ["176A8"] = "camera", ["16A14"] = "texture animations", ["33110"] = "player state 0/5/6",
        ["174C4"] = "frame head", ["340F8"] = "slots", ["34FC4"] = "slots",
        ["27D18"] = "HUD", ["26964(1A)"] = "HUD", ["26508"] = "HUD", ["26964(0)"] = "HUD",
        ["26228"] = "HUD", ["26964(20)"] = "HUD", ["26964(25)"] = "HUD",
        ["25E00 (26964)"] = "HUD", ["25C24 (26964)"] = "HUD", ["26020 (26964)"] = "HUD",
        ["2181C"] = "tints", ["218D8"] = "tints", ["21654"] = "load fade",
        ["21AC4(0)"] = "held models", ["21AC4(1)"] = "held models",
        ["scene 20A80"] = "map (other)", ["1C100 cells"] = "map (other)",
        ["1F954"] = "scene's own", ["1FD00"] = "scene's own",
        ["1D2A8"] = "map bulk", ["1DB64"] = "map bulk",
        ["1E5A0"] = "map near", ["1EEAC"] = "map near",
        ["20164"] = "map (other)", ["20588"] = "map (other)",
        ["1C8E0"] = "visibility", ["1C9C8"] = "visibility", ["1CAB8"] = "visibility",
        ["241D0"] = "models", ["251C8"] = "models", ["246E4"] = "effects",
        ["255F0 cels"] = "cels", ["28784"] = "text",
    };

    readonly record struct Span(uint Lo, uint Hi, string Domain);

    sealed class Row { public long Packets, Words; public readonly Dictionary<string, long> Kinds = new(); }

    static readonly ModInfo _self = new() { Id = "st.scenecensus", Name = "Scene census", Version = "1.0" };
    static readonly Dictionary<uint, string> _domainByReturn = new();
    static readonly Stack<(string? Domain, uint Start, bool Scene)> _calls = new();
    static readonly List<Span> _spans = new();
    static readonly SortedDictionary<string, Row> _rows = new();
    static int _depth;
    static long _frames, _windowStart = -1;

    public static void Install(string? spec)
    {
        if (spec?.Trim() != "1") return;
        foreach (var s in GeometryProbe.Sites)
        {
            string label = s.Label.Trim();
            string key = Domains.ContainsKey(label) ? label : label.Split(' ')[0];
            _domainByReturn[s.Return] = Domains.TryGetValue(key, out var d) ? d
                : label.Contains("(21AC4)") ? "held models" : label.Contains("(2") ? "models" : "map (other)";
        }
        HookAttach.OnOverlayLoad("scene census", Attach);
    }

    static bool Attach()
    {
        SymbolRegistry.Build();
        MethodInfo Hook(string name) => typeof(SceneCensus).GetMethod(name, BindingFlags.Public | BindingFlags.Static)!;
        var targets = new List<MethodInfo>();
        foreach (uint a in GeometryProbe.Sites.Select(s => s.Callee).Append(MainLoop.Draw.Routine).Distinct())
        {
            if (SymbolRegistry.Resolve(MainLoop.Overlay, null, a) is not { } t) return false;
            targets.Add(t);
        }
        HookManager.AddPre(_self, targets[^1], Hook(nameof(DrawPre)));
        HookManager.AddPost(_self, targets[^1], Hook(nameof(DrawPost)));
        foreach (var t in targets.SkipLast(1))
        {
            HookManager.AddPre(_self, t, Hook(nameof(Pre)));
            HookManager.AddPost(_self, t, Hook(nameof(Post)));
        }
        HookManager.Commit();
        int n = targets.Count(HookAttach.Installed);
        Console.WriteLine($"[ST] scene census: {n}/{targets.Count} functions hooked");
        return n == targets.Count;
    }

    // The cursor of the frame being built: the record 0x80165394 points at, +8. Inside
    // the scene the map's routines move their own, the scratchpad's +0x10, and the
    // scene writes it back to the record at the end, so a call made inside the scene
    // reads that one.
    const uint SceneReturn = 0x80026D2C, ScratchCursor = 0x1F800010;
    static int _inScene;

    static uint Cursor(IMemory m)
    {
        if (_inScene > 0) return m.ReadU32(ScratchCursor);
        uint rec = m.ReadU32(0x80165394);
        return rec < 0x80000000u ? 0u : m.ReadU32(rec + 8u);
    }

    public static void DrawPre(CpuContext c, IMemory m)
    {
        if (_depth++ == 0) { _spans.Clear(); _inScene = 0; }
    }

    public static void DrawPost(CpuContext c, IMemory m)
    {
        if (_depth > 0) _depth--;
        if (_depth != 0 || c.RA != MainLoop.Draw.ReturnAddress) return;
        _frames++;
        OrderingTable.EachPacket(m, MaxPackets, (p, tag) =>
        {
            int len = (int)(tag >> 24);
            if (len == 0) return;
            var row = RowOf(OwnerOf(0x80000000u | (p & 0x00FFFFFFu)));
            row.Packets++;
            row.Words += len;
            string kind = OrderingTable.Kind(m.ReadU32(p + 4u) >> 24);
            row.Kinds[kind] = row.Kinds.GetValueOrDefault(kind) + 1;
        });
        Report();
    }

    public static void Pre(CpuContext c, IMemory m)
    {
        string? domain = _depth > 0 && _domainByReturn.TryGetValue(c.RA, out var d) ? d : null;
        _calls.Push((domain, Cursor(m), c.RA == SceneReturn));
        if (c.RA == SceneReturn) _inScene++;
    }

    public static void Post(CpuContext c, IMemory m)
    {
        if (_calls.Count == 0) return;
        var (domain, start, scene) = _calls.Pop();
        if (scene && _inScene > 0) _inScene--;
        if (domain == null) return;
        uint end = Cursor(m);
        if (end > start) _spans.Add(new Span(start, end, domain));
    }

    static Row RowOf(string domain)
    {
        if (!_rows.TryGetValue(domain, out var r)) _rows[domain] = r = new Row();
        return r;
    }

    // The innermost (latest-starting) span holding the packet.
    static string OwnerOf(uint pkt)
    {
        string best = Unattributed;
        uint bestLo = 0;
        foreach (var s in _spans)
            if (pkt >= s.Lo && pkt < s.Hi && s.Lo >= bestLo) { best = s.Domain; bestLo = s.Lo; }
        return best;
    }

    static void Report()
    {
        long now = Environment.TickCount64;
        if (_windowStart < 0) { _windowStart = now; return; }
        double s = (now - _windowStart) / 1000.0;
        if (s < 5.0 || _frames == 0) return;
        double f = _frames;
        long all = _rows.Values.Sum(r => r.Packets);
        Console.WriteLine($"[ST] scene census: {_frames / s:0.0} frames/s, {all / f:0.0} packets a frame in {AgentBeacon.Map}");
        foreach (var (domain, r) in _rows.OrderByDescending(kv => kv.Value.Packets))
        {
            string kinds = string.Join(" ", r.Kinds.OrderByDescending(kv => kv.Value).Take(6).Select(kv => $"{kv.Key} {kv.Value / f:0.0}"));
            Console.WriteLine($"[ST] scene census:   {domain}: {r.Packets / f:0.0} packets, {r.Words / f:0} words a frame; {kinds}");
        }
        _rows.Clear();
        _frames = 0;
        _windowStart = now;
        Console.Out.Flush();
    }
}
