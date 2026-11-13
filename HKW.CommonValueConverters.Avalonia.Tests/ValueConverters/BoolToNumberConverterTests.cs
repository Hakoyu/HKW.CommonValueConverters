using Avalonia.Data;
using HKW.CommonValueConvertersTest.Testing;
using HKW.HKWUtils;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class BoolToNumberConverterTests
{
    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:BoolToNumberConverter", () =>
                    new ConverterCase(
                        "BoolToNumberConverter",
                        typeof(BoolToNumberConverter),
                        () => new BoolToNumberConverter { TrueValue = 7, FalseValue = 3 },
                        [true],
                        7,
                        [new(0, false, 3)],
                        null,
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToNumberConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, false);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToNumberConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToNumberConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToNumberConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new BoolToNumberConverter());
            var converter = (BoolToNumberConverter)c.Create();
            var pristine = new BoolToNumberConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToNumberConverter.NumberTypeProperty,
                nameof(BoolToNumberConverter.NumberType),
                x => x.NumberType,
                (x, value) => x.NumberType = value,
                NumberType.Decimal
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToNumberConverter.TrueValueProperty,
                nameof(BoolToNumberConverter.TrueValue),
                x => x.TrueValue,
                (x, value) => x.TrueValue = value,
                new object()
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToNumberConverter.FalseValueProperty,
                nameof(BoolToNumberConverter.FalseValue),
                x => x.FalseValue,
                (x, value) => x.FalseValue = value,
                new object()
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToNumberConverter.IsNullableProperty,
                nameof(BoolToNumberConverter.IsNullable),
                x => x.IsNullable,
                (x, value) => x.IsNullable = value,
                converter.IsNullable is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToNumberConverter.NullValueProperty,
                nameof(BoolToNumberConverter.NullValue),
                x => x.NullValue,
                (x, value) => x.NullValue = value,
                new object()
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToNumberConverter.IsInvertedProperty,
                nameof(BoolToNumberConverter.IsInverted),
                x => x.IsInverted,
                (x, value) => x.IsInverted = value,
                converter.IsInverted is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToNumberConverter.PreferredCultureProperty,
                nameof(BoolToNumberConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (BoolToNumberConverter)configured;
        var actual = (BoolToNumberConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.NumberType, actual.NumberType);
        ConverterAssertions.ConfigurationEqual(expected.TrueValue, actual.TrueValue);
        ConverterAssertions.ConfigurationEqual(expected.FalseValue, actual.FalseValue);
        ConverterAssertions.ConfigurationEqual(expected.IsNullable, actual.IsNullable);
        ConverterAssertions.ConfigurationEqual(expected.NullValue, actual.NullValue);
        ConverterAssertions.ConfigurationEqual(expected.IsInverted, actual.IsInverted);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
