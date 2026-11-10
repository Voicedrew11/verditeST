using System.Diagnostics;
using System.Globalization;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Sdk;

namespace St;

/// <summary>
/// ST_FPS_PROBE=1: <see cref="FramePacing"/>'s line a second: fps drawn, VSync calls,
/// ticks taken, stages run and skipped, and packets a frame on ticked and idle frames.
/// A gated stage that fed the picture would show as fewer packets on idle frames.
/// With the boundary lost, a line from the vblank says how fast the watchdog ticks.
/// See "Frame pacing" in docs/DEVELOPMENT.md.
/// </summary>
static class PacingProbe
{
    public static bool On { get; set; }

    static readonly Stopwatch _clock = Stopwatch.StartNew();
    static double _windowStart;
    static long _frames, _ticks, _presentsMark, _stageRuns, _stageSkips;
    static long _packetsTicked, _framesTicked, _packetsIdle, _framesIdle;
    static double _watchdogStart = -1;
    static long _fallbackTicks;

    public static void Tick() => _ticks++;

    public static void FallbackTick() => _fallbackTicks++;

    public static void Stage(bool ran)
    {
        if (ran) _stageRuns++; else _stageSkips++;
    }

    /// <summary>A frame boundary: count the frame just drawn, and report once a second.</summary>
    public static void Frame(IMemory m, double now, bool ticked)
    {
        if (!On) return;
        long packets = CountPackets(m);
        if (ticked) { _packetsTicked += packets; _framesTicked++; }
        else { _packetsIdle += packets; _framesIdle++; }

        _frames++;
        double elapsed = now - _windowStart;
        if (elapsed < 1000.0) return;
        long presents = LibEtc.VSyncCalls - _presentsMark;
        _presentsMark = LibEtc.VSyncCalls;
        string target = FramePacing.Uncapped ? "uncapped" : $"{FramePacing.TargetFps:0.#}";
        Console.WriteLine($"[ST] pacing: {_frames * 1000.0 / elapsed:0.0} fps drawn of {target}, " +
                          $"{presents * 1000.0 / elapsed:0.0} VSync call(s)/s, " +
                          $"{_ticks * 1000.0 / elapsed:0.0} tick(s)/s of {FramePacing.LogicHz:0.#} Hz, " +
                          $"stages {_stageRuns} run/{_stageSkips} skipped, " +
                          $"packets/frame {Average(_packetsTicked, _framesTicked)} ticked, " +
                          $"{Average(_packetsIdle, _framesIdle)} idle");
        _windowStart = now;
        _frames = _ticks = _stageRuns = _stageSkips = 0;
        _packetsTicked = _framesTicked = _packetsIdle = _framesIdle = 0;
    }

    /// <summary>From the vblank: the watchdog's rate, when the boundary is lost.</summary>
    public static void Vblank()
    {
        if (!On) return;
        double now = _clock.Elapsed.TotalMilliseconds;
        if (_watchdogStart < 0) { _watchdogStart = now; return; }
        if (now - _watchdogStart < 1000.0) return;
        if (_fallbackTicks > 0)
            Console.WriteLine($"[ST] pacing: no boundary, {_fallbackTicks * 1000.0 / (now - _watchdogStart):0.0} tick(s)/s from the watchdog");
        _fallbackTicks = 0;
        _watchdogStart = now;
    }

    static string Average(long sum, long n) => n == 0 ? "-" : (sum / (double)n).ToString("0", CultureInfo.InvariantCulture);

    // The table is drawn from its last entry and linked by the low 24 bits of each
    // tag; the top byte is the packet's length in words.
    static long CountPackets(IMemory m)
    {
        uint ot = m.ReadU32(MainLoop.OtPointer);
        if ((ot & 0xFF000000) != 0x80000000) return 0;
        uint p = (ot + (MainLoop.OtWords - 1) * 4) & 0xFFFFFF;
        long n = 0;
        for (int guard = 0; guard < 200000 && p != 0xFFFFFF; guard++)
        {
            uint tag = m.ReadU32(0x80000000 | p);
            if ((tag >> 24) != 0) n++;
            p = tag & 0xFFFFFF;
        }
        return n;
    }
}
