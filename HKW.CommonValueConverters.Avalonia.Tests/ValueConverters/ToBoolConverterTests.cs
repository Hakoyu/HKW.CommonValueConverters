using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class ToBoolConverterTests
{
    public static IEnumerable<object[]> SharedCases => ToBoolConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(ToBoolConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(SingleConversionCase<ToBoolConverterSuite.Configuration> scenario) =>
        PlatformThread.Run(() => scenario.Run(CreateShared));

    private static SingleConversionSubject<ToBoolConverterSuite.Configuration> CreateShared(
        ToBoolConverterSuite.Configuration configuration
    )
    {
        var converter = new ToBoolConverter();
        var subject = AvaloniaConversionAdapter.Single(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetNullValue is not null)
                converter.NullValue = settings.GetNullValue();
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(ToBoolConverter), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:ToBoolConverter", () =>
                    new ConverterCase(
                        "ToBoolConverter",
                        typeof(ToBoolConverter),
                        () => new ToBoolConverter(),
                        ["1"],
                        true,
                        [new(0, "0", false)],
                        null,
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(ToBoolConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, false);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(ToBoolConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(ToBoolConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(ToBoolConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new ToBoolConverter());
            var converter = (ToBoolConverter)c.Create();
            var pristine = new ToBoolConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                ToBoolConverter.NullValueProperty,
                nameof(ToBoolConverter.NullValue),
                x => x.NullValue,
                (x, value) => x.NullValue = value,
                true
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                ToBoolConverter.PreferredCultureProperty,
                nameof(ToBoolConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (ToBoolConverter)configured;
        var actual = (ToBoolConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.NullValue, actual.NullValue);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
