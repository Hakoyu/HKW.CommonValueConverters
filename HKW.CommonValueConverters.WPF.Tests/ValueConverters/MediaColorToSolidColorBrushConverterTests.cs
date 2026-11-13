using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class MediaColorToSolidColorBrushConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "MediaColorToSolidColorBrushConverter",
                    typeof(MediaColorToSolidColorBrushConverter),
                    () =>
                        S<MediaColorToSolidColorBrushConverter>(
                            Colors.Red,
                            Colors.Blue,
                            Colors.Red,
                            Colors.Blue
                        ),
                    "MediaColorToSolidColorBrushConverter.xaml",
                    "fallback",
                    (expected, actual) =>
                    {
                        var e = (MediaColorToSolidColorBrushConverter)expected;
                        var a = (MediaColorToSolidColorBrushConverter)actual;
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
    [ConverterTest(typeof(MediaColorToSolidColorBrushConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(MediaColorToSolidColorBrushConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(MediaColorToSolidColorBrushConverter), ConverterTestLayer.Xaml)]
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
    [ConverterTest(typeof(MediaColorToSolidColorBrushConverter), ConverterTestLayer.Behavior)]
    public void ObjectPlatformBehavior(WpfPlatformCase scenario) =>
        WpfPlatformBehavior.Verify(scenario);

    // 原平台回归场景按对象保留.
    [TestMethod]
    public void MediaColorToSolidColorBrushConverter_TypedBrushRoundTrip() =>
        PlatformThread.Run(() =>
            WpfTwoWayBindingRunner.Verify(
                new MediaColorToSolidColorBrushConverter(),
                new Border(),
                Border.BackgroundProperty,
                Colors.Red,
                Colors.Red,
                Brushes.Blue,
                Colors.Blue
            )
        );

    [TestMethod]
    public void PlatformDefaultsAndInvalidInputs_AreExplicit() =>
        PlatformThread.Run(() =>
        {
            var color = new MediaColorToSolidColorBrushConverter();
            Assert.AreSame(DependencyProperty.UnsetValue, color.Convert(null, null, null, null));
            Assert.AreSame(
                DependencyProperty.UnsetValue,
                color.ConvertBack(Colors.Red, null, null, null)
            );
            var first = (SolidColorBrush)color.Convert(Colors.Red, null, null, null)!;
            var second = (SolidColorBrush)color.Convert(Colors.Red, null, null, null)!;
            Assert.AreNotSame(first, second);
            Assert.IsFalse(first.IsFrozen);
            first.Color = Colors.Blue;
            Assert.AreEqual(Colors.Red, second.Color);
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
        WpfPropertyAssertions.VerifyInherited(() => new MediaColorToSolidColorBrushConverter());
    }
}
