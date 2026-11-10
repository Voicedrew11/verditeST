using System.Reflection;
using RecompOne.Runtime;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Events;
using RecompOne.Runtime.Hle;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// The game drawn wider than its 320-pixel screen, Verdite3's Widescreen on this disc.
/// The runtime does the work: a display buffer is drawn into a render target with a
/// margin of extra columns either side (<c>Display.WideMargin</c>), only the original
/// columns go back to VRAM, and the target is presented at <c>Display.WideAspect</c>.
/// The projection is untouched, so the margin shows only what the game submits past
/// its screen edge for the GPU to clip. This file is the switch, the latch clear, the
/// full-screen tints stretched, and the HUD at the screen edges.
///
///     ST_WIDESCREEN=16:9        aspect for the run; "1.777" and "off" also parse
///     ST_WIDESCREEN_PROBE=1     a line every 2 s: primitives reaching the margin, tints stretched
///     ST_WIDESCREEN_EFFECTS=0   leave the full-screen tints 320 wide
///     ST_WIDESCREEN_HUD=1       move the HUD's panels out to the new edges
///
/// The tints (<c>ScreenTintA</c>/<c>B</c>, the load fade) are flat, semi-transparent quads
/// spanning the clip rectangle exactly, and are widened by that shape. The HUD is
/// moved by its records: <c>func_80026964</c>'s 0x14-byte panels from <c>0x8007F3F4</c>,
/// the screen X an s16 at <c>+6</c>, the width a byte at <c>+4</c>, moved out for the call
/// and put back after it. See docs/WIDESCREEN.md.
/// </summary>
public static class Widescreen
{
    public const string AspectKey = "st.widescreen.aspect";
    public const string HudKey = "st.widescreen.hud";
    public const float FourThree = 4f / 3f;
    public const float Widest = 3f;

    /// <summary>What a player who never chose gets: 16:9, the siblings' judged value
    /// (Verdite3, the user's choice of 2026-10-06), as the plan ships enhancements.</summary>
    public const float DefaultAspect = 16f / 9f;

    public static float Aspect { get; private set; } = FourThree;
    public static bool StretchEffects { get; private set; } = true;
    public static bool AnchorHud { get; private set; }
    public static bool On => Display.WideAspect > 0f;
    public static int Margin => Display.WideMargin(320);

    static float? _forced;
    static bool? _forcedEffects, _forcedHud;
    static bool _measure;

    // A tint lands exactly on 0 and 320; this is only insurance.
    const int EdgeSlack = 2;
    static long _stretched, _inside, _margin;
    static double _windowStart;
    static double Now => Environment.TickCount64 / 1000.0;

    static readonly ModInfo _self = new() { Id = "st.widescreen", Name = "Widescreen", Version = "1.0" };

    public static void Configure(string? aspect, string? probe, string? effects, string? hud)
    {
        if (Parse(aspect) is { } ratio) _forced = ratio;
        _measure = probe?.Trim() is { Length: > 0 } p && p != "0";
        if (!string.IsNullOrWhiteSpace(effects)) _forcedEffects = effects.Trim() != "0";
        if (!string.IsNullOrWhiteSpace(hud)) _forcedHud = hud.Trim() != "0";
    }

    /// <summary>The saved choice is read on <see cref="RuntimeReadyEvent"/>: the config
    /// loads inside the host window's start, after Program.cs.</summary>
    public static void Install()
    {
        _windowStart = Now;
        if (_forced is { } forced) { Aspect = forced; Apply(); }

        Event.AddListener<RuntimeReadyEvent>(_ =>
        {
            Aspect = _forced ?? Runtime.View.GetFloat(AspectKey, DefaultAspect);
            StretchEffects = _forcedEffects ?? true;
            AnchorHud = _forcedHud ?? Runtime.View.GetInt(HudKey, 0) != 0;
            Apply();
            Console.WriteLine(On
                ? $"[ST] widescreen: {Aspect:0.###}:1, margin {Margin} px a side, HUD " +
                  $"{(AnchorHud ? "at the edges" : "in its 4:3 box")}, tints {(StretchEffects ? "stretched" : "320 wide")}"
                : "[ST] widescreen: off (4:3)");
        });

        HookAttach.OnOverlayLoad("widescreen HUD", AttachHud);

        // The margin latch lasts for an executable's session. The area modules
        // (fdat...) are ST.EXE still running, and clearing on them would drop every
        // area load to the 4:3 fallback.
        Event.AddListener<OverlayLoadedEvent>(e =>
        {
            if (e.Name is "str" or "title" or "optxt" or "st" or "end" or "vs")
                (GpuHle.Backend as GlCore)?.ClearMarginLatches();
        });
    }

    public static void SetAspect(float aspect)
    {
        Aspect = Math.Clamp(aspect, FourThree, Widest);
        Apply();
    }

    public static void SetAnchorHud(bool on) => AnchorHud = on;

    static void Apply()
    {
        Display.WideAspect = Aspect > FourThree + 0.001f ? Aspect : 0f;
        Event.RemoveListener<RenderPrimEvent>(OnPrim);
        if (_measure || (On && StretchEffects)) Event.AddListener<RenderPrimEvent>(OnPrim);
    }

