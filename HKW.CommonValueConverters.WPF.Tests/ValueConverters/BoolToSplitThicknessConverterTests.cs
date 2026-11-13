using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class BoolToSplitThicknessConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "BoolToSplitThicknessConverter",
                    typeof(BoolToSplitThicknessConverter),
                    () =>
                        S<BoolToSplitThicknessConverter>(
                            true,
                            false,
                            new Thickness(1, 2, 3, 4),
                            new Thickness(5),
                            "1,2,3,4|5|0",
                            () => new() { Separator = "|" }
                        ),
                    "BoolToSplitThicknessConverter.xaml",
                    new Thickness(),
                    (expected, actual) =>
                    {
                        var e = (BoolToSplitThicknessConverter)expected;
                        var a = (BoolToSplitThicknessConverter)actual;
                        WpfAssert.AssertResult(e.Separator, a.Separator);
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
    [ConverterTest(typeof(BoolToSplitThicknessConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(BoolToSplitThicknessConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(BoolToSplitThicknessConverter), ConverterTestLayer.Xaml)]
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
    [ConverterTest(typeof(BoolToSplitThicknessConverter), ConverterTestLayer.Behavior)]
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
    public void SplitThicknessBinding_UsesEffectiveConverterCulture() =>
        PlatformThread.Run(() =>
        {
            using var errors = new BindingErrors();
            var saved = CultureInfo.CurrentCulture;
            var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            culture.NumberFormat.NegativeSign = "~";
            var converter = new BoolToSplitThicknessConverter
            {
                Separator = "|",
                PreferredCulture = Core.PreferredCulture.ConverterCulture,
            };
            var source = new TestViewModel { A = true };
            var target = new System.Windows.Controls.Border();
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
                BindingOperations.SetBinding(
                    target,
                    FrameworkElement.MarginProperty,
                    new Binding("A")
                    {
                        Source = source,
                        Converter = converter,
                        ConverterCulture = culture,
                        ConverterParameter = "~1,~2,~3,~4|~5",
                        Mode = BindingMode.OneWay,
                    }
                );
                PlatformThread.Drain();
                Assert.AreEqual(new Thickness(-1, -2, -3, -4), target.Margin);
                source.A = false;
                PlatformThread.Drain();
                Assert.AreEqual(new Thickness(-5), target.Margin);
                converter.PreferredCulture = Core.PreferredCulture.CurrentCulture;
                CultureInfo.CurrentCulture = culture;
                Assert.AreEqual(
                    new Thickness(-2),
                    converter.Convert(true, null, "~2|0", CultureInfo.InvariantCulture)
                );
                errors.AssertEmpty();
            }
            finally
            {
                BindingOperations.ClearAllBindings(target);
                CultureInfo.CurrentCulture = saved;
            }
        });

    [TestMethod]
    public void BoolToSplitThicknessConverter_InvalidAndNonFiniteComponentsUseFallback() =>
        PlatformThread.Run(() =>
        {
            var converter = new BoolToSplitThicknessConverter
            {
                Separator = "|",
                DefaultResult = "invalid",
            };
            foreach (var invalid in new[] { "bad", "NaN", "Infinity", "1,2,3,bad", "1,2,3,1E999" })
                Assert.AreEqual(
                    "invalid",
                    converter.Convert(
                        true,
                        null,
                        invalid + "|0",
                        System.Globalization.CultureInfo.InvariantCulture
                    )
                );
            converter.DefaultResult = null;
            Assert.IsNull(converter.Convert(true, null, "bad|0", null));
            Assert.AreEqual(new Thickness(), converter.Convert(true, null, "1,2|0", null));
        });

    [TestMethod]
    public void BoolToSplitThicknessConverter_UsesLiveDelegateAndTypedThicknessTarget_BoolToSplitThicknessConverter() =>
        PlatformThread.Run(() =>
        {
            var converter = new BoolToSplitThicknessConverter { Separator = "|" };
            Assert.AreEqual(
                new Thickness(1, 2, 3, 4),
                converter.Convert(true, typeof(Thickness), "1,2,3,4|5|0", null)
            );
            var source = new TestViewModel { A = true };
            var border = new Border();
            try
            {
                BindingOperations.SetBinding(
                    border,
                    FrameworkElement.MarginProperty,
                    new Binding("A")
                    {
                        Source = source,
                        Converter = converter,
                        ConverterParameter = "1,2,3,4|5|0",
                    }
                );
                PlatformThread.Drain();
                Assert.AreEqual(new Thickness(1, 2, 3, 4), border.Margin);
                source.A = false;
                PlatformThread.Drain();
                Assert.AreEqual(new Thickness(5), border.Margin);
                converter.ConvertSplitValue = (_, _) => new Thickness(9);
                source.A = true;
                PlatformThread.Drain();
                Assert.AreEqual(new Thickness(9), border.Margin);
            }
            finally
            {
                BindingOperations.ClearAllBindings(border);
            }
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
        WpfPropertyAssertions.VerifyInherited(() => new BoolToSplitThicknessConverter());
        WpfPropertyAssertions.Probe(
            () => new BoolToSplitThicknessConverter(),
            BoolToSplitThicknessConverter.SeparatorProperty,
            (c, value) => c.Separator = value,
            c => c.Separator,
            "|",
            ",",
            c => c.Convert(true, null, "3|4,5", null),
            new Thickness(3),
            DependencyProperty.UnsetValue
        );
    }
}
