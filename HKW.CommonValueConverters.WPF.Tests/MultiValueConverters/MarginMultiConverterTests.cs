using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class MarginMultiConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "MarginMultiConverter",
                    typeof(MarginMultiConverter),
                    () =>
                        M<MarginMultiConverter>(
                            [
                                [1d, 2d, 3d, 4d],
                                [5d, 2d, 3d, 4d],
                                [5d, 6d, 3d, 4d],
                                [5d, 6d, 7d, 4d],
                                [5d, 6d, 7d, 8d],
                            ],
                            [
                                new Thickness(1, 2, 3, 4),
                                new Thickness(5, 2, 3, 4),
                                new Thickness(5, 6, 3, 4),
                                new Thickness(5, 6, 7, 4),
                                new Thickness(5, 6, 7, 8),
                            ]
                        ),
                    "MarginMultiConverter.xaml",
                    "fallback",
                    (expected, actual) =>
                    {
                        var e = (MarginMultiConverter)expected;
                        var a = (MarginMultiConverter)actual;
                        WpfAssert.AssertResult(e.HasRatio, a.HasRatio);
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
    [ConverterTest(typeof(MarginMultiConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(MarginMultiConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(MarginMultiConverter), ConverterTestLayer.Xaml)]
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
    [ConverterTest(typeof(MarginMultiConverter), ConverterTestLayer.Behavior)]
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
    public void MarginMultiConverter_RejectsUniformOverflowLikePerSideOverflow() =>
        PlatformThread.Run(() =>
        {
            var converter = new MarginMultiConverter { HasRatio = true, DefaultResult = "invalid" };
            Assert.AreEqual("invalid", converter.Convert([2d, double.MaxValue], null, null, null));
            Assert.AreEqual(
                "invalid",
                converter.Convert([2d, double.MaxValue, 1d, 1d, 1d], null, null, null)
            );
        });

    [TestMethod]
    public void MarginMultiConverter_TwoSidesPreserveBothSlotsAndDefaultMissingSides() =>
        PlatformThread.Run(() =>
        {
            var converter = new MarginMultiConverter();
            Assert.AreEqual(new Thickness(1, 2, 0, 0), converter.Convert([1, 2], null, null, null));
            Assert.AreEqual(
                new Thickness(1, 2, 3, 0),
                converter.Convert([1, 2, 3], null, null, null)
            );
            Assert.AreEqual(
                new Thickness(0, 2, 0, 0),
                converter.Convert([null, 2], null, null, null)
            );
            converter.HasRatio = true;
            Assert.AreEqual(
                new Thickness(2, 4, 0, 0),
                converter.Convert([2, 1, 2], null, null, null)
            );
            Assert.AreEqual(new Thickness(2), converter.Convert([2, 1], null, null, null));
        });

    [TestMethod]
    public void Defaults_UnconfiguredAndClearedResultsUseCorePolicy_MarginMultiConverter() =>
        PlatformThread.Run(() =>
        {
            var margin = new MarginMultiConverter();
            Assert.AreEqual(new Thickness(), margin.Convert(["invalid"], null, null, null));
            margin.DefaultResult = "fallback";
            margin.ClearValue(ConverterBase.DefaultResultProperty);
            Assert.AreEqual(new Thickness(), margin.Convert([double.NaN], null, null, null));
        });

    [TestMethod]
    public void DatesTimesAndMargin_UseExplicitCultureAndFormatPriority_MarginMultiConverter() =>
        PlatformThread.Run(() =>
        {
            var french = CultureInfo.GetCultureInfo("fr-FR");
            var margin = new MarginMultiConverter
            {
                PreferredCulture = Core.PreferredCulture.ConverterCulture,
            };
            Assert.AreEqual(new Thickness(1.5), margin.Convert(["1,5"], null, null, french));
        });

    [TestMethod]
    public void PlatformDefaultsAndInvalidInputs_AreExplicit() =>
        PlatformThread.Run(() =>
        {
            var margin = new MarginMultiConverter { HasRatio = true };
            Assert.AreEqual(
                new Thickness(2, 4, 6, 8),
                margin.Convert([2, 1, 2, 3, 4], null, null, null)
            );
            Assert.AreEqual(new Thickness(), margin.Convert([], null, null, null));
            Assert.AreEqual(new Thickness(), margin.Convert([1, 2, 3, 4, 5, 6], null, null, null));
            Assert.AreEqual(
                new Thickness(),
                margin.Convert([double.PositiveInfinity, 1], null, null, null)
            );
            Assert.AreEqual(new Thickness(), margin.Convert([1, double.NaN], null, null, null));
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
        WpfPropertyAssertions.VerifyInherited(() => new MarginMultiConverter());
        WpfPropertyAssertions.Probe(
            () => new MarginMultiConverter(),
            MarginMultiConverter.HasRatioProperty,
            (c, value) => c.HasRatio = value,
            c => c.HasRatio,
            true,
            false,
            c => c.Convert([2, 3], null, null, null),
            new Thickness(6),
            new Thickness(2, 3, 0, 0)
        );
    }
}
