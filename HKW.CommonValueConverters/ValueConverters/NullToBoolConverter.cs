using System;
using System.Collections;
using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 空到布尔转换器
/// </summary>
public class NullToBoolConverter : InvertibleValueConverterBase
{
    /// <inheritdoc/>
    public override object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        return value == null ^ GetIsInverted();
    }
}
