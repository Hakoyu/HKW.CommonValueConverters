using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using HKW.HKWUtils;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class StringToNumberConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "StringToNumberConverter",
                    typeof(StringToNumberConverter),
                    () => S<StringToNumberConverter>("12", "34", 12, 34),
                    "StringToNumberConverter.xaml",
                    "fallback",
                    (expected, actual) =>
                    {
                        var e = (StringToNumberConverter)expected;
                        var a = (StringToNumberConverter)actual;
                        WpfAssert.AssertResult(e.NumberStyles, a.NumberStyles);
                        WpfAssert.AssertResult(e.NumberType, a.NumberType);
                        WpfAssert.AssertResult(e.DefaultResult, a.DefaultResult);
                        WpfAssert.AssertResult(e.PreferredCulture, a.PreferredCulture);
                    },
                    VerifyPropertyWiring
                ),
            ],
        ];

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(StringToNumberConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(StringToNumberConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(StringToNumberConverter), ConverterTestLayer.Xaml)]
    public void ObjectXaml(WpfPlatformCase scenario) => WpfXamlRunner.Verify(scenario);

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void ObjectInputBoundaries(WpfPlatformCase scenario) =>
        WpfBoundaryAssertions.Verify(scenario);

    public static IEnumerable<object[]> BehaviorCases => StringToNumberConverterSuite.Cases;

    [TestMethod]
    [DynamicData(
        nameof(BehaviorCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(StringToNumberConverter), ConverterTestLayer.Behavior)]
    public void ObjectBehavior(
        SingleConversionCase<StringToNumberConverterSuite.Configuration> scenario
    ) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new StringToNumberConverter();
                return WpfConversionAdapter.Single(
                    configuration,
                    converter,
                    typeof(StringToNumberConverter),
                    settings =>
                    {
                        if (settings.PreferredCulture is { } culture)
                            converter.PreferredCulture = culture;
                        if (settings.GetNumberStyles is not null)
                            converter.NumberStyles = settings.GetNumberStyles();
                        if (settings.GetNumberType is not null)
                            converter.NumberType = settings.GetNumberType();
                        if (settings.GetDefaultResult is not null)
                            converter.DefaultResult = WpfConversionAdapter.ToPlatform(
                                settings.GetDefaultResult()
                            );
                    }
                );
            })
        );

    // 原平台回归场景按对象保留.
    [TestMethod]
    public void Binding_UnconfiguredAndClearedDefaultActivatesTypedFallback() =>
        PlatformThread.Run(() =>
        {
            using var errors = new BindingErrors();
            var source = new TestViewModel { A = "12" };
            var target = new Slider { Maximum = 100 };
            var converter = new StringToNumberConverter
            {
                NumberType = HKW.HKWUtils.NumberType.Double,
            };
            try
            {
                BindingOperations.SetBinding(
                    target,
                    Slider.ValueProperty,
                    new Binding("A")
                    {
                        Source = source,
                        Converter = converter,
                        Mode = BindingMode.OneWay,
                        FallbackValue = 3d,
                        TargetNullValue = 5d,
                    }
                );
                PlatformThread.Drain();
                Assert.AreEqual(12d, target.Value);
                source.A = "invalid";
                PlatformThread.Drain();
                Assert.AreEqual(3d, target.Value);
                converter.DefaultResult = null;
                source.A = "still-invalid";
                PlatformThread.Drain();
                Assert.AreEqual(5d, target.Value);
                converter.ClearValue(ConverterBase.DefaultResultProperty);
                source.A = "cleared-invalid";
                PlatformThread.Drain();
                Assert.AreEqual(3d, target.Value);
                errors.AssertEmpty();
            }
            finally
            {
                BindingOperations.ClearAllBindings(target);
            }
        });

    [TestMethod]
    public void NumberBinding_UsesConverterCultureForInitialAndUpdatedValues() =>
        PlatformThread.Run(() =>
        {
            var source = new TestViewModel { A = "12,5" };
            var target = new TestTarget();
            var converter = new StringToNumberConverter
            {
                NumberType = NumberType.Decimal,
                PreferredCulture = Core.PreferredCulture.ConverterCulture,
            };
            try
            {
                BindingOperations.SetBinding(
                    target,
                    TestTarget.ResultProperty,
                    new Binding("A")
                    {
                        Source = source,
                        Converter = converter,
                        ConverterCulture = CultureInfo.GetCultureInfo("fr-FR"),
                    }
                );
                PlatformThread.Drain();
                Assert.AreEqual(12.5m, target.Result);
                source.A = "13,75";
                PlatformThread.Drain();
                Assert.AreEqual(13.75m, target.Result);
                Assert.AreEqual(
                    "12,5",
                    converter.ConvertBack(12.5m, null, null, CultureInfo.GetCultureInfo("fr-FR"))
                );
            }
            finally
            {
                BindingOperations.ClearAllBindings(target);
            }
        });

    [TestMethod]
    public void StringToNumberConverter_TypedSliderRoundTrip() =>
        PlatformThread.Run(() =>
            WpfTwoWayBindingRunner.Verify(
                new StringToNumberConverter { NumberType = NumberType.Double },
                new Slider { Maximum = 100 },
                Slider.ValueProperty,
                "12",
                12d,
                34d,
                "34"
            )
        );

    [TestMethod]
    public void DatesTimesAndMargin_UseExplicitCultureAndFormatPriority_StringToNumberConverter() =>
        PlatformThread.Run(() =>
        {
            var french = CultureInfo.GetCultureInfo("fr-FR");
            var number = new StringToNumberConverter { NumberStyles = NumberStyles.HexNumber };
            Assert.AreEqual(255, number.Convert("FF", null, null, CultureInfo.InvariantCulture));
            Assert.AreSame(
                DependencyProperty.UnsetValue,
                number.Convert("not-hex", null, null, french)
            );
        });

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() => new StringToNumberConverter());
        WpfPropertyAssertions.Probe(
            () => new StringToNumberConverter(),
            StringToNumberConverter.NumberTypeProperty,
            (c, value) => c.NumberType = value,
            c => c.NumberType,
            NumberType.Double,
            NumberType.Decimal,
            c => c.Convert("12", null, null, null),
            12d,
            12m
        );
        WpfPropertyAssertions.Probe(
            () => new StringToNumberConverter(),
            StringToNumberConverter.NumberStylesProperty,
            (c, value) => c.NumberStyles = value,
            c => c.NumberStyles,
            NumberStyles.HexNumber,
            NumberStyles.Integer,
            c => c.Convert("FF", null, null, null),
            255,
            DependencyProperty.UnsetValue
        );
    }
}
