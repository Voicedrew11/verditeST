using RecompOne.Runtime.Events;
using RecompOne.Runtime.Hardware;
using RecompOne.Runtime.Memory;

namespace St;

/// <summary>
/// Boot into play with nobody at the pad:
///
///     ST_AUTOSTART=new    Start through the movies, the title and the opening text
///     ST_AUTOSTART=load   Load on the title, then Cross through the card screen
///
/// <c>load</c> takes whichever save the card screen offers first. Both stop pressing
/// once the player is in an area, and print one line saying where. Presses go in
/// through <c>PadReadEvent</c>, as the command channel's do. See "Driving the game
/// without a person" in docs/DEVELOPMENT.md.
/// </summary>
public static class AutoStart
{
    enum Goal { None, NewGame, LoadSave }

    // A pulse: held for the first frames of every period, at 60 vblanks a second.
    const int StartPeriod = 30, StartHeld = 14;
    const int CrossPeriod = 40, CrossHeld = 12;

    // The title's menu, by vblanks after TITLE.EXE loads: past its fade-in, Start
    // opens the menu, Down moves to Load, Start picks it.
    const int TitleMenuOpen = 480, TitleMenuDown = 660, TitleMenuPick = 720;
    const int TitlePressHeld = 12;
    const int TitleGiveUp = 1800;

    // The card screen's step counter, non-zero while it runs (func_80062C28, "The
    // menu, saving and loading" in docs/GAME_INTERNALS.md).
    const uint CardScreenStep = 0x1F8002DC;
    const int SettledFrames = 120;
    const int CardScreenGiveUp = 3600;

    static Goal _goal;
    static IMemory _memory = null!;

    // Game thread.
    static ushort _held;
    static long _titleLoadedAt = -1;
    static long _gameLoadedAt = -1;
    static bool _cardScreenSeen;
    static int _settled;
    static bool _done;

    public static void Install(string? spec, IMemory memory)
    {
        _goal = ParseGoal(spec);
        if (_goal == Goal.None) return;
        _memory = memory;

        Event.AddListener<OverlayLoadedEvent>(OnOverlayLoaded);
        Event.AddListener<VSyncEvent>(OnVSync);
        Event.AddListener<PadReadEvent>(OnPadRead);
        Console.WriteLine($"[ST] autostart: {(_goal == Goal.NewGame ? "a new game" : "loading the first save")}");
    }

    static Goal ParseGoal(string? spec) => (spec ?? "").Trim().ToLowerInvariant() switch
    {
        "" or "0" or "off" => Goal.None,
        "new" => Goal.NewGame,
        "load" => Goal.LoadSave,
        _ => throw new ArgumentException($"ST_AUTOSTART: '{spec}' is not new or load"),
    };

    // Game thread.
    static void OnOverlayLoaded(OverlayLoadedEvent e)
    {
        if (e.Name == "title") _titleLoadedAt = AgentBeacon.Frames;
        if (e.Name == "st") _gameLoadedAt = AgentBeacon.Frames;
    }

    // Game thread.
    static void OnVSync(VSyncEvent e)
    {
        if (_done) return;
        long frame = AgentBeacon.Frames;
        _held = AgentBeacon.Program switch
        {
            "str" or "optxt" => Pulse(frame, StartPeriod, StartHeld, Controller.Start),
            "title" when _goal == Goal.NewGame => Pulse(frame, StartPeriod, StartHeld, Controller.Start),
            "title" => TitleMenu(frame - _titleLoadedAt),
            "st" when _goal == Goal.LoadSave => CardScreen(frame),
            "st" => NewGameArrived(),
            _ => (ushort)0,
        };
    }

    static ushort Pulse(long frame, int period, int held, ushort button) =>
        frame % period < held ? button : (ushort)0;

    static ushort TitleMenu(long sinceLoad)
    {
        if (sinceLoad >= TitleGiveUp) Finish("still at the title after the menu presses; stopping");
        if (sinceLoad is >= TitleMenuOpen and < TitleMenuOpen + TitlePressHeld) return Controller.Start;
        if (sinceLoad is >= TitleMenuDown and < TitleMenuDown + TitlePressHeld) return Controller.Down;
        if (sinceLoad is >= TitleMenuPick and < TitleMenuPick + TitlePressHeld) return Controller.Start;
        return 0;
    }

    // Cross through the card screen's prompts until it has run and been gone for a
    // couple of seconds with the player in an area.
    static ushort CardScreen(long frame)
    {
        bool running = _memory.ReadU16(CardScreenStep) != 0;
        _cardScreenSeen |= running;
        _settled = _cardScreenSeen && !running && AgentBeacon.InGame ? _settled + 1 : 0;
        if (_settled >= SettledFrames)
        {
            Finish("loaded, " + WhereThePlayerIs());
            return 0;
        }
        if (frame - _gameLoadedAt >= CardScreenGiveUp)
        {
            Finish("the card screen never finished (no save on card A?); stopping");
            return 0;
        }
        return Pulse(frame, CrossPeriod, CrossHeld, Controller.Cross);
    }

    static ushort NewGameArrived()
    {
        _settled = AgentBeacon.InGame ? _settled + 1 : 0;
        if (_settled >= SettledFrames) Finish("new game, " + WhereThePlayerIs());
        return 0;
    }

    static string WhereThePlayerIs()
    {
        var m = _memory;
        return $"map {AgentBeacon.Map}, HP {m.ReadU16(AgentBeacon.Hp)}/{m.ReadU16(AgentBeacon.MaxHp)}, " +
               $"at ({(int)m.ReadU32(AgentBeacon.PosX)}, {(int)m.ReadU32(AgentBeacon.PosY)}, {(int)m.ReadU32(AgentBeacon.PosZ)})";
    }

    static void Finish(string what)
    {
        _done = true;
        _held = 0;
        Console.WriteLine($"[ST] autostart: {what}");
    }

    // Game thread. The event's buttons are active-low with their two bytes swapped
    // against Controller's layout.
    static void OnPadRead(PadReadEvent e)
    {
        if (e.Port != 0 || _held == 0) return;
        e.Buttons &= (ushort)~(ushort)((_held >> 8) | (_held << 8));
    }
}
