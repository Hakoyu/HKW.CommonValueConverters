using System.Windows;
using System.Windows.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class StringFormatMultiConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "StringFormatMultiConverter",
                    typeof(StringFormatMultiConverter),
                    () =>
                        M<StringFormatMultiConverter>(
                            [
                                ["one", 1],
                                ["two", 1],
                                ["two", 2],
                            ],
                            ["one:1", "two:1", "two:2"],
                            "{0}:{1}"
                        ),
                    "StringFormatMultiConverter.xaml",
                    "fallback",
                    (expected, actual) =>
                    {
                        var e = (StringFormatMultiConverter)expected;
                        var a = (StringFormatMultiConverter)actual;
                        WpfAssert.AssertResult(e.ReplaceUnsetValue, a.ReplaceUnsetValue);
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
    [ConverterTest(typeof(StringFormatMultiConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(StringFormatMultiConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(StringFormatMultiConverter), ConverterTestLayer.Xaml)]
    public void ObjectXaml(WpfPlatformCase scenario) => WpfXamlRunner.Verify(scenario);

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void ObjectInputBoundaries(WpfPlatformCase scenario) =>
        WpfBoundaryAssertions.Verify(scenario);

    public static IEnumerable<object[]> BehaviorCases => StringFormatMultiConverterSuite.Cases;

    [TestMethod]
    [DynamicData(
        nameof(BehaviorCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(StringFormatMultiConverter), ConverterTestLayer.Behavior)]
    public void ObjectBehavior(
        MultiConversionCase<StringFormatMultiConverterSuite.Configuration> scenario
    ) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new StringFormatMultiConverter();
                return WpfConversionAdapter.Multi(
                    configuration,
                    converter,
                    typeof(StringFormatMultiConverter),
                    settings =>
                    {
                        if (settings.PreferredCulture is { } culture)
                            converter.PreferredCulture = culture;
                        if (settings.GetReplaceUnsetValue is not null)
                            converter.ReplaceUnsetValue = settings.GetReplaceUnsetValue();
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
                new StringFormatMultiConverter().Convert([1], null, "{9}", null)
            );
        });

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() => new StringFormatMultiConverter());
        WpfPropertyAssertions.Probe(
            () => new StringFormatMultiConverter(),
            StringFormatMultiConverter.ReplaceUnsetValueProperty,
            (c, value) => c.ReplaceUnsetValue = value,
            c => c.ReplaceUnsetValue,
            "missing",
            "none",
            c => c.Convert([DependencyProperty.UnsetValue], null, "{0}", null),
            "missing",
            "none"
        );
    }
}
