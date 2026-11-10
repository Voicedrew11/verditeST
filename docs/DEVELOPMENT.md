# Development: build, run and diagnose

How to build the recompiler, recompile the game into `generated/`, build and run
`ShadowTower`, and read a run's logs. Verdite3's `docs/DEVELOPMENT.md` is the
model. The commands are in "Build and run" in `AGENTS.md`.

## The acceptance test

```bash
dotnet build ShadowTowerRecomp.csproj -c Release
python3 scripts/acceptance.py
```

It boots through the movies, title and opening text into the first area
(`ST_AUTOSTART=new`), walks, saves to card A, walks away, and loads from the in-game
menu. A second process loads from the title (`ST_AUTOSTART=load`), warps to map 0 at
the saved spot and checks the player is alive, warps to map 1, where the player dies
on arrival, and presses Cross back to the title. It judges by the beacon's state and
the card, never the picture. About 1 minute 50. `carda.sav` is set aside for the run
and put back. Passed three times in a row on 2026-11-09, which is Phase 3's gate.

The save step enters the game's own save screen through `call`, because the in-world
trigger is not found yet (`docs/GAME_INTERNALS.md`). Area changes use `warp`, not a
door. The area change comes after the saves because of the death on arrival
("Changing areas" in `docs/GAME_INTERNALS.md`), and in the second process because
`call` stops running after the in-game menu load (below).

## What is still open

- **The test failed once in four runs on 2026-11-10** (after the draw routines went to
  C# and `warp` took arrival points): in the second process, the queued call for
  `warp 1` never ran (the log shows `warp 0`'s `80044EE8` returning and nothing
  after), so map 1 was never reached. The next three runs passed. Calls wait for a
  vblank from the game's own `VSync`; which loop withheld it is not read, nor whether
  `ST_LITERAL=0` changes the rate.

- A load lands up to `0x2630` in height (Y, not Z as written before) from the
  position read just before saving. The menu load and the title load differ by that
  much. Not explained; the test compares X and Z only, within `0x800`.
- After the in-game menu load, queued calls (and so `warp`) stop running: they wait for
  a vblank from the game's own `VSync`, so the game is probably sitting in a loop
  without one. Not read.
- By eye, not checked by anything here: the picture, the movies, the menus, the save
  screens, whether the audio sounds right.

## Frame pacing

The world ticks at the game's own 20 Hz and the picture is drawn at any rate. On by
default at 144 fps, as in the King's Field ports; `ST_PACING=0` turns it off. Three
patches, all reading the stage table in `patches/MainLoop.cs` ("The main loop" in
`docs/GAME_INTERNALS.md`):

- **`patches/FramePacing.cs`**, Verdite3's mechanism on this loop. The frame gate
  `func_8001535C` is skipped (its count zeroed). **The frame boundary** is ST.EXE's
  `DrawOTag` after a `VSync` call; the frame is paced there to `ST_FPS`, and the world
  clock advanced by wall time. **Stages 1 to 13 run only on a tick of the 20 Hz clock**,
  decided once an iteration at stage 1, and only when called from the main loop's own
  sites. Stage 14 runs every frame. A watchdog: no boundary for 500 ms and stage 1
  ticks from the wall clock instead (`ST_PACING_NOBOUNDARY=1` tests it; it read 20.0
  ticks a second). `patches/PacingProbe.cs` prints `ST_FPS_PROBE`'s line.
- **`patches/VBlankPacing.cs`**: `VSync(n)` waits n vblanks on a 60 Hz grid in all six
  programs, as libetc does; the runtime's non-blocking `VSync` returns at once for any
  mode. This is what holds the game to 20 with pacing off, and VS mode and the ending
  to theirs. With pacing on, the main loop's own call to stage 14 is exempt (its
  boundary is FramePacing's); loops that call stage 14 themselves, the menu and fades,
  are held, so they draw and step at the console's 20.
- **`patches/DrawStageTick.cs`**: what stage 14 advances once a frame, held on a frame
  the world did not tick: the cel animations, the light's flicker `rand()` (so the
  world's random sequence is also the console's at any rate), and the rumble's
  countdown ("What runs at the drawn rate" in `docs/GAME_INTERNALS.md`).

Smoothing between ticks is Phase 6, so at 144 the picture still changes 20 times a
second.

Measured 2026-11-10 in map 0, `ST_FPS_PROBE=1` and `ST_VBLANKPACING_PROBE=1`; "walked"
is the distance from holding Up for one second, three times each:

