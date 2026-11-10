using System.Globalization;
using System.Reflection;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// ST_WORDWATCH=&lt;hex address&gt;: which of stage 14's calls changes a RAM word. The
/// word is read before and after every call in <see cref="GeometryProbe.Sites"/> made
/// inside stage 14; every five seconds, the calls that changed it, how often, and on
/// how many of those the frame was idle (no world tick). For a word the rate census
/// (<see cref="RateCensus"/>) shows changing between ticks. Nested calls each count.
/// </summary>
public static class WordWatch
{
    static readonly ModInfo _self = new() { Id = "st.wordwatch", Name = "Word watch", Version = "1.0" };

    static uint _address;
    static readonly Dictionary<uint, string> _labelByReturn = new();
    static readonly Stack<(string? Label, uint Before)> _calls = new();
    static readonly SortedDictionary<string, (long All, long Idle)> _changes = new();
    static int _depth;
    static long _windowStart = -1;

    public static void Install(string? spec)
    {
        if (string.IsNullOrWhiteSpace(spec) ||
            !uint.TryParse(spec.Trim(), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _address)) return;
        foreach (var s in GeometryProbe.Sites) _labelByReturn[s.Return] = s.Label.Trim();
        HookAttach.OnOverlayLoad("wordwatch", Attach);
    }

    static bool Attach()
    {
        SymbolRegistry.Build();
        var self = typeof(WordWatch);
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
        Console.WriteLine($"[ST] wordwatch: 0x{_address:X8}, {targets.Count(HookAttach.Installed)}/{targets.Count} functions hooked");
        return true;
    }

    public static void DrawPre(CpuContext c, IMemory m) => _depth++;

    public static void DrawPost(CpuContext c, IMemory m)
    {
        if (_depth > 0) _depth--;
        if (_depth == 0) Report();
    }

    public static void Pre(CpuContext c, IMemory m)
    {
        string? label = _depth > 0 && _labelByReturn.TryGetValue(c.RA, out var l) ? l : null;
        _calls.Push((label, m.ReadU32(_address)));
    }

    public static void Post(CpuContext c, IMemory m)
    {
        if (_calls.Count == 0) return;
        var (label, before) = _calls.Pop();
        if (label == null || m.ReadU32(_address) == before) return;
        var (all, idle) = _changes.GetValueOrDefault(label);
        _changes[label] = (all + 1, idle + (FramePacing.DrawingIdleFrame ? 1 : 0));
    }

    static void Report()
    {
        long now = Environment.TickCount64;
        if (_windowStart < 0) { _windowStart = now; return; }
        if (now - _windowStart < 5000) return;
        string calls = string.Join(", ", _changes.Select(kv => $"{kv.Key} {kv.Value.All} ({kv.Value.Idle} idle)"));
        Console.WriteLine($"[ST] wordwatch: 0x{_address:X8} changed by {(calls.Length == 0 ? "no call" : calls)}");
        _changes.Clear();
        _windowStart = now;
    }
}
