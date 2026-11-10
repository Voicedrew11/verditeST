#!/usr/bin/env python3
"""Disassemble a routine of a PS-X EXE extracted from the disc.

    python3 scripts/disasm.py scratch/ST.EXE 800174C4 [count]

Prints from the address to the first `jr ra` and its delay slot, or `count`
instructions. Needs capstone (`pip install capstone`). The executables are extracted
into scratch/ (gitignored) by tools/verdite-core/scripts/extract_file.py. Reading the
MIPS is quicker than reading generated/, whose PGXP bookkeeping triples each line.
"""
import struct
import sys

from capstone import CS_ARCH_MIPS, CS_MODE_32, CS_MODE_LITTLE_ENDIAN, Cs

HEADER = 0x800


def main() -> None:
    if len(sys.argv) < 3:
        sys.exit(__doc__)
    data = open(sys.argv[1], "rb").read()
    text = struct.unpack_from("<I", data, 0x18)[0]
    start = int(sys.argv[2], 16)
    limit = int(sys.argv[3]) if len(sys.argv) > 3 else 4096
    offset = HEADER + start - text
    if not 0 <= offset < len(data):
        sys.exit(f"{start:08X} is outside the text ({text:08X}..)")
    md = Cs(CS_ARCH_MIPS, CS_MODE_32 | CS_MODE_LITTLE_ENDIAN)
    md.skipdata = True  # GTE (COP2) instructions are not decoded; step over them as .byte
    stop_after = None
    for n, ins in enumerate(md.disasm(data[offset:offset + limit * 4], start)):
        print(f"{ins.address:08X}  {ins.mnemonic:8} {ins.op_str}")
        if stop_after is not None:
            break
        if ins.mnemonic == "jr" and ins.op_str == "$ra" and len(sys.argv) <= 3:
            stop_after = n + 1


if __name__ == "__main__":
    main()
