namespace St;

/// <summary>
/// Where <c>warp &lt;map&gt;</c> puts the player when no point is given: the arrival
/// point a door into that map names. Each area's object records (0x58 bytes from
/// <c>0x801C2230</c>) hold, for a door (type <c>0xED</c> or <c>0xEF</c> at <c>+0x05</c>),
/// the target map at <c>+0x48</c> and the arrival point in it at <c>+0x49..+0x4F</c>, which
/// the door code decodes at <c>0x800525D0</c>. scripts/door_census.py read these from every
/// map on 2026-11-10; each one here kept the player alive and standing for six seconds.
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
    /// door names it (-1 for the new game's start). The door code writes the heading's
    /// byte, times 0x400, to <c>player+0x26A</c> and zeroes <c>+0x268</c>; which of the two
    /// is the yaw is not settled, so the warp does not write it.</summary>
    public readonly record struct Point(int X, int Y, int Z, int Heading, int From);

    static readonly Dictionary<int, Point> Known = new()
    {
        [0] = new(262144, 267712, 258048, 0, -1),   // a new game's start
        [5] = new(1290240, 260544, 1302528, 3072, 6),   // map 6's record 18, type ED
        [6] = new(1286144, 260544, 282624, 3072, 5),   // map 5's record 26, type ED
        [9] = new(1301504, 260544, 274432, 3072, 37),   // map 37's record 6, type ED
        [10] = new(286720, 260544, 286720, 2048, 17),   // map 17's record 3, type ED
        [13] = new(323584, 260544, 1282048, 0, 38),   // map 38's record 3, type EF
        [14] = new(1253376, 264128, 1282048, 3072, 25),   // map 25's record 7, type ED
        [15] = new(1277952, 260544, 1269760, 3072, 6),   // map 6's record 41, type ED
        [17] = new(294912, 260544, 275456, 3072, 10),   // map 10's record 14, type ED
        [18] = new(397312, 276928, 1294336, 0, 20),   // map 20's record 93, type EF
        [20] = new(1286144, 1306048, 1277952, 0, 18),   // map 18's record 17, type EF
        [23] = new(344064, 259520, 262144, 0, 25),   // map 25's record 10, type ED
        [24] = new(262144, 256448, 270336, 2048, 18),   // map 18's record 6, type EF
        [25] = new(1257472, 260544, 262144, 2048, 14),   // map 14's record 2, type ED
        [30] = new(262144, 260544, 274432, 0, 41),   // map 41's record 21, type ED
        [33] = new(1306624, 260544, 307200, 3072, 15),   // map 15's record 19, type ED
        [37] = new(1297408, 260544, 272384, 3072, 9),   // map 9's record 10, type ED
        [38] = new(284672, 260544, 266240, 2048, 2),   // map 2's record 10, type ED
        [41] = new(262144, 285120, 262144, 2048, 30),   // map 30's record 9, type EF
        [44] = new(262144, 260544, 262144, 0, 5),   // map 5's record 25, type ED
        [45] = new(262144, 260288, 259072, 0, 6),   // map 6's record 17, type ED
        [46] = new(262144, 260288, 259072, 0, 2),   // map 2's record 11, type ED
        [47] = new(262144, 260288, 259072, 0, 11),   // map 11's record 41, type ED
    };

    /// <summary>The arrival point for a map, if a door into it is known and was safe.
    /// Map 2's (from map 38) killed the player at once; twelve maps have no door into
    /// them among the records.</summary>
    public static Point? For(int map) => Known.TryGetValue(map, out var p) ? p : null;
}
