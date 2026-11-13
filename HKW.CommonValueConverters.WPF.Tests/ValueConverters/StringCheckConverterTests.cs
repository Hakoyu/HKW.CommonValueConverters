using System.Windows.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using HKW.HKWUtils;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class StringCheckConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "StringCheckConverter",
                    typeof(StringCheckConverter),
                    () => S<StringCheckConverter>(null, "value", true, false),
                    "StringCheckConverter.xaml",
                    true,
                    (expected, actual) =>
                    {
                        var e = (StringCheckConverter)expected;
                        var a = (StringCheckConverter)actual;
                        WpfAssert.AssertResult(e.StringCheckType, a.StringCheckType);
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
    [ConverterTest(typeof(StringCheckConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(StringCheckConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(StringCheckConverter), ConverterTestLayer.Xaml)]
    public void ObjectXaml(WpfPlatformCase scenario) => WpfXamlRunner.Verify(scenario);

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void ObjectInputBoundaries(WpfPlatformCase scenario) =>
        WpfBoundaryAssertions.Verify(scenario);

    public static IEnumerable<object[]> BehaviorCases => StringCheckConverterSuite.Cases;

    [TestMethod]
    [DynamicData(
        nameof(BehaviorCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(StringCheckConverter), ConverterTestLayer.Behavior)]
    public void ObjectBehavior(
        SingleConversionCase<StringCheckConverterSuite.Configuration> scenario
    ) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new StringCheckConverter();
                return WpfConversionAdapter.Single(
                    configuration,
                    converter,
                    typeof(StringCheckConverter),
                    settings =>
                    {
                        if (settings.PreferredCulture is { } culture)
                            converter.PreferredCulture = culture;
                        if (settings.GetStringCheckType is not null)
                            converter.StringCheckType = settings.GetStringCheckType();
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

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() => new StringCheckConverter());
        WpfPropertyAssertions.Probe(
            () => new StringCheckConverter(),
            StringCheckConverter.StringCheckTypeProperty,
            (c, value) => c.StringCheckType = value,
            c => c.StringCheckType,
            StringCheckType.NullOrEmpty,
            StringCheckType.Null,
            c => c.Convert("", null, null, null),
            true,
            false
        );
        WpfPropertyAssertions.Probe(
            () => new StringCheckConverter(),
            StringCheckConverter.IsInvertedProperty,
            (c, value) => c.IsInverted = value,
            c => c.IsInverted,
            true,
            false,
            c => c.Convert(null, null, null, null),
            false,
            true
        );
    }
}
