Place your app icons here:

  app.png    — Main application icon (PNG, 256x256 or higher recommended)
  tray.png   — System tray icon (PNG, 22x22 or 32x32, transparent background)

Unlike the Windows port (.ico format), Linux uses PNG icons directly.

You can export from the macOS .icns assets:
  sips -s format png NestVault.icns --out app.png -z 256 256
  sips -s format png NestVault.icns --out tray.png -z 32 32

Or use any image editor to create 256x256 and 32x32 PNGs.

The tray icon should work well at 22x22 — keep it simple and recognizable at small sizes.
On GNOME, the tray icon uses libappindicator (apt install libappindicator3-1).
