using System.Reflection;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// ST_PRIMBUF_PROBE=1: how much of the frame's packet buffer is spent. A measurement
/// only; nothing is moved. Verdite1's PrimBufferProbe on this game's frame head.
///
/// The frame head <c>func_800174C4</c> flips the buffer byte (<c>0x8017BD52</c>), points
/// the word at <c>0x80165394</c> at that buffer's record in the table at
/// <c>0x8016537C</c> (two records of 12 bytes: <c>{ start, +4, cursor }</c>) and rewinds
/// the cursor to the start. Before the flip the word still points at the record of the
/// frame just drawn, so a pre hook there reads its final cursor. See "The packet
/// buffer" in docs/GEOMETRY.md.
/// </summary>
public static class PrimBufferProbe
{
    const uint FrameHead = 0x800174C4;
    const uint RecordPointer = 0x80165394;

    static readonly ModInfo _self = new() { Id = "st.primbufprobe", Name = "Packet buffer probe", Version = "1.0" };

    static long _frames, _usedSum, _peak;
    static uint _start, _field4;
    static long _windowStart = -1;

    public static void Install(string? spec)
    {
        if (spec?.Trim() != "1") return;
        HookAttach.OnOverlayLoad("primbufprobe", Attach);
    }

    static bool Attach()
    {
        SymbolRegistry.Build();
        if (SymbolRegistry.Resolve(MainLoop.Overlay, null, FrameHead) is not { } head) return false;
        var pre = typeof(PrimBufferProbe).GetMethod(nameof(Pre), BindingFlags.Public | BindingFlags.Static)!;
        HookManager.AddPre(_self, head, pre);
        HookManager.Commit();
        bool ok = HookAttach.Installed(head);
        Console.WriteLine($"[ST] primbufprobe: {(ok ? "hooked" : "not hooked")}");
        return ok;
    }

    public static void Pre(CpuContext c, IMemory m)
    {
        uint record = m.ReadU32(RecordPointer);
        if (record < 0x80000000u || record >= 0x80200000u) return;
        uint start = m.ReadU32(record), cursor = m.ReadU32(record + 8);
        if (cursor < start || cursor - start > 0x200000u) return;
        long used = cursor - start;
        _frames++;
        _usedSum += used;
        _peak = Math.Max(_peak, used);
        _start = start;
        _field4 = m.ReadU32(record + 4);
        Report();
    }

    static void Report()
    {
        long now = Environment.TickCount64;
        if (_windowStart < 0) { _windowStart = now; return; }
        if (now - _windowStart < 5000) return;
        Console.WriteLine($"[ST] primbufprobe: {_frames} frames, mean 0x{_usedSum / Math.Max(1, _frames):X} bytes, " +
                          $"peak 0x{_peak:X}; record start 0x{_start:X8}, +4 0x{_field4:X8}");
        Console.Out.Flush();
        _frames = _usedSum = _peak = 0;
        _windowStart = now;
    }
}
