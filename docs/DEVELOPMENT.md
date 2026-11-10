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

- A load lands up to `0x2630` in height (Y, not Z as written before) from the
  position read just before saving. The menu load and the title load differ by that
  much. Not explained; the test compares X and Z only, within `0x800`.
- After the in-game menu load, queued calls (and so `warp`) stop running: they wait for
  a vblank from the game's own `VSync`, so the game is probably sitting in a loop
  without one. Not read.
- By eye, not checked by anything here: the picture, the movies, the menus, the save
  screens, whether the audio sounds right.

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
| `warp <map> [x y z]` | load that map's module, and place the player at x y z (decimal) first; without a point the player keeps their coordinates, which in another map is usually a death |
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
