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

**The cell cull and the carried eye.** The visible-cell list (`func_8001C100`, called
by the scene with `0x80199170`) is built from the player's position on the tick, not
the camera: `func_80046270` turns X, Y and Z into 0x1000-unit cells (`>> 12`) and
ANDs three per-axis bitmasks (64 bytes a cell, tables from `0x801856D0`, `+0x2000`,
`+0x4000`) into a 512-bit set. The carried eye trails the tick by up to one tick, so
near a cell's edge it may stand in the cell behind the one culled for. The probe
counts those frames ("in another cell"): over the session, **44 frames in 36 s**, about
1% of those drawn while walking, each within one tick of crossing an edge. Whether a
cell is missing in them shows only by eye; if it does, the cull can be run for the
previous tick's position as well and the two sets ORed.

**By eye (the user):** turning and walking at 144, against `ST_SMOOTH=0`; and
walking across cell edges (every 0x1000 units) for a cell popping in late.

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

**Poses.** The model drawer builds a pose from the u16 at `+0x2A`, passed to
`func_80034604`/`func_80035AA8` with the pose state at `+0xA4` (per-entity pose slots
of 0x94 bytes, allocated by `func_800152D0`; not read further). Sampled in map 2, an
animating entity's `+0x2A` stepped by 128 to 1,280 between samples a few ticks apart
and wrapped at 0x1000 (3328, then 384), so it is a **clip time**, and since the drawer
takes the time itself, carrying it as an angle is carried gives the poses between
ticks. A step over 0x300 snaps (a new clip). `+0x94` is a scale (three shorts into
`ScaleMatrix`), not carried.

Measured with `ST_SMOOTH_PROBE=1`, which hashes the packets the model list writes each
frame: over the tour, map 24 read a median of **144 distinct model packets a second**
with 576 entity draws carried a second; with `ST_SMOOTH_MODELS=0` the same visit read
none, but creatures do not act the same from run to run, so the pair is not a clean
comparison. In maps 44 to 46 an entity was carried on every frame and its packets did
not change: it was not in view. A creature followed on purpose is still to do.

**The pose routine blends between keys**, so a carried time gives poses between ticks,
not the next key a frame early. The model drawer hands `func_80035AA8` (or
`func_80034604`) the time; on the path through `func_80034364`, `func_80033CFC` walks
the clip's keys (a count byte, then per key an offset to a record whose first u16 is
its length in time) to the one holding the time, and returns the fraction through it,
`(time - key start) << 12 / length`, 0 to 0x1000. `func_80074A04` then adds, for
every vertex, that fraction times the key's delta to the base (`IR0` = the fraction,
the GTE's `GPF`, three shorts at a time). The other branch (`func_8003569C`, when the
clip header's `+6` is set) was not read.

## The arm

`func_80021AC4(hand)`, called twice by the draw stage (0 and 1), draws the model the
player holds in that hand: the u16 at `player+0x130 + 2 * hand` (`0x80199058`), `-1`
for none, posed through `func_80035AA8` like a model and lit with the matrix at
`0x8017BD78`. **A new game holds nothing** (both `0xFFFF`), so in every measurement
so far it drew nothing; pressing each shoulder and face button once ran its model
calls for about 0.75 s with nothing to draw. Carrying the arm's swing waits for a
held weapon in a measured run: equipping one through the menu, or a save that holds
one (`ST_AUTOSTART=load` needs the acceptance test's way through the card screen).

## The HUD's gauges

Nothing to carry: dropping HP from 752 to 300 by poke changed one byte of the HUD's
panel records (`0x8007F3F4`, 0x14 bytes each: record 15's `+2`, `0xD0` to `0x90`) in
the first 50 ms sample, and nothing more over the next two seconds. The gauge steps
once on the tick, as on the console; there is no eased animation to carry.

## Not carried yet

The arm, sprite cels (held
to the tick by `patches/DrawStageTick.cs`, not carried), and the loops that draw
their own frames (the menu, which has no world behind it).
