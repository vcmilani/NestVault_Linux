using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using NestVault_Linux.Models;
using NestVault_Linux.ViewModels;

namespace NestVault_Linux.Views;

public partial class BackupConfigsPage : UserControl
{
    private BackupConfigsViewModel? Vm => DataContext as BackupConfigsViewModel;

    public BackupConfigsPage()
    {
        InitializeComponent();
        DataContext = new BackupConfigsViewModel(App.Api, App.Config);
    }

    private async void BrowseFolder_Click(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null) return;

        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Source Folder",
            AllowMultiple = false
        });

        if (folders.Count > 0 && Vm?.EditingProfile is not null)
        {
            var path = folders[0].TryGetLocalPath() ?? folders[0].Path.LocalPath;
            Vm.EditingProfile.SourcePath = path;
            SourcePathBox.Text = path;
            Vm.MarkDirty();
        }
    }

    private async void RunBackup_Click(object? sender, RoutedEventArgs e)
    {
        if (Vm?.EditingProfile is null) return;
        var dialog = new BackupRunnerDialog(Vm.EditingProfile);
        await dialog.ShowDialog(TopLevel.GetTopLevel(this) as Window ?? new Window());
    }

    private async void RunQueue_Click(object? sender, RoutedEventArgs e)
    {
        var dialog = new BackupQueueDialog();
        await dialog.ShowDialog(TopLevel.GetTopLevel(this) as Window ?? new Window());
    }
}
