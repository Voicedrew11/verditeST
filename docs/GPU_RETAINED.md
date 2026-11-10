# The retained world: the map and models on the GPU

Phase 11, Steps 3 and 4 (`docs/RENDERER_PLAN.md`), built 2026-11-10. The findings it
rests on are in `docs/GPU_RENDERER.md` (the map) and `docs/GPU_MODELS.md` (the
models). Inspection, runtime exercise, numerical equivalence and the user's
judgement are kept apart: everything below is the first three. **Nothing here has
been looked at by anyone**, and so it is off by default: `ST_GPU_WORLD=1`, or
`Settings ▸ Testing ▸ Retained GPU world`.

## How it is built

| file | job |
|---|---|
| `patches/Render/GpuWorld.cs` | the switch, the retained frame's lifetime, the probe's line and the GPU's time by pass |
| `patches/Render/RetainedMeshes.cs` | the face records (FT3, FT4, GT3, GT4) as corners in the runtime's mesh store, and still vertices as rigid poses; one decoder for map and models |
| `patches/Render/RetainedMap.cs` | every visible cell an instance of its mesh |
| `patches/Render/RetainedModels.cs` | every model part an instance at its step's entry |
| `patches/Render/RetainedPoses.cs` | a blended pose as a kept key and delta and a per-frame weight |
| `patches/Render/RetainedBlend.cs` | blended faces handed to the backend at the table entry the game links them at |
| `patches/Render/Flicker.cs` | the flickering light's recolour, as runtime `0124`'s rule |
| `patches/Render/PacketFilter.cs` | a writer's packets taken back off the ordering table |
| `patches/Render/RetainedCheck.cs` | `ST_GPU_WORLD_CHECK=1`: the GPU's numbers against the game's, every frame |

**Instances, not a static map.** The runtime's retained backend was built for
Verdite3's 80 by 80 tile grid (`SetStatic`, halves, chunks). This map is cells on a
0x1000 grid with a byte coordinate each, each cell a mesh of the area's bank at a
quarter turn: the shape of a model. So the map and the models both go through the
backend's model instances (`AddMesh`, `AddPose`, `AddInstance`), and nothing of the
tile grid is used.

**The frame's world is its view space.** The scene's entry (`func_80020A80`) begins a
retained frame with the identity view, and every instance is placed by the very
rotation and translation the GTE projects it with: a cell by `0x8017BDB8 + turn * 0x20`
and the scene's table of translations (`ScenePrepare4`'s `a0`), a model part by the
GTE's own matrix at its step's entry. Nothing world-sized passes through a float,
and the carried camera, the carried models and the widescreen projection are
whatever the game's matrices already hold. The table walk draws the frame at slot 1,
ahead of every packet, and the Z-buffer tests every packet after against it.

**The packets.** The face routines still run as the game wrote them (the six map
routines, the twelve model writers, the near splits), so every other effect they have
is the game's; `PacketFilter` then takes what each call linked back off the table
(every changed entry walked from its new head to its old one) and rewinds the cursor.
A frame or a part that cannot be published keeps its packets, counted by reason.

## The map

Per visible cell: the mesh's faces (from the descriptor's `+0x10`, FT3s, FT4s, then
the smooth GT3s and GT4s drawn by `SceneStep1E5A0` and `SceneStep1EEAC`, which the
scene's own flat cells never draw) and its vertices as a rigid pose, both kept by content; the record's light
matrix at the cell's turn, its colour matrix and back colour, the face RGB from the
scratchpad's `+0x44`; fogged on runtime `0123`'s curve 7 between the record's near and
far, the scene's own flat cells (`+4 & 3 == 0`) unfogged. `Tile` instances: no depth
range, and the GPU's near plane and each face's plane against the eye in place of the
game's near split through libgte's division. Under the flicker, `0124`'s rule.

## The models

