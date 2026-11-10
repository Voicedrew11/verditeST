#!/usr/bin/env python3
"""Sum a run's Literal verify lines per routine: calls, mismatches, interrupted.

    python3 scripts/verify_summary.py <log>

Prints the first few mismatch samples too. See "The draw path in C#" in
docs/GEOMETRY.md.
"""
import collections
import re
import sys

LINE = re.compile(r"verify (.*?) \((func_\w+)\): (\d+) call\(s\), (\d+) mismatch\(es\), (\d+) interrupted")


def main() -> None:
    if len(sys.argv) < 2:
        sys.exit(__doc__)
    totals = collections.defaultdict(lambda: [0, 0, 0])
    samples = []
    for line in open(sys.argv[1], errors="replace"):
        if m := LINE.search(line):
            t = totals[f"{m[1]} ({m[2]})"]
            t[0] += int(m[3]); t[1] += int(m[4]); t[2] += int(m[5])
        elif line.startswith("[ST]   ") and len(samples) < 10:
            samples.append(line.rstrip())
    for name, (calls, bad, interrupted) in sorted(totals.items()):
        print(f"{name}: {calls} calls, {bad} mismatches, {interrupted} interrupted")
    for s in samples:
        print(s)


if __name__ == "__main__":
    main()
