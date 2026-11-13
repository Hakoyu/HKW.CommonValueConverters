using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class DateTimeOffsetToStringConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "DateTimeOffsetToStringConverter",
                    typeof(DateTimeOffsetToStringConverter),
                    () =>
                        S<DateTimeOffsetToStringConverter>(
                            new DateTimeOffset(
                                new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Local)
                            ),
                            new DateTimeOffset(
                                new DateTime(2027, 3, 4, 0, 0, 0, DateTimeKind.Local)
                            ),
                            "2026-01-02",
                            "2027-03-04",
                            factory: () => new() { Format = "yyyy-MM-dd" }
                        ),
                    "DateTimeOffsetToStringConverter.xaml",
                    "fallback",
                    (expected, actual) =>
                    {
                        var e = (DateTimeOffsetToStringConverter)expected;
                        var a = (DateTimeOffsetToStringConverter)actual;
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
    [ConverterTest(typeof(DateTimeOffsetToStringConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(DateTimeOffsetToStringConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(DateTimeOffsetToStringConverter), ConverterTestLayer.Xaml)]
    public void ObjectXaml(WpfPlatformCase scenario) => WpfXamlRunner.Verify(scenario);

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void ObjectInputBoundaries(WpfPlatformCase scenario) =>
        WpfBoundaryAssertions.Verify(scenario);

    public static IEnumerable<object[]> BehaviorCases => DateTimeOffsetToStringConverterSuite.Cases;

    [TestMethod]
    [DynamicData(
        nameof(BehaviorCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(DateTimeOffsetToStringConverter), ConverterTestLayer.Behavior)]
    public void ObjectBehavior(
        SingleConversionCase<DateTimeOffsetToStringConverterSuite.Configuration> scenario
    ) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new DateTimeOffsetToStringConverter();
                return WpfConversionAdapter.Single(
                    configuration,
                    converter,
                    typeof(DateTimeOffsetToStringConverter),
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
    public void DateTimeOffsetToStringConverter_TypedTextBoxRoundTrip() =>
        PlatformThread.Run(() =>
            WpfTwoWayBindingRunner.Verify(
                new DateTimeOffsetToStringConverter { Format = "yyyy-MM-dd" },
                new TextBox(),
                TextBox.TextProperty,
                new DateTimeOffset(new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Local)),
                "2026-01-02",
                "2027-03-04",
                new DateTimeOffset(
                    new DateTime(2027, 3, 4, 0, 0, 0, DateTimeKind.Local)
                ).ToUniversalTime()
            )
        );

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() => new DateTimeOffsetToStringConverter());
        WpfPropertyAssertions.Probe(
            () => new DateTimeOffsetToStringConverter(),
            DateTimeOffsetToStringConverter.FormatProperty,
            (c, value) => c.Format = value,
            c => c.Format,
            "yyyy",
            "MM",
            c =>
                c.Convert(
                    new DateTimeOffset(new DateTime(2026, 9, 4)),
                    null,
                    null,
                    CultureInfo.InvariantCulture
                ),
            "2026",
            "09"
        );
    }
}
