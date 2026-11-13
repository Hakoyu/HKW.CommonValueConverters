using Avalonia.Data;
using Avalonia.Data.Converters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

internal static class AvaloniaBindingRunner
{
    public static void Run(ConverterCase c)
    {
        using var errors = new BindingErrorScope();
        var vm = new TestViewModel(c.Initial);
        var converter = c.Create();
        Assert.AreEqual(c.Type, converter.GetType());
        var target = new TestTarget { DataContext = vm };
        BindingBase binding;
        if (c.Multi)
        {
            var multi = new MultiBinding
            {
                Converter = (IMultiValueConverter)converter,
                ConverterParameter = c.Parameter,
            };
            for (var i = 0; i < c.Initial.Length; i++)
                multi.Bindings.Add(new Binding($"V{i}"));
            binding = multi;
        }
        else
            binding = new Binding("V0")
            {
                Converter = (IValueConverter)converter,
                ConverterParameter = c.Parameter,
                Mode = BindingMode.OneWay,
            };
        using var subscription = target.Bind(TestTarget.ResultProperty, binding);
        try
        {
            ConverterAssertions.VerifyUpdates(c, target, vm);
        }
        finally
        {
            target.ClearValue(TestTarget.ResultProperty);
            target.DataContext = null;
        }
    }
}
