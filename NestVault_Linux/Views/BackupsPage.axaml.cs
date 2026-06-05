using Avalonia.Controls;
using NestVault_Linux.ViewModels;

namespace NestVault_Linux.Views;

public partial class BackupsPage : UserControl
{
    public BackupsPage()
    {
        InitializeComponent();
        DataContext = new BackupsViewModel(App.Api);
    }
}
