#!/usr/bin/env python3
"""Rebuild config/funcmaps/ from the disc: sweep, cut at the end of code, harvest.

The order matters and is explained in docs/RECOMPILATION.md, "The function maps".
Run tools/verdite-core/scripts/merge_branch_spans.py after this, then
merge_sdk_names.py.

  python3 scripts/make_funcmaps.py
"""
import json
import struct
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
sys.path.insert(0, str(ROOT / "tools/verdite-core/scripts"))
from add_call_targets import merge, read_exe, scan_call_targets  # noqa: E402
from extract_file import find_entry  # noqa: E402
from inspect_disc import open_disc, resolve_image  # noqa: E402

CUE = ROOT / "disc/ShadowTower.cue"
MAPS = ROOT / "config/funcmaps"
RECOMPILER = ["dotnet", "run", "--project", str(ROOT / "tools/RecompOne/RecompOne.Recompiler"),
              "-c", "Release", "--no-build", "--"]

# name, file on the disc, end of code (the end of the last real function).
EXECUTABLES = [
    ("main", "SLUS_008.63", None),
    ("str", "STR.EXE", 0x8001FDFC),
    ("st", "ST.EXE", 0x8007F3E4),
    ("title", "TITLE.EXE", 0x8002690C),
    ("optxt", "OPTXT.EXE", 0x8001DF50),
    ("vs", "VS.EXE", 0x8004A218),
    ("end", "END.EXE", 0x8001D280),
]

# Starts the sweep's call discovery made from a jal-shaped data word, in the middle
# of straight-line code; nothing calls them and the instruction before runs on.
FALSE_STARTS = {"vs": [0x80042FF0]}

MODULE_BASE = 0x801EB560
MODULE_HEADER_WORDS = 1 + 32 + 1 + 81  # id, slots, second id, pointer table
MODULE_FIRST_POINTER = 4 * MODULE_HEADER_WORDS
OP_JAL = 3
FDAT = "ST/COM/FDAT.T"
# Maps whose ST/CHRn/Mnn.T is a placeholder have no code module.
MODULELESS_MAPS = {7, 16, 19, 22, 29, 31, 34, 35, 36, 39, 42, 43}
JR_RA = 0x03E00008


def sweep(args, out):
    cmd = RECOMPILER + ["--generate-function-file", "-linear-sweep", "-disc", str(CUE), *args,
                        "-out", str(out)]
    subprocess.run(cmd, check=True, stdout=subprocess.DEVNULL)
    return json.loads(out.read_text())


def rejoin_false_starts(funcs, false_starts):
    out = []
    for f in funcs:
        if int(f["address"], 16) in false_starts:
            out[-1]["size"] += int(f["size"])
        else:
            out.append(f)
    return out


def make_executable(disc, name, file, code_end):
    out = MAPS / f"{name}.json"
    base, text, entry_pc = read_exe(disc, file)
    doc = sweep(["-file", file, "-base", f"{base:X}", "-skip", "800"], out)
    swept = len(doc["functions"])
    # What precedes the entry point is read-only data, and past code_end is more.
    funcs = [f for f in doc["functions"]
             if (code_end is None or int(f["address"], 16) < code_end)
             and (code_end is None or int(f["address"], 16) >= entry_pc)]
    funcs = rejoin_false_starts(funcs, FALSE_STARTS.get(name, []))
    intervals = [(int(f["address"], 16), int(f["size"])) for f in funcs]
    limit = base + len(text) if code_end is None else code_end
    code = text[:limit - base]
    targets = scan_call_targets(base, code, intervals)
    targets.add(entry_pc)
    doc["functions"] = merge(funcs, targets, base, limit)
    out.write_text(json.dumps(doc, indent=2))
    print(f"{name}: swept {swept}, cut {len(funcs)}, harvested {len(doc['functions'])}")


def module_entries(disc):
    fdat = find_entry(disc, FDAT)
    data = disc.read(fdat["lba"], fdat["size"])
    count = struct.unpack_from("<H", data, 0)[0]
    starts = struct.unpack_from(f"<{count + 1}H", data, 2)
    for map_number in range(48):
        if map_number in MODULELESS_MAPS:
            continue
        index = map_number * 10 + 2
        offset, size = starts[index] * 2048, (starts[index + 1] - starts[index]) * 2048
        yield index, offset, data[offset:offset + size]


def make_module(index, offset, module):
    words = struct.unpack(f"<{len(module) // 4}I", module)
    code_end = 4 * max(i for i, word in enumerate(words) if word == JR_RA) + 8
    # Code begins at the lowest address anything refers to: the 32 slots, the 81
    # pointers after them, the data and pointers some modules add, and internal jals.
    in_module = lambda a: MODULE_FIRST_POINTER <= a - MODULE_BASE < code_end
    jals = {0x80000000 | ((w & 0x03FFFFFF) << 2) for w in words if w >> 26 == OP_JAL}
    header_end = 4 * MODULE_HEADER_WORDS
    code_offset = min(a - MODULE_BASE for a in jals | set(words[1:MODULE_HEADER_WORDS]) if in_module(a))
    pointers = {w for w in words[1:code_offset // 4] if in_module(w)}
    assert code_offset >= header_end, (index, hex(code_offset))
    name = f"fdat{index:03d}"
    out = MAPS / f"{name}.json"
    file_offset = offset + code_offset
    doc = sweep(["-file", FDAT, "-base", f"{MODULE_BASE + code_offset:X}",
                 "-offset", f"{file_offset:X}", "-skip", "0", "-size", f"{code_end - code_offset:X}"], out)
    funcs = doc["functions"]
    intervals = [(int(f["address"], 16), int(f["size"])) for f in funcs]
    code = module[code_offset:code_end]
    base = MODULE_BASE + code_offset
    targets = scan_call_targets(base, code, intervals) | pointers
    doc["functions"] = merge(funcs, targets, base, MODULE_BASE + code_end)
    out.write_text(json.dumps(doc, indent=2))
    print(f"{name}: offset {offset:#x} size {len(module):#x} code {code_offset:#x}-{code_end:#x}"
          f" swept {len(funcs)} harvested {len(doc['functions'])}")


def main():
    MAPS.mkdir(parents=True, exist_ok=True)
    disc = open_disc(resolve_image(CUE))
    for name, file, code_end in EXECUTABLES:
        make_executable(disc, name, file, code_end)
    for index, offset, module in module_entries(disc):
        make_module(index, offset, module)


if __name__ == "__main__":
    main()
