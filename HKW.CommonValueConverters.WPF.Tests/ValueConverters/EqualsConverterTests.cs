using System.Windows.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class EqualsConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "EqualsConverter",
                    typeof(EqualsConverter),
                    () =>
                        S<EqualsConverter>("match", "other", true, false, factory: () =>
                            new() { Other = "match" }
                        ),
                    "EqualsConverter.xaml",
                    false,
                    (expected, actual) =>
                    {
                        var e = (EqualsConverter)expected;
                        var a = (EqualsConverter)actual;
                        WpfAssert.AssertResult(e.Other, a.Other);
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
    [ConverterTest(typeof(EqualsConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(EqualsConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(EqualsConverter), ConverterTestLayer.Xaml)]
    public void ObjectXaml(WpfPlatformCase scenario) => WpfXamlRunner.Verify(scenario);

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void ObjectInputBoundaries(WpfPlatformCase scenario) =>
        WpfBoundaryAssertions.Verify(scenario);

    public static IEnumerable<object[]> BehaviorCases => EqualsConverterSuite.Cases;

    [TestMethod]
    [DynamicData(
        nameof(BehaviorCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(EqualsConverter), ConverterTestLayer.Behavior)]
    public void ObjectBehavior(SingleConversionCase<EqualsConverterSuite.Configuration> scenario) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new EqualsConverter();
                return WpfConversionAdapter.Single(
                    configuration,
                    converter,
                    typeof(EqualsConverter),
                    settings =>
                    {
                        if (settings.PreferredCulture is { } culture)
                            converter.PreferredCulture = culture;
                        if (settings.GetOther is not null)
                            converter.Other = WpfConversionAdapter.ToPlatform(settings.GetOther());
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

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() => new EqualsConverter());
        WpfPropertyAssertions.Probe(
            () => new EqualsConverter(),
            EqualsConverter.OtherProperty,
            (c, value) => c.Other = value,
            c => c.Other,
            "match",
            "other",
            c => c.Convert("match", null, null, null),
            true,
            false
        );
        WpfPropertyAssertions.Probe(
            () => new EqualsConverter() { Other = 2 },
            EqualsConverter.IsStringEqualsProperty,
            (c, value) => c.IsStringEquals = value,
            c => c.IsStringEquals,
            true,
            false,
            c => c.Convert("2", null, null, null),
            true,
            false
        );
        WpfPropertyAssertions.Probe(
            () => new EqualsConverter() { Other = "match" },
            EqualsConverter.IsInvertedProperty,
            (c, value) => c.IsInverted = value,
            c => c.IsInverted,
            true,
            false,
            c => c.Convert("match", null, null, null),
            false,
            true
        );
    }
}
