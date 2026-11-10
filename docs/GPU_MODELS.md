# The models' light and fog: findings

Phase 11's log for the models, beside `docs/GPU_RENDERER.md` (the map). Read
2026-11-10 from the literal transcriptions in `patches/Draw/` and the generated
writers; **inspection only**: nothing here is measured or checked numerically yet.

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
FT3 shape. Which writer is which kind (FT3, FT4, GT3, GT4) is read from the first
of each row only; the rest are inferred from their order and their GTE calls.

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
name predates the reading). The face writers' record layout, the blend's records
and weight, and the held models' path (`HeldModel`, `func_80021AC4`, which calls
the same steps) are next. A model reference in the shape of the map's
(`patches/Render/MapReference.cs`) needs the face records first.
