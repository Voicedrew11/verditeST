# Smoothing

Phase 6 of `docs/PORTING_PLAN.md`: what the picture carries between the world's
20 Hz ticks when it is drawn at 144. Verdite1's `docs/SMOOTHING.md` is the model; none
of its addresses apply. Measured with `scripts/session.py` ("Measuring the draw path"
in `docs/DEVELOPMENT.md`).

## The camera carried

`patches/ViewSmoothing.cs`, on whenever pacing is (`ST_SMOOTH=0` turns it off).

Stage 10 (`func_8003E734`) fills two buffers on the main loop's stack on a tick: the
eye's position at `sp+0x18` (three s32 and a fourth word, 0 so far) and three angles
at `sp+0x28` (pitch, yaw, roll; 0x1000 a turn; the yaw stays in 0..0xFFF). Stage 14's
first call, `func_800176A8` (the camera set-up, `docs/GEOMETRY.md`), reads them on
every frame. Between ticks they hold the last tick's eye, so the picture changed 20
times a second.

A pre hook on the camera set-up, for the draw stage's own call (return `0x80026B30`)
inside the main loop's draw, keeps the last two ticks' eyes and hands the set-up the
eye interpolated between them at `FramePacing.TickFraction` (the world clock's credit
toward its next tick); a post hook puts the tick's eye back, so nothing else sees the
carried one. It interpolates and never extrapolates, so the picture is up to one tick
behind the world. A step over 0x1000 units or 0x300 in an angle in one tick snaps (a
warp, a load), and an overlay load re-primes.

Measured 2026-11-10 with `ST_SMOOTH_PROBE=1` over the session at the default 144 fps:
**144.0 distinct cameras a second** walking or turning (20 before), 0 snaps, **0
backward steps** (a frame whose position or yaw moved against the way the last tick
went). The acceptance test passes with it on.

Not done yet: the visible-cell list is built from the player's cell on the tick, so a
carried eye near a cell's edge may show the cull a frame early or late; not measured.

**By eye (the user):** turning and walking at 144, against `ST_SMOOTH=0`.

## Not carried yet

Models and creatures and their poses, the arm, the HUD's gauges, sprite cels (held
to the tick by `patches/DrawStageTick.cs`, not carried), and the loops that draw
their own frames (the menu, which has no world behind it).
