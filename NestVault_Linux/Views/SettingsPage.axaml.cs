using Avalonia.Controls;
using NestVault_Linux.ViewModels;

namespace NestVault_Linux.Views;

public partial class SettingsPage : UserControl
{
    public SettingsPage()
    {
        InitializeComponent();
        DataContext = new SettingsViewModel(App.Api, App.Power, App.Scheduler);
    }
}
