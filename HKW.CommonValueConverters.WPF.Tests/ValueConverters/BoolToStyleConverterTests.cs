using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class BoolToStyleConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "BoolToStyleConverter",
                    typeof(BoolToStyleConverter),
                    () =>
                        S<BoolToStyleConverter>(
                            true,
                            false,
                            new StyleExpectation(0.25),
                            new StyleExpectation(0.75),
                            factory: () =>
                                new() { TrueValue = Style(0.25), FalseValue = Style(0.75) }
                        ),
                    "BoolToStyleConverter.xaml",
                    new StyleExpectation(0.75),
                    (expected, actual) =>
                    {
                        var e = (BoolToStyleConverter)expected;
                        var a = (BoolToStyleConverter)actual;
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
    [ConverterTest(typeof(BoolToStyleConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(BoolToStyleConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(BoolToStyleConverter), ConverterTestLayer.Xaml)]
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
    [ConverterTest(typeof(BoolToStyleConverter), ConverterTestLayer.Behavior)]
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
            Assert.IsNull(new BoolToStyleConverter().TrueValue);
        });

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void StronglyTypedTarget(WpfPlatformCase scenario) =>
        WpfBindingRunner.VerifyTyped(scenario, () => new Border(), FrameworkElement.StyleProperty);

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() => new BoolToStyleConverter());
        WpfPropertyAssertions.Probe(
            () => new BoolToStyleConverter(),
            BoolToStyleConverter.TrueValueProperty,
            (c, value) => c.TrueValue = value,
            c => c.TrueValue,
            Style(0.25),
            Style(0.75),
            c => c.Convert(true, null, null, null),
            Style(0.25),
            Style(0.75)
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToStyleConverter(),
            BoolToStyleConverter.FalseValueProperty,
            (c, value) => c.FalseValue = value,
            c => c.FalseValue,
            Style(0.25),
            Style(0.75),
            c => c.Convert(false, null, null, null),
            Style(0.25),
            Style(0.75)
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToStyleConverter() { IsNullable = true },
            BoolToStyleConverter.NullValueProperty,
            (c, value) => c.NullValue = value,
            c => c.NullValue,
            Style(0.25),
            Style(0.75),
            c => c.Convert(null, null, null, null),
            Style(0.25),
            Style(0.75)
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToStyleConverter() { NullValue = Style(0.25), FalseValue = Style(0.75) },
            BoolToStyleConverter.IsNullableProperty,
            (c, value) => c.IsNullable = value,
            c => c.IsNullable,
            true,
            false,
            c => c.Convert(null, null, null, null),
            Style(0.25),
            Style(0.75)
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToStyleConverter() { TrueValue = Style(0.25), FalseValue = Style(0.75) },
            BoolToStyleConverter.IsInvertedProperty,
            (c, value) => c.IsInverted = value,
            c => c.IsInverted,
            true,
            false,
            c => c.Convert(true, null, null, null),
            Style(0.75),
            Style(0.25)
        );
    }
}
