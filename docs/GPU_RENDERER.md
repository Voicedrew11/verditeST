# The retained GPU renderer: findings

Phase 11's working log. The plan is `docs/RENDERER_PLAN.md`; this file holds what
has been read, run and measured for it. Inspection, runtime exercise, numerical
equivalence and the user's judgement are kept apart, as in Verdite3's.

## The map's records and colours

Read 2026-11-10 from the literal `MapQuads` (`func_8001DB64`,
`patches/Draw/MapQuads.cs` and `MapQuadsPackets.cs`), then checked against a new
game standing in map 0: the visible list read `0, 1, 3, 5, 6, 0xE, 0xF, 0x11, ...`,
cell 0's record `00 FF 00 01 01 00 00 FE 41 42 3F 80` (mesh 0, rotation 1, drawn),
the bank pointer at the scratchpad's `+0xC` `0x800FC488`, the rotation table at
`+0x3C4` `0x8018C960`, and the face RGB at `+0x44` `80 80 80`. Read between frames,
so the scratchpad's per-frame words (the corner buffer at `+0x34`) were not live.

**The visible cells.** A u16 count at `0x801B9E14` and a u16 list of cell indices
at `0x801B9F16` (the list `SceneQuads` walks too), each into a table of 12-byte
cell records at `0x8017BED0`:

| offset | what |
|---|---|
| `+0x00` | u8, the mesh's index in the area's bank |
| `+0x03` | u8, `& 3`: the cell's rotation, a quarter turn each |
| `+0x04` | u8, `& 3`: 0 hands the cell to `SceneQuads`/`SceneTriangles`, drawn flat and unfogged; the map routines skip it (its vertices still advance the buffer below) |
| `+0x08` | u8, the cell's X: origin `X = b8 << 12` (world units; cells are 0x1000 wide) |
| `+0x09` | u8, the cell's height: `Y = (b9 << 12) + ((b11 - 0x80) << 7)` (larger is lower) |
| `+0x0A` | u8, the cell's Z: `Z = b10 << 12` |
| `+0x0B` | u8, a fine height, biased by 0x80, in steps of 0x80 |

**The projected corners.** The scene's preparation passes (`ScenePrepare1..4`, which
write no packets) project every visible cell's vertices into a buffer the
scratchpad's `+0x34` points at, 8 bytes a corner: `+0` the screen XY (`SXY`), `+4`
the depth (`SZ`, s16), `+6` the depth cue's factor (u16, the GTE's `IR0`). The walk
advances it by the mesh's vertex count times 8 after each cell. So the map's
projection is done once a frame on the CPU, before any face is read.

**The mesh**, a 0x28-byte descriptor at `bank + 0xC + index * 0x28`, the bank at
the scratchpad's `+0xC`; its offsets are from `bank + 0xC`:

| offset | what |
|---|---|
| `+0x00` | offset of the vertex data (to the scratchpad's `+0x3C`) |
| `+0x04` | the vertex count |
| `+0x08` | offset of the normals (to the scratchpad's `+0x24`) |
| `+0x10` | offset of the faces |
| `+0x18` | u16, the triangle count; the quads follow the triangles' 0x14-byte records |
| `+0x1C` | u16, the quad count |

**A quad**, 0x1C bytes:

| offset | what |
|---|---|
| `+0x00` | u32, the packet's CLUT and first UV |
| `+0x04` | u32, the texture page and second UV |
| `+0x08` | u16, the third UV |
| `+0x0A` | s8, a depth bias added to the order key |
| `+0x0B` | u8, the GP0 code, `| 0x3C` (GT4) |
| `+0x0C` | u16, the fourth UV |
| `+0x10` | u16, the face's normal, a byte offset into the normals |
| `+0x12..+0x18` | four u16 byte offsets into the projected corners |

**A triangle** (`MapTriangles`, `func_8001D2A8`), 0x14 bytes: `+0x00` u32 the CLUT
and first UV, `+0x04` u32 the texture page and second UV, `+0x08` u16 the third UV,
`+0x0A` s8 the depth bias, `+0x0B` the GP0 code `| 0x34` (GT3), `+0x0C` u16 the
normal's offset, `+0x0E..+0x12` three u16 offsets into the projected corners. The
same shape as a quad's, one corner and one UV fewer.

**The projection** (`ScenePrepare4`, `func_8001CDF8`). Per visible cell, the GTE's
rotation is one of four matrices at `0x8017BDB8` (0x20 bytes each), picked by the
cell's rotation, and its translation is the cell's entry in a 16-byte-a-cell table
the caller passes in `a0`, built by `ScenePrepare3` (`func_8001CB88`): the cell's
origin minus the eye (the player's `+0x248`, `+0x24C` plus the s16s at `+0x2CE` and
`+0x2D2`, `+0x250`), halved until each axis fits an s16 with the count of halvings
kept at the entry's `+0xC`, then turned into view space by `MVMVA` with the view's
rotation. Cell 0 in map 0 (`41 42 3F`, fine `80`) is at `(0x41000, 0x42000,
0x3F000)`, beside the player at `(0x40000, 0x415C0, 0x3F000)`, so the bytes are
world cells with no bias taken off. The four matrices are the view's rotation times each
quarter turn, rebuilt every frame: standing in map 0 at heading 0, the first read
`[0 0 -1; 0 1 0; 1 0 0]` in 4.12. The vertices are 8-byte `SVECTOR`s in the mesh's
own space, three to an `RTPT`.

**The fog is linear in depth, computed on the CPU.** For each corner `ScenePrepare4`
writes `IR0 = (SZ - near) * 4096 / (far - near)` over the word at `+6`, truncating,
with near and far from the scratchpad's `+0x64` and `+0x68` (equal to the record's,
below). It is not the GTE's `DQA`/`DQB` curve. Two quirks, found by the map
reference: the result is kept in 16 bits and read back signed, then 0 when negative,
so a corner far enough past the far depth to pass 32767 wraps to no fog; and a mesh's
vertices go through `RTPT` three at a time, the last one or two through `RTPS` singly,
and only those are held under 4066 (to 4065). Past 4096 `DPCS` carries the colour
beyond the far colour, clamped per channel.

**The lighting record**, pointed at by the scratchpad's `+0x3C4` (`0x8018C960` in map
0, from a table indexed at `0x80165090 + 0x18000 + 0x6E08`), read standing in map 0:

