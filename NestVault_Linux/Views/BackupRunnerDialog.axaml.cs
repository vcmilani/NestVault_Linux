using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using NestVault_Linux.Models;
using NestVault_Linux.ViewModels;

namespace NestVault_Linux.Views;

public partial class BackupRunnerDialog : Window
{
    private readonly BackupRunnerViewModel _vm;

    public BackupRunnerDialog(BackupProfile profile)
    {
        InitializeComponent();
        _vm = new BackupRunnerViewModel(App.Api, App.Config, App.Scheduler);
        DataContext = _vm;

        Opened += async (_, _) =>
        {
            await _vm.StartAsync(profile);
        };

        _vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(BackupRunnerViewModel.Entries))
                LogScroller.ScrollToEnd();
        };
    }

    private void Close_Click(object? sender, RoutedEventArgs e) => Close();
}
