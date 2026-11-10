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

// What a script needs to know (ST_AGENT=1 prints it), the command channel that
// answers with it, and booting into play with nobody at the pad.
St.AgentBeacon.Install(Verdite.Core.Game.Env("AGENT"), memory);
St.AgentServer.Install(Verdite.Core.Game.Env("SHELL"), memory);
St.AutoStart.Install(Verdite.Core.Game.Env("AUTOSTART"), memory);

// The main loop's survey: which stages write the ordering table.
St.StageProbe.Install(Verdite.Core.Game.Env("STAGEPROBE"));
// The draw path's survey: what each of stage 14's calls adds to the table.
St.GeometryProbe.Install(Verdite.Core.Game.Env("GEOPROBE"));
// Who sends each frame to the screen: stage 14, or a loop drawing its own.
St.PresentProbe.Install(Verdite.Core.Game.Env("PRESENTPROBE"));
// The picture (Phase 7, docs/PICTURE.md): the runtime's switches and their probes.
St.NoDither.Configure(Verdite.Core.Game.Env("NODITHER"), Verdite.Core.Game.Env("NODITHER_PROBE"));
St.NoDither.Install();
St.TrueColor.Configure(Verdite.Core.Game.Env("TRUECOLOR"));
St.Perspective.Configure(Verdite.Core.Game.Env("PERSPECTIVE"), Verdite.Core.Game.Env("PERSPECTIVE_PROBE"));
St.Perspective.Install();
St.Subpixel.Configure(Verdite.Core.Game.Env("SUBPIXEL"), Verdite.Core.Game.Env("SUBPIXEL_PROBE"));
St.Subpixel.Install();
St.ZBuffer.Configure(Verdite.Core.Game.Env("ZBUFFER"), Verdite.Core.Game.Env("ZBUFFER_PROBE"));
St.ZBuffer.Install();
St.MapCoverage.Install(Verdite.Core.Game.Env("MAPCOVERAGE"));
St.WordWatch.Install(Verdite.Core.Game.Env("WORDWATCH"));

// The draw path's routines in C# (patches/Draw/): ST_LITERAL=1|0|verify, or one
// routine's own ST_<NAME>. See "The draw path in C#" in docs/GEOMETRY.md.
St.Literal.InstallAll();
// How much of the frame's packet buffer is spent.
St.PrimBufferProbe.Install(Verdite.Core.Game.Env("PRIMBUF_PROBE"));

// The world at the game's own 20 Hz, the picture at any rate (FramePacing), and
// VSync(n) waiting n vblanks as the console's does (VBlankPacing), which is what
// holds the game to 20 with pacing off. See "Frame pacing" in docs/DEVELOPMENT.md.
St.FramePacing.Configure(Verdite.Core.Game.Env("PACING"), Verdite.Core.Game.Env("FPS"),
                         Verdite.Core.Game.Env("TICKRATE"), Verdite.Core.Game.Env("FPS_PROBE"));
St.FramePacing.Install();
St.VBlankPacing.Configure(Verdite.Core.Game.Env("VBLANKPACING"), Verdite.Core.Game.Env("VBLANKPACING_PROBE"));
St.VBlankPacing.Install();
// What stage 14 advances once a frame (cel animations, the light's flicker, the
// rumble countdown), held to the world's tick under pacing.
St.DrawStageTick.Install();
St.EffectTimers.Install();
St.RateCensus.Install(Verdite.Core.Game.Env("RATECENSUS"));
// The camera carried between world ticks. See "The camera carried" in docs/SMOOTHING.md.
St.ViewSmoothing.Configure(Verdite.Core.Game.Env("SMOOTH"), Verdite.Core.Game.Env("SMOOTH_PROBE"));
St.ViewSmoothing.Install();
St.ModelSmoothing.Configure(Verdite.Core.Game.Env("SMOOTH"), Verdite.Core.Game.Env("SMOOTH_MODELS"),
                            Verdite.Core.Game.Env("SMOOTH_PROBE"));
St.ModelSmoothing.Install();

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
