using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using HKW.HKWUtils;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class EnumToEnumInfoDisplayConverterTests
{
    public static IEnumerable<object[]> SharedCases => EnumToEnumInfoDisplayConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(EnumToEnumInfoDisplayConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        SingleConversionCase<EnumToEnumInfoDisplayConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static SingleConversionSubject<EnumToEnumInfoDisplayConverterSuite.Configuration> CreateShared(
        EnumToEnumInfoDisplayConverterSuite.Configuration configuration
    )
    {
        var converter = new EnumToEnumInfoDisplayConverter();
        var subject = AvaloniaConversionAdapter.Single(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetEnumInfoDisplayTarget is not null)
                converter.EnumInfoDisplayTarget = settings.GetEnumInfoDisplayTarget();
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(EnumToEnumInfoDisplayConverter), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:EnumToEnumInfoDisplayConverter", () =>
                    new ConverterCase(
                        "EnumToEnumInfoDisplayConverter",
                        typeof(EnumToEnumInfoDisplayConverter),
                        () => new EnumToEnumInfoDisplayConverter(),
                        [SampleEnum.First],
                        "First",
                        [new(0, SampleEnum.Second, "Second")],
                        null,
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(EnumToEnumInfoDisplayConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, false);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(EnumToEnumInfoDisplayConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(EnumToEnumInfoDisplayConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(EnumToEnumInfoDisplayConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new EnumToEnumInfoDisplayConverter());
            var converter = (EnumToEnumInfoDisplayConverter)c.Create();
            var pristine = new EnumToEnumInfoDisplayConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                EnumToEnumInfoDisplayConverter.EnumInfoDisplayTargetProperty,
                nameof(EnumToEnumInfoDisplayConverter.EnumInfoDisplayTarget),
                x => x.EnumInfoDisplayTarget,
                (x, value) => x.EnumInfoDisplayTarget = value,
                Enum.GetValues<EnumInfoDisplayTarget>()
                    .First(value => value != converter.EnumInfoDisplayTarget)
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                EnumToEnumInfoDisplayConverter.PreferredCultureProperty,
                nameof(EnumToEnumInfoDisplayConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (EnumToEnumInfoDisplayConverter)configured;
        var actual = (EnumToEnumInfoDisplayConverter)loaded;
        ConverterAssertions.ConfigurationEqual(
            expected.EnumInfoDisplayTarget,
            actual.EnumInfoDisplayTarget
        );
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