    static void OnPrim(RenderPrimEvent e)
    {
        if (_measure) Census(e);
        if (StretchEffects && On) Stretch(e);
    }

    // The clip rect and the vertices are in the same VRAM space, so reaching the
    // margin is a vertex outside the clip.
    static void Census(RenderPrimEvent e)
    {
        bool outside = false;
        for (int i = 0; i < e.Count && !outside; i++) outside = e.X[i] < e.DrawLeft || e.X[i] > e.DrawRight;
        if (outside) _margin++; else _inside++;

        double window = Now - _windowStart;
        if (window < 2.0) return;
        long total = _inside + _margin;
        Console.WriteLine(total == 0
            ? "[ST] widescreen: no primitives in the last window"
            : $"[ST] widescreen: {_margin * 100.0 / total:F1}% of {total} prims reach the margin ({total / window:F0}/s)" +
              (_stretched > 0 ? $"; {_stretched} full-screen tint(s) stretched" : "") +
              $"; {NearScreen.Faces} near face(s), {NearScreen.Widened} widened");
        _inside = _margin = _stretched = 0;
        NearScreen.Faces = NearScreen.Widened = 0;
        _windowStart = Now;
    }

    // A full-screen tint: semi-transparent, flat, from the clip's left edge to its
    // right. World geometry is Gouraud-shaded; a 2D picture is opaque and stays.
    static void Stretch(RenderPrimEvent e)
    {
        if (!e.SemiTransparent || e.Gouraud) return;
        int margin = Display.WideMargin(e.DrawRight - e.DrawLeft + 1);
        if (margin <= 0) return;
        int left = e.DrawLeft, right = e.DrawRight + 1;

        int lo = int.MaxValue, hi = int.MinValue;
        for (int i = 0; i < e.Count; i++) { lo = Math.Min(lo, e.X[i]); hi = Math.Max(hi, e.X[i]); }
        if (lo > left + EdgeSlack || hi < right - EdgeSlack) return;

        for (int i = 0; i < e.Count; i++)
        {
            if (e.X[i] <= left + EdgeSlack) e.X[i] = left - margin;
            else if (e.X[i] >= right - EdgeSlack) e.X[i] = right + margin;
        }
        // The backend's latch must not take this width for margin the game drew.
        GpuHle.PortWidenedPrim = true;
        _stretched++;
    }

    // ---- the HUD, moved by its records ---------------------------------------

    const uint HudPanels = 0x80026964;
    const uint PanelTable = 0x8007F3F4;
    const uint PanelBytes = 0x14, PanelX = 6, PanelW = 4;
    const int MaxPanels = 64;
    const int ScreenCentre = 160;

    static readonly short[] _savedX = new short[MaxPanels];
    static uint _first;
    static int _moved;
    static bool _hudQueued;

    static bool AttachHud()
    {
        SymbolRegistry.Build();
        if (SymbolRegistry.Resolve(MainLoop.Overlay, null, HudPanels) is not { } t) return false;
        if (!_hudQueued)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;
            HookManager.AddPre(_self, t, typeof(Widescreen).GetMethod(nameof(BeforePanels), flags)!);
            HookManager.AddPost(_self, t, typeof(Widescreen).GetMethod(nameof(AfterPanels), flags)!);
            _hudQueued = true;
        }
        HookManager.Commit();
        return HookAttach.Installed(t);
    }

    /// <summary>Every panel the call will walk (from a0 to the 0xFF) moved to its edge:
    /// left of the screen's middle by the margin to the left, right of it to the right.</summary>
    public static void BeforePanels(CpuContext c, IMemory m)
    {
        _moved = 0;
        if (!(AnchorHud && On)) return;
        int margin = Margin;
        _first = PanelTable + (c.A0 & 0xFF) * PanelBytes;
        for (int i = 0; i < MaxPanels; i++)
        {
            uint rec = _first + (uint)i * PanelBytes;
            if (m.ReadU8(rec) == 0xFF) break;
            short x = (short)m.ReadU16(rec + PanelX);
            int middle = x + m.ReadU8(rec + PanelW) / 2;
            _savedX[i] = x;
            m.WriteU16(rec + PanelX, (ushort)(x + (middle < ScreenCentre ? -margin : margin)));
            _moved = i + 1;
        }
    }

    public static void AfterPanels(CpuContext c, IMemory m)
    {
        for (int i = 0; i < _moved; i++) m.WriteU16(_first + (uint)i * PanelBytes + PanelX, (ushort)_savedX[i]);
        _moved = 0;
    }

    /// <summary>"16:9", "1.777" or "off" (4:3); null for anything else.</summary>
    static float? Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        value = value.Trim();
        if (value.Equals("off", StringComparison.OrdinalIgnoreCase)) return FourThree;
        int colon = value.IndexOf(':');
        if (colon > 0)
            return float.TryParse(value[..colon], out float w) && float.TryParse(value[(colon + 1)..], out float h) && h > 0f
                ? w / h : null;
        return float.TryParse(value, out float ratio) && ratio > 0f ? ratio : null;
    }
}
