using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class BoolToFontWeightConverterTests
{
    [TestMethod]
    public Task BoolToFontWeightConverter_TextBlockFontWeight() =>
        PlatformThread.Run(() =>
            AvaloniaTypedBindingRunner.VerifyBoolean(
                new TextBlock(),
                TextBlock.FontWeightProperty,
                new BoolToFontWeightConverter(),
                FontWeight.Bold,
                FontWeight.Normal
            )
        );

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:BoolToFontWeightConverter", () =>
                    new ConverterCase(
                        "BoolToFontWeightConverter",
                        typeof(BoolToFontWeightConverter),
                        () => new BoolToFontWeightConverter(),
                        [true],
                        FontWeight.Bold,
                        [new(0, false, FontWeight.Normal)],
                        null,
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToFontWeightConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, false);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToFontWeightConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToFontWeightConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToFontWeightConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new BoolToFontWeightConverter());
            var converter = (BoolToFontWeightConverter)c.Create();
            var pristine = new BoolToFontWeightConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToFontWeightConverter.TrueValueProperty,
                nameof(BoolToFontWeightConverter.TrueValue),
                x => x.TrueValue,
                (x, value) => x.TrueValue = value,
                FontWeight.Light
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToFontWeightConverter.FalseValueProperty,
                nameof(BoolToFontWeightConverter.FalseValue),
                x => x.FalseValue,
                (x, value) => x.FalseValue = value,
                FontWeight.Light
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToFontWeightConverter.IsNullableProperty,
                nameof(BoolToFontWeightConverter.IsNullable),
                x => x.IsNullable,
                (x, value) => x.IsNullable = value,
                converter.IsNullable is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToFontWeightConverter.NullValueProperty,
                nameof(BoolToFontWeightConverter.NullValue),
                x => x.NullValue,
                (x, value) => x.NullValue = value,
                FontWeight.Light
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToFontWeightConverter.IsInvertedProperty,
                nameof(BoolToFontWeightConverter.IsInverted),
                x => x.IsInverted,
                (x, value) => x.IsInverted = value,
                converter.IsInverted is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToFontWeightConverter.PreferredCultureProperty,
                nameof(BoolToFontWeightConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (BoolToFontWeightConverter)configured;
        var actual = (BoolToFontWeightConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.TrueValue, actual.TrueValue);
        ConverterAssertions.ConfigurationEqual(expected.FalseValue, actual.FalseValue);
        ConverterAssertions.ConfigurationEqual(expected.IsNullable, actual.IsNullable);
        ConverterAssertions.ConfigurationEqual(expected.NullValue, actual.NullValue);
        ConverterAssertions.ConfigurationEqual(expected.IsInverted, actual.IsInverted);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
