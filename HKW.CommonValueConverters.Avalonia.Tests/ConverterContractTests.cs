using System.Collections;
using System.Globalization;
using System.Reflection;
using HKW.CommonValueConverters.Avalonia;
using HKW.HKWUtils;
using Core = HKW.CommonValueConverters;
using MultiInterface = global::Avalonia.Data.Converters.IMultiValueConverter;
using SingleInterface = global::Avalonia.Data.Converters.IValueConverter;

namespace HKW.PlatformConverters.Avalonia.Tests;

[TestClass]
public sealed class ConverterContractTests
{
    [TestMethod]
    public void CompleteSurfaceAndEveryConfigurationDelegateArePresent() =>
        PlatformThread.Run(() =>
        {
            var coreTypes = typeof(Core.ValueConverterBase)
                .Assembly.GetExportedTypes()
                .Where(t =>
                    !t.IsAbstract
                    && (
                        typeof(Core.ValueConverterBase).IsAssignableFrom(t)
                        || typeof(Core.MultiValueConverterBase).IsAssignableFrom(t)
                    )
                )
                .ToArray();
            Assert.HasCount(38, coreTypes);
            var wrappers = typeof(ConverterBase)
                .Assembly.GetExportedTypes()
                .Where(t => !t.IsAbstract && typeof(ConverterBase).IsAssignableFrom(t))
                .ToArray();
            Assert.HasCount(44, wrappers);
            foreach (var coreType in coreTypes)
            {
                var wrapperType = wrappers.Single(t => t.Name == coreType.Name);
                var closedCoreType = coreType.IsGenericTypeDefinition
                    ? coreType.MakeGenericType(typeof(object))
                    : coreType;
                var closedWrapperType = wrapperType.IsGenericTypeDefinition
                    ? wrapperType.MakeGenericType(typeof(object))
                    : wrapperType;
                var wrapper = (ConverterBase)Activator.CreateInstance(closedWrapperType)!;
                foreach (
                    var field in closedWrapperType
                        .GetFields(
                            BindingFlags.Public
                                | BindingFlags.Static
                                | BindingFlags.FlattenHierarchy
                        )
                        .Where(f => f.Name.EndsWith("Property"))
                )
                {
                    var property = closedWrapperType.GetProperty(field.Name[..^"Property".Length]);
                    Assert.IsNotNull(
                        property,
                        $"{closedWrapperType.Name}.{field.Name} CLR property"
                    );
                    var handleType = typeof(Core.CommonDependencyProperty<>).MakeGenericType(
                        property.PropertyType
                    );
                    Assert.AreEqual(handleType, field.FieldType, $"{field.Name} wrapper type");
                    var handle = field.GetValue(null);
                    Assert.IsNotNull(handle);
                    var native = handleType.GetProperty("Value")!.GetValue(handle);
                    Assert.IsInstanceOfType(
                        native,
                        typeof(global::Avalonia.StyledProperty<>).MakeGenericType(
                            property.PropertyType
                        )
                    );
                    var nativeProperty = (global::Avalonia.AvaloniaProperty)native!;
                    Assert.AreEqual(property.Name, nativeProperty.Name);
                    Assert.AreEqual(property.PropertyType, nativeProperty.PropertyType);
                    Assert.AreEqual(field.DeclaringType, nativeProperty.OwnerType);
                    Assert.IsTrue(
                        global::Avalonia
                            .AvaloniaPropertyRegistry.Instance.GetRegistered(
                                nativeProperty.OwnerType
                            )
                            .Any(p => ReferenceEquals(p, native))
                    );
                }
                var core = (Core.CommonConverterBase)
                    typeof(ConverterBase)
                        .GetProperty(
                            "CoreConverter",
                            BindingFlags.Instance | BindingFlags.NonPublic
                        )!
                        .GetValue(wrapper)!;
                var pristine = Activator.CreateInstance(closedCoreType)!;
                foreach (
                    var getter in closedCoreType
                        .GetProperties()
                        .Where(p => p.Name.StartsWith("Get") && p.Name != "GetDefaultResult")
                )
                {
                    var name = getter.Name[3..];
                    var property = closedWrapperType.GetProperty(name)!;
                    Assert.IsNotNull(property, $"{closedWrapperType.Name}.{name}");
                    var field = closedWrapperType.GetField(
                        name + "Property",
                        BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy
                    );
                    Assert.IsNotNull(field, $"{name} platform property");
                    var originalDefault = ((Delegate)getter.GetValue(pristine)!).DynamicInvoke();
                    Assert.AreEqual(
                        originalDefault,
                        property.GetValue(wrapper),
                        $"{wrapperType.Name}.{name} default"
                    );
                    object? alternate =
                        property.PropertyType == typeof(bool) ? !Equals(originalDefault, true)
                        : property.PropertyType == typeof(bool?)
                            ? (Equals(originalDefault, true) ? null : true)
                        : property.PropertyType == typeof(string) ? "changed"
                        : property.PropertyType.IsEnum
                            ? Enum.GetValues(property.PropertyType)
                                .Cast<object>()
                                .First(v => !v.Equals(originalDefault))
                        : new object();
                    property.SetValue(wrapper, alternate);
                    Assert.AreEqual(
                        alternate,
                        ((Delegate)getter.GetValue(core)!).DynamicInvoke(),
                        $"{wrapperType.Name}.{name} wiring"
                    );
                    property.SetValue(wrapper, originalDefault);
                    Assert.AreEqual(
                        originalDefault,
                        ((Delegate)getter.GetValue(core)!).DynamicInvoke(),
                        $"{wrapperType.Name}.{name} refresh"
                    );
                }
            }
        });

