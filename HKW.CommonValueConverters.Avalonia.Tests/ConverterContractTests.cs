using System.Collections;
using System.Globalization;
using System.Reflection;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using HKW.HKWUtils;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class ConverterContractTests
{
    private static readonly HashSet<Type> Reversible =
    [
        typeof(DateTimeToStringConverter),
        typeof(DateTimeOffsetToStringConverter),
        typeof(DebugConverter),
        typeof(EnumToEnumInfoConverter),
        typeof(GuidToStringConverter),
        typeof(TimeSpanToStringConverter),
        typeof(StringToNumberConverter),
        typeof(MediaColorToSolidColorBrushConverter),
        typeof(SolidColorBrushToMediaColorConverter),
    ];

    [TestMethod]
    public Task EverySingleConverter_DeclaresSupportedOrUnsupportedReverseContract() =>
        PlatformThread.Run(() =>
        {
            foreach (var c in ConverterTestCases.All().Where(c => c.Multi is false))
            {
                var converter = (IValueConverter)c.Create();
                if (Reversible.Contains(c.Type) is false)
                {
                    Assert.ThrowsExactly<NotSupportedException>(
                        () =>
                            converter.ConvertBack(
                                c.Expected,
                                typeof(object),
                                c.Parameter,
                                CultureInfo.InvariantCulture
                            ),
                        c.Id
                    );
                    continue;
                }
                var converted = converter.Convert(
                    c.Initial[0],
                    typeof(object),
                    c.Parameter,
                    CultureInfo.InvariantCulture
                );
                var original = c.Initial[0];
                if (original is DateTime date)
                    original = date.ToUniversalTime();
                if (original is DateTimeOffset offset)
                    original = offset.ToUniversalTime();
                if (original is ISolidColorBrush brush)
                    original = brush.Color;
                ConverterAssertions.Equal(
                    original,
                    converter.ConvertBack(
                        converted,
                        c.Initial[0]!.GetType(),
                        c.Parameter,
                        CultureInfo.InvariantCulture
                    )
                );
            }
        });

    [TestMethod]
    public Task Defaults_AreIsolatedAndClearRestoresOriginalResult() =>
        PlatformThread.Run(() =>
        {
            foreach (var c in ConverterTestCases.All())
            {
                var first = c.Create();
                var second = c.Create();
                var original = first.DefaultResult;
                var marker = new object();
                first.DefaultResult = marker;
                Assert.AreSame(marker, first.DefaultResult, c.Id);
                Assert.AreNotSame(marker, second.DefaultResult, c.Id);
                first.DefaultResult = null;
                Assert.IsNull(first.DefaultResult, c.Id);
                first.ClearValue(ConverterBase.DefaultResultProperty);
                // 用例工厂可能设置本地回退值, 清除后应恢复全新实例的核心默认值.
                var pristine = (ConverterBase)Activator.CreateInstance(c.Type)!;
                Assert.AreEqual(pristine.DefaultResult, first.DefaultResult, c.Id);
                Assert.AreEqual(original, second.DefaultResult, c.Id);
            }
            Assert.AreSame(
                AvaloniaProperty.UnsetValue,
                new StringToNumberConverter().Convert("invalid", typeof(int), null, null)
            );
            Assert.AreEqual(
                new Thickness(),
                new MarginMultiConverter().Convert(["invalid"], typeof(Thickness), null, null)
            );
        });

    [TestMethod]
    public Task StyledProperties_RegisterTypesAndSynchronizeNativeWrites() =>
        PlatformThread.Run(() =>
        {
            foreach (var c in ConverterTestCases.All())
            {
                foreach (
                    var field in c
                        .Type.GetFields(
                            BindingFlags.Public
                                | BindingFlags.Static
                                | BindingFlags.FlattenHierarchy
                        )
                        .Where(f =>
                            f.FieldType.IsGenericType
                            && f.FieldType.GetGenericTypeDefinition()
                                == typeof(Core.CommonDependencyProperty<>)
                        )
                )
                {
                    var handle = field.GetValue(null)!;
                    var native = (AvaloniaProperty)
                        handle.GetType().GetProperty("Value")!.GetValue(handle)!;
                    Assert.AreEqual(
                        field.FieldType.GenericTypeArguments[0],
                        native.PropertyType,
                        c.Id + ": " + field.Name
                    );
                    Assert.AreEqual(field.Name[..^8], native.Name);
                }
            }
            var converter = new BoolToValueConverter<int> { TrueValue = 1, FalseValue = 2 };
            converter.SetValue(BoolToValueConverter<int>.TrueValueProperty, 7);
            Assert.AreEqual(7, converter.Convert(true, null, null, null));
            converter.ClearValue(BoolToValueConverter<int>.TrueValueProperty);
            Assert.AreEqual(0, converter.Convert(true, null, null, null));
            converter.IsInverted = true;
            Assert.AreEqual(2, converter.Convert(true, null, null, null));
            Assert.ThrowsExactly<ArgumentNullException>(() => converter.GetValue<int>(null!));
            Assert.ThrowsExactly<ArgumentException>(() =>
                converter.GetValue(new Core.CommonDependencyProperty<int>(new object()))
            );
        });

    [TestMethod]
    public Task MappedClrSentinels_DifferFromNativePropertyControlValues() =>
        PlatformThread.Run(() =>
        {
            var converter = new BoolToValueConverter { TrueValue = "original" };
            converter.TrueValue = AvaloniaProperty.UnsetValue;
            Assert.AreSame(AvaloniaProperty.UnsetValue, converter.Convert(true, null, null, null));
            Assert.AreSame(
                Core.ConverterResults.UnsetValue,
                converter.GetValue(BoolToValueConverter<object?>.TrueValueProperty)
            );
            converter.TrueValue = BindingOperations.DoNothing;
            Assert.AreSame(BindingOperations.DoNothing, converter.Convert(true, null, null, null));
            Assert.AreSame(
                Core.ConverterResults.DoNothing,
                converter.GetValue(BoolToValueConverter<object?>.TrueValueProperty)
            );
            converter.SetValue(
                BoolToValueConverter<object?>.TrueValueProperty,
                AvaloniaProperty.UnsetValue
            );
            Assert.IsNull(converter.TrueValue);
            converter.TrueValue = "keep";
            converter.SetValue(
                BoolToValueConverter<object?>.TrueValueProperty,
                BindingOperations.DoNothing
            );
            Assert.AreEqual("keep", converter.TrueValue);
            converter.TrueValue = null;
            Assert.IsNull(converter.Convert(true, null, null, null));
        });

    [TestMethod]
    public Task SingleAndMultiBoundaries_RejectSentinelsWithoutMutatingInputs() =>
        PlatformThread.Run(() =>
        {
            var marker = new object();
            var numeric = new StringToNumberConverter { DefaultResult = marker };
            foreach (
                var invalid in new[]
                {
                    AvaloniaProperty.UnsetValue,
                    BindingOperations.DoNothing,
                    Core.ConverterResults.UnsetValue,
                    Core.ConverterResults.DoNothing,
                    "invalid",
                    null,
                }
            )
                Assert.AreSame(marker, numeric.Convert(invalid, null, null, null));
            var dictionary = new Hashtable
            {
                ["nil"] = null,
                ["array"] = new[] { 1, 2 },
                ["unset"] = AvaloniaProperty.UnsetValue,
            };
            var single = new GetDictionaryValueConverter { DefaultResult = marker };
            Assert.IsNull(single.Convert(dictionary, null, "nil", null));
            Assert.AreSame(dictionary["array"], single.Convert(dictionary, null, "array", null));
            var multi = new GetDictionaryValueMultiConverter { DefaultResult = marker };
            foreach (
                var invalid in new object?[][]
                {
                    [],
                    [dictionary],
                    [dictionary, "missing"],
                    [dictionary, null],
                    [AvaloniaProperty.UnsetValue, "nil"],
                }
            )
            {
                var snapshot = invalid.ToArray();
                Assert.AreSame(marker, multi.Convert(invalid, null, null, null));
                CollectionAssert.AreEqual(snapshot, invalid);
            }
            Assert.IsNull(multi.Convert([dictionary, "nil"], null, null, null));
            Assert.AreSame(
                dictionary["array"],
                multi.Convert([dictionary, "array"], null, null, null)
            );
            var format = new StringFormatMultiConverter { ReplaceUnsetValue = "missing" };
            object?[] values = [AvaloniaProperty.UnsetValue, "tail"];
            Assert.AreEqual(
                "missing:tail",
                format.Convert(values, null, "{0}:{1}", CultureInfo.InvariantCulture)
            );
            Assert.AreSame(AvaloniaProperty.UnsetValue, values[0]);
            Assert.AreEqual("tail", values[1]);
        });

    [TestMethod]
    public Task NullableMappings_AndEqualityConfigurationAreHonored() =>
        PlatformThread.Run(() =>
        {
            var boolean = new BoolToValueConverter<string>
            {
                TrueValue = "yes",
                FalseValue = "no",
                NullValue = "nil",
            };
            Assert.AreEqual("no", boolean.Convert(null, null, null, null));
            boolean.IsNullable = true;
            Assert.AreEqual("nil", boolean.Convert(null, null, null, null));
            boolean.IsInverted = true;
            Assert.AreEqual("no", boolean.Convert(true, null, null, null));
            var equality = new EqualsToValueConverter<string>
            {
                Other = 12,
                TrueValue = "yes",
                FalseValue = "no",
                NullValue = "nil",
            };
            Assert.AreEqual("no", equality.Convert("12", null, null, null));
            equality.IsStringEquals = true;
            Assert.AreEqual("yes", equality.Convert("12", null, null, null));
            Assert.AreEqual("nil", equality.Convert(null, null, null, null));
            var all = new AllBoolToValueMultiConverter<string>
            {
                TrueValue = "yes",
                FalseValue = "no",
                NullValue = "nil",
                DefaultResult = "mixed",
            };
            Assert.AreEqual("nil", all.Convert([null, null], null, null, null));
            Assert.AreEqual("mixed", all.Convert([true, false], null, null, null));
            Assert.AreEqual("mixed", all.Convert([AvaloniaProperty.UnsetValue], null, null, null));
        });

    [TestMethod]
    public Task ValueToEnumerableConverter_KeepsArrayAsOneElementAndPreservesNull() =>
        PlatformThread.Run(() =>
        {
            var value = new[] { 1, 2 };
            var converter = new ValueToEnumerableConverter();
            var result = (object[])converter.Convert(value, typeof(IEnumerable), null, null)!;
            Assert.HasCount(1, result);
            Assert.AreSame(value, result[0]);
            Assert.IsNull(converter.Convert(null, null, null, null));
        });

    [TestMethod]
    public Task MarginMultiConverter_MissingSidesDefaultToZeroAndEveryInputIsValidated() =>
        PlatformThread.Run(() =>
        {
            var marker = new object();
            var converter = new MarginMultiConverter { DefaultResult = marker };
            Assert.AreEqual(new Thickness(), converter.Convert([], null, null, null));
            Assert.AreEqual(new Thickness(2), converter.Convert([2], null, null, null));
            Assert.AreEqual(new Thickness(1, 2, 0, 0), converter.Convert([1, 2], null, null, null));
            Assert.AreEqual(
                new Thickness(1, 2, 3, 0),
                converter.Convert([1, 2, 3], null, null, null)
            );
            Assert.AreEqual(
                new Thickness(1, 0, 3, 0),
                converter.Convert([1, null, 3], null, null, null)
            );
            Assert.AreSame(marker, converter.Convert([1, double.NaN], null, null, null));
            Assert.AreSame(
                marker,
                converter.Convert([1, double.PositiveInfinity], null, null, null)
            );
            Assert.AreSame(marker, converter.Convert([1, 2, 3, 4, 5], null, null, null));
        });

    [TestMethod]
    public Task PlatformConverters_RejectOffThreadConversion() =>
        PlatformThread.Run(() =>
        {
            var converter = new BoolInverterConverter();
            var dispatcher = converter.Dispatcher;
            var uiThreadId = Environment.CurrentManagedThreadId;
            var contextMarker = new AsyncLocal<object?> { Value = new object() };
            Assert.AreSame(global::Avalonia.Threading.Dispatcher.UIThread, dispatcher);
            Assert.IsTrue(dispatcher.CheckAccess());
            Assert.IsTrue(converter.CheckAccess());
            System.Runtime.ExceptionServices.ExceptionDispatchInfo? failure = null;
            var worker = new Thread(() =>
            {
                try
                {
                    Assert.AreNotEqual(uiThreadId, Environment.CurrentManagedThreadId);
                    Assert.IsFalse(Thread.CurrentThread.IsThreadPoolThread);
                    Assert.IsNull(contextMarker.Value);
                    Assert.AreSame(dispatcher, converter.Dispatcher);
                    Assert.IsFalse(dispatcher.CheckAccess());
                    Assert.IsFalse(converter.CheckAccess());
                    Assert.ThrowsExactly<InvalidOperationException>(() =>
                        converter.Convert(true, null, null, null)
                    );
                }
                catch (Exception exception)
                {
                    failure = System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(
                        exception
                    );
                }
            })
            {
                IsBackground = true,
                Name = "Avalonia converter access probe",
            };
            // 同步等待 Task 可能内联执行任务, 独立线程并禁止上下文流动才能保证跨线程验证.
            using (ExecutionContext.SuppressFlow())
                worker.Start();
            Assert.IsTrue(
                worker.Join(TimeSpan.FromSeconds(10)),
                "The off-thread conversion probe did not finish."
            );
            failure?.Throw();
            Assert.IsTrue(dispatcher.CheckAccess());
            Assert.AreEqual(false, converter.Convert(true, null, null, null));
        });

    [TestMethod]
    public Task LegacyBoolToStyleConverter_PreservesFullyQualifiedCoreDelegateContract() =>
        PlatformThread.Run(() =>
        {
            var trueStyle = new global::Avalonia.Styling.Style();
            var falseStyle = new global::Avalonia.Styling.Style();
            var nullStyle = new global::Avalonia.Styling.Style();
            var legacy = new global::HKW.CommonValueConverters.WPF.BoolToStyleConverter
            {
                GetTrueValue = () => trueStyle,
                GetFalseValue = () => falseStyle,
                GetNullValue = () => nullStyle,
                GetIsNullable = () => true,
            };
            Assert.AreEqual(
                "HKW.CommonValueConverters.WPF.BoolToStyleConverter",
                legacy.GetType().FullName
            );
            Assert.AreSame(typeof(ConverterBase).Assembly, legacy.GetType().Assembly);
            Assert.IsInstanceOfType<Core.BoolToValueConverter<global::Avalonia.Styling.Style>>(
                legacy
            );
            Assert.IsFalse(typeof(ConverterBase).IsAssignableFrom(legacy.GetType()));
            Assert.AreSame(trueStyle, legacy.GetTrueValue());
            Assert.AreSame(
                trueStyle,
                legacy.Convert(
                    true,
                    typeof(global::Avalonia.Styling.Style),
                    null,
                    CultureInfo.InvariantCulture
                )
            );
            Assert.AreSame(falseStyle, legacy.Convert(false, null, null, null));
            Assert.AreSame(nullStyle, legacy.Convert(null, null, null, null));
            legacy.GetTrueValue = () => nullStyle;
            Assert.AreSame(nullStyle, legacy.Convert(true, null, null, null));
            legacy.GetIsInverted = () => true;
            Assert.AreSame(falseStyle, legacy.Convert(true, null, null, null));
            Assert.ThrowsExactly<NotSupportedException>(() =>
                legacy.ConvertBack(trueStyle, null, null, null)
            );
        });

    [TestMethod]
    public Task SplitDelegateConfiguration_RemainsLiveAfterConstruction() =>
        PlatformThread.Run(() =>
        {
            var converter = new BoolToSplitThicknessConverter { Separator = "|" };
            Assert.AreEqual(
                new Thickness(2),
                converter.Convert(true, null, "2|3", CultureInfo.InvariantCulture)
            );
            converter.ConvertSplitValue = (text, c) => "custom:" + text;
            Assert.AreEqual(
                "custom:2",
                converter.Convert(true, null, "2|3", CultureInfo.InvariantCulture)
            );
        });

    [TestMethod]
    public Task BrushConversions_RejectWrongTypesAndDoNotAliasMutableBrushes() =>
        PlatformThread.Run(() =>
        {
            var converter = new MediaColorToSolidColorBrushConverter();
            var first = (SolidColorBrush)converter.Convert(Colors.Red, null, null, null)!;
            var second = (SolidColorBrush)converter.Convert(Colors.Red, null, null, null)!;
            Assert.AreNotSame(first, second);
            first.Color = Colors.Blue;
            Assert.AreEqual(Colors.Red, second.Color);
            Assert.AreSame(AvaloniaProperty.UnsetValue, converter.Convert(null, null, null, null));
            Assert.AreSame(
                AvaloniaProperty.UnsetValue,
                converter.ConvertBack(Brushes.Red, null, null, null)
            );
            Assert.AreSame(
                AvaloniaProperty.UnsetValue,
                new SolidColorBrushToMediaColorConverter().Convert(Brushes.Red, null, null, null)
            );
        });
}
