using System.Windows.Controls;
using System.Windows.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class GuidToStringConverterTests
{
    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "GuidToStringConverter",
                    typeof(GuidToStringConverter),
                    () =>
                        S<GuidToStringConverter>(
                            Guid.Parse("01234567-89ab-cdef-0123-456789abcdef"),
                            Guid.Empty,
                            "0123456789abcdef0123456789abcdef",
                            "00000000000000000000000000000000",
                            factory: () => new() { Format = "N", StringTo = false }
                        ),
                    "GuidToStringConverter.xaml",
                    "fallback",
                    (expected, actual) =>
                    {
                        var e = (GuidToStringConverter)expected;
                        var a = (GuidToStringConverter)actual;
                        WpfAssert.AssertResult(e.StringTo, a.StringTo);
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
    [ConverterTest(typeof(GuidToStringConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(GuidToStringConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(GuidToStringConverter), ConverterTestLayer.Xaml)]
    public void ObjectXaml(WpfPlatformCase scenario) => WpfXamlRunner.Verify(scenario);

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void ObjectInputBoundaries(WpfPlatformCase scenario) =>
        WpfBoundaryAssertions.Verify(scenario);

    public static IEnumerable<object[]> BehaviorCases => GuidToStringConverterSuite.Cases;

    [TestMethod]
    [DynamicData(
        nameof(BehaviorCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(GuidToStringConverter), ConverterTestLayer.Behavior)]
    public void ObjectBehavior(
        SingleConversionCase<GuidToStringConverterSuite.Configuration> scenario
    ) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new GuidToStringConverter();
                return WpfConversionAdapter.Single(
                    configuration,
                    converter,
                    typeof(GuidToStringConverter),
                    settings =>
                    {
                        if (settings.PreferredCulture is { } culture)
                            converter.PreferredCulture = culture;
                        if (settings.GetStringTo is not null)
                            converter.StringTo = settings.GetStringTo();
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
    public void GuidToStringConverter_TypedTextBoxRoundTrip() =>
        PlatformThread.Run(() =>
            WpfTwoWayBindingRunner.Verify(
                new GuidToStringConverter { Format = "D", StringTo = false },
                new TextBox(),
                TextBox.TextProperty,
                Guid.Empty,
                "00000000-0000-0000-0000-000000000000",
                "01234567-89ab-cdef-0123-456789abcdef",
                Guid.Parse("01234567-89ab-cdef-0123-456789abcdef")
            )
        );

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() => new GuidToStringConverter());
        WpfPropertyAssertions.Probe(
            () => new GuidToStringConverter() { StringTo = false },
            GuidToStringConverter.FormatProperty,
            (c, value) => c.Format = value,
            c => c.Format,
            "N",
            "D",
            c => c.Convert(Guid.Empty, null, null, null),
            "00000000000000000000000000000000",
            "00000000-0000-0000-0000-000000000000"
        );
        WpfPropertyAssertions.Probe(
            () => new GuidToStringConverter() { Format = "D" },
            GuidToStringConverter.StringToProperty,
            (c, value) => c.StringTo = value,
            c => c.StringTo,
            true,
            false,
            c => c.Convert(Guid.Parse("01234567-89ab-cdef-0123-456789abcdef"), null, null, null),
            "01234567-89AB-CDEF-0123-456789ABCDEF",
            "01234567-89ab-cdef-0123-456789abcdef"
        );
    }
}
