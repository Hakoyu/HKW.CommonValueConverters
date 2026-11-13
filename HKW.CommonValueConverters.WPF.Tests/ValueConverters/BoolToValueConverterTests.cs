using System.Windows;
using System.Windows.Data;
using HKW.CommonValueConvertersTest;
using HKW.CommonValueConvertersTest.Testing;
using static HKW.CommonValueConverters.WPF.Tests.WpfScenario;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class BoolToValueConverterTests
{
    public static IEnumerable<object[]> StringCases =>
        [
            [
                new WpfPlatformCase(
                    "BoolToValueConverter_String",
                    typeof(BoolToValueConverter<string>),
                    () =>
                        S<BoolToValueConverter<string>>(true, false, "yes", "no", factory: () =>
                            new()
                            {
                                TrueValue = "yes",
                                FalseValue = "no",
                                NullValue = "null",
                                IsNullable = true,
                            }
                        ),
                    "BoolToValueConverter.xaml",
                    "null",
                    (expected, actual) =>
                    {
                        var e = (BoolToValueConverter<string>)expected;
                        var a = (BoolToValueConverter<string>)actual;
                        WpfAssert.AssertResult(e.TrueValue, a.TrueValue);
                        WpfAssert.AssertResult(e.FalseValue, a.FalseValue);
                        WpfAssert.AssertResult(e.IsNullable, a.IsNullable);
                        WpfAssert.AssertResult(e.NullValue, a.NullValue);
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
    [ConverterTest(typeof(BoolToValueConverter<>), ConverterTestLayer.Properties)]
    public void StringProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(StringCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(BoolToValueConverter<>), ConverterTestLayer.Binding)]
    public void StringBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(StringCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(BoolToValueConverter<>), ConverterTestLayer.Xaml)]
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
                    "BoolToValueConverter",
                    typeof(BoolToValueConverter),
                    () => S<BoolToValueConverter>(true, false, "yes", "no", factory: () => new()
                            {
                                TrueValue = "yes",
                                FalseValue = "no",
                                NullValue = "null",
                                IsNullable = true,
                            }),
                    "BoolToValueConverter.xaml",
                    "null",
                    (expected, actual) =>
                    {
                        var e = (BoolToValueConverter)expected;
                        var a = (BoolToValueConverter)actual;
                        WpfAssert.AssertResult(e.TrueValue, a.TrueValue);
                        WpfAssert.AssertResult(e.FalseValue, a.FalseValue);
                        WpfAssert.AssertResult(e.IsNullable, a.IsNullable);
                        WpfAssert.AssertResult(e.NullValue, a.NullValue);
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
    [ConverterTest(typeof(BoolToValueConverter), ConverterTestLayer.Properties)]
    public void ObjectProperties(WpfPlatformCase scenario) =>
        WpfPropertyAssertions.Verify(scenario);

    [TestMethod]
    [TestCategory("Binding")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(BoolToValueConverter), ConverterTestLayer.Binding)]
    public void ObjectBinding(WpfPlatformCase scenario) => WpfBindingRunner.Verify(scenario);

    [TestMethod]
    [TestCategory("Xaml")]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(BoolToValueConverter), ConverterTestLayer.Xaml)]
    public void ObjectXaml(WpfPlatformCase scenario) => WpfXamlRunner.Verify(scenario);

    [TestMethod]
    [DynamicData(
        nameof(ObjectCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    public void ObjectInputBoundaries(WpfPlatformCase scenario) =>
        WpfBoundaryAssertions.Verify(scenario);

    public static IEnumerable<object[]> BehaviorCases => BoolToValueConverterSuite.Cases;

    [TestMethod]
    [DynamicData(
        nameof(BehaviorCases),
        DynamicDataDisplayName = nameof(WpfTestDisplay.Name),
        DynamicDataDisplayNameDeclaringType = typeof(WpfTestDisplay)
    )]
    [ConverterTest(typeof(BoolToValueConverter<>), ConverterTestLayer.Behavior)]
    public void StringBehavior(
        SingleConversionCase<BoolToValueConverterSuite.Configuration> scenario
    ) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new BoolToValueConverter<string>();
                return WpfConversionAdapter.Single(
                    configuration,
                    converter,
                    typeof(BoolToValueConverter<string>),
                    settings =>
                    {
                        if (settings.PreferredCulture is { } culture)
                            converter.PreferredCulture = culture;
                        if (settings.GetTrueValue is not null)
                            converter.TrueValue = settings.GetTrueValue();
                        if (settings.GetFalseValue is not null)
                            converter.FalseValue = settings.GetFalseValue();
                        if (settings.GetIsNullable is not null)
                            converter.IsNullable = settings.GetIsNullable();
                        if (settings.GetNullValue is not null)
                            converter.NullValue = settings.GetNullValue();
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
    [ConverterTest(typeof(BoolToValueConverter), ConverterTestLayer.Behavior)]
    public void ObjectBehavior(
        SingleConversionCase<BoolToValueConverterSuite.Configuration> scenario
    ) =>
        PlatformThread.Run(() =>
            scenario.Run(configuration =>
            {
                var converter = new BoolToValueConverter();
                return WpfConversionAdapter.Single(
                    configuration,
                    converter,
                    typeof(BoolToValueConverter),
                    settings =>
                    {
                        if (settings.PreferredCulture is { } culture)
                            converter.PreferredCulture = culture;
                        if (settings.GetTrueValue is not null)
                            converter.TrueValue = settings.GetTrueValue();
                        if (settings.GetFalseValue is not null)
                            converter.FalseValue = settings.GetFalseValue();
                        if (settings.GetIsNullable is not null)
                            converter.IsNullable = settings.GetIsNullable();
                        if (settings.GetNullValue is not null)
                            converter.NullValue = settings.GetNullValue();
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
    public void NativeSetClearAndCurrentValue_UpdateCoreConfiguration() =>
        PlatformThread.Run(() =>
        {
            var converter = new BoolToValueConverter<int>();
            var property = (DependencyProperty)BoolToValueConverter<int>.TrueValueProperty.Value;
            Assert.AreEqual(typeof(int), property.PropertyType);
            Assert.AreEqual(0, property.DefaultMetadata.DefaultValue);
            converter.SetValue(property, 7);
            Assert.AreEqual(7, converter.TrueValue);
            Assert.AreEqual(7, converter.Convert(true, null, null, null));
            converter.SetCurrentValue(BoolToValueConverter<int>.TrueValueProperty, 9);
            Assert.AreEqual(9, converter.Convert(true, null, null, null));
            converter.ClearValue(property);
            Assert.AreEqual(0, converter.Convert(true, null, null, null));
            converter.TrueValue = 2;
            Assert.AreEqual(2, converter.GetValue(property));
            Assert.ThrowsExactly<ArgumentException>(() => converter.SetValue(property, "invalid"));
        });

    [TestMethod]
    public void MappedClrSetterAndNativeUnsetHaveDifferentSemantics() =>
        PlatformThread.Run(() =>
        {
            var converter = new BoolToValueConverter();
            var property = (DependencyProperty)BoolToValueConverter<object>.TrueValueProperty.Value;
            converter.TrueValue = DependencyProperty.UnsetValue;
            Assert.AreSame(Core.ConverterResults.UnsetValue, converter.ReadLocalValue(property));
            Assert.AreSame(DependencyProperty.UnsetValue, converter.TrueValue);
            converter.SetValue(property, DependencyProperty.UnsetValue);
            Assert.AreSame(DependencyProperty.UnsetValue, converter.ReadLocalValue(property));
            Assert.IsNull(converter.TrueValue);
            converter.TrueValue = Binding.DoNothing;
            Assert.AreSame(Core.ConverterResults.DoNothing, converter.ReadLocalValue(property));
            converter.ClearValue(property);
            Assert.IsNull(converter.TrueValue);
        });

    [TestMethod]
    public void Binding_FallbackDoNothingAndNullAreDistinct() =>
        PlatformThread.Run(() =>
        {
            var source = new TestViewModel { A = true };
            var target = new TestTarget();
            var converter = new BoolToValueConverter
            {
                TrueValue = "value",
                FalseValue = Binding.DoNothing,
                NullValue = null,
                IsNullable = true,
            };
            try
            {
                BindingOperations.SetBinding(
                    target,
                    TestTarget.ResultProperty,
                    new Binding("A")
                    {
                        Source = source,
                        Converter = converter,
                        FallbackValue = "fallback",
                        TargetNullValue = "null-target",
                        Mode = BindingMode.OneWay,
                    }
                );
                PlatformThread.Drain();
                Assert.AreEqual("value", target.Result);
                source.A = false;
                PlatformThread.Drain();
                Assert.AreEqual("value", target.Result);
                source.A = null;
                PlatformThread.Drain();
                Assert.AreEqual("null-target", target.Result);
                converter.TrueValue = DependencyProperty.UnsetValue;
                source.A = true;
                PlatformThread.Drain();
                Assert.AreEqual("fallback", target.Result);
                converter.TrueValue = "restored";
                source.A = true;
                PlatformThread.Drain();
                Assert.AreEqual("restored", target.Result);
            }
            finally
            {
                BindingOperations.ClearAllBindings(target);
            }
        });

    [TestMethod]
    public void NullableAndInversionConfiguration_RemainsLive_BoolToValueConverter() =>
        PlatformThread.Run(() =>
        {
            var converter = new BoolToValueConverter<int>
            {
                TrueValue = 7,
                FalseValue = 3,
                NullValue = 5,
            };
            Assert.AreEqual(3, converter.Convert(null, null, null, null));
            converter.IsNullable = true;
            Assert.AreEqual(5, converter.Convert(null, null, null, null));
            converter.IsInverted = true;
            Assert.AreEqual(3, converter.Convert(true, null, null, null));
        });

    [TestMethod]
    public void SentinelMappings_PreserveNullAndObjectIdentity() =>
        PlatformThread.Run(() =>
        {
            var converter = new BoolToValueConverter
            {
                TrueValue = DependencyProperty.UnsetValue,
                FalseValue = Binding.DoNothing,
                NullValue = null,
                IsNullable = true,
            };
            Assert.AreSame(
                DependencyProperty.UnsetValue,
                converter.Convert(true, null, null, null)
            );
            Assert.AreSame(Binding.DoNothing, converter.Convert(false, null, null, null));
            Assert.IsNull(converter.Convert(null, null, null, null));
            Assert.AreSame(
                Core.ConverterResults.UnsetValue,
                converter.GetValue(BoolToValueConverter<object>.TrueValueProperty)
            );
            Assert.AreSame(
                Core.ConverterResults.DoNothing,
                converter.GetValue(BoolToValueConverter<object>.FalseValueProperty)
            );
            converter.DefaultResult = "unavailable";
            foreach (var sentinel in new[] { DependencyProperty.UnsetValue, Binding.DoNothing })
            {
                Assert.AreEqual("unavailable", converter.Convert(sentinel, null, null, null));
            }
            var objectValue = new object();
            converter.TrueValue = objectValue;
            Assert.AreSame(objectValue, converter.Convert(true, null, null, null));
        });

    private static void VerifyPropertyWiring()
    {
        WpfPropertyAssertions.VerifyInherited(() => new BoolToValueConverter());
        WpfPropertyAssertions.Probe(
            () => new BoolToValueConverter<string>(),
            BoolToValueConverter<string>.TrueValueProperty,
            (c, value) => c.TrueValue = value,
            c => c.TrueValue,
            "one",
            "two",
            c => c.Convert(true, null, null, null),
            "one",
            "two"
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToValueConverter<string>(),
            BoolToValueConverter<string>.FalseValueProperty,
            (c, value) => c.FalseValue = value,
            c => c.FalseValue,
            "one",
            "two",
            c => c.Convert(false, null, null, null),
            "one",
            "two"
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToValueConverter<string>() { IsNullable = true },
            BoolToValueConverter<string>.NullValueProperty,
            (c, value) => c.NullValue = value,
            c => c.NullValue,
            "one",
            "two",
            c => c.Convert(null, null, null, null),
            "one",
            "two"
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToValueConverter<string>() { NullValue = "one", FalseValue = "two" },
            BoolToValueConverter<string>.IsNullableProperty,
            (c, value) => c.IsNullable = value,
            c => c.IsNullable,
            true,
            false,
            c => c.Convert(null, null, null, null),
            "one",
            "two"
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToValueConverter<string>() { TrueValue = "one", FalseValue = "two" },
            BoolToValueConverter<string>.IsInvertedProperty,
            (c, value) => c.IsInverted = value,
            c => c.IsInverted,
            true,
            false,
            c => c.Convert(true, null, null, null),
            "two",
            "one"
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToValueConverter(),
            BoolToValueConverter.TrueValueProperty,
            (c, value) => c.TrueValue = value,
            c => c.TrueValue,
            "one",
            "two",
            c => c.Convert(true, null, null, null),
            "one",
            "two"
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToValueConverter(),
            BoolToValueConverter.FalseValueProperty,
            (c, value) => c.FalseValue = value,
            c => c.FalseValue,
            "one",
            "two",
            c => c.Convert(false, null, null, null),
            "one",
            "two"
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToValueConverter() { IsNullable = true },
            BoolToValueConverter.NullValueProperty,
            (c, value) => c.NullValue = value,
            c => c.NullValue,
            "one",
            "two",
            c => c.Convert(null, null, null, null),
            "one",
            "two"
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToValueConverter() { NullValue = "one", FalseValue = "two" },
            BoolToValueConverter.IsNullableProperty,
            (c, value) => c.IsNullable = value,
            c => c.IsNullable,
            true,
            false,
            c => c.Convert(null, null, null, null),
            "one",
            "two"
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToValueConverter() { TrueValue = "one", FalseValue = "two" },
            BoolToValueConverter.IsInvertedProperty,
            (c, value) => c.IsInverted = value,
            c => c.IsInverted,
            true,
            false,
            c => c.Convert(true, null, null, null),
            "two",
            "one"
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToValueConverter<int>(),
            BoolToValueConverter<int>.TrueValueProperty,
            (c, value) => c.TrueValue = value,
            c => c.TrueValue,
            7,
            9,
            c => c.Convert(true, null, null, null),
            7,
            9
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToValueConverter<int>(),
            BoolToValueConverter<int>.FalseValueProperty,
            (c, value) => c.FalseValue = value,
            c => c.FalseValue,
            7,
            9,
            c => c.Convert(false, null, null, null),
            7,
            9
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToValueConverter<int>() { IsNullable = true },
            BoolToValueConverter<int>.NullValueProperty,
            (c, value) => c.NullValue = value,
            c => c.NullValue,
            7,
            9,
            c => c.Convert(null, null, null, null),
            7,
            9
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToValueConverter<int>() { NullValue = 7, FalseValue = 9 },
            BoolToValueConverter<int>.IsNullableProperty,
            (c, value) => c.IsNullable = value,
            c => c.IsNullable,
            true,
            false,
            c => c.Convert(null, null, null, null),
            7,
            9
        );
        WpfPropertyAssertions.Probe(
            () => new BoolToValueConverter<int>() { TrueValue = 7, FalseValue = 9 },
            BoolToValueConverter<int>.IsInvertedProperty,
            (c, value) => c.IsInverted = value,
            c => c.IsInverted,
            true,
            false,
            c => c.Convert(true, null, null, null),
            9,
            7
        );
    }
}
