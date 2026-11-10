using System.Reflection;
using RecompOne.Runtime;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;

namespace St;

/// <summary>Literal's verify: the recompiled routine and the transcription run from
/// one state and compared. See Literal.</summary>
public sealed partial class Literal
{
    const uint StackWindow = 0x2000;
    const int Deferred = 1 << 30;
    static readonly FieldInfo? Countdown =
        typeof(Interrupts).GetField("_countdown", BindingFlags.NonPublic | BindingFlags.Static);

    // The HLE rand()'s seed lives outside guest RAM, so a routine that calls rand() (the
    // scene's light flicker) would draw a different number on its second run.
    static readonly FieldInfo? RandSeed =
        typeof(RecompOne.Runtime.Bios.BiosA).GetField("_randSeed", BindingFlags.NonPublic | BindingFlags.Static);

    byte[] _before = [], _theirs = [];
    readonly uint[] _padBefore = new uint[Differential.PadWords], _padTheirs = new uint[Differential.PadWords],
                    _padOurs = new uint[Differential.PadWords];
    readonly Gte.State _gteEntry = new(), _gteTheirs = new(), _gteOurs = new();
    long _calls, _bad, _interrupted, _twinBad;
    readonly List<string> _samples = [];
    double _reportAt;

    void Verify(Action<CpuContext, IMemory> orig, CpuContext c, PSMemory mem)
    {
        var ram = Differential.Ram(mem);
        if (_before.Length != ram.Length)
        {
            _before = new byte[ram.Length];
            _theirs = new byte[ram.Length];
        }

        uint a0 = c.A0, a1 = c.A1, ra = c.RA;
        ram.CopyTo(_before);
        Differential.SavePad(mem, _padBefore);
        var entry = c.Snapshot();
        Gte.Save(_gteEntry);
        int countdown = (int?)Countdown?.GetValue(null) ?? 0;
        object? seed = RandSeed?.GetValue(null);
        Countdown?.SetValue(null, Deferred);
        uint slow = Interrupts.SlowPolls;

        orig(c, mem);
        ram.CopyTo(_theirs);
        Differential.SavePad(mem, _padTheirs);
        var theirs = c.Snapshot();
        Gte.Save(_gteTheirs);
        int countTheirs = (int?)Countdown?.GetValue(null) ?? 0;
        object? seedTheirs = RandSeed?.GetValue(null);
        uint slowTheirs = Interrupts.SlowPolls;

        _before.CopyTo(ram);
        Differential.LoadPad(mem, _padBefore);
        c.Restore(entry);
        Gte.Load(_gteEntry);
        Countdown?.SetValue(null, Deferred);
        RandSeed?.SetValue(null, seed);
        _body(c, mem);
        Differential.SavePad(mem, _padOurs);
        var ours = c.Snapshot();
        Gte.Save(_gteOurs);
        int countOurs = (int?)Countdown?.GetValue(null) ?? 0;
        uint slowOurs = Interrupts.SlowPolls;

        _calls++;
        string at = $"a0={a0:X} a1={a1:X} ra={ra:X}";
        if (slowTheirs != slow || slowOurs != slowTheirs) _interrupted++;
        else
        {
            string? why = null;
            int hi = (int)(entry.SP & (uint)(ram.Length - 1));
            int lo = Math.Max(0, hi - (int)StackWindow);
            why ??= Differential.Describe(ram, _theirs, 0, lo) ?? Differential.Describe(ram, _theirs, hi, ram.Length);
            why ??= Differential.DescribePad(_padOurs, _padTheirs);
            why ??= Registers(ours, theirs);
            why ??= Gte.Diff(_gteTheirs, _gteOurs) is { } g ? $"GTE {g}" : null;
            why ??= countOurs != countTheirs ? $"polls: recompiled left {countTheirs}, ours {countOurs}" : null;
            if (why != null)
            {
                if (_bad == 0 && Dump is { } dir) DumpStates(dir, ram);
                _bad++;
                if (_samples.Count < 8) _samples.Add($"{at}: {why}");
            }
        }

        if (_twin != null && slowTheirs == slow)
        {
            _before.CopyTo(ram);
            Differential.LoadPad(mem, _padBefore);
            c.Restore(entry);
            RandSeed?.SetValue(null, seed);
            _twin(c, mem);
            Differential.SavePad(mem, _padOurs);
            int hi = (int)(entry.SP & (uint)(ram.Length - 1));
            int lo = Math.Max(0, hi - (int)StackWindow);
            string? why = Differential.Describe(ram, _theirs, 0, lo) ?? Differential.Describe(ram, _theirs, hi, ram.Length)
                       ?? Differential.DescribePad(_padOurs, _padTheirs);
            if (why != null)
            {
                _twinBad++;
                if (_samples.Count < 8) _samples.Add($"twin {at}: {why}");
            }
        }

        // The recompiled result stands.
        _theirs.CopyTo(ram);
        Differential.LoadPad(mem, _padTheirs);
        c.Restore(theirs);
        Gte.Load(_gteTheirs);
        Countdown?.SetValue(null, Math.Max(1, countdown - (Deferred - countTheirs)));
        RandSeed?.SetValue(null, seedTheirs);

        double now = Environment.TickCount64 / 1000.0;
        if (now < _reportAt) return;
        _reportAt = now + 2.0;
        Console.WriteLine($"[ST] verify {Name} (func_{Address:X8}): {_calls} call(s), {_bad} mismatch(es), " +
                          $"{_interrupted} interrupted" + (_twin != null ? $", twin {_twinBad} mismatch(es)" : ""));
        foreach (var s in _samples) Console.WriteLine($"[ST]   {s}");
        _samples.Clear();
        _calls = _bad = _interrupted = _twinBad = 0;
    }

    /// <summary>ST_LITERAL_DUMP=dir: the first mismatch of each routine writes the
    /// entry, recompiled and transcribed RAM images there, for diffing.</summary>
    static readonly string? Dump = Environment.GetEnvironmentVariable("ST_LITERAL_DUMP");

    void DumpStates(string dir, Span<byte> ours)
    {
        Directory.CreateDirectory(dir);
        string stem = Path.Combine(dir, $"{Address:X8}-{DateTime.Now:HHmmss}");
        File.WriteAllBytes(stem + "-entry.bin", _before);
        File.WriteAllBytes(stem + "-recompiled.bin", _theirs);
        File.WriteAllBytes(stem + "-ours.bin", ours.ToArray());
    }

    static readonly string[] Names =
        ["at", "v0", "v1", "a0", "a1", "a2", "a3", "t0", "t1", "t2", "t3", "t4", "t5", "t6", "t7",
         "s0", "s1", "s2", "s3", "s4", "s5", "s6", "s7", "t8", "t9", "gp", "sp", "fp", "ra", "lo", "hi"];

    static uint[] Fields(in CpuSnapshot s) =>
        [s.At, s.V0, s.V1, s.A0, s.A1, s.A2, s.A3, s.T0, s.T1, s.T2, s.T3, s.T4, s.T5, s.T6, s.T7,
         s.S0, s.S1, s.S2, s.S3, s.S4, s.S5, s.S6, s.S7, s.T8, s.T9, s.GP, s.SP, s.FP, s.RA, s.LO, s.HI];

    static string? Registers(in CpuSnapshot ours, in CpuSnapshot theirs)
    {
        var o = Fields(ours);
        var t = Fields(theirs);
        for (int i = 0; i < o.Length; i++)
            if (o[i] != t[i]) return $"{Names[i]} recompiled {t[i]:X8} ours {o[i]:X8}";
        return null;
    }
}
