using Avalonia;
using Avalonia.Data;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

internal static class AvaloniaPropertyAssertions
{
    internal static void Defaults(ConverterCase scenario, Func<ConverterBase> createPristine)
    {
        var native = (StyledProperty<object?>)ConverterBase.DefaultResultProperty.Value;
        Assert.AreEqual(nameof(ConverterBase.DefaultResult), native.Name);
        Assert.AreEqual(typeof(object), native.PropertyType);
        Assert.AreEqual(typeof(ConverterBase), native.OwnerType);
        var first = scenario.Create();
        var second = scenario.Create();
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
