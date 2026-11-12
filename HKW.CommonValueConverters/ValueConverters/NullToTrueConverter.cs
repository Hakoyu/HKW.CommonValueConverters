using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 空到布尔真值转换器
/// </summary>
public class NullToTrueConverter : InvertibleValueConverterBase
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
