using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class BoolToSolidColorBrushConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "BoolToSolidColorBrushConverter",
                    typeof(BoolToSolidColorBrushConverter),
                    () =>
                        S<BoolToSolidColorBrushConverter>(true, false, Colors.White, Colors.Black),
                    "BoolToSolidColorBrushConverter.xaml",
                    Colors.Black,
                    (expected, actual) =>
                    {
                        var e = (BoolToSolidColorBrushConverter)expected;
                        var a = (BoolToSolidColorBrushConverter)actual;
                        WpfAssert.AssertResult(e.TrueValue, a.TrueValue);
                        WpfAssert.AssertResult(e.FalseValue, a.FalseValue);
                        WpfAssert.AssertResult(e.IsNullable, a.IsNullable);
                        WpfAssert.AssertResult(e.NullValue, a.NullValue);
                        WpfAssert.AssertResult(e.IsInverted, a.IsInverted);
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
    [ConverterTest(typeof(BoolToSolidColorBrushConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(BoolToSolidColorBrushConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(BoolToSolidColorBrushConverter), ConverterTestLayer.Xaml)]
    public void ObjectXaml(WpfPlatformCase scenario) => WpfXamlRunner.Verify(scenario);

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void ObjectInputBoundaries(WpfPlatformCase scenario) =>
        WpfBoundaryAssertions.Verify(scenario);

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(BoolToSolidColorBrushConverter), ConverterTestLayer.Behavior)]
    public void ObjectPlatformBehavior(WpfPlatformCase scenario) =>
        WpfPlatformBehavior.Verify(scenario);

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void ObjectUnsupportedReverse(WpfPlatformCase scenario) =>
        WpfPlatformBehavior.VerifyUnsupportedReverse(scenario);

    // 原平台回归场景按对象保留.
    [TestMethod]
    public void PlatformDefaultsAndInvalidInputs_AreExplicit_0() =>
        PlatformThread.Run(() =>
        {
            Assert.AreSame(Brushes.White, new BoolToSolidColorBrushConverter().TrueValue);
        });

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void StronglyTypedTarget(WpfPlatformCase scenario) =>
        WpfBindingRunner.VerifyTyped(scenario, () => new Border(), Border.BackgroundProperty);

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() => new BoolToSolidColorBrushConverter());
        WpfPropertyAssertions.Probe(
            () => new BoolToSolidColorBrushConverter(),
            BoolToSolidColorBrushConverter.TrueValueProperty,
            (c, value) => c.TrueValue = value,
            c => c.TrueValue,
            Brushes.Red,
            Brushes.Blue,
            c => c.Convert(true, null, null, null),
            Brushes.Red,
            Brushes.Blue
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToSolidColorBrushConverter(),
            BoolToSolidColorBrushConverter.FalseValueProperty,
            (c, value) => c.FalseValue = value,
            c => c.FalseValue,
            Brushes.Red,
            Brushes.Blue,
            c => c.Convert(false, null, null, null),
            Brushes.Red,
            Brushes.Blue
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToSolidColorBrushConverter() { IsNullable = true },
            BoolToSolidColorBrushConverter.NullValueProperty,
            (c, value) => c.NullValue = value,
            c => c.NullValue,
            Brushes.Red,
            Brushes.Blue,
            c => c.Convert(null, null, null, null),
            Brushes.Red,
            Brushes.Blue
        );
        WpfPropertyAssertions.Probe(
            () =>
                new BoolToSolidColorBrushConverter()
                {
                    NullValue = Brushes.Red,
                    FalseValue = Brushes.Blue,
                },
            BoolToSolidColorBrushConverter.IsNullableProperty,
            (c, value) => c.IsNullable = value,
            c => c.IsNullable,
            true,
            false,
            c => c.Convert(null, null, null, null),
            Brushes.Red,
            Brushes.Blue
        );
        WpfPropertyAssertions.Probe(
            () =>
                new BoolToSolidColorBrushConverter()
                {
                    TrueValue = Brushes.Red,
                    FalseValue = Brushes.Blue,
                },
            BoolToSolidColorBrushConverter.IsInvertedProperty,
            (c, value) => c.IsInverted = value,
            c => c.IsInverted,
            true,
            false,
            c => c.Convert(true, null, null, null),
            Brushes.Blue,
            Brushes.Red
        );
    }
}
