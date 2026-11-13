using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using HKW.HKWUtils;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class NumberClampConverterTests
{
    public static IEnumerable<object[]> SharedCases => NumberClampConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(NumberClampConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        SingleConversionCase<NumberClampConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static SingleConversionSubject<NumberClampConverterSuite.Configuration> CreateShared(
        NumberClampConverterSuite.Configuration configuration
    )
    {
        var converter = new NumberClampConverter();
        var subject = AvaloniaConversionAdapter.Single(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetNumberType is not null)
                converter.NumberType = settings.GetNumberType();
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(NumberClampConverter), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:NumberClampConverter", () =>
                    new ConverterCase(
                        "NumberClampConverter",
                        typeof(NumberClampConverter),
                        () => new NumberClampConverter(),
                        [5],
                        true,
                        [new(0, 12, false)],
                        "0,10",
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(NumberClampConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, false);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(NumberClampConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(NumberClampConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(NumberClampConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new NumberClampConverter());
            var converter = (NumberClampConverter)c.Create();
            var pristine = new NumberClampConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                NumberClampConverter.NumberTypeProperty,
                nameof(NumberClampConverter.NumberType),
                x => x.NumberType,
                (x, value) => x.NumberType = value,
                NumberType.Decimal
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                NumberClampConverter.PreferredCultureProperty,
                nameof(NumberClampConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (NumberClampConverter)configured;
        var actual = (NumberClampConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.NumberType, actual.NumberType);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
