using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using NestVault_Linux.ViewModels;

namespace NestVault_Linux.Views;

public partial class CleanupPage : UserControl
{
    private CleanupViewModel? Vm => DataContext as CleanupViewModel;

    public CleanupPage()
    {
        InitializeComponent();
        DataContext = new CleanupViewModel(App.Api);
    }

    private void TargetAll_Click(object? sender, RoutedEventArgs e)
    {
        if (Vm is not null) Vm.Target = CleanupViewModel.CleanupTarget.All;
    }

    private void TargetSpecific_Click(object? sender, RoutedEventArgs e)
    {
        if (Vm is not null) Vm.Target = CleanupViewModel.CleanupTarget.Specific;
    }

    private async void RunCleanup_Click(object? sender, RoutedEventArgs e)
    {
        var dialog = new Window
        {
            Title  = "Confirm Cleanup",
            Width  = 360,
            Height = 160,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new StackPanel
            {
                Margin  = new Avalonia.Thickness(20),
                Spacing = 16,
                Children =
                {
                    new TextBlock { Text = $"Remove old versions keeping the last {Vm?.KeepCount}?\nThis cannot be undone.", TextWrapping = Avalonia.Media.TextWrapping.Wrap },
                    new StackPanel
                    {
                        Orientation = Avalonia.Layout.Orientation.Horizontal,
                        Spacing     = 8,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                        Children =
                        {
                            new Button { Content = "Cancel" },
                            new Button { Content = "Run Cleanup" }
                        }
                    }
                }
            }
        };

        var cancelBtn  = ((StackPanel)((StackPanel)dialog.Content!).Children[1]).Children[0] as Button;
        var confirmBtn = ((StackPanel)((StackPanel)dialog.Content!).Children[1]).Children[1] as Button;

        bool confirmed = false;
        if (cancelBtn  is not null) cancelBtn.Click  += (_, _) => dialog.Close();
        if (confirmBtn is not null) confirmBtn.Click += (_, _) => { confirmed = true; dialog.Close(); };

        await dialog.ShowDialog(TopLevel.GetTopLevel(this) as Window ?? new Window());

        if (confirmed && Vm is not null)
            await Vm.RunCleanupCommand.ExecuteAsync(null);
    }
}
