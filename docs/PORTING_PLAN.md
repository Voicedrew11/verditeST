# Plan: porting Shadow Tower

Prepared 2026-11-09. **This is planned work, not a record of anything done**,
except struck-through phases. What was read off the disc that day is now in
`docs/RECOMPILATION.md`, "What is on the disc". When a phase's gate closes, strike it through here, as the siblings' `TODO.md` does.
Findings made along the way go in the documents named under each phase, not here.

The models are Verdite2's `docs/SHARING_PLAN.md` (Phase 2, the bootstrap, and
Phase 3, features one at a time), Verdite1's `docs/RENDERER_PLAN.md` (the
renderer, built once) and Verdite3's bring-up as its git history records it.
Verdite2 is `../KFII-PC`, Verdite3 `../verdite3`, Verdite1 `../verdite1`.

## The rules, carried over

- **No fact carries over.** Shadow Tower shares an engine lineage with King's
  Field, not addresses. Every address, record layout, table size, tick rate,
  fog formula and overlay base is re-derived from this disc. Copy mechanisms and
  conventions; use `match_code.py` to find counterparts, then check each does the
  same job with the same arguments at the same point in the frame.
- **Build the final path once.** The recompiled packet path stays as the
  reference and the fallback; every C# replacement has a literal mode verified
  against the recompiled routine (`Differential`) before it changes anything.
- **Extract on evidence.** A patch is built here first, adapted from the nearest
  sibling. It moves to Verdite Core only when this port's working copy and a
  sibling's turn out to be the same mechanism with different data.
- **Shared-subtree edits are their own commits**, pushed to the shared repo soon
  after, and no sibling's acceptance test may break because Shadow Tower needed
  something. A sibling pulls a fork change only when the user decides.
- **Nothing copyrighted is committed**: the disc, anything extracted from it,
  `generated/`, saves. Nothing goes upstream to RecompOne.
- **Anything judged by eye is the user's.** Each phase ends with counters and
  probes, and a short list of what still needs looking at. Enhancements ship on
  at the siblings' judged values, as Verdite1 does since 2026-10-17, so the
  manual pass meets them in play.
- **Findings go in `docs/`**, commit messages state the finding with its
  observable consequence, and `NOTES.md` is the index.

## Decisions made here

Left to this plan; change any of them before Phase 0 and nothing else moves.

| what | choice | why |
|---|---|---|
| target disc | `SLUS-00863`, Shadow Tower (USA) | the dump supplied |
| port name | `VerditeST` | the directory's name |
| assembly, csproj | `ShadowTower`, `ShadowTowerRecomp.csproj` | as `KingsField3` |
| tag, env prefix | `ST`, `ST_` | the disc's own name for itself (`ST.EXE`, `/ST/`) |
| recompiler config | `config/st.json` | |
| disc path | `disc/ShadowTower.cue` / `.bin` | |
| launcher, app id | `VerditeST.Launcher`, `verditest` | data in `~/.local/share/verditest` |
| MCP project | `ShadowTowerMcp` | |
| shared pins | the fork at `7031630`, Verdite Core at `f1138e1` | both repos' `main` today; newer than Verdite3's pins, so the first build here is also the first build on them |

**Ask first** (not decided here): creating the GitHub repository and any push;
whether the Japanese disc (`SLPS-01420`) is ever a target; pulling a fork change
made here into a sibling.

The disc as read on 2026-11-09 is in `docs/RECOMPILATION.md`, "What is on the disc".

## What will be different from the King's Field ports

Expected, none of it measured:

- **Six overlays, and a mode the others lack.** `VS.EXE` is a second playable
  program, 0x52800 of text. It is recompiled with the rest so the disc runs
  whole, and gets no enhancement work until the main game is done (Phase 12).
- **Movies are their own program.** `STR.EXE` is the first thing the stub is
  likely to run, so MDEC playback is on the boot path, not behind a title.
- **A newer SDK.** Expect the later pad library (analog sticks and vibration
  read by the game itself), the later card library, and XA streaming through
  `CdlSetfilter`/`CdlReadS`. Each is either bound to the runtime's HLE, left to
  run recompiled against the hardware model, or a fork change. Which, is found
  at boot (Phase 3).
- **The game already reads the sticks** if it uses the analog pad. Twin-stick
  and `Analog` then start from the game's own handling, not from the D-pad
  rate as in Verdite3.
