using System.Windows.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using HKW.HKWUtils;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class EnumToEnumInfoDisplayConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "EnumToEnumInfoDisplayConverter",
                    typeof(EnumToEnumInfoDisplayConverter),
                    () =>
                        S<EnumToEnumInfoDisplayConverter>(
                            DayOfWeek.Monday,
                            DayOfWeek.Tuesday,
                            "Monday",
                            "Tuesday"
                        ),
                    "EnumToEnumInfoDisplayConverter.xaml",
                    "fallback",
                    (expected, actual) =>
                    {
                        var e = (EnumToEnumInfoDisplayConverter)expected;
                        var a = (EnumToEnumInfoDisplayConverter)actual;
                        WpfAssert.AssertResult(e.EnumInfoDisplayTarget, a.EnumInfoDisplayTarget);
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
    [ConverterTest(typeof(EnumToEnumInfoDisplayConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(EnumToEnumInfoDisplayConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(EnumToEnumInfoDisplayConverter), ConverterTestLayer.Xaml)]
    public void ObjectXaml(WpfPlatformCase scenario) => WpfXamlRunner.Verify(scenario);

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void ObjectInputBoundaries(WpfPlatformCase scenario) =>
        WpfBoundaryAssertions.Verify(scenario);

    public static IEnumerable<object[]> BehaviorCases => EnumToEnumInfoDisplayConverterSuite.Cases;

    [TestMethod]
    [DynamicData(
        nameof(BehaviorCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(EnumToEnumInfoDisplayConverter), ConverterTestLayer.Behavior)]
    public void ObjectBehavior(
        SingleConversionCase<EnumToEnumInfoDisplayConverterSuite.Configuration> scenario
    ) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new EnumToEnumInfoDisplayConverter();
                return WpfConversionAdapter.Single(
                    configuration,
                    converter,
                    typeof(EnumToEnumInfoDisplayConverter),
                    settings =>
                    {
                        if (settings.PreferredCulture is { } culture)
                            converter.PreferredCulture = culture;
                        if (settings.GetEnumInfoDisplayTarget is not null)
                            converter.EnumInfoDisplayTarget = settings.GetEnumInfoDisplayTarget();
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
        WpfPropertyAssertions.VerifyInherited(() => new EnumToEnumInfoDisplayConverter());
        WpfPropertyAssertions.Probe(
            () => new EnumToEnumInfoDisplayConverter(),
            EnumToEnumInfoDisplayConverter.EnumInfoDisplayTargetProperty,
            (c, value) => c.EnumInfoDisplayTarget = value,
            c => c.EnumInfoDisplayTarget,
            EnumInfoDisplayTarget.Name,
            EnumInfoDisplayTarget.ShortName,
            c => c.Convert(DisplayValue.First, null, null, null),
            "wire-name",
            "wire-short"
        );
    }

    private enum DisplayValue
    {
        [System.ComponentModel.DataAnnotations.Display(
            Name = "wire-name",
            ShortName = "wire-short"
        )]
        First,
    }
}
