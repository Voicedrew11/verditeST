namespace St;

/// <summary>
/// Where <c>warp &lt;map&gt;</c> puts the player when no point is given: the arrival
/// point a door into that map names. Each area's object records (0x58 bytes from
/// <c>0x801C2230</c>) hold, for a door (type <c>0xED</c> or <c>0xEF</c> at <c>+0x05</c>),
/// the target map at <c>+0x48</c> and the arrival point in it at <c>+0x49..+0x4F</c>, which
/// the door code decodes at <c>0x800525D0</c>. scripts/door_census.py read these from every
/// map on 2026-11-10. Each one here kept the player alive for a visit of scripts/tour.py
/// (stand, turn a circle) with the scene drawn, once the warp loaded areas as a door
/// does; the other door points read killed the player (docs/GAME_INTERNALS.md).
/// See "Arrival points" in docs/GAME_INTERNALS.md.
/// </summary>
static class Arrivals
{
    /// <summary>The maps with an area module, FDAT.T entry 10n+2 (config/st.json); the
    /// others' entries are not modules.</summary>
    public static readonly HashSet<int> Maps =
    [
        0, 1, 2, 3, 4, 5, 6, 8, 9, 10, 11, 12, 13, 14, 15, 17, 18, 20,
        21, 23, 24, 25, 26, 27, 28, 30, 32, 33, 37, 38, 40, 41, 44, 45, 46, 47,
    ];

    /// <summary>A point in a map: position, the heading the door gives, and the map whose
    /// door names it (-1 for the new game's start, -2 for the cell every 0xE1 record
    /// decodes to, used where no door is known). The door code writes the heading's
    /// byte, times 0x400, to <c>player+0x26A</c> and zeroes <c>+0x268</c>; which of the two
    /// is the yaw is not settled, so the warp does not write it.</summary>
    public readonly record struct Point(int X, int Y, int Z, int Heading, int From);

    static readonly Dictionary<int, Point> Known = new()
    {
        [0] = new(262144, 267712, 258048, 0, -1),   // a new game's start
        [2] = new(262144, 260544, 262144, 0, -2),   // cell (0x40, 0x40, 0x40), the point 0xE1 records decode to
        [4] = new(262144, 260544, 262144, 0, -2),   // cell (0x40, 0x40, 0x40), the point 0xE1 records decode to
        [21] = new(262144, 260544, 262144, 0, -2),   // cell (0x40, 0x40, 0x40), the point 0xE1 records decode to
        [23] = new(344064, 259520, 262144, 0, 25),   // map 25's record 10, type ED
        [24] = new(262144, 256448, 270336, 2048, 18),   // map 18's record 6, type EF
        [32] = new(262144, 260544, 262144, 0, -2),   // cell (0x40, 0x40, 0x40), the point 0xE1 records decode to
        [38] = new(284672, 260544, 266240, 2048, 2),   // map 2's record 10, type ED
        [41] = new(262144, 285120, 262144, 2048, 30),   // map 30's record 9, type EF
        [44] = new(262144, 260544, 262144, 0, 5),   // map 5's record 25, type ED
        [45] = new(262144, 260288, 259072, 0, 6),   // map 6's record 17, type ED
        [46] = new(262144, 260288, 259072, 0, 2),   // map 2's record 11, type ED
    };

    /// <summary>The arrival point for a map, if a door into it is known and was safe.
    /// Twelve maps; the other 24 have no point that holds yet.</summary>
    public static Point? For(int map) => Known.TryGetValue(map, out var p) ? p : null;
}
