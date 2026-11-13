using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class BoolToSplitParameterConverterTests
{
    public static IEnumerable<object[]> SharedCases => BoolToSplitParameterConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(BoolToSplitParameterConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        SingleConversionCase<BoolToSplitParameterConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static SingleConversionSubject<BoolToSplitParameterConverterSuite.Configuration> CreateShared(
        BoolToSplitParameterConverterSuite.Configuration configuration
    )
    {
        var converter = new BoolToSplitParameterConverter();
        var subject = AvaloniaConversionAdapter.Single(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetSeparator is not null)
                converter.Separator = settings.GetSeparator();
            if (settings.ConvertSplitValue is not null)
                converter.ConvertSplitValue = settings.ConvertSplitValue;
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(BoolToSplitParameterConverter), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:BoolToSplitParameterConverter", () =>
                    new ConverterCase(
                        "BoolToSplitParameterConverter",
                        typeof(BoolToSplitParameterConverter),
                        () => new BoolToSplitParameterConverter(),
                        [true],
                        "yes",
                        [new(0, false, "no")],
                        "yes,no,nil",
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToSplitParameterConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, false);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToSplitParameterConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToSplitParameterConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToSplitParameterConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new BoolToSplitParameterConverter());
            var converter = (BoolToSplitParameterConverter)c.Create();
            var pristine = new BoolToSplitParameterConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToSplitParameterConverter.SeparatorProperty,
                nameof(BoolToSplitParameterConverter.Separator),
                x => x.Separator,
                (x, value) => x.Separator = value,
                "alternate-Separator"
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToSplitParameterConverter.PreferredCultureProperty,
                nameof(BoolToSplitParameterConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (BoolToSplitParameterConverter)configured;
        var actual = (BoolToSplitParameterConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.Separator, actual.Separator);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
