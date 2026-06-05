namespace NestVault_Linux.Services;

// Stub — no universal taskbar progress API on Linux.
// On GNOME with libunity/libtaskbaritem, progress could be shown via DBus,
// but that's an optional enhancement. For now, in-window ProgressBar is sufficient.
public static class TaskbarProgressHelper
{
    public static void SetProgress(double value) { }
    public static void ClearProgress() { }
    public static void SetIndeterminate() { }
}
