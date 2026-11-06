using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

internal static class ConverterResultMapping
{
    internal static object? ToCore(object? value)
    {
        if (ReferenceEquals(value, System.Windows.DependencyProperty.UnsetValue))
        {
            return Core.ConverterResults.UnsetValue;
        }
        else if (ReferenceEquals(value, System.Windows.Data.Binding.DoNothing))
        {
            return Core.ConverterResults.DoNothing;
        }
        else
            return value;
    }

    internal static object? FromCore(object? value)
    {
        if (ReferenceEquals(value, Core.ConverterResults.UnsetValue))
        {
            return System.Windows.DependencyProperty.UnsetValue;
        }
        else if (ReferenceEquals(value, Core.ConverterResults.DoNothing))
        {
            return System.Windows.Data.Binding.DoNothing;
        }
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
