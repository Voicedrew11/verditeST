#!/usr/bin/env python3
"""Walk through real doors to find arrival points the game itself chooses.

    python3 scripts/door_walk.py <log> <doors.json> [from-to ...]

For each ED/EF door record (from scripts/door_census.py's JSON) in a map with a known
arrival point (patches/Arrivals.cs) that leads to a map without one: warp to the
source map, stand the player at the door's trigger (+0x24/+0x28/+0x2C, the eye 0x640
above it), face its heading (+0x20), press Cross, and wait for the target map. Then,
with no more help, read where the door code put the player and whether they are
still alive 8 s later. Prints one line a door. See "Arrival points" in
docs/GAME_INTERNALS.md.
"""
import json
import os
import struct
import sys
import time

sys.path.insert(0, os.path.dirname(__file__))
from acceptance import Game, Failed  # noqa: E402
from tour import maps_with_arrivals  # noqa: E402

PLAYER = 0x80198F28
POS, STATE, HP = 0x80199170, "80199140", "80198F2A"
HEADINGS = (0x268, 0x26A, 0x27A)   # the heading, the door's arrival heading, the yaw
EYE = 0x640


def poke32(game, address, value):
    game.send(f"poke {address:08X} {struct.pack('<i', value).hex().upper()}")


def stand_at_door(game, raw, away=0):
    """At the trigger, or `away` units off it in X (out of its 0xC00 box)."""
    tx, ty, tz = struct.unpack_from("<iii", raw, 0x24)
    tx += away
    heading = struct.unpack_from("<H", raw, 0x20)[0]
    for off in HEADINGS:
        game.send(f"poke {PLAYER + off:08X} {struct.pack('<H', heading).hex().upper()}")
    poke32(game, POS, tx)
    poke32(game, POS + 4, ty - EYE)
    poke32(game, POS + 8, tz)


def walk(log, door):
    game = Game("new", log)
    try:
        game.wait_until("reached an area", lambda s: s["inGame"] and s["map"] == 0, 120)
        if door["from"] != 0:
            game.warp(door["from"])
        game.send(f"poke {STATE} 00")
        game.send(f"poke {HP} F002")
        time.sleep(1)
        raw = bytes.fromhex(door["raw"])
        for _ in range(4):
            # Out of the box first: the door just arrived through stays marked
            # (the byte at +0x50) until the player leaves it.
            stand_at_door(game, raw, away=0x2000)
            time.sleep(0.5)
            game.send(f"poke {STATE} 00")
            game.send(f"poke {HP} F002")
            stand_at_door(game, raw)
            time.sleep(0.3)
            game.press("Cross", 600, 1.0)
            if game.state()["map"] != door["from"]:
                break
        try:
            game.wait_until("reached the target", lambda s: s["map"] == door["to"] and s["inGame"], 40)
        except Failed:
            return f"no load: {game.state()}"
        time.sleep(1)
        arrived = game.state()
        time.sleep(8)
        later = game.state()
        return f"arrived {arrived['pos']} heading {arrived['heading']}; 8 s later dead {later['dead']} pos {later['pos']}"
    finally:
        game.stop()


def main() -> None:
    if len(sys.argv) < 3:
        sys.exit(__doc__)
    known = set(maps_with_arrivals())
    doors = json.load(open(sys.argv[2]))
    only = set(sys.argv[3:])   # "41-30" and so on: only these doors
    todo = [dict(r, to=int(t)) for t, recs in doors.items() for r in recs
            if r["from"] in known and int(t) not in known and r["type"] in ("ED", "EF")
            and (not only or f"{r['from']}-{t}" in only)]
    for i, door in enumerate(todo):
        print(f"{door['from']} -> {door['to']} (record {door['record']}, {door['type']}): "
              f"{walk(f'{sys.argv[1]}.{i}', door)}", flush=True)


if __name__ == "__main__":
    main()
