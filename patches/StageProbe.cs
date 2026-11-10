using System.Reflection;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// ST_STAGEPROBE=1: which main-loop stages write the ordering table and present
/// frames. Each stage's table words are diffed across its call, and its calls and
/// VSync presents counted; one line every five seconds. Only calls from the main
/// loop's own call sites count. Verdite3's StageProbe on this game's loop. See "The
/// main loop" in docs/GAME_INTERNALS.md.
/// </summary>
public static class StageProbe
{
    static readonly ModInfo _self = new() { Id = "st.stageprobe", Name = "Stage probe", Version = "1.0" };

    static readonly MainLoop.Stage[] Stages = MainLoop.Stages;
    static readonly long[] _calls = new long[Stages.Length];
    static readonly long[] _otWrites = new long[Stages.Length];
    static readonly long[] _presents = new long[Stages.Length];
    static readonly uint[][] _before = new uint[Stages.Length][];
    static readonly long[] _vsyncAt = new long[Stages.Length];
    static readonly uint[] _after = new uint[MainLoop.OtWords];
    static readonly Stack<int> _open = new();
    static long _windowStart = -1;

    public static void Install(string? spec)
    {
        if (spec != "1") return;
        for (int i = 0; i < Stages.Length; i++) _before[i] = new uint[MainLoop.OtWords];
        HookAttach.OnOverlayLoad("stageprobe", Attach);
    }

    static bool Attach()
    {
        SymbolRegistry.Build();
        var pre = typeof(StageProbe).GetMethod(nameof(Pre), BindingFlags.Public | BindingFlags.Static)!;
        var post = typeof(StageProbe).GetMethod(nameof(Post), BindingFlags.Public | BindingFlags.Static)!;
        var targets = new List<MethodInfo>();
        foreach (var stage in Stages)
        {
            if (SymbolRegistry.Resolve(MainLoop.Overlay, null, stage.Routine) is not { } target) return false;
            HookManager.AddPre(_self, target, pre);
            HookManager.AddPost(_self, target, post);
            targets.Add(target);
        }
        HookManager.Commit();
        int hooked = targets.Count(HookAttach.Installed);
        Console.WriteLine($"[ST] stageprobe: {hooked}/{Stages.Length} stages hooked");
        return hooked == Stages.Length;
    }

    public static void Pre(CpuContext c, IMemory m)
    {
        int i = Array.FindIndex(Stages, s => s.ReturnAddress == c.RA);
        _open.Push(i);
        if (i < 0) return;
        _calls[i]++;
        _vsyncAt[i] = RecompOne.Runtime.Sdk.LibEtc.VSyncCalls;
        Snapshot(m, _before[i]);
    }

    public static void Post(CpuContext c, IMemory m)
    {
        int i = _open.Count > 0 ? _open.Pop() : -1;
        if (i < 0) return;
        _presents[i] += RecompOne.Runtime.Sdk.LibEtc.VSyncCalls - _vsyncAt[i];
        Snapshot(m, _after);
        for (int k = 0; k < MainLoop.OtWords; k++) if (_after[k] != _before[i][k]) _otWrites[i]++;
        if (i == Stages.Length - 1) Report();
    }

    static void Snapshot(IMemory m, uint[] into)
    {
        uint ot = m.ReadU32(MainLoop.OtPointer);
        if ((ot & 0xFF000000) != 0x80000000) { Array.Clear(into); return; }
        for (int k = 0; k < MainLoop.OtWords; k++) into[k] = m.ReadU32(ot + (uint)k * 4);
    }

    static void Report()
    {
        long now = Environment.TickCount64;
        if (_windowStart < 0) { _windowStart = now; return; }
        double seconds = (now - _windowStart) / 1000.0;
        if (seconds < 5.0) return;
        var parts = new List<string>();
        for (int i = 0; i < Stages.Length; i++)
        {
            parts.Add($"{Stages[i].Number}:{Stages[i].Routine:X8} {_calls[i] / seconds:0.0}/s " +
                      $"ot {_otWrites[i] / Math.Max(1, _calls[i])} vs {_presents[i]}");
            _calls[i] = _otWrites[i] = _presents[i] = 0;
        }
        Console.WriteLine("[ST] stageprobe: " + string.Join(" | ", parts));
        _windowStart = now;
    }
}
