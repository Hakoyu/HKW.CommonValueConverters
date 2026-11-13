using System.Globalization;
using System.Windows.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class StringCaseConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "StringCaseConverter",
                    typeof(StringCaseConverter),
                    () => S<StringCaseConverter>("first", "next", "FIRST", "NEXT", "U"),
                    "StringCaseConverter.xaml",
                    "fallback",
                    (expected, actual) =>
                    {
                        var e = (StringCaseConverter)expected;
                        var a = (StringCaseConverter)actual;
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
    [ConverterTest(typeof(StringCaseConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(StringCaseConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(StringCaseConverter), ConverterTestLayer.Xaml)]
    public void ObjectXaml(WpfPlatformCase scenario) => WpfXamlRunner.Verify(scenario);

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void ObjectInputBoundaries(WpfPlatformCase scenario) =>
        WpfBoundaryAssertions.Verify(scenario);

    public static IEnumerable<object[]> BehaviorCases => StringCaseConverterSuite.Cases;

    [TestMethod]
    [DynamicData(
        nameof(BehaviorCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(StringCaseConverter), ConverterTestLayer.Behavior)]
    public void ObjectBehavior(
        SingleConversionCase<StringCaseConverterSuite.Configuration> scenario
    ) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new StringCaseConverter();
                return WpfConversionAdapter.Single(
                    configuration,
                    converter,
                    typeof(StringCaseConverter),
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
    public void GlobalInstanceAndClearedCulturePreferences_AreLive() =>
        PlatformThread.Run(() =>
        {
            var saved = Core.ValueConvertersConfig.DefaultPreferredCulture;
            var ui = CultureInfo.CurrentUICulture;
            var current = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("tr-TR");
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
                var converter = new StringCaseConverter();
                Core.ValueConvertersConfig.DefaultPreferredCulture =
                    Core.PreferredCulture.CurrentUICulture;
                Assert.AreEqual(
                    "İ",
                    converter.Convert("i", null, "U", CultureInfo.InvariantCulture)
                );
                converter.PreferredCulture = Core.PreferredCulture.ConverterCulture;
                Assert.AreEqual(
                    "I",
                    converter.Convert("i", null, "U", CultureInfo.InvariantCulture)
                );
                converter.ClearValue(ConverterBase.PreferredCultureProperty);
                Assert.AreEqual(
                    "İ",
                    converter.Convert("i", null, "U", CultureInfo.InvariantCulture)
                );
                Core.ValueConvertersConfig.DefaultPreferredCulture =
                    Core.PreferredCulture.ConverterCulture;
                Assert.AreEqual(
                    "I",
                    converter.Convert("i", null, "U", CultureInfo.InvariantCulture)
                );
            }
            finally
            {
                Core.ValueConvertersConfig.DefaultPreferredCulture = saved;
                CultureInfo.CurrentUICulture = ui;
                CultureInfo.CurrentCulture = current;
            }
        });

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() => new StringCaseConverter());
    }
}
