# Game internals

The reverse-engineered game: the main loop and its stages, player state, movement,
areas, saves and the boot stub. Every address and routine this project learns is
written here rather than left in the commit that found it. Verdite3's
`docs/GAME_INTERNALS.md` is the model, but none of its addresses apply.

## The main loop

Read 2026-11-09 from the disassembly and measured with `ST_STAGEPROBE=1`.
`func_80014A30` is the session: it initialises the libraries, opens the vblank
event, runs the start-up card screen or a new game, loads the first area
(`func_80044EE8`), then runs **the main loop at `0x80014BD4`**, fourteen calls an
iteration, while the word at `gp+0x2E4` (`0x800881A4`, `gp` is `0x80087EC0`) is 0.
2 and 3 leave it; it then waits `VSync(10)` twice, sets the stub's selectors at
`0x800103F0..F2` and returns to the stub. `patches/MainLoop.cs` holds the table.

| # | function | call site | what is known | Verdite3's counterpart (`match_code.py`) |
|---|---|---|---|---|
| 1 | `func_800164B4` | `0x80014BD4` | once 60 vblanks have passed, copies the frames drawn since (`0x8017BD50`) to `0x8017BD51` and zeroes it; nothing reads `0x8017BD51` | |
| 2 | `func_80016508` | `0x80014BDC` | also called at start-up | stage 2 `func_80034180` (0.49) |
| 3 | `func_80042D74` | `0x80014BE4` | **the player's tick**: writes the heading (`+0x268`) and, through `func_80041150`/`func_8004162C`, the position; the menu, the load and the rumble calls run inside it | stage 4 `func_80030FCC` (0.23, weak) |
| 4 | `func_80050768` | `0x80014BEC` | the area's object records, doors among them ("Changing areas") | |
| 5 | `func_8004E324` | `0x80014BF4` | | |
| 6 | `func_80053AA4` | `0x80014BFC` | | |
| 7 | `func_8003BC40` | `0x80014C04` | | stage 6 `func_8005BC50` (0.73) |
| | (inline) | `0x80014C10` | `sb 0 → player+0x21C` (`0x80199144`) | the inline store between Verdite3's stages 6 and 7 |
| 8 | `func_8005735C` | `0x80014C0C` | **the area module's tick**: calls slot 1 of the module header through `*0x801B592C + 4` (`0x801EB564`, the module's slots) | |
| 9 | `func_800456F8` | `0x80014C14` | the area loader's state machine (`0x801B5920`) | stage 8 `func_80018358`, the loader (0.31) |
| 10 | `func_8003E734(sp+0x18, sp+0x28)` | `0x80014C20` | fills the two buffers stage 14 draws with | stage 10 `func_8002B330`, the camera (0.91) |
| 11 | `func_80058288(sp+0x18, sp+0x28)` | `0x80014C2C` | | stage 11 `func_800156BC` (0.66) |
| 12 | `func_80045FFC` | `0x80014C34` | parses streamed images, 16 sectors a call ("The disc queue") | stage 13 `func_80018CD0` (0.68) |
| 13 | `func_80058620` | `0x80014C3C` | | stage 14 `func_80015A48` (0.64) |
| 14 | `func_80026AFC(sp+0x18, sp+0x28)` | `0x80014C48` | **builds and draws the frame**: the visible-cell list (`func_8001C100`), the scene, the HUD, the frame swap `func_80017680`, then the frame gate `func_8001535C`, then `0x8017BD50`++ | stage 15 `func_800422B8` |

**Only stage 14 writes the ordering table.** Measured 2026-11-10 in map 0, 60 calls a
second each: stages 1 to 13 changed 0 of its 4096 words and presented no frame;
stage 14 wrote all 4096 and presented one frame a call. Stage 14 is also called by
loops that draw their own frames: `func_80026DD4`, `func_80042CFC`, stage 3 at
`0x80043134` (the menu), `func_800500AC` (after the loop ends) and `func_8005335C`.

