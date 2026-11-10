using ImGuiNET;
using RecompOne.Runtime.Events;
using RecompOne.Runtime.Host.Window;
using Verdite.Core;
using Rt = RecompOne.Runtime.Runtime;

namespace St;

/// <summary>
/// A settings tab of the port's own, <b>Testing</b>: the port's switches, live, so a
/// change can be compared without a restart. Each control writes the same state its
/// <c>ST_*</c> variable sets at boot.
///
/// The on/off choices, the frame rate and the shading are kept in interface.ini
/// (<c>st.*</c>) and put back at the next boot, <b>unless the variable is set</b>, which
/// wins. Frame pacing itself is not here: turning it off is a different program path
/// (its hooks are not installed), so it stays <c>ST_PACING=0</c>. Verdite3's
/// TestingSection with this game's switches. See "The Testing tab" in
/// docs/DEVELOPMENT.md.
/// </summary>
public sealed class TestingSection : ISettingsSection
{
    public string Id => "sttesting";
    public string TitleKey => "settings.sttesting";

    // After Audio (10), before Paths (20).
    public int Order => 15;

    const string Names = """
    {
      "strings": {
        "settings.sttesting": { "en": "Testing", "pt-BR": "Testes", "es-419": "Pruebas" }
      }
    }
    """;

    /// <summary>A kept switch: its key, its variable, how to read it and how to set it.</summary>
    sealed record Kept(string Key, string Env, Func<bool> Get, Action<bool> Set);

    static readonly Kept[] Switches =
    [
        new("st.vblank_hold", "ST_VBLANKPACING", () => VBlankPacing.Enabled, v => VBlankPacing.Enabled = v),
        new("st.smooth", "ST_SMOOTH", () => ViewSmoothing.Enabled, v => ViewSmoothing.Enabled = v),
        new("st.smooth_models", "ST_SMOOTH_MODELS", () => ModelSmoothing.Enabled, v => ModelSmoothing.Enabled = v),
        new("st.perspective", "ST_PERSPECTIVE", () => Perspective.Enabled, v => Perspective.Enabled = v),
        new("st.subpixel", "ST_SUBPIXEL", () => Subpixel.Enabled, v => Subpixel.Enabled = v),
        new("st.zbuffer", "ST_ZBUFFER", () => ZBuffer.Enabled, v => ZBuffer.Enabled = v),
        new("st.nearscreen", "ST_NEARSCREEN", () => NearScreen.Enabled, v => NearScreen.Enabled = v),
        new("st.menuworld", "ST_MENUWORLD", () => MenuWorld.Enabled, v => MenuWorld.Enabled = v),
        new(Widescreen.HudKey, "ST_WIDESCREEN_HUD", () => Widescreen.AnchorHud, Widescreen.SetAnchorHud),
        new(Mouse.OnKey, "ST_MOUSE", () => Mouse.Enabled, v => Mouse.Enabled = v),
        new(Mouse.LeadKey, "ST_MOUSE_LEAD", () => Mouse.Lead, v => Mouse.Lead = v),
    ];

    const string FpsKey = "st.fps", ShadingKey = "st.shading";

    static bool _installed;

    public static void Install()
    {
        if (_installed) return;
        _installed = true;
        Event.AddListener<RuntimeReadyEvent>(_ => Ready());
    }

    /// <summary>The config loads inside the host window's start, after Program.cs, so
    /// kept values go back here; a set variable is left as it chose.</summary>
    static void Ready()
    {
        Localization.Merge(Names);
        SettingsRegistry.Register(new TestingSection());

        int restored = 0;
        if (Unset("ST_FPS") && Rt.View.GetInt(FpsKey, -1) is >= 0 and var fps) { FramePacing.SetTarget(fps); restored++; }
        foreach (var k in Switches)
            if (Unset(k.Env) && Rt.View.GetInt(k.Key, -1) is >= 0 and var v) { k.Set(v != 0); restored++; }
        if (Unset("ST_TRUECOLOR") && Unset("ST_NODITHER") && Rt.View.GetInt(ShadingKey, -1) is >= 0 and var sh)
        {
            SetShading(sh);
            restored++;
        }
        Console.WriteLine($"[ST] testing tab: registered, {restored} kept value(s) restored");
    }

