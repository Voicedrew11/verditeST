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
