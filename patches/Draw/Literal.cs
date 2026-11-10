using System.Reflection;
using RecompOne.Runtime;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// One routine of the draw path transcribed literally into C#: the same reads and
/// stores in the same order, each callee called directly so its hooks still fire.
///
///     ST_LITERAL=1|0|verify   every routine below at once (default 1)
///     ST_&lt;NAME&gt;=1|0|verify    one routine, over ST_LITERAL
///
/// Verify runs the recompiled routine, then the transcription from the same state,
/// and compares all of RAM outside a stack window under the call, the scratchpad,
/// every register with LO and HI, the GTE and the interrupt poll's countdown (so a
/// loop that polls a different number of times shows). Both runs start with the
/// countdown pushed out of reach, so no interrupt handler runs inside either (they
/// read the clock); the countdown is put back less the polls taken, and a handler due
/// runs at the next poll after the call. **The recompiled result stands.** Verdite1's Literal; Phase 5's step 2 in
/// docs/PORTING_PLAN.md, and "The draw path in C#" in docs/GEOMETRY.md.
///
/// A routine may also have a <c>twin</c>: the same drawing written plainly over memory,
/// as <see cref="MenuDraw"/> is. Verify runs it a third time from the same entry and
/// compares RAM and the scratchpad only, since it keeps no registers.
/// </summary>
public sealed partial class Literal
{
    enum Mode { Off, On, Verify }

    public static readonly List<Literal> All = [];

    public string Name { get; }
    public string Env { get; }
    public uint Address { get; }

    readonly Type _owner;
    readonly Action<CpuContext, PSMemory> _body;
    readonly Action<CpuContext, PSMemory>? _twin;
    readonly string _tip;
    Mode _mode = Mode.On;
    bool _queued, _hooked;

    /// <summary>How many times the transcription has run in place of the routine.</summary>
    public long Runs;

    /// <summary>Non-zero while a verify runs a routine twice: nothing is published then.</summary>
    public static int Verifying;

    readonly ModInfo _self;


    /// <param name="owner">The class holding a static <c>Replace(orig, c, m)</c> that forwards here.</param>
    /// <param name="twin">The plain version, read from the entry registers; checked under verify.</param>
    public Literal(Type owner, string name, string env, uint address, Action<CpuContext, PSMemory> body, string tip,
                   Action<CpuContext, PSMemory>? twin = null)
    {
        _owner = owner;
        Name = name;
        Env = env;
        Address = address;
        _body = body;
        _twin = twin;
        _tip = tip;
        _self = new ModInfo
        {
            Id = "st.literal." + env.ToLowerInvariant(),
            Name = name,
            Version = "1.0",
            Description = $"func_{address:X8} in C#.",
        };
        All.Add(this);
    }

    public string Tip => _tip;

    /// <summary>The mode as the Testing tab sets it: 0 recompiled, 1 C#, 2 verify.</summary>
    public int Setting { get => (int)_mode; set => _mode = (Mode)Math.Clamp(value, 0, 2); }

    public bool InCSharp => _mode == Mode.On;

    static Mode Parse(string? s, Mode fallback) => s?.Trim().ToLowerInvariant() switch
    {
        "0" or "off" => Mode.Off,
        "1" or "on" => Mode.On,
        "verify" => Mode.Verify,
        _ => fallback,
    };

    /// <summary>Read ST_LITERAL and each routine's own variable, and attach them all.</summary>
    public static void InstallAll()
    {
        // Touch every routine's class so its Literal registers itself.
        foreach (var t in typeof(Literal).Assembly.GetTypes())
            if (t.Namespace == "St" && t.GetField("R", BindingFlags.Public | BindingFlags.Static)?.FieldType == typeof(Literal))
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(t.TypeHandle);
        All.Sort((a, b) => a.Address.CompareTo(b.Address));

        var all = Parse(Environment.GetEnvironmentVariable("ST_LITERAL"), Mode.On);
        foreach (var r in All) r._mode = Parse(Environment.GetEnvironmentVariable("ST_" + r.Env), all);
        HookAttach.OnOverlayLoad("literal routines", Attach);
    }

    static bool Attach()
    {
        SymbolRegistry.Build();
        int n = 0;
        foreach (var r in All)
        {
            var target = SymbolRegistry.Resolve(MainLoop.Overlay, null, r.Address);
            if (target == null) continue;
            if (!r._queued)
            {
                var impl = r._owner.GetMethod("Replace", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                if (impl == null) { Console.Error.WriteLine($"[ST] literal {r.Name}: no Replace"); continue; }
                r._queued = HookManager.AddReplace(r._self, target, impl);
            }
        }
        HookManager.Commit();
        foreach (var r in All)
        {
            var target = SymbolRegistry.Resolve(MainLoop.Overlay, null, r.Address);
            r._hooked = target != null && HookAttach.Installed(target);
            if (r._hooked) n++;
        }
        var modes = All.GroupBy(r => r._mode).Select(g => $"{g.Count()} {g.Key.ToString().ToLowerInvariant()}");
        Console.WriteLine($"[ST] literal routines: {n} of {All.Count} installed ({string.Join(", ", modes)})");
        return n == All.Count;
    }

    /// <summary>The owner's replace hook forwards here.</summary>
    public void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m)
    {
        // PGXP's RAM shadow is kept by the recompiled stores, which C# stores skip.
        if (_mode == Mode.Off || RecompOne.Runtime.Pgxp.Pgxp.CpuTracking || m is not PSMemory mem)
        {
            orig(c, m);
            return;
        }
        if (_mode == Mode.Verify)
        {
            Verifying++;
            try { Verify(orig, c, mem); }
            finally { Verifying--; }
        }
        else
        {
            Runs++;
            _body(c, mem);
            if (Probe) Report();
        }
    }

    /// <summary>ST_LITERAL_PROBE=1: every routine's runs in C# over the last 5 s.</summary>
    static readonly bool Probe = Environment.GetEnvironmentVariable("ST_LITERAL_PROBE")?.Trim() == "1";
    static double _probeAt;
    static readonly Dictionary<Literal, long> _probeMark = [];

    static void Report()
    {
        double now = Environment.TickCount64 / 1000.0;
        if (now < _probeAt) return;
        _probeAt = now + 5.0;
        var parts = All.Select(r =>
        {
            long d = r.Runs - _probeMark.GetValueOrDefault(r);
            _probeMark[r] = r.Runs;
            return $"{r.Name} {d}";
        });
        var paths = _paths.Select(kv => $"{kv.Key} {kv.Value}").ToList();
        _paths.Clear();
        Console.WriteLine($"[ST] literal probe (5 s): {string.Join(", ", parts)}" +
                          (paths.Any() ? $"; paths: {string.Join(", ", paths)}" : ""));
        Console.Out.Flush();
    }

    static readonly Dictionary<string, long> _paths = [];

    /// <summary>Counts a branch of a transcription taken in C#, for ST_LITERAL_PROBE: a
    /// path that never runs has verified nothing.</summary>
    public static void Taken(string path)
    {
        if (Probe && Verifying == 0) _paths[path] = _paths.GetValueOrDefault(path) + 1;
    }
}
