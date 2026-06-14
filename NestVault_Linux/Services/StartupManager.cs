using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Win32;

namespace NestVault_Linux.Services;

public static class StartupManager
{
    private const string AppName = "NestVault";

    public static bool IsEnabled =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? IsEnabledWindows()
            : File.Exists(LinuxDesktopFilePath);

    public static void SetEnabled(bool enable)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            SetEnabledWindows(enable);
        else
            SetEnabledLinux(enable);
    }

    // ── Windows ──────────────────────────────────────────────────────────────

    private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";

    [SupportedOSPlatform("windows")]
    private static bool IsEnabledWindows()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKey, writable: false);
        return key?.GetValue(AppName) is not null;
    }

    [SupportedOSPlatform("windows")]
    private static void SetEnabledWindows(bool enable)
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKey, writable: true);
        if (key is null) return;

        if (enable)
        {
            var exe = Environment.ProcessPath
                   ?? System.Reflection.Assembly.GetExecutingAssembly().Location;
            key.SetValue(AppName, $"\"{exe}\"");
        }
        else
        {
            key.DeleteValue(AppName, throwOnMissingValue: false);
        }
    }

    // ── Linux (XDG autostart) ─────────────────────────────────────────────

    private static string LinuxAutostartDir =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "autostart");

    private static string LinuxDesktopFilePath =>
        Path.Combine(LinuxAutostartDir, "nestvault.desktop");

    private static void SetEnabledLinux(bool enable)
    {
        if (enable)
        {
            var exe = Environment.ProcessPath
                   ?? System.Reflection.Assembly.GetExecutingAssembly().Location;

            Directory.CreateDirectory(LinuxAutostartDir);

            var content = $"""
                [Desktop Entry]
                Type=Application
                Name={AppName}
                Exec={exe}
                Icon=nestvault
                Comment=NestVault Backup Manager
                X-GNOME-Autostart-enabled=true
                Hidden=false
                NoDisplay=false
                """;

            File.WriteAllText(LinuxDesktopFilePath, content);
        }
        else
        {
            try { File.Delete(LinuxDesktopFilePath); }
            catch { /* ignore if not present */ }
        }
    }
}
