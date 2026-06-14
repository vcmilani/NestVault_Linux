#!/usr/bin/env bash
# Creates a macOS .app bundle for NestVault.
# Usage: ./scripts/macos-bundle.sh [arm64|x64|all] [version]
# Examples:
#   ./scripts/macos-bundle.sh             # builds arm64 + x64
#   ./scripts/macos-bundle.sh arm64 3.0.0
#   ./scripts/macos-bundle.sh x64   3.0.0

set -euo pipefail

ARCH=${1:-all}
VERSION=${2:-3.0.0}
PROJECT="NestVault_Linux/NestVault_Linux.csproj"

build_bundle() {
    local arch="$1"
    local rid="osx-${arch}"
    local publish_dir="publish/${rid}"
    local bundle_dir="${publish_dir}/NestVault.app"
    local archive="NestVault-macos-${arch}-v${VERSION}.tar.gz"

    echo "→ Publishing ${rid} v${VERSION}..."
    dotnet publish "$PROJECT" \
      -c Release -r "$rid" --self-contained true \
      -p:PublishSingleFile=true \
      -p:Version="$VERSION" \
      -o "$publish_dir"

    echo "→ Creating .app bundle (${arch})..."
    rm -rf "${bundle_dir}"
    mkdir -p "${bundle_dir}/Contents/MacOS"
    mkdir -p "${bundle_dir}/Contents/Resources"

    cp "${publish_dir}/NestVault" "${bundle_dir}/Contents/MacOS/NestVault"
    chmod +x "${bundle_dir}/Contents/MacOS/NestVault"

    cp "NestVault_Linux/Assets/AppIcon.icns" "${bundle_dir}/Contents/Resources/AppIcon.icns"

    cat > "${bundle_dir}/Contents/Info.plist" <<PLIST
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

    echo "→ Signing ad-hoc (${arch})..."
    codesign --deep --force --sign - "${bundle_dir}"

    echo "→ Packaging (${arch})..."
    tar -czf "$archive" -C "$publish_dir" "NestVault.app"

    echo "✓ Done: ${archive}"
    echo "  Bundle: ${bundle_dir}"
}

case "$ARCH" in
    all)
        build_bundle arm64
        build_bundle x64
        ;;
    arm64|x64)
        build_bundle "$ARCH"
        ;;
    *)
        echo "Usage: $0 [arm64|x64|all] [version]" >&2
        exit 1
        ;;
esac
