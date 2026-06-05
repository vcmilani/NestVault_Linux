using System;
using System.IO;

namespace NestVault_Linux.Services;

public static class StartupManager
{
    private const string AppName = "NestVault";

    // XDG autostart: ~/.config/autostart/nestvault.desktop
    private static string AutostartDir =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "autostart");

    private static string DesktopFilePath =>
        Path.Combine(AutostartDir, "nestvault.desktop");

    public static bool IsEnabled => File.Exists(DesktopFilePath);

    public static void SetEnabled(bool enable)
    {
        if (enable)
        {
            var exe = Environment.ProcessPath
                   ?? System.Reflection.Assembly.GetExecutingAssembly().Location;

            Directory.CreateDirectory(AutostartDir);

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

            File.WriteAllText(DesktopFilePath, content);
        }
        else
        {
            try { File.Delete(DesktopFilePath); }
            catch { /* ignore if not present */ }
        }
    }
}
