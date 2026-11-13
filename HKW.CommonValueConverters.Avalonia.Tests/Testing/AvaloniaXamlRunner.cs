using Avalonia.Markup.Xaml;

namespace HKW.CommonValueConverters.Avalonia.Tests;

internal static class AvaloniaXamlRunner
{
    public static void Run(
        ConverterCase c,
        Action<ConverterBase, ConverterBase> compareConfiguration
    )
    {
        using var errors = new BindingErrorScope();
        var target = (TestTarget)
            AvaloniaXamlLoader.Load(
                new Uri($"avares://HKW.CommonValueConverters.Avalonia.Tests/TestViews/{c.Id}.axaml")
            );
        try
        {
            Assert.HasCount(1, target.Resources);
            var converter = (ConverterBase)target.Resources[c.Id]!;
            var actualType = converter.GetType();
            Assert.AreEqual(c.Type, c.Type.IsGenericType ? actualType.BaseType : actualType);
            Assert.IsTrue(c.Type.IsInstanceOfType(converter));
            var expectedConfiguration = c.Create();
            compareConfiguration(expectedConfiguration, converter);
            var vm = new TestViewModel(c.Initial);
            target.DataContext = vm;
            ConverterAssertions.VerifyUpdates(c, target, vm);
        }
        finally
        {
            target.ClearValue(TestTarget.ResultProperty);
            target.DataContext = null;
            target.Resources.Clear();
        }
    }
}
