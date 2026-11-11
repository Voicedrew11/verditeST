using RecompOne.Runtime.Events;
using RecompOne.Runtime.Hardware;

namespace St;

/// <summary>
/// One key opens and closes the game's menu, as in a desktop game. Select (Tab on the
/// port's keys) opens it, but the menu's loop ignores Select and leaves on Circle
/// (docs/GAME_INTERNALS.md, "The menu's loop"), so a player who opened it with Tab
/// could not close it with Tab. While the menu runs (<see cref="MenuPointer.Open"/>), a
/// Select press that began inside it is Circle: it closes the menu, or backs out of
/// a screen under it.
///
/// The press that opened the menu is not taken for a close: Select must be let go
/// inside the menu first. Nor is a Select still held when the menu closes taken for
/// an open: it is hidden from the game until it is let go.
///
/// Rewritten inside the pad read, so a pad's Select does the same. See "The keyboard"
/// in docs/INPUT.md.
/// </summary>
public static class MenuKey
{
    static bool _armed, _wasOpen, _holdingClose;

    public static void Install() => Event.AddListener(_onRead);

    static readonly Action<PadReadEvent> _onRead = e =>
    {
        if (e.Port != 0) return;
        bool select = Pressed(e.Buttons, Controller.Select);
        bool open = MenuPointer.Open;

        if (!open)
        {
            if (_wasOpen) _holdingClose = true;
            _wasOpen = _armed = false;
            if (!select) _holdingClose = false;
            if (_holdingClose) e.Buttons = Release(e.Buttons, Controller.Select);
            return;
        }

        _wasOpen = true;
        if (!_armed)
        {
            _armed = !select;
            return;
        }
        if (!select) return;
        e.Buttons = Release(e.Buttons, Controller.Select);
        e.Buttons = Press(e.Buttons, Controller.Circle);
    };

    // The pad read's word is active low, its two button bytes swapped from Controller's.
    static ushort Bit(ushort button) => (ushort)((button >> 8) | (button << 8));

    static bool Pressed(ushort buttons, ushort button) => (buttons & Bit(button)) == 0;

    static ushort Press(ushort buttons, ushort button) => (ushort)(buttons & ~Bit(button));

    static ushort Release(ushort buttons, ushort button) => (ushort)(buttons | Bit(button));
}
