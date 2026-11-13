using System.Windows;
using System.Windows.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class AllBoolToValueMultiConverterTests
{
    public static IEnumerable<object[]> StringCases =>
        [
            [
                new WpfPlatformCase(
                    "AllBoolToValueMultiConverter_String",
                    typeof(AllBoolToValueMultiConverter<string>),
                    () =>
                        M<AllBoolToValueMultiConverter<string>>(
                            [
                                [true, true],
                                [false, true],
                                [false, false],
                                [true, false],
                                [true, true],
                            ],
                            ["yes", "mixed", "no", "mixed", "yes"],
                            factory: () =>
                                new()
                                {
                                    TrueValue = "yes",
                                    FalseValue = "no",
                                    NullValue = "null",
                                    DefaultResult = "mixed",
                                }
                        ),
                    "AllBoolToValueMultiConverter.xaml",
                    "fallback",
                    (expected, actual) =>
                    {
                        var e = (AllBoolToValueMultiConverter<string>)expected;
                        var a = (AllBoolToValueMultiConverter<string>)actual;
                        WpfAssert.AssertResult(e.TrueValue, a.TrueValue);
                        WpfAssert.AssertResult(e.FalseValue, a.FalseValue);
                        WpfAssert.AssertResult(e.NullValue, a.NullValue);
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
    [ConverterTest(typeof(AllBoolToValueMultiConverter<>), ConverterTestLayer.Properties)]
    public void StringProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(StringCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(AllBoolToValueMultiConverter<>), ConverterTestLayer.Binding)]
    public void StringBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(StringCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(AllBoolToValueMultiConverter<>), ConverterTestLayer.Xaml)]
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
                    "AllBoolToValueMultiConverter",
                    typeof(AllBoolToValueMultiConverter),
                    () =>
                        M<AllBoolToValueMultiConverter>(
                            [
                                [true, true],
                                [false, true],
                                [false, false],
                                [true, false],
                                [true, true],
                            ],
                            ["yes", "mixed", "no", "mixed", "yes"],
                            factory: () =>
                                new()
                                {
                                    TrueValue = "yes",
                                    FalseValue = "no",
                                    NullValue = "null",
                                    DefaultResult = "mixed",
                                }
                        ),
                    "AllBoolToValueMultiConverter.xaml",
                    "fallback",
                    (expected, actual) =>
                    {
                        var e = (AllBoolToValueMultiConverter)expected;
                        var a = (AllBoolToValueMultiConverter)actual;
                        WpfAssert.AssertResult(e.TrueValue, a.TrueValue);
                        WpfAssert.AssertResult(e.FalseValue, a.FalseValue);
                        WpfAssert.AssertResult(e.NullValue, a.NullValue);
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
    [ConverterTest(typeof(AllBoolToValueMultiConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(AllBoolToValueMultiConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(AllBoolToValueMultiConverter), ConverterTestLayer.Xaml)]
    public void ObjectXaml(WpfPlatformCase scenario) => WpfXamlRunner.Verify(scenario);

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void ObjectInputBoundaries(WpfPlatformCase scenario) =>
        WpfBoundaryAssertions.Verify(scenario);

    public static IEnumerable<object[]> BehaviorCases => AllBoolToValueMultiConverterSuite.Cases;

    [TestMethod]
    [DynamicData(
        nameof(BehaviorCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(AllBoolToValueMultiConverter<>), ConverterTestLayer.Behavior)]
    public void StringBehavior(
        MultiConversionCase<AllBoolToValueMultiConverterSuite.Configuration> scenario
    ) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new AllBoolToValueMultiConverter<string>();
                return WpfConversionAdapter.Multi(
                    configuration,
                    converter,
                    typeof(AllBoolToValueMultiConverter<string>),
                    settings =>
                    {
                        if (settings.PreferredCulture is { } culture)
                            converter.PreferredCulture = culture;
                        if (settings.GetTrueValue is not null)
                            converter.TrueValue = settings.GetTrueValue();
                        if (settings.GetFalseValue is not null)
                            converter.FalseValue = settings.GetFalseValue();
                        if (settings.GetNullValue is not null)
                            converter.NullValue = settings.GetNullValue();
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
    [ConverterTest(typeof(AllBoolToValueMultiConverter), ConverterTestLayer.Behavior)]
    public void ObjectBehavior(
        MultiConversionCase<AllBoolToValueMultiConverterSuite.Configuration> scenario
    ) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new AllBoolToValueMultiConverter();
                return WpfConversionAdapter.Multi(
                    configuration,
                    converter,
                    typeof(AllBoolToValueMultiConverter),
                    settings =>
                    {
                        if (settings.PreferredCulture is { } culture)
                            converter.PreferredCulture = culture;
                        if (settings.GetTrueValue is not null)
                            converter.TrueValue = settings.GetTrueValue();
                        if (settings.GetFalseValue is not null)
                            converter.FalseValue = settings.GetFalseValue();
                        if (settings.GetNullValue is not null)
                            converter.NullValue = settings.GetNullValue();
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
    public void MultiBinding_FallbackDoNothingAndNullAreDistinct() =>
        PlatformThread.Run(() =>
        {
            var source = new TestViewModel { A = true, B = true };
            var target = new TestTarget();
            var converter = new AllBoolToValueMultiConverter
            {
                TrueValue = "value",
                FalseValue = Binding.DoNothing,
                NullValue = null,
                DefaultResult = Binding.DoNothing,
            };
            var binding = new MultiBinding
            {
                Converter = converter,
                FallbackValue = "fallback",
                TargetNullValue = "null-target",
                Mode = BindingMode.OneWay,
            };
            binding.Bindings.Add(new Binding("A") { Source = source });
            binding.Bindings.Add(new Binding("B") { Source = source });
            try
            {
                BindingOperations.SetBinding(target, TestTarget.ResultProperty, binding);
                PlatformThread.Drain();
                Assert.AreEqual("value", target.Result);
                source.Apply([false, false]);
                PlatformThread.Drain();
                Assert.AreEqual("value", target.Result);
                source.Apply([null, null]);
                PlatformThread.Drain();
                Assert.AreEqual("null-target", target.Result);
                converter.DefaultResult = DependencyProperty.UnsetValue;
                source.Apply([true, false]);
                PlatformThread.Drain();
                Assert.AreEqual("fallback", target.Result);
            }
            finally
            {
                BindingOperations.ClearAllBindings(target);
            }
        });

    [TestMethod]
    public void Sentinels_PreserveInputArray() =>
        PlatformThread.Run(() =>
        {
            foreach (var sentinel in new[] { DependencyProperty.UnsetValue, Binding.DoNothing })
            {
                var values = new object?[] { sentinel, true };
                var snapshot = values.ToArray();
                var multi = new AllBoolToValueMultiConverter { DefaultResult = "unavailable" };
                Assert.AreEqual("unavailable", multi.Convert(values, null, null, null));
                CollectionAssert.AreEqual(snapshot, values);
            }
        });

    [TestMethod]
    public void MultiValueBoundaries_EmptyShortAndSelectedBranchInputs_2() =>
        PlatformThread.Run(() =>
        {
            Assert.AreEqual(
                "yes",
                new AllBoolToValueMultiConverter<string> { TrueValue = "yes" }.Convert(
                    [],
                    null,
                    null,
                    null
                )
            );
        });

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() => new AllBoolToValueMultiConverter());
        WpfPropertyAssertions.Probe(
            () => new AllBoolToValueMultiConverter<string>(),
            AllBoolToValueMultiConverter<string>.TrueValueProperty,
            (c, value) => c.TrueValue = value,
            c => c.TrueValue,
            "one",
            "two",
            c => c.Convert([true, true], null, null, null),
            "one",
            "two"
        );
        WpfPropertyAssertions.Probe(
            () => new AllBoolToValueMultiConverter<string>(),
            AllBoolToValueMultiConverter<string>.FalseValueProperty,
            (c, value) => c.FalseValue = value,
            c => c.FalseValue,
            "one",
            "two",
            c => c.Convert([false, false], null, null, null),
            "one",
            "two"
        );
        WpfPropertyAssertions.Probe(
            () => new AllBoolToValueMultiConverter<string>(),
            AllBoolToValueMultiConverter<string>.NullValueProperty,
            (c, value) => c.NullValue = value,
            c => c.NullValue,
            "one",
            "two",
            c => c.Convert([null, null], null, null, null),
            "one",
            "two"
        );
        WpfPropertyAssertions.Probe(
            () => new AllBoolToValueMultiConverter(),
            AllBoolToValueMultiConverter.TrueValueProperty,
            (c, value) => c.TrueValue = value,
            c => c.TrueValue,
            "one",
            "two",
            c => c.Convert([true, true], null, null, null),
            "one",
            "two"
        );
        WpfPropertyAssertions.Probe(
            () => new AllBoolToValueMultiConverter(),
            AllBoolToValueMultiConverter.FalseValueProperty,
            (c, value) => c.FalseValue = value,
            c => c.FalseValue,
            "one",
            "two",
            c => c.Convert([false, false], null, null, null),
            "one",
            "two"
        );
        WpfPropertyAssertions.Probe(
            () => new AllBoolToValueMultiConverter(),
            AllBoolToValueMultiConverter.NullValueProperty,
            (c, value) => c.NullValue = value,
            c => c.NullValue,
            "one",
            "two",
            c => c.Convert([null, null], null, null, null),
            "one",
            "two"
        );
        WpfPropertyAssertions.Probe(
            () => new AllBoolToValueMultiConverter<int>(),
            AllBoolToValueMultiConverter<int>.TrueValueProperty,
            (c, value) => c.TrueValue = value,
            c => c.TrueValue,
            7,
            9,
            c => c.Convert([true, true], null, null, null),
            7,
            9
        );
        WpfPropertyAssertions.Probe(
            () => new AllBoolToValueMultiConverter<int>(),
            AllBoolToValueMultiConverter<int>.FalseValueProperty,
            (c, value) => c.FalseValue = value,
            c => c.FalseValue,
            7,
            9,
            c => c.Convert([false, false], null, null, null),
            7,
            9
        );
        WpfPropertyAssertions.Probe(
            () => new AllBoolToValueMultiConverter<int>(),
            AllBoolToValueMultiConverter<int>.NullValueProperty,
            (c, value) => c.NullValue = value,
            c => c.NullValue,
            7,
            9,
            c => c.Convert([null, null], null, null, null),
            7,
            9
        );
    }
}
