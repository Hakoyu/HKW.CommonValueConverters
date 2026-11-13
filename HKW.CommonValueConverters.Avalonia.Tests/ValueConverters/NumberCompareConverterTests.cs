using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using HKW.HKWUtils;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class NumberCompareConverterTests
{
    public static IEnumerable<object[]> SharedCases => NumberCompareConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(NumberCompareConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        SingleConversionCase<NumberCompareConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static SingleConversionSubject<NumberCompareConverterSuite.Configuration> CreateShared(
        NumberCompareConverterSuite.Configuration configuration
    )
    {
        var converter = new NumberCompareConverter();
        var subject = AvaloniaConversionAdapter.Single(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetNumberType is not null)
                converter.NumberType = settings.GetNumberType();
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(NumberCompareConverter), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:NumberCompareConverter", () =>
                    new ConverterCase(
                        "NumberCompareConverter",
                        typeof(NumberCompareConverter),
                        () => new NumberCompareConverter(),
                        [5],
                        1,
                        [new(0, 1, -1)],
                        "3",
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(NumberCompareConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, false);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(NumberCompareConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(NumberCompareConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(NumberCompareConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new NumberCompareConverter());
            var converter = (NumberCompareConverter)c.Create();
            var pristine = new NumberCompareConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                NumberCompareConverter.NumberTypeProperty,
                nameof(NumberCompareConverter.NumberType),
                x => x.NumberType,
                (x, value) => x.NumberType = value,
                NumberType.Decimal
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                NumberCompareConverter.PreferredCultureProperty,
                nameof(NumberCompareConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (NumberCompareConverter)configured;
        var actual = (NumberCompareConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.NumberType, actual.NumberType);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