- **Game systems King's Field has no counterpart for**: two hands, equipment
  that wears out, no levels, a creature book, no in-game map. `GearCompare`
  has more to show here (durability); `MapKey` has nothing to open.
- **The tick rate is unknown.** Verdite1 runs at 20, Verdite3 at 15 and its
  port had been running the world at 30. Measure before pacing.

## ~~Phase 0: lay out the repository~~ (done 2026-11-09)

Before `git init`: the disc sits in the directory's root today.

1. Move the dump to `disc/ShadowTower.cue` and `.bin`, and change the cue's
   `FILE` line to match. Write `.gitignore` (Verdite3's, unchanged in substance)
   **before the first commit**.
2. `git init`; `LICENSE`, `README.md`, `AGENTS.md`, `NOTES.md`, `disc/README.md`,
   and empty `docs/` (`DEVELOPMENT`, `RECOMPILATION`, `GAME_INTERNALS`,
   `ENV_VARS`, `TODO`), each from Verdite3's `bb1290a` layout with this game's
   names. `AGENTS.md` says which game this is and that no sibling fact applies.
3. `git subtree add --squash` the fork at `tools/RecompOne` and Verdite Core at
   `tools/verdite-core`, at the pins above; `scripts/setup_tools.sh` from
   Verdite3 with nothing changed but the comments.
4. `config/verdite.json`: the disc, `config/st.json`, `config/funcmaps`, and
   `sdkOverlays` once Phase 2 knows them (expected: all six).
5. `ShadowTowerRecomp.csproj` and a first `Program.cs` from Verdite3's, with
   `Game.Configure(tag: "ST")` and the patches' wiring removed.

**Gate:** a fresh clone builds the recompiler with nothing fetched, and
`git status` shows no disc data. Move "What is on the disc" above into
`docs/RECOMPILATION.md`.

## ~~Phase 1: recompile~~ (done 2026-11-09)

Verdite3's order, which matters (`docs/RECOMPILATION.md` there, "The function
maps"):

1. **Read the stub**: the order it runs the six in, and how one hands over to
   the next. Then each executable's end of code (the last `jr ra` that no `jal`
   lands past) and where its text starts with data.
2. **Sweep** a function map per executable into `config/funcmaps/` (`main`,
   `str`, `st`, `title`, `optxt`, `vs`, `end`); cut each at the end of code;
   then `add_call_targets.py`; then `merge_branch_spans.py`. Run the false-start
   check (a start nothing calls that the instruction before runs into).
3. **Name the SDK**: `setup_tools.sh --signatures`, `--autoconfigure` per
   executable, `merge_sdk_names.py --auto`. Bind the same 21 entry points the
   siblings bind (`VSync`, the libcd set, `DrawSync`, `DrawOTag`, `PutDrawEnv`,
   `PutDispEnv`, the `St*` ring, `DMACallback`) in every executable that links
   them; find one executable's by signature and hand, then the others by
   per-object delta. Record the interrupt-callback table per overlay for
   `Program.cs`.
4. **The area modules**: score the load base over the 36 modules' pointers
   (slot targets landing on a prologue or just after a `jr ra`), find the
   dispatch-slot header, declare each as an overlay with `"skip": 0`, sweep from
   the first code byte with the slot targets merged in. Find how `ST.EXE` names
   the destination if it can be read.
5. Write `config/st.json` and recompile.

**Gate:** the recompiler reports its counts (`applied N patches, 0
reimplementations`) with no unresolved jump table left unexplained.
**Writes:** `docs/RECOMPILATION.md`.

## ~~Phase 2: boot, reach play, save and load~~ (gate passed 2026-11-09; by-eye checks open, `docs/TODO.md`)

Run with the log channels on and fix what stops it, in the order the disc runs:
the movies (`STR.EXE`), the title, the opening text, a new game in the first
area.

Expected stops, each a finding for `docs/RECOMPILATION.md` or a numbered fork
patch (the next free number after `0116`, in its own commit):
- an SDK routine the runtime has no HLE for (pad, card, XA, MDEC variants);
- an unmapped call into an area module (the base or a slot is wrong);
- the vblank count: check it is delivered once a vblank and what rate the game
  holds itself to;
- XA playback: the filter, the channel, looping, and stopping at an area change.

