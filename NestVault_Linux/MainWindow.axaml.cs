using Avalonia.Controls;
using NestVault_Linux.Views;

namespace NestVault_Linux;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Navigate("dashboard");
        MainNav.SelectedIndex = 0;
    }

    private void MainNav_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 0) return;
        if (e.AddedItems[0] is ListBoxItem item && item.Tag is string tag)
        {
            FooterNav.SelectedIndex = -1;
            Navigate(tag);
        }
    }

    private void FooterNav_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 0) return;
        if (e.AddedItems[0] is ListBoxItem item && item.Tag is string tag)
        {
            MainNav.SelectedIndex = -1;
            Navigate(tag);
        }
    }

    private void Navigate(string tag)
    {
        ContentFrame.Content = tag switch
        {
            "dashboard"  => new DashboardPage(),
            "backups"    => new BackupsPage(),
            "mybackups"  => new BackupConfigsPage(),
            "cleanup"    => new CleanupPage(),
            "settings"   => new SettingsPage(),
            _            => new DashboardPage()
        };
    }
}
