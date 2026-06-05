using System;
using System.Collections;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using NestVault_Linux.Services;

namespace NestVault_Linux.Views;

// bool → bool (IsVisible)
public sealed class BoolToVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true;
}

// !bool → bool (IsVisible)
public sealed class BoolNegToVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is not true;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is not true;
}

// !bool
public sealed class BoolNegateConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && !b;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && !b;
}

// int/count → bool (IsVisible, >0 = true)
public sealed class CountToVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int i) return i > 0;
        if (value is ICollection c) return c.Count > 0;
        return false;
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => 0;
}

// bool → Green/Red SolidColorBrush (for status dot)
public sealed class BoolToGreenRedColorConverter : IValueConverter
{
    private static readonly SolidColorBrush Green = new(Color.FromArgb(255, 52, 199, 89));
    private static readonly SolidColorBrush Red   = new(Color.FromArgb(255, 255, 69, 58));

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? Green : Red;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => false;
}

// bool → red/white SolidColorBrush (for error counts)
public sealed class BoolToRedBrushConverter : IValueConverter
{
    private static readonly SolidColorBrush Red   = new(Color.FromArgb(255, 255, 69, 58));
    private static readonly SolidColorBrush White = new(Color.FromArgb(255, 220, 220, 220));

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? Red : White;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => false;
}

// LogKind → SolidColorBrush
public sealed class LogKindToColorBrushConverter : IValueConverter
{
    private static readonly SolidColorBrush Info    = new(Color.FromArgb(255, 200, 200, 200));
    private static readonly SolidColorBrush Success = new(Color.FromArgb(255, 52, 199, 89));
    private static readonly SolidColorBrush Warning = new(Color.FromArgb(255, 255, 159, 10));
    private static readonly SolidColorBrush Error   = new(Color.FromArgb(255, 255, 69, 58));

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is BackupRunner.LogKind kind
            ? kind switch
            {
                BackupRunner.LogKind.Success => Success,
                BackupRunner.LogKind.Warning => Warning,
                BackupRunner.LogKind.Error   => Error,
                _                             => Info
            }
            : Info;
    public object? ConvertBack(object? v, Type _, object? __, CultureInfo ___) => v;
}