    [TestMethod]
    public void NonGenericEntriesAcceptObjectValues() =>
        PlatformThread.Run(() =>
        {
            var marker = new object();
            Assert.AreSame(
                marker,
                new BoolToValueConverter { TrueValue = marker }.Convert(true, null, null, null)
            );
            Assert.AreSame(
                marker,
                new EqualsToValueConverter { Other = 1, TrueValue = marker }.Convert(
                    1,
                    null,
                    null,
                    null
                )
            );
            Assert.AreSame(
                marker,
                new AllBoolToValueMultiConverter { TrueValue = marker }.Convert(
                    [true],
                    null,
                    null,
                    null
                )
            );
            Assert.AreEqual(
                true,
                new AllEqualsMultiConverter { Value = marker }.Convert([marker], null, null, null)
            );
            Assert.AreEqual(
                true,
                new AnyEqualsMultiConverter { Value = marker }.Convert(
                    [null, marker],
                    null,
                    null,
                    null
                )
            );
            Assert.AreEqual(
                1,
                new EqualsCountMultiConverter { Value = marker }.Convert(
                    [null, marker],
                    null,
                    null,
                    null
                )
            );
        });

    [TestMethod]
    public void MissingSingleValueCasesHaveExplicitExpectedResults() =>
        PlatformThread.Run(() =>
        {
            var infoConverter = new EnumToEnumInfoConverter();
            var info = infoConverter.Convert(DayOfWeek.Monday, null, null, null);
            Assert.IsInstanceOfType<IEnumInfo>(info);
            Assert.AreEqual(DayOfWeek.Monday, infoConverter.ConvertBack(info, null, null, null));
            Assert.AreSame(
                ConverterResults.UnsetValue,
                infoConverter.ConvertBack("bad", null, null, null)
            );
            var toBool = new ToBoolConverter();
            Assert.AreEqual(true, toBool.Convert("1", null, null, null));
            Assert.AreEqual(false, toBool.Convert(-1, null, null, null));
            toBool.NullValue = null;
            Assert.IsNull(toBool.Convert("bad", null, null, null));
            var marker = new object();
            var enumerable = new ValueToEnumerableConverter();
            Assert.AreSame(marker, ((object[])enumerable.Convert(marker, null, null, null)!)[0]);
            Assert.IsNull(enumerable.Convert(null, null, null, null));
        });

