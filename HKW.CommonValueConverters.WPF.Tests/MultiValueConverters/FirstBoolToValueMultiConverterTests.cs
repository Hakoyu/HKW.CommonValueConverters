using System.Windows;
using System.Windows.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class FirstBoolToValueMultiConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "FirstBoolToValueMultiConverter",
                    typeof(FirstBoolToValueMultiConverter),
                    () =>
                        M<FirstBoolToValueMultiConverter>(
                            [
                                [true, "yes", "no", "null"],
                                [false, "yes", "no", "null"],
                                [false, "yes", "changed", "null"],
                                [true, "yes", "changed", "null"],
                                [true, "updated", "changed", "null"],
                                [null, "updated", "changed", "null"],
                                [null, "updated", "changed", "nil"],
                            ],
                            ["yes", "no", "changed", "yes", "updated", "null", "nil"]
                        ),
                    "FirstBoolToValueMultiConverter.xaml",
                    "fallback",
                    (expected, actual) =>
                    {
                        var e = (FirstBoolToValueMultiConverter)expected;
                        var a = (FirstBoolToValueMultiConverter)actual;
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
    [ConverterTest(typeof(FirstBoolToValueMultiConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(FirstBoolToValueMultiConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(FirstBoolToValueMultiConverter), ConverterTestLayer.Xaml)]
    public void ObjectXaml(WpfPlatformCase scenario) => WpfXamlRunner.Verify(scenario);

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void ObjectInputBoundaries(WpfPlatformCase scenario) =>
        WpfBoundaryAssertions.Verify(scenario);

    public static IEnumerable<object[]> BehaviorCases => FirstBoolToValueMultiConverterSuite.Cases;

    [TestMethod]
    [DynamicData(
        nameof(BehaviorCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(FirstBoolToValueMultiConverter), ConverterTestLayer.Behavior)]
    public void ObjectBehavior(
        MultiConversionCase<FirstBoolToValueMultiConverterSuite.Configuration> scenario
    ) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new FirstBoolToValueMultiConverter();
                return WpfConversionAdapter.Multi(
                    configuration,
                    converter,
                    typeof(FirstBoolToValueMultiConverter),
                    settings =>
                    {
                        if (settings.PreferredCulture is { } culture)
                            converter.PreferredCulture = culture;
                        if (settings.GetDefaultResult is not null)
                            converter.DefaultResult = WpfConversionAdapter.ToPlatform(
                                settings.GetDefaultResult()
                            );
                    }
                );
            })
        );

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
    public void MultiValueBoundaries_EmptyShortAndSelectedBranchInputs_0() =>
        PlatformThread.Run(() =>
        {
            Assert.AreSame(
                DependencyProperty.UnsetValue,
                new FirstBoolToValueMultiConverter().Convert([true], null, null, null)
            );
        });

    [TestMethod]
    public void MultiValueBoundaries_EmptyShortAndSelectedBranchInputs_1() =>
        PlatformThread.Run(() =>
        {
            Assert.AreEqual(
                "selected",
                new FirstBoolToValueMultiConverter().Convert(
                    [true, "selected", DependencyProperty.UnsetValue],
                    null,
                    null,
                    null
                )
            );
        });

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() => new FirstBoolToValueMultiConverter());
    }
}
