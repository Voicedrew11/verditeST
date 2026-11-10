# The draw path

Phase 5's inventory of `ST.EXE`: which routine writes which packets, in which
context. Verdite1's `docs/RENDERER_PLAN.md` Step 0 is the model; none of its
addresses apply. Measured with `ST_GEOPROBE=1` and `ST_PRESENTPROBE=1` over
`scripts/session.py` ("Measuring the draw path" in `docs/DEVELOPMENT.md`). Calls are
named by the return address they leave in RA (the call site plus 8), as
`scripts/callsites.py` prints them and the probes key them; the presents table gives
call sites.

## Who presents a frame

Measured 2026-11-10 over the session (map 0: stand, walk, turn, the menu, a death).
`func_80017634` is the present (`DrawOTag` of the table); it is called from the swap
`func_80017680` or directly.

| context | presented by | rate | what it draws |
|---|---|---|---|
| play | stage 14 `func_80026AFC`, swap at `0x80026D90` | every frame (144 at the default) | the world, then the HUD |
| entering an area | `func_800214CC`, swap at `0x80021500` | 6.4 a second for about a second | nothing: it clears the table (`func_800174C4`) and swaps, a blank frame. Called from `func_8001531C` and `func_80015F0C` |
| the menu (Select) | `func_80029B74`, direct at `0x8002A1A0` | 20 | **only the menu**: it clears the table and draws text and panels (`func_8002732C`, `func_800276BC`, `func_80026ECC`, `func_80029618`, `func_8002948C`); no world behind it. Called from `func_80058EF4` |
| the death prompt | `func_8002CEF8`, direct at `0x8002D08C` | 20 | a panel and two text lines (`func_80027D18`, `func_800286A8`, `func_8002732C`, `func_80029618`) after a clear; no world. Called from `func_80063260`, the yes/no |

While the menu or the death prompt is up, stage 14 runs about 3 times a second (the
rest of the loop is stopped inside stage 3), so these loops present nearly every
frame. They step at 20 because `VBlankPacing` holds their `VSync(3)`.

Not reached yet: the card screens (`func_80062C28`), the opening of the game, the
area modules' own drawing, the other menu pages, a fight, `VS.EXE`.

## Stage 14, call by call

Map 0's start, standing and turning, per call:

| return address | routine | packets | what |
|---|---|---|---|
| `0x80026B30` | `func_800176A8` | none | frame set-up (calls `func_80049CD4`, `func_800498CC`) |
| `0x80026B74` | `func_800174C4` | removes the last frame's | **frame head**: clears the ordering table |
| `0x80026D14` | `func_80021654` | FT4S, now and then | |
| `0x80026D2C` | `func_80020A80` | GT4 120-200, GT3 11-22, FT4 1-30 | **the scene** (below) |
| `0x80026D4C` | `func_800241D0` | FT4 26-76, FT3, GT3, GT4, and their S forms | **the models** (below) |
| `0x80026D54` | `func_800246E4` | FT3 5, FT4 2.5, in one window | an effect on a timer kept in vblanks |
| `0x80026C7C` | `func_80026964(0)` | E1-6 10, sprite 8, F4 2, FT4 2; 80 words | **the HUD**: `func_80025C24` a sprite, `func_80025E00` an FT4, `func_80026020` an F4, each with its mode packet |
| `0x80026D90` | `func_80028784` | | only when `0x8017BD66` is set; text (`func_80028364`, `func_80028220`, `func_800286A8`) |
| `0x80026D98` | `func_80017680` | | **the swap**: `DrawSync`, `VSync(3)`, the environments, `DrawOTag` |

The other calls added nothing in this session: `func_80016A14`, `func_80033110`,
`func_800340F8`, `func_80034FC4`, `func_80027D18` and the other three
`func_80026964` calls (arguments `0x1A`, `0x20`, `0x25`), `func_80026508`,
`func_80026228`, `func_8002181C`, `func_800218D8`, `func_80021AC4` (0 and 1),
`func_8001C8E0`, `func_8001C9C8`, `func_8001CAB8`, `func_800251C8`, `func_800255F0`
(the cel animations). Several are gated on state not met standing in map 0.

### The scene, `func_80020A80`

| return address | routine | packets | what |
|---|---|---|---|
| `0x80020CA8` | `func_8001C100` | none | the visible-cell list |
| `0x80020CB0`, `0x80020CB8` | `func_8001C264`, `func_8001C60C` | none | preparation, not read |
| `0x80020D08`, `0x80020DA0` | `func_8001CB88`, `func_8001CDF8` | none | |
| `0x80020DAC` | `func_8001F954` | FT3, now and then | a leaf: no calls |
| `0x80020DB8` | `func_8001FD00` | FT4, 1 to 30 | a leaf; up to 30 while walking |
| `0x80020DDC` | `func_8001D2A8` | **GT3**, 11 to 22 | **map triangles**; calls only `func_80075EC4` (8 bytes) |
| `0x80020DE8` | `func_8001DB64` | **GT4**, 120 to 200 | **map quads**; calls only `func_80076884` (8 bytes) |
| `0x80020DC4`, `0x80020DD0`, `0x80020DF4`, `0x80020E00` | `func_80020164`, `func_80020588`, `func_8001E5A0`, `func_8001EEAC` | none here | |

