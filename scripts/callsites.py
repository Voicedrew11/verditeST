#!/usr/bin/env python3
"""List the calls a recompiled routine makes, with the return address each leaves in RA.

    python3 scripts/callsites.py st 80026AFC [80020A80 ...]

Reads generated/<overlay>.cs (run the recompiler first). For each routine, prints one
line per direct call: the return address, the callee, and the constant argument
registers set since the previous call, which is enough to tell apart two calls to the
same callee. Hooks that key on a call site (StageProbe, GeometryProbe) take their
return addresses from here.
"""
import re
import sys

CALL = re.compile(r"c\.RA = 0x([0-9A-F]{8})u;.*")
CALLEE = re.compile(r"\b(?:\w+\.)?(func_[0-9A-F]{8})\(c, m\)")
CONST_ARG = re.compile(r"c\.(A[0-3]) = (0x[0-9A-F]+u|0u);")


def body(src: str, name: str) -> list[str]:
    start = src.find(f"void {name}(CpuContext c, IMemory m)")
    if start < 0:
        sys.exit(f"{name}: not in the generated file")
    end = src.find("\n    }\n", start)
    return src[start:end].splitlines()


def calls(lines: list[str]):
    ret, args = None, {}
    for line in lines:
        for reg, val in CONST_ARG.findall(line):
            args[reg] = int(val.rstrip("u"), 16)
        if m := CALL.search(line):
            ret = m.group(1)
            continue
        if ret and (m := CALLEE.search(line)):
            yield ret, m.group(1), dict(args)
            ret, args = None, {}


def main() -> None:
    if len(sys.argv) < 3:
        sys.exit(__doc__)
    src = open(f"generated/{sys.argv[1]}.cs").read()
    for addr in sys.argv[2:]:
        name = f"func_{addr.upper().removeprefix('0X')}"
        print(name)
        for ret, callee, args in calls(body(src, name)):
            shown = " ".join(f"{r}={v:X}" for r, v in sorted(args.items()))
            print(f"  ra {ret}  {callee}  {shown}")


if __name__ == "__main__":
    main()
