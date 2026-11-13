using System.Windows.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class AllEqualsMultiConverterTests
{
    public static IEnumerable<object[]> StringCases =>
        [
            [
                new WpfPlatformCase(
                    "AllEqualsMultiConverter_String",
                    typeof(AllEqualsMultiConverter<string>),
                    () =>
                        M<AllEqualsMultiConverter<string>>(
                            [
                                ["match", "match"],
                                ["other", "match"],
                                ["other", "other"],
                                ["match", "other"],
                                ["match", "match"],
                            ],
                            [true, false, false, false, true],
                            factory: () => new() { Value = "match" }
                        ),
                    "AllEqualsMultiConverter.xaml",
                    "fallback",
                    (expected, actual) =>
                    {
                        var e = (AllEqualsMultiConverter<string>)expected;
                        var a = (AllEqualsMultiConverter<string>)actual;
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
    [ConverterTest(typeof(AllEqualsMultiConverter<>), ConverterTestLayer.Properties)]
    public void StringProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(StringCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(AllEqualsMultiConverter<>), ConverterTestLayer.Binding)]
    public void StringBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(StringCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(AllEqualsMultiConverter<>), ConverterTestLayer.Xaml)]
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
                    "AllEqualsMultiConverter",
                    typeof(AllEqualsMultiConverter),
                    () =>
                        M<AllEqualsMultiConverter>(
                            [
                                ["match", "match"],
                                ["other", "match"],
                                ["other", "other"],
                                ["match", "other"],
                                ["match", "match"],
                            ],
                            [true, false, false, false, true],
                            factory: () => new() { Value = "match" }
                        ),
                    "AllEqualsMultiConverter.xaml",
                    "fallback",
                    (expected, actual) =>
                    {
                        var e = (AllEqualsMultiConverter)expected;
                        var a = (AllEqualsMultiConverter)actual;
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
    [ConverterTest(typeof(AllEqualsMultiConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(AllEqualsMultiConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(AllEqualsMultiConverter), ConverterTestLayer.Xaml)]
    public void ObjectXaml(WpfPlatformCase scenario) => WpfXamlRunner.Verify(scenario);

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void ObjectInputBoundaries(WpfPlatformCase scenario) =>
        WpfBoundaryAssertions.Verify(scenario);

    public static IEnumerable<object[]> BehaviorCases => AllEqualsMultiConverterSuite.Cases;

    [TestMethod]
    [DynamicData(
        nameof(BehaviorCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(AllEqualsMultiConverter<>), ConverterTestLayer.Behavior)]
    public void StringBehavior(
        MultiConversionCase<AllEqualsMultiConverterSuite.Configuration> scenario
    ) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new AllEqualsMultiConverter<string>();
                return WpfConversionAdapter.Multi(
                    configuration,
                    converter,
                    typeof(AllEqualsMultiConverter<string>),
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
    [ConverterTest(typeof(AllEqualsMultiConverter), ConverterTestLayer.Behavior)]
    public void ObjectBehavior(
        MultiConversionCase<AllEqualsMultiConverterSuite.Configuration> scenario
    ) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new AllEqualsMultiConverter();
                return WpfConversionAdapter.Multi(
                    configuration,
                    converter,
                    typeof(AllEqualsMultiConverter),
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
    public void NullableAndInversionConfiguration_RemainsLive_AllEqualsMultiConverter() =>
        PlatformThread.Run(() =>
        {
            var all = new AllEqualsMultiConverter<int> { Value = 2 };
            Assert.AreEqual(true, all.Convert([2, 2], null, null, null));
            all.IsInverted = true;
            Assert.AreEqual(false, all.Convert([2, 2], null, null, null));
        });

    [TestMethod]
    public void MultiValueBoundaries_EmptyShortAndSelectedBranchInputs_1() =>
        PlatformThread.Run(() =>
        {
            Assert.AreEqual(
                true,
                new AllEqualsMultiConverter<string>().Convert([], null, null, null)
            );
        });

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() => new AllEqualsMultiConverter());
        WpfPropertyAssertions.Probe(
            () => new AllEqualsMultiConverter<string>(),
            AllEqualsMultiConverter<string>.ValueProperty,
            (c, value) => c.Value = value,
            c => c.Value,
            "match",
            "other",
            c => c.Convert(["match", "match"], null, null, null),
            true,
            false
        );
        WpfPropertyAssertions.Probe(
            () => new AllEqualsMultiConverter<string>() { Value = "match" },
            AllEqualsMultiConverter<string>.IsInvertedProperty,
            (c, value) => c.IsInverted = value,
            c => c.IsInverted,
            true,
            false,
            c => c.Convert(["match", "match"], null, null, null),
            false,
            true
        );
        WpfPropertyAssertions.Probe(
            () => new AllEqualsMultiConverter(),
            AllEqualsMultiConverter.ValueProperty,
            (c, value) => c.Value = value,
            c => c.Value,
            "match",
            "other",
            c => c.Convert(["match", "match"], null, null, null),
            true,
            false
        );
        WpfPropertyAssertions.Probe(
            () => new AllEqualsMultiConverter() { Value = 2 },
            AllEqualsMultiConverter.IsStringEqualsProperty,
            (c, value) => c.IsStringEquals = value,
            c => c.IsStringEquals,
            true,
            false,
            c => c.Convert(["2", "2"], null, null, null),
            true,
            false
        );
        WpfPropertyAssertions.Probe(
            () => new AllEqualsMultiConverter() { Value = "match" },
            AllEqualsMultiConverter.IsInvertedProperty,
            (c, value) => c.IsInverted = value,
            c => c.IsInverted,
            true,
            false,
            c => c.Convert(["match", "match"], null, null, null),
            false,
            true
        );
    }
}
