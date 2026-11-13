using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class BoolToSolidColorBrushConverterTests
{
    [TestMethod]
    public Task BoolToSolidColorBrushConverter_BorderBackground() =>
        PlatformThread.Run(() =>
            AvaloniaTypedBindingRunner.VerifyBoolean(
                new Border(),
                Border.BackgroundProperty,
                new BoolToSolidColorBrushConverter(),
                Colors.White,
                Colors.Black
            )
        );

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:BoolToSolidColorBrushConverter", () =>
                    new ConverterCase(
                        "BoolToSolidColorBrushConverter",
                        typeof(BoolToSolidColorBrushConverter),
                        () => new BoolToSolidColorBrushConverter(),
                        [true],
                        Colors.White,
                        [new(0, false, Colors.Black)],
                        null,
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToSolidColorBrushConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, false);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToSolidColorBrushConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToSolidColorBrushConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToSolidColorBrushConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new BoolToSolidColorBrushConverter());
            var converter = (BoolToSolidColorBrushConverter)c.Create();
            var pristine = new BoolToSolidColorBrushConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToSolidColorBrushConverter.TrueValueProperty,
                nameof(BoolToSolidColorBrushConverter.TrueValue),
                x => x.TrueValue,
                (x, value) => x.TrueValue = value,
                new SolidColorBrush(Colors.Orange)
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToSolidColorBrushConverter.FalseValueProperty,
                nameof(BoolToSolidColorBrushConverter.FalseValue),
                x => x.FalseValue,
                (x, value) => x.FalseValue = value,
                new SolidColorBrush(Colors.Orange)
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToSolidColorBrushConverter.IsNullableProperty,
                nameof(BoolToSolidColorBrushConverter.IsNullable),
                x => x.IsNullable,
                (x, value) => x.IsNullable = value,
                converter.IsNullable is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToSolidColorBrushConverter.NullValueProperty,
                nameof(BoolToSolidColorBrushConverter.NullValue),
                x => x.NullValue,
                (x, value) => x.NullValue = value,
                new SolidColorBrush(Colors.Orange)
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToSolidColorBrushConverter.IsInvertedProperty,
                nameof(BoolToSolidColorBrushConverter.IsInverted),
                x => x.IsInverted,
                (x, value) => x.IsInverted = value,
                converter.IsInverted is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToSolidColorBrushConverter.PreferredCultureProperty,
                nameof(BoolToSolidColorBrushConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (BoolToSolidColorBrushConverter)configured;
        var actual = (BoolToSolidColorBrushConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.TrueValue, actual.TrueValue);
        ConverterAssertions.ConfigurationEqual(expected.FalseValue, actual.FalseValue);
        ConverterAssertions.ConfigurationEqual(expected.IsNullable, actual.IsNullable);
        ConverterAssertions.ConfigurationEqual(expected.NullValue, actual.NullValue);
        ConverterAssertions.ConfigurationEqual(expected.IsInverted, actual.IsInverted);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
