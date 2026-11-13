using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class BoolToValueConverterTests
{
    [TestMethod]
    public Task UnsetFallback_DoNothingPreservesTarget_NullRemainsNull() =>
        PlatformThread.Run(() =>
        {
            var source = new TypedSource<bool?>(true);
            var converter = new BoolToValueConverter
            {
                TrueValue = "initial",
                FalseValue = BindingOperations.DoNothing,
                IsNullable = true,
                NullValue = null,
            };
            var target = new TextBox();
            using var subscription = target.Bind(
                TextBox.TextProperty,
                new Binding(nameof(source.Value))
                {
                    Source = source,
                    Converter = converter,
                    Mode = BindingMode.OneWay,
                    FallbackValue = "fallback",
                }
            );
            Assert.AreEqual("initial", target.Text);
            source.Value = false;
            PlatformThread.Drain();
            Assert.AreEqual("initial", target.Text);
            source.Value = null;
            PlatformThread.Drain();
            Assert.IsNull(target.Text);
            converter.TrueValue = AvaloniaProperty.UnsetValue;
            source.Value = true;
            PlatformThread.Drain();
            Assert.AreEqual("fallback", target.Text);
        });

    [TestMethod]
    public Task MappedClrSentinels_DifferFromNativePropertyControlValues() =>
        PlatformThread.Run(() =>
        {
            var converter = new BoolToValueConverter { TrueValue = "original" };
            converter.TrueValue = AvaloniaProperty.UnsetValue;
            Assert.AreSame(AvaloniaProperty.UnsetValue, converter.Convert(true, null, null, null));
            Assert.AreSame(
                Core.ConverterResults.UnsetValue,
                converter.GetValue(BoolToValueConverter<object?>.TrueValueProperty)
            );
            converter.TrueValue = BindingOperations.DoNothing;
            Assert.AreSame(BindingOperations.DoNothing, converter.Convert(true, null, null, null));
            Assert.AreSame(
                Core.ConverterResults.DoNothing,
                converter.GetValue(BoolToValueConverter<object?>.TrueValueProperty)
            );
            converter.SetValue(
                BoolToValueConverter<object?>.TrueValueProperty,
                AvaloniaProperty.UnsetValue
            );
            Assert.IsNull(converter.TrueValue);
            converter.TrueValue = "keep";
            converter.SetValue(
                BoolToValueConverter<object?>.TrueValueProperty,
                BindingOperations.DoNothing
            );
            Assert.AreEqual("keep", converter.TrueValue);
            converter.TrueValue = null;
            Assert.IsNull(converter.Convert(true, null, null, null));
        });

    [TestMethod]
    public Task NullableMappings_AndEqualityConfigurationAreHonored() =>
        PlatformThread.Run(() =>
        {
            var boolean = new BoolToValueConverter<string>
            {
                TrueValue = "yes",
                FalseValue = "no",
                NullValue = "nil",
            };
            Assert.AreEqual("no", boolean.Convert(null, null, null, null));
            boolean.IsNullable = true;
            Assert.AreEqual("nil", boolean.Convert(null, null, null, null));
            boolean.IsInverted = true;
            Assert.AreEqual("no", boolean.Convert(true, null, null, null));
        });

    public static IEnumerable<object[]> SharedCases => BoolToValueConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(BoolToValueConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        SingleConversionCase<BoolToValueConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static SingleConversionSubject<BoolToValueConverterSuite.Configuration> CreateShared(
        BoolToValueConverterSuite.Configuration configuration
    )
    {
        var converter = new BoolToValueConverter();
        var subject = AvaloniaConversionAdapter.Single(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetTrueValue is not null)
                converter.TrueValue = settings.GetTrueValue();
            if (settings.GetFalseValue is not null)
                converter.FalseValue = settings.GetFalseValue();
            if (settings.GetIsNullable is not null)
                converter.IsNullable = settings.GetIsNullable();
            if (settings.GetNullValue is not null)
                converter.NullValue = settings.GetNullValue();
            if (settings.GetIsInverted is not null)
                converter.IsInverted = settings.GetIsInverted();
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(BoolToValueConverter), subject.Instance.GetType());
        return subject;
    }

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(BoolToValueConverter<string>), ConverterTestLayer.Behavior)]
    public Task SharedBehaviorGeneric(
        SingleConversionCase<BoolToValueConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateSharedGeneric));

    private static SingleConversionSubject<BoolToValueConverterSuite.Configuration> CreateSharedGeneric(
        BoolToValueConverterSuite.Configuration configuration
    )
    {
        var converter = new BoolToValueConverter<string>();
        var subject = AvaloniaConversionAdapter.Single(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetTrueValue is not null)
                converter.TrueValue = settings.GetTrueValue();
            if (settings.GetFalseValue is not null)
                converter.FalseValue = settings.GetFalseValue();
            if (settings.GetIsNullable is not null)
                converter.IsNullable = settings.GetIsNullable();
            if (settings.GetNullValue is not null)
                converter.NullValue = settings.GetNullValue();
            if (settings.GetIsInverted is not null)
                converter.IsInverted = settings.GetIsInverted();
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(BoolToValueConverter<string>), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:BoolToValueConverter", () =>
                    new ConverterCase(
                        "BoolToValueConverter",
                        typeof(BoolToValueConverter),
                        () =>
                            new BoolToValueConverter
                            {
                                TrueValue = "yes",
                                FalseValue = "no",
                                IsNullable = true,
                                NullValue = "nil",
                            },
                        [true],
                        "yes",
                        [new(0, false, "no")],
                        null,
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToValueConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, false);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToValueConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToValueConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(BoolToValueConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new BoolToValueConverter());
            var converter = (BoolToValueConverter)c.Create();
            var pristine = new BoolToValueConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToValueConverter.TrueValueProperty,
                nameof(BoolToValueConverter.TrueValue),
                x => x.TrueValue,
                (x, value) => x.TrueValue = value,
                new object()
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToValueConverter.FalseValueProperty,
                nameof(BoolToValueConverter.FalseValue),
                x => x.FalseValue,
                (x, value) => x.FalseValue = value,
                new object()
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToValueConverter.IsNullableProperty,
                nameof(BoolToValueConverter.IsNullable),
                x => x.IsNullable,
                (x, value) => x.IsNullable = value,
                converter.IsNullable is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToValueConverter.NullValueProperty,
                nameof(BoolToValueConverter.NullValue),
                x => x.NullValue,
                (x, value) => x.NullValue = value,
                new object()
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToValueConverter.IsInvertedProperty,
                nameof(BoolToValueConverter.IsInverted),
                x => x.IsInverted,
                (x, value) => x.IsInverted = value,
                converter.IsInverted is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToValueConverter.PreferredCultureProperty,
                nameof(BoolToValueConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (BoolToValueConverter)configured;
        var actual = (BoolToValueConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.TrueValue, actual.TrueValue);
        ConverterAssertions.ConfigurationEqual(expected.FalseValue, actual.FalseValue);
        ConverterAssertions.ConfigurationEqual(expected.IsNullable, actual.IsNullable);
        ConverterAssertions.ConfigurationEqual(expected.NullValue, actual.NullValue);
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
                new AvaloniaIntegrationCase("integration:BoolToValueConverter_String", () =>
                    new ConverterCase(
                        "BoolToValueConverter_String",
                        typeof(BoolToValueConverter<string>),
                        () =>
                            new BoolToValueConverter<string>
                            {
                                TrueValue = "yes",
                                FalseValue = "no",
                                IsNullable = true,
                                NullValue = "nil",
                            },
                        [true],
                        "yes",
                        [new(0, false, "no")],
                        null,
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCasesGeneric))]
    [ConverterTest(typeof(BoolToValueConverter<string>), ConverterTestLayer.Behavior)]
    public Task PlatformBehaviorGeneric(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, false);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCasesGeneric))]
    [ConverterTest(typeof(BoolToValueConverter<string>), ConverterTestLayer.Binding)]
    public Task BindingGeneric(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCasesGeneric))]
    [ConverterTest(typeof(BoolToValueConverter<string>), ConverterTestLayer.Xaml)]
    public Task XamlGeneric(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
            AvaloniaXamlRunner.Run(scenario.Create(), CompareConfigurationGeneric)
        );

    [TestMethod]
    [DynamicData(nameof(IntegrationCasesGeneric))]
    [ConverterTest(typeof(BoolToValueConverter<string>), ConverterTestLayer.Properties)]
    public Task StyledPropertiesGeneric(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new BoolToValueConverter<string>());
            var converter = (BoolToValueConverter<string>)c.Create();
            var pristine = new BoolToValueConverter<string>();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToValueConverter<string>.TrueValueProperty,
                nameof(BoolToValueConverter<string>.TrueValue),
                x => x.TrueValue,
                (x, value) => x.TrueValue = value,
                "alternate-TrueValue"
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToValueConverter<string>.FalseValueProperty,
                nameof(BoolToValueConverter<string>.FalseValue),
                x => x.FalseValue,
                (x, value) => x.FalseValue = value,
                "alternate-FalseValue"
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToValueConverter<string>.IsNullableProperty,
                nameof(BoolToValueConverter<string>.IsNullable),
                x => x.IsNullable,
                (x, value) => x.IsNullable = value,
                converter.IsNullable is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToValueConverter<string>.NullValueProperty,
                nameof(BoolToValueConverter<string>.NullValue),
                x => x.NullValue,
                (x, value) => x.NullValue = value,
                "alternate-NullValue"
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToValueConverter<string>.IsInvertedProperty,
                nameof(BoolToValueConverter<string>.IsInverted),
                x => x.IsInverted,
                (x, value) => x.IsInverted = value,
                converter.IsInverted is false
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                BoolToValueConverter<string>.PreferredCultureProperty,
                nameof(BoolToValueConverter<string>.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfigurationGeneric(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (BoolToValueConverter<string>)configured;
        var actual = (BoolToValueConverter<string>)loaded;
        ConverterAssertions.ConfigurationEqual(expected.TrueValue, actual.TrueValue);
        ConverterAssertions.ConfigurationEqual(expected.FalseValue, actual.FalseValue);
        ConverterAssertions.ConfigurationEqual(expected.IsNullable, actual.IsNullable);
        ConverterAssertions.ConfigurationEqual(expected.NullValue, actual.NullValue);
        ConverterAssertions.ConfigurationEqual(expected.IsInverted, actual.IsInverted);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }

    [TestMethod]
    public Task IntegerNativeWritesAndClearChangeConversion() =>
        PlatformThread.Run(() =>
        {
            var converter = new BoolToValueConverter<int> { TrueValue = 1, FalseValue = 2 };
            converter.SetValue(BoolToValueConverter<int>.TrueValueProperty, 7);
            Assert.AreEqual(7, converter.Convert(true, null, null, null));
            converter.ClearValue(BoolToValueConverter<int>.TrueValueProperty);
            Assert.AreEqual(0, converter.Convert(true, null, null, null));
            converter.IsInverted = true;
            Assert.AreEqual(2, converter.Convert(true, null, null, null));
        });
}