| run | drawn a second | world ticks a second | walked | the game's own count (`0x8017BD51`) |
|---|---|---|---|---|
| before (no `VSync(n)` wait, no pacing) | 60 | 60 | 9663 to 11328 | 60 |
| `ST_PACING=0` | 20 (20.0 `VSync(3)` a second, mean wait 49.2 ms) | 20 | 4224 to 4416 | 20 |
| `ST_FPS=60` | 60.0 | 19.7 to 20.0 | 4224 | 60 |
| `ST_FPS=144` (the default) | 144.0 | 19.9 to 20.0 | 4224 to 4416 | 144 |
| `ST_FPS=off` | 1650 to 2000 | 20.0 | 4224 | wraps past 255 |

Packets in a frame are the same on ticked and idle frames (332 and 332 standing), so no
gated stage feeds the picture. `VS.EXE` (`ST_BOOTEXE=vs`) holds 20.0 `VSync(3)` a
second. The acceptance test passes with pacing on and with `ST_PACING=0`.

**By eye, not checked here:** game speed against a console or an emulator, the menus
and fades at 20, the movies, the title and the opening text with their counted waits
honoured.

## Measuring the draw path

`scripts/session.py <log> [ST_X=1 ...]` boots a new game and plays the same moves
every run: stand, walk forward and back along the first heading, turn in place, open
and close the menu, then walk off the ledge to the left of the start and press Cross
at the death prompt. Map 0's start is ringed by fatal drops: walking after a turn
fell every time, so the session only walks along the first heading until the death
it means. It prints the player's HP and position after each move and judges nothing;
the probe's lines are in the log. About 1 minute 30. `carda.sav` is not touched by
the moves, but copy it aside anyway.

`scripts/tour.py <log> [ST_X=1 ...]` warps to each of the 23 maps with an arrival point,
stands and turns a circle in each: about ten minutes. Under `ST_LITERAL=verify` it is
the draw path's verify corpus beside the session.

`ST_GEOPROBE=1`, `ST_PRESENTPROBE=1` and `ST_PRIMBUF_PROBE=1` are the probes; the tables in
`docs/GEOMETRY.md` came from them. `scripts/callsites.py st <routine>` lists a
routine's calls with their return addresses, which is what the probes key on, and `scripts/disasm.py scratch/ST.EXE <address>`
disassembles one from the extracted executable (quicker to read than `generated/`).

## Driving the game without a person

`ST_AUTOSTART=new` or `load` boots into play with nobody at the pad
(`patches/AutoStart.cs`): `new` pulses Start through the movies, the title and the
opening text; `load` picks Load on the title by timed presses (480, 660 and 720
vblanks after `TITLE.EXE` loads), then pulses Cross until the card screen has run and
the player is in an area. It prints one `[ST] autostart:` line saying where the player
is, or that it gave up.

`ST_AGENT=1` prints the beacon (`patches/AgentBeacon.cs`) as an `[ST-AGENT]` line
about once a second, and a line on every program or area load. The same JSON is the
command channel's `state`:

```json
{"program":"st","map":0,"frames":1576,"inGame":true,"hp":752,"maxHp":752,"mp":550,
 "maxMp":550,"dead":false,"pos":[262144,267712,258048],"heading":0}
```

`map` is -1 outside `ST.EXE`; outside an area only the first four fields are there.
`inGame` is `ST.EXE` with a module in and a maximum HP set.

`ST_SHELL=1` opens a command channel on `127.0.0.1:27904` (`patches/AgentServer.cs`).
One request per line, one line back:

| command | what |
|---|---|
| `state` | the beacon's JSON, above |
| `press <a+b> [ms]` | hold pad buttons, default 150 ms |
| `warp <map> [x y z]` | load that map's module, and place the player at x y z (decimal) first; without a point the player lands where a door into the map would put them (`patches/Arrivals.cs`, 23 maps), or keeps their coordinates in a map with none, which is usually a death |
| `peek <addr> [bytes]`, `poke <addr> <hex>` | read and write guest memory |
| `dump <file>` | the 2 MB of RAM |
| `call <addr> [a0] [a1]` | run a game routine, from the next vblank that comes from the game's own `VSync`; the reply says only that it is queued |

`call 8003CF28 <delta>` changes HP the game's way (`FFFFFD10` is -752, a death).

Addresses are hex without `0x`. Requests run on the game thread at each vblank, so a
reply can take as long as the game goes without one; a client should wait at least
40 s. After 5 s the channel answers `error: timed out`.

Only the host pad path feeds this: the press goes through `PadReadEvent` (runtime
`0120`).

**A run stopped by `timeout` loses the last few KB of console output**, since stdout
is block-buffered when redirected. Do not count log lines to decide whether something
happened while the game is still running.
