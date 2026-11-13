using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class AllBoolToValueMultiConverterTests
{
    [TestMethod]
    public Task MultiBinding_DefaultTargetNullValueUsesFallback() =>
        PlatformThread.Run(() =>
        {
            var source = new TestViewModel(null, null);
            var converter = new AllBoolToValueMultiConverter { NullValue = null };
            Assert.IsNull(converter.Convert([null, null], typeof(string), null, null));
            var target = new TextBlock { DataContext = source };
            var binding = new MultiBinding { Converter = converter, FallbackValue = "fallback" };
            binding.Bindings.Add(new Binding("V0"));
            binding.Bindings.Add(new Binding("V1"));
            using var subscription = target.Bind(TextBlock.TextProperty, binding);
            // Avalonia 12 的默认 TargetNullValue 为 UnsetValue, 因此 null 继续采用 FallbackValue.
            Assert.AreEqual("fallback", target.Text);
        });

    [TestMethod]
    public Task MultiBinding_UnsetFallbackDoNothingAndNullAreDistinct() =>
        PlatformThread.Run(() =>
        {
            var source = new TestViewModel(true, true);
            var converter = new AllBoolToValueMultiConverter
            {
                TrueValue = "yes",
                FalseValue = "no",
                NullValue = null,
            };
            var target = new TextBlock { DataContext = source };
            var binding = new MultiBinding
            {
                Converter = converter,
                FallbackValue = "fallback",
                TargetNullValue = null!,
            };
            binding.Bindings.Add(new Binding("V0"));
            binding.Bindings.Add(new Binding("V1"));
            using var subscription = target.Bind(TextBlock.TextProperty, binding);
            Assert.AreEqual("yes", target.Text);
            source.V0 = false;
            PlatformThread.Drain();
            Assert.AreEqual("fallback", target.Text);
            source.V0 = true;
            PlatformThread.Drain();
            Assert.AreEqual("yes", target.Text);
            converter.DefaultResult = BindingOperations.DoNothing;
            source.V0 = false;
            PlatformThread.Drain();
            Assert.AreEqual("yes", target.Text);
            source.V0 = null;
            source.V1 = null;
            PlatformThread.Drain();
            Assert.IsNull(target.Text);
        });

    [TestMethod]
    public Task NullableMappings_AndEqualityConfigurationAreHonored() =>
        PlatformThread.Run(() =>
        {
            var all = new AllBoolToValueMultiConverter<string>
            {
                TrueValue = "yes",
                FalseValue = "no",
                NullValue = "nil",
                DefaultResult = "mixed",
            };
            Assert.AreEqual("nil", all.Convert([null, null], null, null, null));
            Assert.AreEqual("mixed", all.Convert([true, false], null, null, null));
            Assert.AreEqual("mixed", all.Convert([AvaloniaProperty.UnsetValue], null, null, null));
        });

    public static IEnumerable<object[]> SharedCases => AllBoolToValueMultiConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(AllBoolToValueMultiConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        MultiConversionCase<AllBoolToValueMultiConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static MultiConversionSubject<AllBoolToValueMultiConverterSuite.Configuration> CreateShared(
        AllBoolToValueMultiConverterSuite.Configuration configuration
    )
    {
        var converter = new AllBoolToValueMultiConverter();
        var subject = AvaloniaConversionAdapter.Multi(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetTrueValue is not null)
                converter.TrueValue = settings.GetTrueValue();
            if (settings.GetFalseValue is not null)
                converter.FalseValue = settings.GetFalseValue();
            if (settings.GetNullValue is not null)
                converter.NullValue = settings.GetNullValue();
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(AllBoolToValueMultiConverter), subject.Instance.GetType());
        return subject;
    }

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(AllBoolToValueMultiConverter<string>), ConverterTestLayer.Behavior)]
    public Task SharedBehaviorGeneric(
        MultiConversionCase<AllBoolToValueMultiConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateSharedGeneric));

    private static MultiConversionSubject<AllBoolToValueMultiConverterSuite.Configuration> CreateSharedGeneric(
        AllBoolToValueMultiConverterSuite.Configuration configuration
    )
    {
        var converter = new AllBoolToValueMultiConverter<string>();
        var subject = AvaloniaConversionAdapter.Multi(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetTrueValue is not null)
                converter.TrueValue = settings.GetTrueValue();
            if (settings.GetFalseValue is not null)
                converter.FalseValue = settings.GetFalseValue();
            if (settings.GetNullValue is not null)
                converter.NullValue = settings.GetNullValue();
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(AllBoolToValueMultiConverter<string>), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:AllBoolToValueMultiConverter", () =>
                    new ConverterCase(
                        "AllBoolToValueMultiConverter",
                        typeof(AllBoolToValueMultiConverter),
                        () =>
                            new AllBoolToValueMultiConverter
                            {
                                TrueValue = "yes",
                                FalseValue = "no",
                                NullValue = "nil",
                                DefaultResult = "mixed",
                            },
                        [true, true],
                        "yes",
                        [new(0, false, "mixed"), new(1, false, "no")],
                        null,
                        true
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(AllBoolToValueMultiConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(AllBoolToValueMultiConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(AllBoolToValueMultiConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(AllBoolToValueMultiConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new AllBoolToValueMultiConverter());
            var converter = (AllBoolToValueMultiConverter)c.Create();
            var pristine = new AllBoolToValueMultiConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                AllBoolToValueMultiConverter.TrueValueProperty,
                nameof(AllBoolToValueMultiConverter.TrueValue),
                x => x.TrueValue,
                (x, value) => x.TrueValue = value,
                new object()
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                AllBoolToValueMultiConverter.FalseValueProperty,
                nameof(AllBoolToValueMultiConverter.FalseValue),
                x => x.FalseValue,
                (x, value) => x.FalseValue = value,
                new object()
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                AllBoolToValueMultiConverter.NullValueProperty,
                nameof(AllBoolToValueMultiConverter.NullValue),
                x => x.NullValue,
                (x, value) => x.NullValue = value,
                new object()
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                AllBoolToValueMultiConverter.PreferredCultureProperty,
                nameof(AllBoolToValueMultiConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (AllBoolToValueMultiConverter)configured;
        var actual = (AllBoolToValueMultiConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.TrueValue, actual.TrueValue);
        ConverterAssertions.ConfigurationEqual(expected.FalseValue, actual.FalseValue);
        ConverterAssertions.ConfigurationEqual(expected.NullValue, actual.NullValue);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }

    public static IEnumerable<object[]> IntegrationCasesGeneric
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:AllBoolToValueMultiConverter_String", () =>
                    new ConverterCase(
                        "AllBoolToValueMultiConverter_String",
                        typeof(AllBoolToValueMultiConverter<string>),
                        () =>
                            new AllBoolToValueMultiConverter<string>
                            {
                                TrueValue = "yes",
                                FalseValue = "no",
                                NullValue = "nil",
                                DefaultResult = "mixed",
                            },
                        [true, true],
                        "yes",
                        [new(0, false, "mixed"), new(1, false, "no")],
                        null,
                        true
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCasesGeneric))]
    [ConverterTest(typeof(AllBoolToValueMultiConverter<string>), ConverterTestLayer.Behavior)]
    public Task PlatformBehaviorGeneric(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCasesGeneric))]
    [ConverterTest(typeof(AllBoolToValueMultiConverter<string>), ConverterTestLayer.Binding)]
    public Task BindingGeneric(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCasesGeneric))]
    [ConverterTest(typeof(AllBoolToValueMultiConverter<string>), ConverterTestLayer.Xaml)]
    public Task XamlGeneric(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
            AvaloniaXamlRunner.Run(scenario.Create(), CompareConfigurationGeneric)
        );

    [TestMethod]
    [DynamicData(nameof(IntegrationCasesGeneric))]
    [ConverterTest(typeof(AllBoolToValueMultiConverter<string>), ConverterTestLayer.Properties)]
    public Task StyledPropertiesGeneric(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () =>
                new AllBoolToValueMultiConverter<string>()
            );
            var converter = (AllBoolToValueMultiConverter<string>)c.Create();
            var pristine = new AllBoolToValueMultiConverter<string>();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                AllBoolToValueMultiConverter<string>.TrueValueProperty,
                nameof(AllBoolToValueMultiConverter<string>.TrueValue),
                x => x.TrueValue,
                (x, value) => x.TrueValue = value,
                "alternate-TrueValue"
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                AllBoolToValueMultiConverter<string>.FalseValueProperty,
                nameof(AllBoolToValueMultiConverter<string>.FalseValue),
                x => x.FalseValue,
                (x, value) => x.FalseValue = value,
                "alternate-FalseValue"
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                AllBoolToValueMultiConverter<string>.NullValueProperty,
                nameof(AllBoolToValueMultiConverter<string>.NullValue),
                x => x.NullValue,
                (x, value) => x.NullValue = value,
                "alternate-NullValue"
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                AllBoolToValueMultiConverter<string>.PreferredCultureProperty,
                nameof(AllBoolToValueMultiConverter<string>.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfigurationGeneric(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (AllBoolToValueMultiConverter<string>)configured;
        var actual = (AllBoolToValueMultiConverter<string>)loaded;
        ConverterAssertions.ConfigurationEqual(expected.TrueValue, actual.TrueValue);
        ConverterAssertions.ConfigurationEqual(expected.FalseValue, actual.FalseValue);
        ConverterAssertions.ConfigurationEqual(expected.NullValue, actual.NullValue);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }
}
