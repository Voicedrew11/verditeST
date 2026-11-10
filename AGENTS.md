# AGENTS.md

READBAILITY RULE!
- Files should generally not exceed 300 lines, and should absolutly not exceed 500 lines

Guidance for agents working in this repository.

## What this is

A static recompilation of **Shadow Tower** (FromSoftware, 1998), disc
**`SLUS-00863`**, using [RecompOne](https://github.com/BlackLabelHQ/RecompOne).

Shadow Tower shares an engine lineage with the King's Field games, not addresses.
The sibling projects Verdite1 (`../verdite1`), Verdite2 (`../KFII-PC`) and
Verdite3 (`../verdite3`) are *different games*: no address, overlay name, record
layout, table size, tick rate, fog formula or patch table from them applies here,
and this disc's addresses do not apply there. Copy mechanisms and conventions,
never facts; use `tools/verdite-core/scripts/match_code.py` to find a counterpart,
then check it does the same job with the same arguments at the same point in the
frame.

There is no decompilation, no ELF and no `.map`. Function boundaries come from a
linear sweep, and PSY-Q library functions are identified with the signature bank
and by hand, so most work here is *reverse engineering*: find an SDK function's
address in the disc image, map it to the runtime's HLE implementation, re-run the
recompiler, run the game, read the logs.

**`NOTES.md` is the index, and `docs/` is the working log.** Read `NOTES.md`
before starting anything, then the one or two documents the task touches:

| file | when |
|---|---|
| `docs/PORTING_PLAN.md` | the phases, their gates and what is decided |
| `docs/DEVELOPMENT.md` | build, run, diagnose, measure |
| `docs/RECOMPILATION.md` | config, overlays, function maps, SDK addresses |
| `docs/GAME_INTERNALS.md` | the game's own addresses and routines |
| `docs/ENV_VARS.md` | every `ST_*` switch, in one list |
| `docs/TODO.md` | next steps and open questions |
| `tools/RecompOne/docs/RECOMPONE_PATCHES.md` | every change the fork makes to RecompOne |

Findings go in the right document, not only in commit messages and not in this
file.

## Build and run

The disc is gitignored, user-supplied, and expected at `disc/ShadowTower.cue`.

```bash
# build the recompiler (tools/RecompOne)
bash scripts/setup_tools.sh

# recompile MIPS -> C# into generated/
dotnet run --project tools/RecompOne/RecompOne.Recompiler -c Release --no-build -- config/st.json

# build and run the game
dotnet build ShadowTowerRecomp.csproj -c Release
dotnet run --project ShadowTowerRecomp.csproj -c Release -- disc/ShadowTower.cue
```

The assembly is `ShadowTower`, the environment-variable prefix is `ST_`, and the
MCP project is `ShadowTowerMcp`. `tools/RecompOne` is a **subtree** of the fork
`Voicedrew11/verdite-recompone`, pinned at `7031630`; `tools/verdite-core` is a
subtree of the shared, game-agnostic `Voicedrew11/verdite-core`, pinned at
`f1138e1`. Their sources are tracked here, so a fresh clone already has them and
nothing needs fetching.

There are no tests. Verification is empirical: run the game with log channels on
and check the trace against what the SDK sequence should look like. The `ST_*`
switches are listed in `docs/ENV_VARS.md`.

**Anything judged by eye is the user's job, not yours.** Do not capture,
screenshot or otherwise scrape the game window: it burns a lot of context and
produces nothing a person could not say in one sentence. Measure what a counter
can measure, then say plainly what still needs looking at and ask.

## Two traps

These are RecompOne facts, so they hold here regardless of the game:

- **Number bases differ between the config and the CLI.** In `config/st.json`,
  `base` is a hex *string* while `size`/`skip`/`offset`/`lba` are decimal numbers;
  on `--generate-function-file`, `-size`/`-skip`/`-offset` are hex.
  `"skip": 2048` in the config is `-skip 800` on the command line.
- **Overlays are read as raw bytes.** `ResolveOverlay` does not parse the PS-X EXE
  header, so every `.EXE` overlay needs `"skip": 2048` to step past the 0x800-byte
  header, and `base` must be the header's real text address. The *boot*
  executable is different: it goes through `Psx/Parser.cs`, which strips the
  header itself.

## The shared subtrees

`tools/RecompOne` and `tools/verdite-core` are subtrees: their sources are
tracked here, so an edit inside one is a change to this repository like any
other.

- **Shared-subtree edits go in their own commits** (a commit that touches a
  shared subtree touches nothing else) and are pushed to that subtree's shared
  repo soon after. No sibling's acceptance test may break because Shadow Tower
  needed something, and a sibling pulls a change only when the user decides.
- **This repository's copy must always equal some commit of the shared repo.**
- **Nothing goes upstream.** Not a pull request, and not an issue either.
  Upstream rejects AI-authored pull requests. A defect found here is recorded in
  `docs/` and fixed in the vendored tree, which is the point of vendoring it.

## Repository conventions

Never commit disc data or recompiler output: `disc/`, `generated/`, `*.sav` and
`settings.json` are gitignored for copyright and cleanliness reasons.

Commit messages state the *finding*, in the imperative, with the observable
consequence.