Then walk, change areas, save `BASLUS-00863FS_ST` to card A, load it in game and
from the title, die once, and reach `END.EXE` by a boot switch (`ST_BOOTEXE`).

**Gate:** the acceptance test, written in `docs/DEVELOPMENT.md` as the siblings'
is: boot to an area, change areas, save, load both ways. **By eye:** the
picture, the movies, the menus, the save screens, and whether the audio sounds
right.

## ~~Phase 3: drive it without a person~~ (gate passed 2026-11-09; the fuzzer and arrival points open, `docs/TODO.md`)

Port the agent harness before any enhancement, as both later ports did:
`AgentBeacon`, `AgentServer` (the command channel), `AutoStart`, the scripted
pad, and `BootExe`; from Verdite1, `Warp` and the headless fuzzer (`Fuzz.cs`,
`scripts/fuzz.py`, fork `0112`) once a warp exists. Each needs this game's
state addresses (which executable, which area, the player's cell, HP), which is
the first real entry in `docs/GAME_INTERNALS.md`. `ST/FN.` names many of them.

**Gate:** the acceptance test runs as a program, twice, with the same result.
Area changes, saving and the title load may stay by hand at first, as they did
in Verdite3.

## ~~Phase 4: the main loop, pacing, and what runs at the drawn rate~~ (gate passed 2026-11-10; by-eye checks open, `docs/TODO.md`)

1. **Survey `ST.EXE`'s main loop** stage by stage (`StageProbe`, `RateCensus`,
   the profiler's known sections) and write it in `GAME_INTERNALS.md` beside
   Verdite3's stage list, with `match_code.py` naming counterparts.
2. **`FramePacing`**: the world at its own rate, drawing at any rate.
   `VBlankPacing` and `LoopPacing` for the loops that draw their own frames
   (menus, loading, message boxes), `OpenPacing`-style handling for the four
   small executables if they need it.
3. **The rate census**: everything that still advances per drawn frame
   (texture scrolls, sprite cels, gauges, fades) is held to the tick.

**Gate:** the world's tick count per second is the console's at 60, 144 and
uncapped; the census lists nothing unexplained. **By eye:** game speed against
a console or an emulator, menus, fades.

## Phase 5: the draw path, surveyed and in C#

This is Verdite1's `RENDERER_PLAN.md` Steps 0 and 1, and everything after rests
on it.

1. **Inventory**: the frame head and present, the camera block, the map walk,
   the model walk and its submitters, the pose blender, the polygon assemblers,
   the arm (two hands here), the HUD, the sky if there is one. `GeometryProbe`,
   `DrawCensus`/`drawing_inventory.py`, `PrimBufferProbe`.
2. **Literal C#** for each, verified with `Differential` to 0 mismatches over a
   scripted session that turns, walks, fights and changes areas, then on by
   default.

**Gate:** every draw routine in the inventory is C#, verified, with the verify
mode kept. **Writes:** `GAME_INTERNALS.md` ("The geometry path"), `GEOMETRY.md`.

## Phase 6: smoothing

On the C# draw path: the camera carried between ticks (`ViewSmoothing`), then
models, creatures and their clip times (`ModelSmoothing`, `MoPose`), the arm's
swing, the HUD's eased gauges, sprite cels held to the tick.

**Gate:** at 144 fps, 144 distinct cameras a second turning, a looping creature
a new pose on every frame, 0 backward steps. **By eye:** all of it. **Writes:**
`SMOOTHING.md`.

## Phase 7: the picture

`NoDither`, `TrueColor`, `Perspective`, `Subpixel`, `ZBuffer`: first through the
runtime's address map, as Verdite1 did, with its coverage measured per routine;
then depth records from the C# assemblers where the map falls short, and the
near path if this game subdivides near polygons.

**Gate:** coverage numbers per routine, verify still 0 with each on.
**By eye:** each switch. **Writes:** `PICTURE.md`.

## Phase 8: input

1. What the game reads: the pad library, digital against analog, vibration,
   and the button mask table.
2. `KeyLayout` and Verdite Core's `Mouse` (mouse look through the game's own
   turn and look velocities), `MenuMouse`.
3. Twin-stick (`Analog`), starting from the game's own stick handling if it has
   one; `Rumble` passed through from the game's own vibration calls if it makes
   them; gyro where a sibling's gate has a counterpart.

