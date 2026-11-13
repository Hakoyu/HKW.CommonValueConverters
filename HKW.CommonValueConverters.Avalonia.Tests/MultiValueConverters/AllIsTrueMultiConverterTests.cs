using Avalonia.Data;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class AllIsTrueMultiConverterTests
{
    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:AllIsTrueMultiConverter", () =>
                    new ConverterCase(
                        "AllIsTrueMultiConverter",
                        typeof(AllIsTrueMultiConverter),
                        () => new AllIsTrueMultiConverter(),
                        [true, true],
                        true,
                        [new(0, false, false), new(0, true, true), new(1, false, false)],
                        null,
                        true
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(AllIsTrueMultiConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(AllIsTrueMultiConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(AllIsTrueMultiConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(AllIsTrueMultiConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new AllIsTrueMultiConverter());
            var converter = (AllIsTrueMultiConverter)c.Create();
            var pristine = new AllIsTrueMultiConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                AllIsTrueMultiConverter.ValueProperty,
                nameof(AllIsTrueMultiConverter.Value),
                x => x.Value,
                (x, value) => x.Value = value,
                converter.Value is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                AllIsTrueMultiConverter.IsStringEqualsProperty,
                nameof(AllIsTrueMultiConverter.IsStringEquals),
                x => x.IsStringEquals,
                (x, value) => x.IsStringEquals = value,
                converter.IsStringEquals is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                AllIsTrueMultiConverter.IsInvertedProperty,
                nameof(AllIsTrueMultiConverter.IsInverted),
                x => x.IsInverted,
                (x, value) => x.IsInverted = value,
                converter.IsInverted is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                AllIsTrueMultiConverter.PreferredCultureProperty,
                nameof(AllIsTrueMultiConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (AllIsTrueMultiConverter)configured;
        var actual = (AllIsTrueMultiConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.Value, actual.Value);
        ConverterAssertions.ConfigurationEqual(expected.IsStringEquals, actual.IsStringEquals);
        ConverterAssertions.ConfigurationEqual(expected.IsInverted, actual.IsInverted);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
