# Widescreen

Phase 9 of `docs/PORTING_PLAN.md`: Verdite3's widescreen (itself Verdite2's) on this
disc. Started 2026-11-10.

## Status

**Mechanism measured, picture not judged. 16:9 by default**, Verdite3's judged value
(the user's choice there, 2026-10-06), as the plan ships enhancements:
`ST_WIDESCREEN=off` (also `16:10`, `21:9`, a number) for 4:3, and the kept
`st.widescreen.aspect` once a settings page writes it (Phase 10).

| piece | here |
|---|---|
| the margin | the runtime's (`Display.WideAspect`), shared fork |
| the margin latch cleared on an executable load | `Widescreen.cs`: `str`, `title`, `optxt`, `st`, `end`, `vs`; never the `fdat` area modules |
| full-screen tints stretched | `Widescreen.cs`, by shape (semi-transparent, flat, the clip's full width) |
| the HUD at the screen edges | `Widescreen.cs`, by its records; off (`ST_WIDESCREEN_HUD=1`) |
| the cell cull | nothing to widen: built from the player's position alone (below) |
| the near polygons | `NearScreen.cs`: libgte's division's screen width widened |
| the packet buffer | measured, below |
| the field of view | `FieldOfView.cs`: the GTE's H from a chosen angle (below) |
| the world behind menus | `MenuWorld.cs`: the world drawn live in place of the menu's 320-wide still (below) |

## The menus

The menu's backdrop is **a still of the world**, as in Verdite3 (corrected
2026-11-10; an hour earlier this section said there was none). As the menu opens,
`func_80058C04` calls `func_80026E04`: `DrawSync(0)`, then `StoreImage` of the draw
buffer's clip, 320 by 240, into `0x80136C88`, the tail of the packet buffers, which
is why buffer 0 ends at start + `0xD400` while the menu is up. Every menu frame,
`MenuBackdrop` (`func_8002948C`) puts it back with `LoadImage` before the panels are
drawn over it. Measured with a probe on both calls: one `StoreImage` (return
`0x80026E70`) on opening, then a `LoadImage` of `0,0,320,240` or `0,240,320,240` from
`0x80136C88` a frame (return `0x80029518`).

So at 16:9 the backdrop is 320 wide and the margin beside it is whatever the
runtime keeps there.

**`MenuWorld`** (`patches/MenuWorld.cs`, `ST_MENUWORLD=0` for the still) skips that
paste and, in its place, runs stage 14's world half (the camera from the last main-loop
stage 14's arguments, the slot marks, `func_80021AC4`, the scene, the three
visibility lists, both model lists) into an ordering table of its own in the still's
store, with its packets after it. Its front table is linked in as `func_80017634`
does, its end to the head of the menu's table, and the menu's `DrawOTag` starts at
it. Not run: the texture animations, effects (`rand`), cels, tints, the fade, the
HUD; the light's flicker takes the held `rand` (`DrawStageTick`).

The pass runs with interrupts held off (`EnterCriticalSection`'s), and every byte of
game memory it changed outside its store and the stack is put back after it. That
is 60 bytes a pass in map 0: the camera block, the table and descriptor pointers and
frame counters, a saved stack pointer at `0x80087C50`, and the pose slots at
`0x8016780C`, which the menu frees and the models claim again (a creature's record
then points at slot 0). Measured 2026-11-10 with `ST_MENUWORLD_PROBE=1`: 20 passes a
second, 0x3958 bytes of packets of 0x216F0, and inside the menu the slots read the
game's own `08 FFFFFF`. After the menu closes, slot 0's flags read `09` or `0A` with
the pass on or off, run to run. The acceptance test passed 2 of 3 with it on and 2
of 2 off; its one failure was the busy-loader race fixed since (`docs/DEVELOPMENT.md`). The death prompt (`func_8002CEF8`) draws no
backdrop. The load fade follows the vblank count (`0x801B5938`), not the tick.

## The margin, the latches and the tints

The runtime renders a margin of extra columns either side of the 320-pixel screen
(54 at 16:9), returns only the original columns to VRAM and presents the target at
the aspect. The projection is untouched, so the margin shows what the game submits
past its screen edge for the GPU to clip. The tints, `ScreenTintA`/`B` and the load
fade (`docs/GAME_INTERNALS.md`), are flat semi-transparent quads from 0 to 320 and
are snapped to the new edges.

Measured 2026-11-10, `ST_WIDESCREEN_PROBE=1`, 144 fps:

- **Map 0, the scripted session** (walk, turn, menu, death): 18 windows of play,
  **33.5% of 1.43 million primitives reach the margin**; standing at the start,
  18.7%. The game leaves the screen edge to the GPU, so the margin has a picture.
- **Tints**: with a status flash poked on (`+0x2D6`), 144 stretched a second, one
  a frame (the tint is held for the tick, `DrawStageTick`); 0 in play otherwise.
- Pacing held 144.0 fps; the acceptance test passes at 16:9, and the 54 draw
  routines the session reaches verify at 0 with it and `NearScreen` on.

## The HUD at the screen edges

`func_80026964`'s panels, 0x14-byte records from `0x8007F3F4 + 0x14 * a0` to a first
byte of 0xFF, are drawn as a sprite, an FT4 or an F4 with the screen X an s16 at `+6`
and the width a byte at `+4` (read from `HudSprite`, `func_80025C24`, which copies
`+6`, `+8`, `+4`, `+5` into the sprite's X, Y, W, H). Moved out by the margin for the
call, left of the screen's middle to the left, right of it to the right, and put back
after it. **Off by default**, as in Verdite3. Measured standing in map 0 at 16:9:
the margin's share went from 18.7% to 21.2% with it on (about 8 primitives a frame,
the panels), and the records read the same between frames as at 4:3.

## The cell cull

Nothing to widen. The visible-cell list (`func_8001C100`) is built from the player's
position (`docs/SMOOTHING.md`, "The cell cull"), not the view: the same ten cells at
4:3 and 16:9 standing at map 0's start (`0x801B9E12`: count 10, cells 0x3, 0x23,
0x25, 0x27, 0x2A, 0x1D1, 0x1D3, 0x1DA, 0x1DB).

