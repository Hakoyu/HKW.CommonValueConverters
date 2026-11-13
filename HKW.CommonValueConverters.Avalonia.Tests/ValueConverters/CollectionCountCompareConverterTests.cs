using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class CollectionCountCompareConverterTests
{
    public static IEnumerable<object[]> SharedCases => CollectionCountCompareConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(CollectionCountCompareConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        SingleConversionCase<CollectionCountCompareConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static SingleConversionSubject<CollectionCountCompareConverterSuite.Configuration> CreateShared(
        CollectionCountCompareConverterSuite.Configuration configuration
    )
    {
        var converter = new CollectionCountCompareConverter();
        var subject = AvaloniaConversionAdapter.Single(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(CollectionCountCompareConverter), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:CollectionCountCompareConverter", () =>
                    new ConverterCase(
                        "CollectionCountCompareConverter",
                        typeof(CollectionCountCompareConverter),
                        () => new CollectionCountCompareConverter(),
                        [new[] { 1, 2, 3 }],
                        0,
                        [new(0, new[] { 1 }, -1)],
                        "3",
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(CollectionCountCompareConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, false);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(CollectionCountCompareConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(CollectionCountCompareConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(CollectionCountCompareConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new CollectionCountCompareConverter());
            var converter = (CollectionCountCompareConverter)c.Create();
            var pristine = new CollectionCountCompareConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                CollectionCountCompareConverter.PreferredCultureProperty,
                nameof(CollectionCountCompareConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (CollectionCountCompareConverter)configured;
        var actual = (CollectionCountCompareConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