Every part reaches one of the three steps (`func_800192D4` fogged, `func_80019DC8`
unfogged, `func_8001BF10` mixed) from `ModelPolygons` or from `ModelDraw`'s own
single-part path, with the GTE holding the part's view-space matrix and the model's
light, and the part's posed vertices at the scratchpad's `+0x48`: the bank's own for
`ModelPolygons`, a posed copy for `ModelDraw` (read from the bank instead, a tour put
parts in map 38 1,295 units out). So the step's entry covers every caller: entities,
effects, the second list and cels; not the held models, which stay packets in the
front table, drawn over the world (`HeldModelLayer`, "The arm" in `docs/SMOOTHING.md`). The instance: the four runs FT3,
FT4, GT3, GT4 from the descriptor, the GTE's matrix and light, the face colour the
steps set (`0x80`, or under the warm flicker `0x88 - (w >> 13)` with `& 0x10`, else
`0xA0 - (w >> 11)`, `w` the light's random word at `0x8017BE88`), fogged on curve 6
(held under 4066, as every model vertex is) for the fogged step and for the mixed one
unless `0x8017BE8C & 0x10`. Mixed parts are `Tile` instances; the others keep the
game's depth range, `+0x80 < key < +4`, as view depth `(lo + 1) * 8 <= z < hi * 8`.
Runtime `0103`'s near clip is on (`ST_GPU_MODEL_CLIP=0` compares).

## The poses

A part's vertices on the GPU never come from a per-frame upload when they need not:

- **a blended pose**: libgte's `gteMIMefunc` (`func_80074A04`, called from
  `func_80033CD4`) adds `(delta * fraction) >> 12` to each vertex in place, in 16
  bits, which is the runtime's pose blend to the bit. At its entry the vertices still
  hold the key; key and deltas go into the pose store once, kept by content, and the
  part is drawn with that pose and the fraction as its weight. Before it is used the
  part's vertices are checked to be exactly what the blend made;
- **still vertices**: a rigid pose, kept by address and content, while the address's
  content has changed no more than twice;
- otherwise the vertices are uploaded for the frame, counted.

## Blended faces

Drawn by the backend where the table walk would have drawn their packets
(`RetainedScene.AddBlendFace`, runtime `0085`), each at the table entry the game links
it at, from the corner depths the game projected the same frame: the map's by its
deepest corner (`>> 5`, plus the record's bias, dropped at 4096 or under the near
limit), the models' by their mean (dropped outside `+0x80 < key < +4`). The walk
passes this game's 0x1000-entry table from its far end and reads a key `K` as entry
`0x1FFF - K` from that end, so entry `k` is key `k + 0x1000`. A part whose caller
blends every face (`flags & 4`: the effects' half and half, the cels' quarter added)
is drawn whole as blended faces at the caller's mode (`TwinMode`). The walk drew
them only under `0079`'s reordering, which needs packet depth records this port has
not got; runtime `0125` lets it draw them without. `ST_GPU_BLEND=0` keeps blended
faces as packets.

## Checked against the game

`ST_GPU_WORLD_CHECK=1` compares, every published frame, what the GPU is given with
what the game computed: each vertex placed as the world shader places it against the
corner buffer the game filled (screen XY, depth), each corner's fog factor on the
GPU's curve against the factor the game wrote, each map face's light as the shader
lights it (floats) against `MapLight.Lit`, and which model faces the GPU's depth range
keeps against the table's. Not checked: the GPU's own output, the drawn picture.

Over `scripts/session.py` in map 0, the map: 4,070,317 vertices, depth worst 1 unit;
on screen, 930 (0.02%) further off than a pixel and two units of the GTE's whole-unit
truncation (worth `H / z` pixels each near the eye), worst 2.1 px; fog factors the same
but 10,963 (0.28%) the game held at 4065 (a mesh's last one or two vertices, projected
singly); 2,442,930 faces lit with **0** a step off. The models: 933,476 vertices, depth
worst 1 unit, 1,663 (0.18%) further off on screen; fog factors **0** off; 1,050,652
faces kept alike, 24 by the game only.

**Over the tour** (`scripts/tour.py`, all 12 maps with an arrival point, with
`ST_GPU_WORLD=1`, the probe, the check and the census on; 2026-11-10): 0 GPU draws
missed, 0 fallbacks, 0 parts uploaded a frame; about 889,000 blended faces published
and all drawn; of 4,104 blended poses 4,065 reused a kept key; 109,311 parts drawn as
rigid poses. The map, in the largest of the three logs: 12.0M vertices, depth worst 1
unit, 750 further off on screen; fog exact but 14,122 held at 4065; 7.16M faces lit,
7,027 a step off, none more. The models: 7.6M vertices, depth worst 1, 5,513 further
off; fog **0** off; 8,224,356 faces kept alike, 142 by the game only, 1,631 by the GPU
only.

