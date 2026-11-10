using System.Diagnostics;
using System.Reflection;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// ST_DRAW_TIMING=1: how long the main loop's stage 14 takes up to its swap
/// (<c>func_80017680</c>, which waits on the pacing, so is left out): the whole
/// frame's drawing on the CPU, packets included, per map, every five seconds: mean, 95th
/// percentile and worst in milliseconds, and calls a second. Phase 11's Step 0.3: the
/// reference figure with the GPU path off, so the GPU path's figure has something to
/// be compared with. Wall time on the game thread, so other probes on at the same time
/// inflate it; measure with nothing else on. See docs/GPU_COST.md.
/// </summary>
public static class DrawTiming
{
    static readonly ModInfo _self = new() { Id = "st.drawtiming", Name = "Draw timing", Version = "1.0" };
    static readonly List<double> _ms = [];
    static readonly Stopwatch _clock = new();
    const uint Swap = 0x80017680;
    static int _depth;
    static long _windowStart = -1;

    public static void Install(string? spec)
    {
        if (spec?.Trim() != "1") return;
        HookAttach.OnOverlayLoad("draw timing", Attach);
    }

    static bool Attach()
    {
        SymbolRegistry.Build();
        if (SymbolRegistry.Resolve(MainLoop.Overlay, null, MainLoop.Draw.Routine) is not { } t) return false;
        MethodInfo Hook(string n) => typeof(DrawTiming).GetMethod(n, BindingFlags.Public | BindingFlags.Static)!;
        // Outermost (hooks run in ascending order), so every other hook on stage 14 is inside the time.
        HookManager.AddPre(_self, t, Hook(nameof(Pre)), order: int.MinValue);
        HookManager.AddPost(_self, t, Hook(nameof(Post)), order: int.MaxValue);
        if (SymbolRegistry.Resolve(MainLoop.Overlay, null, Swap) is not { } swap) return false;
        HookManager.AddPre(_self, swap, Hook(nameof(SwapPre)), order: int.MinValue);
        HookManager.Commit();
        Console.WriteLine($"[ST] draw timing: {(HookAttach.Installed(t) ? "on" : "not attached")}");
        return HookAttach.Installed(t);
    }

    public static void Pre(CpuContext c, IMemory m)
    {
        if (_depth++ == 0 && c.RA == MainLoop.Draw.ReturnAddress) _clock.Restart();
    }

    // The clock stops where the frame is handed to the swap, before its wait.
    public static void SwapPre(CpuContext c, IMemory m)
    {
        if (_depth != 1 || !_clock.IsRunning) return;
        _clock.Stop();
        _ms.Add(_clock.Elapsed.TotalMilliseconds);
        Report();
    }

    public static void Post(CpuContext c, IMemory m)
    {
        if (_depth > 0) _depth--;
    }

    static void Report()
    {
        long now = Environment.TickCount64;
        if (_windowStart < 0) _windowStart = now;
        double s = (now - _windowStart) / 1000.0;
        if (s < 5.0) return;
        _ms.Sort();
        double mean = _ms.Average(), p95 = _ms[(int)(_ms.Count * 0.95)], worst = _ms[^1];
        Console.WriteLine($"[ST] draw timing in {AgentBeacon.Map}: {_ms.Count / s:0.0} a second, " +
                          $"mean {mean:0.000} ms, p95 {p95:0.000} ms, worst {worst:0.000} ms");
        Console.Out.Flush();
        _ms.Clear();
        _windowStart = now;
    }
}
