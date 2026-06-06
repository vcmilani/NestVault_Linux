# NestVault — Linux / macOS Client

Cross-platform desktop client for the [NestVault](https://github.com/vcmilani/NestVault) self-hosted backup server, built with Avalonia UI 12 and .NET 10.

---

## Requirements

| Item | Minimum |
|------|---------|
| Linux | Ubuntu 22.04 / Fedora 39 / any modern distro |
| macOS | 12 Monterey or later |
| .NET | 10.0 SDK |
| Desktop (Linux) | X11 or Wayland (XWayland) |
| Server | backup_files v2.6+ |

---

## Project Structure

```
NestVault_Linux/
├── NestVault_Linux.sln
└── NestVault_Linux/
    ├── NestVault_Linux.csproj   # net10.0, Avalonia UI 12.0.4
    ├── App.axaml + .axaml.cs    # TrayIcon (Avalonia built-in), static services
    ├── MainWindow.axaml + .axaml.cs  # Grid nav pane + ContentControl navigation
    ├── Models/
    │   └── Models.cs            # All data models (API contract + BackupSchedule)
    ├── Services/
    │   ├── APIService.cs        # Network layer
    │   ├── ConfigStore.cs       # Local profile persistence (~/.config JSON)
    │   ├── BackupRunner.cs      # Single backup engine
    │   ├── BackupQueue.cs       # Sequential queue engine
    │   ├── ScheduleManager.cs   # Timer-based scheduler
    │   ├── PowerMonitor.cs      # Battery + network (/sys/class/power_supply/)
    │   ├── StartupManager.cs    # Auto-start via XDG autostart .desktop
    │   ├── ToastHelper.cs       # Notifications via notify-send
    │   └── TaskbarProgressHelper.cs  # Stub (no universal Linux/macOS API)
    ├── ViewModels/
    │   ├── DashboardViewModel.cs
    │   ├── BackupsViewModel.cs
    │   ├── BackupConfigsViewModel.cs
    │   ├── CleanupViewModel.cs
    │   ├── SettingsViewModel.cs
    │   ├── BackupRunnerViewModel.cs
    │   └── BackupQueueViewModel.cs
    ├── Views/
    │   ├── Converters.cs
    │   ├── AppResources.axaml
    │   ├── DashboardPage.axaml + .axaml.cs
    │   ├── BackupsPage.axaml + .axaml.cs
    │   ├── BackupConfigsPage.axaml + .axaml.cs
    │   ├── CleanupPage.axaml + .axaml.cs
    │   ├── SettingsPage.axaml + .axaml.cs
    │   ├── BackupRunnerDialog.axaml + .axaml.cs
    │   ├── BackupQueueDialog.axaml + .axaml.cs
    │   └── Controls/
    │       └── StatCard.axaml + .axaml.cs
    ├── Strings/
    │   ├── en-US/Resources.resw
    │   └── pt-BR/Resources.resw
    └── Assets/
        ├── app.png    # 256×256 PNG — window icon
        └── tray.png   # 32×32 PNG — system tray / menu bar icon
```

---

## Setup

### System Dependencies (Ubuntu/Debian)

```bash
# .NET 10 SDK
sudo apt install dotnet-sdk-10.0

# Avalonia runtime dependencies
sudo apt install libx11-dev libice-dev libsm-dev libxext-dev \
                 libxrandr-dev libxcb1-dev libxcb-render0-dev \
                 libxcb-shm0-dev libfontconfig1 libfreetype6

# System tray support on GNOME
sudo apt install libappindicator3-1

# Desktop notifications
sudo apt install libnotify-bin   # provides notify-send
```

### System Dependencies (Fedora)

```bash
sudo dnf install dotnet-sdk-10.0 libX11-devel fontconfig freetype \
                 libappindicator-gtk3 libnotify
```

### System Dependencies (macOS)

```bash
# Install .NET 10 SDK from https://dotnet.microsoft.com/download
# No additional dependencies required — Avalonia runs natively on macOS
```

### Build and Run

```bash
cd NestVault_Linux
dotnet build
dotnet run --project NestVault_Linux/NestVault_Linux.csproj
```

### VS Code

Open the project folder in VS Code. The `.vscode/` directory contains pre-configured `tasks.json` (build / run / publish) and `launch.json` (debug). Recommended extensions are in `.vscode/extensions.json` (C# Dev Kit).

### Publish (self-contained)

Gera um binário único que não depende do .NET instalado na máquina do usuário. Substitua `1.0.0` pela versão desejada.

#### Linux x64

```bash
dotnet publish NestVault_Linux/NestVault_Linux.csproj \
  -c Release -r linux-x64 --self-contained true \
  -p:PublishSingleFile=true -p:Version=1.0.0 \
  -o publish/linux-x64

tar -czf NestVault-linux-x64.tar.gz -C publish/linux-x64 .
```

#### Linux ARM64

```bash
dotnet publish NestVault_Linux/NestVault_Linux.csproj \
  -c Release -r linux-arm64 --self-contained true \
  -p:PublishSingleFile=true -p:Version=1.0.0 \
  -o publish/linux-arm64

tar -czf NestVault-linux-arm64.tar.gz -C publish/linux-arm64 .
```

> Cross-compilation funciona sem configuração extra — é possível gerar o binário ARM64 em uma máquina x64 e vice-versa.

#### macOS ARM64 (Apple Silicon)

```bash
dotnet publish NestVault_Linux/NestVault_Linux.csproj \
  -c Release -r osx-arm64 --self-contained true \
  -p:PublishSingleFile=true -p:Version=1.0.0 \
  -o publish/osx-arm64

# Assinatura ad-hoc (evita quarentena do Gatekeeper)
codesign --deep --force --sign - publish/osx-arm64/NestVault_Linux

tar -czf NestVault-macos-arm64.tar.gz -C publish/osx-arm64 .
```

#### macOS x64 (Intel)

```bash
dotnet publish NestVault_Linux/NestVault_Linux.csproj \
  -c Release -r osx-x64 --self-contained true \
  -p:PublishSingleFile=true -p:Version=1.0.0 \
  -o publish/osx-x64

codesign --deep --force --sign - publish/osx-x64/NestVault_Linux

tar -czf NestVault-macos-x64.tar.gz -C publish/osx-x64 .
```

#### Windows x64

```powershell
dotnet publish NestVault_Linux/NestVault_Linux.csproj `
  -c Release -r win-x64 --self-contained true `
  -p:PublishSingleFile=true -p:Version=1.0.0 `
  -o publish/win-x64

Compress-Archive -Path publish/win-x64/* -DestinationPath NestVault-win-x64.zip
```

#### Windows ARM64

```powershell
dotnet publish NestVault_Linux/NestVault_Linux.csproj `
  -c Release -r win-arm64 --self-contained true `
  -p:PublishSingleFile=true -p:Version=1.0.0 `
  -o publish/win-arm64

Compress-Archive -Path publish/win-arm64/* -DestinationPath NestVault-win-arm64.zip
```

> **Nota macOS:** sem um Developer ID da Apple, o Gatekeeper pode bloquear o binário ao abrir pela primeira vez. A assinatura ad-hoc (`codesign --sign -`) resolve na maioria dos casos. Caso ainda seja bloqueado, o usuário pode rodar `xattr -d com.apple.quarantine NestVault_Linux` no terminal.

#### Automatizado via CI

Ao criar uma tag no formato `vX.X.X`, o GitHub Actions gera e publica os 6 artefatos automaticamente. Veja [`.github/workflows/release.yml`](.github/workflows/release.yml).

---

## Features

### Dashboard
- Cards: total backups, versions, files, and storage
- List of active backups with size and last version date
- Alert banner when the server is unreachable

### Backups (Server Browser)
- 3-panel layout: backups → versions → files
- Filter by backup label and file name
- File table with SHA-256, status, size
- Delete individual version via context menu

### My Backups (Local Profiles)
- Create and manage local backup profiles
- Each profile: name, label, source folder, server override, workers, prefix, excludes, schedule
- Native folder picker (`IStorageProvider.OpenFolderPickerAsync`)
- 4-tab editor: General / Server / Schedule / Exclusions
- Run individual backup (window with live log)
- Run queue with selection UI and per-item progress
- Delete backup from server

### Smart Skip
- Optional per-profile toggle: skip if no local changes detected
- Compares local file tree against hash cache (`mtime` + `size`) before running
- If 0 files changed: calls `POST /absorb` instead of uploading — no network traffic
- 1-week safety override: forces a full backup if the last full run was more than 7 days ago

### Scheduling
- 5 modes: Disabled / Hourly / Daily / Weekly / Custom (minutes)
- Daily and Weekly respect a configured time-of-day
- Checks every 30s; respects network, battery state, and active backup lock

### Cleanup
- Mode: all backups or specific label
- Keep N most recent versions (default: 5)
- Per-label preview before executing
- Mandatory confirmation dialog

### System Tray
- Minimize to tray on close (Avalonia built-in `TrayIcon`)
- Context menu: Open NestVault / Quit
- Desktop notifications via `notify-send` after each backup (Linux)
- Menu bar icon on macOS

### Settings
- **General:** startup toggle (XDG autostart), network status, battery/power source
- **Server:** URL and API Key, test connection
- **Queue Schedule:** schedule settings
- **About:** version

---

## Platform-Specific Implementation

| Feature | Linux | macOS |
|---------|-------|-------|
| Tray icon | Avalonia `TrayIcon` + libappindicator | Avalonia `TrayIcon` (menu bar) |
| Notifications | `notify-send` subprocess | — (stub) |
| Taskbar progress | Not supported (stub) | Not supported (stub) |
| Auto-start | `~/.config/autostart/nestvault.desktop` (XDG) | — (stub) |
| Battery | `/sys/class/power_supply/BAT0/` (sysfs) | `PowerMonitor` (cross-platform) |
| Network | `NetworkInterface.GetAllNetworkInterfaces()` | same |
| Folder picker | `IStorageProvider.OpenFolderPickerAsync()` | same |
| UI dispatch | `Dispatcher.UIThread.InvokeAsync` | same |
| Config storage | `~/.config/NestVault/config.json` | `~/Library/Preferences/NestVault/` |

---

## API Contract (v2.6)

| Method | Endpoint | Usage |
|--------|----------|-------|
| `GET` | `/health` | Check connection |
| `GET` | `/backups` | List backups |
| `GET` | `/backups/{label}/versions` | List versions |
| `GET` | `/files?backup_label=&version_key=` | List files |
| `POST` | `/backups` | Create backup |
| `POST` | `/backups/{label}/versions` | Create version |
| `POST` | `/check/batch` | Check up to 100 files |
| `POST` | `/upload` | Upload file (binary or header-only) |
| `POST` | `/sync` | Mark absent files as deleted |
| `PATCH` | `/backups/{label}/versions/{key}` | Finalize version |
| `POST` | `/backups/{label}/cleanup` | Remove old versions |
| `DELETE` | `/backups/{label}/versions/{key}` | Delete version |
| `DELETE` | `/backups/{label}` | Delete entire backup |

---

## Local Persistence

Profiles and settings are stored as JSON via `ConfigStore` using `Environment.SpecialFolder.ApplicationData`:
- **Linux:** `~/.config/NestVault/config.json`
- **macOS:** `~/Library/Preferences/NestVault/config.json`

---

## Localization

Supported languages: **English** (default) and **Brazilian Portuguese**. The app uses the system language automatically via `.resw` resource files.

---

## Troubleshooting

| Symptom | Cause | Fix |
|---------|-------|-----|
| Window doesn't open (Linux) | Missing X11/Wayland libs | Install `libx11-dev libfontconfig1` |
| Tray icon missing (Linux) | No appindicator support | `sudo apt install libappindicator3-1` |
| No notifications (Linux) | `notify-send` not installed | `sudo apt install libnotify-bin` |
| Battery always shows 0% | Non-standard sysfs path | Check `/sys/class/power_supply/` for your battery name |
| Connection refused | Server offline | `systemctl status backup-server` on the server host |

---

## Server Quick Start

```bash
cd backup_files/server
python3 -m venv .venv && source .venv/bin/activate
pip install -r requirements.txt

export BACKUP_API_KEY="your-key"
export STORAGE_DIR="/mnt/external/backups"

uvicorn main:app --host 0.0.0.0 --port 8000
```

Swagger UI: `http://<server-ip>:8000/docs`