**The census** then showed one hole: in maps 44, 45 and 46, 150 to 430 GT3s and GT4s
a frame of "map near", and as many of "map bulk", still packets. `SceneStep1E5A0` and
`SceneStep1EEAC` are the map's **smooth faces**: a cell's third and fourth runs, GT3
records (0x18 bytes, counted at the descriptor's `+0x1A`) and GT4 records (0x20, at
`+0x1E`), after its FT3s and FT4s, each corner with its own normal, lit by `NCDS` with
the same light, colour and fog as the flat ones and keyed by the deepest corner; the
near splits they call were the "map bulk". Only maps 44 to 46 have them; the scene's
own flat cells never draw them. The record is the models' GT3 and GT4, so the map's
mesh now carries all four runs and the two routines are filtered like the others.
Over maps 0, 44, 45 and 46 again: every frame published, 0 fallbacks; the map 7.86M
vertices, depth worst 1, 128 further off on screen, fog exact but 2,188 held at
4065, 18.1M normals lit (one a flat face, one a corner a smooth one), 57,472 (0.3%) a
step off, none more; the models 4.32M vertices, 221 off, fog 0 off, 3.58M faces kept
alike, 0 by one side only. **What is left as packets** in those maps is 2 to 4 a
frame, libgpu's own and the load fade; in map 0, 24, the HUD's. No world packet is
left on the table anywhere the tour reaches.

## Cost

Over `scripts/session.py` (2026-11-10). CPU, `ST_DRAW_TIMING=1`, stage 14 to the swap,
the steady windows' p95: 0.65 to 0.77 ms on packets, 0.85 to 0.96 ms retained (the
face routines still run under the filter). GPU, `ST_GPU_TIMES=1` (runtime `0084`), a
present: packets 0.17 to 0.22 ms; retained 0.06 to 0.09 ms of what is left as packets
plus 0.13 to 0.15 ms of the retained world. Both well inside a 144 Hz frame.

## What differs from the console, by construction

- **The far colour.** The GTE fogs toward `(5, 5, 5)` (`(6, 4, 2)` and `(2, 6, 6)` under
  the flicker), the GPU toward black: at most 6 of 255 at full fog.
- **A map mesh's last one or two vertices** are held at 4065 by the game, not by the
  GPU's curve 7 (0.28% of corners).
- **Faces within 128 units of the eye**, which the map routines drop by key (0.24% of
  map faces), the GPU draws, clipped at its near plane.
- **The near faces** are drawn whole and clipped at the GPU's near plane, not split by
  libgte's division into affine pieces; the mixed writers' `+0x98` UV offset, applied
  to their near faces only, is not applied.
- **A model face's fog** is the corner's own, interpolated, where the writers use one
  `IR0` for the face (the mean, corner 0 twice in a fogged GT3, a GT4's fourth corner
  on f0 alone).
- **Precision**: floats against the GTE's whole units, a pixel or two near the eye.

## Not done

- **The user's eye**, on everything: walls, floors and creatures against the packets
  (`ST_GPU_WORLD=0`), the fog at distance, maps 24 and 32 under the flicker, the effects
  and cels (blended), the arm over the world while swinging (Triangle), the menu's world, the near walls.
- **The face routines still run.** Their packets are taken off, not never made: the
  CPU still spends what it did (about 0.15 ms more at p95, the filter included). A
  native enumeration that publishes without running them is Verdite3's end state
  and the plan's. What they leave besides packets was measured
  (`ST_WRITER_EFFECTS=1` over the session, every 16th call of the six map routines
  and the three model steps, all of RAM but the packet buffers and tables, the
  scratchpad, the GTE and the saved registers): **no RAM of the game's**, only what
  the vblank handler, the pad and the sound library write while a routine polls
  (`GAME_INTERNALS.md`, the wall-clock words); the saved registers kept. The rest is
  scratchpad and GTE: the map routines' working words (`+0x24`, `+0x2C`, `+0x34`,
  `+0x3C`, `+0x44`, `+0x70`, `+0x74`, `+0x80..+0x88`, `+0x184`, `+0x1E4`), three
  statistics (`+0x1B4` faces walked, `+0x1BC` faces written, `+0x1C0` near pieces,
  `1 << 2n` a split), and the GTE's data registers and control 0 to 12 (the cell's
  matrix and light); the model steps' `+0x1C`, `+0x24`, `+0x60`, `+0x70`, `+0x8C`,
  `+0x90`. The scene rewrites `+0x34` before each routine, and copies `+0x1B0..+0x1E0`
  to `0x801653AC..` after the last, which `FrameHead` zeroes every frame: statistics
  no reader of has been found. Still to show before the routines can be skipped:
  that the code after the scene (the models first) reads none of the GTE registers
  or scratchpad words they leave before writing them.
- **Step 5's enhancements** (per-pixel light, fog by pixel depth, SSAO, anisotropy,
  mipmaps) through the retained world: untried.
- **Step 6** (a second view) and `VS.EXE`'s own loop.
- **The siblings' acceptance runs** for runtime `0123` to `0125`, before they are
  pushed to the fork.