The light's flicker `rand()` is at `0x80020B98` ("What runs at the drawn rate" in
`docs/GAME_INTERNALS.md`).

### The models, `func_800241D0`

`func_800241D0` calls `func_80022678` (2960 bytes) from three sites; in map 0 only
the second (`0x80024544`) drew. Every model polygon comes from **`func_80022010`**
(1640 bytes), called by `func_80022678` at `0x80022C38`: FT4 to GT4, semi-transparent
or not. `func_80022678`'s other callees (`func_80017D28`, `func_80034604`,
`func_80035AA8`, `func_8001BF10`, `func_800192D4`, `func_80019DC8`) added nothing.
`func_800251C8` is the same shape (`func_80022678` twice, then `func_80023ED8`) and
drew nothing in map 0. `func_80021AC4` calls `func_80022010` too, at `0x80021FA0`,
and drew nothing either.

## The packet buffer

`func_800174C4`, the frame head, flips the buffer byte at `0x8017BD52`, points
`0x80165394` at that buffer's record in the table at `0x8016537C` (two records of
12 bytes, `{ start, end, cursor }`) and sets the cursor to the start. It also points
`0x80165174` at that buffer's ordering table (0x1000 entries at `0x8015D174 +
buffer * 0x4000`, cleared with `ClearOTagR`) and `0x80165378` at a second table of
0x40 entries (`0x80165178 + buffer * 0x100`), and zeroes eleven words from
`0x801653AC`.

| buffer | start | end | bytes |
|---|---|---|---|
| 0 | `0x8011C488` | `0x8013C488` | 0x20000 |
| 1 | `0x8013C488` | `0x8015C488` | 0x20000 |

Measured 2026-11-10 with `ST_PRIMBUF_PROBE=1` over `scripts/session.py`: a frame of
play spends 0x2C00 to 0x3F00 bytes on average, **0x6708 at most** (20% of a buffer);
the menu's frames 0x3E8C, the death prompt's 0xBD0. While the menu is up, buffer 0's
end reads `0x80129888` (start + 0xD400) instead; what uses the rest is not read.
Widescreen (Phase 9) widens the cull, so this is its baseline. Which writer checks the
end, and what it does when full, is not read.

## The draw path in C#

Each routine is transcribed literally into `patches/Draw/`: the same reads and stores
in the same order, callees called directly so their hooks still fire. Verdite1's
`Literal` (`patches/Draw/Literal.cs`, `LiteralVerify.cs`, `Mips.cs`) runs them:
`ST_LITERAL=1|0|verify` for all, `ST_<NAME>` for one, on by default. Verify runs the
recompiled routine and the transcription from one state and compares RAM outside a
stack window, the scratchpad, every register, the GTE and the interrupt poll's
countdown; **the recompiled result stands**. `ST_LITERAL_PROBE=1` counts runs.

`scripts/transcribe.py` writes the first draft from `generated/`: it strips the PGXP
bookkeeping and the recompiler's temporaries and keeps everything else, labels and
`goto`s included. It is tidied by hand afterwards; nothing it writes is trusted until
verify reads 0. Verify was checked once by planting a wrong constant in
`HudPanels` (`0x61` for `0x60`): it reported 24 bytes from `0x8011C48D`, recompiled
`60`, ours `61`.

| routine | class, switch | verified (2026-11-10, `scripts/session.py`) |
|---|---|---|
| `func_80026964`, the HUD's panel list | `HudPanels`, `ST_HUDPANELS` | 0 in 6,163 calls |
| `func_80025C24`, a HUD sprite | `HudSprite`, `ST_HUDSPRITE` | 0 in 48,933 (the menu's text too) |
| `func_80025E00`, a HUD FT4 | `HudQuad`, `ST_HUDQUAD` | 0 in 12,325 |
| `func_80026020`, a HUD F4 | `HudFill`, `ST_HUDFILL` | 0 in 12,325 |
| `func_8001DB64`, the map's GT4s | `MapQuads`, `ST_MAPQUADS` | 0 in 3,083 |
| `func_8001D2A8`, the map's GT3s | `MapTriangles`, `ST_MAPTRIANGLES` | 0 in 3,083 |
| `func_8001FD00`, the scene's FT4s | `SceneQuads`, `ST_SCENEQUADS` | 0 in 3,083 |
| `func_8001F954`, the scene's FT3s | `SceneTriangles`, `ST_SCENETRIANGLES` | 0 in 3,083 |
| `func_80022010`, every model polygon | `ModelPolygons`, `ST_MODELPOLYGONS` | 0 in 1,322 |
| `func_80020A80`, the scene | `Scene`, `ST_SCENE` | 0 in 1,094 |
| `func_8001C100`, the visible-cell list | `VisibleCells`, `ST_VISIBLECELLS` | 0 in 2,187 |
| `func_8001C264`, scene preparation, not read | `ScenePrepare1`, `ST_SCENEPREPARE1` | 0 in 2,187 |
| `func_8001C60C`, scene preparation, not read | `ScenePrepare2`, `ST_SCENEPREPARE2` | 0 in 2,187 |
| `func_8001CB88`, scene preparation, not read | `ScenePrepare3`, `ST_SCENEPREPARE3` | 0 in 2,187 |
| `func_8001CDF8`, scene preparation, not read | `ScenePrepare4`, `ST_SCENEPREPARE4` | 0 in 2,187 |
| `func_80020164`, a scene step, no packets in map 0 | `SceneStep20164`, `ST_SCENESTEP20164` | 0 in 2,187 |
| `func_80020588`, a scene step, no packets in map 0 | `SceneStep20588`, `ST_SCENESTEP20588` | 0 in 2,187 |
| `func_8001E5A0`, a scene step, no packets in map 0 | `SceneStep1E5A0`, `ST_SCENESTEP1E5A0` | 0 in 2,187 |
| `func_8001EEAC`, a scene step, no packets in map 0 | `SceneStep1EEAC`, `ST_SCENESTEP1EEAC` | 0 in 2,187 |
| `func_800241D0`, the model list | `Models`, `ST_MODELS` | 0 in 1,094 |
| `func_80022678`, one model | `ModelDraw`, `ST_MODELDRAW` | 0 in 921 |
| `func_80033DB8`, per entity, before its model | `ModelLight`, `ST_MODELLIGHT` | 0 in 6,560 |
| `func_80023C6C`, after the model list | `ModelExtras`, `ST_MODELEXTRAS` | 0 in 2,187 |
| `func_800251C8`, a second model list, empty in map 0 | `Models2`, `ST_MODELS2` | 0 in 1,094 |
| `func_80023ED8`, after the second list | `ModelExtras2`, `ST_MODELEXTRAS2` | 0 in 2,187 |
| `func_80021AC4`, a model from the draw stage, twice; empty in map 0 | `HeldModel`, `ST_HELDMODEL` | 0 in 2,187 |
| `func_80021654`, an FT4S over the scene, now and then | `Overlay21654`, `ST_OVERLAY21654` | 0 in 1,094 |
| `func_800174C4`, the frame head | `FrameHead`, `ST_FRAMEHEAD` | 0 in 1,417 |
| `func_800176A8`, frame set-up | `FrameSetup`, `ST_FRAMESETUP` | 0 in 1,094 |

`scripts/verify_summary.py <log>` sums a run's verify lines. The acceptance test
passes with all 29 on (2026-11-10), and play still draws 144 frames and 20 ticks a
second. The names with numbers in them are placeholders until the routine is read.
`scripts/split_literal.py` moves a long transcription's straight-line runs (no label,
`goto`, brace or poll) into `<Class>Runs.cs`, each named after the label before it,
so no file passes 500 lines; `ModelDraw`, `SceneStep1EEAC` and `SceneStep1E5A0` are
split that way and still verify at 0.
A count is calls, not coverage: `Models2` and `HeldModel` drew nothing in map 0, so
their drawing branches have not been compared yet.

Verify restores the HLE `rand()`'s seed (`BiosA._randSeed`, outside guest RAM) before
the second run, as it does the interrupt countdown; without it the scene, which calls
`rand()` for the light's flicker, read a 2-byte mismatch at `0x8017BE88` on two calls
in three.

The map writers each have **two packet paths**, kept in `MapQuadsPackets.cs` and
`MapTrianglesPackets.cs` so no file passes the length rule: the ordinary one writes
the GT4 or GT3 at the cursor; the near one (`NearQuad` from `0x8001DF7C`,
`NearTriangle` from `0x8001D60C`) puts the corners' colours on the stack with the
screen's 320 by 240 and, when `0x8017BE8C & 6`, calls through `FP` once a corner.
It is probably the near-polygon clip or subdivision; it is not read. **Both paths run
in the session**: `Literal.Taken` counts a path under `ST_LITERAL_PROBE=1`, and over
five seconds of map 0 it read about 110,000 ordinary quads to 50,000 near ones, and
13,000 ordinary triangles to 3,000 near ones, so their 0 mismatches are not vacuous.
What is called through `FP` is not read.

## Open

- Which objects `func_800241D0`'s three sites draw, and what `func_800251C8` and
  `func_80021AC4` draw (creatures, the hands, items are candidates).
- The leaves `func_8001F954` and `func_8001FD00`.
- The card screens, a fight, other areas, the area modules, `VS.EXE`.
- Who checks the packet buffer's end, and what the menu does with buffer 0's top.
