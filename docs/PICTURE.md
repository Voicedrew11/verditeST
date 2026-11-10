# The picture

Phase 7 of `docs/PORTING_PLAN.md`: the picture's switches, each the runtime's work
with a switch and a probe here (`patches/Picture/`), ported from Verdite1's. All on by
default, each with an `ST_` switch to compare (`docs/ENV_VARS.md`). Judged by eye: the
user's.

| switch | what | file |
|---|---|---|
| `ST_NODITHER` | the GPU's ordered dither off: the runtime masks bit 9 of every draw mode (`Gpu.SuppressDither`), so the game's memory is untouched | `NoDither.cs` |
| `ST_TRUECOLOR` | 24-bit output: the display target is RGBA8 (`GteDepth.TrueColor`) | `TrueColor.cs` |
| `ST_PERSPECTIVE` | perspective-correct textures: the runtime's address map (`GteVertexMap`) recovers each corner's depth from the GTE and the renderer interpolates U/W, V/W, 1/W | `Perspective.cs` |
| `ST_SUBPIXEL` | the fraction of a pixel the GTE truncates, kept by the same map | `Subpixel.cs` |
| `ST_ZBUFFER` | per-pixel occlusion instead of the ordering table, from the same map's depths; a triangle with no depth keeps painter's order, so a miss is the old picture. Blended surfaces drawn in table order (`ST_BLENDORDER`, the fork's `0079`) | `ZBuffer.cs` |

The probes hook every program's `DrawOTag` and `PutDrawEnv` (the six columns of
"The SDK in each program" in `docs/RECOMPILATION.md`).

## Measured

2026-11-10 over `scripts/session.py` (map 0, the menu, a death) at 144 fps:

- **Dither:** every draw env (140 to 144 a second) and every E1 word in the table
  (about 1,400 a second, ten a frame) asked for dither; GPUSTAT's dither bit read 0
  after every frame with the switch on.
- **The address map** (perspective and sub-pixel): 106,000 to 141,000 roots and
  285,000 to 347,000 propagations a second; **98.3 to 98.7% of lookups hit**, about
  2,250 misses a second. A miss draws that polygon as the console would.
- **Sub-pixel:** 86,000 to 124,000 vertices a second carry a fraction; mean offset
  0.71 to 0.75 px, at most 1.41 px.
- With all four on, the 54 draw routines the session reaches verify at 0 mismatches,
  and the acceptance test passes.

**The misses by writer** (`ST_MAPCOVERAGE=1`, `MapCoverage.cs`, the same session):
every miss is a 2D packet. The HUD's panels (`func_80026964`'s `HudQuad`, `HudFill`
and its own F4: four polygons a frame, about 2,300 vertices a second) and the
full-screen FT4S of the load fade (`func_80021654`) are written without the GTE, so the
map has nothing to find and they draw as on the console, which is right for them.
The 3D writers: the scene (`func_80020A80`) 92,000 to 119,000 vertices a second at
**99.98%**, at most 19 a second missed; the models (`func_800241D0`) 19,000 to 52,000
at **100.0%**.

Over `scripts/tour.py` (the 12 maps in `patches/Arrivals.cs`, four boots after
deaths), summed over each boot: the scene 99.98 to 100.00% of 70,000 to 3,600,000
vertex-seconds, the models (`func_800241D0`, its `func_80022678` site b) 100.00%,
the effects (`func_800246E4`) 100.00%, the second model list (`func_800251C8`)
100.00%. The HUD and the tint at 0%, as in map 0.

The scene's own calls own no packets in this probe: they keep the cursor in the
scratchpad block (`docs/GEOMETRY.md`) and the scene writes it back once, so its row
holds all of them.

- **Z-buffer:** 55,000 to 87,000 triangles a second depth-tested, 97.9 to 98.7% of
  those submitted; about 1,150 a second keep painter's order. No depth clears. Play
  holds 144 fps with it on, and the acceptance test passes.

Verdite1's Z-buffer takes its depths first from its C# writers' packet records
(`PacketDepth`); this port's writers keep none yet, so the address map's depths are
the only source. With the 3D writers at 99.98 to 100% (above) the packet records
would add nothing in map 0; the triangles kept in painter's order are the HUD's.

## Not ported

Verdite1's `SemiAbr` is a probe of its own retained models' blend modes, not a
switch; blended surfaces in table order are `ST_BLENDORDER`, already on. Nothing of
Verdite1's picture is left to port.

**By eye (the user):** each switch against its `=0`.
