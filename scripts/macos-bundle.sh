#!/usr/bin/env bash
# Creates a macOS .app bundle for NestVault.
# Usage: ./scripts/macos-bundle.sh [arm64|x64] [version]
# Example: ./scripts/macos-bundle.sh arm64 3.0.0

set -euo pipefail

ARCH=${1:-arm64}
VERSION=${2:-3.0.0}
RID="osx-${ARCH}"
PROJECT="NestVault_Linux/NestVault_Linux.csproj"
PUBLISH_DIR="publish/${RID}"
BUNDLE_NAME="NestVault.app"
BUNDLE_DIR="${PUBLISH_DIR}/${BUNDLE_NAME}"

echo "→ Publishing ${RID} v${VERSION}..."
dotnet publish "$PROJECT" \
  -c Release -r "$RID" --self-contained true \
  -p:PublishSingleFile=true \
  -p:Version="$VERSION" \
  -o "$PUBLISH_DIR"

echo "→ Creating .app bundle..."
rm -rf "${BUNDLE_DIR}"
mkdir -p "${BUNDLE_DIR}/Contents/MacOS"
mkdir -p "${BUNDLE_DIR}/Contents/Resources"

# Binary
cp "${PUBLISH_DIR}/NestVault_Linux" "${BUNDLE_DIR}/Contents/MacOS/NestVault"
chmod +x "${BUNDLE_DIR}/Contents/MacOS/NestVault"

# Icon
cp "NestVault_Linux/Assets/AppIcon.icns" "${BUNDLE_DIR}/Contents/Resources/AppIcon.icns"

# Info.plist
cat > "${BUNDLE_DIR}/Contents/Info.plist" <<PLIST
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleIdentifier</key>    <string>com.vcmilani.nestvault</string>
    <key>CFBundleName</key>         <string>NestVault</string>
    <key>CFBundleDisplayName</key>  <string>NestVault</string>
    <key>CFBundleVersion</key>      <string>${VERSION}</string>
    <key>CFBundleShortVersionString</key> <string>${VERSION}</string>
    <key>CFBundleIconFile</key>     <string>AppIcon</string>
    <key>CFBundlePackageType</key>  <string>APPL</string>
    <key>CFBundleExecutable</key>   <string>NestVault</string>
    <key>NSHighResolutionCapable</key> <true/>
    <key>LSMinimumSystemVersion</key>  <string>12.0</string>
    <key>NSPrincipalClass</key>     <string>NSApplication</string>
</dict>
</plist>
PLIST

echo "→ Signing ad-hoc..."
codesign --deep --force --sign - "${BUNDLE_DIR}"

echo "→ Packaging..."
ARCHIVE="NestVault-macos-${ARCH}-v${VERSION}.tar.gz"
tar -czf "$ARCHIVE" -C "$PUBLISH_DIR" "$BUNDLE_NAME"

echo "✓ Done: ${ARCHIVE}"
echo "  Bundle: ${BUNDLE_DIR}"
