using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class AllEqualsMultiConverterTests
{
    public static IEnumerable<object[]> SharedCases => AllEqualsMultiConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(AllEqualsMultiConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        MultiConversionCase<AllEqualsMultiConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static MultiConversionSubject<AllEqualsMultiConverterSuite.Configuration> CreateShared(
        AllEqualsMultiConverterSuite.Configuration configuration
    )
    {
        var converter = new AllEqualsMultiConverter();
        var subject = AvaloniaConversionAdapter.Multi(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetValue is not null)
                converter.Value = settings.GetValue();
            if (settings.GetIsStringEquals is not null)
                converter.IsStringEquals = settings.GetIsStringEquals();
            if (settings.GetIsInverted is not null)
                converter.IsInverted = settings.GetIsInverted();
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(AllEqualsMultiConverter), subject.Instance.GetType());
        return subject;
    }

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(AllEqualsMultiConverter<string>), ConverterTestLayer.Behavior)]
    public Task SharedBehaviorGeneric(
        MultiConversionCase<AllEqualsMultiConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateSharedGeneric));

    private static MultiConversionSubject<AllEqualsMultiConverterSuite.Configuration> CreateSharedGeneric(
        AllEqualsMultiConverterSuite.Configuration configuration
    )
    {
        var converter = new AllEqualsMultiConverter<string>();
        var subject = AvaloniaConversionAdapter.Multi(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetValue is not null)
                converter.Value = settings.GetValue();
            if (settings.GetIsStringEquals is not null)
                converter.IsStringEquals = settings.GetIsStringEquals();
            if (settings.GetIsInverted is not null)
                converter.IsInverted = settings.GetIsInverted();
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(AllEqualsMultiConverter<string>), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:AllEqualsMultiConverter", () =>
                    new ConverterCase(
                        "AllEqualsMultiConverter",
                        typeof(AllEqualsMultiConverter),
                        () => new AllEqualsMultiConverter { Value = "match" },
                        ["match", "match"],
                        true,
                        [new(0, "other", false), new(0, "match", true), new(1, "other", false)],
                        null,
                        true
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(AllEqualsMultiConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(AllEqualsMultiConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(AllEqualsMultiConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(AllEqualsMultiConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new AllEqualsMultiConverter());
            var converter = (AllEqualsMultiConverter)c.Create();
            var pristine = new AllEqualsMultiConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                AllEqualsMultiConverter.ValueProperty,
                nameof(AllEqualsMultiConverter.Value),
                x => x.Value,
                (x, value) => x.Value = value,
                new object()
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                AllEqualsMultiConverter.IsStringEqualsProperty,
                nameof(AllEqualsMultiConverter.IsStringEquals),
                x => x.IsStringEquals,
                (x, value) => x.IsStringEquals = value,
                converter.IsStringEquals is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                AllEqualsMultiConverter.IsInvertedProperty,
                nameof(AllEqualsMultiConverter.IsInverted),
                x => x.IsInverted,
                (x, value) => x.IsInverted = value,
                converter.IsInverted is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                AllEqualsMultiConverter.PreferredCultureProperty,
                nameof(AllEqualsMultiConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (AllEqualsMultiConverter)configured;
        var actual = (AllEqualsMultiConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.Value, actual.Value);
        ConverterAssertions.ConfigurationEqual(expected.IsStringEquals, actual.IsStringEquals);
        ConverterAssertions.ConfigurationEqual(expected.IsInverted, actual.IsInverted);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }

    public static IEnumerable<object[]> IntegrationCasesGeneric
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:AllEqualsMultiConverter_String", () =>
                    new ConverterCase(
                        "AllEqualsMultiConverter_String",
                        typeof(AllEqualsMultiConverter<string>),
                        () => new AllEqualsMultiConverter<string> { Value = "match" },
                        ["match", "match"],
                        true,
                        [new(0, "other", false), new(0, "match", true), new(1, "other", false)],
                        null,
                        true
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCasesGeneric))]
    [ConverterTest(typeof(AllEqualsMultiConverter<string>), ConverterTestLayer.Behavior)]
    public Task PlatformBehaviorGeneric(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCasesGeneric))]
    [ConverterTest(typeof(AllEqualsMultiConverter<string>), ConverterTestLayer.Binding)]
    public Task BindingGeneric(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCasesGeneric))]
    [ConverterTest(typeof(AllEqualsMultiConverter<string>), ConverterTestLayer.Xaml)]
    public Task XamlGeneric(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
            AvaloniaXamlRunner.Run(scenario.Create(), CompareConfigurationGeneric)
        );

    [TestMethod]
    [DynamicData(nameof(IntegrationCasesGeneric))]
    [ConverterTest(typeof(AllEqualsMultiConverter<string>), ConverterTestLayer.Properties)]
    public Task StyledPropertiesGeneric(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new AllEqualsMultiConverter<string>());
            var converter = (AllEqualsMultiConverter<string>)c.Create();
            var pristine = new AllEqualsMultiConverter<string>();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                AllEqualsMultiConverter<string>.ValueProperty,
                nameof(AllEqualsMultiConverter<string>.Value),
                x => x.Value,
                (x, value) => x.Value = value,
                "alternate-Value"
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                AllEqualsMultiConverter<string>.IsStringEqualsProperty,
                nameof(AllEqualsMultiConverter<string>.IsStringEquals),
                x => x.IsStringEquals,
                (x, value) => x.IsStringEquals = value,
                converter.IsStringEquals is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                AllEqualsMultiConverter<string>.IsInvertedProperty,
                nameof(AllEqualsMultiConverter<string>.IsInverted),
                x => x.IsInverted,
                (x, value) => x.IsInverted = value,
                converter.IsInverted is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                AllEqualsMultiConverter<string>.PreferredCultureProperty,
                nameof(AllEqualsMultiConverter<string>.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfigurationGeneric(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (AllEqualsMultiConverter<string>)configured;
        var actual = (AllEqualsMultiConverter<string>)loaded;
        ConverterAssertions.ConfigurationEqual(expected.Value, actual.Value);
        ConverterAssertions.ConfigurationEqual(expected.IsStringEquals, actual.IsStringEquals);
        ConverterAssertions.ConfigurationEqual(expected.IsInverted, actual.IsInverted);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
