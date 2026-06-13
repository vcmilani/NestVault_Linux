using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia.Platform;

namespace NestVault_Linux.Services;

public static class DesktopIntegration
{
    private const string AppId   = "nestvault";
    private const string AppName = "NestVault";

    private static string IconDir =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".local", "share", "icons", "hicolor", "256x256", "apps");

    private static string IconPath => Path.Combine(IconDir, $"{AppId}.png");

    private static string ApplicationsDir =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".local", "share", "applications");

    private static string DesktopFilePath => Path.Combine(ApplicationsDir, $"{AppId}.desktop");

    public static void EnsureInstalled()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return;

        try
        {
            InstallIcon();
            InstallDesktopFile();
            RefreshCaches();
        }
        catch
        {
            // Non-fatal: icon may not appear in taskbar but app still works
        }
    }

    private static void InstallIcon()
    {
        Directory.CreateDirectory(IconDir);

        using var src = AssetLoader.Open(new Uri("avares://NestVault_Linux/Assets/app.png"));
        using var dst = File.Create(IconPath);
        src.CopyTo(dst);
    }

    private static void InstallDesktopFile()
    {
        var exe = Environment.ProcessPath
               ?? System.Reflection.Assembly.GetExecutingAssembly().Location;

        Directory.CreateDirectory(ApplicationsDir);

        var content = $"""
            [Desktop Entry]
            Type=Application
            Name={AppName}
            Exec={exe}
            Icon={AppId}
            Comment=NestVault Backup Manager
            Categories=Utility;System;
            StartupWMClass=NestVault_Linux
            """;

        File.WriteAllText(DesktopFilePath, content);
    }

    private static void RefreshCaches()
    {
        RunIfExists("update-desktop-database", ApplicationsDir);
        RunIfExists("gtk-update-icon-cache", "-f", "-t", Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".local", "share", "icons", "hicolor"));
    }

    private static void RunIfExists(string command, params string[] args)
    {
        try
        {
            using var p = Process.Start(new ProcessStartInfo
            {
                FileName               = command,
                Arguments              = string.Join(' ', args),
                UseShellExecute        = false,
                RedirectStandardOutput = true,
                RedirectStandardError  = true,
            });
            p?.WaitForExit(3000);
        }
        catch { /* command not installed — ignore */ }
    }
}
