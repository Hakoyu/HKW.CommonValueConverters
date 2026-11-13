using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using HKW.HKWUtils;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class StringToNumberConverterTests
{
    [TestMethod]
    public Task StringToNumberConverter_TwoWayNumericUpDown() =>
        PlatformThread.Run(() =>
            AvaloniaTypedBindingRunner.VerifyTwoWay(
                "12.5",
                "27.5",
                new NumericUpDown(),
                NumericUpDown.ValueProperty,
                new StringToNumberConverter { NumberType = NumberType.Decimal },
                12.5m,
                (decimal?)27.5m
            )
        );

    [TestMethod]
    public Task NumericBinding_UsesExplicitCultureAndUpdates() =>
        PlatformThread.Run(() =>
        {
            using var errors = new BindingErrorScope();
            var saved = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
                var converter = new StringToNumberConverter
                {
                    NumberType = NumberType.Decimal,
                    PreferredCulture = Core.PreferredCulture.ConverterCulture,
                };
                var source = new TypedSource<string>("12,5");
                var target = new NumericUpDown();
                using var subscription = target.Bind(
                    NumericUpDown.ValueProperty,
                    new Binding("Value")
                    {
                        Source = source,
                        Converter = converter,
                        ConverterCulture = CultureInfo.GetCultureInfo("fr-FR"),
                        Mode = BindingMode.TwoWay,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    }
                );
                Assert.AreEqual(12.5m, target.Value);
                source.Value = "23,5";
                PlatformThread.Drain();
                Assert.AreEqual(23.5m, target.Value);
                target.SetCurrentValue(NumericUpDown.ValueProperty, 34.5m);
                PlatformThread.Drain();
                Assert.AreEqual("34,5", source.Value);
            }
            finally
            {
                CultureInfo.CurrentCulture = saved;
            }
        });

    [TestMethod]
    public Task SingleAndMultiBoundaries_RejectSentinelsWithoutMutatingInputs() =>
        PlatformThread.Run(() =>
        {
            var marker = new object();
            var numeric = new StringToNumberConverter { DefaultResult = marker };
            foreach (
                var invalid in new[]
                {
                    AvaloniaProperty.UnsetValue,
                    BindingOperations.DoNothing,
                    Core.ConverterResults.UnsetValue,
                    Core.ConverterResults.DoNothing,
                    "invalid",
                    null,
                }
            )
                Assert.AreSame(marker, numeric.Convert(invalid, null, null, null));
        });

    public static IEnumerable<object[]> SharedCases => StringToNumberConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(SharedCases))]
    [ConverterTest(typeof(StringToNumberConverter), ConverterTestLayer.Behavior)]
    public Task SharedBehavior(
        SingleConversionCase<StringToNumberConverterSuite.Configuration> scenario
    ) => PlatformThread.Run(() => scenario.Run(CreateShared));

    private static SingleConversionSubject<StringToNumberConverterSuite.Configuration> CreateShared(
        StringToNumberConverterSuite.Configuration configuration
    )
    {
        var converter = new StringToNumberConverter();
        var subject = AvaloniaConversionAdapter.Single(configuration, converter, settings =>
        {
            if (settings.PreferredCulture is { } culture)
                converter.PreferredCulture = culture;
            if (settings.GetNumberStyles is not null)
                converter.NumberStyles = settings.GetNumberStyles();
            if (settings.GetNumberType is not null)
                converter.NumberType = settings.GetNumberType();
            if (settings.GetDefaultResult is not null)
                converter.DefaultResult = settings.GetDefaultResult();
        });
        Assert.AreEqual(typeof(StringToNumberConverter), subject.Instance.GetType());
        return subject;
    }

    public static IEnumerable<object[]> IntegrationCases
    {
        get
        {
            yield return
            [
                new AvaloniaIntegrationCase("integration:StringToNumberConverter", () =>
                    new ConverterCase(
                        "StringToNumberConverter",
                        typeof(StringToNumberConverter),
                        () => new StringToNumberConverter(),
                        ["12"],
                        12,
                        [new(0, "34", 34)],
                        null,
                        false
                    )
                ),
            ];
        }
    }

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(StringToNumberConverter), ConverterTestLayer.Behavior)]
    public Task PlatformBehavior(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            ConverterAssertions.Behavior(c);
            ConverterAssertions.Reverse(c, true);
        });

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(StringToNumberConverter), ConverterTestLayer.Binding)]
    public Task Binding(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaBindingRunner.Run(scenario.Create()));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(StringToNumberConverter), ConverterTestLayer.Xaml)]
    public Task Xaml(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() => AvaloniaXamlRunner.Run(scenario.Create(), CompareConfiguration));

    [TestMethod]
    [DynamicData(nameof(IntegrationCases))]
    [ConverterTest(typeof(StringToNumberConverter), ConverterTestLayer.Properties)]
    public Task StyledProperties(AvaloniaIntegrationCase scenario) =>
        PlatformThread.Run(() =>
        {
            var c = scenario.Create();
            AvaloniaPropertyAssertions.Defaults(c, () => new StringToNumberConverter());
            var converter = (StringToNumberConverter)c.Create();
            var pristine = new StringToNumberConverter();
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                StringToNumberConverter.NumberStylesProperty,
                nameof(StringToNumberConverter.NumberStyles),
                x => x.NumberStyles,
                (x, value) => x.NumberStyles = value,
                NumberStyles.HexNumber
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                StringToNumberConverter.NumberTypeProperty,
                nameof(StringToNumberConverter.NumberType),
                x => x.NumberType,
                (x, value) => x.NumberType = value,
                NumberType.Decimal
            );
            AvaloniaPropertyAssertions.Property(
                converter,
                pristine,
                StringToNumberConverter.PreferredCultureProperty,
                nameof(StringToNumberConverter.PreferredCulture),
                x => x.PreferredCulture,
                (x, value) => x.PreferredCulture = value,
                Core.PreferredCulture.CurrentCulture
            );
        });

    private static void CompareConfiguration(ConverterBase configured, ConverterBase loaded)
    {
        var expected = (StringToNumberConverter)configured;
        var actual = (StringToNumberConverter)loaded;
        ConverterAssertions.ConfigurationEqual(expected.NumberStyles, actual.NumberStyles);
        ConverterAssertions.ConfigurationEqual(expected.NumberType, actual.NumberType);
        ConverterAssertions.ConfigurationEqual(expected.DefaultResult, actual.DefaultResult);
        ConverterAssertions.ConfigurationEqual(expected.PreferredCulture, actual.PreferredCulture);
    }

    [TestMethod]
    public Task DefaultFallbackMatchesPlatformPreset() =>
        PlatformThread.Run(() =>
        {
            Assert.AreSame(
                AvaloniaProperty.UnsetValue,
                new StringToNumberConverter().Convert("invalid", typeof(int), null, null)
            );
        });
}
