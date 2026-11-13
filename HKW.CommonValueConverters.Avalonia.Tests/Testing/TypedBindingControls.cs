using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using HKW.HKWUtils;

namespace HKW.CommonValueConverters.Avalonia.Tests;

public sealed class TypedSource<T>(T value) : INotifyPropertyChanged
{
    private T _value = value;
    public T Value
    {
        get => _value;
        set
        {
            _value = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;
}

public sealed class TypedTarget : Control
{
    public static readonly StyledProperty<Color> ColorProperty = AvaloniaProperty.Register<
        TypedTarget,
        Color
    >(nameof(Color));
    public static readonly StyledProperty<IEnumInfo?> InfoProperty = AvaloniaProperty.Register<
        TypedTarget,
        IEnumInfo?
    >(nameof(Info));
    public Color Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }
    public IEnumInfo? Info
    {
        get => GetValue(InfoProperty);
        set => SetValue(InfoProperty, value);
    }
}
