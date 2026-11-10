# Environment variables

Every `ST_*` switch the port reads, in one list, with what each does and its
default. Add an entry here when a switch is added to `Program.cs`.

| variable | default | what |
|---|---|---|
| `ST_LOG` | none | the runtime's log channels, comma-separated: `bios,cd,gpu,dma,sdk,spu,mdec,irq`, or `all` |
| `ST_BOOTEXE` | none | start at `title`, `optxt`, `st`, `end` or `vs` instead of the movies; a diagnostic, since `st` gets none of the title's hand-over |
| `ST_AGENT` | off | `1`: print the beacon's state about once a second and on every load |
| `ST_AUTOSTART` | off | `new` or `load`: boot into play with nobody at the pad |
| `ST_SHELL` | off | `1` or a port: the command channel, see "Driving the game without a person" in `docs/DEVELOPMENT.md` |
| `ST_PACING` | on | `0`: frame pacing off; the game keeps its own pace, which `VBlankPacing` holds to 20 |
| `ST_FPS` | 144 | the picture's rate under pacing, or `off` for uncapped |
| `ST_TICKRATE` | 20 | the world's rate under pacing (5 to 60 Hz); changes gameplay speed, a comparison only |
| `ST_FPS_PROBE` | off | `1`: a pacing line a second: fps drawn, VSync calls, ticks, stages run and skipped, packets a frame |
| `ST_PACING_NOBOUNDARY` | off | `1`: leave the frame boundary unhooked, to test the watchdog |
| `ST_VBLANKPACING` | on | `0`: `VSync(n)` returns at once, as the runtime's does; with pacing off the game then runs at 60, three times its speed. A comparison only |
| `ST_VBLANKPACING_PROBE` | off | `1`: a line a second: `VSync` calls held, by mode, the mean wait, and calls exempted inside the main loop's draw |
| `ST_STAGEPROBE` | off | `1`: every five seconds, each main-loop stage's calls a second, ordering-table words it wrote, and frames it presented |
| `ST_GEOPROBE` | off | `1`: every five seconds, the packets each of stage 14's calls (and the scene's and the models' calls) added to the ordering table, by type |
| `ST_PRESENTPROBE` | off | `1`: every five seconds, frames presented a second by who presented them: stage 14 or a loop drawing its own |
| `ST_PRIMBUF_PROBE` | off | `1`: every five seconds, the packet-buffer bytes a frame spent, mean and peak, and the buffer's bounds |
| `ST_LITERAL` | 1 | `0`: every draw routine in `patches/Draw/` on its recompiled body; `verify`: both run and compared, the recompiled result kept |
| `ST_<NAME>` | `ST_LITERAL` | one draw routine's mode, `1`, `0` or `verify`; the names are in "The draw path in C#" in `docs/GEOMETRY.md` |
| `ST_LITERAL_PROBE` | off | `1`: every five seconds, how many times each draw routine ran in C# |
| `ST_LITERAL_DUMP` | none | a directory: the first mismatch of each routine under verify writes the entry, recompiled and transcribed RAM there |
| `ST_SMOOTH` | on | `0`: the camera is not carried between ticks; the picture's eye changes 20 times a second |
| `ST_SMOOTH_MODELS` | on | `0`: the models' positions and angles are not carried, the camera still is |
| `ST_SMOOTH_PROBE` | off | `1`: a line a second: frames drawn, distinct cameras, tick samples, snaps, backward steps |
| `ST_RATECENSUS` | off | seconds: with pacing on, the RAM words that change on frames no stage ran on, printed once |
