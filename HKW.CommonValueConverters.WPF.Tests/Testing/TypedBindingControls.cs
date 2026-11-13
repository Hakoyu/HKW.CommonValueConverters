using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using HKW.HKWUtils;

namespace HKW.CommonValueConverters.WPF.Tests;

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

public sealed class ColorTarget : FrameworkElement
{
    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
        nameof(Color),
        typeof(Color),
        typeof(ColorTarget)
    );
    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }
}

public sealed class EnumInfoTarget : FrameworkElement
{
    public static readonly DependencyProperty InfoProperty = DependencyProperty.Register(
        nameof(Info),
        typeof(IEnumInfo),
        typeof(EnumInfoTarget)
    );
    public IEnumInfo? Info
    {
        get => (IEnumInfo?)GetValue(InfoProperty);
        set => SetValue(InfoProperty, value);
    }
}
