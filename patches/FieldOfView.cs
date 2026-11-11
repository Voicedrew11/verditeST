using ImGuiNET;
using RecompOne.Runtime;
using RecompOne.Runtime.Events;
using RecompOne.Runtime.Memory;

namespace St;

/// <summary>
/// The field of view, adjustable. The game writes the GTE's projection distance H
/// (control register 26) as an immediate 300 in its four display set-ups
/// (<c>func_80016FB8</c>, <c>func_800171C8</c>, <c>func_800172BC</c>, <c>func_800173A8</c>)
/// and never again, so the 320-wide screen spans 2·atan(160/300) = 56° across. Every
/// reader takes H from the GTE (libgte's divisions, <c>GpuWorld</c>), so changing the
/// register is the whole change.
///
/// At each frame boundary (<see cref="FramePacing.Boundary"/>, the game thread, between
/// one frame's drawing and the next) the chosen H is written when the register holds
/// the stock 300 or the value last written, so a set-up writing anything else is left
/// alone. No routine is hooked: a hook on the draw path shifted when vblanks land and
/// starved the agent server's calls (docs/WIDESCREEN.md, "The field of view").
///
///     ST_FOV=75     degrees across the 4:3 screen (40 to 120); unset is the kept
///                   choice, else the console's 56
///
/// Widescreen adds to the sides (Hor+): at 16:9 the console's 56° shows 71° across.
/// With frame pacing off (<c>ST_PACING=0</c>) there is no boundary and H stays stock.
/// </summary>
public static class FieldOfView
{
    public const string Key = "st.fov";
    const int GteH = 26;
    const uint StockH = 300;
    const float HalfWidth = 160f, MinDegrees = 40f, MaxDegrees = 120f;

    /// <summary>The console's: 2·atan(160/300).</summary>
    public static readonly float StockDegrees = Degrees(StockH);

    // Written on the host thread (the settings window), read on the game thread.
    static int _h = (int)StockH;
    // Game thread only.
    static uint _written = StockH;
    static float? _forced;

    /// <summary>The H the chosen angle needs.</summary>
    public static uint H => (uint)Volatile.Read(ref _h);

    public static float Chosen => Degrees(H);

    public static void Configure(string? degrees)
    {
        if (float.TryParse(degrees, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out var d))
            _forced = d;
    }

    public static void Install()
    {
        Event.AddListener<RuntimeReadyEvent>(_ =>
        {
            Set(_forced ?? Runtime.View.GetFloat(Key, StockDegrees));
            Console.WriteLine($"[ST] field of view: {Chosen:0.#}° across 4:3 (H {H}; the console's {StockDegrees:0.#}°, H {StockH})");
        });
        FramePacing.Boundary += AtBoundary;
    }

    /// <summary>Any thread: the angle across the 4:3 screen, clamped; applied from the
    /// next drawn frame.</summary>
    public static void Set(float degrees)
    {
        float d = Math.Clamp(degrees, MinDegrees, MaxDegrees);
        double h = HalfWidth / Math.Tan(d * Math.PI / 360.0);
        Volatile.Write(ref _h, (int)Math.Round(h));
    }

    /// <summary>On the settings window: the slider, kept on release.</summary>
    public static void DrawSlider()
    {
        float d = Chosen;
        ImGui.SetNextItemWidth(240);
        if (ImGui.SliderFloat("Field of view", ref d, MinDegrees, MaxDegrees, "%.0f°")) Set(d);
        if (ImGui.IsItemDeactivatedAfterEdit()) Keep();
        if (ImGui.IsItemHovered())
            ImGui.SetTooltip($"Degrees across the 4:3 picture; the console's is {StockDegrees:0}°. " +
                             "A wider aspect adds to the sides. Ctrl+click to type a value.");
        ImGui.SameLine();
        if (ImGui.SmallButton("Console")) { Set(StockDegrees); Keep(); }
        float half = HalfWidth + Widescreen.Margin;
        ImGui.TextDisabled($"{2.0 * Math.Atan(half / H) * 180.0 / Math.PI:0}° across the screen at {Widescreen.Aspect:0.##}:1");
    }

    static void Keep()
    {
        Runtime.View.SetFloat(Key, Chosen);
        Runtime.SaveView();
    }

    static float Degrees(uint h) => (float)(2.0 * Math.Atan(HalfWidth / h) * 180.0 / Math.PI);

    /// <summary>Game thread: the chosen H, over the stock one or our own last.</summary>
    static void AtBoundary(IMemory m)
    {
        uint now = (ushort)Gte.ReadControl(GteH);
        if (now != StockH && now != _written) return;
        uint want = H;
        if (want != _written) Console.WriteLine($"[ST] field of view: H {_written} -> {want}");
        _written = want;
        if (now != want) Gte.WriteControl(GteH, want);
    }
}
