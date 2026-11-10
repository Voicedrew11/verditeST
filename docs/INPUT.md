# Input

Phase 8 of `docs/PORTING_PLAN.md`. Step 1, what the game reads, measured and read
2026-11-10; steps 2 and 3 (key layout, mouse, rumble options) not started.

## The pad library

`ST.EXE` links the newer `libpad` (bound by address, "The pad library" in
`docs/RECOMPILATION.md`). Its set-up, `func_80057684`:

- `PadInitDirect(0x801CD130, ...)`: the raw buffer for port 1, status at `+0`
  (0 connected), the buttons as an active-low u16 at `+2`.
- `PadSetAct(0, 0x801CD174, 4)`: the actuator buffer. `+1` the small motor (on/off),
  `+3` the large one (0 to 255).
- `PadStartCom`, then `func_80057428` polls `PadGetState(0)` for 6 or 2.
- `func_800574C0`: `PadInfoAct(0, -1, 0)`, the actuator count, into the pad block's
  `+0x18`; then `PadSetActAlign(0, 0x801CD19A)` until it succeeds, six bytes
  `00 00 01 01 FF FF` with two actuators (`00 00 FF FF FF FF` otherwise).

**Digital only.** Nothing calls `PadInfoMode` or `PadSetMainMode`, and nothing reads
the stick bytes (`+4` to `+7` of the raw buffer, searched for by absolute address and
through the buffer's base register). A twin-stick layer has no stick handling of the
game's to start from: it would turn the sticks into the game's own buttons.

## The pad block

`0x801CD180`, filled by `func_80057744` from the raw buffer (see also "Pad state" in
`docs/GAME_INTERNALS.md`): `+0x02` the buttons held now (active high), `+0x00` the
previous call's, `+0x04` newly pressed (latched until consumed), `+0x0C` the
repeat-filtered set the cursors read, `+0x11` the repeat countdown, reloaded from
the bytes at `0x801CD126` and `0x801CD127` (first delay and repeat, by the look of
it).

**The button table**, `+0x22` to `+0x38`, twelve u16 masks, one per action. The
defaults, from `func_800573BC`:

| `+0x22` | `+0x24` | `+0x26` | `+0x28` | `+0x2A` | `+0x2C` | `+0x2E` | `+0x30` | `+0x32` | `+0x34` | `+0x36` | `+0x38` |
|---|---|---|---|---|---|---|---|---|---|---|---|
| Up | Down | Left | Right | L1 | R1 | L2 | R2 | Triangle | Square | Cross | Circle |

`func_80060334` rewrites the table (the controller page of the options, by the look
of it) and `func_8005738C` ORs `+0x32` to `+0x38`, the four face buttons, into
`+0x3A`, the mask `func_80057744` filters presses with.

What each entry drives, from the routines that read it:

| entry | default | read by | drives |
|---|---|---|---|
| `+0x22`, `+0x24` | Up, Down | `func_800420A0` | forward and back: the speed at `player+0x2C0` |
| `+0x2A`, `+0x2C` | L1, R1 | `func_800420A0` | sidestep: `player+0x2B8` |
| `+0x26`, `+0x28` | Left, Right | `func_80041D4C` | turning: `player+0x2CA` |
| `+0x2E`, `+0x30` | L2, R2 | `func_80041D4C` | looking up and down: `player+0x2CC` (and `+0x27A`) |
| `+0x32`, `+0x34` | Triangle, Square | `func_8003F4EC`, `func_800402AC`, `func_80042900` | the hands, probably (not read) |
| `+0x36` | Cross | `func_80042B9C`, `func_80050768` | the action: doors and what is in front |
| `+0x38` | Circle | `func_80042D74`, `func_8005335C` | not read |

The speeds are the ones the warp zeroes on arrival (`patches/Warp.cs`), so a mouse
look has the game's own turn and look velocities to drive (step 2).

## Mouse look

`patches/MouseLook.cs`, Verdite Core's `Mouse` (on by default; a click on the picture
captures the pointer) spent through **`func_80041D4C`**, the turn and look routine,
once a world tick. Each axis has the three branches Verdite2 and Verdite3 found in
theirs:

| | velocity | held button | none held | base angle |
|---|---|---|---|---|
| yaw | `player+0x2CA` | Left adds, Right takes `rate >> 2` (`rate` the s32 at `+0x2E8`), clamped to `±rate` | decays by `rate >> 2` toward 0 | `+0x27A = (yaw + vel) & 0xFFF` |
| pitch | `player+0x2CC` | R2 adds, L2 takes 3, clamped to `±0x20`; both recentre | decays by 3 toward 0 | `+0x278`, limited to `0x2BC` and `0xD44` by `func_8004A194` |

