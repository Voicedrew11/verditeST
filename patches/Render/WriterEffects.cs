using System.Reflection;
using RecompOne.Runtime;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// ST_WRITER_EFFECTS=1: what the face routines leave besides their packets, so the
/// retained world can stop running them. Around every 16th call of each of the scene's
/// four face routines and the models' three steps, all of RAM but the two packet
/// buffers and the ordering tables, the scratchpad, the GTE and the registers are
/// compared before and after; every word that changed is counted by routine and
/// address. A diagnostic, every five seconds. See "Not done" in docs/GPU_RETAINED.md.
/// </summary>
static class WriterEffects
{
    static readonly (uint Address, string Name)[] Routines =
    [
        (0x8001F954, "SceneTriangles"), (0x8001FD00, "SceneQuads"), (0x8001D2A8, "MapTriangles"),
        (0x8001DB64, "MapQuads"), (0x800192D4, "ModelStep192D4"), (0x80019DC8, "ModelStep19DC8"),
        (0x8001BF10, "ModelStep1BF10"), (0x8001E5A0, "MapSmoothTriangles"), (0x8001EEAC, "MapSmoothQuads"),
    ];

    // The packet buffers and both tables of both frames (docs/GEOMETRY.md, "The packet buffer").
    const uint PacketsStart = 0x0011C488, PacketsEnd = 0x0015C488, TablesStart = 0x0015D174, TablesEnd = 0x00165378;

    static readonly ModInfo _self = new() { Id = "st.writereffects", Name = "Writer effects", Version = "1.0" };
    static readonly SortedDictionary<string, long> _changed = [];
    static readonly long[] _calls = new long[Routines.Length];
    static byte[] _ram = [], _pad = [];
    static readonly uint[] _gte = new uint[64];
    static readonly uint[] _regs = new uint[32];
    static int _active = -1;
    static long _at;

    public static void Install(string? spec)
    {
        if (spec?.Trim() != "1") return;
        HookAttach.OnOverlayLoad("writer effects", Attach);
    }

    static bool Attach()
    {
        SymbolRegistry.Build();
        MethodInfo Hook(string n) => typeof(WriterEffects).GetMethod(n, BindingFlags.Public | BindingFlags.Static)!;
        for (int i = 0; i < Routines.Length; i++)
        {
            if (SymbolRegistry.Resolve(MainLoop.Overlay, null, Routines[i].Address) is not { } t) return false;
            HookManager.AddPre(_self, t, Hook("Pre" + i), order: int.MinValue);
            HookManager.AddPost(_self, t, Hook(nameof(Post)), order: int.MaxValue);
        }
        HookManager.Commit();
        Console.WriteLine("[ST] writer effects: on");
        return true;
    }

    public static void Pre0(CpuContext c, IMemory m) => Pre(0, c, m);
    public static void Pre1(CpuContext c, IMemory m) => Pre(1, c, m);
    public static void Pre2(CpuContext c, IMemory m) => Pre(2, c, m);
    public static void Pre3(CpuContext c, IMemory m) => Pre(3, c, m);
    public static void Pre4(CpuContext c, IMemory m) => Pre(4, c, m);
    public static void Pre5(CpuContext c, IMemory m) => Pre(5, c, m);
    public static void Pre6(CpuContext c, IMemory m) => Pre(6, c, m);
    public static void Pre7(CpuContext c, IMemory m) => Pre(7, c, m);
    public static void Pre8(CpuContext c, IMemory m) => Pre(8, c, m);

    static void Pre(int routine, CpuContext c, IMemory m)
    {
        if (_active >= 0 || _calls[routine]++ % 16 != 0 || m is not PSMemory mem) return;
        _active = routine;
        _ram = mem.Ram.ToArray();
        _pad = new byte[0x400];
        for (uint i = 0; i < 0x400; i += 4) BitConverter.TryWriteBytes(_pad.AsSpan((int)i), m.ReadU32(0x1F800000 + i));
        for (int i = 0; i < 32; i++) { _gte[i] = Gte.Read(i); _gte[32 + i] = Gte.ReadControl(i); }
        for (int i = 0; i < 32; i++) _regs[i] = c[i];
    }

    public static void Post(CpuContext c, IMemory m)
    {
        if (_active < 0 || m is not PSMemory mem) return;
        string name = Routines[_active].Name;
        _active = -1;
        var now = mem.Ram;
        for (int a = 0; a + 4 <= now.Length; a += 4)
        {
            if (a >= PacketsStart && a < PacketsEnd || a >= TablesStart && a < TablesEnd) continue;
            if (BitConverter.ToUInt32(now.Slice(a, 4)) == BitConverter.ToUInt32(_ram, a)) continue;
            // The stack below the caller's pointer is dead after the return.
            if (a >= (_regs[29] & 0x1FFFFF) - 0x2000 && a < (_regs[29] & 0x1FFFFF)) continue;
            Count($"{name} ram {0x80000000 | (uint)a:X8}");
        }
        for (uint i = 0; i < 0x400; i += 4)
            if (m.ReadU32(0x1F800000 + i) != BitConverter.ToUInt32(_pad, (int)i)) Count($"{name} scratch +{i:X3}");
        for (int i = 0; i < 32; i++)
        {
            if (Gte.Read(i) != _gte[i]) Count($"{name} gte data {i}");
            if (Gte.ReadControl(i) != _gte[32 + i]) Count($"{name} gte control {i}");
        }
        foreach (int r in new[] { 16, 17, 18, 19, 20, 21, 22, 23, 28, 29, 30, 31 })
            if (c[r] != _regs[r]) Count($"{name} reg {r}");
        if (Environment.TickCount64 < _at) return;
        _at = Environment.TickCount64 + 5000;
        Console.WriteLine($"[ST] writer effects: {string.Join(", ", _changed.Select(e => $"{e.Key} x{e.Value}"))}");
    }

    static void Count(string key) => _changed[key] = _changed.GetValueOrDefault(key) + 1;
}
