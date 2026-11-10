#!/usr/bin/env python3
"""Print the table of draw routines in C# for docs/GEOMETRY.md, from patches/Draw/ and
a verify run's log.

    python3 scripts/literal_table.py <verify log>

One row per Literal: the routine, its class and switch, and its verify count. A
routine the run never called reads "not reached".
"""
import glob
import re
import sys

REG = re.compile(r'new\(typeof\((\w+)\), "[^"]*", "(\w+)", 0x([0-9A-F]{8}), Run,\s*"([^"]*)"\)', re.S)
LINE = re.compile(r"\((func_[0-9A-F]{8})\): (\d+) call\(s\), (\d+) mismatch")


def main() -> None:
    if len(sys.argv) < 2:
        sys.exit(__doc__)
    calls, bad = {}, {}
    for line in open(sys.argv[1], errors="replace"):
        if m := LINE.search(line):
            calls[m[1]] = calls.get(m[1], 0) + int(m[2])
            bad[m[1]] = bad.get(m[1], 0) + int(m[3])
    rows = []
    for path in glob.glob("patches/Draw/*.cs"):
        for cls, env, addr, tip in REG.findall(open(path).read()):
            f = f"func_{addr}"
            seen = f"{bad[f]} in {calls[f]:,}" if f in calls else "not reached"
            rows.append((addr, f"| `{f}` | {tip.rstrip('.')} | `{cls}`, `ST_{env}` | {seen} |"))
    print("| routine | what | class, switch | mismatches in calls |")
    print("|---|---|---|---|")
    for _, row in sorted(rows):
        print(row)


if __name__ == "__main__":
    main()
