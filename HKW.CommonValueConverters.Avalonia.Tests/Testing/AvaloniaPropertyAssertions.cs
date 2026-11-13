using System.Reflection;
using Avalonia;
using Avalonia.Data;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

internal static class AvaloniaPropertyAssertions
{
    internal static void Defaults(ConverterCase scenario, Func<ConverterBase> createPristine)
    {
        VerifyRegisteredMetadata(scenario.Type);
        var native = (StyledProperty<object?>)ConverterBase.DefaultResultProperty.Value;
        Assert.AreEqual(nameof(ConverterBase.DefaultResult), native.Name);
        Assert.AreEqual(typeof(object), native.PropertyType);
        Assert.AreEqual(typeof(ConverterBase), native.OwnerType);
        var first = scenario.Create();
        var second = scenario.Create();
        CompareRegisteredProperties(scenario.Type, first, second);
        var original = first.DefaultResult;
        var marker = new object();
        first.DefaultResult = marker;
        Assert.AreSame(marker, first.DefaultResult, scenario.Id);
        Assert.AreNotSame(marker, second.DefaultResult, scenario.Id);
        first.DefaultResult = null;
        Assert.IsNull(first.DefaultResult, scenario.Id);
        first.ClearValue(ConverterBase.DefaultResultProperty);
        var pristine = createPristine();
        Assert.AreEqual(pristine.DefaultResult, first.DefaultResult, scenario.Id);
        Assert.AreEqual(original, second.DefaultResult, scenario.Id);
    }

    internal static void VerifyRegisteredMetadata(Type converterType)
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
            var handle = field.GetValue(null)!;
            var native = (AvaloniaProperty)field.FieldType.GetProperty("Value")!.GetValue(handle)!;
            Assert.AreEqual(field.FieldType.GenericTypeArguments[0], native.PropertyType);
            Assert.AreEqual(field.Name[..^"Property".Length], native.Name);
            Assert.IsTrue(native.OwnerType.IsAssignableFrom(converterType));
            var clrProperty = converterType.GetProperty(native.Name);
            Assert.IsNotNull(clrProperty);
            Assert.IsTrue(clrProperty.CanRead && clrProperty.CanWrite);
        }
    }

    internal static void CompareRegisteredProperties(
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
            ConverterAssertions.ConfigurationEqual(
                clrProperty.GetValue(expected),
                clrProperty.GetValue(actual)
            );
        }
    }

    internal static void Property<TConverter, TValue>(
        TConverter converter,
        TConverter pristine,
        Core.CommonDependencyProperty<TValue> handle,
        string name,
        Func<TConverter, TValue> read,
        Action<TConverter, TValue> write,
        TValue alternate
    )
        where TConverter : ConverterBase
    {
        var native = (StyledProperty<TValue>)handle.Value;
        Assert.AreEqual(name, native.Name);
        Assert.AreEqual(typeof(TValue), native.PropertyType);
        Assert.IsTrue(native.OwnerType.IsAssignableFrom(typeof(TConverter)));
        var original = read(converter);
        var peerOriginal = read(pristine);
        write(converter, alternate);
        Assert.AreEqual(alternate, read(converter));
        Assert.AreEqual(alternate, converter.GetValue(handle));
        Assert.AreEqual(peerOriginal, read(pristine));
        converter.SetValue(handle, original);
        Assert.AreEqual(original, read(converter));
        converter.ClearValue(handle);
        pristine.ClearValue(handle);
        Assert.AreEqual(read(pristine), read(converter));
        using (converter.SetValue(handle, alternate, BindingPriority.Style))
        {
            Assert.AreEqual(alternate, read(converter));
            converter.SetValue(handle, original);
            Assert.AreEqual(original, read(converter));
            converter.ClearValue(handle);
            Assert.AreEqual(alternate, read(converter));
        }
        Assert.AreEqual(read(pristine), read(converter));
    }
}
