# NestVault — Linux Client

Native Linux client for the [NestVault](https://github.com/vcmilani/NestVault) self-hosted backup server, built with Avalonia UI 11 and C# 12 / .NET 8.

---

## Requirements

| Item | Minimum |
|------|---------|
| Linux | Ubuntu 22.04 / Fedora 39 / any modern distro |
| .NET | 8.0 SDK |
| Desktop | X11 or Wayland (XWayland) |
| Server | backup_files v2.6+ |

---

## Project Structure

```
NestVault_Linux/
├── NestVault_Linux.sln
└── NestVault_Linux/
    ├── NestVault_Linux.csproj   # net8.0, Avalonia UI 11.2.3
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
    │   └── TaskbarProgressHelper.cs  # Stub (no universal Linux API)
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
        ├── app.png    # 256×256 PNG
        └── tray.png   # 32×32 PNG
```

---

## Setup

### System Dependencies (Ubuntu/Debian)

```bash
# .NET 8 SDK
sudo apt install dotnet-sdk-8.0

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
sudo dnf install dotnet-sdk-8.0 libX11-devel fontconfig freetype \
                 libappindicator-gtk3 libnotify
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

```bash
dotnet publish NestVault_Linux/NestVault_Linux.csproj \
  -c Release -r linux-x64 --self-contained true \
  -o publish/linux-x64
```

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
- Desktop notifications via `notify-send` after each backup

### Settings
- **General:** startup toggle (XDG autostart), network status, battery/power source
- **Server:** URL and API Key, test connection
- **Queue Schedule:** schedule settings
- **About:** version, links

---

## Platform-Specific Implementation

| Feature | Implementation |
|---------|---------------|
| Tray icon | Avalonia built-in `TrayIcon` |
| Notifications | `notify-send` subprocess |
| Taskbar progress | Not supported (stub) |
| Auto-start | `~/.config/autostart/nestvault.desktop` (XDG autostart spec) |
| Battery | `/sys/class/power_supply/BAT0/capacity` + `/AC/online` (sysfs) |
| Network | `NetworkInterface.GetAllNetworkInterfaces()` |
| Folder picker | `IStorageProvider.OpenFolderPickerAsync()` |
| UI dispatch | `Dispatcher.UIThread.InvokeAsync` |
| Config storage | `~/.config/NestVault/config.json` (`SpecialFolder.ApplicationData`) |

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

Profiles and settings are stored as JSON in `~/.config/NestVault/config.json` via `ConfigStore` (`Environment.SpecialFolder.ApplicationData` resolves to `~/.config` on Linux).

---

## Localization

Supported languages: **English** (default) and **Brazilian Portuguese**. The app uses the system language automatically via `.resw` resource files.

---

## Troubleshooting

| Symptom | Cause | Fix |
|---------|-------|-----|
| Window doesn't open | Missing X11/Wayland libs | Install `libx11-dev libfontconfig1` |
| Tray icon missing | No appindicator support | `sudo apt install libappindicator3-1` |
| No notifications | `notify-send` not installed | `sudo apt install libnotify-bin` |
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
