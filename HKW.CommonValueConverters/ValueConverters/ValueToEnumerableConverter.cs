using System;
using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 值到集合转换器
/// </summary>
public class ValueToEnumerableConverter : ValueConverterBase
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
        if (value is not null)
            return new object[] { value };

        return null;
    }
}
