using RecompOne.Runtime.Events;
using RecompOne.Runtime.Memory;

namespace St;

/// <summary>
/// What a program needs to know about the game without a screenshot: which program
/// runs, which area is in, the player's HP and MP and where they stand. The command
/// channel's <c>state</c> answers with <see cref="Snapshot"/>; ST_AGENT=1 also prints
/// it as an [ST-AGENT] line about once a second, and a line on every program or area
/// load. Read-only. See "Driving the game without a person" in docs/DEVELOPMENT.md and
/// "The player" in docs/GAME_INTERNALS.md.
/// </summary>
public static class AgentBeacon
{
    // ST.EXE's player block, 0x80198F28. func_8003CF28 adds to HP and clamps it to the
    // maximum; at 0 it calls func_8003BD14, which sets the state byte to 4.
    public const uint MaxHp = 0x80198F28;   // u16
    public const uint Hp = 0x80198F2A;      // u16
    public const uint MaxMp = 0x80198F2C;   // u16
    public const uint Mp = 0x80198F2E;      // u16
    public const uint State = 0x80199140;   // u8, block + 0x218: 0 free to act, 4 dead
    public const uint PosX = 0x80199170;    // s32, block + 0x248
    public const uint PosY = 0x80199174;    // s32, the height: the eye, larger is lower
    public const uint PosZ = 0x80199178;    // s32
    public const uint Heading = 0x80199190; // u16, an angle: func_8003BD14 masks it with 0x7FF
    public const byte StateDead = 4;

    const long PeriodMs = 1000;
    const int NoArea = -1;

    static IMemory _memory = null!;
    static bool _printing;
    static long _lastPrintMs;

    // Game thread.
    static string _program = "boot";
    static int _map = NoArea;

    /// <summary>The running program: str, title, optxt, st, vs or end.</summary>
    public static string Program => _program;

    /// <summary>The area module's map number (FDAT.T entry 10n+2), or -1 outside ST.EXE.</summary>
    public static int Map => _map;

    /// <summary>Vblanks seen since boot.</summary>
    public static long Frames { get; private set; }

    /// <summary>In an area with a player: ST.EXE, a module in, a maximum HP set.</summary>
    public static bool InGame => _program == "st" && _map != NoArea && _memory.ReadU16(MaxHp) != 0;

    public static void Install(string? spec, IMemory memory)
    {
        _memory = memory;
        _printing = (spec ?? "").Trim().ToLowerInvariant() is "1" or "on";
        Event.AddListener<OverlayLoadedEvent>(OnOverlayLoaded);
        Event.AddListener<VSyncEvent>(OnVSync);
        if (_printing) Console.WriteLine("[ST-AGENT] beacon on");
    }

    // Game thread. A program load leaves the area module's name behind in the
    // dispatcher, so the area is cleared here rather than read from it.
    static void OnOverlayLoaded(OverlayLoadedEvent e)
    {
        if (TryParseAreaModule(e.Name, out int map))
            _map = map;
        else
        {
            _program = e.Name;
            _map = NoArea;
        }
        if (_printing) Console.WriteLine($"[ST-AGENT] loaded {e.Name}");
    }

    static bool TryParseAreaModule(string name, out int map)
    {
        map = NoArea;
        if (!name.StartsWith("fdat", StringComparison.Ordinal) || !int.TryParse(name.AsSpan(4), out int entry))
            return false;
        map = entry / 10;
        return true;
    }

    // Game thread.
    static void OnVSync(VSyncEvent e)
    {
        Frames++;
        if (!_printing) return;
        long now = Environment.TickCount64;
        if (now - _lastPrintMs < PeriodMs) return;
        _lastPrintMs = now;
        Console.WriteLine("[ST-AGENT] " + Snapshot());
    }

    /// <summary>One line of JSON. Game thread.</summary>
    public static string Snapshot()
    {
        string head = $"\"program\":\"{_program}\",\"map\":{_map},\"frames\":{Frames}";
        if (!InGame) return $"{{{head},\"inGame\":false}}";

        var m = _memory;
        return $"{{{head},\"inGame\":true," +
               $"\"hp\":{m.ReadU16(Hp)},\"maxHp\":{m.ReadU16(MaxHp)}," +
               $"\"mp\":{m.ReadU16(Mp)},\"maxMp\":{m.ReadU16(MaxMp)}," +
               $"\"dead\":{(m.ReadU8(State) == StateDead ? "true" : "false")}," +
               $"\"pos\":[{(int)m.ReadU32(PosX)},{(int)m.ReadU32(PosY)},{(int)m.ReadU32(PosZ)}]," +
               $"\"heading\":{m.ReadU16(Heading)}}}";
    }
}
