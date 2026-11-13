using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class FirstBoolToValueMultiConverterTests
{
    public static IEnumerable<object[]> SharedCases => FirstBoolToValueMultiConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(FirstBoolToValueMultiConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        MultiConversionCase<FirstBoolToValueMultiConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static MultiConversionSubject<FirstBoolToValueMultiConverterSuite.Configuration> CreateShared(
        FirstBoolToValueMultiConverterSuite.Configuration configuration
    )
    {
        var converter = new FirstBoolToValueMultiConverter();
        var subject = AvaloniaConversionAdapter.Multi(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(FirstBoolToValueMultiConverter), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:FirstBoolToValueMultiConverter", () =>
                    new ConverterCase(
                        "FirstBoolToValueMultiConverter",
                        typeof(FirstBoolToValueMultiConverter),
                        () => new FirstBoolToValueMultiConverter(),
                        [true, "yes", "no", "nil"],
                        "yes",
                        [
                            new(0, false, "no"),
                            new(2, "changed", "changed"),
                            new(0, true, "yes"),
                            new(1, "updated", "updated"),
                            new(0, null, "nil"),
                            new(3, "empty", "empty"),
                        ],
                        null,
                        true
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(FirstBoolToValueMultiConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(FirstBoolToValueMultiConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(FirstBoolToValueMultiConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(FirstBoolToValueMultiConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new FirstBoolToValueMultiConverter());
            var converter = (FirstBoolToValueMultiConverter)c.Create();
            var pristine = new FirstBoolToValueMultiConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                FirstBoolToValueMultiConverter.PreferredCultureProperty,
                nameof(FirstBoolToValueMultiConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (FirstBoolToValueMultiConverter)configured;
        var actual = (FirstBoolToValueMultiConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
