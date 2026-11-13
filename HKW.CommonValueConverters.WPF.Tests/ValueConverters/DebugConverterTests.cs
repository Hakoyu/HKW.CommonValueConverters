using System.Windows.Controls;
using System.Windows.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class DebugConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "DebugConverter",
                    typeof(DebugConverter),
                    () => S<DebugConverter>("before", "after", "before", "after"),
                    "DebugConverter.xaml",
                    null,
                    (expected, actual) =>
                    {
                        var e = (DebugConverter)expected;
                        var a = (DebugConverter)actual;
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
    [ConverterTest(typeof(DebugConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(DebugConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(DebugConverter), ConverterTestLayer.Xaml)]
    public void ObjectXaml(WpfPlatformCase scenario) => WpfXamlRunner.Verify(scenario);

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void ObjectInputBoundaries(WpfPlatformCase scenario) =>
        WpfBoundaryAssertions.Verify(scenario, passThroughUnavailable: true);

    public static IEnumerable<object[]> BehaviorCases => DebugConverterSuite.Cases;

    [TestMethod]
    [DynamicData(
        nameof(BehaviorCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(DebugConverter), ConverterTestLayer.Behavior)]
    public void ObjectBehavior(SingleConversionCase<DebugConverterSuite.Configuration> scenario) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new DebugConverter();
                return WpfConversionAdapter.Single(
                    configuration,
                    converter,
                    typeof(DebugConverter),
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

    // 原平台回归场景按对象保留.
    [TestMethod]
    public void DebugConverter_TypedTextBoxRoundTrip() =>
        PlatformThread.Run(() =>
            WpfTwoWayBindingRunner.Verify(
                new DebugConverter(),
                new TextBox(),
                TextBox.TextProperty,
                "first",
                "first",
                "next",
                "next"
            )
        );

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(
            () => new DebugConverter(),
            passThroughUnavailable: true
        );
    }
}
