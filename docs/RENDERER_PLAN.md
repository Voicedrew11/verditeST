# Plan: native scene submission and the retained GPU renderer (Phase 11)

Prepared 2026-11-10 from Phase 5's inventory (`docs/GEOMETRY.md`). **This is
planned work, not a record of anything done.** Findings go in `docs/GPU_RENDERER.md`
(new), `GEOMETRY.md`, `PICTURE.md` and `GAME_INTERNALS.md`, not here; strike a
step through when its gate closes.

**The model is Verdite3**, the sibling that is a recompilation like this one: its
`docs/GPU_RENDERER.md`, built from `../KFII-PC/docs/VERDITE3_GPU_PLAN.md`. Not
Verdite1's renderer plan: Verdite1 has a decompilation beside it, and its steps lean
on reading source this port does not have. Verdite3's rules hold here:

- **Build the final path once.** Hand-written C# owns the scene's enumeration and
  submission; persistent GPU meshes and pose data supply the geometry; the GPU
  projects, lights, fogs and depth-tests. A native walk that still calls recompiled
  submitters is not the end state.
- **Packets stay** as the numerical reference and the explicit fallback. A
  fallback is counted and attributed by caller and reason, never silent.
- **Inspection, runtime exercise, numerical equivalence and the user's judgement
  are four different claims.** None implies the next; each is reported apart.
- **Presentation is owned by caller domain**, not by which assembler it reaches:
  the HUD, text, menu chrome, tints and fades stay packets; a world redrawn behind
  a menu uses the world renderer.
- **No fact carries over.** Copy mechanisms; re-derive every number from
  `SLUS-00863`.
- Work on the shared runtime's retained backend (`RetainedScene`, `GpuRetainedMain`,
  `GteLightMap`, `LinearDepthCue`, `DistanceFade`, `NeighbourBlend`, `AoGeometry`,
  `tools/RecompOne/RecompOne.Runtime/Gpu/`); read their APIs before adding any. A
  runtime change is game-neutral, in its own commit, and leaves the siblings'
  acceptance tests reading the same.

## Where this port stands (2026-11-10)

- **Inventory**: every call stage 14 makes, what each writes, and the address
  map's coverage (99.98% of the scene's packets, 100% of the models'). Every
  presenting loop is named (`GEOMETRY.md`, "Who presents").
- **Literal C#**: 57 draw routines in `patches/Draw/`, `ST_LITERAL=1|0|verify`, each
  at 0 mismatches over the session and the 12-map tour. Verify compares RAM; it does
  not yet compare the scratchpad, callee-saved registers or the GTE as Verdite3's
  `NativeScene` does.
- **Corpus driving**: `warp <map>` (12 of 36 maps hold an arrival point), the
  beacon with `loop`, `menu` and `loading`, `scripts/tour.py`.
