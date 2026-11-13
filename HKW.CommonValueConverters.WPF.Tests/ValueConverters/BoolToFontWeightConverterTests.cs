using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class BoolToFontWeightConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "BoolToFontWeightConverter",
                    typeof(BoolToFontWeightConverter),
                    () =>
                        S<BoolToFontWeightConverter>(
                            true,
                            false,
                            FontWeights.Bold,
                            FontWeights.Normal
                        ),
                    "BoolToFontWeightConverter.xaml",
                    FontWeights.Normal,
                    (expected, actual) =>
                    {
                        var e = (BoolToFontWeightConverter)expected;
                        var a = (BoolToFontWeightConverter)actual;
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
    [ConverterTest(typeof(BoolToFontWeightConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(BoolToFontWeightConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(BoolToFontWeightConverter), ConverterTestLayer.Xaml)]
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
    [ConverterTest(typeof(BoolToFontWeightConverter), ConverterTestLayer.Behavior)]
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
            Assert.AreEqual(FontWeights.Bold, new BoolToFontWeightConverter().TrueValue);
        });

    [TestMethod]
    public void PlatformDefaultsAndInvalidInputs_AreExplicit_1() =>
        PlatformThread.Run(() =>
        {
            Assert.AreEqual(FontWeights.Normal, new BoolToFontWeightConverter().FalseValue);
        });

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void StronglyTypedTarget(WpfPlatformCase scenario) =>
        WpfBindingRunner.VerifyTyped(scenario, () => new TextBlock(), TextBlock.FontWeightProperty);

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() => new BoolToFontWeightConverter());
        WpfPropertyAssertions.Probe(
            () => new BoolToFontWeightConverter(),
            BoolToFontWeightConverter.TrueValueProperty,
            (c, value) => c.TrueValue = value,
            c => c.TrueValue,
            FontWeights.Bold,
            FontWeights.Light,
            c => c.Convert(true, null, null, null),
            FontWeights.Bold,
            FontWeights.Light
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToFontWeightConverter(),
            BoolToFontWeightConverter.FalseValueProperty,
            (c, value) => c.FalseValue = value,
            c => c.FalseValue,
            FontWeights.Bold,
            FontWeights.Light,
            c => c.Convert(false, null, null, null),
            FontWeights.Bold,
            FontWeights.Light
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToFontWeightConverter() { IsNullable = true },
            BoolToFontWeightConverter.NullValueProperty,
            (c, value) => c.NullValue = value,
            c => c.NullValue,
            FontWeights.Bold,
            FontWeights.Light,
            c => c.Convert(null, null, null, null),
            FontWeights.Bold,
            FontWeights.Light
        );
        WpfPropertyAssertions.Probe(
            () =>
                new BoolToFontWeightConverter()
                {
                    NullValue = FontWeights.Bold,
                    FalseValue = FontWeights.Light,
                },
            BoolToFontWeightConverter.IsNullableProperty,
            (c, value) => c.IsNullable = value,
            c => c.IsNullable,
            true,
            false,
            c => c.Convert(null, null, null, null),
            FontWeights.Bold,
            FontWeights.Light
        );
        WpfPropertyAssertions.Probe(
            () =>
                new BoolToFontWeightConverter()
                {
                    TrueValue = FontWeights.Bold,
                    FalseValue = FontWeights.Light,
                },
            BoolToFontWeightConverter.IsInvertedProperty,
            (c, value) => c.IsInverted = value,
            c => c.IsInverted,
            true,
            false,
            c => c.Convert(true, null, null, null),
            FontWeights.Light,
            FontWeights.Bold
        );
    }
}
