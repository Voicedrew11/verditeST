using System.Reflection;
using RecompOne.Runtime;
using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Modding;

namespace St;

/// <summary>
/// The models on the GPU. Every model part reaches one of three steps (fogged
/// <c>func_800192D4</c>, unfogged <c>func_80019DC8</c>, mixed with the near path
/// <c>func_8001BF10</c>) with the GTE holding the part's view-space matrix and the
/// model's light, and the part's vertices posed in place in the bank. At the step's
/// entry the part becomes an instance: its faces from the mesh store
/// (<see cref="RetainedMeshes"/>), its posed vertices uploaded for the frame (a pose
/// changes from one entity to the next in the same frame), placed by the GTE's matrix
/// with the frame's identity view, lit by the GTE's LLM, BK and LCM and the step's face
/// colour, fogged on runtime 0123's curve 6 (held under 4066, as every model vertex is).
/// The step then runs as the game wrote it and <see cref="PacketFilter"/> takes its
/// packets off but the blended ones. Every caller (the entities, the effects, the
/// second list, the cels, the held models) goes through these steps. See "The retained
/// models" in docs/GPU_RENDERER.md.
/// </summary>
static class RetainedModels
{
    const uint Scratch = 0x1F800000, SceneFlags = 0x8017BE8C, Flicker = 0x8017BE88;
    const uint Fogged = 0x800192D4, Unfogged = 0x80019DC8, Mixed = 0x8001BF10;

    static readonly ModInfo _self = new() { Id = "st.retainedmodels", Name = "Retained models", Version = "1.0" };
    static readonly PacketFilter _filter = new();
    static bool _filtering;
    static long _parts, _vertices, _blendedParts;

    public static void Install() => HookAttach.OnOverlayLoad("retained models", Attach);

    static bool Attach()
    {
        SymbolRegistry.Build();
        MethodInfo Hook(string n) => typeof(RetainedModels).GetMethod(n, BindingFlags.Public | BindingFlags.Static)!;
        var targets = new List<MethodInfo>();
        foreach (var (a, pre) in new[] { (Fogged, nameof(BeforeFogged)), (Unfogged, nameof(BeforeUnfogged)), (Mixed, nameof(BeforeMixed)) })
        {
            if (SymbolRegistry.Resolve(MainLoop.Overlay, null, a) is not { } t) return false;
            HookManager.AddPre(_self, t, Hook(pre), order: int.MinValue + 2);
            HookManager.AddPost(_self, t, Hook(nameof(After)), order: int.MaxValue - 2);
            targets.Add(t);
        }
        HookManager.Commit();
        return targets.All(HookAttach.Installed);
    }

    public static void BeforeFogged(CpuContext c, IMemory m) => Before(c, m, Fogged);
    public static void BeforeUnfogged(CpuContext c, IMemory m) => Before(c, m, Unfogged);
    public static void BeforeMixed(CpuContext c, IMemory m) => Before(c, m, Mixed);

    static void Before(CpuContext c, IMemory m, uint step)
    {
        _filtering = false;
        if (!GpuWorld.Capture) return;
        if (!Publish(c, m, step)) return;
        if (!GpuWorld.Drawing) return;
        _filter.Open(m);
        _filtering = true;
    }

    public static void After(CpuContext c, IMemory m)
    {
        if (!_filtering) return;
        _filtering = false;
        _filter.Close(m, PacketFilter.Semi);
    }

