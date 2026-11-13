using System.Windows.Controls;
using System.Windows.Data;
using HKW.CommonValueConvertersTest.Testing;
using HKW.HKWUtils;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class BoolToNumberConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "BoolToNumberConverter",
                    typeof(BoolToNumberConverter),
                    () =>
                        S<BoolToNumberConverter>(true, false, 7, 2, factory: () =>
                            new() { TrueValue = 7, FalseValue = 2 }
                        ),
                    "BoolToNumberConverter.xaml",
                    2,
                    (expected, actual) =>
                    {
                        var e = (BoolToNumberConverter)expected;
                        var a = (BoolToNumberConverter)actual;
                        WpfAssert.AssertResult(e.NumberType, a.NumberType);
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
    [ConverterTest(typeof(BoolToNumberConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(BoolToNumberConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(BoolToNumberConverter), ConverterTestLayer.Xaml)]
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
    [ConverterTest(typeof(BoolToNumberConverter), ConverterTestLayer.Behavior)]
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
            Assert.AreEqual(0, new BoolToNumberConverter().TrueValue);
        });

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void StronglyTypedTarget(WpfPlatformCase scenario) =>
        WpfBindingRunner.VerifyTyped(
            scenario,
            () => new Slider { Maximum = 100 },
            Slider.ValueProperty,
            value => System.Convert.ToDouble(value)
        );

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() => new BoolToNumberConverter());
        WpfPropertyAssertions.Probe(
            () => new BoolToNumberConverter { TrueValue = 7 },
            BoolToNumberConverter.NumberTypeProperty,
            (c, value) => c.NumberType = value,
            c => c.NumberType,
            NumberType.Double,
            NumberType.Decimal,
            c => c.Convert(true, null, null, null),
            7,
            7
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToNumberConverter(),
            BoolToNumberConverter.TrueValueProperty,
            (c, value) => c.TrueValue = value,
            c => c.TrueValue,
            7,
            9,
            c => c.Convert(true, null, null, null),
            7,
            9
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToNumberConverter(),
            BoolToNumberConverter.FalseValueProperty,
            (c, value) => c.FalseValue = value,
            c => c.FalseValue,
            7,
            9,
            c => c.Convert(false, null, null, null),
            7,
            9
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToNumberConverter() { IsNullable = true },
            BoolToNumberConverter.NullValueProperty,
            (c, value) => c.NullValue = value,
            c => c.NullValue,
            7,
            9,
            c => c.Convert(null, null, null, null),
            7,
            9
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToNumberConverter() { NullValue = 7, FalseValue = 9 },
            BoolToNumberConverter.IsNullableProperty,
            (c, value) => c.IsNullable = value,
            c => c.IsNullable,
            true,
            false,
            c => c.Convert(null, null, null, null),
            7,
            9
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToNumberConverter() { TrueValue = 7, FalseValue = 9 },
            BoolToNumberConverter.IsInvertedProperty,
            (c, value) => c.IsInverted = value,
            c => c.IsInverted,
            true,
            false,
            c => c.Convert(true, null, null, null),
            9,
            7
        );
    }
}
