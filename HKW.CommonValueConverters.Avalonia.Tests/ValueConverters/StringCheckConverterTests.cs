using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using HKW.HKWUtils;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class StringCheckConverterTests
{
    public static IEnumerable<object[]> SharedCases => StringCheckConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(StringCheckConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        SingleConversionCase<StringCheckConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static SingleConversionSubject<StringCheckConverterSuite.Configuration> CreateShared(
        StringCheckConverterSuite.Configuration configuration
    )
    {
        var converter = new StringCheckConverter();
        var subject = AvaloniaConversionAdapter.Single(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetStringCheckType is not null)
                converter.StringCheckType = settings.GetStringCheckType();
            if (settings.GetIsInverted is not null)
                converter.IsInverted = settings.GetIsInverted();
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(StringCheckConverter), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:StringCheckConverter", () =>
                    new ConverterCase(
                        "StringCheckConverter",
                        typeof(StringCheckConverter),
                        () => new StringCheckConverter(),
                        [null],
                        true,
                        [new(0, "text", false)],
                        null,
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(StringCheckConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, false);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(StringCheckConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(StringCheckConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(StringCheckConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new StringCheckConverter());
            var converter = (StringCheckConverter)c.Create();
            var pristine = new StringCheckConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                StringCheckConverter.StringCheckTypeProperty,
                nameof(StringCheckConverter.StringCheckType),
                x => x.StringCheckType,
                (x, value) => x.StringCheckType = value,
                Enum.GetValues<StringCheckType>().First(value => value != converter.StringCheckType)
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                StringCheckConverter.IsInvertedProperty,
                nameof(StringCheckConverter.IsInverted),
                x => x.IsInverted,
                (x, value) => x.IsInverted = value,
                converter.IsInverted is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                StringCheckConverter.PreferredCultureProperty,
                nameof(StringCheckConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (StringCheckConverter)configured;
        var actual = (StringCheckConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.StringCheckType, actual.StringCheckType);
        ConverterAssertions.ConfigurationEqual(expected.IsInverted, actual.IsInverted);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
