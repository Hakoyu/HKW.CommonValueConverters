using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class AnyEqualsMultiConverterTests
{
    public static IEnumerable<object[]> SharedCases => AnyEqualsMultiConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(AnyEqualsMultiConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        MultiConversionCase<AnyEqualsMultiConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static MultiConversionSubject<AnyEqualsMultiConverterSuite.Configuration> CreateShared(
        AnyEqualsMultiConverterSuite.Configuration configuration
    )
    {
        var converter = new AnyEqualsMultiConverter();
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
        Assert.AreEqual(typeof(AnyEqualsMultiConverter), subject.Instance.GetType());
        return subject;
    }

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(AnyEqualsMultiConverter<string>), ConverterTestLayer.Behavior)]
    public Task SharedBehaviorGeneric(
        MultiConversionCase<AnyEqualsMultiConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateSharedGeneric));

    private static MultiConversionSubject<AnyEqualsMultiConverterSuite.Configuration> CreateSharedGeneric(
        AnyEqualsMultiConverterSuite.Configuration configuration
    )
    {
        var converter = new AnyEqualsMultiConverter<string>();
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
        Assert.AreEqual(typeof(AnyEqualsMultiConverter<string>), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:AnyEqualsMultiConverter", () =>
                    new ConverterCase(
                        "AnyEqualsMultiConverter",
                        typeof(AnyEqualsMultiConverter),
                        () => new AnyEqualsMultiConverter { Value = "match" },
                        ["match", "other"],
                        true,
                        [new(0, "other", false), new(1, "match", true)],
                        null,
                        true
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(AnyEqualsMultiConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(AnyEqualsMultiConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(AnyEqualsMultiConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(AnyEqualsMultiConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new AnyEqualsMultiConverter());
            var converter = (AnyEqualsMultiConverter)c.Create();
            var pristine = new AnyEqualsMultiConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                AnyEqualsMultiConverter.ValueProperty,
                nameof(AnyEqualsMultiConverter.Value),
                x => x.Value,
                (x, value) => x.Value = value,
                new object()
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                AnyEqualsMultiConverter.IsStringEqualsProperty,
                nameof(AnyEqualsMultiConverter.IsStringEquals),
                x => x.IsStringEquals,
                (x, value) => x.IsStringEquals = value,
                converter.IsStringEquals is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                AnyEqualsMultiConverter.IsInvertedProperty,
                nameof(AnyEqualsMultiConverter.IsInverted),
                x => x.IsInverted,
                (x, value) => x.IsInverted = value,
                converter.IsInverted is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                AnyEqualsMultiConverter.PreferredCultureProperty,
                nameof(AnyEqualsMultiConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (AnyEqualsMultiConverter)configured;
        var actual = (AnyEqualsMultiConverter)loaded;
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
                new AvaloniaIntegrationCase("integration:AnyEqualsMultiConverter_String", () =>
                    new ConverterCase(
                        "AnyEqualsMultiConverter_String",
                        typeof(AnyEqualsMultiConverter<string>),
                        () => new AnyEqualsMultiConverter<string> { Value = "match" },
                        ["match", "other"],
                        true,
                        [new(0, "other", false), new(1, "match", true)],
                        null,
                        true
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCasesGeneric))]
    [ConverterTest(typeof(AnyEqualsMultiConverter<string>), ConverterTestLayer.Behavior)]
    public Task PlatformBehaviorGeneric(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCasesGeneric))]
    [ConverterTest(typeof(AnyEqualsMultiConverter<string>), ConverterTestLayer.Binding)]
    public Task BindingGeneric(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCasesGeneric))]
    [ConverterTest(typeof(AnyEqualsMultiConverter<string>), ConverterTestLayer.Xaml)]
    public Task XamlGeneric(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
            AvaloniaXamlRunner.Run(scenario.Create(), CompareConfigurationGeneric)
        );

    [TestMethod]
    [DynamicData(nameof(IntegrationCasesGeneric))]
    [ConverterTest(typeof(AnyEqualsMultiConverter<string>), ConverterTestLayer.Properties)]
    public Task StyledPropertiesGeneric(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new AnyEqualsMultiConverter<string>());
            var converter = (AnyEqualsMultiConverter<string>)c.Create();
            var pristine = new AnyEqualsMultiConverter<string>();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                AnyEqualsMultiConverter<string>.ValueProperty,
                nameof(AnyEqualsMultiConverter<string>.Value),
                x => x.Value,
                (x, value) => x.Value = value,
                "alternate-Value"
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                AnyEqualsMultiConverter<string>.IsStringEqualsProperty,
                nameof(AnyEqualsMultiConverter<string>.IsStringEquals),
                x => x.IsStringEquals,
                (x, value) => x.IsStringEquals = value,
                converter.IsStringEquals is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                AnyEqualsMultiConverter<string>.IsInvertedProperty,
                nameof(AnyEqualsMultiConverter<string>.IsInverted),
                x => x.IsInverted,
                (x, value) => x.IsInverted = value,
                converter.IsInverted is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                AnyEqualsMultiConverter<string>.PreferredCultureProperty,
                nameof(AnyEqualsMultiConverter<string>.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfigurationGeneric(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (AnyEqualsMultiConverter<string>)configured;
        var actual = (AnyEqualsMultiConverter<string>)loaded;
        ConverterAssertions.ConfigurationEqual(expected.Value, actual.Value);
        ConverterAssertions.ConfigurationEqual(expected.IsStringEquals, actual.IsStringEquals);
        ConverterAssertions.ConfigurationEqual(expected.IsInverted, actual.IsInverted);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
