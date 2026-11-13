using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using HKW.HKWUtils;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class EnumToEnumInfoConverterTests
{
    [TestMethod]
    public Task EnumToEnumInfoConverter_TwoWayTypedInfoProperty() =>
        PlatformThread.Run(() =>
        {
            var info = (IEnumInfo)
                new EnumToEnumInfoConverter().Convert(
                    SampleEnum.Second,
                    typeof(IEnumInfo),
                    null,
                    null
                )!;
            Assert.AreEqual(SampleEnum.Second, info.Value);
            AvaloniaTypedBindingRunner.VerifyTwoWay(
                SampleEnum.First,
                SampleEnum.Second,
                new TypedTarget(),
                TypedTarget.InfoProperty,
                new EnumToEnumInfoConverter(),
                SampleEnum.First,
                info
            );
        });

    public static IEnumerable<object[]> SharedCases => EnumToEnumInfoConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(EnumToEnumInfoConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        SingleConversionCase<EnumToEnumInfoConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static SingleConversionSubject<EnumToEnumInfoConverterSuite.Configuration> CreateShared(
        EnumToEnumInfoConverterSuite.Configuration configuration
    )
    {
        var converter = new EnumToEnumInfoConverter();
        var subject = AvaloniaConversionAdapter.Single(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(EnumToEnumInfoConverter), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:EnumToEnumInfoConverter", () =>
                    new ConverterCase(
                        "EnumToEnumInfoConverter",
                        typeof(EnumToEnumInfoConverter),
                        () => new EnumToEnumInfoConverter(),
                        [SampleEnum.First],
                        SampleEnum.First,
                        [new(0, SampleEnum.Second, SampleEnum.Second)],
                        null,
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(EnumToEnumInfoConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, true);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(EnumToEnumInfoConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(EnumToEnumInfoConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(EnumToEnumInfoConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new EnumToEnumInfoConverter());
            var converter = (EnumToEnumInfoConverter)c.Create();
            var pristine = new EnumToEnumInfoConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                EnumToEnumInfoConverter.PreferredCultureProperty,
                nameof(EnumToEnumInfoConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (EnumToEnumInfoConverter)configured;
        var actual = (EnumToEnumInfoConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
