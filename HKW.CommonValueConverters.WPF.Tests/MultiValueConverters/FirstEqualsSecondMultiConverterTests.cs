using System.Windows;
using System.Windows.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class FirstEqualsSecondMultiConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "FirstEqualsSecondMultiConverter",
                    typeof(FirstEqualsSecondMultiConverter),
                    () =>
                        M<FirstEqualsSecondMultiConverter>(
                            [
                                ["one", "one"],
                                ["two", "one"],
                                ["two", "two"],
                            ],
                            [true, false, true]
                        ),
                    "FirstEqualsSecondMultiConverter.xaml",
                    "fallback",
                    (expected, actual) =>
                    {
                        var e = (FirstEqualsSecondMultiConverter)expected;
                        var a = (FirstEqualsSecondMultiConverter)actual;
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
    [ConverterTest(typeof(FirstEqualsSecondMultiConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(FirstEqualsSecondMultiConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(FirstEqualsSecondMultiConverter), ConverterTestLayer.Xaml)]
    public void ObjectXaml(WpfPlatformCase scenario) => WpfXamlRunner.Verify(scenario);

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void ObjectInputBoundaries(WpfPlatformCase scenario) =>
        WpfBoundaryAssertions.Verify(scenario);

    public static IEnumerable<object[]> BehaviorCases => FirstEqualsSecondMultiConverterSuite.Cases;

    [TestMethod]
    [DynamicData(
        nameof(BehaviorCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(FirstEqualsSecondMultiConverter), ConverterTestLayer.Behavior)]
    public void ObjectBehavior(
        MultiConversionCase<FirstEqualsSecondMultiConverterSuite.Configuration> scenario
    ) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new FirstEqualsSecondMultiConverter();
                return WpfConversionAdapter.Multi(
                    configuration,
                    converter,
                    typeof(FirstEqualsSecondMultiConverter),
                    settings =>
                    {
                        if (settings.PreferredCulture is { } culture)
                            converter.PreferredCulture = culture;
                        if (settings.GetIsStringEquals is not null)
                            converter.IsStringEquals = settings.GetIsStringEquals();
                        if (settings.GetIsInverted is not null)
                            converter.IsInverted = settings.GetIsInverted();
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
                new FirstEqualsSecondMultiConverter().Convert([1], null, null, null)
            );
        });

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() => new FirstEqualsSecondMultiConverter());
        WpfPropertyAssertions.Probe(
            () => new FirstEqualsSecondMultiConverter(),
            FirstEqualsSecondMultiConverter.IsStringEqualsProperty,
            (c, value) => c.IsStringEquals = value,
            c => c.IsStringEquals,
            true,
            false,
            c => c.Convert([2, "2"], null, null, null),
            true,
            false
        );
        WpfPropertyAssertions.Probe(
            () => new FirstEqualsSecondMultiConverter(),
            FirstEqualsSecondMultiConverter.IsInvertedProperty,
            (c, value) => c.IsInverted = value,
            c => c.IsInverted,
            true,
            false,
            c => c.Convert([2, 2], null, null, null),
            false,
            true
        );
    }
}
