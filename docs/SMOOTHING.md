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

## Models carried

`patches/ModelSmoothing.cs`, on with the camera (`ST_SMOOTH_MODELS=0` turns the models
alone off). The model list `func_800241D0` hands each entity to the model drawer
`func_80022678` with its position from the record's `+0x74` (three s32) and its angles
from `+0x8C`, `+0x8E`, `+0x90`; the records are 0xD4 bytes from `0x801BA138`, 117 slots
up to the list's count at `0x801C0238`. Around the draw stage's call of the model list
(return `0x80026D4C`), every slot's position and angles are replaced by the ones
interpolated between its last two ticks, and the tick's are put back after. A slot that
steps over 0x800 units or 0x300 in an angle in one tick snaps (a new entity in the
slot), and an overlay load re-primes.

Measured 2026-11-10 over `scripts/tour.py`'s 12 areas with `ST_SMOOTH_PROBE=1`: windows
with something moving carried 32 to 303 entity draws a second at 144 fps, one or two
moving entities on every frame; snaps came in two windows only (44 and 39), around area
loads. Most windows had nothing moving. The acceptance test passes with it on.

Not measured: a creature followed on purpose (none was approached), and its **pose**:
the animation frame is still the tick's, so a walking creature glides between ticks
with its legs held. The poses are next.

## Not carried yet

Creatures' poses, the arm, the HUD's gauges, sprite cels (held
to the tick by `patches/DrawStageTick.cs`, not carried), and the loops that draw
their own frames (the menu, which has no world behind it).
