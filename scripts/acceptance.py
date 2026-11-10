#!/usr/bin/env python3
"""Phase 2's acceptance test: boot to an area, change areas, save, load both ways.

Run from the repository root after building Release. It drives the game through
ST_SHELL and judges it by memory and the card, never by the picture. See "The
acceptance test" in docs/DEVELOPMENT.md. carda.sav is set aside for the run, and
put back afterwards.
"""

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
NEAR_SAVED_SPOT = 0x3000

PLAYER_POSITION = "80199170"
AREA_REQUEST = "801B5924"
REQUEST_AREA_CHANGE = "80044EE8"
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
    def __init__(self, boot_exe, log_path):
        env = dict(os.environ, ST_SHELL="1")
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
        if not reply.startswith("program"):
            return {"program": "?", "area": "?", "frames": "0"}
        return dict(field.split("=") for field in reply.split())

    def press(self, button, hold_ms=200, settle_s=3.0):
        self.send(f"press {button} {hold_ms}")
        time.sleep(hold_ms / 1000 + settle_s)

    def position(self):
        return self.send(f"peek {PLAYER_POSITION} 8")

    def wait_for(self, program, area=None, pressing=None):
        deadline = time.time() + BOOT_SECONDS
        while time.time() < deadline:
            state = self.state()
            if state["program"] == program and (area is None or state["area"] == area):
                return
            if pressing:
                self.send(f"press {pressing} 200")
            time.sleep(2.5)
        raise Failed(f"never reached {program} {area or ''}: {self.state()}")

    def request_area(self, map_number):
        name = f"fdat{map_number * 10 + 2:03d}"
        self.send(f"poke {AREA_REQUEST} {map_number:02X}00000000")
        self.send(f"call {REQUEST_AREA_CHANGE}")
        deadline = time.time() + 15
        while time.time() < deadline and self.state()["area"] != name:
            time.sleep(0.5)
        if self.state()["area"] != name:
            raise Failed(f"area {name} never loaded")

        before = int(self.state()["frames"])
        time.sleep(1)
        if int(self.state()["frames"]) - before < 20:
            raise Failed(f"frames stopped after loading {name}")


def card_has_save():
    with open(CARD, "rb") as card:
        data = card.read()
    frames = (data[FRAME_SIZE * i: FRAME_SIZE * (i + 1)] for i in range(1, DIRECTORY_ENTRIES + 1))
    return any(SAVE_FILE_NAME in frame for frame in frames)


def near_saved_spot(position, saved):
    """Within a fraction of the walk away: a load lands up to 0x2630 from where the
    position read before saving says ("What is still open" in docs/DEVELOPMENT.md)."""
    if len(position) != 16:
        return False
    for start in (0, 8):
        here = int.from_bytes(bytes.fromhex(position[start:start + 8]), "little", signed=True)
        there = int.from_bytes(bytes.fromhex(saved[start:start + 8]), "little", signed=True)
        if abs(here - there) > NEAR_SAVED_SPOT:
            return False
    return True


def walk_to_new_spot(game):
    start = game.position()
    game.press("Up", 1200, 0.5)
    if game.position() == start:
        raise Failed("holding Up did not move the player")


def confirm_until(game, done):
    for _ in range(MAX_PROMPTS):
        game.press("Cross")
        if done():
            return
    raise Failed(f"the prompts ran out before the screen finished: position {game.position()}")


def play_save_change_load(log_dir):
    game = Game(None, os.path.join(log_dir, "boot.log"))
    try:
        game.wait_for("st", "fdat002", pressing="Start")
        print("ok   boot: movies, title, opening text, first area")
        time.sleep(SETTLE_SECONDS)

        walk_to_new_spot(game)
        print("ok   walk")

        game.request_area(1)
        game.request_area(0)
        print("ok   area change: fdat012 and back to fdat002")
        time.sleep(SETTLE_SECONDS)

        saved = game.position()
        game.send(f"call {SAVE_SCREEN_SETUP} 1 1")
        game.send(f"call {SAVE_SCREEN} 1")
        time.sleep(3)
        confirm_until(game, lambda: card_has_save() and game.send(f"peek {SAVE_SCREEN_STATE} 2") == "0000")
        game.send(f"call {SAVE_SCREEN_TEARDOWN}")
        time.sleep(4)
        print(f"ok   save: {SAVE_FILE_NAME.decode()} on card A")

        game.press("Left", 600, 0.3)
        game.press("Up", 800, 0.5)
        if near_saved_spot(game.position(), saved):
            raise Failed("the player did not leave the saved spot")

        game.press("Select", 200, 2.0)
        for _ in range(MENU_ITEM_LOAD):
            game.press("Down", 150, 0.4)
        game.press("Cross")
        confirm_until(game, lambda: near_saved_spot(game.position(), saved))
        print("ok   load from the in-game menu: position matches the save")
        return saved
    finally:
        game.stop()


def load_from_title(log_dir, saved):
    game = Game("title", os.path.join(log_dir, "title.log"))
    try:
        game.wait_for("title")
        time.sleep(8)
        game.press("Start")
        game.press("Down", 150, 1.0)
        game.press("Start", 200, 4.0)
        game.wait_for("st", pressing="Start")
        confirm_until(game, lambda: near_saved_spot(game.position(), saved))
        print("ok   load from the title: position matches the save")
    finally:
        game.stop()


def main():
    log_dir = tempfile.mkdtemp(prefix="st-acceptance-")
    backup = os.path.join(log_dir, "carda.before.sav")
    had_card = os.path.exists(CARD)
    if had_card:
        shutil.move(CARD, backup)
    try:
        saved = play_save_change_load(log_dir)
        time.sleep(1)
        load_from_title(log_dir, saved)
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