`player+0x21C` is set by stages 3 to 5 (`func_80050768`, `func_80052894`) and read by
stage 5 (`func_8004E324`), so it is set and consumed within one iteration and the
inline clear needs no care under pacing, unlike Verdite3's arrival flag.

### The frame swap, the frame gate and the vblank

`func_80017680` is the swap: `func_800175A4` (`DrawSync(0)`, **`VSync(3)`**,
`PutDispEnv` and `PutDrawEnv` for buffer `*(u8*)0x8017BD52`), then `func_80017634`
(`DrawOTag(*0x80165174 + 0x3FFC)`, 0x1000 entries). Menus call `func_800175A4` and
`func_80017634` directly too.

**`func_8001535C` is the frame gate**: while the u32 at `0x8008820C` is 0, `VSync(0)`;
then it writes 0. One vblank at least between two passes.

The count is bumped by **`func_800153B4`, the vblank handler**, which
`func_80016374` registers: `OpenEvent(0xF2000003 RCntCNT3, 2, 0x1000,
func_800153B4)`, `EnableEvent`, `SetRCnt(.., 60000, ..)`, `StartRCnt`. Each call
increments `0x80088204` (vblanks in all), `0x8008820C` (the gate's count) and
`player+0x33`, which at 60 is zeroed and bumps the play-time seconds at
`player+0x34` (`0x80198F5C`); then it calls `func_80057744`, the pad.

**So the console runs the game at 20 frames a second at most**, and since every
stage runs once a frame, 20 Hz is also its world's speed: `VSync(3)` returns once
three vblanks have passed since the last `VSync`. The runtime's non-blocking `VSync`
presents and returns for any mode, so **the port ran the whole loop at 60** until
2026-11-10 (the game's own count at `0x8017BD51` read 60, and holding Up for a second
walked about 10,500 units against about 4,300 now). `patches/VBlankPacing.cs` makes
`VSync(n)` wait n vblanks in every program; `VS.EXE`'s swap is the same
(`func_8001D298`, `VSync(3)`), so it ran at 60 too. The other counted waits are
`VSync(3)` or `VSync(4)` between two CD commands, in every program, `VSync(10)` in
`ST.EXE` and `END.EXE`, and `VSync(60)` in `STR.EXE`.

### What runs at the drawn rate

Measured 2026-11-10 with `ST_RATECENSUS=30` at `ST_FPS=144`, standing in map 0 with
nobody at the pad: the RAM words that change between two frames on which no stage ran,
sampled at the frame boundary (an earlier census sampled from the vblank, which the
interrupt poll delivers in the middle of stage 14, and caught its scratch lists half
built). Before `patches/DrawStageTick.cs`:

| words | what | |
|---|---|---|
| `0x8015C488` | `func_800255F0`'s frame counter: each entry of its cel table (`0x8015C48C`, 128 of 0x18 bytes: `+3` the cel count, `+4` the frames a cel is held, `+5` the cel shown) steps when the counter is a multiple of `+4` | held to the tick |
| `0x8017BE88` | `func_80020A80` calls `rand()` (`func_80078AC4`, BIOS A `0x2F`) every frame and, when `0x8017BE8C & 2`, sets the light's colour from it: a flicker | held to the tick |
| `0x80088204`, `0x8008820C`, `0x80198F58`, `0x801CD190`, `0x80087BD0`, `0x80087DFC`, `0x801E95B4` | the vblank handler's counts, the pad, libetc's and libgpu's own counters, the sound library: 39-42% of pairs, 60 of 144 | wall clock, as on the console |
| `0x80165174`, `0x80165378`, `0x80165394`, `0x8017BD50` | the buffer being drawn, the frames-drawn count | per frame by nature |
| six words in `0x801FEBB4..0x801FFEF4` | stack slots | |

Read from stage 14 and not in the census, because nothing was rumbling:
`func_80057B10` counts down `0x801CD199` once a frame and stops the motors
(`0x801CD175`, `0x801CD177`) at 0. `func_80057A68(strength)` starts them, for
`strength / 16 + 5` frames; it is called from stages 3, 4 and 7. Held to the tick.

`func_800246E4`, also in stage 14, calls `rand()` too, but only when a timer kept in
vblanks (`0x80088204`) runs out, so it runs on the wall clock and needs nothing.

With the three held, the census lists only the last three rows. One area, standing:
an area with other animations, or a fight, may show more.

### The draw path

What stage 14 calls, which routine writes which packets, and the loops that present
frames of their own (the menu, the death prompt, a blank frame entering an area) are
in `docs/GEOMETRY.md`.

## The disc queue

`ST.EXE` loads in the background through a ring of 0x34-byte requests at
`0x80088214`, ending at `0x80088BD4`. The write pointer is at `0x80088BD4` and the
read pointer at `0x80088BD8`. `func_80015BCC` adds a request and `func_80015AD4`
waits, drawing frames, until the ring is empty.

| offset | what |
|---|---|
| `0x00` | operation: 0 is a free slot, 2 a checked read, 3 a streamed image, 4 seen, not read yet |
| `0x01` | state: 0 seeking, 1 reading, 2 finished |
| `0x10` | set to 1 by the op-3 completion callback (`func_800443F4`) |
| `0x18` | disc position |
| `0x1C` | where the next sector goes |
| `0x20` | destination |
| `0x24` | sectors left |
| `0x28` | sector count |
| `0x2C` | completion callback |

`func_8001561C` is the sync callback and `func_800154E8` the sector callback. Each
sector is read as a 3-word header, 0x200 words of data and 0x46 words discarded.

- **Op 2** verifies with `func_80015B78`: `0x12345678` plus every word but the last
  must equal the last word. A failure seeks and reads the whole request again, with
  no limit.
- **Op 3** is parsed once a frame by `func_80045FFC`, 16 sectors at a time: 16-byte
  records whose first 8 bytes are repeated in the next 8, each followed by pixels
  sent with `LoadImage`. A record that does not repeat restarts the request, also
  with no limit, so bad data shows up as an endless loop and not as a crash.

## The player

One block at `0x80198F28`, used by most of `ST.EXE` through that base address. The
position was found by comparing RAM across walking and turning; HP and MP by matching
the save against RAM (the save stores 752 and 550 once each, and the block holds both
twice) and then by the routines that change them; the axes by the door code, which
writes all three.

| address | offset | what |
|---|---|---|
| `0x80198F28` | `+0x000` | u16, maximum HP (752 in a new game) |
| `0x80198F2A` | `+0x002` | u16, HP |
| `0x80198F2C` | `+0x004` | u16, maximum MP (550) |
| `0x80198F2E` | `+0x006` | u16, MP |
| `0x80198F8A` | `+0x062` | 14 u16s drawn as a list with bars by `func_8002E94C` (62, 128, 50, 50, 98, 59, 57, then 50s): attributes, not named yet |
| `0x80199140` | `+0x218` | byte, the player's state: 0 free to act (the in-world action runs only then), 4 dead |
| `0x80199170` | `+0x248` | X, s32 |
| `0x80199174` | `+0x24C` | Y, s32: the height of the eye, larger is lower. Recorded as Z before 2026-11-09 |
| `0x80199178` | `+0x250` | Z, s32 |
| `0x80199190` | `+0x268` | heading, u16: `func_8003BD14` masks it with `0x7FF`; one press of Left took it from 0 to `0xFDE0` |

Positions are 0x1000 to a cell. A new game starts at (`0x40000`, `0x415C0`,
`0x3F000`).

- **`func_8003CF28(delta)`** adds `delta` to HP, clamps it to the maximum, and at 0 or
  below stores 0 and calls **`func_8003BD14`**, which sets the state byte to 4 and
  starts the fall (`+0x17A`, `+0x17C`). `func_8003CF94(delta)` is the same for MP,
  without the death.
- **Dying**, tested: with the state at 4 the game waits; the next Cross goes to
  `TITLE.EXE`, and Start there begins a new game at full HP.
- `ST/FN.` on the disc is a debug start-up file (floor, appear location, HP and MP "0:
  from the equipment", 999 of each item, a "super mode flag"). Nothing in `ST.EXE` or
  `VS.EXE` names it, so the retail game does not read it. Some of its text turns up in
  a save, as leftover buffer contents.

## Pad state

`0x801CD180` is the game's pad block. `+0x04` (u16) is the buttons newly pressed,
latched by the vblank handler and cleared by whoever consumes them; `+0x0C` is a second
set the cursors read (repeat-filtered, by the look of it); `+0x36` is the bound action button (`0x4000`,
Cross). Bits are the PlayStation's: Up `0x10`, Right `0x20`, Down `0x40`, Left `0x80`,
L1 `0x400`, Circle `0x2000`, Cross `0x4000`.

## The menu, saving and loading

Select opens the menu (`func_80058EF4`), cursor at `0x1F8002DE`, eight items. Item 6
is **Load** (`func_80062C28` with state 1, tested); item 7 is a yes/no dialog
(`func_80063078`), not Save, and what it asks is not read. Items 0 to 5 are other
screens, not read.

`func_80062C28(a0)` is the card screen, for both directions. `func_80058DC4(a0, a1)`
prepares it and `func_80058ED4` undoes it; **`func_80058DC4`'s `a0` picks the
direction**: non-zero starts `0x1F8002DC` at `0x100` (save), zero at 1 (load). That
word is a one-hot step counter, and `& 0xFF00` means "saving" throughout the routine.

| caller | `DC4` args | direction |
|---|---|---|
| `func_80055374`, at `0x80055918` | 1, 1 | save, in the world |
| `func_800633D4`, at `0x80063554` | 1, 0 | save, from `func_80014A30` when `gp+0x2E4` is 3 |
| `func_80042D74`, at `0x80043A64` | 0, 1 | load, after the yes/no at `func_80063260` |
| `func_80014A30`, at `0x80014B14` | 0, 0 | load, on start-up |

**What makes the player save is not found yet.** `func_80055374` is reached from
`func_80042B9C` when the action button is pressed and `func_80053174` finds a target,
so it is probably an object in the world.

The message at `0x1F800000` is a text id. `0x160`, no save on the card, was seen in
a run. Read from the code and not seen: `0x15E` no card, `0x15F` an unformatted card,
`0x161` no room. The screen's prompts spin on
`0x801CD184` with no `VSync` in the loop, which is why `Program.cs` sets
`LibEtc.VBlankFromPoll`.

The save is one file per slot, `BASLUS-00863FS_ST01` and so on, 2 blocks.

## Changing areas

`func_80044EE8` loads an area: it takes the map number from the byte at `0x801B5924`
(and four more bytes after it, 0 is fine) and drives the state machine at
`0x801B5920` (`func_800456F8`) until the module is in. Its only direct caller is
start-up, `func_80014A30` at `0x80014B84`. The module is `FDAT.T` entry `10n+2`; 36
of the 48 map numbers have one (`config/st.json`).

**A door** is handled by `func_80050768`, which walks the area's object records
(0xF0 of 0x58 bytes from `0x801C2230`) and switches on a type; `0xED` and `0xEF`
take the door branch at `0x80052338`. Which byte holds the type is not confirmed:
`+0x05` looks like one. The branch saves the player's cell into
`+0x220..+0x22A` of the player block, calls `func_80045044` with the target map, then,
while the module loads, writes the arrival point from the record: cell bytes
biased by 0x40, times 0x1000, plus a signed fine offset times 0x80, and 0x640 off the
height for the eye (`0x800525D0..0x80052628`). The record's field offsets are not
pinned down yet. Read as `+0x05`, map 0's records have types `0xE0`, `0xEC` and
`0x32` and no door of either kind, so its way into map 1 would be something else.

**Arriving with no floor is a death.** Loading map 1 with the player at map 0's start
coordinates drops the height by `0x39D0` at once and kills the player within a second.
Phase 2's acceptance test changed area that way before saving, so it saved a dead
player; that save loads the position from the title but no area module, which the
test, judging by position alone, did not notice.
