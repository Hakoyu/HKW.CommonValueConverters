using System.Windows.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class FirstStringStateToOtherStringMultiConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "FirstStringStateToOtherStringMultiConverter",
                    typeof(FirstStringStateToOtherStringMultiConverter),
                    () =>
                        M<FirstStringStateToOtherStringMultiConverter>(
                            [
                                ["text", "value", "nil", "empty", "space"],
                                [null, "value", "nil", "empty", "space"],
                                [null, "value", "null", "empty", "space"],
                                ["", "value", "null", "empty", "space"],
                                ["", "value", "null", "blank", "space"],
                                [" ", "value", "null", "blank", "space"],
                                [" ", "value", "null", "blank", "white"],
                                ["text", "value", "null", "blank", "white"],
                                ["text", "new", "null", "blank", "white"],
                            ],
                            [
                                "value",
                                "nil",
                                "null",
                                "empty",
                                "blank",
                                "space",
                                "white",
                                "value",
                                "new",
                            ]
                        ),
                    "FirstStringStateToOtherStringMultiConverter.xaml",
                    "fallback",
                    (expected, actual) =>
                    {
                        var e = (FirstStringStateToOtherStringMultiConverter)expected;
                        var a = (FirstStringStateToOtherStringMultiConverter)actual;
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
    [ConverterTest(
        typeof(FirstStringStateToOtherStringMultiConverter),
        ConverterTestLayer.Properties
    )]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(FirstStringStateToOtherStringMultiConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(FirstStringStateToOtherStringMultiConverter), ConverterTestLayer.Xaml)]
    public void ObjectXaml(WpfPlatformCase scenario) => WpfXamlRunner.Verify(scenario);

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void ObjectInputBoundaries(WpfPlatformCase scenario) =>
        WpfBoundaryAssertions.Verify(scenario);

    public static IEnumerable<object[]> BehaviorCases =>
        FirstStringStateToOtherStringMultiConverterSuite.Cases;

    [TestMethod]
    [DynamicData(
        nameof(BehaviorCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(
        typeof(FirstStringStateToOtherStringMultiConverter),
        ConverterTestLayer.Behavior
    )]
    public void ObjectBehavior(
        MultiConversionCase<FirstStringStateToOtherStringMultiConverterSuite.Configuration> scenario
    ) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new FirstStringStateToOtherStringMultiConverter();
                return WpfConversionAdapter.Multi(
                    configuration,
                    converter,
                    typeof(FirstStringStateToOtherStringMultiConverter),
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
            Assert.AreEqual(
                "selected",
                new FirstStringStateToOtherStringMultiConverter().Convert(
                    ["text", "selected", Binding.DoNothing],
                    null,
                    null,
                    null
                )
            );
        });

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() =>
            new FirstStringStateToOtherStringMultiConverter()
        );
    }
}
