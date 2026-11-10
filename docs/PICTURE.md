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

Which routines the 1.5% of misses come from is not measured (Verdite1's `MapCoverage`
does it by writer); nor is any other area than map 0.

- **Z-buffer:** 55,000 to 87,000 triangles a second depth-tested, 97.9 to 98.7% of
  those submitted; about 1,150 a second keep painter's order. No depth clears. Play
  holds 144 fps with it on, and the acceptance test passes.

Verdite1's Z-buffer takes its depths first from its C# writers' packet records
(`PacketDepth`); this port's writers keep none yet, so the address map's depths are
the only source. Recording them in `MapQuads`, `MapTriangles` and `ModelPolygons` is
next if the misses show.

## Not ported yet

Semi-transparency order (`SemiAbr`).

**By eye (the user):** each switch against its `=0`.
