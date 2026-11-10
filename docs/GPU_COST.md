# The draw's cost: the baseline

Phase 11, Step 0.3 (`docs/RENDERER_PLAN.md`): the figure with the GPU path off, so the
GPU path's has something to be compared with.

**How**: `ST_DRAW_TIMING=1` (`patches/Render/DrawTiming.cs`) times the main loop's
stage 14 from its entry to its swap (`func_80017680`, which waits on the pacing and
is left out): every routine of the frame's drawing on the CPU, the literal C# and
every hook on the way included. Wall time on the game thread, so another probe on at
the same time inflates it; measured with nothing else on. Five-second windows,
mean, 95th percentile and worst, by map.

**Measured** 2026-11-10 over `scripts/tour.py` (the defaults: literal C# on, 144 fps,
the picture's switches on, 16:9), windows of more than 100 frames a second:

| map | mean ms | p95 ms (largest window) | worst ms |
|---|---|---|---|
| 0 | 1.35 | 0.76 | 606 (the first frames) |
| 2 | 0.45 | 0.60 | 1.3 |
| 4 | 1.17 | 0.61 | 604 (the first frames) |
| 21 | 0.32 | 0.53 | 1.3 |
| 23 | 0.30 | 0.37 | 21.9 |
| 24 | 0.57 | 0.74 | 33.0 |
| 32 | 0.38 | 0.60 | 3.5 |
| 38 | 0.39 | 0.50 | 1.1 |
| 41 | 1.15 | 0.87 | 605 (the first frames) |
| 44 | 0.84 | 1.37 | 173 |
| 45 | 0.69 | 1.16 | 57.1 |
| 46 | 0.61 | 0.73 | 1.4 |

So the frame's drawing costs **under 1.4 ms at the 95th percentile everywhere**, about
a fifth of a 144 Hz frame (6.94 ms). The means above 1 ms carry a single stall of
about 600 ms on the first frames after a new game or an arrival (the window that
holds a boot or a load); the next windows of the same map are back under 1 ms. Not
read: what the stall is (JIT, a first upload, the loader's last work).

**Before**: timed to the end of stage 14 instead, every frame read 6.94 ms, the
pacing's wait in the swap. A GPU figure must be taken the same way, to the swap.
