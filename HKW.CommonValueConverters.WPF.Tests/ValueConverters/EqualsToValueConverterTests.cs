using System.Windows.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class EqualsToValueConverterTests
{
    public static IEnumerable<object[]> StringCases =>
        [
            [
                new WpfPlatformCase(
                    "EqualsToValueConverter_String",
                    typeof(EqualsToValueConverter<string>),
                    () =>
                        S<EqualsToValueConverter<string>>(
                            "match",
                            "other",
                            "yes",
                            "no",
                            factory: () =>
                                new()
                                {
                                    Other = "match",
                                    TrueValue = "yes",
                                    FalseValue = "no",
                                    NullValue = "null",
                                }
                        ),
                    "EqualsToValueConverter.xaml",
                    "null",
                    (expected, actual) =>
                    {
                        var e = (EqualsToValueConverter<string>)expected;
                        var a = (EqualsToValueConverter<string>)actual;
                        WpfAssert.AssertResult(e.Other, a.Other);
                        WpfAssert.AssertResult(e.TrueValue, a.TrueValue);
                        WpfAssert.AssertResult(e.FalseValue, a.FalseValue);
                        WpfAssert.AssertResult(e.NullValue, a.NullValue);
                        WpfAssert.AssertResult(e.IsStringEquals, a.IsStringEquals);
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
    [ConverterTest(typeof(EqualsToValueConverter<>), ConverterTestLayer.Properties)]
    public void StringProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(StringCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(EqualsToValueConverter<>), ConverterTestLayer.Binding)]
    public void StringBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(StringCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(EqualsToValueConverter<>), ConverterTestLayer.Xaml)]
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
                    "EqualsToValueConverter",
                    typeof(EqualsToValueConverter),
                    () =>
                        S<EqualsToValueConverter>("match", "other", "yes", "no", factory: () =>
                            new()
                            {
                                Other = "match",
                                TrueValue = "yes",
                                FalseValue = "no",
                                NullValue = "null",
                            }
                        ),
                    "EqualsToValueConverter.xaml",
                    "null",
                    (expected, actual) =>
                    {
                        var e = (EqualsToValueConverter)expected;
                        var a = (EqualsToValueConverter)actual;
                        WpfAssert.AssertResult(e.Other, a.Other);
                        WpfAssert.AssertResult(e.TrueValue, a.TrueValue);
                        WpfAssert.AssertResult(e.FalseValue, a.FalseValue);
                        WpfAssert.AssertResult(e.NullValue, a.NullValue);
                        WpfAssert.AssertResult(e.IsStringEquals, a.IsStringEquals);
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
    [ConverterTest(typeof(EqualsToValueConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(EqualsToValueConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(EqualsToValueConverter), ConverterTestLayer.Xaml)]
    public void ObjectXaml(WpfPlatformCase scenario) => WpfXamlRunner.Verify(scenario);

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void ObjectInputBoundaries(WpfPlatformCase scenario) =>
        WpfBoundaryAssertions.Verify(scenario);

    public static IEnumerable<object[]> BehaviorCases => EqualsToValueConverterSuite.Cases;

    [TestMethod]
    [DynamicData(
        nameof(BehaviorCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(EqualsToValueConverter<>), ConverterTestLayer.Behavior)]
    public void StringBehavior(
        SingleConversionCase<EqualsToValueConverterSuite.Configuration> scenario
    ) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new EqualsToValueConverter<string>();
                return WpfConversionAdapter.Single(
                    configuration,
                    converter,
                    typeof(EqualsToValueConverter<string>),
                    settings =>
                    {
                        if (settings.PreferredCulture is { } culture)
                            converter.PreferredCulture = culture;
                        if (settings.GetOther is not null)
                            converter.Other = WpfConversionAdapter.ToPlatform(settings.GetOther());
                        if (settings.GetTrueValue is not null)
                            converter.TrueValue = settings.GetTrueValue();
                        if (settings.GetFalseValue is not null)
                            converter.FalseValue = settings.GetFalseValue();
                        if (settings.GetNullValue is not null)
                            converter.NullValue = settings.GetNullValue();
                        if (settings.GetIsStringEquals is not null)
                            converter.IsStringEquals = settings.GetIsStringEquals();
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
    [ConverterTest(typeof(EqualsToValueConverter), ConverterTestLayer.Behavior)]
    public void ObjectBehavior(
        SingleConversionCase<EqualsToValueConverterSuite.Configuration> scenario
    ) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new EqualsToValueConverter();
                return WpfConversionAdapter.Single(
                    configuration,
                    converter,
                    typeof(EqualsToValueConverter),
                    settings =>
                    {
                        if (settings.PreferredCulture is { } culture)
                            converter.PreferredCulture = culture;
                        if (settings.GetOther is not null)
                            converter.Other = WpfConversionAdapter.ToPlatform(settings.GetOther());
                        if (settings.GetTrueValue is not null)
                            converter.TrueValue = settings.GetTrueValue();
                        if (settings.GetFalseValue is not null)
                            converter.FalseValue = settings.GetFalseValue();
                        if (settings.GetNullValue is not null)
                            converter.NullValue = settings.GetNullValue();
                        if (settings.GetIsStringEquals is not null)
                            converter.IsStringEquals = settings.GetIsStringEquals();
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
    public void NullableAndInversionConfiguration_RemainsLive_EqualsToValueConverter() =>
        PlatformThread.Run(() =>
        {
            var equals = new EqualsToValueConverter<int>
            {
                Other = "2",
                TrueValue = 7,
                FalseValue = 3,
                NullValue = 5,
            };
            Assert.AreEqual(3, equals.Convert(2, null, null, null));
            equals.IsStringEquals = true;
            Assert.AreEqual(7, equals.Convert(2, null, null, null));
            Assert.AreEqual(5, equals.Convert(null, null, null, null));
        });

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() => new EqualsToValueConverter());
        WpfPropertyAssertions.Probe(
            () => new EqualsToValueConverter<string>() { Other = "match" },
            EqualsToValueConverter<string>.TrueValueProperty,
            (c, value) => c.TrueValue = value,
            c => c.TrueValue,
            "one",
            "two",
            c => c.Convert("match", null, null, null),
            "one",
            "two"
        );
        WpfPropertyAssertions.Probe(
            () => new EqualsToValueConverter<string>() { Other = "match" },
            EqualsToValueConverter<string>.FalseValueProperty,
            (c, value) => c.FalseValue = value,
            c => c.FalseValue,
            "one",
            "two",
            c => c.Convert("other", null, null, null),
            "one",
            "two"
        );
        WpfPropertyAssertions.Probe(
            () => new EqualsToValueConverter<string>() { Other = "match" },
            EqualsToValueConverter<string>.NullValueProperty,
            (c, value) => c.NullValue = value,
            c => c.NullValue,
            "one",
            "two",
            c => c.Convert(null, null, null, null),
            "one",
            "two"
        );
        WpfPropertyAssertions.Probe(
            () => new EqualsToValueConverter<string>() { TrueValue = "one", FalseValue = "two" },
            EqualsToValueConverter<string>.OtherProperty,
            (c, value) => c.Other = value,
            c => c.Other,
            "match",
            "other",
            c => c.Convert("match", null, null, null),
            "one",
            "two"
        );
        WpfPropertyAssertions.Probe(
            () =>
                new EqualsToValueConverter<string>()
                {
                    Other = 2,
                    TrueValue = "one",
                    FalseValue = "two",
                },
            EqualsToValueConverter<string>.IsStringEqualsProperty,
            (c, value) => c.IsStringEquals = value,
            c => c.IsStringEquals,
            true,
            false,
            c => c.Convert("2", null, null, null),
            "one",
            "two"
        );
        WpfPropertyAssertions.Probe(
            () => new EqualsToValueConverter() { Other = "match" },
            EqualsToValueConverter.TrueValueProperty,
            (c, value) => c.TrueValue = value,
            c => c.TrueValue,
            "one",
            "two",
            c => c.Convert("match", null, null, null),
            "one",
            "two"
        );
        WpfPropertyAssertions.Probe(
            () => new EqualsToValueConverter() { Other = "match" },
            EqualsToValueConverter.FalseValueProperty,
            (c, value) => c.FalseValue = value,
            c => c.FalseValue,
            "one",
            "two",
            c => c.Convert("other", null, null, null),
            "one",
            "two"
        );
        WpfPropertyAssertions.Probe(
            () => new EqualsToValueConverter() { Other = "match" },
            EqualsToValueConverter.NullValueProperty,
            (c, value) => c.NullValue = value,
            c => c.NullValue,
            "one",
            "two",
            c => c.Convert(null, null, null, null),
            "one",
            "two"
        );
        WpfPropertyAssertions.Probe(
            () => new EqualsToValueConverter() { TrueValue = "one", FalseValue = "two" },
            EqualsToValueConverter.OtherProperty,
            (c, value) => c.Other = value,
            c => c.Other,
            "match",
            "other",
            c => c.Convert("match", null, null, null),
            "one",
            "two"
        );
        WpfPropertyAssertions.Probe(
            () =>
                new EqualsToValueConverter()
                {
                    Other = 2,
                    TrueValue = "one",
                    FalseValue = "two",
                },
            EqualsToValueConverter.IsStringEqualsProperty,
            (c, value) => c.IsStringEquals = value,
            c => c.IsStringEquals,
            true,
            false,
            c => c.Convert("2", null, null, null),
            "one",
            "two"
        );
        WpfPropertyAssertions.Probe(
            () => new EqualsToValueConverter<int>() { Other = "match" },
            EqualsToValueConverter<int>.TrueValueProperty,
            (c, value) => c.TrueValue = value,
            c => c.TrueValue,
            7,
            9,
            c => c.Convert("match", null, null, null),
            7,
            9
        );
        WpfPropertyAssertions.Probe(
            () => new EqualsToValueConverter<int>() { Other = "match" },
            EqualsToValueConverter<int>.FalseValueProperty,
            (c, value) => c.FalseValue = value,
            c => c.FalseValue,
            7,
            9,
            c => c.Convert("other", null, null, null),
            7,
            9
        );
        WpfPropertyAssertions.Probe(
            () => new EqualsToValueConverter<int>() { Other = "match" },
            EqualsToValueConverter<int>.NullValueProperty,
            (c, value) => c.NullValue = value,
            c => c.NullValue,
            7,
            9,
            c => c.Convert(null, null, null, null),
            7,
            9
        );
        WpfPropertyAssertions.Probe(
            () => new EqualsToValueConverter<int>() { TrueValue = 7, FalseValue = 9 },
            EqualsToValueConverter<int>.OtherProperty,
            (c, value) => c.Other = value,
            c => c.Other,
            "match",
            "other",
            c => c.Convert("match", null, null, null),
            7,
            9
        );
        WpfPropertyAssertions.Probe(
            () =>
                new EqualsToValueConverter<int>()
                {
                    Other = 2,
                    TrueValue = 7,
                    FalseValue = 9,
                },
            EqualsToValueConverter<int>.IsStringEqualsProperty,
            (c, value) => c.IsStringEquals = value,
            c => c.IsStringEquals,
            true,
            false,
            c => c.Convert("2", null, null, null),
            7,
            9
        );
    }
}
