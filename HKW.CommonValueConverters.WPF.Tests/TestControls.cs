using System.ComponentModel;
using System.Windows;

namespace HKW.CommonValueConverters.WPF.Tests;

public sealed class TestTarget : FrameworkElement
{
    public static readonly DependencyProperty ResultProperty = DependencyProperty.Register(
        nameof(Result),
        typeof(object),
        typeof(TestTarget),
        new PropertyMetadata(null)
    );
    public object? Result
    {
        get => GetValue(ResultProperty);
        set => SetValue(ResultProperty, value);
    }
}

public sealed class TestViewModel : INotifyPropertyChanged
{
    private readonly object?[] _values = new object?[5];
    public event PropertyChangedEventHandler? PropertyChanged;
    public object? A
    {
        get => _values[0];
        set => Set(0, value);
    }
    public object? B
    {
        get => _values[1];
        set => Set(1, value);
    }
    public object? C
    {
        get => _values[2];
        set => Set(2, value);
    }
    public object? D
    {
        get => _values[3];
        set => Set(3, value);
    }
    public object? E
    {
        get => _values[4];
        set => Set(4, value);
    }

    public void Set(int index, object? value)
    {
        _values[index] = value;
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(((char)('A' + index)).ToString())
        );
    }

    public void Apply(object?[] values)
    {
        for (var index = 0; index < values.Length; index++)
        {
            if (Equals(_values[index], values[index]) is false)
                Set(index, values[index]);
        }
    }
}

public sealed class StringBoolConverter : BoolToValueConverter<string> { }

public sealed class StringEqualsConverter : EqualsToValueConverter<string> { }

public sealed class StringAllBoolConverter : AllBoolToValueMultiConverter<string> { }

public sealed class StringAllEqualsConverter : AllEqualsMultiConverter<string> { }

public sealed class StringAnyEqualsConverter : AnyEqualsMultiConverter<string> { }

public sealed class StringEqualsCountConverter : EqualsCountMultiConverter<string> { }
