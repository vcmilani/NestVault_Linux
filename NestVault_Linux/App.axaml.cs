using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Threading;
using NestVault_Linux.Services;

namespace NestVault_Linux;

public partial class App : Application
{
    public static APIService      Api       { get; private set; } = null!;
    public static ConfigStore     Config    { get; private set; } = null!;
    public static PowerMonitor    Power     { get; private set; } = null!;
    public static ScheduleManager Scheduler { get; private set; } = null!;
    public static BackupRunner    Runner    { get; private set; } = null!;

    public static MainWindow? MainAppWindow { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Api       = new APIService();
        Config    = new ConfigStore();
        Power     = new PowerMonitor();
        Runner    = new BackupRunner(Api);
        Scheduler = new ScheduleManager(Api, Config, Power);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var mainWindow = new MainWindow();
            MainAppWindow  = mainWindow;
            desktop.MainWindow = mainWindow;

            SetupTray();

            mainWindow.Closing += (_, e) =>
            {
                e.Cancel = true;
                mainWindow.Hide();
            };

            _ = Task.Run(async () =>
            {
                await Api.CheckHealthAsync();
                if (Api.IsConnected)
                    await Api.FetchBackupsAsync();
            });

            Scheduler.Start();
            mainWindow.Show();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void SetupTray()
    {
        using var iconStream = AssetLoader.Open(new Uri("avares://NestVault_Linux/Assets/tray.png"));
        var trayIcon = new TrayIcon
        {
            ToolTipText = "NestVault",
            Icon = new WindowIcon(iconStream),
            Menu = new NativeMenu()
        };

        var openItem = new NativeMenuItem("Open NestVault");
        openItem.Click += (_, _) => ShowMainWindow();

        var separator = new NativeMenuItemSeparator();

        var quitItem = new NativeMenuItem("Quit");
        quitItem.Click += (_, _) =>
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.Shutdown();
        };

        trayIcon.Menu!.Add(openItem);
        trayIcon.Menu!.Add(separator);
        trayIcon.Menu!.Add(quitItem);
        trayIcon.Clicked += (_, _) => ShowMainWindow();

        TrayIcon.SetIcons(this, new TrayIcons { trayIcon });
    }

    public static void ShowMainWindow()
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            MainAppWindow?.Show();
            MainAppWindow?.Activate();
        });
    }
}