    static bool Publish(CpuContext c, IMemory m, uint step)
    {
        int scene = m.ReadU8(SceneFlags);
        if ((scene & 6) != 0) { GpuWorld.Fallback("models", "flicker-recolour"); return false; }
        uint flags = c.A1 & 0xFF;
        if ((flags & 4) != 0) { GpuWorld.Fallback("models", "blended-model"); _blendedParts++; return false; }
        uint bank = m.ReadU32(Scratch + 0xC), base_ = bank + 0xC;
        uint descriptor = base_ + (c.A0 & 0xFFFF) * 0x28;
        uint faces = base_ + m.ReadU32(descriptor + 0x10);
        int ft3 = m.ReadU16(descriptor + 0x18), ft4 = m.ReadU16(descriptor + 0x1C);
        int gt3 = m.ReadU16(descriptor + 0x1A), gt4 = m.ReadU16(descriptor + 0x1E);
        var runs = new RetainedMeshes.Run[]
        {
            new(RetainedMeshes.FaceShape.FlatTriangle, faces, ft3),
            new(RetainedMeshes.FaceShape.FlatQuad, faces + (uint)ft3 * 0x14, ft4),
            new(RetainedMeshes.FaceShape.SmoothTriangle, faces + (uint)ft3 * 0x14 + (uint)ft4 * 0x1C, gt3),
            new(RetainedMeshes.FaceShape.SmoothQuad, faces + (uint)ft3 * 0x14 + (uint)ft4 * 0x1C + (uint)gt3 * 0x18, gt4),
        };
        uint normals = base_ + m.ReadU32(descriptor + 8), vertices = base_ + m.ReadU32(descriptor);
        int count = (int)m.ReadU32(descriptor + 4);
        var mesh = RetainedMeshes.Get(m, descriptor, runs, normals, count, out string? reason);
        if (mesh == null) { GpuWorld.Fallback("models", reason ?? "mesh"); return false; }
        if (!RetainedMeshes.InRam(vertices, (uint)count * 8)) { GpuWorld.Fallback("models", "vertex-range"); return false; }
        int at = RetainedScene.AddModelVertices(RetainedMeshes.Shorts(m, vertices, count));
        if (at < 0) { GpuWorld.Fallback("models", "no-frame"); return false; }
        bool fog = step == Fogged || step == Mixed && (scene & 0x10) == 0;
        var inst = new RetainedScene.ModelInstance
        {
            MeshStart = mesh.Start, MeshCount = mesh.Opaque, MeshAll = mesh.Total, Pose = 0, VertBase = at,
            Rgbc = FaceColour(scene), Tile = step == Mixed,
            Near = ((int)m.ReadU32(Scratch + 0x80) + 1) * 8f, Far = (int)m.ReadU32(Scratch + 4) * 8f,
            Dqa = (int)m.ReadU32(Scratch + 0x64), Dqb = (int)m.ReadU32(Scratch + 0x68),
            Curve = fog ? LinearFogFactor.Held : 0,
        };
        ReadGte(ref inst);
        RetainedScene.AddInstance(inst);
        if (RetainedCheck.On)
            RetainedCheck.Model(m, inst, vertices, count, m.ReadU32(Scratch + 0x40), fog);
        _parts++;
        _vertices += count;
        return true;
    }

    /// <summary>The face colour the steps set at the scratchpad's <c>+0x60</c>: grey,
    /// or under the flicker a level from the light's random word.</summary>
    static uint FaceColour(int scene) => 0x808080;

    /// <summary>The part's view-space matrix and the model's light, as the GTE holds them.</summary>
    static void ReadGte(ref RetainedScene.ModelInstance i)
    {
        static short Lo(uint w) => (short)w;
        static short Hi(uint w) => (short)(w >> 16);
        uint a = Gte.ReadControl(0), b = Gte.ReadControl(1), c = Gte.ReadControl(2), d = Gte.ReadControl(3);
        i.V00 = Lo(a); i.V01 = Hi(a); i.V02 = Lo(b); i.V10 = Hi(b); i.V11 = Lo(c); i.V12 = Hi(c);
        i.V20 = Lo(d); i.V21 = Hi(d); i.V22 = Lo(Gte.ReadControl(4));
        i.Vtx = (int)Gte.ReadControl(5); i.Vty = (int)Gte.ReadControl(6); i.Vtz = (int)Gte.ReadControl(7);
        i.R00 = i.V00 / 4096f; i.R01 = i.V01 / 4096f; i.R02 = i.V02 / 4096f;
        i.R10 = i.V10 / 4096f; i.R11 = i.V11 / 4096f; i.R12 = i.V12 / 4096f;
        i.R20 = i.V20 / 4096f; i.R21 = i.V21 / 4096f; i.R22 = i.V22 / 4096f;
        i.Tx = i.Vtx; i.Ty = i.Vty; i.Tz = i.Vtz;
        a = Gte.ReadControl(8); b = Gte.ReadControl(9); c = Gte.ReadControl(10); d = Gte.ReadControl(11);
        i.Llm0 = Lo(a) / 4096f; i.Llm1 = Hi(a) / 4096f; i.Llm2 = Lo(b) / 4096f; i.Llm3 = Hi(b) / 4096f;
        i.Llm4 = Lo(c) / 4096f; i.Llm5 = Hi(c) / 4096f; i.Llm6 = Lo(d) / 4096f; i.Llm7 = Hi(d) / 4096f;
        i.Llm8 = Lo(Gte.ReadControl(12)) / 4096f;
        i.Bk0 = (int)Gte.ReadControl(13); i.Bk1 = (int)Gte.ReadControl(14); i.Bk2 = (int)Gte.ReadControl(15);
        a = Gte.ReadControl(16); b = Gte.ReadControl(17); c = Gte.ReadControl(18); d = Gte.ReadControl(19);
        i.L0 = Lo(a); i.L1 = Hi(a); i.L2 = Lo(b); i.L3 = Hi(b); i.L4 = Lo(c); i.L5 = Hi(c);
        i.L6 = Lo(d); i.L7 = Hi(d); i.L8 = Lo(Gte.ReadControl(20));
    }

    public static string Report() =>
        $"{_parts} parts/{_vertices} vertices ({_blendedParts} blended parts left as packets), " +
        $"packets off {_filter.Removed}/kept {_filter.Kept}";
}
