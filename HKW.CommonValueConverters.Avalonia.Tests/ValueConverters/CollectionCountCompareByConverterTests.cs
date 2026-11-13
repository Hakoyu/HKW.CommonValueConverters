using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class CollectionCountCompareByConverterTests
{
    public static IEnumerable<object[]> SharedCases => CollectionCountCompareByConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(CollectionCountCompareByConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        SingleConversionCase<CollectionCountCompareByConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static SingleConversionSubject<CollectionCountCompareByConverterSuite.Configuration> CreateShared(
        CollectionCountCompareByConverterSuite.Configuration configuration
    )
    {
        var converter = new CollectionCountCompareByConverter();
        var subject = AvaloniaConversionAdapter.Single(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(CollectionCountCompareByConverter), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:CollectionCountCompareByConverter", () =>
                    new ConverterCase(
                        "CollectionCountCompareByConverter",
                        typeof(CollectionCountCompareByConverter),
                        () => new CollectionCountCompareByConverter(),
                        [new[] { 1, 2, 3 }],
                        true,
                        [new(0, new[] { 1 }, false)],
                        ">2",
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(CollectionCountCompareByConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, false);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(CollectionCountCompareByConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(CollectionCountCompareByConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(CollectionCountCompareByConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new CollectionCountCompareByConverter());
            var converter = (CollectionCountCompareByConverter)c.Create();
            var pristine = new CollectionCountCompareByConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                CollectionCountCompareByConverter.PreferredCultureProperty,
                nameof(CollectionCountCompareByConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (CollectionCountCompareByConverter)configured;
        var actual = (CollectionCountCompareByConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
