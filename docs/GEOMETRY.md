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
| `0x80026B30` | `func_800176A8` | none | **the camera**: the eye's position (16 bytes, stage 10's first buffer) to `0x8017BE58`, its angles (the second) to `0x8017BE68`, `RotMatrix` into `0x8017BD78`; then `func_80049CD4` and `func_800498CC` |
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

**The scene's working block** is in the scratchpad from `0x1F800000`, set up by
`func_80020A80` before its calls, which read it there: `+0x00` the ordering table
(`*0x80165174`), `+0x04` 0x1000 (its entries), `+0x0C` `*0x8017BE98` when
`*0x8017BE84` is set, else `*0x8017BE94`, `+0x10` the packet cursor and `+0x14` the
buffer's end (from the record at `*0x80165394`), `+0x8C` 4, `+0x90`/`+0x92` the screen,
240 and 320, `+0x44..+0x46` the light's colour (0x80, or from `rand()` when
`0x8017BE8C & 2`), and `+0x1B0..+0x1E0` the eleven words from `0x801653AC` that the
frame head zeroes (per-frame counters, by the look of `+0x1BC`/`+0x1C0` in the map
writers). Read 2026-11-10 from the disassembly.

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

Verified 2026-11-10 over `scripts/session.py` (map 0, the menu, a death) and
`scripts/tour.py` (standing and turning; `scripts/literal_table.py` writes this table
from `patches/Draw/` and the runs' logs). **The first tour drew no scene**: the warp
then loaded areas through the start-up loader, which left every map with the HUD
alone, so the counts below from it are HUD and set-up calls, not 23 areas' geometry.
The tour over the 12 areas the fixed warp reaches is below the table.

| routine | what | class, switch | mismatches in calls |
|---|---|---|---|
| `func_80016A14` | the draw stage's second call; calls func_80079DCC and func_80079D0C (libgpu); not read | `Draw16A14`, `ST_DRAW16A14` | 0 in 1,114 |
| `func_800174C4` | the frame head: flips the buffer, rewinds the packet cursor, clears both ordering tables | `FrameHead`, `ST_FRAMEHEAD` | 0 in 7,238 |
| `func_800176A8` | the camera: the eye's position and angles from stage 10's buffers, the view matrix | `CameraSetup`, `ST_CAMERASETUP` | 0 in 1,114 |
| `func_80017D28` | one model's set-up step in func_80022678 and func_80021AC4; not read | `ModelStep17D28`, `ST_MODELSTEP17D28` | 0 in 1,864 |
| `func_800192D4` | a model step in func_80022678; not read | `ModelStep192D4`, `ST_MODELSTEP192D4` | 0 in 7,602 |
| `func_80019DC8` | a model step in func_80022678 and func_80021AC4; not read | `ModelStep19DC8`, `ST_MODELSTEP19DC8` | 0 in 4,646 |
| `func_8001BF10` | a model step in func_80022678; not read | `ModelStep1BF10`, `ST_MODELSTEP1BF10` | not reached |
| `func_8001C100` | the visible-cell list, from the eye's cell outwards | `VisibleCells`, `ST_VISIBLECELLS` | 0 in 2,236 |
| `func_8001C264` | the scene's first preparation pass after the cell list; writes no packet | `ScenePrepare1`, `ST_SCENEPREPARE1` | 0 in 2,236 |
| `func_8001C60C` | the scene's second preparation pass; writes no packet | `ScenePrepare2`, `ST_SCENEPREPARE2` | 0 in 2,236 |
| `func_8001C8E0` | a draw-stage call after the scene; calls func_8001C88C; not read | `Draw1C8E0`, `ST_DRAW1C8E0` | 0 in 1,131 |
| `func_8001C9C8` | a draw-stage call after the scene; calls func_8001C88C; not read | `Draw1C9C8`, `ST_DRAW1C9C8` | 0 in 1,131 |
| `func_8001CAB8` | a draw-stage call after the scene; calls func_8001C88C; not read | `Draw1CAB8`, `ST_DRAW1CAB8` | 0 in 1,131 |
| `func_8001CB88` | the scene's third preparation pass; writes no packet | `ScenePrepare3`, `ST_SCENEPREPARE3` | 0 in 2,236 |
| `func_8001CDF8` | the scene's fourth preparation pass; writes no packet | `ScenePrepare4`, `ST_SCENEPREPARE4` | 0 in 2,241 |
| `func_8001D2A8` | the map's GT3s, every visible cell's triangles | `MapTriangles`, `ST_MAPTRIANGLES` | 0 in 2,246 |
| `func_8001DB64` | the map's GT4s, every visible cell's quads | `MapQuads`, `ST_MAPQUADS` | 0 in 2,257 |
| `func_8001E5A0` | a scene step after the map's quads; wrote no packet in map 0 | `SceneStep1E5A0`, `ST_SCENESTEP1E5A0` | 0 in 2,257 |
| `func_8001EEAC` | the scene's last step; wrote no packet in map 0 | `SceneStep1EEAC`, `ST_SCENESTEP1EEAC` | 0 in 2,257 |
| `func_8001F954` | the scene's FT3s, beside func_8001FD00's FT4s. A leaf | `SceneTriangles`, `ST_SCENETRIANGLES` | 0 in 2,241 |
| `func_8001FD00` | the scene's FT4s: a list of 12-byte records at 0x8017BED0, indexed from 0x801B9F16, counted at 0x801B9E14. A leaf | `SceneQuads`, `ST_SCENEQUADS` | 0 in 2,241 |
| `func_80020164` | a scene step between the scene's own quads and the map's; wrote no packet in map 0 | `SceneStep20164`, `ST_SCENESTEP20164` | 0 in 2,241 |
| `func_80020588` | a scene step before the map's triangles; wrote no packet in map 0 | `SceneStep20588`, `ST_SCENESTEP20588` | 0 in 2,243 |
| `func_80020A80` | the scene: the light's flicker, the visible-cell list, the map's preparation, then its quads and triangles and the scene's own | `Scene`, `ST_SCENE` | 0 in 1,131 |
| `func_80021654` | an FT4S over the scene, now and then (a tint or flash, probably) | `Overlay21654`, `ST_OVERLAY21654` | 0 in 1,117 |
| `func_8002181C` | a draw-stage call; calls func_80021518 with 320 by 240; not read | `Draw2181C`, `ST_DRAW2181C` | 0 in 1,117 |
| `func_800218D8` | a draw-stage call; calls func_80021518 with 320 by 240; not read | `Draw218D8`, `ST_DRAW218D8` | 0 in 1,117 |
| `func_80021AC4` | a model drawn with func_80022010 from the draw stage, called twice (0 and 1): a hand each, probably; drew nothing in map 0 | `HeldModel`, `ST_HELDMODEL` | 0 in 2,236 |
| `func_80022010` | every model polygon: FT3 to GT4, semi-transparent or not; called by func_80022678 and func_80021AC4 | `ModelPolygons`, `ST_MODELPOLYGONS` | 0 in 1,718 |
| `func_80022678` | one model: its matrix, light and pose, then func_80022010 for its polygons | `ModelDraw`, `ST_MODELDRAW` | 0 in 1,030 |
| `func_80023C6C` | func_800241D0's last call, after the models | `ModelExtras`, `ST_MODELEXTRAS` | 0 in 2,260 |
| `func_80023ED8` | func_800251C8's last call | `ModelExtras2`, `ST_MODELEXTRAS2` | 0 in 2,266 |
| `func_800241D0` | the model list: the entity records (0xD4 bytes from 0x801BA138) named by the u16 list at 0x801C023C, counted at 0x801C0238, each through func_80022678 | `Models`, `ST_MODELS` | 0 in 1,131 |
| `func_800246E4` | an effect drawn when a timer kept in vblanks runs out (calls rand()); FT3 and FT4; not read | `Effect246E4`, `ST_EFFECT246E4` | 0 in 1,134 |
| `func_800251C8` | a second model list, the same shape as func_800241D0's; drew nothing in map 0 | `Models2`, `ST_MODELS2` | 0 in 1,134 |
| `func_80025C24` | one HUD panel record drawn as a sprite with its mode packet; called by func_80026964 | `HudSprite`, `ST_HUDSPRITE` | 0 in 17,718 |
| `func_80025E00` | one HUD panel record drawn as an FT4; called by func_80026964 | `HudQuad`, `ST_HUDQUAD` | 0 in 4,454 |
| `func_80026020` | one HUD panel record drawn as an F4 with its mode packet; called by func_80026964 | `HudFill`, `ST_HUDFILL` | 0 in 4,454 |
| `func_80026228` | called up to five times by the draw stage, one per active entry of a table at 0x8007F3F4; not read | `Draw26228`, `ST_DRAW26228` | 0 in 10 |
| `func_80026508` | a draw-stage call when 0x801D2D23 is set and 0x801B5929 is under 0x2C; not read | `Draw26508`, `ST_DRAW26508` | 0 in 1,115 |
| `func_80026964` | the HUD's panel list: 0x14-byte records from 0x8007F3F4 + 0x14 * a0, to a 0xFF, each drawn as a sprite, an FT4 or an F4 | `HudPanels`, `ST_HUDPANELS` | 0 in 2,228 |
| `func_80026ECC` | drawn by the menu at (0x6A, y); not read | `Text26ECC`, `ST_TEXT26ECC` | 0 in 216,471 |
| `func_8002732C` | a line of text at a position; the menu and the prompt; not read | `Text2732C`, `ST_TEXT2732C` | 0 in 12,763 |
| `func_800276BC` | text or a number at a position; the menu; not read | `Text276BC`, `ST_TEXT276BC` | 0 in 385 |
| `func_80027D18` | a draw-stage call when the player's state is 1, and the death prompt's; not read | `Draw27D18`, `ST_DRAW27D18` | 0 in 5,973 |
| `func_80028220` | text for func_80028784; not read | `Text28220`, `ST_TEXT28220` | 0 in 12,099 |
| `func_80028364` | text for func_80028784 and the menu; not read | `Text28364`, `ST_TEXT28364` | 0 in 673,517 |
| `func_800286A8` | text for func_80028784, the menu and the prompt; not read | `Text286A8`, `ST_TEXT286A8` | 0 in 18,106 |
| `func_80028784` | the draw stage's text when 0x8017BD66 is set; not read | `Text28784`, `ST_TEXT28784` | not reached |
| `func_8002948C` | the menu's last call before presenting; not read | `Draw2948C`, `ST_DRAW2948C` | 0 in 34 |
| `func_80029618` | a window or panel, x y w h; the menu and the prompt | `Window29618`, `ST_WINDOW29618` | 0 in 6,012 |
| `func_80033110` | called from the draw stage when the byte at 0x80199140 is 0, 5 or 6; not read | `Draw33110`, `ST_DRAW33110` | 0 in 995 |
| `func_80033DB8` | per entity before its model is drawn; calls func_80015258 | `ModelLight`, `ST_MODELLIGHT` | 0 in 11,164 |
| `func_800340F8` | a draw-stage call; a leaf; not read | `Draw340F8`, `ST_DRAW340F8` | 0 in 1,116 |
| `func_80034604` | a model step in func_80022678 and func_80021AC4; not read | `ModelStep34604`, `ST_MODELSTEP34604` | not reached |
| `func_80034FC4` | a draw-stage call; a leaf; not read | `Draw34FC4`, `ST_DRAW34FC4` | 0 in 1,116 |
| `func_80035AA8` | a model step in func_80022678 and func_80021AC4; not read | `ModelStep35AA8`, `ST_MODELSTEP35AA8` | 0 in 1,714 |

`scripts/verify_summary.py <log>` sums a run's verify lines. The acceptance test
passes with all 57 on (2026-11-10), and play still draws 144 frames and 20 ticks a
second. Three were not reached by the session: `func_8001BF10` and `func_80034604`
(model steps) and `func_80028784` (the draw stage's text). The names with numbers in them are placeholders until the routine is read.
`scripts/split_literal.py` moves a long transcription's straight-line runs (no label,
`goto`, brace or poll) into `<Class>Runs.cs`, each named after the label before it,
so no file passes 500 lines; `ModelDraw`, `SceneStep1EEAC` and `SceneStep1E5A0` are
split that way and still verify at 0.
A count is calls, not coverage: `Models2` and `HeldModel` drew nothing in map 0, so
their drawing branches have not been compared yet.

**Mismatches from the sound chip.** Over the 12-area tour (2026-11-10) `Models` read 2
mismatches in 2,875 calls and `func_800246E4` 1 in 2,878, each one byte in
`0x801DDBF8..0x801DDBFA`, recompiled `01`, ours `03`. `func_800241D0` calls
`SpuGetAllKeysStatus(0x801DDBF8)`, which copies the 24 voices' key states there from
the SPU, outside RAM: a voice that changes state between verify's two runs reads
differently. A clock, not a transcription error; verify cannot restore the SPU.

**A routine that presents a frame cannot be verified this way**: the second run
waits for another vblank, and the sound library's and the disc queue's callbacks run
again (`func_800214CC`, the blank frame, read thousands of bytes different, first at
`0x80087DFC`, and the session never reached the area). So the menu's frame
`func_80029B74`, the prompt's `func_8002CEF8` and the blank frame stay recompiled;
the drawing they call is in C# and verified. Verdite1 verified its menu with a
`twin` written over memory; that, or a verify that stops at the present, is open.

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
- The card screens, a fight on purpose, the area modules' own drawing, `VS.EXE`.
  `scripts/tour.py` reaches 12 areas; the other 24 need an arrival point that holds.
- Who checks the packet buffer's end, and what the menu does with buffer 0's top.