| offset | what | map 0 |
|---|---|---|
| `+0x00` | four light matrices, 0x14 bytes each, one per quarter turn | |
| `+0x50` | the colour matrix (`LCM`, control 16 to 20); the fifth word's top half is not part of it | `0x800 0x800 0x900` ... |
| `+0x64..+0x66` | the back colour, a byte each, `<< 4` (control 13 to 15) | `80 80 80` |
| `+0x68` | s16, the fog's near depth | `0x4000` |
| `+0x6A` | s16, the fog's far depth | `0x5000` |

`ModelStep17D28` (`func_80017D28`) blends two such records for a model by a weight,
so the models' light can lie between records; not read further.

**The colour.** Per face, `NCCS` with the face's one normal (in the mesh's space,
lit by the light matrix for the cell's rotation) and the RGB at the scratchpad's
`+0x44`: one lit colour a face. Then per corner, `DPCS` of that colour with the
corner's `IR0` toward the **far colour**, which the scene sets from the byte at
`0x8017BE8C`: `(5, 5, 5)` (`<< 4`, so nearly black), `(6, 4, 2)` when `& 2` (the
flickering light, which also sets `+0x44` from `rand()`), `(2, 6, 6)` when `& 4`;
`+0x44` is `0x80` grey otherwise. The fog is all that makes a GT4's corners differ.

The order key is the largest of the four `SZ` shifted right by 5, plus the bias;
4096 or more, or under the near limit at the scratchpad's `+0x8C`, drops the face.
A face whose `NCLIP` areas pass 16384 goes to the near path (`NearQuad`, libgte's
division); the rest take the bulk path. Two bits of `0x8017BE8C`, the byte whose
`& 2` also turns on the light's flicker (`GAME_INTERNALS.md`), set the frame
pointer to `0x8001DAE0` (`& 2`) or `0x8001DB20` (`& 4`), used by the packet half; not
read.

