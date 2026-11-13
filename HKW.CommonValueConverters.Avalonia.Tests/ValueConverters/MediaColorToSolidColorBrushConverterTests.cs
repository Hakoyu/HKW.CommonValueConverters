using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class MediaColorToSolidColorBrushConverterTests
{
    [TestMethod]
    public Task MediaColorToSolidColorBrushConverter_TwoWayBorder() =>
        PlatformThread.Run(() =>
            AvaloniaTypedBindingRunner.VerifyTwoWay(
                Colors.Red,
                Colors.Blue,
                new Border(),
                Border.BackgroundProperty,
                new MediaColorToSolidColorBrushConverter(),
                Colors.Red,
                (IBrush?)new SolidColorBrush(Colors.Blue)
            )
        );

    [TestMethod]
    public Task BrushConversions_RejectWrongTypesAndDoNotAliasMutableBrushes() =>
        PlatformThread.Run(() =>
        {
            var converter = new MediaColorToSolidColorBrushConverter();
            var first = (SolidColorBrush)converter.Convert(Colors.Red, null, null, null)!;
            var second = (SolidColorBrush)converter.Convert(Colors.Red, null, null, null)!;
            Assert.AreNotSame(first, second);
            first.Color = Colors.Blue;
            Assert.AreEqual(Colors.Red, second.Color);
            Assert.AreSame(AvaloniaProperty.UnsetValue, converter.Convert(null, null, null, null));
            Assert.AreSame(
                AvaloniaProperty.UnsetValue,
                converter.ConvertBack(Brushes.Red, null, null, null)
            );
        });

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase(
                    "integration:MediaColorToSolidColorBrushConverter",
                    () =>
                        new ConverterCase(
                            "MediaColorToSolidColorBrushConverter",
                            typeof(MediaColorToSolidColorBrushConverter),
                            () => new MediaColorToSolidColorBrushConverter(),
                            [Colors.Red],
                            Colors.Red,
                            [new(0, Colors.Blue, Colors.Blue)],
                            null,
                            false
                        )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(MediaColorToSolidColorBrushConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, true);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(MediaColorToSolidColorBrushConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(MediaColorToSolidColorBrushConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(MediaColorToSolidColorBrushConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () =>
                new MediaColorToSolidColorBrushConverter()
            );
            var converter = (MediaColorToSolidColorBrushConverter)c.Create();
            var pristine = new MediaColorToSolidColorBrushConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                MediaColorToSolidColorBrushConverter.PreferredCultureProperty,
                nameof(MediaColorToSolidColorBrushConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (MediaColorToSolidColorBrushConverter)configured;
        var actual = (MediaColorToSolidColorBrushConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
