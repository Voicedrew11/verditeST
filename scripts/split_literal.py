#!/usr/bin/env python3
"""Move a long transcription's straight-line runs into a second file.

    python3 scripts/split_literal.py patches/Draw/ModelDraw.cs [max_lines]

A run is a stretch of top-level statements in `Run` (or a method beside it) with no
label, `goto`, `return`, brace or loop: code that is entered at its first line and
left at its last. The longest runs move, each into a method named after the label
before it (`After8001E0C4`), in `<Class>Runs.cs` as part of the same partial class,
until the first file is under max_lines (default 300). Order of reads and stores is
unchanged, so verify must still read 0; run it after.
"""
import os
import re
import sys

TOP = re.compile(r"^        (?! )")          # a statement at the method body's level
LABEL = re.compile(r"^\s*(L[0-9A-F]{8}): ;")
BREAKS = ("goto ", "return;", "{", "}", "Poll(")
MIN_RUN = int(os.environ.get("MIN_RUN", 12))


def runs(lines):
    """(start, end, label) of each straight-line run, end exclusive."""
    label, start = "Entry", None
    for i, line in enumerate(lines + ["}"]):
        if m := LABEL.match(line):
            label = m.group(1)[1:]
        straight = TOP.match(line) and not LABEL.match(line) and not any(b in line for b in BREAKS)
        if straight and start is None:
            start, run_label = i, label
        elif not straight and start is not None:
            if i - start >= MIN_RUN:
                yield start, i, run_label
            start = None


def siblings(path):
    """The class's Runs files already written, whose method names are taken."""
    base = path.replace(".cs", "Runs")
    return [f for f in (base + ".cs", *(f"{base}{i}.cs" for i in range(2, 6))) if os.path.exists(f)]


def main() -> None:
    if len(sys.argv) < 2:
        sys.exit(__doc__)
    path = sys.argv[1]
    limit = int(sys.argv[2]) if len(sys.argv) > 2 else 300
    cls = re.search(r"public static (?:partial )?class (\w+)", open(path).read()).group(1)
    lines = open(path).read().split("\n")
    moved, used = [], set()
    for start, end, label in sorted(runs(lines), key=lambda r: r[0] - r[1]):
        if len(lines) - sum(e - s - 1 for s, e, _ in moved) <= limit:
            break
        moved.append((start, end, label))
    for start, end, label in sorted(moved, reverse=True):
        name = f"After{label}"
        n = 2
        while name in used or f"void {name}(" in "".join(open(f).read() for f in siblings(path)):
            name, n = f"After{label}_{n}", n + 1
        used.add(name)
        body = lines[start:end]
        lines[start:end] = [f"        {name}(c, mem);"]
        moved[moved.index((start, end, label))] = (name, body)
    text = "\n".join(lines).replace(f"public static class {cls}", f"public static partial class {cls}")
    open(path, "w").write(text)
    methods = []
    for name, body in sorted(moved):
        methods += ["", f"    static void {name}(CpuContext c, PSMemory mem)", "    {", *body, "    }"]
    head = f"""using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary><see cref="{cls}"/>'s straight-line runs, moved out whole so neither file
/// passes the length rule (scripts/split_literal.py). Each is named after the label
/// before it.</summary>
public static partial class {cls}
{{"""
    out, n = path.replace(".cs", "Runs.cs"), 2
    while os.path.exists(out):  # a second pass over the same class writes Runs2.cs
        out, n = path.replace(".cs", f"Runs{n}.cs"), n + 1
    open(out, "w").write(head + "\n".join(methods) + "\n}\n")
    print(f"{path}: {len(lines)} lines; {out}: {len(methods) + 14} lines, {len(moved)} runs")


if __name__ == "__main__":
    main()
