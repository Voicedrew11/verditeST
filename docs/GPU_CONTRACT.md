# Step 1: the shared runtime against this game

Phase 11, Step 1 (`docs/RENDERER_PLAN.md`): can the shared retained backend
(`tools/RecompOne/RecompOne.Runtime/Gpu/`) express what Step 2 found this game does
(`docs/GPU_RENDERER.md`, `docs/GPU_MODELS.md`)? **Read 2026-11-10 from the runtime's
public surface only**: nothing is wired, no runtime file is changed, and every "fits"
below is a reading, not a run. The gate (the contract expresses every Step 0
family; the siblings still pass) is not closed.

## What fits

- **The lighting record.** `RetainedScene.Records` keeps 64 records of 52 ints: the
  light matrix at each quarter turn (4 x 9), the colour matrix (9), the back colour as
  the GTE takes it (3), then a fog word, `DQA`, `DQB` and a curve. This game's record
  is the same shape up to the fog: four light matrices pre-turned a quarter turn
  each at `+0x00` (0x14 bytes each), the colour matrix at `+0x50`, the back colour at
  `+0x64` (bytes, `<< 4`). How many records an area has is not read (the scene
  indexes them through `0x8017BE98`).
- **A map corner lit from the record.** `Vertex.Light` with `LightRecord` set carries
  the face's normal and the record and quarter turn: the map's `NCCS` per face with
  the cell's rotation. The neighbour blend bits (`EvenFog`, `NeighbourBlend`) are
  Verdite3's per-tile blend; this game draws a frame's cells from one record, so
  they stay clear.
- **The flat cells** (`SceneQuads`, `SceneTriangles`): one lit colour a face and no
  fog, a vertex with curve 0.
- **The near faces**: whole faces in the face's own key; the division only changes
  texture interpolation and what the screen clip drops (`GPU_RENDERER.md`, "The near
  splits").
- **Models**: `AddMesh`, `AddPose`, `AddInstance` and `AddMainModel` take meshes,
  poses and placements; the model's light matrix (the record's times the model's
  rotation) and its blended `LCM`/`BK` are per-instance inputs, not read against the
  instance record's fields yet.

## What does not fit, and what each needs

| this game | the runtime | needs |
|---|---|---|
| fog factor `(SZ - near) * 4096 / (far - near)`, full SZ, truncating, kept in 16 bits signed then 0 when negative; past 4096 extrapolated and clamped per channel | `LinearDepthCue` (curve 5): quarter-depth quantised, `* 16384`, clamped to 0..7951; a sibling's formula | a new curve, game-neutral: linear, scale 4096, no quantising, the 16-bit wrap, no upper clamp on the weight (the channel clamp does it) |
| the fog's near and far in the record (`+0x68`, `+0x6A`) | `DQA`, `DQB` and a fog word | the record's fog slots carry near and far for the new curve |
| models' fog held at 4065, and the map's last one or two vertices of a mesh | none | a per-vertex cap, or the factor baked per corner |
| a model face's one `IR0` (the corners' mean; corner 0 twice in a smooth fogged triangle; a GT4's fourth corner on f0 alone) | a cue per corner (`Dqa`, `Dqb`, curve) | the factor baked per corner by the port, the per-face rules applied on the CPU when the instance is published |
| the flicker's recolour after the fog (one channel held under 0x56, the colour made `(3b, 2b, b)` or `(r, 3r, 3r)`) | none | a post-cue colour rule, game-neutral (a channel picked, capped, scaled into the three), or the flicker's frames left on packets as a counted fallback |
| the mixed writers' UV offset (`+0x98`) on near faces only | none needed if near faces are drawn whole | the offset applied to the whole face, which is a change; read what `+0x98` is first |

## The choice this leaves

The two pieces that need a runtime change (a linear 4096 curve and a post-cue
colour rule) are small, game-neutral additions in the shape of `LinearDepthCue`.
Recommended order: the curve first (every fogged face needs it), with
`MapLight.FogFactor` and `MapLight.Fogged` as its CPU reference; the flicker's frames
on packets as a counted fallback until the rule is added. Each is a fork patch in its
own commit, and the siblings' acceptance runs must read the same after it.
