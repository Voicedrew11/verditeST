using System.Reflection;
using RecompOne.Runtime;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// ST_MAPCOVERAGE=1: how the GTE vertex address map covers the frame, by the call that
/// wrote each packet. Every call in <see cref="GeometryProbe.Sites"/> made inside stage
/// 14 records the range of the packet buffer it moved the cursor over; after stage 14
/// returns from the main loop, the table is walked and each polygon's vertex words are
/// asked of <see cref="GteVertexMap.Peek"/>, which does not move the map's counters. A
/// packet is owned by the innermost range holding it. One block every five seconds.
/// The map has to be on (perspective, sub-pixel or the Z-buffer). Verdite1's
/// MapCoverage on this game's draw stage. See docs/PICTURE.md.
/// </summary>
public static class MapCoverage
{
    const int MaxPackets = 1 << 17;
    const string Unowned = "(no call)";

    readonly record struct Span(uint Lo, uint Hi, string Label);

    sealed class Row { public long Packets, Vertices, Hits; }

    static readonly ModInfo _self = new() { Id = "st.mapcoverage", Name = "Address map coverage", Version = "1.0" };

    static readonly Dictionary<uint, string> _labelByReturn = new();
    static readonly Stack<(string? Label, uint Start)> _calls = new();
    static readonly List<Span> _spans = new();
    static readonly SortedDictionary<string, Row> _rows = new();
    static int _depth;
    static long _frames, _windowStart = -1;

    static uint Mask => Runtime.RamWordMask;

    public static void Install(string? spec)
    {
        if (spec?.Trim() != "1") return;
        foreach (var s in GeometryProbe.Sites) _labelByReturn[s.Return] = s.Label.Trim();
        HookAttach.OnOverlayLoad("mapcoverage", Attach);
    }

    static bool Attach()
    {
        SymbolRegistry.Build();
        var self = typeof(MapCoverage);
        MethodInfo Hook(string name) => self.GetMethod(name, BindingFlags.Public | BindingFlags.Static)!;
        var targets = new List<MethodInfo>();
        foreach (uint a in GeometryProbe.Sites.Select(s => s.Callee).Append(MainLoop.Draw.Routine).Distinct())
        {
            if (SymbolRegistry.Resolve(MainLoop.Overlay, null, a) is not { } t) return false;
            targets.Add(t);
        }
        var draw = targets[^1];
        HookManager.AddPre(_self, draw, Hook(nameof(DrawPre)));
        HookManager.AddPost(_self, draw, Hook(nameof(DrawPost)));
        foreach (var t in targets.SkipLast(1))
        {
            HookManager.AddPre(_self, t, Hook(nameof(Pre)));
            HookManager.AddPost(_self, t, Hook(nameof(Post)));
        }
        HookManager.Commit();
        int n = targets.Count(HookAttach.Installed);
        Console.WriteLine($"[ST] mapcoverage: {n}/{targets.Count} functions hooked");
        return n == targets.Count;
    }

    // The cursor of the frame being built: the record 0x80165394 points at, +8.
    static uint Cursor(IMemory m)
    {
        uint rec = m.ReadU32(0x80165394);
        return rec < 0x80000000u ? 0u : m.ReadU32(rec + 8u) & Mask;
    }

    public static void DrawPre(CpuContext c, IMemory m)
    {
        if (_depth++ == 0) _spans.Clear();
    }

    public static void DrawPost(CpuContext c, IMemory m)
    {
        if (_depth > 0) _depth--;
        if (_depth != 0 || c.RA != MainLoop.Draw.ReturnAddress) return;
        _frames++;
        OrderingTable.EachPacket(m, MaxPackets, (p, tag) => { if ((tag >> 24) != 0) Count(m, p & Mask); });
        Report();
    }

    public static void Pre(CpuContext c, IMemory m)
    {
        string? label = _depth > 0 && _labelByReturn.TryGetValue(c.RA, out var l) ? l : null;
        _calls.Push((label, Cursor(m)));
    }

    public static void Post(CpuContext c, IMemory m)
    {
        if (_calls.Count == 0) return;
        var (label, start) = _calls.Pop();
        if (label == null) return;
        uint end = Cursor(m);
        if (end > start) _spans.Add(new Span(start, end, label));
    }

    // A polygon's vertex words: the xy word of each corner, after the command word;
    // bit 2 of the command textured (a uv word a corner), bit 4 shaded (a colour word).
    static void Count(IMemory m, uint pkt)
    {
        uint body = pkt + 4u;
        uint cmd = m.ReadU32(body) >> 24;
        if (cmd < 0x20u || cmd > 0x3Fu) return;
        var row = RowOf(OwnerOf(pkt));
        row.Packets++;
        int corners = (cmd & 0x08u) != 0 ? 4 : 3;
        uint stride = 4u + ((cmd & 0x04u) != 0 ? 4u : 0u) + ((cmd & 0x10u) != 0 ? 4u : 0u);
        for (int k = 0; k < corners; k++)
        {
            uint at = (body + 4u + (uint)k * stride) & Mask;
            row.Vertices++;
            if (GteVertexMap.Peek(at, m.ReadU32(at), out _)) row.Hits++;
        }
    }

    static Row RowOf(string label)
    {
        if (!_rows.TryGetValue(label, out var r)) _rows[label] = r = new Row();
        return r;
    }

    // The innermost (latest-starting) range holding the packet.
    static string OwnerOf(uint pkt)
    {
        string best = Unowned;
        uint bestLo = 0;
        foreach (var s in _spans)
            if (pkt >= s.Lo && pkt < s.Hi && s.Lo >= bestLo) { best = s.Label; bestLo = s.Lo; }
        return best;
    }

    static void Report()
    {
        long now = Environment.TickCount64;
        if (_windowStart < 0) { _windowStart = now; return; }
        double s = (now - _windowStart) / 1000.0;
        if (s < 5.0) return;
        long misses = _rows.Values.Sum(r => r.Vertices - r.Hits);
        Console.WriteLine($"[ST] mapcoverage: {_frames / s:0.0} frames/s, {misses / s:0} missed vertices/s" +
                          (GteVertexMap.Active ? "" : " (the address map is off)"));
        foreach (var (label, r) in _rows.OrderByDescending(kv => kv.Value.Vertices - kv.Value.Hits))
        {
            double hit = r.Vertices == 0 ? 0.0 : 100.0 * r.Hits / r.Vertices;
            Console.WriteLine($"[ST] mapcoverage:   {label}: {r.Packets / s:0}/s polygons, {r.Vertices / s:0}/s vertices, " +
                              $"{hit:0.0}% hit, {(r.Vertices - r.Hits) / s:0}/s missed");
        }
        _rows.Clear();
        _frames = 0;
        _windowStart = now;
        Console.Out.Flush();
    }
}