## The near polygons

A polygon large on screen is subdivided by libgte's division, `func_80075EC4`
(triangles) or `func_80076884` (quads), handed a block the writer fills on its stack
for every face: the division level, then 320 and 240 as immediates. The division
drops pieces past `OFX ± w/2`, so at 16:9 near pieces in the margin were lost.
`NearScreen.cs` rewrites the width at the entry. Over the session: 24,000 faces a
second widened; with it on, **5,300 more primitives drawn (+0.4%) and 7,200 more in
the margin** than with `ST_NEARSCREEN=0`. Standing at map 0's start nothing changed:
no near piece there was past the edge.

## The field of view

The game writes the GTE's projection distance H (control register 26) as an
immediate **300** in its four display set-ups (`func_80016FB8`, `func_800171C8`,
`func_800172BC`, `func_800173A8`; libgte's `InitGeom` writes 1000 before them) and
never again; no area module writes it. So the 320-wide screen spans
2·atan(160/300) = **56°** across, and at 16:9 (margin 54) 71°. Every reader takes H
from the GTE: libgte's divisions (`func_80075ACC`, `func_80075ECC`, `func_8007636C`,
`func_8007688C`, `cfc2` 26) and `GpuWorld.SceneIn`.

`FieldOfView.cs` (`ST_FOV`, Settings ▸ Testing ▸ *Field of view*, kept as `st.fov`)
takes degrees across the 4:3 screen and writes H = 160 / tan(angle/2) at each frame
boundary, only over the stock 300 or its own last value. Widescreen adds to the sides
(Hor+). The arm is drawn at the same H, so it looks smaller at a wide angle.

**No hook.** A first version wrote H after the camera set-up (`func_800176A8`) and drew
the arm at 300 with a pre and post hook on `func_80021AC4`. Either hook alone, even
writing nothing, stopped the acceptance test's save call (and a direct `call
80058DC4`) running in most runs: the agent server runs a queued call only on a vblank
delivered inside the game's own `VSync` (game stack), and with those hooks every vblank
came from the interrupt poll (`SP 0x0000E000`, `RA` in `func_8001C...`). Without them
the call ran at `SP 0x807FFF08`, `RA 0x800175C4`, after about 240 vblanks from the
poll. So the harness depends on where vblanks land, which is open in `docs/TODO.md`.

Measured 2026-11-10: `ST_FOV=90` gives H 160, written on the first frame of play
(`H 300 -> 160`). **By eye (the user):** the picture at a wide angle, objects at the
edges (any cull by angle would show as pop-in), the arm's size.

## The packet buffer

The margin costs no packets: the game submits what it did. Over `scripts/tour.py` at
16:9 (`ST_PRIMBUF_PROBE=1`), the busiest area is map 41, **peak 0xA89C bytes, 33% of
one 0x20000 buffer**; map 44 0x7E28, map 4 0x57C4, the rest under 0x4C00.

**By eye (the user):** the margins in play and while turning; the fades and flashes
reaching the edges; the title and menus (pillar-boxed, by design); the HUD at the
edges (`ST_WIDESCREEN_HUD=1`).
