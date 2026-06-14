#!/usr/bin/env bash
# Publica o NestVault para Linux e gera um pacote instalável.
# Modos:
#   ./scripts/linux-bundle.sh              → empacota x64 + arm64
#   ./scripts/linux-bundle.sh x64  3.0.0  → só x64
#   ./scripts/linux-bundle.sh arm64 3.0.0 → só arm64
#   ./scripts/linux-bundle.sh install      → instala a versão x64 localmente (requer sudo)
#
# O pacote gerado é um tarball com a seguinte estrutura:
#   NestVault-linux-<arch>-v<version>/
#     NestVault          ← executável self-contained
#     NestVault.desktop  ← atalho para o menu do sistema
#     app.png            ← ícone 512×512
#     install.sh         ← script de instalação one-liner

set -euo pipefail

ARCH=${1:-all}
VERSION=${2:-3.0.0}
PROJECT="NestVault_Linux/NestVault_Linux.csproj"
ICON_SRC="NestVault_Linux/Assets/app.png"

# ---------------------------------------------------------------------------
build_bundle() {
    local arch="$1"
    local rid="linux-${arch}"
    local publish_dir="publish/${rid}"
    local pkg_name="NestVault-linux-${arch}-v${VERSION}"
    local pkg_dir="publish/${pkg_name}"
    local archive="${pkg_name}.tar.gz"

    echo "→ Publicando ${rid} v${VERSION}..."
    dotnet publish "$PROJECT" \
      -c Release -r "$rid" --self-contained true \
      -p:PublishSingleFile=true \
      -p:Version="$VERSION" \
      -o "$publish_dir"

    echo "→ Montando pacote (${arch})..."
    rm -rf "$pkg_dir"
    mkdir -p "$pkg_dir"

    cp "${publish_dir}/NestVault" "${pkg_dir}/NestVault"
    chmod +x "${pkg_dir}/NestVault"

    cp "$ICON_SRC" "${pkg_dir}/app.png"

    # Arquivo .desktop (padrão XDG)
    cat > "${pkg_dir}/NestVault.desktop" <<DESKTOP
[Desktop Entry]
Version=1.0
Type=Application
Name=NestVault
Comment=Gerenciador de backups NestVault
Exec=/opt/NestVault/NestVault
Icon=/opt/NestVault/app.png
Terminal=false
Categories=Utility;System;
Keywords=backup;vault;
DESKTOP

    # Script de instalação incluso no pacote
    cat > "${pkg_dir}/install.sh" <<'INSTALL'
#!/usr/bin/env bash
set -euo pipefail
INSTALL_DIR="/opt/NestVault"
DESKTOP_DIR="/usr/share/applications"
BIN_LINK="/usr/local/bin/NestVault"

echo "Instalando NestVault em ${INSTALL_DIR}..."
sudo mkdir -p "$INSTALL_DIR"
sudo cp NestVault "$INSTALL_DIR/NestVault"
sudo chmod +x "$INSTALL_DIR/NestVault"
sudo cp app.png "$INSTALL_DIR/app.png"

echo "Registrando atalho no menu do sistema..."
sudo cp NestVault.desktop "$DESKTOP_DIR/NestVault.desktop"
sudo update-desktop-database "$DESKTOP_DIR" 2>/dev/null || true

echo "Criando link simbólico em ${BIN_LINK}..."
sudo ln -sf "$INSTALL_DIR/NestVault" "$BIN_LINK"

echo "✓ NestVault instalado! Você pode iniciá-lo pelo menu ou digitando 'NestVault' no terminal."
INSTALL
    chmod +x "${pkg_dir}/install.sh"

    echo "→ Gerando tarball..."
    tar -czf "$archive" -C "publish" "$pkg_name"
    rm -rf "$pkg_dir"

    echo "✓ Pronto: ${archive}"
}

# ---------------------------------------------------------------------------
install_local() {
    local rid="linux-x64"
    local publish_dir="publish/${rid}"

    echo "→ Publicando linux-x64 v${VERSION} para instalação local..."
    dotnet publish "$PROJECT" \
      -c Release -r "$rid" --self-contained true \
      -p:PublishSingleFile=true \
      -p:Version="$VERSION" \
      -o "$publish_dir"

    INSTALL_DIR="/opt/NestVault"
    echo "→ Instalando em ${INSTALL_DIR} (requer sudo)..."
    sudo mkdir -p "$INSTALL_DIR"
    sudo cp "${publish_dir}/NestVault" "$INSTALL_DIR/NestVault"
    sudo chmod +x "$INSTALL_DIR/NestVault"
    sudo cp "$ICON_SRC" "$INSTALL_DIR/app.png"

    echo "→ Registrando .desktop..."
    cat | sudo tee /usr/share/applications/NestVault.desktop >/dev/null <<DESKTOP
[Desktop Entry]
Version=1.0
Type=Application
Name=NestVault
Comment=Gerenciador de backups NestVault
Exec=/opt/NestVault/NestVault
Icon=/opt/NestVault/app.png
Terminal=false
Categories=Utility;System;
Keywords=backup;vault;
DESKTOP
    sudo update-desktop-database /usr/share/applications 2>/dev/null || true

    echo "→ Criando link simbólico /usr/local/bin/NestVault..."
    sudo ln -sf "$INSTALL_DIR/NestVault" /usr/local/bin/NestVault

    echo "✓ Instalado! Execute 'NestVault' no terminal ou abra pelo menu."
}

# ---------------------------------------------------------------------------
case "$ARCH" in
    all)
        build_bundle x64
        build_bundle arm64
        ;;
    x64|arm64)
        build_bundle "$ARCH"
        ;;
    install)
        install_local
        ;;
    *)
        echo "Uso: $0 [x64|arm64|all|install] [version]" >&2
        exit 1
        ;;
esac
