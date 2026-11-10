#!/usr/bin/env python3
"""Every area's object records, and the arrival points their doors name.

    python3 scripts/door_census.py <log> <out.json>

Boots a new game, warps into each map that has a module, reads its 0xF0 object records
(0x58 bytes from 0x801C2230) and keeps every record whose +0x48 names another map with
a module. Writes them as JSON: for each map, the records that lead into it, with the
arrival point decoded as the door code at 0x800525D0 decodes it ("Changing areas" in
docs/GAME_INTERNALS.md). A warp lands at the player's old coordinates, which in most
maps is a fall and a death; the records are read before that matters, and the player
is kept alive by poking the state byte and HP after each warp.
"""
import json
import os
import struct
import sys
import time

sys.path.insert(0, os.path.dirname(__file__))
from acceptance import Game  # noqa: E402

MAPS = [0, 1, 2, 3, 4, 5, 6, 8, 9, 10, 11, 12, 13, 14, 15, 17, 18, 20, 21, 23, 24, 25, 26,
        27, 28, 30, 32, 33, 37, 38, 40, 41, 44, 45, 46, 47]
RECORDS, RECORD_BYTES, RECORD_COUNT = 0x801C2230, 0x58, 0xF0
STATE, HP, MAX_HP = "80199140", "80198F2A", 752


def read(game, address, count):
    data = b""
    while len(data) < count:
        n = min(4096, count - len(data))
        data += bytes.fromhex(game.send(f"peek {address + len(data):08X} {n}"))
    return data


def arrival(rec):
    """The door code's arrival point: cell bytes biased by 0x40, times 0x1000, plus a
    signed fine offset times 0x80; 0x640 off the height for the eye."""
    cx, cy, cz = (b + 0x40 for b in rec[0x49:0x4C])
    fx, fy, fz = struct.unpack_from("bbb", rec, 0x4C)
    return [(cx << 12) + fx * 0x80, (cy << 12) + fy * 0x80 - 0x640, (cz << 12) + fz * 0x80, rec[0x4F] << 10]


def keep_alive(game):
    game.send(f"poke {STATE} 00")
    game.send(f"poke {HP} {struct.pack('<H', MAX_HP).hex().upper()}")


def main() -> None:
    if len(sys.argv) < 3:
        sys.exit(__doc__)
    game = Game("new", sys.argv[1])
    into = {m: [] for m in MAPS}
    try:
        game.wait_until("reached an area", lambda s: s.get("inGame", False))
        for source in MAPS:
            if source != 0:
                game.send(f"warp {source}")
                game.wait_until(f"reached map {source}", lambda s: s["map"] == source and s["inGame"], 30)
            keep_alive(game)
            time.sleep(0.5)
            data = read(game, RECORDS, RECORD_BYTES * RECORD_COUNT)
            kinds = {}
            for i in range(RECORD_COUNT):
                rec = data[i * RECORD_BYTES:(i + 1) * RECORD_BYTES]
                kinds[rec[5]] = kinds.get(rec[5], 0) + 1
                target = rec[0x48]
                if target in into and target != source and rec[5] not in (0, 0xFF):
                    into[target].append({"from": source, "record": i, "type": f"{rec[5]:02X}",
                                         "arrival": arrival(rec), "raw": rec.hex()})
            print(f"map {source}: types {', '.join(f'{k:02X}x{v}' for k, v in sorted(kinds.items()))}", flush=True)
            keep_alive(game)
    finally:
        game.stop()
        json.dump(into, open(sys.argv[2], "w"), indent=1)


if __name__ == "__main__":
    main()
