using System.Reflection;
using RecompOne.Runtime;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Events;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// Phase 11's switch and the retained frame's lifetime: the scene (<c>func_80020A80</c>)
/// begins a retained frame, the map (<see cref="RetainedMap"/>) and the models
/// (<see cref="RetainedModels"/>) publish into it, and the table walk draws it at slot 1,
/// ahead of every packet, which the Z-buffer then tests against it. The frame's "world"
/// is the frame's own view space: the view is the identity, so an instance is placed by
/// the very matrix the GTE projected it with, and nothing world-sized is lost to floats.
///
///     ST_GPU_WORLD=0|off     packets only (the default until judged)
///     ST_GPU_WORLD=shadow    meshes built and instances published, packets drawn
///     ST_GPU_WORLD=1         the retained world drawn, its packets taken off the table
///     ST_GPU_WORLD_PROBE=1   a line every 5 s: frames, instances, fallbacks by reason
///
/// A fallback leaves the game's packets for what it covers, counted by family and
/// reason. See "The retained map" in docs/GPU_RENDERER.md.
/// </summary>
public static class GpuWorld
{
    const uint SceneRoutine = 0x80020A80, FrameHeadRoutine = 0x800174C4;

    static readonly ModInfo _self = new() { Id = "st.gpuworld", Name = "Retained world", Version = "1.0" };

    /// <summary>0 packets, 1 shadow (published, not drawn), 2 drawn.</summary>
    public static int Mode { get; set; }

    public static bool Probe { get; set; }

    static bool _frame, _reset;
    static long _reportAt;
    public static long Frames, Instances, StoreResets;
    static readonly SortedDictionary<string, long> _reasons = [];

    /// <summary>A frame is open and published into.</summary>
    public static bool Capture => Mode != 0 && _frame;

    /// <summary>What is published replaces the packets.</summary>
    public static bool Drawing => Capture && Mode == 2;

    /// <summary>Why the retained world cannot draw at all, or null.</summary>
    public static string? Blocker => !RetainedScene.Supported ? "backend-capability"
        : !GteDepth.ZBuffer ? "zbuffer-off"
        : Literal.Verifying > 0 ? "literal-verify" : null;

    // Past these the stores are emptied as an area change empties them.
    const int MaxMeshCorners = 1 << 20, MaxPoseTexels = 1 << 20;

    public static void Configure(string? mode, string? probe)
    {
        Mode = (mode?.Trim().ToLowerInvariant()) switch
        {
            "1" or "on" => 2,
            "shadow" => 1,
            _ => 0,
        };
        Probe = probe?.Trim() == "1";
    }

    public static void Install()
    {
        Event.AddListener<OverlayLoadedEvent>(_ => _reset = true);
        HookAttach.OnOverlayLoad("retained world", Attach);
        RetainedMap.Install();
        RetainedModels.Install();
    }

    static bool Attach()
    {
        SymbolRegistry.Build();
        MethodInfo Hook(string n) => typeof(GpuWorld).GetMethod(n, BindingFlags.Public | BindingFlags.Static)!;
        if (SymbolRegistry.Resolve(MainLoop.Overlay, null, SceneRoutine) is not { } scene) return false;
        if (SymbolRegistry.Resolve(MainLoop.Overlay, null, FrameHeadRoutine) is not { } head) return false;
        HookManager.AddPre(_self, scene, Hook(nameof(SceneIn)), order: int.MinValue + 1);
        HookManager.AddPost(_self, scene, Hook(nameof(SceneOut)), order: int.MaxValue - 1);
        HookManager.AddPost(_self, head, Hook(nameof(FrameHead)));
        HookManager.Commit();
        bool on = HookAttach.Installed(scene) && HookAttach.Installed(head);
        Console.WriteLine($"[ST] retained world: {(on ? "attached" : "not attached")}, mode {Mode}");
        return on;
    }

    /// <summary>The tables are cleared for a new frame: a world begun before belongs to a
    /// frame that was never drawn.</summary>
    public static void FrameHead(CpuContext c, IMemory m)
    {
        RetainedScene.MainSerial = 0;
        RetainedScene.MainView = false;
        if (_reset || RetainedScene.MeshCornerCount > MaxMeshCorners || RetainedScene.PoseTexels > MaxPoseTexels)
        {
            if (!_reset) StoreResets++;
            _reset = false;
            RetainedMeshes.Clear();
        }
        Report();
        RetainedCheck.Report();
    }

    public static void SceneIn(CpuContext c, IMemory m)
    {
        _frame = false;
        if (Mode == 0) return;
        if (Blocker is { } why) { Fallback("world", why); return; }
        if ((ushort)Gte.ReadControl(26) is > 0 and var h)
        {
            GteDepth.ProjH = h;
            GteDepth.ProjCx = (int)Gte.ReadControl(24) / 65536f;
            GteDepth.ProjCy = (int)Gte.ReadControl(25) / 65536f;
        }
        RetainedScene.BeginFrame(new RetainedScene.View
        {
            R00 = 1, R11 = 1, R22 = 1,
            H = GteDepth.ProjH, Cx = GteDepth.ProjCx, Cy = GteDepth.ProjCy,
        });
        RetainedScene.MainView = Mode == 2;
        RetainedScene.MainSerial = Mode == 2 ? RetainedScene.Serial : 0;
        _frame = true;
        Frames++;
    }

    /// <summary>The models and the hands publish after the scene, until the swap.</summary>
    public static void SceneOut(CpuContext c, IMemory m) { }

    public static void Fallback(string family, string reason)
    {
        string key = $"{family}:{reason}";
        _reasons[key] = _reasons.GetValueOrDefault(key) + 1;
    }

    static void Report()
    {
        if (!Probe || Mode == 0 || Environment.TickCount64 < _reportAt) return;
        _reportAt = Environment.TickCount64 + 5000;
        string reasons = _reasons.Count == 0 ? "none" : string.Join(", ", _reasons.Select(r => $"{r.Key} {r.Value}"));
        Console.WriteLine($"[ST] retained world: mode {Mode}, frames {Frames}, map {RetainedMap.Report()}, " +
                          $"models {RetainedModels.Report()}, meshes {RetainedMeshes.Builds} built/{RetainedMeshes.Hits} hits/" +
                          $"{RetainedMeshes.Rebuilds} rebuilt, store {RetainedScene.MeshCornerCount} corners/" +
                          $"{RetainedScene.PoseTexels} texels, GPU draws {RetainedScene.MainDraws}/missed {RetainedScene.MainMissed}, " +
                          $"instances drawn {RetainedScene.InstancesDrawn}, fallbacks: {reasons}");
    }
}
