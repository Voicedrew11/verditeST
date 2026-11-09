# TODO

The phases and their gates are in `docs/PORTING_PLAN.md`. This file holds what is
open now.

## Phase 2: first run

- Boot: the stub, then `STR.EXE`. The stub loads programs with BIOS `Load` and
  `Exec`; check the runtime's handling of both.
- With `ST_BOOTEXE=st` and no pad input, `ST.EXE` loads `fdat002`, draws 1774
  frames and hands over to `TITLE.EXE`. Why is not read yet. It may only be the
  missing hand-over from the title. From `ST_BOOTEXE=optxt` it stayed in the game
  for the rest of a 2-minute run.
- `func_80068854` (`st`) and `func_80035F6C` (`vs`), `libcard`'s jump into BIOS
  memory: see "Indirect jumps nothing resolves" in `docs/RECOMPILATION.md`.