    [TestMethod]
    public void DefaultsRemainDynamicAndNullIsAnExplicitOverride() =>
        PlatformThread.Run(() =>
        {
            var saved = Core.CommonConverterBase.GlobalDefaultResult;
            try
            {
                var converter = new StringCaseConverter();
                var other = new StringCaseConverter();
                var enumConverter = new EnumEqualsConverter();
                var marker = new object();
                Core.CommonConverterBase.GlobalDefaultResult = marker;
                Assert.AreSame(marker, converter.Convert(1, null, null, null));
                Assert.AreEqual(false, enumConverter.Convert(1, null, null, null));
                converter.DefaultResult = null;
                enumConverter.DefaultResult = null;
                Assert.IsNull(converter.Convert(1, null, null, null));
                Assert.IsNull(enumConverter.Convert(1, null, null, null));
                Assert.AreSame(marker, other.Convert(1, null, null, null));
                converter.DefaultResult = ConverterResults.DoNothing;
                Assert.AreSame(ConverterResults.DoNothing, converter.Convert(1, null, null, null));
                converter.ClearValue(ConverterBase.DefaultResultProperty);
                enumConverter.ClearValue(ConverterBase.DefaultResultProperty);
                Assert.AreSame(marker, converter.Convert(1, null, null, null));
                Assert.AreEqual(false, enumConverter.Convert(1, null, null, null));
                Core.CommonConverterBase.GlobalDefaultResult = Core.ConverterResults.UnsetValue;
                Assert.AreSame(ConverterResults.UnsetValue, converter.Convert(1, null, null, null));
            }
            finally
            {
                Core.CommonConverterBase.GlobalDefaultResult = saved;
            }
        });

