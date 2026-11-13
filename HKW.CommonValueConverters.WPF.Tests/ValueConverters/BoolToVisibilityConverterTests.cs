using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class BoolToVisibilityConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "BoolToVisibilityConverter",
                    typeof(BoolToVisibilityConverter),
                    () =>
                        S<BoolToVisibilityConverter>(
                            true,
                            false,
                            Visibility.Visible,
                            Visibility.Collapsed
                        ),
                    "BoolToVisibilityConverter.xaml",
                    Visibility.Collapsed,
                    (expected, actual) =>
                    {
                        var e = (BoolToVisibilityConverter)expected;
                        var a = (BoolToVisibilityConverter)actual;
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
    [ConverterTest(typeof(BoolToVisibilityConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(BoolToVisibilityConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(BoolToVisibilityConverter), ConverterTestLayer.Xaml)]
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
    [ConverterTest(typeof(BoolToVisibilityConverter), ConverterTestLayer.Behavior)]
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
            Assert.AreEqual(
                Visibility.Collapsed,
                new BoolToVisibilityConverter().Convert(null, null, null, null)
            );
        });

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void StronglyTypedTarget(WpfPlatformCase scenario) =>
        WpfBindingRunner.VerifyTyped(scenario, () => new Border(), UIElement.VisibilityProperty);

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() => new BoolToVisibilityConverter());
        WpfPropertyAssertions.Probe(
            () => new BoolToVisibilityConverter(),
            BoolToVisibilityConverter.TrueValueProperty,
            (c, value) => c.TrueValue = value,
            c => c.TrueValue,
            Visibility.Visible,
            Visibility.Hidden,
            c => c.Convert(true, null, null, null),
            Visibility.Visible,
            Visibility.Hidden
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToVisibilityConverter(),
            BoolToVisibilityConverter.FalseValueProperty,
            (c, value) => c.FalseValue = value,
            c => c.FalseValue,
            Visibility.Visible,
            Visibility.Hidden,
            c => c.Convert(false, null, null, null),
            Visibility.Visible,
            Visibility.Hidden
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToVisibilityConverter() { IsNullable = true },
            BoolToVisibilityConverter.NullValueProperty,
            (c, value) => c.NullValue = value,
            c => c.NullValue,
            Visibility.Visible,
            Visibility.Hidden,
            c => c.Convert(null, null, null, null),
            Visibility.Visible,
            Visibility.Hidden
        );
        WpfPropertyAssertions.Probe(
            () =>
                new BoolToVisibilityConverter()
                {
                    NullValue = Visibility.Visible,
                    FalseValue = Visibility.Hidden,
                },
            BoolToVisibilityConverter.IsNullableProperty,
            (c, value) => c.IsNullable = value,
            c => c.IsNullable,
            true,
            false,
            c => c.Convert(null, null, null, null),
            Visibility.Visible,
            Visibility.Hidden
        );
        WpfPropertyAssertions.Probe(
            () =>
                new BoolToVisibilityConverter()
                {
                    TrueValue = Visibility.Visible,
                    FalseValue = Visibility.Hidden,
                },
            BoolToVisibilityConverter.IsInvertedProperty,
            (c, value) => c.IsInverted = value,
            c => c.IsInverted,
            true,
            false,
            c => c.Convert(true, null, null, null),
            Visibility.Hidden,
            Visibility.Visible
        );
    }
}
