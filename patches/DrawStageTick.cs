using System.Reflection;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// Hold what stage 14 advances once a frame to the world's tick, so a frame drawn
/// between ticks shows the world as the last tick left it:
///
/// - the cel animations <c>func_800255F0</c> steps off its frame counter
///   (<c>0x8015C488</c>): on an idle frame the counter and every entry's cel are put
///   back after the call;
/// - the <c>rand()</c> <c>func_80020A80</c> takes for the light's flicker: on an idle
///   frame the call returns the last tick's number without drawing a new one, so the
///   flicker keeps the console's rate and the world's random sequence is the
///   console's at any frame rate;
/// - the rumble countdown <c>func_80057B10</c> (<c>0x801CD199</c>, frames until the
///   motors stop): not run on an idle frame;
/// - the texture animations <c>func_80016A14</c> steps in VRAM (<c>MoveImage</c>,
///   <c>LoadImage</c>) off a countdown per entry: not run on an idle frame, since VRAM
///   keeps the last step. A line each time the area's count of entries changes.
///
/// Found by <see cref="RateCensus"/> and by reading stage 14. See "What runs at the
/// drawn rate" in docs/GAME_INTERNALS.md.
/// </summary>
public static class DrawStageTick
{
    static readonly ModInfo _self = new() { Id = "st.drawstagetick", Name = "Draw stage held to the tick", Version = "1.0" };

    /// <summary><c>func_800255F0</c>'s frame counter, and its cel table: 128 entries of
    /// 0x18 bytes, <c>u16 +0</c> 0xFFFF when empty, <c>+3</c> the cel count, <c>+4</c> the
    /// frames a cel is held, <c>+5</c> the cel shown.</summary>
    const uint CelRoutine = 0x800255F0;
    const uint CelCounter = 0x8015C488;
    const uint CelTable = 0x8015C48C;
    const int CelEntries = 0x80;
    const uint CelEntryBytes = 0x18;
    const uint CelShown = 5;

    /// <summary>ST.EXE's BIOS <c>rand()</c> thunk (A0 table, 0x2F), and the return address
    /// of <c>func_80020A80</c>'s call.</summary>
    const uint Rand = 0x80078AC4;
    const uint FlickerRandReturn = 0x80020B98;

    const uint RumbleCountdown = 0x80057B10;

    /// <summary><c>func_80016A14</c>'s table: 16 entries of 0x18 bytes, <c>+0</c> the kind
    /// (0 or 0xFF none), <c>+1</c> the period, <c>+2</c> the countdown, <c>s16 +4</c> the area (0xFF
    /// any), matched against the byte at <c>0x801B5929</c>.</summary>
    const uint TextureAnimations = 0x80016A14;
    const uint TextureTable = 0x8017BBD0;
    const int TextureEntries = 16;
    const uint TextureEntryBytes = 0x18;
    const uint Area = 0x801B5929;
    static int _textureCount = -1;

    static uint _celCounter;
    static readonly byte[] _cels = new byte[CelEntries];
    static bool _celsHeld;
    static uint _flickerRand;

    public static void Install() => HookAttach.OnOverlayLoad("draw stage tick", Attach);

    static bool Attach()
    {
        SymbolRegistry.Build();
        MethodInfo M(string n) => typeof(DrawStageTick).GetMethod(n, BindingFlags.Public | BindingFlags.Static)!;
        MethodInfo? Resolve(uint address) => SymbolRegistry.Resolve(MainLoop.Overlay, null, address);

        var targets = new List<MethodInfo>();
        if (Resolve(CelRoutine) is { } cel && HookManager.AddPre(_self, cel, M(nameof(BeforeCels)))
            && HookManager.AddPost(_self, cel, M(nameof(AfterCels)))) targets.Add(cel);
        if (Resolve(Rand) is { } rand && HookManager.AddPre(_self, rand, M(nameof(BeforeRand)))
            && HookManager.AddPost(_self, rand, M(nameof(AfterRand)))) targets.Add(rand);
        if (Resolve(RumbleCountdown) is { } rumble && HookManager.AddPre(_self, rumble, M(nameof(BeforeRumble)))) targets.Add(rumble);
        if (Resolve(TextureAnimations) is { } tex && HookManager.AddPre(_self, tex, M(nameof(BeforeTextures)))) targets.Add(tex);
        HookManager.Commit();

        int hooked = targets.Count(HookAttach.Installed);
        Console.WriteLine($"[ST] draw stage tick: {hooked}/4 held (cels, flicker rand, rumble countdown, texture animations)");
        return hooked == 4;
    }

    public static void BeforeCels(CpuContext c, IMemory m)
    {
        _celsHeld = FramePacing.DrawingIdleFrame;
        if (!_celsHeld) return;
        _celCounter = m.ReadU32(CelCounter);
        for (uint i = 0; i < CelEntries; i++) _cels[i] = m.ReadU8(CelTable + i * CelEntryBytes + CelShown);
    }

    public static void AfterCels(CpuContext c, IMemory m)
    {
        if (!_celsHeld) return;
        _celsHeld = false;
        m.WriteU32(CelCounter, _celCounter);
        for (uint i = 0; i < CelEntries; i++) m.WriteU8(CelTable + i * CelEntryBytes + CelShown, _cels[i]);
    }

    public static bool BeforeRand(CpuContext c, IMemory m)
    {
        if (c.RA != FlickerRandReturn || !FramePacing.DrawingIdleFrame) return true;
        c.V0 = _flickerRand;
        return false;
    }

    public static void AfterRand(CpuContext c, IMemory m)
    {
        if (c.RA == FlickerRandReturn) _flickerRand = c.V0;
    }

    public static bool BeforeRumble(CpuContext c, IMemory m) => !FramePacing.DrawingIdleFrame;

    public static bool BeforeTextures(CpuContext c, IMemory m)
    {
        int count = ActiveTextures(m);
        if (count != _textureCount)
        {
            _textureCount = count;
            Console.WriteLine($"[ST] draw stage tick: {count} texture animation(s) in area {m.ReadU8(Area)}: {Describe(m)}");
        }
        return !FramePacing.DrawingIdleFrame;
    }

    // Each live entry as kind/period/area, for the log.
    static string Describe(IMemory m) => string.Join(" ", Enumerable.Range(0, TextureEntries)
        .Select(i => TextureTable + (uint)i * TextureEntryBytes)
        .Where(e => m.ReadU8(e) is not (0 or 0xFF))
        .Select(e => $"{m.ReadU8(e):X2}/{m.ReadU8(e + 1)}/{(short)m.ReadU16(e + 4)}"));

    static int ActiveTextures(IMemory m)
    {
        int n = 0;
        short area = m.ReadU8(Area);
        for (uint i = 0; i < TextureEntries; i++)
        {
            uint e = TextureTable + i * TextureEntryBytes;
            short at = (short)m.ReadU16(e + 4);
            byte kind = m.ReadU8(e);
            if (kind is not (0 or 0xFF) && (at == area || at == 0xFF)) n++;
        }
        return n;
    }
}
