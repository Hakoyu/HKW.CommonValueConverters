using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class EnumsToEnumInfosConverterTests
{
    public static IEnumerable<object[]> SharedCases => EnumsToEnumInfosConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(EnumsToEnumInfosConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        SingleConversionCase<EnumsToEnumInfosConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static SingleConversionSubject<EnumsToEnumInfosConverterSuite.Configuration> CreateShared(
        EnumsToEnumInfosConverterSuite.Configuration configuration
    )
    {
        var converter = new EnumsToEnumInfosConverter();
        var subject = AvaloniaConversionAdapter.Single(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetOnlyValid is not null)
                converter.OnlyValid = settings.GetOnlyValid();
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(EnumsToEnumInfosConverter), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:EnumsToEnumInfosConverter", () =>
                    new ConverterCase(
                        "EnumsToEnumInfosConverter",
                        typeof(EnumsToEnumInfosConverter),
                        () => new EnumsToEnumInfosConverter(),
                        [new[] { SampleEnum.First }],
                        new object[] { SampleEnum.First, SampleEnum.Second },
                        [new(0, new[] { OtherEnum.Other }, new object[] { OtherEnum.Other })],
                        null,
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(EnumsToEnumInfosConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, false);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(EnumsToEnumInfosConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(EnumsToEnumInfosConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(EnumsToEnumInfosConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new EnumsToEnumInfosConverter());
            var converter = (EnumsToEnumInfosConverter)c.Create();
            var pristine = new EnumsToEnumInfosConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                EnumsToEnumInfosConverter.OnlyValidProperty,
                nameof(EnumsToEnumInfosConverter.OnlyValid),
                x => x.OnlyValid,
                (x, value) => x.OnlyValid = value,
                converter.OnlyValid is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                EnumsToEnumInfosConverter.PreferredCultureProperty,
                nameof(EnumsToEnumInfosConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (EnumsToEnumInfosConverter)configured;
        var actual = (EnumsToEnumInfosConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.OnlyValid, actual.OnlyValid);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
