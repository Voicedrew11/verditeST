using RecompOne.Runtime.Config;
using RecompOne.Runtime.Events;
using RecompOne.Runtime.Hardware;
using RecompOne.Runtime.Host.Window;
using Silk.NET.Input;
using Verdite.Core;
using Rt = RecompOne.Runtime.Runtime;

namespace St;

/// <summary>
/// The keyboard layout the port ships, not RecompOne's console-on-a-keyboard one (face
/// buttons on Z X A S, shoulders on Q W E R, the D-pad on the arrows), which puts
/// walking and turning on the same four keys. Verdite3's KeyLayout with this game's
/// actions (the button table in docs/INPUT.md).
///
///     ST_KEYS=fps      force this layout on (the default for a fresh install)
///     ST_KEYS=stock    leave RecompOne's own alone
///
/// A fresh install gets it as the default (<see cref="Configure"/> runs before the
/// config loads); an existing install is migrated once, and only from exactly the
/// stock bindings, so a key someone chose stops it. Verdite Core's KeyLayoutApply does
/// both; the marker is kept in interface.ini. Tab is the menu's key, and
/// closes it too (MenuKey).
/// See "The keyboard" in docs/INPUT.md.
/// </summary>
public static class KeyLayout
{
    public const string AppliedKey = "st.keys.layout";
    const int Version = 1;

    /// <summary>Layouts shipped before and since changed; none yet.</summary>
    static readonly KeyBindings[] Superseded = [];

    /// <summary>Which key presses each pad button; what the button does is the game's
    /// own button table, as for a pad.</summary>
    public static KeyBindings Layout() => new()
    {
        // Walk and sidestep: the table's Up/Down (player+0x2C0) and L1/R1 (+0x2B8).
        Up = "W",
        Down = "S",
        L1 = "A",
        R1 = "D",

        // Turn (+0x2CA) on the arrows; KeyLayoutApply's extras walk on them too.
        Left = "Left",
        Right = "Right",

        // Looking up and down (+0x2CC) is the mouse's, as in Verdite3.
        L2 = "",
        R2 = "",

        // The hands, the action (doors, what is in front), the menu.
        Square = "Space",
        Triangle = "Q",
        Cross = "F",
        Circle = "E",
        Start = "Enter",
        Select = "Tab",

        L3 = "",
        R3 = "",
    };

    const string Announce = "WASD layout applied (W/S walk, A/D sidestep, arrows walk and turn, " +
                            "Space and Q the hands, F act, Tab menu). Input settings has both layouts.";

    /// <summary>Install as the default bindings. Must run before the config loads.</summary>
    public static void Configure() =>
        KeyLayoutApply.Configure(Layout, Version, Superseded, Announce, GetApplied, SetApplied);

    public static void Install()
    {
        KeyLayoutApply.Install();
    }

    public static void Apply() => KeyLayoutApply.Apply();
    public static void ApplyStock() => KeyLayoutApply.ApplyStock();
    public static bool IsApplied() => KeyLayoutApply.IsApplied();

    static int GetApplied() => Rt.View.GetInt(AppliedKey, 0);

    static void SetApplied(int version)
    {
        Rt.View.SetInt(AppliedKey, version);
        Rt.SaveView();
    }
}
