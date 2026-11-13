using Avalonia.Controls;
using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class DebugConverterTests
{
    [TestMethod]
    public Task DebugConverter_TwoWayTextBox() =>
        PlatformThread.Run(() =>
            AvaloniaTypedBindingRunner.VerifyTwoWay(
                "before",
                "after",
                new TextBox(),
                TextBox.TextProperty,
                new DebugConverter(),
                "before",
                "after"
            )
        );

    public static IEnumerable<object[]> SharedCases => DebugConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(DebugConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(SingleConversionCase<DebugConverterSuite.Configuration> scenario) =>
        PlatformThread.Run(() => scenario.Run(CreateShared));

    private static SingleConversionSubject<DebugConverterSuite.Configuration> CreateShared(
        DebugConverterSuite.Configuration configuration
    )
    {
        var converter = new DebugConverter();
        var subject = AvaloniaConversionAdapter.Single(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(DebugConverter), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:DebugConverter", () =>
                    new ConverterCase(
                        "DebugConverter",
                        typeof(DebugConverter),
                        () => new DebugConverter(),
                        ["before"],
                        "before",
                        [new(0, "after", "after")],
                        null,
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(DebugConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, true);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(DebugConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(DebugConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(DebugConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new DebugConverter());
            var converter = (DebugConverter)c.Create();
            var pristine = new DebugConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                DebugConverter.PreferredCultureProperty,
                nameof(DebugConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (DebugConverter)configured;
        var actual = (DebugConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
