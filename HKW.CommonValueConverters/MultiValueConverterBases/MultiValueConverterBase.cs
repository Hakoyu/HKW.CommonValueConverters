using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 多个值转换器
/// </summary>

public abstract class MultiValueConverterBase : CommonConverterBase
{
    /// <inheritdoc/>
    public abstract object? Convert(
        IList<object?> value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    );

    /// <inheritdoc/>
    public virtual object[] ConvertBack(
        object? value,
        IList<Type?> targetTypes,
        object? parameter,
        CultureInfo? culture
    )
    {
        throw new NotSupportedException(
            $"Converter '{GetType().FullName}' does not support backward conversion."
        );
    }
}
