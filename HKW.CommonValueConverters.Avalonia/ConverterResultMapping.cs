using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

internal static class ConverterResultMapping
{
    internal static object? ToCore(object? value)
    {
        if (ReferenceEquals(value, global::Avalonia.AvaloniaProperty.UnsetValue))
            return Core.ConverterResults.UnsetValue;
        else if (ReferenceEquals(value, global::Avalonia.Data.BindingOperations.DoNothing))
            return Core.ConverterResults.DoNothing;
        else
            return value;
    }

    internal static object? FromCore(object? value)
    {
        if (ReferenceEquals(value, Core.ConverterResults.UnsetValue))
            return global::Avalonia.AvaloniaProperty.UnsetValue;
        else if (ReferenceEquals(value, Core.ConverterResults.DoNothing))
            return global::Avalonia.Data.BindingOperations.DoNothing;
        else
            return value;
    }

    internal static T ToCoreValue<T>(T value) =>
        typeof(T) == typeof(object) ? (T)ToCore(value)! : value;

    internal static T FromCoreValue<T>(T value) =>
        typeof(T) == typeof(object) ? (T)FromCore(value)! : value;

    internal static IList<object?> ToCoreValues(IList<object?>? values) =>
        values is null ? null! : values.Select(ToCore).ToArray();
}
