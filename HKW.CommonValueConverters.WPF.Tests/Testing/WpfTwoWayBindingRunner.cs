using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace HKW.CommonValueConverters.WPF.Tests;

internal static class WpfTwoWayBindingRunner
{
    public static void Verify<T>(
        IValueConverter converter,
        FrameworkElement target,
        DependencyProperty property,
        T initial,
        object expectedTarget,
        object updatedTarget,
        T expectedSource
    )
    {
        using var errors = new BindingErrors();
        var source = new TypedSource<T>(initial);
        try
        {
            var expression = (BindingExpression)
                BindingOperations.SetBinding(
                    target,
                    property,
                    new Binding("Value")
                    {
                        Source = source,
                        Converter = converter,
                        ConverterCulture = CultureInfo.InvariantCulture,
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.Explicit,
                    }
                );
            PlatformThread.Drain();
            WpfAssert.AssertResult(expectedTarget, target.GetValue(property));
            target.SetCurrentValue(property, updatedTarget);
            expression.UpdateSource();
            PlatformThread.Drain();
            Assert.AreEqual(BindingStatus.Active, expression.Status);
            Assert.IsFalse(expression.HasError);
            if (expectedSource is SolidColorBrush brush)
                WpfAssert.AssertResult(brush.Color, source.Value);
            else
                Assert.AreEqual(expectedSource, source.Value);
            source.Value = initial;
            PlatformThread.Drain();
            WpfAssert.AssertResult(expectedTarget, target.GetValue(property));
            errors.AssertEmpty();
        }
        finally
        {
            BindingOperations.ClearAllBindings(target);
        }
    }
}
