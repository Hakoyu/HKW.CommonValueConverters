using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class SolidColorBrushToMediaColorConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "SolidColorBrushToMediaColorConverter",
                    typeof(SolidColorBrushToMediaColorConverter),
                    () =>
                        S<SolidColorBrushToMediaColorConverter>(
                            Brushes.Red,
                            Brushes.Blue,
                            Colors.Red,
                            Colors.Blue
                        ),
                    "SolidColorBrushToMediaColorConverter.xaml",
                    "fallback",
                    (expected, actual) =>
                    {
                        var e = (SolidColorBrushToMediaColorConverter)expected;
                        var a = (SolidColorBrushToMediaColorConverter)actual;
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
    [ConverterTest(typeof(SolidColorBrushToMediaColorConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(SolidColorBrushToMediaColorConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(SolidColorBrushToMediaColorConverter), ConverterTestLayer.Xaml)]
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
    [ConverterTest(typeof(SolidColorBrushToMediaColorConverter), ConverterTestLayer.Behavior)]
    public void ObjectPlatformBehavior(WpfPlatformCase scenario) =>
        WpfPlatformBehavior.Verify(scenario);

    // 原平台回归场景按对象保留.
    [TestMethod]
    public void SolidColorBrushToMediaColorConverter_TypedColorRoundTrip() =>
        PlatformThread.Run(() =>
            WpfTwoWayBindingRunner.Verify(
                new SolidColorBrushToMediaColorConverter(),
                new ColorTarget(),
                ColorTarget.ColorProperty,
                Brushes.Red,
                Colors.Red,
                Colors.Blue,
                Brushes.Blue
            )
        );

    [TestMethod]
    public void PlatformDefaultsAndInvalidInputs_AreExplicit() =>
        PlatformThread.Run(() =>
        {
            Assert.AreSame(
                DependencyProperty.UnsetValue,
                new SolidColorBrushToMediaColorConverter().Convert(
                    new LinearGradientBrush(),
                    null,
                    null,
                    null
                )
            );
        });

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void StronglyTypedTarget(WpfPlatformCase scenario) =>
        WpfBindingRunner.VerifyTyped(scenario, () => new ColorTarget(), ColorTarget.ColorProperty);

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() => new SolidColorBrushToMediaColorConverter());
    }
}
