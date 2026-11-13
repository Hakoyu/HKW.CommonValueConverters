using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class FirstStringStateToOtherStringMultiConverterTests
{
    public static IEnumerable<object[]> SharedCases =>
        FirstStringStateToOtherStringMultiConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(
        typeof(FirstStringStateToOtherStringMultiConverter),
        ConverterTestLayer.Behavior
    )]
    public Task SharedBehavior(
        MultiConversionCase<FirstStringStateToOtherStringMultiConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static MultiConversionSubject<FirstStringStateToOtherStringMultiConverterSuite.Configuration> CreateShared(
        FirstStringStateToOtherStringMultiConverterSuite.Configuration configuration
    )
    {
        var converter = new FirstStringStateToOtherStringMultiConverter();
        var subject = AvaloniaConversionAdapter.Multi(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
                if (settings.GetDefaultResult is not null)
                    converter.DefaultResult = settings.GetDefaultResult();
            }
        );
        Assert.AreEqual(
            typeof(FirstStringStateToOtherStringMultiConverter),
            subject.Instance.GetType()
        );
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase(
                    "integration:FirstStringStateToOtherStringMultiConverter",
                    () =>
                        new ConverterCase(
                            "FirstStringStateToOtherStringMultiConverter",
                            typeof(FirstStringStateToOtherStringMultiConverter),
                            () => new FirstStringStateToOtherStringMultiConverter(),
                            ["text", "text-result", "null-result", "empty-result", "space-result"],
                            "text-result",
                            [
                                new(1, "updated", "updated"),
                                new(0, null, "null-result"),
                                new(2, "nil", "nil"),
                                new(0, "", "empty-result"),
                                new(3, "empty", "empty"),
                                new(0, " ", "space-result"),
                                new(4, "space", "space"),
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
    [ConverterTest(
        typeof(FirstStringStateToOtherStringMultiConverter),
        ConverterTestLayer.Behavior
    )]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(FirstStringStateToOtherStringMultiConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(FirstStringStateToOtherStringMultiConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(
        typeof(FirstStringStateToOtherStringMultiConverter),
        ConverterTestLayer.Properties
    )]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(
                c,
                () => new FirstStringStateToOtherStringMultiConverter()
            );
            var converter = (FirstStringStateToOtherStringMultiConverter)c.Create();
            var pristine = new FirstStringStateToOtherStringMultiConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                FirstStringStateToOtherStringMultiConverter.PreferredCultureProperty,
                nameof(FirstStringStateToOtherStringMultiConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (FirstStringStateToOtherStringMultiConverter)configured;
        var actual = (FirstStringStateToOtherStringMultiConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
