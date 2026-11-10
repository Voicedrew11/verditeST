# Development: build, run and diagnose

How to build the recompiler, recompile the game into `generated/`, build and run
`ShadowTower`, and read a run's logs. Verdite3's `docs/DEVELOPMENT.md` is the
model. The commands are in "Build and run" in `AGENTS.md`.

## The acceptance test

```bash
dotnet build ShadowTowerRecomp.csproj -c Release
python3 scripts/acceptance.py
```

It boots through the movies, title and opening text into the first area, walks,
changes area and comes back, saves to card A, walks away, loads from the in-game
menu, then starts a second process and loads from the title. It judges by memory and
the card, never the picture. About 2.5 minutes. `carda.sav` is set aside for the run
and put back. Passed twice in a row on 2026-11-09.

The save step enters the game's own save screen through `call`, because the in-world
trigger is not found yet (`docs/GAME_INTERNALS.md`). Area changes use the game's
routine with a poked map number, not a door.

## What is still open

- A load lands up to `0x2630` in Z from the position read just before saving. The
  menu load and the title load differ by that much. Not explained.
- Dying is untested: the player's HP is not found.
- By eye, not checked by anything here: the picture, the movies, the menus, the save
  screens, whether the audio sounds right.

## Driving the game without a person

`ST_SHELL=1` opens a command channel on `127.0.0.1:27904` (`patches/AgentServer.cs`).
One request per line, one line back:

| command | what |
|---|---|
| `state` | program, area module, frames |
| `press <a+b> [ms]` | hold pad buttons, default 150 ms |
| `peek <addr> [bytes]`, `poke <addr> <hex>` | read and write guest memory |
| `dump <file>` | the 2 MB of RAM |
| `call <addr> [a0] [a1]` | run a game routine, from the next vblank that comes from the game's own `VSync`; the reply says only that it is queued |

Addresses are hex without `0x`. Requests run on the game thread at each vblank, so a
reply can take as long as the game goes without one; a client should wait at least
40 s. After 5 s the channel answers `error: timed out`.

Only the host pad path feeds this: the press goes through `PadReadEvent` (runtime
`0120`).

**A run stopped by `timeout` loses the last few KB of console output**, since stdout
is block-buffered when redirected. Do not count log lines to decide whether something
happened while the game is still running.
