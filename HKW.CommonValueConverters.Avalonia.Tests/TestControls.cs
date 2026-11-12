using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;

namespace HKW.CommonValueConverters.Avalonia.Tests;

public sealed class TestTarget : Control
{
    public static readonly StyledProperty<object?> ResultProperty = AvaloniaProperty.Register<
        TestTarget,
        object?
    >(nameof(Result));
    public object? Result
    {
        get => GetValue(ResultProperty);
        set => SetValue(ResultProperty, value);
    }
}

public sealed class TestViewModel(params object?[] values) : INotifyPropertyChanged
{
    private readonly object?[] _values = Enumerable
        .Range(0, 5)
        .Select(i => i < values.Length ? values[i] : null)
        .ToArray();
    public object? V0
    {
        get => _values[0];
        set => Set(0, value);
    }
    public object? V1
    {
        get => _values[1];
        set => Set(1, value);
    }
    public object? V2
    {
        get => _values[2];
        set => Set(2, value);
    }
    public object? V3
    {
        get => _values[3];
        set => Set(3, value);
    }
    public object? V4
    {
        get => _values[4];
        set => Set(4, value);
    }
    public event PropertyChangedEventHandler? PropertyChanged;

    public void Set(int index, object? value)
    {
        _values[index] = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs($"V{index}"));
    }
}

public sealed class StringBoolToValueConverter : BoolToValueConverter<string> { }

public sealed class StringEqualsToValueConverter : EqualsToValueConverter<string> { }

public sealed class StringAllBoolToValueMultiConverter : AllBoolToValueMultiConverter<string> { }

public sealed class StringAllEqualsMultiConverter : AllEqualsMultiConverter<string> { }

public sealed class StringAnyEqualsMultiConverter : AnyEqualsMultiConverter<string> { }

public sealed class StringEqualsCountMultiConverter : EqualsCountMultiConverter<string> { }