- **On packets**: pacing, the camera and models carried, the picture switches, the
  Z-buffer from the address map, widescreen, and the world behind the menu
  (`MenuWorld`, a second pass of stage 14's world half).

The world's families, as known:

| family | routines |
|---|---|
| frame | `FrameHead` (`ClearOTagR` 0x1000, front table 0x40), swap `func_80017680` |
| camera | `CameraSetup` → matrix `0x8017BD78`, eye `0x8017BE58`, angles `0x8017BE68` |
| map | `Scene`: `VisibleCells`, `ScenePrepare1..4` (no packets; the projection is here), `MapQuads` GT4, `MapTriangles` GT3, near `SceneStep1E5A0`/`1EEAC` through libgte's division (`0x80075EC4`, `0x80076884`) |
| scene's own | `SceneQuads` FT4 (records at `0x8017BED0`), `SceneTriangles` FT3 |
| models | `Models`, `Models2` → `ModelDraw` → `ModelPolygons` (FT3..GT4); `ModelLight` per entity |
| poses | `func_80033CFC`, `func_80074A04`; slots `MarkClipSlots` `0x8016768C`, `MarkPoseSlots` `0x8016780C` |
| view-space | `HeldModel` (0) and (1): the hands, probably; nothing held in a new game |
| effects | `Effect246E4` (vblank timers, `rand`) |
| presentation | HUD (`HudPanels`, `HudHpMp`, `HudNumber`), `PanelBorder`, text, tints, `LoadFade`, the menu's backdrop |
| VRAM | `TextureAnimations` (`MoveImage`/`LoadImage`), the light's flicker `0x8017BE88` |

**Read 2026-11-10** ("The map's records and colours" in `docs/GPU_RENDERER.md`): the
map's cells, meshes and quads, and its colour: `NCCS` per face with the cell's
rotated light matrix, then the GTE's depth cue per corner. **Still unknown**: the
vertex record and how a cell is placed; the triangles' record; the models' light
inputs (`ModelPolygons` uses `MVMVA`/`RTPS`).

## Step 0: close the inventory, establish the baseline

1. A **scene census** (`ST_SCENE_CENSUS=1`), bracketing every world entry point:
   caller, domain, packet kinds and bytes, table positions, and whether native code
   ran. `GeometryProbe` and `MapCoverage` are the start.
2. The contexts not reached yet (`TODO.md`, Phase 5): the card screens, a fight,
   the area modules' own drawing, `VS.EXE`, a held weapon. Listed as open, not
   covered.
3. A reference performance figure with the GPU path off, so a zero with it on
   cannot mean a probe failed to attach.

**Gate**: every known emitter has an owner and a verification route.

## Step 1: the shared-runtime contract

Check the retained backend against this game's face types (FT3..GT4, semi forms),
its two tables, its fog and its light inputs; choose the map's storage and update
strategy once. Shared changes are fork patches.

**Gate**: the contract can express every Step 0 family; the siblings still pass.

## Step 2: native submission and references

A `NativeScene`-style unit: replace hooks on the map walk, the model list and the
submitters, literal to the register, scratchpad, RAM, GTE and call order, with a
verify that compares all of them (Verdite3's verifier, extended past RAM). The
reference records transforms, normals, unclamped light, fog, facing decisions and
table keys at the original operations, as instrumentation, not a renderer.

**Gate**: references match the recompiled routines on everything above.

## Step 3: retained map, meshes, poses

1. **The map** as a world-space mesh from the cell records, uploaded once an area
   settles (`loading` false), with texture pages, CLUTs and colours; dirty updates
   for anything that rewrites cells mid-area (doors are object records here, so
   read whether any cell changes at all).
2. **Model meshes** cached per bank, instances published by native `ModelDraw` for
   both model lists and the hands.
3. **Poses** on the GPU from cached keys and deltas, with the carried clip time;
   every guest reader of the posed buffers accounted for before their writes are
   skipped, and a materialise for any packet consumer.
4. **The carried camera and placements** for the presented frame; `MenuWorld`'s
   pass a scene too (Verdite3's `GpuWorld.SceneIn`).

**Gate**: stable assets upload once per area; a pose change uploads no vertices;
GPU pose results match the native integers, including negative deltas and wraps;
unknown data is a counted fallback. `ST_GPU_WORLD=shadow` exercises all of it
beside the packets before `1` suppresses any.

## Step 4: the main view on the GPU

Opaque map and models, then the near pieces (read the division's arithmetic
before replacing it with unsplit triangles: UV truncation and order change), all
blend modes, the effects, the front table's order, the hands over the world, and
packet presentation over everything.

**Gate**: every face type and submit path has a GPU path and an exercised check;
residual world packets are listed by caller and reason.

## Step 5: lighting, fog and the buffers

The game's own light and fog first, exactly (numerical checks at corners and
interior depths; the light's flicker kept as the game's), then the enhancements,
each a switch off until judged and live in Settings ▸ Testing: per-pixel lighting,
fog by pixel depth, SSAO with geometry normals, anisotropy and mipmaps,
enhancement distance, neighbour blending if the map has per-cell light levels.

**Gate**: every eligible family reaches the depth, normal and surface buffers;
then the user judges.

## Step 6: second views and water

A second camera proves the caches are view-independent and cannot touch the
simulation, sound, cels or guest matrices. Water and reflections only if the
texture animations or the data show water; classified from this disc's data.

## Step 7: whole-game coverage and adoption

The tour over every reachable area with the census at no unexplained fallback;
the unreached listed. Default on only after the user's judgement.

## First move

Step 0.1 and the first unknown together: read the map's cell records from
`patches/Draw/MapQuads.cs` (the record per visible cell, vertex and face layouts,
where each corner's colour comes from), and start the census from `GeometryProbe`.

## Sources

- `../verdite3/docs/GPU_RENDERER.md` and `GPU_RENDERER_HANDOFF.md`.
- `../KFII-PC/docs/VERDITE3_GPU_PLAN.md` (the plan Verdite3 was built from).
- `docs/GEOMETRY.md`, `SMOOTHING.md`, `PICTURE.md`, `WIDESCREEN.md`.
