#!/usr/bin/env python3
"""A scripted play session for probes: boot a new game, turn, walk, open the menu.

    python3 scripts/session.py <log> [ST_X=value ...]

Run from the repository root after building Release. The extra ST_* settings are
passed to the game (a probe's switch, usually); its output goes to <log>. The same
moves every run, so two runs' counters can be compared. It judges nothing: read the
log. See "Measuring the draw path" in docs/DEVELOPMENT.md.
"""

import os
import sys
import time

sys.path.insert(0, os.path.dirname(__file__))
from acceptance import Game  # noqa: E402

# (buttons, hold in ms, seconds to wait after); an empty button list only waits.
MOVES = [
    ("", 0, 6.0),             # stand: the area as it loads
    ("up", 2000, 1.0),        # walk forward
    ("down", 2000, 1.0),      # and back to the start: the start is ringed by fatal drops,
    ("left", 1500, 1.0),      # so the player only walks along the first heading,
    ("right", 3000, 1.0),     # and turns in place
    ("left", 1500, 1.0),      # back to the first heading (walking while turning fell)
    ("select", 150, 4.0),     # the menu, held open
    ("select", 150, 3.0),
    ("circle", 150, 2.0),
    ("", 0, 6.0),             # stand again
    ("left", 1500, 0.5),      # off the ledge: a death, the death screen,
    ("up", 2000, 8.0),
    ("cross", 150, 8.0),      # and Cross back to the title
]


def main() -> None:
    if len(sys.argv) < 2:
        sys.exit(__doc__)
    for setting in sys.argv[2:]:
        key, _, value = setting.partition("=")
        os.environ[key] = value
    game = Game("new", sys.argv[1])
    try:
        game.wait_until("reached an area", lambda s: s.get("inGame", False))
        for buttons, hold_ms, settle_s in MOVES:
            if buttons:
                game.press(buttons, hold_ms, settle_s)
            else:
                time.sleep(settle_s)
            state = game.state()
            print(f"session: {buttons or 'wait'}: hp {state.get('hp')} pos {state.get('pos')}", flush=True)
    finally:
        game.stop()


if __name__ == "__main__":
    main()