**Gate:** a synthetic hand turns exactly its step a tick and stops dead.
**By eye, on a real pad:** directions, feel, sensitivity. **Writes:** `INPUT.md`.

## Phase 9: widescreen

The margin, the screen tints, the cull widened with the aspect (verified cell
for cell), the primitive buffer's headroom, the HUD at the edges, the world
live behind menus (`MenuWorld`).

**Gate:** cells drawn at 16:9 against 4:3, no primitive-buffer overflow in the
busiest area. **By eye:** the edges while turning, fades and flashes.
**Writes:** `WIDESCREEN.md`.

## Phase 10: settings and first run

Verdite Core's settings store, the Settings window's pages, `Settings ▸ Testing`
with every switch live (built early: add each switch to it as its phase lands),
the page in the game's own menu, and first-run setup (`config/onboarding.json`).
`docs/ENV_VARS.md` lists every `ST_*` switch from the first one.

Open at the start: the fork reads seven `KF2_*` switches by name. Taking the
prefix from the game is a fork change that must leave Verdite2's acceptance
test passing; do it here or keep reading them through `Program.cs` wiring.

## Phase 11: the retained GPU renderer

Verdite3's native scene submission and retained renderer (`../verdite3/docs/GPU_RENDERER.md`,
built from `../KFII-PC/docs/VERDITE3_GPU_PLAN.md`), re-derived for this game:
Verdite3 is a recompilation like this one, so its route fits; Verdite1's renderer
plan leans on a decompilation this port does not have. A scene census, literal
native submitters with a full verify, retained map, meshes and poses, the main
view on the GPU, then lighting, fog and the buffers. The plan is
`docs/RENDERER_PLAN.md` (written 2026-11-10).

**Gate:** native comparisons in every loaded area, no packet fallback for the
main view, frame time measured. **By eye:** everything.

## Phase 12: the rest of the game, and its own features

- **Quality of life**: reload the last save on death (`AutoReload`), a full
  card handled (`FullCard`), pause without focus (Core's `FocusPause`),
  `GearCompare` with durability, the window icon off the save's card icon.
- **Debug tools** as a mod (`mods/stdebug`): noclip, invincibility, the
  character and item editors, area warp, enemies ignore you.
- **`VS.EXE`**: its own loop and draw path. Carry pacing and the picture over
  if its routines match `ST.EXE`'s (`match_code.py`); otherwise it runs as the
  console did.
- **The small executables**: movies at full quality, the ending held with a
  way back to the title.
- **Candidates, each the user's call**: an automap (the game has none;
  Verdite2's is in `../docs-archive/AUTOMAP.md`), Discord presence
  (Verdite1's), the Japanese disc's text over the US disc (Verdite3's).

## Phase 13: release

`VerditeST.Launcher/` (Core's launcher with a `LauncherGame` for this disc:
serial `SLUS-00863`, the six executables and the `.T` files checked from the
recompiler config), `packaging/package.env` with a **new** `INNO_APP_ID`, the
one-line wrappers, the CI and release workflows, `docs/PACKAGING.md`. Because
the volume id is empty, check that the disc check does not depend on it.
Robustness before a release: crash reports, the fuzzer's campaign, `Prejit`,
a soak through every area.

**Gate:** the AppImage builds the game from an empty data folder and passes the
acceptance test. **Ask first:** the repository, the tag, the push.

## Per-phase bookkeeping

At the end of each phase: update `NOTES.md`'s status paragraph and
`docs/TODO.md`; record each shared-repo commit made and which games pin what;
state what was measured with its numbers, and list what the user still has to
look at. Strike the phase through here.

## Sources

- Verdite2 (`../KFII-PC`): `docs/SHARING_PLAN.md`, `docs/SHARING.md`,
  `docs/VERDITE3_GPU_PLAN.md`, `docs/RECOMPILATION.md`.
- Verdite3 (`../verdite3`): `AGENTS.md`, `NOTES.md`, `docs/RECOMPILATION.md`,
  `docs/DEVELOPMENT.md`, `docs/TODO.md`, and its first 40 commits.
- Verdite1 (`../verdite1`): `docs/RENDERER_PLAN.md`, `docs/TODO.md`,
  `docs/MANUAL_PASS.md`.
- Verdite Core's `README.md`; the fork's `docs/RECOMPONE_PATCHES.md`.
