#!/usr/bin/env python3
"""The acceptance test: boot to an area, save, load both ways, change areas, die.

Run from the repository root after building Release. It drives the game through
ST_SHELL and judges it by memory and the card, never by the picture. See "The
acceptance test" in docs/DEVELOPMENT.md. carda.sav is set aside for the run, and
put back afterwards.
"""

import json
import os
import shutil
import socket
import subprocess
import sys
import tempfile
import time

GAME_DLL = "bin/Release/net10.0/ShadowTower.dll"
DISC = "disc/ShadowTower.cue"
CARD = "carda.sav"
PORT = 27904
BOOT_SECONDS = 120
SETTLE_SECONDS = 12
MAX_PROMPTS = 14
NEAR_SAVED_SPOT = 0x800

SAVE_SCREEN_SETUP = "80058DC4"
SAVE_SCREEN = "80062C28"
SAVE_SCREEN_STATE = "1F8002DC"
SAVE_SCREEN_TEARDOWN = "80058ED4"
SAVE_FILE_NAME = b"BASLUS-00863FS_ST"
MENU_ITEM_LOAD = 6
FRAME_SIZE = 0x80
DIRECTORY_ENTRIES = 15


class Failed(Exception):
    pass


class Game:
    def __init__(self, autostart, log_path, boot_exe=None):
        env = dict(os.environ, ST_SHELL="1", ST_AUTOSTART=autostart, ST_SETUP="0")
        if boot_exe:
            env["ST_BOOTEXE"] = boot_exe
        self.log = open(log_path, "w")
        self.process = subprocess.Popen(
            ["dotnet", GAME_DLL, DISC], env=env, stdout=self.log, stderr=subprocess.STDOUT
        )
        self.channel = self.connect()

    def connect(self):
        for _ in range(120):
            try:
                connection = socket.create_connection(("127.0.0.1", PORT), timeout=2)
            except OSError:
                time.sleep(0.5)
                continue
            connection.settimeout(40)
            return connection.makefile("rw")
        raise Failed("the game never opened its command channel")

    def stop(self):
        self.process.terminate()
        self.process.wait()
        self.log.close()

    def send(self, command):
        self.channel.write(command + "\n")
        self.channel.flush()
        return self.channel.readline().strip()

    def state(self):
        reply = self.send("state")
        try:
            return json.loads(reply)
        except json.JSONDecodeError:
            return {"program": "?", "map": -1, "frames": 0, "inGame": False}

    def press(self, button, hold_ms=200, settle_s=3.0):
        self.send(f"press {button} {hold_ms}")
        time.sleep(hold_ms / 1000 + settle_s)

    def position(self):
        """[x, y, z]; y is the height. Empty outside an area."""
        return self.state().get("pos", [])

    def wait_until(self, what, done, seconds=BOOT_SECONDS, pressing=None):
        deadline = time.time() + seconds
        while time.time() < deadline:
            if done(self.state()):
                return
            if pressing:
                self.send(f"press {pressing} 200")
            time.sleep(2.0)
        raise Failed(f"never {what}: {self.state()}")

    def warp(self, map_number, arrival=()):
        self.send(" ".join(["warp", str(map_number), *map(str, arrival)]))
        self.wait_until(f"reached map {map_number}",
                        lambda state: state["map"] == map_number and state["inGame"], 40)
        before = self.state()["frames"]
        time.sleep(1)
        if self.state()["frames"] - before < 20:
            raise Failed(f"frames stopped after loading map {map_number}")


def card_has_save():
    with open(CARD, "rb") as card:
        data = card.read()
    frames = (data[FRAME_SIZE * i: FRAME_SIZE * (i + 1)] for i in range(1, DIRECTORY_ENTRIES + 1))
    return any(SAVE_FILE_NAME in frame for frame in frames)


def near_saved_spot(position, saved):
    """X and Z within a fraction of the walk away. The height is left out: a load
    lands up to 0x2630 higher or lower than read before saving ("What is still open"
    in docs/DEVELOPMENT.md)."""
    if len(position) != 3:
        return False
    return all(abs(position[axis] - saved[axis]) <= NEAR_SAVED_SPOT for axis in (0, 2))