**For the retained map** (`RENDERER_PLAN.md`, Step 3): the source geometry is
per mesh in its own space, placed per cell by a quarter turn and the cell's origin;
the lighting is one normal a face against the record's light and colour matrices
and back colour, which fits the shared `GteLightMap`; the fog is linear between the
record's near and far depths toward a far colour from three fixed choices, which is
the shape of the shared `LinearDepthCue` (Verdite3's near/far cue), with this game's
own arithmetic (full `SZ`, truncating division, the top saturated by `DPCS`). Still
to read: what the halving count does to the vertices,
the record table's index, and when the record changes within an area.

## The scene census

`ST_SCENE_CENSUS=1` (`patches/SceneCensus.cs`, Step 0.1 of the plan): after every
main-loop stage 14, each packet in the ordering table goes to the innermost call of
`GeometryProbe.Sites` whose span of the packet buffer holds it, then to that call's
domain; a packet in no span is counted as unattributed. Inside the scene the map's
routines move their own cursor, the scratchpad's `+0x10`, and the scene writes it
back to the frame's record only at its end, so calls inside the scene are spanned by
that cursor. Without it every map packet read as the scene's.

A caller's span starts where its first child's does when that child writes first, so
a packet goes to the narrowest span holding it (the scene's own quads were read as
the scene's until this tie was broken).

Measured 2026-11-10 over `scripts/session.py` (map 0: stand, walk, turn, the menu, a
death), a frame:

| domain | packets | kinds |
|---|---|---|
| map bulk (`MapQuads`, `MapTriangles`) | 140 to 226 | GT4 128 to 205, GT3 12 to 31 |
| models | 34 to 96 | FT4 most, FT3, GT3, GT4; FT4S and FT3S when a creature is near |
| HUD | 22 | 10 mode packets, 8 sprites, 2 F4, 2 FT4 |
| the scene's own (`SceneQuads`, `SceneTriangles`) | 1 to 32 | FT4, an FT3 |
| effects (`Effect246E4`) | 0 to 7, in the death | FT3, FT4 |
| tints, load fade | under 1 | FT4S |
| libgpu's own | 2 | static packets at `0x80087D6C` (empty) and `0x80087D80` (a zero-sized VRAM copy), beside `ResetGraph`'s block |
| unattributed | **0** | |

Not seen in map 0: the near path (`map near`), the held models and the text. Every
packet of every frame in the session has an owner: Step 0.1's gate for the contexts
this session reaches.

## The map reference

`ST_MAP_REFERENCE=1` (`patches/Render/MapReference.cs`, the arithmetic in
`MapLight.cs`, the face shapes in `MapFaces.cs`): Step 2's reference for the
map's faces. Before `MapTriangles`, `MapQuads`, `SceneTriangles` or `SceneQuads` runs, every visible cell's faces of
its shape are worked out in plain C# from the records alone, without the
GTE: the facing (`NCLIP` of corners 0-1-2 and 3-1-2), the drop by depth, the near
path's choice, the order key, each corner's fog factor from its depth, the face's
`NCCS` colour and each corner's `DPCS` colour, and every word of the GT4. After it,
the packets it wrote are compared word by word, with each tag's link and each key's
head in the table. A near face's packets are written by libgte's split
(`func_80075EC4` for a triangle, `func_80076884` for a quad); the reference steps over them by the cursor that call moves, and
does not model them.

Measured 2026-11-10 over `scripts/session.py` (map 0), 4,974 frames: **0
differences** in any count. 892,747 bulk quads matched whole, 3,570,988 corner fog
factors, the back colour and colour matrix against the record every frame, every tag
and 182,969 heads; 367,547 near faces stepped over, each at the cursor. With the
triangles added, a second run: 888,372 GT4s and 102,176 GT3s matched whole, 3,860,016
fog factors, 365,821 near quads and 24,959 near triangles stepped over, 0 differences.

**The scene's own faces are the map's too.** `SceneQuads` and `SceneTriangles` walk
the same visible cells, meshes and face records, but draw only the cells whose
`+4 & 3` is 0, the ones the map routines skip, as FT4 and FT3: one `NCCS` colour a
face (code `| 0x2C`, `| 0x24`), no fog, no near path (a face of any size is a bulk
packet), the same facing, keys and drop. Added to the reference as two more entries
of `MapFaces`; a third run: 38,821 FT4s and 469 FT3s matched whole beside 894,112
GT4s and 102,263 GT3s, 0 differences. So the retained map is one mesh set with a
per-cell shading choice, not two kinds of geometry. A negative control (one bit off in the reference's red)
failed at the first quad. Along the way:

- the fog's two quirks above;
- the near path runs every frame in map 0, about 74 faces a frame (near the
  player, through `func_80076884`); its packets count as map bulk in the census, whose
  "map near" is the scene's other routines, `SceneStep1E5A0`/`1EEAC`;
- a split that clips a face away entirely writes nothing and leaves the key's head.

Not yet covered: the flicker's per-corner calls when `0x8017BE8C & 6` (colours
skipped and counted then; not reached in map 0), the near splits' own arithmetic
(measured from outside: "The near splits"),
and the models.

**Over the tour** (`scripts/tour.py` with `ST_MAP_REFERENCE=1`, 2026-11-10): maps 0, 2,
4, 21, 23, 24, 32, 38, 41, 44 and 46 (45 never loaded this run), about 3 million map
and model faces matched, **0 differences** in any count.

**The flicker's colour** (`0x8017BE8C & 6`, maps 24 and 32): after `DPCS`, each map
corner's colour word goes through `func_8001DAE0` when `& 2`: blue held under 0x56,
then the colour made `(3b, 2b, b)`, warm; else `func_8001DB20` when `& 4`: red held,
then `(r, 3r, 3r)`, cold. The models' fogged writers call copies of the two
(`func_80018440`, `80018480`) after `NCDS`. So under the flicker the fogged colour
keeps only one channel's brightness, recoloured; `MapLight.Flicker`. A second tour
(all 12 maps): 101,487 map GT4s and 42,429 GT3s under the flicker matched whole with
it, and the fogged models' colour words too, **0 differences**.

## The near splits

Measured 2026-11-10 by `ST_MAP_REFERENCE=1` (`patches/Render/NearSplits.cs`) from
outside libgte's division, over `scripts/session.py` in map 0. The writer hands the
division a block: `+0` the level (1 when an `NCLIP` area passes 16384, 2 past 32768;
the triangles reach 3), `+4`/`+8` the screen's 320 and 240, `+0x14` the face's entry
in the ordering table; the cursor goes in `a0` and comes back in `v0`.

| shape, level | calls | write nothing | pieces when drawn |
|---|---|---|---|
| quads 1 | 174,205 | 171,622 | 4 (2,583 calls) |
| quads 2 | 169,563 | 168,720 | 4 (369), 8 (464), 12 (7), 16 (3) |
| triangles 1 | 9,459 | 9,128 | 4 (331) |
| triangles 2 | 10,513 | 10,443 | 4 (46), 12 (18), 16 (6) |
| triangles 3 | 3,536 | 3,536 | never drawn |

- **About 98.5% of near faces draw nothing, and all of them are off the screen**: a
  second run classed each by its projected corners. Every face that drew nothing
  had its whole box outside 320 by 240 (382,219 of them); no face whose box crossed
  the screen came back empty. The ones that drew with their box outside 320 by 240
  (1,593) lie in the 16:9 margins `NearScreen` widens the division to. 12,628 of the
  empty ones (3.3%) have a corner at depth 0 or under 256, crossing the eye's plane,
  their projection saturated at the GTE's 1024: the only faces a GPU path that clips
  at the near plane might draw where the game draws nothing, a sliver at most.
- Pieces are the face's own kind (GT4 for a quad, GT3 for a triangle), **a level
  makes a 2 by 2 grid** (4 pieces, 16 at level 2), and they are dropped a quarter at
  a time: the counts are multiples of 4.
- Every piece lies end to end from the cursor and **all go into the face's own key**,
  chained between the key's old head and its new one: 0 exceptions in 387,024 calls.

For Step 4: an unsplit face in the face's key keeps the order exactly; what it
changes is the texture's interpolation (the pieces are affine each) and whatever the
division's clip drops.

## What verify compares

Read 2026-11-10 from `patches/Draw/LiteralVerify.cs`, against Verdite3's
`NativeScene` verify (its `GPU_RENDERER.md`, "Native reference unit"). An earlier
draft of `RENDERER_PLAN.md` said this port's verify compared RAM only; it compares:

- all of RAM but the 0x2000 bytes below the stack pointer at entry;
- the scratchpad, every word;
- every CPU register (`at` to `ra`, `lo`, `hi`), not only the callee-saved;
- the GTE's whole state;
- the interrupt-poll countdown, and the BIOS `rand()` seed (restored, so both runs
  draw the same numbers); a call during which a slow poll ran is counted as
  interrupted, not compared.

**The stack window** is the routine's own frame and its callees': by the MIPS
convention nothing below the stack pointer survives a return (there is no red
zone), so it is dead when the caller resumes. Verdite3 compares an 8 KB window
there; here it is excluded on that argument, which stands as long as no routine
reads below its caller's stack pointer.

**The calls a routine makes**, added 2026-11-10 (`patches/Draw/LiteralCalls.cs`):
both runs record every call into a function the recompiled routines call directly
(77 callees, read from the generated methods' IL at attach, so the list follows the
recompiler), at the callee's entry, as its return address and `a0` to `a3`; the
return address names the call site, so it names the callee. Indirect calls (`jalr`,
through the dispatcher) are compared by count and their last 64 targets, the
dispatcher's own ring. A verify nested in another records its own calls; the outer
one sees the call to it. Installed only when a routine starts in verify.

Measured over `scripts/session.py` with `ST_LITERAL=verify`: 54 routines ran, 130,061
verified calls, **524,055 calls inside them compared, 0 mismatches**. A negative
control (one argument bit flipped in every second record of the C# run) gave a
mismatch at call 0 in each routine that calls anything, so a zero here is a match.
The 26 routines with no calls compared are leaves (`MarkClipSlots`, `MarkPoseSlots`,
`HudNumber`, `HudSprite`, `PanelBorder`, the text routines, ...) or call only on a
branch the session does not reach: `TextureAnimations` (none in area 0), the tint,
the held models, the near path, `ModelExtras2`. Those branches stay unverified for
call order until a context reaches them (Step 0.2's list).

Verify now covers what Verdite3's `NativeScene` verify and its supplemental verifier
do together: Step 2's reference comparison is complete for the paths exercised.


## The retained world

Steps 3 and 4 as built, the map, the models, their poses and blended faces on the
GPU, with what was checked and measured: `docs/GPU_RETAINED.md`.
