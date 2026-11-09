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
