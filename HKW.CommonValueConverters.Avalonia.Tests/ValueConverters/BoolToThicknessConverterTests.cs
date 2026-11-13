using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class BoolToThicknessConverterTests
{
    [TestMethod]
    public Task BoolToThicknessConverter_BorderPadding() =>
        PlatformThread.Run(() =>
            AvaloniaTypedBindingRunner.VerifyBoolean(
                new Border(),
                Border.PaddingProperty,
                new BoolToThicknessConverter(),
                new Thickness(5),
                new Thickness()
            )
        );

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:BoolToThicknessConverter", () =>
                    new ConverterCase(
                        "BoolToThicknessConverter",
                        typeof(BoolToThicknessConverter),
                        () => new BoolToThicknessConverter(),
                        [true],
                        new Thickness(5),
                        [new(0, false, new Thickness(0))],
                        null,
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToThicknessConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, false);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToThicknessConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToThicknessConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToThicknessConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new BoolToThicknessConverter());
            var converter = (BoolToThicknessConverter)c.Create();
            var pristine = new BoolToThicknessConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToThicknessConverter.TrueValueProperty,
                nameof(BoolToThicknessConverter.TrueValue),
                x => x.TrueValue,
                (x, value) => x.TrueValue = value,
                new Thickness(9, 8, 7, 6)
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToThicknessConverter.FalseValueProperty,
                nameof(BoolToThicknessConverter.FalseValue),
                x => x.FalseValue,
                (x, value) => x.FalseValue = value,
                new Thickness(9, 8, 7, 6)
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToThicknessConverter.IsNullableProperty,
                nameof(BoolToThicknessConverter.IsNullable),
                x => x.IsNullable,
                (x, value) => x.IsNullable = value,
                converter.IsNullable is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToThicknessConverter.NullValueProperty,
                nameof(BoolToThicknessConverter.NullValue),
                x => x.NullValue,
                (x, value) => x.NullValue = value,
                new Thickness(9, 8, 7, 6)
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToThicknessConverter.IsInvertedProperty,
                nameof(BoolToThicknessConverter.IsInverted),
                x => x.IsInverted,
                (x, value) => x.IsInverted = value,
                converter.IsInverted is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToThicknessConverter.PreferredCultureProperty,
                nameof(BoolToThicknessConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (BoolToThicknessConverter)configured;
        var actual = (BoolToThicknessConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.TrueValue, actual.TrueValue);
        ConverterAssertions.ConfigurationEqual(expected.FalseValue, actual.FalseValue);
        ConverterAssertions.ConfigurationEqual(expected.IsNullable, actual.IsNullable);
        ConverterAssertions.ConfigurationEqual(expected.NullValue, actual.NullValue);
        ConverterAssertions.ConfigurationEqual(expected.IsInverted, actual.IsInverted);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
