using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class BoolToThicknessConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "BoolToThicknessConverter",
                    typeof(BoolToThicknessConverter),
                    () =>
                        S<BoolToThicknessConverter>(
                            true,
                            false,
                            new Thickness(5),
                            new Thickness(0)
                        ),
                    "BoolToThicknessConverter.xaml",
                    new Thickness(),
                    (expected, actual) =>
                    {
                        var e = (BoolToThicknessConverter)expected;
                        var a = (BoolToThicknessConverter)actual;
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
    [ConverterTest(typeof(BoolToThicknessConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(BoolToThicknessConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(BoolToThicknessConverter), ConverterTestLayer.Xaml)]
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
    [ConverterTest(typeof(BoolToThicknessConverter), ConverterTestLayer.Behavior)]
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
            var converter = new BoolToThicknessConverter();
            Assert.AreEqual(new Thickness(0), converter.Convert(new object(), null, null, null));
            Assert.AreEqual(
                new Thickness(0),
                converter.Convert(DependencyProperty.UnsetValue, null, null, null)
            );
            converter.DefaultResult = "fallback";
            Assert.AreEqual(
                "fallback",
                converter.Convert(DependencyProperty.UnsetValue, null, null, null)
            );
        });

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void StronglyTypedTarget(WpfPlatformCase scenario) =>
        WpfBindingRunner.VerifyTyped(scenario, () => new Border(), FrameworkElement.MarginProperty);

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() => new BoolToThicknessConverter());
        WpfPropertyAssertions.Probe(
            () => new BoolToThicknessConverter(),
            BoolToThicknessConverter.TrueValueProperty,
            (c, value) => c.TrueValue = value,
            c => c.TrueValue,
            new Thickness(2),
            new Thickness(4),
            c => c.Convert(true, null, null, null),
            new Thickness(2),
            new Thickness(4)
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToThicknessConverter(),
            BoolToThicknessConverter.FalseValueProperty,
            (c, value) => c.FalseValue = value,
            c => c.FalseValue,
            new Thickness(2),
            new Thickness(4),
            c => c.Convert(false, null, null, null),
            new Thickness(2),
            new Thickness(4)
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToThicknessConverter() { IsNullable = true },
            BoolToThicknessConverter.NullValueProperty,
            (c, value) => c.NullValue = value,
            c => c.NullValue,
            new Thickness(2),
            new Thickness(4),
            c => c.Convert(null, null, null, null),
            new Thickness(2),
            new Thickness(4)
        );
        WpfPropertyAssertions.Probe(
            () =>
                new BoolToThicknessConverter()
                {
                    NullValue = new Thickness(2),
                    FalseValue = new Thickness(4),
                },
            BoolToThicknessConverter.IsNullableProperty,
            (c, value) => c.IsNullable = value,
            c => c.IsNullable,
            true,
            false,
            c => c.Convert(null, null, null, null),
            new Thickness(2),
            new Thickness(4)
        );
        WpfPropertyAssertions.Probe(
            () =>
                new BoolToThicknessConverter()
                {
                    TrueValue = new Thickness(2),
                    FalseValue = new Thickness(4),
                },
            BoolToThicknessConverter.IsInvertedProperty,
            (c, value) => c.IsInverted = value,
            c => c.IsInverted,
            true,
            false,
            c => c.Convert(true, null, null, null),
            new Thickness(4),
            new Thickness(2)
        );
    }
}
