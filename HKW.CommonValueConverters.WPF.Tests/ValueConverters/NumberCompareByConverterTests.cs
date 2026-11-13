using System.Globalization;
using System.Windows;
using System.Windows.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using HKW.HKWUtils;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class NumberCompareByConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "NumberCompareByConverter",
                    typeof(NumberCompareByConverter),
                    () => S<NumberCompareByConverter>(6, 4, true, false, ">5"),
                    "NumberCompareByConverter.xaml",
                    "fallback",
                    (expected, actual) =>
                    {
                        var e = (NumberCompareByConverter)expected;
                        var a = (NumberCompareByConverter)actual;
                        WpfAssert.AssertResult(e.NumberType, a.NumberType);
                        WpfAssert.AssertResult(e.ComparisonType, a.ComparisonType);
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
    [ConverterTest(typeof(NumberCompareByConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(NumberCompareByConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(NumberCompareByConverter), ConverterTestLayer.Xaml)]
    public void ObjectXaml(WpfPlatformCase scenario) => WpfXamlRunner.Verify(scenario);

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void ObjectInputBoundaries(WpfPlatformCase scenario) =>
        WpfBoundaryAssertions.Verify(scenario);

    public static IEnumerable<object[]> BehaviorCases => NumberCompareByConverterSuite.Cases;

    [TestMethod]
    [DynamicData(
        nameof(BehaviorCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(NumberCompareByConverter), ConverterTestLayer.Behavior)]
    public void ObjectBehavior(
        SingleConversionCase<NumberCompareByConverterSuite.Configuration> scenario
    ) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new NumberCompareByConverter();
                return WpfConversionAdapter.Single(
                    configuration,
                    converter,
                    typeof(NumberCompareByConverter),
                    settings =>
                    {
                        if (settings.PreferredCulture is { } culture)
                            converter.PreferredCulture = culture;
                        if (settings.GetNumberType is not null)
                            converter.NumberType = settings.GetNumberType();
                        if (settings.GetComparisonType is not null)
                            converter.ComparisonType = settings.GetComparisonType();
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
        WpfPropertyAssertions.VerifyInherited(() => new NumberCompareByConverter());
        WpfPropertyAssertions.Probe(
            () => new NumberCompareByConverter(),
            NumberCompareByConverter.NumberTypeProperty,
            (c, value) => c.NumberType = value,
            c => c.NumberType,
            NumberType.Double,
            NumberType.Int32,
            c => c.Convert("1.5", null, ">1", CultureInfo.InvariantCulture),
            true,
            DependencyProperty.UnsetValue
        );
        WpfPropertyAssertions.Probe(
            () => new NumberCompareByConverter(),
            NumberCompareByConverter.ComparisonTypeProperty,
            (c, value) => c.ComparisonType = value,
            c => c.ComparisonType,
            ComparisonOperatorType.GreaterThan,
            ComparisonOperatorType.LessThan,
            c => c.Convert(5, null, 2, null),
            true,
            false
        );
    }
}
