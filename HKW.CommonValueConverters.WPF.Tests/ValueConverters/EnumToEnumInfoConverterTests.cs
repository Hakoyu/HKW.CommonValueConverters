using System.Windows.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class EnumToEnumInfoConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "EnumToEnumInfoConverter",
                    typeof(EnumToEnumInfoConverter),
                    () =>
                        S<EnumToEnumInfoConverter>(
                            DayOfWeek.Monday,
                            DayOfWeek.Tuesday,
                            new EnumExpectation(DayOfWeek.Monday),
                            new EnumExpectation(DayOfWeek.Tuesday)
                        ),
                    "EnumToEnumInfoConverter.xaml",
                    "fallback",
                    (expected, actual) =>
                    {
                        var e = (EnumToEnumInfoConverter)expected;
                        var a = (EnumToEnumInfoConverter)actual;
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
    [ConverterTest(typeof(EnumToEnumInfoConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(EnumToEnumInfoConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(EnumToEnumInfoConverter), ConverterTestLayer.Xaml)]
    public void ObjectXaml(WpfPlatformCase scenario) => WpfXamlRunner.Verify(scenario);

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void ObjectInputBoundaries(WpfPlatformCase scenario) =>
        WpfBoundaryAssertions.Verify(scenario);

    public static IEnumerable<object[]> BehaviorCases => EnumToEnumInfoConverterSuite.Cases;

    [TestMethod]
    [DynamicData(
        nameof(BehaviorCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(EnumToEnumInfoConverter), ConverterTestLayer.Behavior)]
    public void ObjectBehavior(
        SingleConversionCase<EnumToEnumInfoConverterSuite.Configuration> scenario
    ) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new EnumToEnumInfoConverter();
                return WpfConversionAdapter.Single(
                    configuration,
                    converter,
                    typeof(EnumToEnumInfoConverter),
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
    public void EnumToEnumInfoConverter_TypedInfoRoundTrip() =>
        PlatformThread.Run(() =>
        {
            var converter = new EnumToEnumInfoConverter();
            // 反向输入由公开枚举元数据生成, 预期值独立指定.
            var nextInfo = HKW.HKWUtils.Extensions.EnumExtensions.GetInfo(DayOfWeek.Tuesday);
            WpfTwoWayBindingRunner.Verify(
                converter,
                new EnumInfoTarget(),
                EnumInfoTarget.InfoProperty,
                DayOfWeek.Monday,
                new EnumExpectation(DayOfWeek.Monday),
                nextInfo,
                DayOfWeek.Tuesday
            );
        });

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() => new EnumToEnumInfoConverter());
    }
}