    [TestMethod]
    public void CultureOverrideAndClearRespectExplicitEnumZero() =>
        PlatformThread.Run(() =>
        {
            var saved = Core.ValueConvertersConfig.DefaultPreferredCulture;
            var savedCurrent = CultureInfo.CurrentCulture;
            var savedUi = CultureInfo.CurrentUICulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("tr-TR");
                var converter = new StringCaseConverter();
                var number = new StringToNumberConverter { NumberType = NumberType.Decimal };
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
                    Core.PreferredCulture.CurrentCulture;
                Assert.AreEqual(
                    12.5m,
                    number.Convert("12,5", null, null, CultureInfo.InvariantCulture)
                );
                Assert.AreEqual(
                    "12,5",
                    number.ConvertBack(12.5m, null, null, CultureInfo.InvariantCulture)
                );
                number.PreferredCulture = Core.PreferredCulture.ConverterCulture;
                Assert.AreEqual(
                    "12.5",
                    number.ConvertBack(12.5m, null, null, CultureInfo.InvariantCulture)
                );
                Assert.AreEqual("12,5", number.ConvertBack(12.5m, null, null, null));
            }
            finally
            {
                Core.ValueConvertersConfig.DefaultPreferredCulture = saved;
                CultureInfo.CurrentCulture = savedCurrent;
                CultureInfo.CurrentUICulture = savedUi;
            }
        });

    [TestMethod]
    public void InterfaceAndDirectEntryPointsMapSentinelsWithoutStringifying() =>
        PlatformThread.Run(() =>
        {
            var core = new EchoCore();
            var wrapper = new Probe(core);
            SingleInterface contract = wrapper;
            var marker = new object();
            foreach (
                var value in new object?[]
                {
                    null,
                    marker,
                    ConverterResults.UnsetValue,
                    ConverterResults.DoNothing,
                    42,
                }
            )
            {
                Assert.AreSame(value, wrapper.Convert(value, null, value, null));
                Assert.AreSame(
                    value,
                    contract.Convert(value, typeof(object), value, CultureInfo.InvariantCulture)
                );
                var expectedCore =
                    ReferenceEquals(value, ConverterResults.UnsetValue)
                        ? Core.ConverterResults.UnsetValue
                    : ReferenceEquals(value, ConverterResults.DoNothing)
                        ? Core.ConverterResults.DoNothing
                    : value;
                Assert.AreSame(expectedCore, core.LastParameter);
                Assert.AreSame(expectedCore, core.LastInput);
                Assert.AreSame(
                    value,
                    contract.ConvertBack(value, typeof(object), value, CultureInfo.InvariantCulture)
                );
                Assert.AreSame(expectedCore, core.LastParameter);
                Assert.AreSame(value, new DebugConverter().Convert(value, null, null, null));
                Assert.AreSame(value, new DebugConverter().ConvertBack(value, null, null, null));
            }
            var multi = new MultiProbe(new EchoMultiCore());
            MultiInterface multiContract = multi;
            var input = new object?[]
            {
                ConverterResults.UnsetValue,
                marker,
                ConverterResults.DoNothing,
            };
            Assert.AreSame(
                ConverterResults.DoNothing,
                multiContract.Convert(
                    input,
                    typeof(object),
                    ConverterResults.DoNothing,
                    CultureInfo.InvariantCulture
                )
            );
            Assert.AreSame(ConverterResults.UnsetValue, input[0]);
            Assert.AreSame(marker, input[1]);
            Assert.AreSame(ConverterResults.DoNothing, input[2]);
        });

    [TestMethod]
    public void ObjectConfigurationSentinelsAndBranchesAreMapped() =>
        PlatformThread.Run(() =>
        {
            var c = new EqualsConverter { Other = Core.ConverterResults.UnsetValue };
            Assert.AreSame(ConverterResults.UnsetValue, c.Convert(1, null, null, null));
            c.Other = ConverterResults.DoNothing;
            Assert.AreSame(ConverterResults.UnsetValue, c.Convert(1, null, null, null));
            var all = new AllEqualsMultiConverter { Value = Core.ConverterResults.UnsetValue };
            Assert.AreSame(ConverterResults.UnsetValue, all.Convert([1], null, null, null));
            all.Value = ConverterResults.DoNothing;
            Assert.AreSame(ConverterResults.UnsetValue, all.Convert([1], null, null, null));
            var branch = new BoolToValueConverter
            {
                TrueValue = Core.ConverterResults.UnsetValue,
                FalseValue = ConverterResults.DoNothing,
            };
            Assert.AreSame(ConverterResults.UnsetValue, branch.Convert(true, null, null, null));
            Assert.AreSame(ConverterResults.DoNothing, branch.Convert(false, null, null, null));
            var input = new object?[]
            {
                true,
                "yes",
                ConverterResults.UnsetValue,
                ConverterResults.DoNothing,
            };
            Assert.AreEqual(
                "yes",
                new FirstBoolToValueMultiConverter().Convert(input, null, null, null)
            );
            Assert.AreSame(ConverterResults.UnsetValue, input[2]);
        });

    [TestMethod]
    public void ConfigurationsAreIsolatedAndExceptionsAreNotSwallowed() =>
        PlatformThread.Run(() =>
        {
            var first = new BoolToValueConverter<string> { TrueValue = "first" };
            var second = new BoolToValueConverter<string> { TrueValue = "second" };
            Assert.AreEqual("first", first.Convert(true, null, null, null));
            first.TrueValue = "changed";
            Assert.AreEqual("changed", first.Convert(true, null, null, null));
            Assert.AreEqual("second", second.Convert(true, null, null, null));
            var split = new BoolToSplitParameterConverter { ConvertSplitValue = s => s.Length };
            Assert.AreEqual(3, split.Convert(true, null, "yes,no", null));
            split.ConvertSplitValue = s => s.ToUpperInvariant();
            Assert.AreEqual("YES", split.Convert(true, null, "yes,no", null));
            var exception = new FormatException("user callback");
            split.ConvertSplitValue = _ => throw exception;
            Assert.AreSame(
                exception,
                Assert.ThrowsExactly<FormatException>(() =>
                    split.Convert(true, null, "yes,no", null)
                )
            );
            Assert.ThrowsExactly<FormatException>(() =>
                new StringFormatMultiConverter().Convert([new ThrowingValue()], null, "{0}", null)
            );
            Assert.ThrowsExactly<NotSupportedException>(() =>
                new NullToBoolConverter().ConvertBack(true, null, null, null)
            );
            var count = 0;
            var core = new EchoCore
            {
                GetDefaultResult = () =>
                {
                    count++;
                    return Core.ConverterResults.DoNothing;
                },
            };
            var probe = new Probe(core);
            Assert.AreEqual(1, probe.Convert(1, null, null, null));
            Assert.AreEqual(0, count);
            core.Fail = true;
            Assert.AreSame(ConverterResults.DoNothing, probe.Convert(1, null, null, null));
            Assert.AreEqual(1, count);
            probe.DefaultResult = null;
            Assert.IsNull(probe.Convert(1, null, null, null));
            Assert.AreEqual(1, count);
        });

    [TestMethod]
    public void EveryWrapperHandlesUnavailableInputOrExplicitPassthrough() =>
        PlatformThread.Run(() =>
        {
            foreach (
                var type in typeof(ConverterBase)
                    .Assembly.GetExportedTypes()
                    .Where(t =>
                        !t.IsAbstract
                        && !t.ContainsGenericParameters
                        && typeof(ConverterBase).IsAssignableFrom(t)
                    )
            )
            {
                var converter = (ConverterBase)Activator.CreateInstance(type)!;
                foreach (
                    var sentinel in new[]
                    {
                        ConverterResults.UnsetValue,
                        ConverterResults.DoNothing,
                    }
                )
                {
                    var result = converter is ValueConverterBase single
                        ? single.Convert(sentinel, null, null, null)
                        : ((MultiValueConverterBase)converter).Convert(
                            [sentinel],
                            null,
                            null,
                            null
                        );
                    if (converter is EnumEqualsConverter)
                        Assert.AreEqual(false, result);
                    else if (converter is DebugConverter)
                        Assert.AreSame(sentinel, result);
                    else
                        Assert.AreSame(ConverterResults.UnsetValue, result, type.Name);
                }
            }
        });

    [TestMethod]
    public void StyledValuesAreHonoredAfterLocalOverridesAreCleared() =>
        PlatformThread.Run(() =>
        {
            var converter = new StringCaseConverter();
            using var cultureValue = converter.SetValue(
                ConverterBase.PreferredCultureProperty,
                Core.PreferredCulture.CurrentUICulture,
                global::Avalonia.Data.BindingPriority.Style
            );
            Assert.AreEqual(Core.PreferredCulture.CurrentUICulture, converter.PreferredCulture);
            converter.PreferredCulture = Core.PreferredCulture.ConverterCulture;
            Assert.AreEqual(Core.PreferredCulture.ConverterCulture, converter.PreferredCulture);
            converter.ClearValue(ConverterBase.PreferredCultureProperty);
            Assert.AreEqual(Core.PreferredCulture.CurrentUICulture, converter.PreferredCulture);
            using var fallback = converter.SetValue(
                ConverterBase.DefaultResultProperty,
                "styled",
                global::Avalonia.Data.BindingPriority.Style
            );
            Assert.AreEqual("styled", converter.Convert(1, null, null, null));
            converter.DefaultResult = null;
            Assert.IsNull(converter.Convert(1, null, null, null));
            converter.ClearValue(ConverterBase.DefaultResultProperty);
            Assert.AreEqual("styled", converter.Convert(1, null, null, null));
        });

    [TestMethod]
    public void NativeSentinelConfigurationDoesNotBecomePropertyCommands() =>
        PlatformThread.Run(() =>
        {
            var converter = new BoolToValueConverter();
            converter.TrueValue = ConverterResults.UnsetValue;
            converter.FalseValue = ConverterResults.DoNothing;
            Assert.AreSame(ConverterResults.UnsetValue, converter.TrueValue);
            Assert.AreSame(ConverterResults.DoNothing, converter.FalseValue);
            Assert.AreSame(ConverterResults.UnsetValue, converter.Convert(true, null, null, null));
            Assert.AreSame(ConverterResults.DoNothing, converter.Convert(false, null, null, null));
            converter.SetValue(
                BoolToValueConverter<object?>.TrueValueProperty,
                Core.ConverterResults.DoNothing
            );
            Assert.AreSame(ConverterResults.DoNothing, converter.Convert(true, null, null, null));
            var enumConverter = new EnumEqualsConverter
            {
                DefaultResult = ConverterResults.UnsetValue,
            };
            Assert.AreSame(ConverterResults.UnsetValue, enumConverter.Convert(1, null, null, null));
            enumConverter.ClearValue(ConverterBase.DefaultResultProperty);
            Assert.AreEqual(false, enumConverter.Convert(1, null, null, null));
        });

    [TestMethod]
    public void MultiCultureAndConstructionDoNotChangeGlobals() =>
        PlatformThread.Run(() =>
        {
            var savedCulture = Core.ValueConvertersConfig.DefaultPreferredCulture;
            var savedDefault = Core.CommonConverterBase.GlobalDefaultResult;
            var savedUi = CultureInfo.CurrentUICulture;
            try
            {
                var marker = new object();
                Core.CommonConverterBase.GlobalDefaultResult = marker;
                Core.ValueConvertersConfig.DefaultPreferredCulture =
                    Core.PreferredCulture.CurrentUICulture;
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("fr-FR");
                var converter = new StringFormatMultiConverter();
                MultiInterface contract = converter;
                Assert.AreEqual(
                    "12,5",
                    contract.Convert([12.5m], typeof(string), "{0}", CultureInfo.InvariantCulture)
                );
                converter.PreferredCulture = Core.PreferredCulture.ConverterCulture;
                Assert.AreEqual(
                    "12.5",
                    contract.Convert([12.5m], typeof(string), "{0}", CultureInfo.InvariantCulture)
                );
                converter.ClearValue(ConverterBase.PreferredCultureProperty);
                Assert.AreEqual(
                    "12,5",
                    converter.Convert([12.5m], null, "{0}", CultureInfo.InvariantCulture)
                );
                Assert.AreEqual(
                    Core.PreferredCulture.CurrentUICulture,
                    Core.ValueConvertersConfig.DefaultPreferredCulture
                );
                Assert.AreSame(marker, Core.CommonConverterBase.GlobalDefaultResult);
            }
            finally
            {
                Core.ValueConvertersConfig.DefaultPreferredCulture = savedCulture;
                Core.CommonConverterBase.GlobalDefaultResult = savedDefault;
                CultureInfo.CurrentUICulture = savedUi;
            }
        });

    private sealed class ThrowingValue
    {
        public override string ToString() => throw new FormatException("user ToString");
    }

    private sealed class Probe(Core.ValueConverterBase converter) : ValueConverterBase(converter);

    private sealed class MultiProbe(Core.MultiValueConverterBase converter)
        : MultiValueConverterBase(converter);

    private sealed class EchoCore : Core.ValueConverterBase
    {
        public object? LastInput { get; private set; }
        public object? LastParameter { get; private set; }
        public bool Fail { get; set; }

        protected override object? ConvertCore(
            object? value,
            Type? targetType,
            object? parameter,
            CultureInfo culture
        )
        {
            LastInput = value;
            LastParameter = parameter;
            return Fail ? GetDefaultResult() : value;
        }

        protected override object? ConvertBackCore(
            object? value,
            Type? targetType,
            object? parameter,
            CultureInfo culture
        ) => ConvertCore(value, targetType, parameter, culture);
    }

    private sealed class EchoMultiCore : Core.MultiValueConverterBase
    {
        protected override object? ConvertCore(
            IList<object?> values,
            Type? targetType,
            object? parameter,
            CultureInfo culture
        )
        {
            Assert.AreSame(Core.ConverterResults.UnsetValue, values[0]);
            Assert.AreSame(Core.ConverterResults.DoNothing, values[2]);
            return parameter;
        }

        protected override object[] ConvertBackCore(
            object? value,
            IList<Type?> targetTypes,
            object? parameter,
            CultureInfo culture
        )
        {
            Assert.AreSame(Core.ConverterResults.UnsetValue, value);
            Assert.AreSame(Core.ConverterResults.DoNothing, parameter);
            return
            [
                Core.ConverterResults.UnsetValue,
                Core.ConverterResults.DoNothing,
                null!,
                targetTypes,
            ];
        }
    }
}
