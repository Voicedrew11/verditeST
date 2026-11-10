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
