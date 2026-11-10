using System.Globalization;
using RecompOne.Runtime.Memory;

namespace St;

/// <summary>
/// <c>warp &lt;map&gt; [x y z]</c> for the command channel, as a door does it: ask for the
/// area with <c>func_80045044(map, map, map, 0xFF, 0xFF, 0x7F, 0x7F, 0x7F, 0, 0, 0)</c>
/// (the door code at <c>0x80052530</c>), let the main loop's loader stage bring it in,
/// and only then put the player at the arrival point. Writing the point first drops the
/// player through the old map's floor and kills them before the load ends. Without a
/// point, the arrival is <see cref="Arrivals"/>'s. See "Changing areas" in
/// docs/GAME_INTERNALS.md.
/// </summary>
static class Warp
{
    public const uint RequestArea = 0x80045044;

    /// <summary>u16, the loader's state (0 idle), then the map in at <c>+4</c>.</summary>
    public const uint Loader = 0x801B5920;
    const uint CurrentMap = Loader + 4;

    // Game thread and command thread both touch this; a lock keeps it whole.
    static readonly object _lock = new();
    static (int Map, int X, int Y, int Z)? _pending;

    /// <summary>Parses the command; returns the reply and, for another map, the
    /// request's arguments for the caller to queue on the game thread.</summary>
    public static (string Reply, uint[]? Request) Start(string[] words, IMemory memory)
    {
        int map = int.Parse(words[1], CultureInfo.InvariantCulture);
        if (!Arrivals.Maps.Contains(map)) throw new ArgumentException($"map {map} has no area module");
        if (AgentBeacon.Program != "st") throw new InvalidOperationException("warp needs ST.EXE running");
        // A request made while the loader is still bringing an area in is dropped by
        // it, and the map number at +4 is not yet the new one.
        if (memory.ReadU16(Loader) != 0) throw new InvalidOperationException("busy: the area loader is running; try again");
        if (words.Length is not (2 or 5)) throw new ArgumentException("warp takes a map and optionally x y z");

        (int X, int Y, int Z)? point = null;
        if (words.Length == 5)
            point = (Parse(words[2]), Parse(words[3]), Parse(words[4]));
        else if (Arrivals.For(map) is { } door)
            point = (door.X, door.Y, door.Z);
        string at = point is { } p ? $"({p.X}, {p.Y}, {p.Z})" : "where the player stands";

        if (map == memory.ReadU8(CurrentMap))
        {
            if (point is { } here) Place(memory, here.X, here.Y, here.Z);
            return ($"moved within map {map} to {at}", null);
        }
        lock (_lock) _pending = point is { } q ? (map, q.X, q.Y, q.Z) : null;
        memory.WriteU8(AgentBeacon.State, 0);
        return ($"warping to map {map} at {at}", [(uint)map, (uint)map, (uint)map, 0xFF, 0xFF, 0x7F, 0x7F, 0x7F, 0, 0, 0]);
    }

    /// <summary>Game thread, every vblank: once the requested map is in and the loader
    /// idle, the arrival point is written, as the door code does at 0x800525E8.</summary>
    public static void Settle(IMemory memory)
    {
        lock (_lock)
        {
            if (_pending is not { } p) return;
            if (memory.ReadU8(CurrentMap) != p.Map || memory.ReadU16(Loader) != 0) return;
            Place(memory, p.X, p.Y, p.Z);
            _pending = null;
        }
    }

    /// <summary>The player block (<c>docs/GAME_INTERNALS.md</c>, "The player").</summary>
    const uint Player = 0x80198F28;

    // The door's own stores around the arrival (0x800525A4..0x80052628) and those of
    // func_8003EAD0, which it calls next: the motion it carried is zeroed, so the
    // arrival is not taken as a fall from the old height.
    static void Place(IMemory memory, int x, int y, int z)
    {
        foreach (uint half in new uint[] { 0x288, 0x28A, 0x28C, 0x2C8, 0x2CA, 0x2CC })
            memory.WriteU16(Player + half, 0);
        memory.WriteU32(AgentBeacon.PosX, (uint)x);
        memory.WriteU32(AgentBeacon.PosZ, (uint)z);
        foreach (uint word in new uint[] { 0x2E4, 0x2E8, 0x2EC, 0x2F0 })
            memory.WriteU32(Player + word, 0);
        memory.WriteU32(AgentBeacon.PosY, (uint)y);
        memory.WriteU8(Player + 0x21E, 1);
    }

    static int Parse(string s) => int.Parse(s, CultureInfo.InvariantCulture);
}
