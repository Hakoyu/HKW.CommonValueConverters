using System.ComponentModel;
using System.Globalization;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace HKW.CommonValueConverters.Avalonia.Tests;

internal static partial class AvaloniaTypedBindingRunner
{
    public static void VerifyTwoWay<TSource, TTarget>(
        TSource initial,
        TSource expectedSource,
        AvaloniaObject target,
        StyledProperty<TTarget> property,
        IValueConverter converter,
        object? expectedInitial,
        TTarget edit
    )
    {
        using var errors = new BindingErrorScope();
        var source = new TypedSource<TSource>(initial);
        using var subscription = target.Bind(
            property,
            new Binding(nameof(source.Value))
            {
                Source = source,
                Converter = converter,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                ConverterCulture = CultureInfo.InvariantCulture,
            }
        );
        PlatformThread.Drain();
        ConverterAssertions.Equal(expectedInitial, target.GetValue(property));
        target.SetCurrentValue(property, edit);
        PlatformThread.Drain();
        if (expectedSource is ISolidColorBrush brush)
            ConverterAssertions.Equal(brush.Color, source.Value);
        else
            Assert.AreEqual(expectedSource, source.Value);
    }
}

internal static partial class AvaloniaTypedBindingRunner
{
    public static void VerifyBoolean<T>(
        AvaloniaObject target,
        StyledProperty<T> property,
        IValueConverter converter,
        object expectedTrue,
        object expectedFalse,
        object? parameter = null
    )
    {
        using var errors = new BindingErrorScope();
        var source = new TypedSource<bool>(true);
        using var subscription = target.Bind(
            property,
            new Binding("Value")
            {
                Source = source,
                Converter = converter,
                ConverterParameter = parameter,
                Mode = BindingMode.OneWay,
            }
        );
        ConverterAssertions.Equal(expectedTrue, target.GetValue(property));
        source.Value = false;
        PlatformThread.Drain();
        ConverterAssertions.Equal(expectedFalse, target.GetValue(property));
    }
}