With an axis's buttons masked out of the held word (`0x801CD182`, by the button
table's own entries, so a rewritten layout is honoured), writing `step ± accel` to
its velocity lands the angle on `step` exactly, through the game's own limit. The
held word is put back after the call, and a tick the mouse drove ends with the
velocity at 0, so the view stops dead with the hand. Mouse right is a negative step,
as Left raises the yaw; mouse down raises the pitch, the way R2 does. The mouse
buttons press Square, Triangle and Cross (the hands and the action; `ST_MOUSE_BUTTONS`).

### Measured

2026-11-10, with the synthetic hand (`ST_MOUSE_SYNTH=yaw,pitch`: that step every tick,
two seconds of every three; `ST_MOUSE_PROBE=1`), standing in map 0:

- **51 and 7 a tick at 144 fps:** every driven tick turned exactly 51; pitch stepped
  exactly 7 until it **stopped at the limit, `0x2BC`**, and stayed there.
- **-37 and -5 at 60 fps:** exactly -37 and -5 every driven tick; pacing held 60 fps
  at 20 ticks a second.
- **The hand stopping:** 0 ticks moved either angle without the hand, in every
  window of both runs.

The acceptance test passed three runs of four with it attached; the fourth's save
screen did not finish (`docs/DEVELOPMENT.md`, "What is still open").

### Instant mouse look (the lead)

The camera's pitch and yaw, as stage 10 hands them to the camera set-up, are the
player's `+0x278` and `+0x27A` exactly (read on every tick of a synthetic turn). So
`MouseLook.Lead`, called by `ViewSmoothing` on every drawn frame, draws the tick's
angles plus the hand's motion since (`Mouse.Pending`), pitch held to the game's limit,
in place of the carried angles: a turn shows on the next frame instead of a tick
later. Only on an axis the mouse drives (it drove the last tick, or has moved since),
so a pad turn keeps the carried view. `Mouse.Lead`, on by default (`ST_MOUSE_LEAD=0`).

Measured 2026-11-10 with the synthetic hand at 51 a tick (it leads by its step times
the tick's fraction, and runs 40 ticks of every 60 so the lead knows what the next tick
applies), 2,000 frames at 144 fps: drawn yaw stepped 1 to 9 a frame (7 in 1,210),
**0 backward, no jump at a tick**; with the lead off, 3 to 9 a frame. With the lead the
view is ahead of the carried one by a whole tick, 50 ms at 20 Hz.

**Not measured**: a real pointer (capture, sensitivity), the buttons.
**By hand (the user):** turning and looking with a mouse; whether mouse down should
look down (`ST_MOUSE_INVERTY`).

## The keyboard

`patches/KeyLayout.cs`, Verdite3's layout through Verdite Core's `KeyLayoutApply`:
RecompOne's own keys put a console on a keyboard (face buttons on Z X A S, the
shoulders on Q W E R, the D-pad on the arrows), so walking and turning share the
arrows. The port's, by the button table above:

| key | button | the game's |
|---|---|---|
| W, S | Up, Down | walk |
| A, D | L1, R1 | sidestep |
| Left, Right | Left, Right | turn (the arrows also walk: `KeyLayoutApply`'s extras) |
| (none) | L2, R2 | looking up and down: the mouse's, as in Verdite3 |
| Space, Q | Square, Triangle | the hands (the mouse's left and right buttons too) |
| F | Cross | the action: doors, what is in front |
| E | Circle | not read yet |
| Tab, Escape | Select | the menu (Escape presses Select inside the pad read) |
| Enter | Start | |

A fresh install gets it as the default; an existing one is migrated once, only from
exactly RecompOne's stock keys, and `st.keys.layout` in `interface.ini` records it.
`ST_KEYS=stock` keeps RecompOne's. Measured 2026-11-10: this checkout's
`settings.json`, stock until then, was migrated on the first run and announced
(`[ST] keys: WASD layout applied ...`). The keys themselves were not pressed in a
measured run (the scripts press the pad).

## Vibration

`func_80057A68(strength)` starts the motors when the byte at `0x801CD125` (the
game's vibration switch, off in a new game) is set: the large motor at
`strength + 0x40` (clamped to 255), and with one actuator also the small one, for
`strength / 16 + 5` ticks (`+0x19`); `func_80057B10` counts down once a frame, held to
the tick by `DrawStageTick`, and stops both at 0. Called from stages 3, 4 and 7.

The runtime drove none of it until the fork's `0122` (2026-11-10): `PadInfoAct`
answered one actuator, and `PadSetAct` read the buffer only when called, which the
game does once. Measured after it with the switch poked on: `func_80057A68(0x80)`
sent large 192 to the pad, then 0 when the countdown ran out about 0.85 s later.

**By eye (hand, the user):** on a real pad, vibration on in the options, a hit or a
fall.
