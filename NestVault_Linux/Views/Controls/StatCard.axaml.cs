using Avalonia;
using Avalonia.Controls;

namespace NestVault_Linux.Views.Controls;

public partial class StatCard : UserControl
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<StatCard, string>(nameof(Title), "");

    public static readonly StyledProperty<string> ValueProperty =
        AvaloniaProperty.Register<StatCard, string>(nameof(Value), "—");

    public static readonly StyledProperty<string> SubtitleProperty =
        AvaloniaProperty.Register<StatCard, string>(nameof(Subtitle), "");

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public string Subtitle
    {
        get => GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public StatCard()
    {
        InitializeComponent();
    }
}
