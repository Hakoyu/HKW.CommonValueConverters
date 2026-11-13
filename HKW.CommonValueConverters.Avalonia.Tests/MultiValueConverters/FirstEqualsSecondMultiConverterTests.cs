using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class FirstEqualsSecondMultiConverterTests
{
    public static IEnumerable<object[]> SharedCases => FirstEqualsSecondMultiConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(FirstEqualsSecondMultiConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        MultiConversionCase<FirstEqualsSecondMultiConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static MultiConversionSubject<FirstEqualsSecondMultiConverterSuite.Configuration> CreateShared(
        FirstEqualsSecondMultiConverterSuite.Configuration configuration
    )
    {
        var converter = new FirstEqualsSecondMultiConverter();
        var subject = AvaloniaConversionAdapter.Multi(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetIsStringEquals is not null)
                converter.IsStringEquals = settings.GetIsStringEquals();
            if (settings.GetIsInverted is not null)
                converter.IsInverted = settings.GetIsInverted();
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(FirstEqualsSecondMultiConverter), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:FirstEqualsSecondMultiConverter", () =>
                    new ConverterCase(
                        "FirstEqualsSecondMultiConverter",
                        typeof(FirstEqualsSecondMultiConverter),
                        () => new FirstEqualsSecondMultiConverter(),
                        ["one", "one"],
                        true,
                        [new(0, "two", false), new(1, "two", true)],
                        null,
                        true
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(FirstEqualsSecondMultiConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(FirstEqualsSecondMultiConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(FirstEqualsSecondMultiConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(FirstEqualsSecondMultiConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new FirstEqualsSecondMultiConverter());
            var converter = (FirstEqualsSecondMultiConverter)c.Create();
            var pristine = new FirstEqualsSecondMultiConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                FirstEqualsSecondMultiConverter.IsStringEqualsProperty,
                nameof(FirstEqualsSecondMultiConverter.IsStringEquals),
                x => x.IsStringEquals,
                (x, value) => x.IsStringEquals = value,
                converter.IsStringEquals is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                FirstEqualsSecondMultiConverter.IsInvertedProperty,
                nameof(FirstEqualsSecondMultiConverter.IsInverted),
                x => x.IsInverted,
                (x, value) => x.IsInverted = value,
                converter.IsInverted is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                FirstEqualsSecondMultiConverter.PreferredCultureProperty,
                nameof(FirstEqualsSecondMultiConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (FirstEqualsSecondMultiConverter)configured;
        var actual = (FirstEqualsSecondMultiConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.IsStringEquals, actual.IsStringEquals);
        ConverterAssertions.ConfigurationEqual(expected.IsInverted, actual.IsInverted);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
