# VerditeST: Shadow Tower (`SLUS-00863`) RecompOne port

Static recompilation of **Shadow Tower** (FromSoftware, 1998) using
[RecompOne](https://github.com/BlackLabelHQ/RecompOne) (MIT). The user supplies
their own dump of `SLUS-00863`.

**This file is the index.** It says what the project is and where it stands;
findings live in `docs/`, split by what you would be doing when you need them.

## Status

Phases 0 to 3 of `docs/PORTING_PLAN.md` have passed their gates, with Phase 2's
by-eye checks still the user's and Phase 3's fuzzer not ported. The repository holds `tools/RecompOne` (the fork at `7031630` plus local
patches through `0121`) and `tools/verdite-core` (Verdite Core at `f1138e1`). The disc
recompiles (4027 functions, `applied 199 patches, 0 reimplementations`).
`scripts/acceptance.py` boots from the movies to the first area, saves, loads both
ways, changes areas and dies, driven by `ST_AUTOSTART`, the beacon's state and `warp`;
all 36 area modules load and run. Phase 4 is next, with Phase 3's open items in
`docs/TODO.md`.

## The documents

- [docs/PORTING_PLAN.md](docs/PORTING_PLAN.md): the phases and their gates.
- [docs/DEVELOPMENT.md](docs/DEVELOPMENT.md): build, run and diagnose.
- [docs/RECOMPILATION.md](docs/RECOMPILATION.md): config, overlays, function maps, SDK addresses.
- [docs/GAME_INTERNALS.md](docs/GAME_INTERNALS.md): the game's own addresses and routines.
- [docs/ENV_VARS.md](docs/ENV_VARS.md): every `ST_*` switch.
- [docs/TODO.md](docs/TODO.md): what is open now.

## Where to write a new finding

The finding goes in the document, not only in the commit message. Pick by what a
reader would be doing when they need it. If it fits nowhere yet, put it in the
general-purpose part of the nearest document.
