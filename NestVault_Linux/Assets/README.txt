Place your app icons here:

  app.png      — Main application icon (PNG, 256x256 or higher recommended)
  tray.png     — System tray / menu bar icon (PNG, 22x22 or 32x32, transparent background)
  AppIcon.icns — macOS app bundle icon (generated from app.png — see below)
  AppIcon.ico  — Windows executable/taskbar icon (multi-resolution — see below)

Linux uses app.png at runtime (Avalonia WindowIcon).
macOS uses app.png at runtime + AppIcon.icns for the .app bundle (Dock + Finder).
Windows uses AppIcon.ico at runtime and embedded in the executable (Explorer + taskbar).

To regenerate AppIcon.icns from app.png (run on macOS):
  mkdir /tmp/AppIcon.iconset
  sips -z 16   16   app.png --out /tmp/AppIcon.iconset/icon_16x16.png
  sips -z 32   32   app.png --out /tmp/AppIcon.iconset/icon_16x16@2x.png
  sips -z 32   32   app.png --out /tmp/AppIcon.iconset/icon_32x32.png
  sips -z 64   64   app.png --out /tmp/AppIcon.iconset/icon_32x32@2x.png
  sips -z 128  128  app.png --out /tmp/AppIcon.iconset/icon_128x128.png
  sips -z 256  256  app.png --out /tmp/AppIcon.iconset/icon_128x128@2x.png
  sips -z 256  256  app.png --out /tmp/AppIcon.iconset/icon_256x256.png
  iconutil -c icns /tmp/AppIcon.iconset -o AppIcon.icns

To regenerate AppIcon.ico from app.png (requires Python + Pillow):
  python3 scripts/gen-ico.py

To bundle for macOS distribution, use: scripts/macos-bundle.sh [arm64|x64] [version]
To bundle for Linux distribution, use:  scripts/linux-bundle.sh [x64|arm64|all] [version]

The tray icon should work well at 22x22 — keep it simple and recognizable at small sizes.
On GNOME, the tray icon uses libappindicator (apt install libappindicator3-1).
