using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class EqualsToVisibilityConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "EqualsToVisibilityConverter",
                    typeof(EqualsToVisibilityConverter),
                    () =>
                        S<EqualsToVisibilityConverter>(
                            "match",
                            "other",
                            Visibility.Visible,
                            Visibility.Collapsed,
                            factory: () => new() { Other = "match" }
                        ),
                    "EqualsToVisibilityConverter.xaml",
                    Visibility.Collapsed,
                    (expected, actual) =>
                    {
                        var e = (EqualsToVisibilityConverter)expected;
                        var a = (EqualsToVisibilityConverter)actual;
                        WpfAssert.AssertResult(e.Other, a.Other);
                        WpfAssert.AssertResult(e.TrueValue, a.TrueValue);
                        WpfAssert.AssertResult(e.FalseValue, a.FalseValue);
                        WpfAssert.AssertResult(e.NullValue, a.NullValue);
                        WpfAssert.AssertResult(e.IsStringEquals, a.IsStringEquals);
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
    [ConverterTest(typeof(EqualsToVisibilityConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(EqualsToVisibilityConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(EqualsToVisibilityConverter), ConverterTestLayer.Xaml)]
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
    [ConverterTest(typeof(EqualsToVisibilityConverter), ConverterTestLayer.Behavior)]
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
                new EqualsToVisibilityConverter().Convert(null, null, null, null)
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
        WpfPropertyAssertions.VerifyInherited(() => new EqualsToVisibilityConverter());
        WpfPropertyAssertions.Probe(
            () => new EqualsToVisibilityConverter() { Other = "match" },
            EqualsToVisibilityConverter.TrueValueProperty,
            (c, value) => c.TrueValue = value,
            c => c.TrueValue,
            Visibility.Visible,
            Visibility.Hidden,
            c => c.Convert("match", null, null, null),
            Visibility.Visible,
            Visibility.Hidden
        );
        WpfPropertyAssertions.Probe(
            () => new EqualsToVisibilityConverter() { Other = "match" },
            EqualsToVisibilityConverter.FalseValueProperty,
            (c, value) => c.FalseValue = value,
            c => c.FalseValue,
            Visibility.Visible,
            Visibility.Hidden,
            c => c.Convert("other", null, null, null),
            Visibility.Visible,
            Visibility.Hidden
        );
        WpfPropertyAssertions.Probe(
            () => new EqualsToVisibilityConverter() { Other = "match" },
            EqualsToVisibilityConverter.NullValueProperty,
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
                new EqualsToVisibilityConverter()
                {
                    TrueValue = Visibility.Visible,
                    FalseValue = Visibility.Hidden,
                },
            EqualsToVisibilityConverter.OtherProperty,
            (c, value) => c.Other = value,
            c => c.Other,
            "match",
            "other",
            c => c.Convert("match", null, null, null),
            Visibility.Visible,
            Visibility.Hidden
        );
        WpfPropertyAssertions.Probe(
            () =>
                new EqualsToVisibilityConverter()
                {
                    Other = 2,
                    TrueValue = Visibility.Visible,
                    FalseValue = Visibility.Hidden,
                },
            EqualsToVisibilityConverter.IsStringEqualsProperty,
            (c, value) => c.IsStringEquals = value,
            c => c.IsStringEquals,
            true,
            false,
            c => c.Convert("2", null, null, null),
            Visibility.Visible,
            Visibility.Hidden
        );
    }
}
