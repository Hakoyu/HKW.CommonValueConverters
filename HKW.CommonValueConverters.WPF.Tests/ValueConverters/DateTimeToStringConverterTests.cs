using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class DateTimeToStringConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "DateTimeToStringConverter",
                    typeof(DateTimeToStringConverter),
                    () =>
                        S<DateTimeToStringConverter>(
                            new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Local),
                            new DateTime(2027, 3, 4, 0, 0, 0, DateTimeKind.Local),
                            "2026-01-02",
                            "2027-03-04",
                            factory: () => new() { Format = "yyyy-MM-dd" }
                        ),
                    "DateTimeToStringConverter.xaml",
                    "fallback",
                    (expected, actual) =>
                    {
                        var e = (DateTimeToStringConverter)expected;
                        var a = (DateTimeToStringConverter)actual;
                        WpfAssert.AssertResult(e.Format, a.Format);
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
    [ConverterTest(typeof(DateTimeToStringConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(DateTimeToStringConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(DateTimeToStringConverter), ConverterTestLayer.Xaml)]
    public void ObjectXaml(WpfPlatformCase scenario) => WpfXamlRunner.Verify(scenario);

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void ObjectInputBoundaries(WpfPlatformCase scenario) =>
        WpfBoundaryAssertions.Verify(scenario);

    public static IEnumerable<object[]> BehaviorCases => DateTimeToStringConverterSuite.Cases;

    [TestMethod]
    [DynamicData(
        nameof(BehaviorCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(DateTimeToStringConverter), ConverterTestLayer.Behavior)]
    public void ObjectBehavior(
        SingleConversionCase<DateTimeToStringConverterSuite.Configuration> scenario
    ) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new DateTimeToStringConverter();
                return WpfConversionAdapter.Single(
                    configuration,
                    converter,
                    typeof(DateTimeToStringConverter),
                    settings =>
                    {
                        if (settings.PreferredCulture is { } culture)
                            converter.PreferredCulture = culture;
                        if (settings.GetFormat is not null)
                            converter.Format = settings.GetFormat();
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
    public void DateTimeToStringConverter_TypedTextBoxRoundTrip() =>
        PlatformThread.Run(() =>
        {
            var first = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Local);
            var next = new DateTime(2027, 3, 4, 0, 0, 0, DateTimeKind.Local).ToUniversalTime();
            WpfTwoWayBindingRunner.Verify(
                new DateTimeToStringConverter { Format = "yyyy-MM-dd" },
                new TextBox(),
                TextBox.TextProperty,
                first,
                "2026-01-02",
                "2027-03-04",
                next
            );
        });

    [TestMethod]
    public void DatesTimesAndMargin_UseExplicitCultureAndFormatPriority_DateTimeToStringConverter() =>
        PlatformThread.Run(() =>
        {
            var french = CultureInfo.GetCultureInfo("fr-FR");
            var date = new DateTimeToStringConverter
            {
                Format = "dd MMMM yyyy",
                PreferredCulture = Core.PreferredCulture.ConverterCulture,
            };
            Assert.AreEqual(
                "04 septembre 2026",
                date.Convert(new DateTime(2026, 9, 4), null, null, french)
            );
            Assert.AreEqual(
                "2026/09",
                date.Convert(new DateTime(2026, 9, 4), null, "yyyy/MM", french)
            );
        });

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() => new DateTimeToStringConverter());
        WpfPropertyAssertions.Probe(
            () => new DateTimeToStringConverter(),
            DateTimeToStringConverter.FormatProperty,
            (c, value) => c.Format = value,
            c => c.Format,
            "yyyy",
            "MM",
            c => c.Convert(new DateTime(2026, 9, 4), null, null, CultureInfo.InvariantCulture),
            "2026",
            "09"
        );
    }
}
