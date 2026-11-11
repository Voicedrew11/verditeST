using System.Reflection;
using RecompOne.Runtime.Config;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Events;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;
using Silk.NET.Input;
using Verdite.Core;

namespace St;

/// <summary>
/// Gives the captured pointer back whenever something wants a pointer, as Verdite3
/// does, so the mouse is never stuck locked and hidden over a menu.
///
/// **The game's menu.** <c>func_80058EF4</c> (Select: Tab, which also closes it through MenuKey)
/// runs the whole menu session, its loop and every screen under it, and returns when
/// the menu closes. Its start is <see cref="Mouse.Suspend"/>(true): the pointer is
/// let go, and the mouse buttons stop pressing pad buttons in the menu. Its return is
/// Suspend(false): the pointer is captured again if the menu took it.
///
/// **The port's menu bar.** F1 showing the bar while the pointer is captured lets the
/// pointer go, since the bar is shown to be clicked. A click on the game captures again.
///
/// See "Mouse look" in docs/INPUT.md.
/// </summary>
public static class MenuPointer
{
    const uint Menu = 0x80058EF4;

    static readonly ModInfo _self = new() { Id = "st.menupointer", Name = "Menu pointer", Version = "1.0" };
    static bool _queued;

    public static void Install()
    {
        Event.AddListener<KeyboardEvent>(OnKey);
        // A load or a reset leaves any menu; a suspension must not outlive it.
        Event.AddListener<OverlayLoadedEvent>(_ => { Open = false; Mouse.Suspend(false); });
        HookAttach.OnOverlayLoad("menu pointer", Attach);
    }

    static bool Attach()
    {
        SymbolRegistry.Build();
        if (SymbolRegistry.Resolve(MainLoop.Overlay, null, Menu) is not { } target) return false;
        if (!_queued)
        {
            MethodInfo M(string n) => typeof(MenuPointer).GetMethod(n, BindingFlags.Public | BindingFlags.Static)!;
            HookManager.AddPre(_self, target, M(nameof(MenuOpened)));
            HookManager.AddPost(_self, target, M(nameof(MenuClosed)));
            _queued = true;
        }
        HookManager.Commit();
        bool ok = HookAttach.Installed(target);
        Console.WriteLine(ok ? "[ST] menu pointer: attached" : "[ST] menu pointer: not installed");
        return ok;
    }

    /// <summary>Whether the game's menu session is running.</summary>
    public static bool Open { get; private set; }

    public static void MenuOpened(CpuContext c, IMemory m)
    {
        Open = true;
        Mouse.Suspend(true);
    }

    public static void MenuClosed(CpuContext c, IMemory m)
    {
        Open = false;
        Mouse.Suspend(false);
    }

    /// <summary>F1 is about to show the bar (the host flips it on its next poll).</summary>
    static void OnKey(KeyboardEvent e)
    {
        if (!e.Pressed || e.Key != (int)Key.F1) return;
        if (!ConfigManager.View.HideTopBar || !Mouse.Captured) return;
        Console.WriteLine("[ST] mouse: released: the menu bar opened");
        Mouse.SetCaptured(false);
    }
}
