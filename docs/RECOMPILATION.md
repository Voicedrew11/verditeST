# Recompilation: config, function maps and SDK addresses

How the recompiler turns the disc's MIPS into C#: the overlays in `config/st.json`,
the swept function maps under `config/funcmaps/`, and mapping PSY-Q entry points by
address to the runtime's HLE. Verdite3's `docs/RECOMPILATION.md` is the model. No
address in it applies here.

## What is on the disc

Read 2026-11-09 with Verdite Core's `inspect_disc.py` and `extract_file.py`.
`Shadow Tower (USA).bin` is 488,105,856 bytes, SHA-1
`df441bf08b7079f8b3d36f1188f7eedd8cb73c50`, one MODE2/2352 data track, no audio
tracks. The volume id is empty, so the disc check goes by `SYSTEM.CNF`, which
boots `cdrom:\SLUS_008.63;1` (`TCB = 4`, `EVENT = 32`, `STACK = 801ffff0`).

| file | LBA | file size | entry pc | text addr | text size |
|---|---|---|---|---|---|
| `SLUS_008.63` | 38 | 0x1000 | `0x800101C0` | `0x80010000` | 0x800 |
| `PSX.EXE` | 40 | 0x1000 | (byte-identical to the above) | | |
| `STR.EXE` | 42 | 0x22800 | `0x800119DC` | `0x80011000` | 0x22000 |
| `ST.EXE` | 111 | 0x78000 | `0x80014320` | `0x80011000` | 0x77800 |
| `TITLE.EXE` | 351 | 0x17800 | `0x80012A04` | **`0x80012000`** | 0x17000 |
| `OPTXT.EXE` | 398 | 0xF000 | `0x80011944` | `0x80011000` | 0xE800 |
| `VS.EXE` | 428 | 0x53000 | `0x80012980` | `0x80011000` | 0x52800 |
| `END.EXE` | 594 | 0x1F800 | `0x80011960` | `0x80011000` | 0x1F000 |

**The King's Field shape, with six executables instead of three.** A 4 KiB stub
holds the six names (`VS`, `END`, `ST`, `OPTXT`, `TITLE`, `STR`), and the six
overlap in RAM, so each is an overlay with `"skip": 2048`. `TITLE.EXE` alone
loads at `0x80012000`. By their strings: `STR.EXE` plays the movies
(`AGETEC.STR`, `OPEN.STR`, `DEMO.STR`), `TITLE.EXE` is the title, `OPTXT.EXE`
the opening text, `ST.EXE` the game, `END.EXE` the ending and staff roll,
`VS.EXE` a second game mode with its own data (`ST/VS/`). `STR`, `TITLE`,
`OPTXT` and `END` each carry all six names again, so they chain to one another;
**who starts whom, and with what arguments, is not read yet.**

