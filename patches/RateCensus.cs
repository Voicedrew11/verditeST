using System.Runtime.InteropServices;
using RecompOne.Runtime.Memory;

namespace St;

/// <summary>
/// ST_RATECENSUS=&lt;seconds&gt; (with pacing on, drawing above the tick rate): which RAM
/// words change between two frames on which no main-loop stage ran, i.e. what still
/// moves at the drawn rate. Prints the busiest runs once, then stops. Verdite3's
/// RateCensus. See "What runs at the drawn rate" in docs/GAME_INTERNALS.md.
/// </summary>
public static class RateCensus
{
    const int RamBytes = 0x200000;
    const int Lo = 0x10000 / 4, Hi = RamBytes / 4;

    static int _seconds;
    static uint[]? _previous;
    static int[]? _changes;
    static int _idlePairs;
    static bool _previousIdle;
    static long _start = -1;
    static bool _done;

    public static void Install(string? seconds)
    {
        if (!int.TryParse(seconds, out _seconds) || _seconds <= 0) return;
        _previous = new uint[Hi];
        _changes = new int[Hi];
        FramePacing.Boundary += Sample;
    }

    // At each frame boundary, so no frame is caught half built, and only in an area;
    // a frame is idle when the main-loop iteration that built it skipped its stages.
    static void Sample(IMemory memory)
    {
        if (_done || memory is not PSMemory m || !AgentBeacon.InGame) return;

        var ram = MemoryMarshal.Cast<byte, uint>(m.Ram[..RamBytes]);
        bool idle = !FramePacing.IterationTicked;
        if (idle && _previousIdle)
        {
            _idlePairs++;
            for (int i = Lo; i < Hi; i++) if (ram[i] != _previous![i]) _changes![i]++;
        }
        ram.CopyTo(_previous);
        _previousIdle = idle;

        if (_start < 0) _start = Environment.TickCount64;
        if (Environment.TickCount64 - _start < _seconds * 1000L) return;
        _done = true;
        Report();
    }

    static void Report()
    {
        int n = _idlePairs;
        Console.WriteLine($"[ST] ratecensus: {n} idle-to-idle frame pairs; runs of words changing on >= 25% of them:");
        int i = Lo;
        while (i < Hi)
        {
            if (_changes![i] * 4 < n) { i++; continue; }
            int start = i, peak = 0;
            while (i < Hi && (_changes[i] * 4 >= n || (i + 1 < Hi && _changes[i + 1] * 4 >= n)))
            {
                peak = Math.Max(peak, _changes[i]);
                i++;
            }
            Console.WriteLine($"[ST] ratecensus: 0x{0x80000000u + (uint)start * 4:X8}-0x{0x80000000u + (uint)i * 4:X8} " +
                              $"({i - start} words) peak {100.0 * peak / Math.Max(1, n):0}%");
        }
        Console.WriteLine("[ST] ratecensus: done");
        Console.Out.Flush();
    }
}
