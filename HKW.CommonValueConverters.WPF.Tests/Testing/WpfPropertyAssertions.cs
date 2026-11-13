using System.Reflection;
using System.Windows;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF.Tests;

internal static class WpfPropertyAssertions
{
    public static void Verify(WpfPlatformCase scenario) =>
        PlatformThread.Run(() =>
        {
            var item = scenario.Create();
            var first = item.Factory();
            var second = item.Factory();
            Assert.AreNotSame(first, second);
            Assert.AreEqual(scenario.Type, first.GetType());
            var fields = scenario
                .Type.GetFields(
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy
                )
                .Where(field =>
                    field.FieldType.IsGenericType
                    && field.FieldType.GetGenericTypeDefinition()
                        == typeof(Core.CommonDependencyProperty<>)
                )
                .ToArray();
            Assert.IsTrue(fields.Length >= 2);
            foreach (var field in fields)
            {
                var handle = field.GetValue(null)!;
                var native = (DependencyProperty)
                    field.FieldType.GetProperty("Value")!.GetValue(handle)!;
                Assert.AreEqual(field.FieldType.GenericTypeArguments[0], native.PropertyType);
                Assert.AreEqual(field.Name[..^"Property".Length], native.Name);
                Assert.IsTrue(native.OwnerType.IsAssignableFrom(scenario.Type));
                var clrProperty = scenario.Type.GetProperty(native.Name);
                Assert.IsNotNull(clrProperty);
                Assert.IsTrue(clrProperty.CanRead && clrProperty.CanWrite);
            }
            CompareRegisteredProperties(scenario.Type, first, second);
            scenario.AssertConfiguration?.Invoke(first, second);
            scenario.VerifyWiring();
            first.DefaultResult = "first";
            second.DefaultResult = "second";
            Assert.AreEqual("first", first.DefaultResult);
            Assert.AreEqual("second", second.DefaultResult);
            first.ClearValue(ConverterBase.DefaultResultProperty);
            var fresh = item.Factory();
            fresh.ClearValue(ConverterBase.DefaultResultProperty);
            WpfAssert.AssertResult(fresh.DefaultResult, first.DefaultResult);
            Assert.AreEqual("second", second.DefaultResult);
        });

    public static void VerifyInherited(
        Func<ConverterBase> factory,
        bool passThroughUnavailable = false
    )
    {
        var first = factory();
        var second = factory();
        first.ClearValue(ConverterBase.DefaultResultProperty);
        var initial = first.DefaultResult;
        var isolated = second.DefaultResult;
        object? Convert(ConverterBase converter) =>
            converter is ValueConverterBase single
                ? single.Convert(DependencyProperty.UnsetValue, null, null, null)
                : ((MultiValueConverterBase)converter).Convert(
                    [DependencyProperty.UnsetValue],
                    null,
                    null,
                    null
                );
        first.DefaultResult = "clr-fallback";
        Assert.AreEqual("clr-fallback", first.DefaultResult);
        WpfAssert.AssertResult(
            passThroughUnavailable ? DependencyProperty.UnsetValue : "clr-fallback",
            Convert(first)
        );
        first.SetValue(ConverterBase.DefaultResultProperty, "native-fallback");
        WpfAssert.AssertResult(
            passThroughUnavailable ? DependencyProperty.UnsetValue : "native-fallback",
            Convert(first)
        );
        first.SetCurrentValue(ConverterBase.DefaultResultProperty, "current-fallback");
        WpfAssert.AssertResult(
            passThroughUnavailable ? DependencyProperty.UnsetValue : "current-fallback",
            Convert(first)
        );
        first.DefaultResult = null;
        Assert.IsNull(first.DefaultResult);
        WpfAssert.AssertResult(
            passThroughUnavailable ? DependencyProperty.UnsetValue : null,
            Convert(first)
        );
        first.ClearValue(ConverterBase.DefaultResultProperty);
        WpfAssert.AssertResult(initial, first.DefaultResult);
        WpfAssert.AssertResult(
            passThroughUnavailable ? DependencyProperty.UnsetValue : initial,
            Convert(first)
        );
        WpfAssert.AssertResult(isolated, second.DefaultResult);
        var originalCulture = first.PreferredCulture;
        first.PreferredCulture = Core.PreferredCulture.ConverterCulture;
        Assert.AreEqual(Core.PreferredCulture.ConverterCulture, first.PreferredCulture);
        first.SetValue(
            ConverterBase.PreferredCultureProperty,
            Core.PreferredCulture.CurrentCulture
        );
        Assert.AreEqual(Core.PreferredCulture.CurrentCulture, first.PreferredCulture);
        first.ClearValue(ConverterBase.PreferredCultureProperty);
        Assert.AreEqual(originalCulture, first.PreferredCulture);
    }

    public static void Probe<TConverter, TValue>(
        Func<TConverter> factory,
        Core.CommonDependencyProperty<TValue> property,
        Action<TConverter, TValue> set,
        Func<TConverter, TValue> get,
        TValue firstValue,
        TValue secondValue,
        Func<TConverter, object?> convert,
        object? firstResult,
        object? secondResult
    )
        where TConverter : ConverterBase
    {
        var first = factory();
        var second = factory();
        var native = (DependencyProperty)property.Value;
        first.ClearValue(native);
        second.ClearValue(native);
        var initial = convert(first);
        var otherInitial = convert(second);
        var initialValue = get(first);
        set(first, firstValue);
        WpfAssert.AssertResult(firstValue, get(first));
        WpfAssert.AssertResult(firstResult, convert(first));
        first.SetValue(native, secondValue);
        WpfAssert.AssertResult(secondValue, get(first));
        WpfAssert.AssertResult(secondResult, convert(first));
        first.SetCurrentValue(native, firstValue);
        WpfAssert.AssertResult(firstResult, convert(first));
        first.ClearValue(native);
        WpfAssert.AssertResult(initialValue, get(first));
        WpfAssert.AssertResult(initial, convert(first));
        WpfAssert.AssertResult(otherInitial, convert(second));
    }

    public static void CompareRegisteredProperties(
        Type converterType,
        ConverterBase expected,
        ConverterBase actual
    )
    {
        var fields = converterType
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(field =>
                field.FieldType.IsGenericType
                && field.FieldType.GetGenericTypeDefinition()
                    == typeof(Core.CommonDependencyProperty<>)
            )
            .ToArray();
        Assert.IsTrue(fields.Length >= 2);
        foreach (var field in fields)
        {
            var propertyName = field.Name[..^"Property".Length];
            var clrProperty = converterType.GetProperty(propertyName);
            Assert.IsNotNull(
                clrProperty,
                $"Missing CLR property '{propertyName}' for '{converterType.FullName}.{field.Name}'."
            );
            WpfAssert.AssertResult(clrProperty.GetValue(expected), clrProperty.GetValue(actual));
        }
    }
}
