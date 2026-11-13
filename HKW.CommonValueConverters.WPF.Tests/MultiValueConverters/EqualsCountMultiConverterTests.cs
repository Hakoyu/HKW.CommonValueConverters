using System.Windows.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class EqualsCountMultiConverterTests
{
    public static IEnumerable<object[]> StringCases =>
        [
            [
                new WpfPlatformCase(
                    "EqualsCountMultiConverter_String",
                    typeof(EqualsCountMultiConverter<string>),
                    () =>
                        M<EqualsCountMultiConverter<string>>(
                            [
                                ["match", "other"],
                                ["other", "other"],
                                ["other", "match"],
                            ],
                            [1, 0, 1],
                            factory: () => new() { Value = "match" }
                        ),
                    "EqualsCountMultiConverter.xaml",
                    "fallback",
                    (expected, actual) =>
                    {
                        var e = (EqualsCountMultiConverter<string>)expected;
                        var a = (EqualsCountMultiConverter<string>)actual;
                        WpfAssert.AssertResult(e.Value, a.Value);
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
        nameof(StringCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(EqualsCountMultiConverter<>), ConverterTestLayer.Properties)]
    public void StringProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(StringCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(EqualsCountMultiConverter<>), ConverterTestLayer.Binding)]
    public void StringBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(StringCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(EqualsCountMultiConverter<>), ConverterTestLayer.Xaml)]
    public void StringXaml(WpfPlatformCase scenario) => WpfXamlRunner.Verify(scenario);

    [TestMethod]
    [DynamicData(
        nameof(StringCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void StringInputBoundaries(WpfPlatformCase scenario) =>
        WpfBoundaryAssertions.Verify(scenario);

    public static IEnumerable<object[]> ObjectCases =>
        [
            [
                new WpfPlatformCase(
                    "EqualsCountMultiConverter",
                    typeof(EqualsCountMultiConverter),
                    () =>
                        M<EqualsCountMultiConverter>(
                            [
                                ["match", "other"],
                                ["other", "other"],
                                ["other", "match"],
                            ],
                            [1, 0, 1],
                            factory: () => new() { Value = "match" }
                        ),
                    "EqualsCountMultiConverter.xaml",
                    "fallback",
                    (expected, actual) =>
                    {
                        var e = (EqualsCountMultiConverter)expected;
                        var a = (EqualsCountMultiConverter)actual;
                        WpfAssert.AssertResult(e.Value, a.Value);
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
    [ConverterTest(typeof(EqualsCountMultiConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(EqualsCountMultiConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(EqualsCountMultiConverter), ConverterTestLayer.Xaml)]
    public void ObjectXaml(WpfPlatformCase scenario) => WpfXamlRunner.Verify(scenario);

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void ObjectInputBoundaries(WpfPlatformCase scenario) =>
        WpfBoundaryAssertions.Verify(scenario);

    public static IEnumerable<object[]> BehaviorCases => EqualsCountMultiConverterSuite.Cases;

    [TestMethod]
    [DynamicData(
        nameof(BehaviorCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(EqualsCountMultiConverter<>), ConverterTestLayer.Behavior)]
    public void StringBehavior(
        MultiConversionCase<EqualsCountMultiConverterSuite.Configuration> scenario
    ) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new EqualsCountMultiConverter<string>();
                return WpfConversionAdapter.Multi(
                    configuration,
                    converter,
                    typeof(EqualsCountMultiConverter<string>),
                    settings =>
                    {
                        if (settings.PreferredCulture is { } culture)
                            converter.PreferredCulture = culture;
                        if (settings.GetValue is not null)
                            converter.Value = settings.GetValue();
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
        nameof(BehaviorCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(EqualsCountMultiConverter), ConverterTestLayer.Behavior)]
    public void ObjectBehavior(
        MultiConversionCase<EqualsCountMultiConverterSuite.Configuration> scenario
    ) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new EqualsCountMultiConverter();
                return WpfConversionAdapter.Multi(
                    configuration,
                    converter,
                    typeof(EqualsCountMultiConverter),
                    settings =>
                    {
                        if (settings.PreferredCulture is { } culture)
                            converter.PreferredCulture = culture;
                        if (settings.GetValue is not null)
                            converter.Value = settings.GetValue();
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
        nameof(StringCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void StringUnsupportedReverse(WpfPlatformCase scenario) =>
        WpfPlatformBehavior.VerifyUnsupportedReverse(scenario);

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
    public void MultiValueBoundaries_EmptyShortAndSelectedBranchInputs_0() =>
        PlatformThread.Run(() =>
        {
            Assert.AreEqual(
                0,
                new EqualsCountMultiConverter<string>().Convert([], null, null, null)
            );
        });

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() => new EqualsCountMultiConverter());
        WpfPropertyAssertions.Probe(
            () => new EqualsCountMultiConverter<string>(),
            EqualsCountMultiConverter<string>.ValueProperty,
            (c, value) => c.Value = value,
            c => c.Value,
            "match",
            "other",
            c => c.Convert(["match", "match"], null, null, null),
            2,
            0
        );
        WpfPropertyAssertions.Probe(
            () => new EqualsCountMultiConverter(),
            EqualsCountMultiConverter.ValueProperty,
            (c, value) => c.Value = value,
            c => c.Value,
            "match",
            "other",
            c => c.Convert(["match", "match"], null, null, null),
            2,
            0
        );
        WpfPropertyAssertions.Probe(
            () => new EqualsCountMultiConverter() { Value = 2 },
            EqualsCountMultiConverter.IsStringEqualsProperty,
            (c, value) => c.IsStringEquals = value,
            c => c.IsStringEquals,
            true,
            false,
            c => c.Convert(["2", "2"], null, null, null),
            2,
            0
        );
    }
}
