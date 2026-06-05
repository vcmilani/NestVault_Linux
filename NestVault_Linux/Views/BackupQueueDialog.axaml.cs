using Avalonia.Controls;
using Avalonia.Interactivity;
using NestVault_Linux.Models;
using NestVault_Linux.ViewModels;

namespace NestVault_Linux.Views;

public partial class BackupQueueDialog : Window
{
    private readonly BackupQueueViewModel _vm;

    public BackupQueueDialog()
    {
        InitializeComponent();
        _vm = new BackupQueueViewModel(App.Api, App.Config, App.Scheduler);
        DataContext = _vm;
    }

    private void ProfileList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        foreach (var added in e.AddedItems)
            if (added is BackupProfile p) _vm.ToggleProfile(p, true);
        foreach (var removed in e.RemovedItems)
            if (removed is BackupProfile p) _vm.ToggleProfile(p, false);
    }

    private void Close_Click(object? sender, RoutedEventArgs e) => Close();
}
