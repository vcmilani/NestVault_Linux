#!/usr/bin/env python3
"""Gera AppIcon.ico a partir de app.png com múltiplas resoluções para Windows."""

import struct, io, os, sys

try:
    from PIL import Image
except ImportError:
    sys.exit("Pillow não encontrado. Instale com: pip3 install Pillow")

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SRC  = os.path.join(ROOT, "NestVault_Linux", "Assets", "app.png")
DST  = os.path.join(ROOT, "NestVault_Linux", "Assets", "AppIcon.ico")

SIZES = [16, 32, 48, 64, 128, 256]

img = Image.open(SRC).convert("RGBA")

frames = []
for s in SIZES:
    buf = io.BytesIO()
    img.resize((s, s), Image.LANCZOS).save(buf, format="PNG")
    frames.append(buf.getvalue())

count = len(frames)
header = struct.pack("<HHH", 0, 1, count)

entry_size = 16
data_offset = 6 + count * entry_size

entries = b""
data    = b""
offset  = data_offset
for s, png in zip(SIZES, frames):
    w = 0 if s == 256 else s  # 0 = 256 no formato ICO
    entries += struct.pack("<BBBBHHII", w, w, 0, 0, 1, 32, len(png), offset)
    data   += png
    offset += len(png)

with open(DST, "wb") as f:
    f.write(header + entries + data)

print(f"Gerado: {DST} ({os.path.getsize(DST) / 1024:.1f} KB, tamanhos: {SIZES})")
