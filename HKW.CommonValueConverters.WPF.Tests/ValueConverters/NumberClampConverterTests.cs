using System.Globalization;
using System.Windows;
using System.Windows.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using HKW.HKWUtils;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class NumberClampConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "NumberClampConverter",
                    typeof(NumberClampConverter),
                    () => S<NumberClampConverter>(5, 9, true, false, "2,8"),
                    "NumberClampConverter.xaml",
                    "fallback",
                    (expected, actual) =>
                    {
                        var e = (NumberClampConverter)expected;
                        var a = (NumberClampConverter)actual;
                        WpfAssert.AssertResult(e.NumberType, a.NumberType);
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
    [ConverterTest(typeof(NumberClampConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(NumberClampConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(NumberClampConverter), ConverterTestLayer.Xaml)]
    public void ObjectXaml(WpfPlatformCase scenario) => WpfXamlRunner.Verify(scenario);

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void ObjectInputBoundaries(WpfPlatformCase scenario) =>
        WpfBoundaryAssertions.Verify(scenario);

    public static IEnumerable<object[]> BehaviorCases => NumberClampConverterSuite.Cases;

    [TestMethod]
    [DynamicData(
        nameof(BehaviorCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(NumberClampConverter), ConverterTestLayer.Behavior)]
    public void ObjectBehavior(
        SingleConversionCase<NumberClampConverterSuite.Configuration> scenario
    ) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new NumberClampConverter();
                return WpfConversionAdapter.Single(
                    configuration,
                    converter,
                    typeof(NumberClampConverter),
                    settings =>
                    {
                        if (settings.PreferredCulture is { } culture)
                            converter.PreferredCulture = culture;
                        if (settings.GetNumberType is not null)
                            converter.NumberType = settings.GetNumberType();
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
        WpfPropertyAssertions.VerifyInherited(() => new NumberClampConverter());
        WpfPropertyAssertions.Probe(
            () => new NumberClampConverter(),
            NumberClampConverter.NumberTypeProperty,
            (c, value) => c.NumberType = value,
            c => c.NumberType,
            NumberType.Double,
            NumberType.Int32,
            c => c.Convert("1.5", null, "1,2", CultureInfo.InvariantCulture),
            true,
            DependencyProperty.UnsetValue
        );
    }
}
