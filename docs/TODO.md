# TODO

The phases and their gates are in `docs/PORTING_PLAN.md`. This file holds what is
open now.

## Phase 2: what is left

The gate's test passes (`scripts/acceptance.py`). Still open:

- **By eye (the user):** the picture, the movies, the menus, the save screens, the
  audio. Nothing has looked at these.
- What makes the player save in the world (`func_80055374`, `docs/GAME_INTERNALS.md`).
- Dying once: the player's HP is not found.
- A load lands up to `0x2630` in Z from the position read before saving.
- Walking through a real door. The area test requests the map directly.
- `func_80068854` (`st`) and `func_80035F6C` (`vs`), `libcard`'s jump into BIOS
  memory: saving and loading worked without reaching them.
- `ST_BOOTEXE=st` with no pad draws 1774 frames and goes to `TITLE.EXE`. The real
  path (title, opening text, game) stayed in the game for about 4000 frames, so this
  looks like the missing hand-over, but it is not read.
