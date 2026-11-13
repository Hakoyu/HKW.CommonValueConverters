using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using HKW.HKWUtils;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class NumberCompareByConverterTests
{
    public static IEnumerable<object[]> SharedCases => NumberCompareByConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(NumberCompareByConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        SingleConversionCase<NumberCompareByConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static SingleConversionSubject<NumberCompareByConverterSuite.Configuration> CreateShared(
        NumberCompareByConverterSuite.Configuration configuration
    )
    {
        var converter = new NumberCompareByConverter();
        var subject = AvaloniaConversionAdapter.Single(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetNumberType is not null)
                converter.NumberType = settings.GetNumberType();
            if (settings.GetComparisonType is not null)
                converter.ComparisonType = settings.GetComparisonType();
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(NumberCompareByConverter), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:NumberCompareByConverter", () =>
                    new ConverterCase(
                        "NumberCompareByConverter",
                        typeof(NumberCompareByConverter),
                        () => new NumberCompareByConverter(),
                        [5],
                        true,
                        [new(0, 1, false)],
                        ">3",
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(NumberCompareByConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, false);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(NumberCompareByConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(NumberCompareByConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(NumberCompareByConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new NumberCompareByConverter());
            var converter = (NumberCompareByConverter)c.Create();
            var pristine = new NumberCompareByConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                NumberCompareByConverter.NumberTypeProperty,
                nameof(NumberCompareByConverter.NumberType),
                x => x.NumberType,
                (x, value) => x.NumberType = value,
                NumberType.Decimal
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                NumberCompareByConverter.ComparisonTypeProperty,
                nameof(NumberCompareByConverter.ComparisonType),
                x => x.ComparisonType,
                (x, value) => x.ComparisonType = value,
                Enum.GetValues<ComparisonOperatorType>()
                    .First(value => value != converter.ComparisonType)
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                NumberCompareByConverter.PreferredCultureProperty,
                nameof(NumberCompareByConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (NumberCompareByConverter)configured;
        var actual = (NumberCompareByConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.NumberType, actual.NumberType);
        ConverterAssertions.ConfigurationEqual(expected.ComparisonType, actual.ComparisonType);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
