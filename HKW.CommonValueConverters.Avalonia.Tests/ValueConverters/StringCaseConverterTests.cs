using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class StringCaseConverterTests
{
    public static IEnumerable<object[]> SharedCases => StringCaseConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(StringCaseConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        SingleConversionCase<StringCaseConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static SingleConversionSubject<StringCaseConverterSuite.Configuration> CreateShared(
        StringCaseConverterSuite.Configuration configuration
    )
    {
        var converter = new StringCaseConverter();
        var subject = AvaloniaConversionAdapter.Single(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(StringCaseConverter), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:StringCaseConverter", () =>
                    new ConverterCase(
                        "StringCaseConverter",
                        typeof(StringCaseConverter),
                        () => new StringCaseConverter(),
                        ["before"],
                        "BEFORE",
                        [new(0, "after", "AFTER")],
                        "U",
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(StringCaseConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, false);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(StringCaseConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(StringCaseConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(StringCaseConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new StringCaseConverter());
            var converter = (StringCaseConverter)c.Create();
            var pristine = new StringCaseConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                StringCaseConverter.PreferredCultureProperty,
                nameof(StringCaseConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (StringCaseConverter)configured;
        var actual = (StringCaseConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
