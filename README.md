# VerditeST

A static recompilation of **Shadow Tower** (FromSoftware, 1998; NTSC-U disc
`SLUS-00863`) using [RecompOne](https://github.com/BlackLabelHQ/RecompOne).

You must supply your own dump of `SLUS-00863`. No disc data is included, and none
ever will be.

**Status: bring-up.** `docs/PORTING_PLAN.md` is the plan and `NOTES.md` says where
it stands.

## No prebuilt binary

A playable binary cannot be shipped. The generated code is a translation of
FromSoftware's own code, so the assembly that plays the game has to be built on
the machine of somebody who owns the disc. The project ships its inputs and builds
the game there.

## Upstream

This project is built on the RecompOne fork, not directly on upstream RecompOne.
No pull requests and no issues go to upstream RecompOne; see `AGENTS.md`.
