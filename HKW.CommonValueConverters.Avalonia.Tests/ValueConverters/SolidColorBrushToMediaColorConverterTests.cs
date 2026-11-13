using Avalonia;
using Avalonia.Data;
using Avalonia.Media;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class SolidColorBrushToMediaColorConverterTests
{
    [TestMethod]
    public Task SolidColorBrushToMediaColorConverter_TwoWayColorProperty() =>
        PlatformThread.Run(() =>
            AvaloniaTypedBindingRunner.VerifyTwoWay(
                new SolidColorBrush(Colors.Red),
                new SolidColorBrush(Colors.Blue),
                new TypedTarget(),
                TypedTarget.ColorProperty,
                new SolidColorBrushToMediaColorConverter(),
                Colors.Red,
                Colors.Blue
            )
        );

    [TestMethod]
    public Task BrushConversions_RejectWrongTypesAndDoNotAliasMutableBrushes() =>
        PlatformThread.Run(() =>
        {
            Assert.AreSame(
                AvaloniaProperty.UnsetValue,
                new SolidColorBrushToMediaColorConverter().Convert(Brushes.Red, null, null, null)
            );
        });

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase(
                    "integration:SolidColorBrushToMediaColorConverter",
                    () =>
                        new ConverterCase(
                            "SolidColorBrushToMediaColorConverter",
                            typeof(SolidColorBrushToMediaColorConverter),
                            () => new SolidColorBrushToMediaColorConverter(),
                            [new SolidColorBrush(Colors.Red)],
                            Colors.Red,
                            [new(0, new SolidColorBrush(Colors.Blue), Colors.Blue)],
                            null,
                            false
                        )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(SolidColorBrushToMediaColorConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, true);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(SolidColorBrushToMediaColorConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(SolidColorBrushToMediaColorConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(SolidColorBrushToMediaColorConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () =>
                new SolidColorBrushToMediaColorConverter()
            );
            var converter = (SolidColorBrushToMediaColorConverter)c.Create();
            var pristine = new SolidColorBrushToMediaColorConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                SolidColorBrushToMediaColorConverter.PreferredCultureProperty,
                nameof(SolidColorBrushToMediaColorConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (SolidColorBrushToMediaColorConverter)configured;
        var actual = (SolidColorBrushToMediaColorConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
