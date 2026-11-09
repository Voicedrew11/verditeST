# TODO

The phases and their gates are in `docs/PORTING_PLAN.md`. This file holds what is
open now.

## Phase 2: first run

- Boot: the stub, then `STR.EXE`. The stub loads programs with BIOS `Load` and
  `Exec`; check the runtime's handling of both.
- `func_80068854` (`st`) and `func_80035F6C` (`vs`), `libcard`'s jump into BIOS
  memory: see "Indirect jumps nothing resolves" in `docs/RECOMPILATION.md`.