**`ST.EXE` loads code, as `GAME.EXE` did.** `ST/COM/FDAT.T` is the same archive
(`u16` count, then `u16` start sectors): 809 entries in groups of ten, and entry
`10n+2` holds MIPS code for exactly 36 values of n from 0 to 47. The other
twelve (7, 16, 19, 22, 29, 31, 34, 35, 36, 39, 42, 43) are exactly the twelve
maps whose `ST/CHRn/Mnn.T` is a 2,048-byte placeholder. So: **36 area code
modules**, 10 to 18 KB each (Verdite3's were 2 to 4 KB), about 144 RAM pointers
apiece. Their load base is not known; score it as Verdite3's was.

The rest:
- `ST/COM/*.T`: `MO` (900 entries), `FDAT` (809), `STAT` (14), `CRT` (411,
  creatures, no code), `ITEM` (256), `ICON`, `EQUIP`, `TALKMSG` (600).
- `ST/CHR0`-`CHR4/M00.T`-`M47.T`: per-map archives, no code.
- `ST/XA0`, `ST/XA1`: 40 XA files named by number, plus `HAM` and `SHOP0`-`3`.
  The game has no sequenced music; **streamed XA is the audio that matters**.
- `ST/STR/*.STR` (five movies), `ST/TITLE`, `ST/OPTXT`, `ST/VS` (each a `.MIC`
  or `.T` and an `.XA`), and `LOAD02.TIM` at the root.
- `ST/FN.`: a 12 KB text file of named start-up parameters (`floor number`,
  `player appear loc x/y/z`, `player hp`, ...) with Shift-JIS notes. A lead for
  the warp, the debug tools and `GAME_INTERNALS.md`; whether the retail game
  reads it is not known.

What the executables say about themselves:
- PSY-Q `sys.c 1.140` of 1998-01-12, `bios.c 1.86`, `intr.c 1.76`: **newer than
  both King's Field discs' libraries**, so no sibling's SDK address, delta or
  normal form applies, and library routines the runtime has never met are
  likely (below).
- The save is `BASLUS-00863FS_ST`, in `ST.EXE` and `VS.EXE`.
- `ST.EXE` names its map files by pattern (`CHRX\MXX.T`) and every XA by path.

Leads from outside, to re-measure before writing down: `ivandsm/kingsfieldre`
says its `tfile_tool` works on Shadow Tower's `.T` files, and FSModTool reads
the game's demo. **No decompilation of this disc was found**; look again before
the renderer work, since Verdite1's turned up late.

## Status

Recompiled 2026-11-09: the boot stub, the six executables and 36 area modules,
4027 functions, `applied 199 patches, 0 reimplementations`. `generated/` compiles
with no errors.

## How one program starts the next

The stub (`SLUS_008.63`) is a loop over six names. The table at `0x80010350` is
indexed by `*0x80010394`, which starts at 0:

| index | program | sets the next index to |
|---|---|---|
| 0 | `STR.EXE` (movies) | 1 |
| 1 | `TITLE.EXE` | 0, 2, 3 or 5, by what was chosen |
| 2 | `OPTXT.EXE` (opening text) | 3 |
| 3 | `ST.EXE` (the game) | 4 |
| 4 | `END.EXE` | 1 |
| 5 | `VS.EXE` | 1 |

The stub calls BIOS `Load` (A0 `0x42`) and `Exec` (A0 `0x43`) with
`cdrom:NAME.EXE;1`, and each program ends by returning from `Exec`. Before
returning it writes the next index to the byte at `0x800103F0`, and the stub copies
that byte to `*0x80010394`. The two bytes after it, `0x800103F1` and `0x800103F2`,
are passed the same way: `ST.EXE` reads `0x800103F1` and compares it
with 2, and clears both when it leaves. `STR`, `TITLE`, `OPTXT` and `END` reach
these bytes through pointers at `0($gp)` onward; `ST` and `VS` use the addresses
directly.

## The function maps

Rebuilt from the disc by `scripts/make_funcmaps.py`, then:

```bash
python3 tools/verdite-core/scripts/merge_branch_spans.py
python3 tools/verdite-core/scripts/merge_sdk_names.py --auto scratch/auto/funcmaps
```

| map | swept | after the cut | after harvest | after merge |
|---|---|---|---|---|
| `main` | 10 | | 11 | 11 |
| `str` | 323 | 320 | 321 | 321 |
| `st` | 1134 | 1131 | 1138 | 1123 |
| `title` | 387 | 386 | 387 | 387 |
| `optxt` | 283 | 282 | 283 | 283 |
| `vs` | 664 | 661 | 668 | 662 |
| `end` | 278 | 275 | 276 | 276 |

**Each executable's text starts and ends with data.** The sweep's first real
function follows the entry point, and everything before the entry point is
read-only data. The last real function ends at `0x8001FDFC` in `STR.EXE`,
`0x8007F3E4` in `ST.EXE`, `0x8002690C` in `TITLE.EXE`, `0x8001DF50` in `OPTXT.EXE`,
`0x8004A218` in `VS.EXE` and `0x8001D280` in `END.EXE`; the sweep's starts past
those do not end in `jr ra`, and no `jal` lands there. The cut comes before the
harvest, or `add_call_targets` reads `jal`s out of the data.

**One false start.** `0x80042FF0` in `VS.EXE`, in the middle of straight-line
matrix code, is a start the sweep took from a `jal`-shaped data word: nothing calls
it and the instruction before runs on into it. `make_funcmaps.py` rejoins it. The
check (a start nothing references that the instruction before runs on into) finds
none now in any of the 43 maps.

`ST.EXE` has its own, real, `0x80042FF0`; the two are unrelated.

## The SDK entry points

`--autoconfigure` (upstream's PSY-Q signature bank) named 276 of 1134 functions in
`ST.EXE`, 268 of 664 in `VS.EXE`, 231 of 387 in `TITLE.EXE`, 165 of 323 in
`STR.EXE`, 151 of 278 in `END.EXE` and 145 of 283 in `OPTXT.EXE`.
`merge_sdk_names.py` wrote 923 of those into the maps and left 313 unnamed because
`SdkPatches` would bind them.

`patches[]` binds the 21 entry points the siblings bind, wherever an executable
links them, 105, the 13 `libpad` routines in the four programs that link them
(below), 52, and seven more `libcd` routines in all six programs (below), 42: 199.
Every address came from a signature, and the distances between
routines of one library agree across all six programs (`CdSync` to `CdInit`
`+0x244`, `DrawOTag` to `DrawSync` `+0x5F4`, `PutDispEnv` to `PutDrawEnv` `+0x1CC`,
`CdReadSync` to `CdRead` `+0x100`).

| function | `str` | `st` | `title` | `optxt` | `vs` | `end` |
|---|---|---|---|---|---|---|
| `VSync` | `8001908C` | `80077104` | `8001FA0C` | `80017220` | `80044024` | `80018BFC` |
| `CdInit` | `8001366C` | `80068A34` | `8001442C` | `800130F0` | `8003614C` | `8001313C` |
| `CdSync` | `800138B0` | `80068C78` | `80014670` | `80013334` | `80036390` | `80013380` |
| `CdReady` | `800138D0` | `80068C98` | `80014690` | `80013354` | `800363B0` | `800133A0` |
| `CdControl` | `80013918` | `80068CE0` | `800146D8` | `8001339C` | `800363F8` | `800133E8` |
| `CdControlF` | `80013A54` | `80068E1C` | `80014814` | `800134D8` | `80036534` | `80013524` |
| `CdControlB` | `80013B88` | `80068F50` | `80014948` | `8001360C` | `80036668` | `80013658` |
| `CdGetSector` | `80013CF4` | `800690BC` | `80014AB4` | `80013778` | `800367D4` | `800137C4` |
| `CdReadSync` | `8001667C` | `8006BA44` | `8001743C` | `80016100` | `8003915C` | `8001614C` |
| `CdRead` | `8001657C` | `8006B944` | `8001733C` | `80016000` | `8003905C` | `8001604C` |
| `DrawSync` | `8001A518` | `80079A60` | `80021008` | `8001864C` | `80047390` | `8001A518` |
| `DrawOTag` | `8001AB0C` | `8007A054` | `800215FC` | `80018C40` | `80047984` | `8001AB0C` |
| `PutDrawEnv` | `8001AB7C` | `8007A0C4` | `8002166C` | `80018CB0` | `800479F4` | `8001AB7C` |
| `PutDispEnv` | `8001AD48` | `8007A290` | `80021838` | `80018E7C` | `80047BC0` | `8001AD48` |
| `DMACallback` | `800192FC` | `80077374` | `8001FC7C` | `80017490` | `80044294` | `80018E6C` |
| `StSetRing` | `8001363C` | | | | `800393FC` | `8001310C` |
| `StClearRing` | `8001682C` | | | | `8003942C` | `800162FC` |
| `StSetStream` | `800169FC` | | | | `8003948C` | `800164CC` |
| `StUnSetRing` | `8001688C` | | | | | `8001635C` |
| `StGetNext` | `80016B7C` | | | | | `8001664C` |
| `StFreeRing` | `80016A8C` | | | | | `8001655C` |

The signature bank also matched `DMACallback` at `0x8005735C` in `ST.EXE`. That is
a thunk of the same shape through a different pointer (`*0x801A592C + 4`), the
game's own, and is not bound. The real one calls through `*0x80087B90 + 4`, as
`VSyncCallback` and `InterruptCallback` beside it do.

The libraries, by signature: `libpad` in `STR`, `TITLE`, `OPTXT` and `ST`; `libcard`
in `ST` and `VS`; `libpress` in `STR` and `END`. `VS.EXE` links only three of the
`St*` ring routines and `ST.EXE` none.

### The disc library

Bound by the 21 names alone, `ST.EXE` froze after "Now Loading": it waits in
`func_80015AD4` for its disc queue to empty (see "The disc queue" in
`docs/GAME_INTERNALS.md`), and the queue is driven by `CdSyncCallback` and
`CdReadyCallback`. Those two were left recompiled, so they wrote libcd's own
variables, and the bound `CdControl` never saw a callback to fire. `patches[]` now
also binds `CdSyncCallback`, `CdReadyCallback`, `CdReadCallback`, `CdDataCallback`,
`CdStatus`, `CdMode` and `CdLastCom` in all six programs (42 entries, 199 patches):
each reads or writes state that only the bound routines keep. `CdSync` to
`CdSyncCallback` is `+0x40` in all six. `CdSearchFile` stays recompiled, because it
works through `CdRead`.

Two runtime defects showed up behind that one, both in `LibCd` (patches 0118 and
0119 in `tools/RecompOne/docs/RECOMPONE_PATCHES.md`):
- The real `CdRead` sizes the copy by the caller's mode but runs the drive with
  `CdlModeSize1` (`ori 0x20` at `0x8006B9B8`), and the drive stays that way. The
  queue's ReadN relies on it: its sector callback takes a 3-word header, 0x200 data
  words and 0x46 trailing words.
- The runtime stepped one sector on after a sector callback, even when the callback
  had just issued a seek.

### The pad library

`STR`, `TITLE`, `OPTXT` and `ST` link the newer `libpad`. Left recompiled, `ST.EXE`
froze before its first frame: `func_80057428` polls `PadGetState(0)` until it reads
6 or 2, with a 60-vblank timeout through `VSync(-1)`. The pad never reported ready,
and on the port's timeline `VSync(-1)` only advances when a frame is presented, so
the timeout never ran out. The 13 routines `SdkPatches` knows (`PadInitDirect`
through `PadSetAct`) are bound by address in those four programs. `merge_sdk_names`
leaves their names off, so the binding has to be in `patches[]`. Any other wait on
`VSync(-1)` before the first frame would freeze the same way.

### The interrupt-callback table

The same 4.x interrupt manager as the King's Field discs, read here from each
program. A pointer next to `InterruptCallback` leads to a struct whose `+8` is
`setIntr` and `+0x1C` the interrupt environment; `setIntr` indexes the table at
`env + 4` by `irq * 4`, and `Program.cs` sets it per executable.

| overlay | `setIntr` | env | table |
|---|---|---|---|
| `str` | `0x800196D8` | `0x80031524` | `0x80031528` |
| `st` | `0x80077750` | `0x80086B08` | `0x80086B0C` |
| `title` | `0x80020058` | `0x80027734` | `0x80027738` |
| `optxt` | `0x8001786C` | `0x8001E430` | `0x8001E434` |
| `vs` | `0x80044670` | `0x80061380` | `0x80061384` |
| `end` | `0x80019248` | `0x8002EA3C` | `0x8002EA40` |

## ST.EXE loads code

`ST/COM/FDAT.T` entry `10n+2`, for the 36 maps that have a module, is **linked for
`0x801EB560`**. Scoring every base by how many slot targets and internal `jal`s
land on a prologue or just after a `jr ra` gives that address for all 36, 962
points against 260 for the runner-up. `ST.EXE` confirms it: its start-up clears BSS
up to `0x801EB560`, so the modules load at the end of the program's own memory.
Observed 2026-11-09: `fdat002` loads on the way into the first area.

A module is:

| offset | what |
|---|---|
| `0x00` | an id: `n + 5` (`0x37` in maps 0, 32 and 40) |
| `0x04` | 32 dispatch slots |
| `0x84` | a second id |
| `0x88` | 81 pointers: the case labels of a switch in the module's code |
| `0x1CC` | in most modules, a few words of data and a second table of 31 case labels; in maps 44 to 47, the code itself |
| `0x1CC`-`0x28C` | the code, from wherever the lowest referenced address is |

The modules are 10 to 18 KB and make internal `jal`s. After the last `jr ra` comes
data, which the sweep does not reach because `make_funcmaps.py` stops there. Each
overlay is declared with `"skip": 0` and the base as `base`, so it covers the
header and the two tables. `merge_branch_spans` folds the case labels back into
their switch functions; each module resolves 112 table entries (81 and 31), 81 in
maps 44 to 47 and 119 in map 27. The slot-dispatch code in `ST.EXE` is not read yet.

## Indirect jumps nothing resolves

The gate asks that no unresolved jump table be left unexplained. Apart from the
BIOS thunks (`addiu t2, 0xA0/B0/C0; jr t2`), the recompiler left three register
jumps without a table:

- `func_800659E4` in `ST.EXE`: a duplicate of a label inside `func_80065770` that
  the escape scan emits as a separate method. The parent resolves this switch at
  `0x80065A58`. As in Verdite3, no other function branches to the duplicates (12 in
  `st`, 9 in `vs`), so they are not expected to run.
- `func_80068854` in `ST.EXE` and `func_80035F6C` in `VS.EXE`: `libcard`'s jump to
  `0xA000DFAC`, an address in BIOS memory. No `jal` reaches either of them. If the
  card library needs them it will show at Phase 2, when saving is tried.
