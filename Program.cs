using RecompOne.Runtime.Memory;
using Recompiled;

// Entry point for the Shadow Tower (SLUS-00863) port. Hand-owned, so RecompOne does
// not generate one into generated/. Init and hooks go here, before Entry.Run.

Verdite.Core.Game.Configure(tag: "ST");

// The runtime's log channels, through an env var:
//     ST_LOG=bios,cd,gpu,dma,sdk,spu,mdec,irq   (or ST_LOG=all)
var channels = (Verdite.Core.Game.Env("LOG") ?? "")
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .Select(s => s.ToLowerInvariant())
    .ToHashSet();

if (channels.Count > 0)
{
    bool all = channels.Contains("all");
    RecompOne.Runtime.Log.BiosOn = all || channels.Contains("bios");
    RecompOne.Runtime.Log.CdOn = all || channels.Contains("cd");
    RecompOne.Runtime.Log.GpuOn = all || channels.Contains("gpu");
    RecompOne.Runtime.Log.DmaOn = all || channels.Contains("dma");
    RecompOne.Runtime.Log.SdkOn = all || channels.Contains("sdk");
    RecompOne.Runtime.Log.SpuOn = all || channels.Contains("spu");
    RecompOne.Runtime.Log.MdecOn = all || channels.Contains("mdec");
    RecompOne.Runtime.Log.IrqOn = all || channels.Contains("irq");
    Console.WriteLine($"[ST] log channels: {string.Join(",", channels)}");
}

// libapi's interrupt-callback table, per executable: the table setIntr indexes by
// irq*4, intrEnv + 4. See "The interrupt-callback table" in docs/RECOMPILATION.md.
RecompOne.Runtime.Events.Event.AddListener<RecompOne.Runtime.Events.OverlayLoadedEvent>(e =>
{
    uint table = e.Name switch
    {
        "str" => 0x80031528u,
        "st" => 0x80086B0Cu,
        "title" => 0x80027738u,
        "optxt" => 0x8001E434u,
        "vs" => 0x80061384u,
        "end" => 0x8002EA40u,
        _ => 0u,
    };
    if (table == 0) return;
    RecompOne.Runtime.Interrupts.CallbackTable = table;
    Console.WriteLine($"[ST] irq callback table: {e.Name} 0x{table:X8}");
});

RecompOne.Runtime.Runtime.AppId = "verditest";

// ST.EXE waits for a button in loops that never call VSync (func_80062C28's message
// prompts), so the vblank handler has to run from the interrupt poll as well.
RecompOne.Runtime.Sdk.LibEtc.VBlankFromPoll = true;

var memory = new PSMemory();

// The command channel: press buttons and read memory from a script.
St.AgentServer.Install(Verdite.Core.Game.Env("SHELL"), memory);

// ST_BOOTEXE=str|title|optxt|st|end|vs starts that program instead of the movies:
// the stub's first index, written once the stub is in memory. See "How one program
// starts the next" in docs/RECOMPILATION.md.
int bootExe = Array.IndexOf(new[] { "str", "title", "optxt", "st", "end", "vs" }, Verdite.Core.Game.Env("BOOTEXE"));
RecompOne.Runtime.Events.Event.AddListener<RecompOne.Runtime.Events.OverlayLoadedEvent>(e =>
{
    if (e.Name != "main" || bootExe <= 0) return;
    memory.WriteU32(0x80010394u, (uint)bootExe);
    Console.WriteLine($"[ST] boot: program {bootExe}");
});

Entry.Run(memory, args.Length > 0 ? args[0] : null);
return 0;
