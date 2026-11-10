#!/usr/bin/env python3
"""Write a recompiled routine out as a literal C# transcription for patches/Draw/.

    python3 scripts/transcribe.py st 80026964 HudPanels "HUD panels" HUDPANELS "what it does"

Reads generated/<overlay>.cs and writes patches/Draw/<Class>.cs: the same reads and
stores in the same order, with the recompiler's PGXP bookkeeping and temporaries
taken out, callees called directly (so their hooks still fire), and a Literal that
registers it under ST_<ENV>. Then the class is tidied by hand: comments, loop shapes,
names. Nothing it writes is trusted until ST_<ENV>=verify reads 0 mismatches
("The draw path in C#" in docs/GEOMETRY.md).
"""
import re
import sys

PGXP_TAIL = re.compile(r" if \(RecompOne\.Runtime\.Pgxp\.PgxpGate\.Cpu && RecompOne\.Runtime\.Pgxp\.Pgxp\.CpuTracking\) [^;]*;")
BLOCK = re.compile(r"^(\s*)\{ (.*) \}$")
TEMP = re.compile(r"var (_\w+) = ([^;]*);\s*")


def body(src: str, name: str) -> list[str]:
    start = src.find(f"void {name}(CpuContext c, IMemory m)")
    if start < 0:
        sys.exit(f"{name}: not in the generated file")
    end = src.find("\n    }\n", start)
    return src[start:end].splitlines()[2:]  # past the signature and the opening brace


def inline_temps(line: str) -> str:
    """`{ var _a = (X); stmt(_a); }` -> `stmt((X));`. Every temporary is read before
    the statement's single assignment, so substituting it keeps the order of reads."""
    m = BLOCK.match(line)
    if not m:
        return line
    indent, inner = m.groups()
    temps = {}
    while t := TEMP.match(inner):
        temps[t.group(1)] = t.group(2)
        inner = inner[t.end():]
    for name, expr in temps.items():
        plain = re.fullmatch(r"[\w.]+", expr) or re.fullmatch(r"\([^()]*\)", expr)
        inner = re.sub(rf"\b{name}\b", expr if plain else f"({expr})", inner)
    return indent + inner.strip()


def clean(line: str) -> str | None:
    line = PGXP_TAIL.sub("", line)
    if "var mem = (PSMemory)m;" in line:
        return None
    line = inline_temps(line)
    line = line.replace("RecompOne.Runtime.Interrupts.Poll(c, m)", "Poll(c, mem)")
    line = re.sub(r"\(c, m\)", "(c, mem)", line)
    line = line.replace("Dispatcher.Call(c, m,", "Dispatcher.Call(c, mem,")
    return line


def main() -> None:
    if len(sys.argv) < 7:
        sys.exit(__doc__)
    overlay, addr, cls, label, env, tip = sys.argv[1:7]
    addr = addr.upper().removeprefix("0X")
    src = open(f"generated/{overlay}.cs").read()
    lines = [l for l in (clean(l) for l in body(src, f"func_{addr}")) if l is not None]
    out = f'''using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_{addr}</c>: {tip} <c>ST_{env}</c>.
/// </summary>
public static class {cls}
{{
    public static readonly Literal R = new(typeof({cls}), "{label}", "{env}", 0x{addr}, Run,
        "{tip}");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {{
''' + "\n".join(lines) + "\n    }\n}\n"
    path = f"patches/Draw/{cls}.cs"
    open(path, "w").write(out)
    print(f"{path}: {len(out.splitlines())} lines")


if __name__ == "__main__":
    main()
