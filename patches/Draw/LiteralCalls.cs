using System.Reflection;
using System.Reflection.Emit;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Dispatch;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// Verify's call order (Phase 11, Step 2): the calls a literal routine makes, recorded
/// on both runs and compared, as Verdite3's supplemental verifier does. A direct call
/// is recorded at its callee's entry by its return address and the four argument
/// registers; the return address names the call site, so it names the callee too.
/// Indirect calls (<c>jalr</c>, through <see cref="Dispatcher.Call"/>) are compared by
/// their count and the last 64 targets, the dispatcher's own ring.
///
/// The callees hooked are those the recompiled routines call directly, read from their
/// IL, so the list cannot go stale. Installed only when a routine starts in verify,
/// since every hook costs a call on a hot path. A verify nested inside another records
/// its own calls; the outer one sees only the call to it.
/// </summary>
static class LiteralCalls
{
    public readonly record struct Call(uint Ra, uint A0, uint A1, uint A2, uint A3);

    const int Ring = 64;
    static readonly ModInfo _self = new() { Id = "st.literal.calls", Name = "Literal call order", Version = "1.0" };
    static readonly FieldInfo? RingAt =
        typeof(Dispatcher).GetField("_recentAt", BindingFlags.NonPublic | BindingFlags.Static);
    static List<Call>? _sink;

    public static bool Installed { get; private set; }

    /// <summary>Hook every function the given recompiled routines call directly.</summary>
    public static int Attach(IEnumerable<MethodInfo> routines)
    {
        var record = typeof(LiteralCalls).GetMethod(nameof(Record), BindingFlags.Public | BindingFlags.Static)!;
        var callees = routines.SelectMany(Callees).Distinct().ToList();
        foreach (var t in callees) HookManager.AddPre(_self, t, record, order: -1000);
        HookManager.Commit();
        Installed = true;
        return callees.Count(HookAttach.Installed);
    }

    // A scan of the method's IL for call instructions to the same generated class. The
    // scan does not decode operands, so a stray 0x28 byte may resolve to some other
    // function: one more hook, recorded the same on both runs, so harmless.
    static IEnumerable<MethodInfo> Callees(MethodInfo routine)
    {
        var il = routine.GetMethodBody()?.GetILAsByteArray() ?? [];
        var module = routine.Module;
        var found = new HashSet<MethodInfo>();
        for (int i = 0; i + 4 < il.Length; i++)
        {
            if (il[i] != OpCodes.Call.Value) continue;
            int token = BitConverter.ToInt32(il, i + 1);
            MethodBase? m;
            try { m = module.ResolveMethod(token); }
            catch (ArgumentException) { continue; }
            if (m is MethodInfo mi && mi.DeclaringType == routine.DeclaringType && mi.Name.StartsWith("func_")
                && mi != routine)
                found.Add(mi);
        }
        return found;
    }

    public static void Record(CpuContext c, IMemory m) =>
        _sink?.Add(new Call(c.RA, c.A0, c.A1, c.A2, c.A3));

    /// <summary>One run's calls: begun before the run, ended after it.</summary>
    public sealed class Trace
    {
        public readonly List<Call> Direct = [];
        public int Indirect;
        public uint[] Tail = [];
        List<Call>? _outer;
        int _ringStart;

        public void Begin()
        {
            Direct.Clear();
            _outer = _sink;
            _sink = Direct;
            _ringStart = RingCount();
        }

        public void End()
        {
            _sink = _outer;
            Indirect = RingCount() - _ringStart;
            var recent = Dispatcher.Recent();
            Tail = recent[Math.Max(0, recent.Length - Math.Min(Indirect, Ring))..];
        }
    }

    static int RingCount() => (int?)RingAt?.GetValue(null) ?? 0;

    /// <summary>The first difference between the recompiled run's calls and ours.</summary>
    public static string? Diff(Trace theirs, Trace ours)
    {
        if (!Installed) return null;
        int n = Math.Min(theirs.Direct.Count, ours.Direct.Count);
        for (int i = 0; i < n; i++)
            if (theirs.Direct[i] != ours.Direct[i])
                return $"call {i}: recompiled {Show(theirs.Direct[i])}, ours {Show(ours.Direct[i])}";
        if (theirs.Direct.Count != ours.Direct.Count)
            return $"calls: recompiled made {theirs.Direct.Count}, ours {ours.Direct.Count}" +
                   (n < theirs.Direct.Count ? $"; next recompiled {Show(theirs.Direct[n])}" : $"; next ours {Show(ours.Direct[n])}");
        if (theirs.Indirect != ours.Indirect)
            return $"indirect calls: recompiled {theirs.Indirect}, ours {ours.Indirect}";
        if (!theirs.Tail.AsSpan().SequenceEqual(ours.Tail))
            return "indirect calls: the targets differ";
        return null;
    }

    static string Show(in Call c) => $"ra {c.Ra:X8} a0={c.A0:X} a1={c.A1:X} a2={c.A2:X} a3={c.A3:X}";
}
