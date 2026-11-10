using RecompOne.Runtime.Events;
using RecompOne.Runtime.Host.Window;
using Verdite.Core;

namespace St;

/// <summary>
/// The first-run setup (Verdite Core's <see cref="Onboarding"/>), from
/// <c>config/onboarding.json</c>. Shadow Tower has no launcher yet, so the game shows
/// it itself, over the title, the first time it starts (the flag
/// <c>verdite.setup</c> in interface.ini is below the spec's version); "Run setup
/// again" on Settings ▸ Testing opens it later. Every key it writes is one the
/// Testing tab owns, applied live through the tab's own setters.
///
///     ST_SETUP=1    show it even when it is not due
///     ST_SETUP=0    never show it on its own
///
/// At boot it checks the spec against the code: each key it writes has a live setter
/// and, unsaved, reads what the spec's <c>defaults</c> say, or Modern (which writes
/// nothing) would not be the defaults. See "First-run setup" in docs/DEVELOPMENT.md.
/// </summary>
public static class Setup
{
    static OnboardingSpec? _spec;
    static OnboardingPopup? _popup;
    static string? _force;

    public static void Install()
    {
        _force = Game.Env("SETUP")?.Trim();
        Onboarding.LiveApply = TestingSection.Apply;
        _spec = Onboarding.Load("ST", Onboarding.Candidates("onboarding.json"));
        // After TestingSection's listener, which is where the kept values come back.
        if (_spec is { } spec) Event.AddListener<RuntimeReadyEvent>(_ => Ready(spec));
    }

    static void Ready(OnboardingSpec spec)
    {
        Check(spec);
        bool due = Onboarding.Due(spec);
        bool show = _force == "1" || (_force != "0" && due);
        Console.WriteLine($"[ST] setup: {(due ? "due" : "done")} (flag {RecompOne.Runtime.Runtime.View.GetInt(Onboarding.FlagKey, 0)}, " +
                          $"spec {spec.Version}){(show ? ", shown" : "")}");
        if (show) Open(inGame: false);
    }

    /// <summary>On the host thread (the settings window, the boot): open the setup.
    /// The first run's welcome offers Skip; the game's offers Cancel.</summary>
    public static void Open(bool inGame)
    {
        if (_spec is null) return;
        if (_popup is null || _popup.IsOpen == false)
        {
            _popup = new OnboardingPopup(_spec, "ST", inGame);
            PopupManager.Register(_popup);
        }
        _popup.Open();
    }

    static void Check(OnboardingSpec spec)
    {
        var keys = spec.Display.SelectMany(f => f.Options).SelectMany(o => o.Set.Keys)
            .Concat(spec.Categories.SelectMany(c => c.Groups).SelectMany(g => g.Tiers).SelectMany(t => t.Set.Keys))
            .Where(k => !Onboarding.IsWindowKey(k))
            .Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var view = RecompOne.Runtime.Runtime.View;
        int problems = 0;
        void Problem(string line) { problems++; Console.WriteLine($"[ST] setup check: {line}"); }

        foreach (var key in keys)
        {
            if (TestingSection.Live(key) is not { } live)
            {
                Problem($"{key} has no live setter; setup would leave it for the next launch");
                continue;
            }
            if (!spec.Defaults.TryGetValue(key, out var declared))
            {
                Problem($"{key} has no entry in defaults");
                continue;
            }
            if (view.Values.ContainsKey(key) || TestingSection.Overridden(key)) continue;
            if (!Onboarding.SameText(live, declared) && !CloseEnough(live, declared))
                Problem($"{key} reads {live} with nothing saved, but defaults says {declared}");
        }
        Console.WriteLine($"[ST] setup check: {keys.Count} key(s), {problems} problem(s)");
    }

    /// <summary>A float kept as text ("1.7777778") against the live single.</summary>
    static bool CloseEnough(string a, string b) =>
        double.TryParse(a, System.Globalization.CultureInfo.InvariantCulture, out var x)
        && double.TryParse(b, System.Globalization.CultureInfo.InvariantCulture, out var y)
        && Math.Abs(x - y) < 1e-4;
}
