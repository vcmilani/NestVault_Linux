Place your app icons here:

  app.png      — Main application icon (PNG, 256x256 or higher recommended)
  tray.png     — System tray / menu bar icon (PNG, 22x22 or 32x32, transparent background)
  AppIcon.icns — macOS app bundle icon (generated from app.png — see below)

Linux and macOS use PNG icons at runtime (Avalonia WindowIcon).
The .icns file is required for the macOS .app bundle (Dock + Finder).

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

To bundle for macOS distribution, use: scripts/macos-bundle.sh [arm64|x64] [version]

The tray icon should work well at 22x22 — keep it simple and recognizable at small sizes.
On GNOME, the tray icon uses libappindicator (apt install libappindicator3-1).
