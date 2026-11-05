using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 空到布尔转换器
/// </summary>
public class NullToBoolConverter : InvertibleValueConverterBase
{
    /// <inheritdoc/>
    protected override object? ConvertCore(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (ConverterUtils.IsUnavailable(value))
            return GetDefaultResult();
        return (value is null) ^ GetIsInverted();
    }
}
