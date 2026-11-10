using System.Reflection;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// ST_GEOPROBE=1: what each of the draw stage's calls adds to the ordering table, by
/// GPU packet type. The table is walked before and after each call and the packets new
/// to it counted; one block every five seconds. Only calls made while stage 14
/// (<c>func_80026AFC</c>) is running count, and each by its call site, so the same
/// routine called from elsewhere is not. Rows nest: the scene's own calls are counted
/// inside the scene's row as well. Verdite1's GeometryProbe on this game's draw stage.
/// See "The draw path" in docs/GAME_INTERNALS.md.
/// </summary>
public static class GeometryProbe
{
    const int MaxPackets = 1 << 17;

    // The draw stage's callees and the scene's (func_80020A80), with the return address
    // each call site leaves in RA (scripts/callsites.py). Indented labels are the scene's.
    static readonly (string Label, uint Callee, uint Return)[] Sites =
    [
        ("176A8", 0x800176A8, 0x80026B30),
        ("16A14", 0x80016A14, 0x80026B38),
        ("33110", 0x80033110, 0x80026B6C),
        ("174C4", 0x800174C4, 0x80026B74),
        ("340F8", 0x800340F8, 0x80026B7C),
        ("34FC4", 0x80034FC4, 0x80026B84),
        ("27D18", 0x80027D18, 0x80026C3C),
        ("26964(1A)", 0x80026964, 0x80026C44),
        ("26508", 0x80026508, 0x80026C74),
        ("26964(0)", 0x80026964, 0x80026C7C),
        ("26228", 0x80026228, 0x80026CC4),
        ("26964(20)", 0x80026964, 0x80026CCC),
        ("26964(25)", 0x80026964, 0x80026CFC),
        ("2181C", 0x8002181C, 0x80026D04),
        ("218D8", 0x800218D8, 0x80026D0C),
        ("21654", 0x80021654, 0x80026D14),
        ("21AC4(0)", 0x80021AC4, 0x80026D1C),
        ("21AC4(1)", 0x80021AC4, 0x80026D24),
        ("scene 20A80", 0x80020A80, 0x80026D2C),
        ("  1C100 cells", 0x8001C100, 0x80020CA8),
        ("  1C264", 0x8001C264, 0x80020CB0),
        ("  1C60C", 0x8001C60C, 0x80020CB8),
        ("  1CB88", 0x8001CB88, 0x80020D08),
        ("  1CDF8", 0x8001CDF8, 0x80020DA0),
        ("  1F954", 0x8001F954, 0x80020DAC),
        ("  1FD00", 0x8001FD00, 0x80020DB8),
        ("  20164", 0x80020164, 0x80020DC4),
        ("  20588", 0x80020588, 0x80020DD0),
        ("  1D2A8", 0x8001D2A8, 0x80020DDC),
        ("  1DB64", 0x8001DB64, 0x80020DE8),
        ("  1E5A0", 0x8001E5A0, 0x80020DF4),
        ("  1EEAC", 0x8001EEAC, 0x80020E00),
        ("    22678 model(241D0 a)", 0x80022678, 0x80024440),
        ("    22678 model(241D0 b)", 0x80022678, 0x80024544),
        ("    22678 model(241D0 c)", 0x80022678, 0x80024614),
        ("    23C6C (241D0)", 0x80023C6C, 0x800246B0),
        ("    33DB8 (241D0)", 0x80033DB8, 0x800242C4),
        ("    22678 model(251C8 a)", 0x80022678, 0x80025418),
        ("    22678 model(251C8 b)", 0x80022678, 0x8002557C),
        ("    23ED8 (251C8)", 0x80023ED8, 0x800255B4),
        ("      17D28 (22678)", 0x80017D28, 0x800228D4),
        ("      34604 (22678)", 0x80034604, 0x80022BB0),
        ("      35AA8 (22678)", 0x80035AA8, 0x80022BFC),
        ("      22010 (22678)", 0x80022010, 0x80022C38),
        ("      1BF10 (22678)", 0x8001BF10, 0x80022D84),
        ("      192D4 (22678)", 0x800192D4, 0x80022F38),
        ("      19DC8 (22678)", 0x80019DC8, 0x80023184),
        ("    25E00 (26964)", 0x80025E00, 0x80026A20),
        ("    25C24 (26964)", 0x80025C24, 0x80026A34),
        ("    26020 (26964)", 0x80026020, 0x80026A54),
        ("    17D28 (21AC4)", 0x80017D28, 0x80021B0C),
        ("    34604 (21AC4)", 0x80034604, 0x80021E3C),
        ("    19DC8 (21AC4)", 0x80019DC8, 0x80021F48),
        ("    35AA8 (21AC4)", 0x80035AA8, 0x80021F90),
        ("    22010 (21AC4)", 0x80022010, 0x80021FA0),
        ("1C8E0", 0x8001C8E0, 0x80026D34),
        ("1C9C8", 0x8001C9C8, 0x80026D3C),
        ("1CAB8", 0x8001CAB8, 0x80026D44),
        ("241D0", 0x800241D0, 0x80026D4C),
        ("246E4", 0x800246E4, 0x80026D54),
        ("251C8", 0x800251C8, 0x80026D5C),
        ("255F0 cels", 0x800255F0, 0x80026D64),
        ("28784", 0x80028784, 0x80026D90),
    ];

