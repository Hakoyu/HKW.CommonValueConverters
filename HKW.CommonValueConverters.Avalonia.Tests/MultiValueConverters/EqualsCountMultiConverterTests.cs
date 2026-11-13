using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class EqualsCountMultiConverterTests
{
    public static IEnumerable<object[]> SharedCases => EqualsCountMultiConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(EqualsCountMultiConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        MultiConversionCase<EqualsCountMultiConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static MultiConversionSubject<EqualsCountMultiConverterSuite.Configuration> CreateShared(
        EqualsCountMultiConverterSuite.Configuration configuration
    )
    {
        var converter = new EqualsCountMultiConverter();
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
        Assert.AreEqual(typeof(EqualsCountMultiConverter), subject.Instance.GetType());
        return subject;
    }

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(EqualsCountMultiConverter<string>), ConverterTestLayer.Behavior)]
    public Task SharedBehaviorGeneric(
        MultiConversionCase<EqualsCountMultiConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateSharedGeneric));

    private static MultiConversionSubject<EqualsCountMultiConverterSuite.Configuration> CreateSharedGeneric(
        EqualsCountMultiConverterSuite.Configuration configuration
    )
    {
        var converter = new EqualsCountMultiConverter<string>();
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
        Assert.AreEqual(typeof(EqualsCountMultiConverter<string>), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:EqualsCountMultiConverter", () =>
                    new ConverterCase(
                        "EqualsCountMultiConverter",
                        typeof(EqualsCountMultiConverter),
                        () => new EqualsCountMultiConverter { Value = "match" },
                        ["match", "other"],
                        1,
                        [new(0, "other", 0), new(1, "match", 1)],
                        null,
                        true
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(EqualsCountMultiConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(EqualsCountMultiConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(EqualsCountMultiConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(EqualsCountMultiConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new EqualsCountMultiConverter());
            var converter = (EqualsCountMultiConverter)c.Create();
            var pristine = new EqualsCountMultiConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                EqualsCountMultiConverter.ValueProperty,
                nameof(EqualsCountMultiConverter.Value),
                x => x.Value,
                (x, value) => x.Value = value,
                new object()
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                EqualsCountMultiConverter.IsStringEqualsProperty,
                nameof(EqualsCountMultiConverter.IsStringEquals),
                x => x.IsStringEquals,
                (x, value) => x.IsStringEquals = value,
                converter.IsStringEquals is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                EqualsCountMultiConverter.IsInvertedProperty,
                nameof(EqualsCountMultiConverter.IsInverted),
                x => x.IsInverted,
                (x, value) => x.IsInverted = value,
                converter.IsInverted is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                EqualsCountMultiConverter.PreferredCultureProperty,
                nameof(EqualsCountMultiConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (EqualsCountMultiConverter)configured;
        var actual = (EqualsCountMultiConverter)loaded;
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
                new AvaloniaIntegrationCase("integration:EqualsCountMultiConverter_String", () =>
                    new ConverterCase(
                        "EqualsCountMultiConverter_String",
                        typeof(EqualsCountMultiConverter<string>),
                        () => new EqualsCountMultiConverter<string> { Value = "match" },
                        ["match", "other"],
                        1,
                        [new(0, "other", 0), new(1, "match", 1)],
                        null,
                        true
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCasesGeneric))]
    [ConverterTest(typeof(EqualsCountMultiConverter<string>), ConverterTestLayer.Behavior)]
    public Task PlatformBehaviorGeneric(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCasesGeneric))]
    [ConverterTest(typeof(EqualsCountMultiConverter<string>), ConverterTestLayer.Binding)]
    public Task BindingGeneric(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCasesGeneric))]
    [ConverterTest(typeof(EqualsCountMultiConverter<string>), ConverterTestLayer.Xaml)]
    public Task XamlGeneric(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
            AvaloniaXamlRunner.Run(scenario.Create(), CompareConfigurationGeneric)
        );

    [TestMethod]
    [DynamicData(nameof(IntegrationCasesGeneric))]
    [ConverterTest(typeof(EqualsCountMultiConverter<string>), ConverterTestLayer.Properties)]
    public Task StyledPropertiesGeneric(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new EqualsCountMultiConverter<string>());
            var converter = (EqualsCountMultiConverter<string>)c.Create();
            var pristine = new EqualsCountMultiConverter<string>();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                EqualsCountMultiConverter<string>.ValueProperty,
                nameof(EqualsCountMultiConverter<string>.Value),
                x => x.Value,
                (x, value) => x.Value = value,
                "alternate-Value"
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                EqualsCountMultiConverter<string>.IsStringEqualsProperty,
                nameof(EqualsCountMultiConverter<string>.IsStringEquals),
                x => x.IsStringEquals,
                (x, value) => x.IsStringEquals = value,
                converter.IsStringEquals is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                EqualsCountMultiConverter<string>.IsInvertedProperty,
                nameof(EqualsCountMultiConverter<string>.IsInverted),
                x => x.IsInverted,
                (x, value) => x.IsInverted = value,
                converter.IsInverted is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                EqualsCountMultiConverter<string>.PreferredCultureProperty,
                nameof(EqualsCountMultiConverter<string>.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfigurationGeneric(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (EqualsCountMultiConverter<string>)configured;
        var actual = (EqualsCountMultiConverter<string>)loaded;
        ConverterAssertions.ConfigurationEqual(expected.Value, actual.Value);
        ConverterAssertions.ConfigurationEqual(expected.IsStringEquals, actual.IsStringEquals);
        ConverterAssertions.ConfigurationEqual(expected.IsInverted, actual.IsInverted);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
