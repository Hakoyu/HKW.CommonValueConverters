using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class EqualsToValueConverterTests
{
    [TestMethod]
    public Task NullableMappings_AndEqualityConfigurationAreHonored() =>
        PlatformThread.Run(() =>
        {
            var equality = new EqualsToValueConverter<string>
            {
                Other = 12,
                TrueValue = "yes",
                FalseValue = "no",
                NullValue = "nil",
            };
            Assert.AreEqual("no", equality.Convert("12", null, null, null));
            equality.IsStringEquals = true;
            Assert.AreEqual("yes", equality.Convert("12", null, null, null));
            Assert.AreEqual("nil", equality.Convert(null, null, null, null));
        });

    public static IEnumerable<object[]> SharedCases => EqualsToValueConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(EqualsToValueConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        SingleConversionCase<EqualsToValueConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static SingleConversionSubject<EqualsToValueConverterSuite.Configuration> CreateShared(
        EqualsToValueConverterSuite.Configuration configuration
    )
    {
        var converter = new EqualsToValueConverter();
        var subject = AvaloniaConversionAdapter.Single(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetOther is not null)
                converter.Other = settings.GetOther();
            if (settings.GetTrueValue is not null)
                converter.TrueValue = settings.GetTrueValue();
            if (settings.GetFalseValue is not null)
                converter.FalseValue = settings.GetFalseValue();
            if (settings.GetNullValue is not null)
                converter.NullValue = settings.GetNullValue();
            if (settings.GetIsStringEquals is not null)
                converter.IsStringEquals = settings.GetIsStringEquals();
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(EqualsToValueConverter), subject.Instance.GetType());
        return subject;
    }

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(EqualsToValueConverter<string>), ConverterTestLayer.Behavior)]
    public Task SharedBehaviorGeneric(
        SingleConversionCase<EqualsToValueConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateSharedGeneric));

    private static SingleConversionSubject<EqualsToValueConverterSuite.Configuration> CreateSharedGeneric(
        EqualsToValueConverterSuite.Configuration configuration
    )
    {
        var converter = new EqualsToValueConverter<string>();
        var subject = AvaloniaConversionAdapter.Single(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetOther is not null)
                converter.Other = settings.GetOther();
            if (settings.GetTrueValue is not null)
                converter.TrueValue = settings.GetTrueValue();
            if (settings.GetFalseValue is not null)
                converter.FalseValue = settings.GetFalseValue();
            if (settings.GetNullValue is not null)
                converter.NullValue = settings.GetNullValue();
            if (settings.GetIsStringEquals is not null)
                converter.IsStringEquals = settings.GetIsStringEquals();
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(EqualsToValueConverter<string>), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:EqualsToValueConverter", () =>
                    new ConverterCase(
                        "EqualsToValueConverter",
                        typeof(EqualsToValueConverter),
                        () =>
                            new EqualsToValueConverter
                            {
                                Other = "match",
                                TrueValue = "yes",
                                FalseValue = "no",
                                NullValue = "nil",
                            },
                        ["match"],
                        "yes",
                        [new(0, "other", "no")],
                        null,
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(EqualsToValueConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, false);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(EqualsToValueConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(EqualsToValueConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(EqualsToValueConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new EqualsToValueConverter());
            var converter = (EqualsToValueConverter)c.Create();
            var pristine = new EqualsToValueConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                EqualsToValueConverter.OtherProperty,
                nameof(EqualsToValueConverter.Other),
                x => x.Other,
                (x, value) => x.Other = value,
                new object()
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                EqualsToValueConverter.TrueValueProperty,
                nameof(EqualsToValueConverter.TrueValue),
                x => x.TrueValue,
                (x, value) => x.TrueValue = value,
                new object()
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                EqualsToValueConverter.FalseValueProperty,
                nameof(EqualsToValueConverter.FalseValue),
                x => x.FalseValue,
                (x, value) => x.FalseValue = value,
                new object()
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                EqualsToValueConverter.NullValueProperty,
                nameof(EqualsToValueConverter.NullValue),
                x => x.NullValue,
                (x, value) => x.NullValue = value,
                new object()
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                EqualsToValueConverter.IsStringEqualsProperty,
                nameof(EqualsToValueConverter.IsStringEquals),
                x => x.IsStringEquals,
                (x, value) => x.IsStringEquals = value,
                converter.IsStringEquals is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                EqualsToValueConverter.PreferredCultureProperty,
                nameof(EqualsToValueConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (EqualsToValueConverter)configured;
        var actual = (EqualsToValueConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.Other, actual.Other);
        ConverterAssertions.ConfigurationEqual(expected.TrueValue, actual.TrueValue);
        ConverterAssertions.ConfigurationEqual(expected.FalseValue, actual.FalseValue);
        ConverterAssertions.ConfigurationEqual(expected.NullValue, actual.NullValue);
        ConverterAssertions.ConfigurationEqual(expected.IsStringEquals, actual.IsStringEquals);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }

    public static IEnumerable<object[]> IntegrationCasesGeneric
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:EqualsToValueConverter_String", () =>
                    new ConverterCase(
                        "EqualsToValueConverter_String",
                        typeof(EqualsToValueConverter<string>),
                        () =>
                            new EqualsToValueConverter<string>
                            {
                                Other = "match",
                                TrueValue = "yes",
                                FalseValue = "no",
                                NullValue = "nil",
                            },
                        ["match"],
                        "yes",
                        [new(0, "other", "no")],
                        null,
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCasesGeneric))]
    [ConverterTest(typeof(EqualsToValueConverter<string>), ConverterTestLayer.Behavior)]
    public Task PlatformBehaviorGeneric(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, false);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCasesGeneric))]
    [ConverterTest(typeof(EqualsToValueConverter<string>), ConverterTestLayer.Binding)]
    public Task BindingGeneric(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCasesGeneric))]
    [ConverterTest(typeof(EqualsToValueConverter<string>), ConverterTestLayer.Xaml)]
    public Task XamlGeneric(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
            AvaloniaXamlRunner.Run(scenario.Create(), CompareConfigurationGeneric)
        );

    [TestMethod]
    [DynamicData(nameof(IntegrationCasesGeneric))]
    [ConverterTest(typeof(EqualsToValueConverter<string>), ConverterTestLayer.Properties)]
    public Task StyledPropertiesGeneric(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new EqualsToValueConverter<string>());
            var converter = (EqualsToValueConverter<string>)c.Create();
            var pristine = new EqualsToValueConverter<string>();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                EqualsToValueConverter<string>.OtherProperty,
                nameof(EqualsToValueConverter<string>.Other),
                x => x.Other,
                (x, value) => x.Other = value,
                new object()
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                EqualsToValueConverter<string>.TrueValueProperty,
                nameof(EqualsToValueConverter<string>.TrueValue),
                x => x.TrueValue,
                (x, value) => x.TrueValue = value,
                "alternate-TrueValue"
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                EqualsToValueConverter<string>.FalseValueProperty,
                nameof(EqualsToValueConverter<string>.FalseValue),
                x => x.FalseValue,
                (x, value) => x.FalseValue = value,
                "alternate-FalseValue"
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                EqualsToValueConverter<string>.NullValueProperty,
                nameof(EqualsToValueConverter<string>.NullValue),
                x => x.NullValue,
                (x, value) => x.NullValue = value,
                "alternate-NullValue"
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                EqualsToValueConverter<string>.IsStringEqualsProperty,
                nameof(EqualsToValueConverter<string>.IsStringEquals),
                x => x.IsStringEquals,
                (x, value) => x.IsStringEquals = value,
                converter.IsStringEquals is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                EqualsToValueConverter<string>.PreferredCultureProperty,
                nameof(EqualsToValueConverter<string>.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfigurationGeneric(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (EqualsToValueConverter<string>)configured;
        var actual = (EqualsToValueConverter<string>)loaded;
        ConverterAssertions.ConfigurationEqual(expected.Other, actual.Other);
        ConverterAssertions.ConfigurationEqual(expected.TrueValue, actual.TrueValue);
        ConverterAssertions.ConfigurationEqual(expected.FalseValue, actual.FalseValue);
        ConverterAssertions.ConfigurationEqual(expected.NullValue, actual.NullValue);
        ConverterAssertions.ConfigurationEqual(expected.IsStringEquals, actual.IsStringEquals);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
