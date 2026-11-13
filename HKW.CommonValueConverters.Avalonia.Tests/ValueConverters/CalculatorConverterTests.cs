using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using HKW.HKWUtils;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class CalculatorConverterTests
{
    public static IEnumerable<object[]> SharedCases => CalculatorConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(CalculatorConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        SingleConversionCase<CalculatorConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static SingleConversionSubject<CalculatorConverterSuite.Configuration> CreateShared(
        CalculatorConverterSuite.Configuration configuration
    )
    {
        var converter = new CalculatorConverter();
        var subject = AvaloniaConversionAdapter.Single(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetNumberType is not null)
                converter.NumberType = settings.GetNumberType();
            if (settings.GetOperatorType is not null)
                converter.OperatorType = settings.GetOperatorType();
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(CalculatorConverter), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:CalculatorConverter", () =>
                    new ConverterCase(
                        "CalculatorConverter",
                        typeof(CalculatorConverter),
                        () => new CalculatorConverter(),
                        [2],
                        10,
                        [new(0, 5, 13)],
                        "+8",
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(CalculatorConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, false);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(CalculatorConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(CalculatorConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(CalculatorConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new CalculatorConverter());
            var converter = (CalculatorConverter)c.Create();
            var pristine = new CalculatorConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                CalculatorConverter.NumberTypeProperty,
                nameof(CalculatorConverter.NumberType),
                x => x.NumberType,
                (x, value) => x.NumberType = value,
                NumberType.Decimal
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                CalculatorConverter.OperatorTypeProperty,
                nameof(CalculatorConverter.OperatorType),
                x => x.OperatorType,
                (x, value) => x.OperatorType = value,
                Enum.GetValues<ArithmeticOperatorType>()
                    .First(value => value != converter.OperatorType)
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                CalculatorConverter.PreferredCultureProperty,
                nameof(CalculatorConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (CalculatorConverter)configured;
        var actual = (CalculatorConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.NumberType, actual.NumberType);
        ConverterAssertions.ConfigurationEqual(expected.OperatorType, actual.OperatorType);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
