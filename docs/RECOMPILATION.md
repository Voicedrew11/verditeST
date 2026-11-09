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
