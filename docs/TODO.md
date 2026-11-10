# TODO

The phases and their gates are in `docs/PORTING_PLAN.md`. This file holds what is
open now.

## Phase 3: what is left

The gate's test passes (`scripts/acceptance.py`, three runs, 2026-11-09). Still open:

- **The fuzzer** (Verdite1's `Fuzz.cs`, `scripts/fuzz.py`, fork `0112`). It needs a
  warp that lands on a floor in any map, which needs the next item.
- **Arrival points.** Decode the door records (`func_80050768`, "Changing areas" in
  `docs/GAME_INTERNALS.md`) so `warp <map>` can arrive where a door would. Until then
  a warp to another map kills the player.
- After the in-game menu load, `call` and `warp` stop running (no vblank from the
  game's own `VSync`). Find the loop it sits in.
- `ST_AUTOSTART=load` takes the first save the card screen offers; picking a slot is
  not done.
- A main-loop marker for the beacon (Verdite3's `loop`), which needs Phase 4's survey.

## Phase 2: what is left

- **By eye (the user):** the picture, the movies, the menus, the save screens, the
  audio. Nothing has looked at these.
- What makes the player save in the world (`func_80055374`, `docs/GAME_INTERNALS.md`).
- A load lands up to `0x2630` in height from the position read before saving.
- Walking through a real door. The test warps.
- `func_80068854` (`st`) and `func_80035F6C` (`vs`), `libcard`'s jump into BIOS
  memory: saving and loading worked without reaching them.
- `ST_BOOTEXE=st` with no pad draws 1774 frames and goes to `TITLE.EXE`. The real
  path (title, opening text, game) stayed in the game for about 4000 frames, so this
  looks like the missing hand-over, but it is not read.
