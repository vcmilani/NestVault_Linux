using System;
using System.Diagnostics;

namespace NestVault_Linux.Services;

public static class ToastHelper
{
    public static void Show(string title, string body)
    {
        try
        {
            // Use notify-send (available on GNOME, KDE, most Linux desktops)
            var safeTitle = title.Replace("\"", "'");
            var safeBody  = body.Replace("\"", "'");
            var psi = new ProcessStartInfo
            {
                FileName  = "notify-send",
                Arguments = $"--app-name=\"NestVault\" --icon=drive-harddisk \"{safeTitle}\" \"{safeBody}\"",
                UseShellExecute       = false,
                RedirectStandardError = true,
                CreateNoWindow        = true
            };
            using var proc = Process.Start(psi);
            proc?.WaitForExit(2000);
        }
        catch { /* notify-send may not be installed — silent fallback */ }
    }
}