    static bool Unset(string env) => string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(env));

    static void Keep(string key, int value)
    {
        Rt.View.SetInt(key, value);
        Rt.SaveView();
    }

    static Kept K(string key) => Switches.First(s => s.Key == key);

    /// <summary>For setup: one kept key applied live from its text (null is the
    /// default, which the caller resolves first). False for a key this tab does not own.</summary>
    internal static bool Apply(string key, string? text)
    {
        if (Switches.FirstOrDefault(s => s.Key.Equals(key, StringComparison.OrdinalIgnoreCase)) is { } k)
        {
            k.Set(Number(text) != 0);
            return true;
        }
        if (key.Equals(FpsKey, StringComparison.OrdinalIgnoreCase)) { FramePacing.SetTarget(Number(text)); return true; }
        if (key.Equals(ShadingKey, StringComparison.OrdinalIgnoreCase)) { SetShading((int)Number(text)); return true; }
        if (key.Equals(Widescreen.AspectKey, StringComparison.OrdinalIgnoreCase)) { Widescreen.SetAspect((float)Number(text)); return true; }
        return false;
    }

    /// <summary>For setup's boot check: what a kept key reads now, as setup writes it.</summary>
    internal static string? Live(string key)
    {
        if (Switches.FirstOrDefault(s => s.Key.Equals(key, StringComparison.OrdinalIgnoreCase)) is { } k)
            return k.Get() ? "1" : "0";
        if (key.Equals(FpsKey, StringComparison.OrdinalIgnoreCase))
            return FramePacing.TargetFps.ToString(System.Globalization.CultureInfo.InvariantCulture);
        if (key.Equals(ShadingKey, StringComparison.OrdinalIgnoreCase)) return Shading.ToString();
        if (key.Equals(Widescreen.AspectKey, StringComparison.OrdinalIgnoreCase))
            return Widescreen.Aspect.ToString(System.Globalization.CultureInfo.InvariantCulture);
        return null;
    }

    /// <summary>For setup's boot check: whether a variable set at boot decides this key.</summary>
    internal static bool Overridden(string key)
    {
        if (Switches.FirstOrDefault(s => s.Key.Equals(key, StringComparison.OrdinalIgnoreCase)) is { } k) return !Unset(k.Env);
        if (key.Equals(FpsKey, StringComparison.OrdinalIgnoreCase)) return !Unset("ST_FPS");
        if (key.Equals(ShadingKey, StringComparison.OrdinalIgnoreCase)) return !Unset("ST_TRUECOLOR") || !Unset("ST_NODITHER");
        return key.Equals(Widescreen.AspectKey, StringComparison.OrdinalIgnoreCase) && !Unset("ST_WIDESCREEN");
    }

    static double Number(string? text) =>
        text is null ? 0
        : bool.TryParse(text, out var b) ? (b ? 1 : 0)
        : double.TryParse(text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var v) ? v
        : 0;

    static void Toggle(string label, Kept k, string tip)
    {
        bool v = k.Get();
        if (ImGui.Checkbox(label, ref v))
        {
            k.Set(v);
            Keep(k.Key, k.Get() ? 1 : 0);
        }
        if (ImGui.IsItemHovered()) ImGui.SetTooltip(tip);
    }

    // Dither (the console), None (15-bit, no crosshatch), Smooth (24-bit, no crosshatch).
    internal static int Shading => TrueColor.Enabled ? 2 : NoDither.Enabled ? 1 : 0;

    internal static void SetShading(int v)
    {
        NoDither.Enabled = v != 0;
        TrueColor.Enabled = v == 2;
    }

    static readonly (string Name, float Ratio)[] Aspects =
        [("4:3 (the console)", Widescreen.FourThree), ("16:10", 16f / 10f), ("16:9", 16f / 9f), ("21:9", 64f / 27f)];

    // The readout, measured from the frame and tick counters every half second.
    static long _frames0, _ticks0;
    static double _at0 = -1.0, _fps, _tps;

    static void Rates()
    {
        double now = Environment.TickCount64 / 1000.0;
        if (_at0 < 0.0 || now - _at0 > 2.0) { _at0 = now; _frames0 = FramePacing.Frames; _ticks0 = FramePacing.Ticks; return; }
        if (now - _at0 < 0.5) return;
        _fps = (FramePacing.Frames - _frames0) / (now - _at0);
        _tps = (FramePacing.Ticks - _ticks0) / (now - _at0);
        _at0 = now; _frames0 = FramePacing.Frames; _ticks0 = FramePacing.Ticks;
    }

    static void Note(string text)
    {
        ImGui.PushStyleColor(ImGuiCol.Text, ImGui.GetStyle().Colors[(int)ImGuiCol.TextDisabled]);
        ImGui.TextWrapped(text);
        ImGui.PopStyleColor();
    }

    public void Draw()
    {
        DrawPacing();
        DrawPicture();
        ImGui.SeparatorText("Mouse");
        Toggle("Mouse look", K(Mouse.OnKey),
            "Turns and looks with the mouse once a click captures it; more options under Input.");
        Toggle("Instant mouse look", K(Mouse.LeadKey),
            "Turns the view the frame you move the mouse, instead of on the game's next tick.");
        DrawProbes();
        ImGui.Separator();
        if (ImGui.Button("Run setup again")) Setup.Open(inGame: true);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("The first-run setup's pages, applied live.");
    }

    static void DrawPacing()
    {
        ImGui.SeparatorText("Frame pacing");
        if (!FramePacing.Enabled)
        {
            Note("Frame pacing is off (ST_PACING=0): the game draws at its own 20 frames a second.");
            return;
        }
        bool uncapped = FramePacing.Uncapped;
        int fps = uncapped ? (int)FramePacing.DefaultFps : (int)FramePacing.TargetFps;
        ImGui.BeginDisabled(uncapped);
        ImGui.SetNextItemWidth(240);
        if (ImGui.SliderInt("Frame rate", ref fps, 30, 360))
        {
            FramePacing.SetTarget(fps);
            Keep(FpsKey, fps);
        }
        ImGui.EndDisabled();
        ImGui.SameLine();
        if (ImGui.Checkbox("Uncapped", ref uncapped))
        {
            FramePacing.SetTarget(uncapped ? 0 : fps);
            Keep(FpsKey, uncapped ? 0 : fps);
        }
        Rates();
        Note($"Drawing {_fps:0.0} fps; the world at {_tps:0.0} ticks a second (the game's own is 20).");
        Toggle("Hold menus and loading screens to the vblank", K("st.vblank_hold"),
            "Waits a real vblank for every VSync call outside the world's drawing, as the console did.");

        ImGui.SeparatorText("Smoothing");
        Toggle("Camera", K("st.smooth"), "Draws the view between the world's ticks.");
        Toggle("Creatures and objects", K("st.smooth_models"),
            "Draws creatures and objects, their positions, angles and poses, between the world's ticks.");
    }

    static void DrawPicture()
    {
        ImGui.SeparatorText("Picture");
        Note("Not judged by eye yet (docs/PICTURE.md, docs/WIDESCREEN.md).");
        string[] shading = ["Dither (the console)", "None", "Smooth (24-bit)"];
        int sh = Shading;
        ImGui.SetNextItemWidth(200);
        if (ImGui.Combo("Shading", ref sh, shading, shading.Length))
        {
            SetShading(sh);
            Keep(ShadingKey, sh);
        }
        if (ImGui.IsItemHovered())
            ImGui.SetTooltip("None drops the crosshatch and keeps 15-bit bands; Smooth keeps 8 bits a channel instead.");

        // The aspect is kept as a float under Widescreen's own key, which it reads at
        // boot; a value none of the presets is (from ST_WIDESCREEN) keeps its own entry.
        int index = Array.FindIndex(Aspects, a => Math.Abs(Widescreen.Aspect - a.Ratio) < 0.001f);
        string[] items = index >= 0
            ? [.. Aspects.Select(a => a.Name)]
            : [.. Aspects.Select(a => a.Name), $"Custom ({Widescreen.Aspect:0.###}:1)"];
        if (index < 0) index = items.Length - 1;
        ImGui.SetNextItemWidth(200);
        if (ImGui.Combo("Aspect", ref index, items, items.Length) && index < Aspects.Length)
        {
            Widescreen.SetAspect(Aspects[index].Ratio);
            Rt.View.SetFloat(Widescreen.AspectKey, Widescreen.Aspect);
            Rt.SaveView();
        }
        if (ImGui.IsItemHovered()) ImGui.SetTooltip("Wider shows more of the room, not a stretched 4:3.");
        Toggle("HUD at the screen's edges", K(Widescreen.HudKey),
            "Moves the gauges and panels out to the widened edges instead of the 4:3 box.");
        Toggle("The world live behind the menu", K("st.menuworld"),
            "Draws the world behind the menu to the screen's edges, instead of the game's 320-wide still of it.");
        Toggle("Near polygons into the margin", K("st.nearscreen"),
            "Keeps the pieces of large near polygons that fall in the widened margin.");
        Toggle("Perspective-correct textures", K("st.perspective"),
            "Textures follow each corner's depth instead of warping across a polygon.");
        Toggle("Sub-pixel vertices", K("st.subpixel"),
            "Corners keep the fraction of a pixel the GTE dropped, so edges stop jittering as the view moves.");
        Toggle("Z-buffer", K("st.zbuffer"),
            "Per-pixel occlusion from the map's depths, instead of the painter's order.");
    }

    static void DrawProbes()
    {
        ImGui.SeparatorText("Console probes");
        bool d = NoDither.ProbeOn;
        if (ImGui.Checkbox("Dither (ST_NODITHER_PROBE)", ref d)) NoDither.ProbeOn = d;
        bool pp = Perspective.ProbeOn;
        if (ImGui.Checkbox("Address map (ST_PERSPECTIVE_PROBE)", ref pp)) Perspective.ProbeOn = pp;
        bool sp = Subpixel.ProbeOn;
        if (ImGui.Checkbox("Sub-pixel (ST_SUBPIXEL_PROBE)", ref sp)) Subpixel.ProbeOn = sp;
    }
}
