# The models' light and fog: findings

Phase 11's log for the models, beside `docs/GPU_RENDERER.md` (the map). Read
2026-11-10 from the literal transcriptions in `patches/Draw/` and the generated
writers; inspection, except "The face writers", whose unfogged and fogged eight are checked
numerically.

## The chain

`Models`/`Models2` → `ModelDraw` (`func_80022678`) → `ModelPolygons`
(`func_80022010`) → one of three model steps by the mesh's flag byte → one of four
face writers by the face's kind. All of it is in the main executable.

**Vertices** (`ModelPolygons`, from `0x80022530`): one at a time through `RTPS`, the
screen XY and the depth kept 8 bytes a vertex as the map's are, and the fog factor
computed on the CPU from the depth with the scratchpad's near and far (`+0x64`,
`+0x68`), the same formula as the map's, **held under 4066 for every vertex** (the
map holds only a mesh's last one or two; "The map's records and colours"). So a model
reaches at most 4065 of 4096 toward the far colour.

**Faces**: the mesh's flag byte picks the step, the step picks the writer:

| flag | step | light | writers (by call site) |
|---|---|---|---|
| `0x80` | `ModelStep192D4` (`func_800192D4`) | `NCDS`/`NCDT`: lit and fogged | `func_800184C4`, `80018828`, `80018BA0`, `80018F48` |
| `0` | `ModelStep19DC8` (`func_80019DC8`) | `NCCS`/`NCCT`: lit, no fog | `func_800194C4`, `800196C0`, `800198F4`, `80019B34` |
| `0x40` | `ModelStep1BF10` (`func_8001BF10`) | both, two faces' worth each | `func_8001A038`, `8001A6EC`, `8001AE7C`, `8001B5DC` |

A writer with one normal (`NCDS`, `NCCS`) colours a flat face; three (`NCDT`,
`NCCT`) a Gouraud one, the fourth corner of a quad taking an `NCDS`/`NCCS` of its
own. The flat FT3 writer `func_800194C4` uses code `| 0x24` and length 7, the map's
FT3 shape. Each row runs FT3, FT4, GT3, GT4 in that order: checked for the first two rows
(below), inferred for the mixed row from its order and its GTE calls.

## The face writers

