# TODO

The phases and their gates are in `docs/PORTING_PLAN.md`. This file holds what is
open now.

## Waiting on the user

- **By eye:** the camera and the models carried between ticks (`docs/SMOOTHING.md`):
  turning, walking and creatures moving at 144, against `ST_SMOOTH=0`.
- **By eye:** the picture's switches (`docs/PICTURE.md`), each against its `=0`.

## Phase 7: what is left

Started 2026-11-10: no dither, 24-bit colour, perspective and sub-pixel are on
(`docs/PICTURE.md`), the address map hitting 98.5% in map 0. Next: the misses by
writer, other areas, the Z-buffer from the address map's depths.

## Phase 6: what is left

Started 2026-11-10: the camera and the models' positions and angles are carried
(`docs/SMOOTHING.md`). Next: the models' poses, the arm, the HUD's gauges, whether
the cell cull lags the carried eye.

## Phase 5: what is left

Started 2026-11-10: the first inventory is in `docs/GEOMETRY.md`, from map 0 and the
menu and death prompt. Next, in order:

- The contexts not reached: the card screens, a fight on purpose, 24 areas without an
  arrival point that holds, the area modules' own drawing, `VS.EXE`.
- Three routines never ran (`func_8001BF10`, `func_80034604`, `func_80028784`), and
  the frames that present (menu, prompt, blank) cannot be verified by running twice.
- Name the routines still called `Draw<address>`, `ModelStep<address>` and so on.
- What `func_800241D0`'s three model sites, `func_800251C8` and `func_80021AC4` draw;
  the leaves `func_8001F954` and `func_8001FD00`.
- Then Step 2: literal C# for each routine, verified with `Differential`.

## Phase 4: what is left

The gate is met (2026-11-10, "Frame pacing" in `docs/DEVELOPMENT.md`). Still open:

- **By eye (the user):** game speed against a console or an emulator; the menus and
  fades, which now step at 20; the movies, title and opening text, whose counted
  `VSync` waits are now honoured.
- The census ran in one area, standing. An area with other animations, a fight, or the
  area modules' own code called from stage 14 may show more at the drawn rate.
- Loops that call stage 14 themselves (the menu, fades) draw at 20 under pacing.
  Redrawing between their ticks, as Verdite3's `LoopPacing` does, waits for Phase 6.
- Stages 2, 4 to 6, 11 and 13 are not named beyond their counterparts.
- The Settings page for pacing (Phase 10); for now the switches are environment
  variables.

## Phase 3: what is left

The gate's test passes (`scripts/acceptance.py`, three runs, 2026-11-09). Still open:

- **The fuzzer** (Verdite1's `Fuzz.cs`, `scripts/fuzz.py`, fork `0112`). It needs a
  warp that lands on a floor in any map, which needs the next item.
- **Arrival points**: 12 of 36 maps hold (2026-11-10, `patches/Arrivals.cs`); in the
  rest the decoded door points kill the player ("Arrival points" in
  `docs/GAME_INTERNALS.md`). Why some doors' points are wrong is the next question.
- After the in-game menu load, `call` and `warp` stop running (no vblank from the
  game's own `VSync`). Find the loop it sits in.
- `ST_AUTOSTART=load` takes the first save the card screen offers; picking a slot is
  not done.
- A main-loop marker for the beacon (Verdite3's `loop`). Phase 4's survey has the
  loop (`docs/GAME_INTERNALS.md`); the marker is not added yet.

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
