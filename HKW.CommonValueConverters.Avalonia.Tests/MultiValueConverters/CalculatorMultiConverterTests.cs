using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using HKW.HKWUtils;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class CalculatorMultiConverterTests
{
    public static IEnumerable<object[]> SharedCases => CalculatorMultiConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(CalculatorMultiConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        MultiConversionCase<CalculatorMultiConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static MultiConversionSubject<CalculatorMultiConverterSuite.Configuration> CreateShared(
        CalculatorMultiConverterSuite.Configuration configuration
    )
    {
        var converter = new CalculatorMultiConverter();
        var subject = AvaloniaConversionAdapter.Multi(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetNumberType is not null)
                converter.NumberType = settings.GetNumberType();
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(CalculatorMultiConverter), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:CalculatorMultiConverter", () =>
                    new ConverterCase(
                        "CalculatorMultiConverter",
                        typeof(CalculatorMultiConverter),
                        () => new CalculatorMultiConverter(),
                        [2, 3],
                        5,
                        [new(0, 4, 7), new(1, 6, 10)],
                        "+",
                        true
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(CalculatorMultiConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(CalculatorMultiConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(CalculatorMultiConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(CalculatorMultiConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new CalculatorMultiConverter());
            var converter = (CalculatorMultiConverter)c.Create();
            var pristine = new CalculatorMultiConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                CalculatorMultiConverter.NumberTypeProperty,
                nameof(CalculatorMultiConverter.NumberType),
                x => x.NumberType,
                (x, value) => x.NumberType = value,
                NumberType.Decimal
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                CalculatorMultiConverter.PreferredCultureProperty,
                nameof(CalculatorMultiConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (CalculatorMultiConverter)configured;
        var actual = (CalculatorMultiConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.NumberType, actual.NumberType);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