Each writer takes `a0` the face count (from the mesh descriptor: `+0x18` flat
triangles, `+0x1C` flat quads, `+0x1A` smooth triangles, `+0x1E` smooth quads), `a1`
the records (the scratchpad's `+0x1C`, which each writer leaves past its last face),
`a2` the ordering table, `a3` the caller's mode: `& 2` the semi-transparency bit of
the GP0 code, `& 0x600000` the blend bits or-ed into the texture page word.

| writer | record | normals | corners |
|---|---|---|---|
| FT3 `func_800194C4` | 0x14, the map's triangle record | `+0xC` | `+0xE`, `+0x10`, `+0x12` |
| FT4 `func_800196C0` | 0x1C, the map's quad record | `+0x10` | `+0x12` to `+0x18` |
| GT3 `func_800198F4` | 0x18 | `+0xC`, `+0x10`, `+0x14` | `+0xE`, `+0x12`, `+0x16` |
| GT4 `func_80019B34` | 0x20 | `+0x10`, `+0x14`, `+0x18`, `+0x1C` | `+0x12`, `+0x16`, `+0x1A`, `+0x1E` |

All share `+0`/`+4` the CLUT/UV and page/UV words, `+8` (and a quad's `+0xC`) the
other UVs, `+0xA` the depth bias, `+0xB` the code bits; a smooth record pairs a
normal with each corner. The corners come from the scratchpad's `+0x40`, the normals
from `+0x24`, the face colour from `+0x60`. Unlike the map: **one** `NCLIP`
(corners 0-1-2) even for a quad; the order key is the **mean** depth `>> 5` (a
triangle's by a multiply by `0x55555556`, a quad's as the sum `>> 7`) plus the bias,
where the map takes the largest; the face is dropped unless the scratchpad's `+0x80`
< key < its `+4`. The packets are the map's FT3, FT4, GT3 and GT4 shapes.

**Checked** 2026-11-10 (`patches/Render/ModelReference.cs`, the writers in
`ModelFaces.cs`, on with `ST_MAP_REFERENCE=1`): before each writer, its faces are
worked out in plain C# from its arguments, the scratchpad and the GTE's light
inputs as left by the caller; after it, every packet, tag, key head and the cursor
are compared. Over `scripts/session.py` in map 0: **2,578 FT3s, 21,111 FT4s, 4,664
GT3s and 1,623 GT4s matched whole, 0 differences**, 1,397 calls of each writer.

**The fogged writers** (`ModelStep192D4`'s: `func_800184C4` FT3, `80018828` FT4,
`80018BA0` GT3, `80018F48` GT4) take the same records and make the same packets,
with `NCDS`/`NCDT` and one `IR0` formed from the corners' fog factors f0..f3:

| writer | IR0 |
|---|---|
| FT3 | `(f0 + f1 + f2) / 3`, truncating |
| FT4 | `(f0 + f1 + f2 + f3) >> 2` |
| GT3 | `(2 f0 + f1 + f2) >> 2`: corner 0 counts twice |
| GT4 | the four's mean for corners 0 to 2 (`NCDT`), then **f0 alone** for corner 3's own `NCDS` |

So a smooth face's fog is one value for the face, not per corner, and a GT4's
fourth corner takes corner 0's fog. Read from the writers after a first run with a
plain mean left about 13% of smooth faces a step off in every channel; with the
rules above, a second run over the session: **all eight writers, 358,572 faces, 0
differences** (fogged: 30,206 FT3s, 268,381 FT4s, 17,935 GT3s, 8,953 GT4s). In map 0
most models are fogged (12,995 calls of each fogged writer against 1,397 unfogged).
`ModelStep1BF10`'s mixed writers are not covered yet: map 0 never runs them
(`ST_LITERAL_PROBE=1`, 0 runs); maps 4, 21, 23 and 24 do. Each picks `NCDS` or `NCCS`
per face from `0x8017BE8C`, adds the scratchpad's `+0x98` to the UVs (a texture
scroll, by the look of it) and has a near path of its own through a second vertex
buffer at `+0x48`. Under the flicker (`0x8017BE8C & 6`) each fogged colour is then rewritten as the
map's is ("The flicker's colour" in `GPU_RENDERER.md`): checked over the tour, 0
differences.

## The light inputs

- **The light matrix** (`LLM`, control 8 to 12), set in `ModelDraw`'s runs
  (`patches/Draw/ModelDrawRuns.cs`, before `0x800228B4`): the three columns of a
  3x3 matrix at `+0xA8` of the record `ModelDraw` works from, each turned by the
  model's rotation (the matrix `+0x394` points at) through `MVMVA`, so the light's
  directions in the model's own space. Unlike the map, whose four light matrices
  are pre-turned per quarter turn in the lighting record.
- **The colour matrix and back colour** (`LCM`, `BK`), set by `ModelStep17D28`
  (`func_80017D28`): a weighted sum of two lighting records' `+0x50` matrices and
  `+0x64` back colours, the two weights summing to 4096, each element `>> 12`; a
  model between two records is lit by a blend of both. Which records and which
  weight is not read.
- **The far colour** (`FC`, control 21 to 23), set by each step from
  `0x8017BE8C`: `(5, 5, 5)`, `(6, 4, 2)` when `& 2`, `(2, 6, 6)` when `& 4`, the map's
  three choices.
- **The face colour** (`RGBC`) and the normals: per face, from the model's data;
  not read.

## Not read yet

`ModelLight` (`func_80033DB8`) is not light: it fetches a pointer from a table at
`0x80165090 + 0x834C + id * 4` and keeps it only if `func_80015258` accepts it (the
name predates the reading). The mixed writers (`ModelStep1BF10`), the blend's records and weight, and the held
models' path (`HeldModel`, `func_80021AC4`, which calls the same steps) are next.
