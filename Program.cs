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

RecompOne.Runtime.Runtime.AppId = "verditest";

var memory = new PSMemory();
Entry.Run(memory, args.Length > 0 ? args[0] : null);
return 0;
