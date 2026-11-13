using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class AllBoolToVisibilityMultiConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "AllBoolToVisibilityMultiConverter",
                    typeof(AllBoolToVisibilityMultiConverter),
                    () =>
                        M<AllBoolToVisibilityMultiConverter>(
                            [
                                [true, true],
                                [false, true],
                                [false, false],
                                [true, false],
                                [true, true],
                            ],
                            [
                                Visibility.Visible,
                                Visibility.Collapsed,
                                Visibility.Collapsed,
                                Visibility.Collapsed,
                                Visibility.Visible,
                            ]
                        ),
                    "AllBoolToVisibilityMultiConverter.xaml",
                    "fallback",
                    (expected, actual) =>
                    {
                        var e = (AllBoolToVisibilityMultiConverter)expected;
                        var a = (AllBoolToVisibilityMultiConverter)actual;
                        WpfAssert.AssertResult(e.TrueValue, a.TrueValue);
                        WpfAssert.AssertResult(e.FalseValue, a.FalseValue);
                        WpfAssert.AssertResult(e.NullValue, a.NullValue);
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
    [ConverterTest(typeof(AllBoolToVisibilityMultiConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(AllBoolToVisibilityMultiConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(AllBoolToVisibilityMultiConverter), ConverterTestLayer.Xaml)]
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
    [ConverterTest(typeof(AllBoolToVisibilityMultiConverter), ConverterTestLayer.Behavior)]
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
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void StronglyTypedTarget(WpfPlatformCase scenario) =>
        WpfBindingRunner.VerifyTyped(scenario, () => new Border(), UIElement.VisibilityProperty);

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() => new AllBoolToVisibilityMultiConverter());
        WpfPropertyAssertions.Probe(
            () => new AllBoolToVisibilityMultiConverter(),
            AllBoolToVisibilityMultiConverter.TrueValueProperty,
            (c, value) => c.TrueValue = value,
            c => c.TrueValue,
            Visibility.Visible,
            Visibility.Hidden,
            c => c.Convert([true, true], null, null, null),
            Visibility.Visible,
            Visibility.Hidden
        );
        WpfPropertyAssertions.Probe(
            () => new AllBoolToVisibilityMultiConverter(),
            AllBoolToVisibilityMultiConverter.FalseValueProperty,
            (c, value) => c.FalseValue = value,
            c => c.FalseValue,
            Visibility.Visible,
            Visibility.Hidden,
            c => c.Convert([false, false], null, null, null),
            Visibility.Visible,
            Visibility.Hidden
        );
        WpfPropertyAssertions.Probe(
            () => new AllBoolToVisibilityMultiConverter(),
            AllBoolToVisibilityMultiConverter.NullValueProperty,
            (c, value) => c.NullValue = value,
            c => c.NullValue,
            Visibility.Visible,
            Visibility.Hidden,
            c => c.Convert([null, null], null, null, null),
            Visibility.Visible,
            Visibility.Hidden
        );
    }
}