def alive(state):
    return state["inGame"] and not state["dead"] and state["hp"] > 0


def walk_to_new_spot(game):
    start = game.position()
    game.press("Up", 1200, 0.5)
    if game.position() == start:
        raise Failed("holding Up did not move the player")


def walk_away(game, saved):
    """Turn and walk until off the saved spot. Presses straight after the save screen
    closes are sometimes not taken, so it tries a few times."""
    for _ in range(4):
        game.press("Left", 600, 0.3)
        game.press("Up", 1000, 0.5)
        if not near_saved_spot(game.position(), saved):
            return
    raise Failed(f"the player did not leave the saved spot: {game.position()}")


def confirm_until(game, done):
    for _ in range(MAX_PROMPTS):
        game.press("Cross")
        if done():
            return
    raise Failed(f"the prompts ran out before the screen finished: position {game.position()}")


def play_save_load(log_dir):
    game = Game("new", os.path.join(log_dir, "boot.log"))
    try:
        game.wait_until("reached the first area", lambda state: state["inGame"] and state["map"] == 0)
        print("ok   boot: movies, title, opening text, first area")
        time.sleep(SETTLE_SECONDS)

        walk_to_new_spot(game)
        print("ok   walk")

        saved = game.position()
        if not alive(game.state()):
            raise Failed(f"the player is not alive to save: {game.state()}")
        game.send(f"call {SAVE_SCREEN_SETUP} 1 1")
        game.send(f"call {SAVE_SCREEN} 1")
        time.sleep(3)
        confirm_until(game, lambda: card_has_save() and game.send(f"peek {SAVE_SCREEN_STATE} 2") == "0000")
        game.send(f"call {SAVE_SCREEN_TEARDOWN}")
        time.sleep(4)
        print(f"ok   save: {SAVE_FILE_NAME.decode()} on card A")

        walk_away(game, saved)

        game.press("Select", 200, 2.0)
        for _ in range(MENU_ITEM_LOAD):
            game.press("Down", 150, 0.4)
        game.press("Cross")
        confirm_until(game, lambda: near_saved_spot(game.position(), saved))
        print("ok   load from the in-game menu: position matches the save")
        return saved
    finally:
        game.stop()


def load_from_title_change_die(log_dir, saved):
    game = Game("load", os.path.join(log_dir, "title.log"), boot_exe="title")
    try:
        game.wait_until("loaded the save from the title",
                        lambda state: alive(state) and near_saved_spot(state["pos"], saved))
        print("ok   load from the title: position matches the save")

        # The area change comes after the saves: map 0's coordinates are no floor in
        # map 1, so the player dies on arriving there ("Changing areas" in
        # docs/GAME_INTERNALS.md), and that death is the test's.
        game.warp(0, saved)
        if not alive(game.state()):
            raise Failed(f"a warp to the saved spot killed the player: {game.state()}")
        game.warp(1)
        print("ok   area change: map 0 at a given point alive, then map 1")

        game.wait_until("died", lambda state: state["dead"] and state["hp"] == 0, 15)
        game.wait_until("went back to the title", lambda state: state["program"] == "title",
                        60, pressing="Cross")
        print("ok   die: HP 0, back to the title")
    finally:
        game.stop()


def main():
    log_dir = tempfile.mkdtemp(prefix="st-acceptance-")
    backup = os.path.join(log_dir, "carda.before.sav")
    had_card = os.path.exists(CARD)
    if had_card:
        shutil.move(CARD, backup)
    try:
        saved = play_save_load(log_dir)
        time.sleep(1)
        load_from_title_change_die(log_dir, saved)
    except Failed as failure:
        print(f"FAIL {failure} (logs in {log_dir})")
        return 1
    finally:
        if os.path.exists(CARD):
            os.remove(CARD)
        if had_card:
            shutil.move(backup, CARD)
    print("PASS")
    return 0


if __name__ == "__main__":
    sys.exit(main())
