# Game internals

The reverse-engineered game: the main loop and its stages, player state, movement,
areas, saves and the boot stub. Every address and routine this project learns is
written here rather than left in the commit that found it. Verdite3's
`docs/GAME_INTERNALS.md` is the model, but none of its addresses apply.

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

Found 2026-11-09 by comparing RAM across walking and turning.

| address | what |
|---|---|
| `0x80199170` | X position, 32-bit |
| `0x80199174` | Z position, 32-bit |
| `0x80199190` | heading |
| `0x80199140` | byte: the in-world action runs only when it is 0 |

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
`0x801B5920` (`func_800456F8`) until the module is in. `func_80050768` and
`func_80052894` are its callers in play. The module is `FDAT.T` entry `10n+2`.
