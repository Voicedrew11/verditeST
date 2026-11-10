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

## The eye the world is placed from

The user saw the view not stay smooth (2026-11-10). The camera above carries only the
**rotation** the world is drawn with. Its position (copied to `0x8017BE58`) is not what
places the world: the map's cell translations (`func_8001CB88`, before `ScenePrepare4`)
and the model drawer (`func_80022678`) each subtract the **player's** position,
`+0x248`/`+0x24C`/`+0x250` of the block at `0x80198F28`, with the s16s at `+0x2CE` and
`+0x2D2` added to Y. Those change only on a tick. So turning was smooth and walking
stepped at 20 Hz, in both the packet and the retained renderer (same game code).
Measured with a temporary probe on the first published cell's translation while
walking: it changed on 21 to 92 frames a second against 120 new cameras.

`patches/EyeSmoothing.cs`, on with the camera, carries those five values between the
last two ticks and hands the carried ones to the scene's and the model list's
draw-stage calls (return `0x80026D2C`, `0x80026D4C`), putting the tick's back after
each. After it: **120 of 120 frames a second** moved the cell while walking at 120 Hz.
The cell cull (`func_8001C100`, passed `0x80199170`) now culls from the carried
position too, so the "in another cell" frames above no longer differ from what is
drawn. The acceptance test passes with it on.

**Two more drawers place from the eye** (the user saw objects other than the map
jitter while walking, 2026-11-10). The draw stage's effect `func_800246E4` (return
`0x80026D54`) and second model list `func_800251C8` (return `0x80026D5C`) both draw
through the model drawer `func_80022678`, which subtracts the player's position, so
left at the tick they stepped against the carried world. The effect also reads
`+0x248`/`+0x250` itself, for a distance to the player that scales what it draws. Both
are now handed the carried eye too. `ST_SMOOTH_PROBE=1` adds a line a second counting the
model drawer's calls inside each of the four, carried and not: at map 0's start the
effect makes 160 to 240 a second (two models a frame), all carried, none not; the
second list made none over six maps. The acceptance test passes.

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
not change: it was not in view.

**A creature in view, the eye still** (2026-11-10): map 24's arrival point, where a
creature comes at the player, kept alive by poke for 24 s. The camera probe read 0 new
cameras a second throughout, and the model probe **144.0 distinct model packets a
second** in all 14 windows, 576 entity draws carried a second (four entities), 0 snaps:
with nothing else moving, every frame's models were new, from the carried poses and
positions alone. A run with `ST_SMOOTH_MODELS=0` is no comparison (the creature hit
the player and the eye moved), and none is needed for the gate.

## The gate

Phase 6's measured gate is met: 144 distinct cameras a second turning, a creature a
new pose on every frame, 0 backward steps. Left: the user's eye, and the arm (below).

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

`func_80021AC4(hand)`, called twice by the draw stage (0 and 1), draws that hand's
arm **while it swings**: the s16 at `player+0x130 + 2 * hand` (`0x80199058`) is the
swing's clip time, `-1` at rest, and is the time handed to `func_80034604` /
`func_80035AA8` to pose it. The hand's record is the pointer at `player+0xE8 + 4 *
hand`: `+0xC`/`+0xE`/`+0x10` its view-space place, `+0x14` its angles. So the arm is
drawn only during an attack, at rest drawing nothing, as on the console. Measured
2026-11-10 on the first save (`ST_AUTOSTART=load`, nothing equipped): Triangle swings
hand 0, its time running `0, 0x114, 0x228, ...` for about a second; the unarmed arm's
record places it at (320, 224, 512), so it swings in from the bottom right, about a
fifth of its front faces with a corner on the screen. Circle, Square, Cross and the
shoulders start no swing.

**Drawn over the world.** Its packets go to the 0x40-entry front table
(`0x80165178 + buffer * 0x100`), which `func_80017634` links ahead of the main
table's entry 0, so the console draws the arm last, with no depth test. The port gave
its corners the address map's depths, so the Z-buffer let nearer floor and walls
cover it (about 7,500 to 11,000 of its triangles a second depth-tested while
swinging), and the retained world took it as an instance in the world's own depth.
`HeldModelLayer` (`patches/Render/HeldModelLayer.cs`) keeps it the console's way:
its parts are not published to the retained world, and after the routine the words
of its packets are dropped from the address map, so they keep painter's order.
Measured over the same swings: 8,400 to 12,100 triangles a second in painter's order
against the HUD's 960, the world's 61,320 tested unchanged. `ST_HELDLAYER=0`
compares. The ST_GEOPROBE rows for `21AC4` read "no packets" because the probe walks
the main table only.

Carrying the swing between ticks: the time steps once a tick, so the arm's pose
steps at 20 Hz; not carried yet.

## The HUD's gauges

Nothing to carry: dropping HP from 752 to 300 by poke changed one byte of the HUD's
panel records (`0x8007F3F4`, 0x14 bytes each: record 15's `+2`, `0xD0` to `0x90`) in
the first 50 ms sample, and nothing more over the next two seconds. The gauge steps
once on the tick, as on the console; there is no eased animation to carry.

## Not carried yet

The arm, sprite cels (held
to the tick by `patches/DrawStageTick.cs`, not carried), and the loops that draw
their own frames (the menu, whose backdrop is a still of the world).
