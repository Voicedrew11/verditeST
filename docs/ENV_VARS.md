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
| `ST_NODITHER` | on | `0`: the GPU's ordered dither as the console draws it |
| `ST_NODITHER_PROBE` | off | `1`: every 2 s, draw envs and table E1 words that asked for dither, and GPUSTAT's dither bit |
| `ST_TRUECOLOR` | on | `0`: the console's 15-bit colour |
| `ST_PERSPECTIVE` | on | `0`: the console's affine texture mapping |
| `ST_PERSPECTIVE_PROBE` | off | `1`: every 2 s, the address map's roots, propagations, hits and misses |
| `ST_SUBPIXEL` | on | `0`: every vertex snapped to a whole pixel |
| `ST_SUBPIXEL_PROBE` | off | `1`: every 2 s, the map's counters and how far the vertices moved |
| `ST_ZBUFFER` | on | `0`: occlusion by the ordering table alone, as the console |
| `ST_WIDESCREEN` | 16:9 | `off` for 4:3; `16:10`, `21:9` or a number for others |
| `ST_WIDESCREEN_PROBE` | off | `1`: every 2 s, primitives reaching the margin, tints stretched, near faces widened |
| `ST_WIDESCREEN_EFFECTS` | on | `0`: leave the full-screen tints 320 wide |
| `ST_WIDESCREEN_HUD` | off | `1`: move the HUD's panels out to the new edges |
| `ST_NEARSCREEN` | on | `0`: leave libgte's division at the 320-pixel screen |
| `ST_KEYS` | fps | `stock`: RecompOne's own keys, not the port's WASD layout |
| `ST_MOUSE` | on | `0`: no mouse look (Verdite Core's `Mouse`; also `ST_MOUSE_TURN`, `ST_MOUSE_LOOK`, `ST_MOUSE_INVERTY`, `ST_MOUSE_BUTTONS`, `ST_MOUSE_KEY`, and `ST_MOUSE_LEAD=0` for no instant mouse look) |
| `ST_MOUSE_SYNTH` | off | `yaw,pitch`: a synthetic hand, that step every tick for 40 ticks of 60 |
| `ST_MOUSE_PROBE` | off | `1`: a line a second, the mouse's ticks driven, landed on their step, and any that moved undriven |
| `ST_MAPCOVERAGE` | off | `1`: every five seconds, the address map's hits by the stage-14 call that wrote each polygon |
| `ST_WORDWATCH` | off | a hex address: every five seconds, which of stage 14's calls changed that word, and how many on idle frames |
| `ST_ZBUFFER_PROBE` | off | `1`: every 2 s, triangles depth-tested and kept in painter's order, depth steps |
| `ST_BLENDORDER` | on | `0`: blended surfaces drawn as the depth test meets them, not in table order |
| `ST_ZBUFFER_THRESHOLD` | 0 | units: restart the depth buffer on a step this large; 0 keeps one buffer a frame |
| `ST_RATECENSUS` | off | seconds: with pacing on, the RAM words that change on frames no stage ran on, printed every window of that many seconds with the map |