    sealed class Row
    {
        public string Label = "";
        public long Calls, Words, Removed;
        public readonly Dictionary<string, long> Types = new();
    }

    static readonly ModInfo _self = new() { Id = "st.geoprobe", Name = "Geometry probe", Version = "1.0" };

    static readonly List<Row> _rows = new();
    static readonly Dictionary<uint, int> _rowByReturn = new();
    static readonly Stack<(int Row, HashSet<uint>? Before)> _calls = new();
    static int _depth;
    static long _mainFrames, _otherFrames;
    static long _windowStart = -1;

    public static void Install(string? spec)
    {
        if (spec?.Trim() != "1") return;
        for (int i = 0; i < Sites.Length; i++)
        {
            _rows.Add(new Row { Label = Sites[i].Label });
            _rowByReturn[Sites[i].Return] = i;
        }
        HookAttach.OnOverlayLoad("geoprobe", Attach);
    }

    static bool Attach()
    {
        SymbolRegistry.Build();
        var self = typeof(GeometryProbe);
        MethodInfo Hook(string name) => self.GetMethod(name, BindingFlags.Public | BindingFlags.Static)!;
        var targets = new List<MethodInfo>();
        foreach (uint a in Sites.Select(s => s.Callee).Append(MainLoop.Draw.Routine).Distinct())
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
        Console.WriteLine($"[ST] geoprobe: {n}/{targets.Count} functions hooked");
        return n == targets.Count;
    }

    // Stage 14 brackets the calls; its own hooks only count frames and report.
    public static void DrawPre(CpuContext c, IMemory m)
    {
        _depth++;
        if (c.RA == MainLoop.Draw.ReturnAddress) _mainFrames++; else _otherFrames++;
    }

    public static void DrawPost(CpuContext c, IMemory m)
    {
        if (_depth > 0) _depth--;
        Report();
    }

    public static void Pre(CpuContext c, IMemory m)
    {
        int row = _depth > 0 && _rowByReturn.TryGetValue(c.RA, out int r) ? r : -1;
        HashSet<uint>? before = null;
        if (row >= 0)
        {
            before = new HashSet<uint>();
            OrderingTable.EachPacket(m, MaxPackets, (p, _) => before.Add(p));
        }
        _calls.Push((row, before));
    }

    public static void Post(CpuContext c, IMemory m)
    {
        if (_calls.Count == 0) return;
        var (row, before) = _calls.Pop();
        if (row < 0 || before == null) return;
        var r = _rows[row];
        r.Calls++;
        int kept = 0;
        OrderingTable.EachPacket(m, MaxPackets, (p, tag) =>
        {
            if (before.Contains(p)) { kept++; return; }
            int len = (int)(tag >> 24);
            r.Words += len;
            string kind = len == 0 ? "empty" : OrderingTable.Kind(m.ReadU32(p + 4) >> 24);
            r.Types[kind] = r.Types.GetValueOrDefault(kind) + 1;
        });
        r.Removed += before.Count - kept;
    }

    static void Report()
    {
        long now = Environment.TickCount64;
        if (_windowStart < 0) { _windowStart = now; return; }
        double s = (now - _windowStart) / 1000.0;
        if (s < 5.0) return;
        Console.WriteLine($"[ST] geoprobe: {(_mainFrames + _otherFrames) / s:0.0} draw-stage calls/s " +
                          $"({_mainFrames / s:0.0} main loop, {_otherFrames / s:0.0} other)");
        foreach (var r in _rows)
        {
            if (r.Calls == 0) continue;
            double calls = r.Calls;
            string types = string.Join(" ", r.Types.OrderByDescending(kv => kv.Value).Take(8)
                .Select(kv => $"{kv.Key} {kv.Value / calls:0.0}"));
            Console.WriteLine($"[ST] geoprobe:   {r.Label} {r.Calls / s:0.0}/s: {(types.Length == 0 ? "no packets" : types)}; " +
                              $"{r.Words / calls:0} words/call" + (r.Removed > 0 ? $", {r.Removed} removed" : ""));
            r.Calls = r.Words = r.Removed = 0;
            r.Types.Clear();
        }
        _mainFrames = _otherFrames = 0;
        _windowStart = now;
        Console.Out.Flush();
    }
}
