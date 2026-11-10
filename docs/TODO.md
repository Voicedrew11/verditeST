# TODO

The phases and their gates are in `docs/PORTING_PLAN.md`. This file holds what is
open now.

## Waiting on the user

- **By eye:** the camera and the models carried between ticks (`docs/SMOOTHING.md`):
  turning, walking and creatures moving at 144, against `ST_SMOOTH=0`; a cell popping
  in late when walking across a cell edge (about 1% of frames carry the eye there).
- **By eye:** widescreen at 16:9, now the default (`docs/WIDESCREEN.md`): the margins
  while turning, the tints at the edges, menus pillar-boxed; the HUD at the edges with
  `ST_WIDESCREEN_HUD=1`, and whether it should be on.
- **By hand, and a decision:** the WASD layout (`docs/INPUT.md`, "The keyboard"), now
  applied to this checkout's `settings.json` (it was RecompOne's stock keys; Input
  settings has both). Whether L2/R2 should have keys for a player without a mouse,
  and whether Circle's item panel belongs on E.
- **By hand:** mouse look (`docs/INPUT.md`): a click captures; turning, looking, and
  whether mouse down should look down.
- **By hand:** vibration on a real pad (`docs/INPUT.md`): the options' vibration on,
  then a hit or a fall.
- **To push (the user):** the fork commit `0122` (`tools/RecompOne`, commit `4f5cd57`),
  to `Voicedrew11/verdite-recompone`; until then this copy is ahead of it.
- **By eye:** a status flash (a full-screen tint), now up for the whole tick at 144:
  poke `801991FE 0010` and `80199200 4000` over the agent channel.
- **By eye:** map 24's two texture animations (`docs/GAME_INTERNALS.md`, "Texture
  animations"), now at the tick's pace: against an emulator, or `ST_PACING=0`.
- **By hand:** `Settings ▸ Testing` (`docs/DEVELOPMENT.md`): each control live,
  and that the wording reads right.
- **By hand:** first-run setup (`docs/DEVELOPMENT.md`, "First-run setup"): it opens
  on the next start of this checkout; walk it, then check the Testing tab agrees.
- **A decision:** the port's settings inside the game's own menu (Phase 10). The
  menu's eight labels are images, not text (`docs/GAME_INTERNALS.md`, "The menu"), so
  a ninth item needs a label drawn for it, in the menu's style. Alternatives: a
  button the menu ignores (Select, L1 or R1 in the menu) opening the Testing tab, or
  leaving it to `Settings` in the window. Recommended: the button, which needs no art.
- **By eye:** the menu at 16:9 (`docs/WIDESCREEN.md`, "The menus"): the world drawn
  behind it to the screen's edges, against `ST_MENUWORLD=0`.
- **By eye:** the picture's switches (`docs/PICTURE.md`), each against its `=0`; the
  Z-buffer most of all (decals, the HUD, fades, the menu).

## Phase 11: what is left

Planned 2026-11-10 (`docs/RENDERER_PLAN.md`), on Verdite3's route (the user's call:
it is a recompilation like this one). Read: the map's cells, meshes, faces,
projection, lighting record and linear fog (`docs/GPU_RENDERER.md`). The scene
census (`ST_SCENE_CENSUS=1`) gives every packet of the session an owner, 0
unattributed. Verify compares RAM, scratchpad, registers, GTE and the calls each
routine makes: 0 mismatches over the session. The map reference
(`ST_MAP_REFERENCE=1`) works out every bulk face of the map in plain C#, the scene's flat
cells included, light and fog too, and matches the game's packets word for word.
The near splits are measured: pieces in the face's own key, and the 98.5% that
draw nothing are all off the screen. The models' light is read
(`docs/GPU_MODELS.md`, inspection only). The models' unfogged and fogged face writers (eight) match a plain-C# reference word
for word, the fogged ones' IR0 rules read. Over the tour, 0 differences in 11 maps.
The flicker's colour is modelled and checked, and all twelve model writers match
over the tour. The blend's modes are read and counted (`docs/GPU_MODELS.md`). The baseline cost is
measured (`docs/GPU_COST.md`, under 1.4 ms a frame at p95). Next: what sets a
model's blend flags and weight, the near divisions' own arithmetic; the census and call
order in the contexts not reached (held models, text, tints, the flicker).

## Phase 10: what is left

Started 2026-11-10: `Settings ▸ Testing` has the switches live and kept ("The Testing
tab" in `docs/DEVELOPMENT.md`); the kept values are measured to come back. First-run
setup is in (`config/onboarding.json`, shown by the game itself; no launcher yet).
The page in the game's own menu waits on a decision ("Waiting on the user").

## Phase 9: what is left

Started 2026-11-10 (`docs/WIDESCREEN.md`): 16:9 by default, the tints stretched, the
HUD movable to the edges (off), the near polygons widened. The measured gate is met:
the same cells at both aspects (the cull is by position), and the busiest area's
packets peak at 33% of a buffer. `MenuWorld` draws the world live behind the menu instead of
its 320-wide still (`docs/WIDESCREEN.md`, "The menus"); the game's memory is put back
after every pass. Left: the user's eye.

## Phase 8: what is left

Started 2026-11-10 (`docs/INPUT.md`): step 1 read. The game is digital only, has a
twelve-entry button table, and drives two motors; the runtime's rumble is fixed in
the fork (`0122`). Mouse look drives the game's own velocities, and the measured gate
is met (a synthetic hand lands its step every tick and stops dead). Instant mouse look leads
the drawn view by the hand's motion since the tick. The WASD layout is the
default (`KeyLayout`). Next: `MenuMouse`, the settings page (Phase 10).

## Phase 7: what is left

Started 2026-11-10: no dither, 24-bit colour, perspective and sub-pixel are on
(`docs/PICTURE.md`), the address map hitting 98.5% in map 0; the Z-buffer is on from
the map's depths. The misses are all the HUD's 2D packets (3D: 99.98 to 100%), so the
packet records are not needed; the 12 reachable maps measure the same. Nothing of
Verdite1's picture is left to port; the rest is the user's eye.

## Phase 6: what is left

Started 2026-11-10: the camera and the models' positions and angles are carried
(`docs/SMOOTHING.md`). The poses blend between keys,
so the carried clip time smooths them. The HUD's gauges step
once, with nothing to carry. The measured gate is met (a creature in map 24 new
on every frame, the eye still). Left: the arm, which needs a held weapon in a measured
run (a new game holds none). The cell
cull trails the carried eye on about 1% of walking frames (`docs/SMOOTHING.md`).

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
  `docs/GAME_INTERNALS.md`). Walking through real doors (`scripts/door_walk.py`) found
  two points that hold (18, 47) but need a warp that marks the door it lands in, and
  showed map 13 kills on arrival through its real door too. Stopped after two tries
  at 41 → 30, which does not open.
- ~~After the in-game menu load, `call` and `warp` stop running.~~ No longer
  reproduces (2026-11-10, `docs/DEVELOPMENT.md`).
- ~~The acceptance test fails at "never reached map 1" about one run in four.~~ A
  warp sent into a busy loader (2026-11-10, `docs/DEVELOPMENT.md`); fixed.
- `ST_AUTOSTART=load` takes the first save the card screen offers; picking a slot is
  not done.
- ~~A main-loop marker for the beacon.~~ Added with a menu marker (2026-11-10,
  `docs/DEVELOPMENT.md`).

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
