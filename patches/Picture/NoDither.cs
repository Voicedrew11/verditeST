using System.Reflection;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// Turns off the GPU's ordered dither, Verdite1's NoDither, on this disc's libgpu.
/// The dither state is bit 9 of a GP0(E1) word, and it reaches the GPU two
/// ways: PutDrawEnv's <c>dtd</c> byte at <c>DRAWENV+0x16</c>, and E1 words linked into
/// the ordering table. Both end in the GPU's draw-mode decode, which the runtime's
/// <c>Gpu.SuppressDither</c> (0094) masks, so the game's memory is never touched; the
/// table walk runs only for the probe. On by default.
///
///     ST_NODITHER=1         no dither; 0 or unset keeps the crosshatch
///     ST_NODITHER_PROBE=1   once every 2 s: draw envs and table words that asked
///                            for dither, and GPUSTAT bit 9 after each frame
///
/// See docs/PICTURE.md.
/// </summary>
public static class NoDither
{
    // libgpu PutDrawEnv and DrawOTag, as bound in config/st.json.
    static readonly (string Overlay, uint Addr)[] PutDrawEnv =
        [("str", 0x8001AB7C), ("st", 0x8007A0C4), ("title", 0x8002166C), ("optxt", 0x80018CB0), ("vs", 0x800479F4), ("end", 0x8001AB7C)];

    static readonly (string Overlay, uint Addr)[] DrawOTag =
        [("str", 0x8001AB0C), ("st", 0x8007A054), ("title", 0x800215FC), ("optxt", 0x80018C40), ("vs", 0x80047984), ("end", 0x8001AB0C)];

    public static bool Enabled
    {
        get => RecompOne.Runtime.Gpu.SuppressDither;
        set => RecompOne.Runtime.Gpu.SuppressDither = value;
    }

    public static bool ProbeOn { get; set; }

    static readonly ModInfo _self = new()
    {
        Id = "st.nodither",
        Name = "No dithering",
        Version = "1.0",
        Description = "Clears the GPU's dither bit.",
    };

    // The probe's window: draw envs and table words that asked for dither, the
    // E1 words seen in the table at all, and GPUSTAT bit 9 or'd over the frames.
    static long _envHits, _otHits, _otE1, _frames;
    static uint _stat;
    static double _windowStart = Now;

    static double Now => Environment.TickCount64 / 1000.0;

    public static void Configure(string? on, string? probe)
    {
        Enabled = string.IsNullOrWhiteSpace(on) || on.Trim() is not ("0" or "off");
        ProbeOn = probe?.Trim() == "1";
    }

    public static void Install() => HookAttach.OnOverlayLoad("dither", Attach);

    static bool Attach()
    {
        SymbolRegistry.Build();
        var self = typeof(NoDither);
        MethodInfo M(string n) => self.GetMethod(n, BindingFlags.Public | BindingFlags.Static)!;

        var targets = new List<MethodInfo>();
        void Hook((string Overlay, uint Addr)[] sites, string pre, string? post)
        {
            foreach (var (overlay, addr) in sites)
            {
                var t = SymbolRegistry.Resolve(overlay, null, addr);
                if (t == null) { Console.Error.WriteLine($"[ST] dither: no function at {overlay}/0x{addr:X8}"); continue; }
                if (HookManager.AddPre(_self, t, M(pre)) && (post == null || HookManager.AddPost(_self, t, M(post)))) targets.Add(t);
            }
        }
        Hook(PutDrawEnv, nameof(BeforePutDrawEnv), null);
        Hook(DrawOTag, nameof(BeforeDrawOTag), nameof(AfterDrawOTag));
        HookManager.Commit();

        int n = targets.Count(HookAttach.Installed);
        Console.WriteLine($"[ST] dither: {(Enabled ? "off" : "on (pass-through)")}, {n}/{PutDrawEnv.Length + DrawOTag.Length} function(s) hooked");
        return n == PutDrawEnv.Length + DrawOTag.Length;
    }

    // The probe's count of draw envs that asked for dither.
    public static void BeforePutDrawEnv(CpuContext c, IMemory m)
    {
        if (ProbeOn && m.ReadU8(c.A0 + 0x16) != 0) _envHits++;
    }

    // The probe's count of table E1 words that asked for dither: DrawOTag's own
    // walk, each header's `next` to the end marker, a packet's words counted by
    // its top byte.
    public static void BeforeDrawOTag(CpuContext c, IMemory m)
    {
        if (ProbeOn)
        {
            uint mask = RecompOne.Runtime.Runtime.RamWordMask;
            uint addr = c.A0 & mask;
            for (int guard = 0; guard < 0x100000; guard++)
            {
                uint header = m.ReadU32(addr);
                Scan(m, addr + 4u, header >> 24);
                uint next = header & 0xFFFFFFu;
                if (next == 0xFFFFFFu || (next & 0x800000u) != 0) break;
                addr = next & mask;
            }
        }
        Report();
    }

    public static void AfterDrawOTag(CpuContext c, IMemory m)
    {
        if (ProbeOn && RecompOne.Runtime.Runtime.Gpu is { } gpu)
            _stat |= (gpu.ReadStat() >> 9) & 1u;
    }

    // One packet's words stepped as GP0 commands, so a vertex with 0xE1 in its
    // top byte is never taken for a draw-mode word.
    static void Scan(IMemory m, uint words, uint count)
    {
        for (uint i = 0; i < count;)
        {
            uint at = words + i * 4u;
            uint word = m.ReadU32(at);
            if (word >> 24 == 0xE1)
            {
                _otE1++;
                if ((word & 0x200u) != 0) _otHits++;
                i++;
                continue;
            }
            int len = Length(word);
            if (len <= 0) return;
            i += (uint)len;
        }
    }

    // GP0 command lengths; negative where the length depends on what follows.
    static int Length(uint word)
    {
        switch (word >> 24)
        {
            case 0x02: return 3;
            case >= 0x20 and <= 0x3F:
            {
                int n = (word & (1u << 27)) != 0 ? 4 : 3;
                bool shaded = (word & (1u << 28)) != 0, tex = (word & (1u << 26)) != 0;
                return 1 + n + (shaded ? n - 1 : 0) + (tex ? n : 0);
            }
            case >= 0x40 and <= 0x5F:
                if ((word & (1u << 27)) != 0) return -1;
                return 3 + ((word & (1u << 28)) != 0 ? 1 : 0);
            case >= 0x60 and <= 0x7F:
            {
                int sz = (int)((word >> 27) & 3);
                bool tex = (word & (1u << 26)) != 0;
                return 2 + (tex ? 1 : 0) + (sz == 0 ? 1 : 0);
            }
            case >= 0x80 and <= 0x9F: return 4;
            case >= 0xA0 and <= 0xBF: return -1;
            case >= 0xC0 and <= 0xDF: return 3;
            default: return 1;
        }
    }

    static void Report()
    {
        _frames++;
        double window = Now - _windowStart;
        if (window < 2.0) return;
        if (ProbeOn)
            Console.WriteLine($"[ST] dither: {(Enabled ? "off" : "on")}; {_envHits / window:F0} draw envs/s and " +
                              $"{_otHits / window:F0} of {_otE1 / window:F0} table E1 words/s asked for dither, " +
                              $"over {_frames / window:F0} frames/s; GPUSTAT dither bit {_stat}");
        _envHits = _otHits = _otE1 = _frames = 0;
        _stat = 0;
        _windowStart = Now;
    }
}
