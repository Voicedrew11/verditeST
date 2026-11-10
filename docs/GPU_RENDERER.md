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
| `+0x04` | u8, `& 3`: 0 skips the cell's faces (its vertices still advance the buffer below) |
| `+0x08..+0x0A` | three bytes biased by 0x40 (`41 42 3F` in cell 0): the cell's position, probably, as the door records' arrival cells are; not read in code yet |

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
| `+0x18` | a u16 count of 0x14-byte records before the quads: the triangles, probably |
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

**The colour and the fog.** The map is lit and fogged by the GTE each frame, not
baked:

1. The cell's rotation loads the GTE's **light matrix** (control 8 to 12) from a
   table of four 0x14-byte matrices at the scratchpad's `+0x3C4`.
2. Per face, `NCCS` with the face's one normal and the RGB at the scratchpad's
   `+0x44` (its code byte replaced): one lit colour for the whole face.
3. Per corner, `DPCS` of that colour with the corner's `IR0`: the GTE's **depth cue**
   toward the far colour. This is all that makes the GT4's corners differ.

The order key is the largest of the four `SZ` shifted right by 5, plus the bias;
4096 or more, or under the near limit at the scratchpad's `+0x8C`, drops the face.
A face whose `NCLIP` areas pass 16384 goes to the near path (`NearQuad`, libgte's
division); the rest take the bulk path. Two bits of `0x8017BE8C`, the byte whose
`& 2` also turns on the light's flicker (`GAME_INTERNALS.md`), set the frame
pointer to `0x8001DAE0` (`& 2`) or `0x8001DB20` (`& 4`), used by the packet half; not
read.

**For the retained map** (`RENDERER_PLAN.md`, Step 3): the source geometry is
per mesh, placed per cell by a rotation and the cell's position. The lighting is
one normal a face against a rotated light matrix, which fits the shared
`GteLightMap`; the fog is the GTE's own depth cue (`DQA`/`DQB` and the far colour),
a curve `GteLightMap` already numbers. Still to read: the vertex record and how a
cell's position is applied (in the preparation passes), the triangles' 0x14-byte
record, and the two flags.
