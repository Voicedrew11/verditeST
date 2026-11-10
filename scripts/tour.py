#!/usr/bin/env python3
"""Visit every area with a known arrival point, stand and turn in each.

    python3 scripts/tour.py <log> [ST_X=value ...]

Run from the repository root after building Release. Boots a new game, then for each
map in patches/Arrivals.cs warps there without a point (so the player lands where a
door would put them), stands, turns a full circle in place and stands again. The
extra ST_* settings go to the game, usually ST_LITERAL=verify or a probe. It prints
each map's HP after the visit and judges nothing else: read the log. After a death it
boots a new game, whose log is <log>.2, <log>.3 and so on. About 25 s a map. See "Measuring the draw path" in docs/DEVELOPMENT.md.
"""
import os
import re
import sys
import time

sys.path.insert(0, os.path.dirname(__file__))
from acceptance import Game  # noqa: E402

KEEP_ALIVE = ["poke 80199140 00", "poke 80198F2A F002"]  # state free, HP 752


def maps_with_arrivals():
    text = open(os.path.join(os.path.dirname(__file__), "..", "patches", "Arrivals.cs")).read()
    return [int(m) for m in re.findall(r"^\s*\[(\d+)\] = new\(", text, re.M)]


def main() -> None:
    if len(sys.argv) < 2:
        sys.exit(__doc__)
    for setting in sys.argv[2:]:
        key, _, value = setting.partition("=")
        os.environ[key] = value
    log = sys.argv[1]
    game, boots = None, 0
    try:
        for area in maps_with_arrivals():
            if game is None:  # first, or after a death: a dead player loads no area
                boots += 1
                game = Game("new", f"{log}.{boots}" if boots > 1 else log)
                game.wait_until("reached an area", lambda s: s.get("inGame", False))
            for command in KEEP_ALIVE:
                game.send(command)
            game.send(f"warp {area}")
            try:
                game.wait_until(f"reached map {area}", lambda s: s["map"] == area and s["inGame"], 40)
            except Exception:
                print(f"tour: map {area}: never loaded", flush=True)
                game.stop()
                game = None
                continue
            time.sleep(4)
            game.press("right", 4000, 1.0)
            time.sleep(3)
            state = game.state()
            print(f"tour: map {area}: hp {state.get('hp')} dead {state.get('dead')} pos {state.get('pos')}", flush=True)
            if state.get("dead"):
                game.stop()
                game = None
    finally:
        if game is not None:
            game.stop()

if __name__ == "__main__":
    main()
