using System.Reflection;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// ST_PRESENTPROBE=1: who sends each frame to the screen. ST.EXE draws a frame with
/// <c>func_80017634</c> (<c>DrawOTag</c> of the table), called either from the swap
/// <c>func_80017680</c> or directly; each present is counted by the return address of
/// the swap's caller, or of the direct call. Stage 14 is the main loop's draw; any
/// other address is a loop that draws its own frames (the menu, death, the card
/// screens). One line every five seconds, when anything was presented. See "Frames
/// drawn outside stage 14" in docs/GAME_INTERNALS.md.
/// </summary>
public static class PresentProbe
{
    const uint Swap = 0x80017680;
    const uint Present = 0x80017634;

    static readonly ModInfo _self = new() { Id = "st.presentprobe", Name = "Present probe", Version = "1.0" };

    static readonly Stack<uint> _swapCallers = new();
    static readonly Dictionary<string, long> _presents = new();
    static long _windowStart = -1;

    public static void Install(string? spec)
    {
        if (spec?.Trim() != "1") return;
        HookAttach.OnOverlayLoad("presentprobe", Attach);
    }

    static bool Attach()
    {
        SymbolRegistry.Build();
        if (SymbolRegistry.Resolve(MainLoop.Overlay, null, Swap) is not { } swap) return false;
        if (SymbolRegistry.Resolve(MainLoop.Overlay, null, Present) is not { } present) return false;
        MethodInfo Hook(string name) => typeof(PresentProbe).GetMethod(name, BindingFlags.Public | BindingFlags.Static)!;
        HookManager.AddPre(_self, swap, Hook(nameof(SwapPre)));
        HookManager.AddPost(_self, swap, Hook(nameof(SwapPost)));
        HookManager.AddPre(_self, present, Hook(nameof(PresentPre)));
        HookManager.Commit();
        bool ok = HookAttach.Installed(swap) && HookAttach.Installed(present);
        Console.WriteLine($"[ST] presentprobe: {(ok ? "hooked" : "not hooked")}");
        return ok;
    }

    public static void SwapPre(CpuContext c, IMemory m) => _swapCallers.Push(c.RA);

    public static void SwapPost(CpuContext c, IMemory m)
    {
        if (_swapCallers.Count > 0) _swapCallers.Pop();
    }

    public static void PresentPre(CpuContext c, IMemory m)
    {
        string key = _swapCallers.Count > 0 ? $"swap ra {_swapCallers.Peek():X8}" : $"direct ra {c.RA:X8}";
        _presents[key] = _presents.GetValueOrDefault(key) + 1;
        Report();
    }

    static void Report()
    {
        long now = Environment.TickCount64;
        if (_windowStart < 0) { _windowStart = now; return; }
        double s = (now - _windowStart) / 1000.0;
        if (s < 5.0) return;
        string all = string.Join(", ", _presents.OrderByDescending(kv => kv.Value)
            .Select(kv => $"{kv.Key} {kv.Value / s:0.0}/s"));
        Console.WriteLine($"[ST] presentprobe: {all}");
        Console.Out.Flush();
        _presents.Clear();
        _windowStart = now;
    }
}
