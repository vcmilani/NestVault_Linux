using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace NestVault_Linux.Services;

public partial class PowerMonitor : ObservableObject, IDisposable
{
    [ObservableProperty] private bool   _isOnAC = true;
    [ObservableProperty] private int    _batteryPercent = 100;
    [ObservableProperty] private bool   _isNetworkAvailable;
    [ObservableProperty] private string _networkType = "Unknown";

    private Timer? _pollTimer;

    public PowerMonitor()
    {
        UpdateBattery();
        UpdateNetwork();
        NetworkChange.NetworkAvailabilityChanged += (_, _) => UpdateNetwork();
        // Poll battery every 60s (Linux has no native battery event like Windows)
        _pollTimer = new Timer(_ => UpdateBattery(), null, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60));
    }

    // MARK: - Battery

    private void UpdateBattery()
    {
        try
        {
            // Try standard sysfs paths
            var acPath      = "/sys/class/power_supply/AC/online";
            var acAltPath   = "/sys/class/power_supply/AC0/online";
            var batPath     = "/sys/class/power_supply/BAT0/capacity";
            var batAltPath  = "/sys/class/power_supply/BAT1/capacity";
            var batStatus   = "/sys/class/power_supply/BAT0/status";

            // Read AC status
            var acFile = File.Exists(acPath) ? acPath : File.Exists(acAltPath) ? acAltPath : null;
            if (acFile is not null)
            {
                var acOnline = File.ReadAllText(acFile).Trim();
                IsOnAC = acOnline == "1";
            }
            else
            {
                // No AC file — check battery status string
                if (File.Exists(batStatus))
                {
                    var status = File.ReadAllText(batStatus).Trim();
                    IsOnAC = status is "Full" or "Charging";
                }
                else
                {
                    IsOnAC = true; // assume AC if no battery info
                }
            }

            // Read battery percent
            var batFile = File.Exists(batPath) ? batPath : File.Exists(batAltPath) ? batAltPath : null;
            if (batFile is not null && int.TryParse(File.ReadAllText(batFile).Trim(), out var pct))
                BatteryPercent = Math.Clamp(pct, 0, 100);
            else
                BatteryPercent = 100; // no battery found — desktop
        }
        catch
        {
            IsOnAC         = true;
            BatteryPercent = 100;
        }
    }

    // MARK: - Network

    private void UpdateNetwork()
    {
        try
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var iface in interfaces)
            {
                if (iface.OperationalStatus != OperationalStatus.Up) continue;
                if (iface.NetworkInterfaceType == NetworkInterfaceType.Loopback) continue;

                IsNetworkAvailable = true;
                NetworkType = iface.NetworkInterfaceType switch
                {
                    NetworkInterfaceType.Wireless80211 => "Wi-Fi",
                    NetworkInterfaceType.Ethernet      => "Ethernet",
                    _                                  => "Connected"
                };
                return;
            }
            IsNetworkAvailable = false;
            NetworkType        = "No Network";
        }
        catch
        {
            IsNetworkAvailable = false;
            NetworkType        = "Unknown";
        }
    }

    public void Dispose()
    {
        _pollTimer?.Dispose();
        GC.SuppressFinalize(this);
    }
}
