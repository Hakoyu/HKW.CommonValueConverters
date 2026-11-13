using System.Windows.Data;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class AllIsTrueMultiConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "AllIsTrueMultiConverter",
                    typeof(AllIsTrueMultiConverter),
                    () =>
                        M<AllIsTrueMultiConverter>(
                            [
                                [true, true],
                                [false, true],
                                [false, false],
                                [true, false],
                                [true, true],
                            ],
                            [true, false, false, false, true]
                        ),
                    "AllIsTrueMultiConverter.xaml",
                    "fallback",
                    (expected, actual) =>
                    {
                        var e = (AllIsTrueMultiConverter)expected;
                        var a = (AllIsTrueMultiConverter)actual;
                        WpfAssert.AssertResult(e.Value, a.Value);
                        WpfAssert.AssertResult(e.IsStringEquals, a.IsStringEquals);
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
    [ConverterTest(typeof(AllIsTrueMultiConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(AllIsTrueMultiConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(AllIsTrueMultiConverter), ConverterTestLayer.Xaml)]
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
    [ConverterTest(typeof(AllIsTrueMultiConverter), ConverterTestLayer.Behavior)]
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

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() => new AllIsTrueMultiConverter());
        WpfPropertyAssertions.Probe(
            () => new AllIsTrueMultiConverter(),
            AllIsTrueMultiConverter.ValueProperty,
            (c, value) => c.Value = value,
            c => c.Value,
            true,
            false,
            c => c.Convert([true, true], null, null, null),
            true,
            false
        );
        WpfPropertyAssertions.Probe(
            () => new AllIsTrueMultiConverter(),
            AllIsTrueMultiConverter.IsInvertedProperty,
            (c, value) => c.IsInverted = value,
            c => c.IsInverted,
            true,
            false,
            c => c.Convert([true, true], null, null, null),
            false,
            true
        );
        WpfPropertyAssertions.Probe(
            () => new AllIsTrueMultiConverter(),
            AllIsTrueMultiConverter.IsStringEqualsProperty,
            (c, value) => c.IsStringEquals = value,
            c => c.IsStringEquals,
            true,
            false,
            c => c.Convert(["True", "True"], null, null, null),
            true,
            false
        );
    }
}
