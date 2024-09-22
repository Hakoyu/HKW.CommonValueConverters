using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 值转换器基类
/// </summary>
public abstract class ValueConverterBase : CommonConverterBase
{
    /// <inheritdoc/>
    public abstract object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    );

    /// <inheritdoc/>
    public virtual object? ConvertBack(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        throw new NotSupportedException(
            $"Converter '{GetType().FullName}' does not support backward conversion."
        );
    }
}
