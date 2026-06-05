using Avalonia.Controls;
using NestVault_Linux.ViewModels;

namespace NestVault_Linux.Views;

public partial class DashboardPage : UserControl
{
    public DashboardPage()
    {
        InitializeComponent();
        DataContext = new DashboardViewModel(App.Api);
    }
}
