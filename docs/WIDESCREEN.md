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
| the world behind menus | nothing to port: the menu has no world behind it (below) |

## The menus

Verdite3's `MenuWorld` draws the world live behind a menu instead of the game's
320-wide still of it. Shadow Tower has no such still. Its menu (`func_80029B74`) and
death prompt (`func_8002CEF8`) clear the ordering table and draw only their panels
and text (`docs/GEOMETRY.md`, "Who presents"), so the screen behind them is black
across the margin as within the 320 columns. There is no world to keep live, and
drawing one would be a change to the game, not a port of it. The load fade follows
the vblank count (`0x801B5938`), so it is not held to 20 either.

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

## The packet buffer

The margin costs no packets: the game submits what it did. Over `scripts/tour.py` at
16:9 (`ST_PRIMBUF_PROBE=1`), the busiest area is map 41, **peak 0xA89C bytes, 33% of
one 0x20000 buffer**; map 44 0x7E28, map 4 0x57C4, the rest under 0x4C00.

**By eye (the user):** the margins in play and while turning; the fades and flashes
reaching the edges; the title and menus (pillar-boxed, by design); the HUD at the
edges (`ST_WIDESCREEN_HUD=1`).
