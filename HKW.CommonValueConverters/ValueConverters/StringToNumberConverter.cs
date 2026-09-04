using System;
using System.Globalization;
using System.Numerics;

namespace HKW.CommonValueConverters;

/// <summary>
/// 字符串到数字转换器
/// </summary>
public class StringToNumberConverter<T> : ValueConverterBase
    where T : struct, INumber<T>
{
    /// <summary>
    /// 默认格式化
    /// </summary>
    protected const NumberStyles DefaultNumberStyles = NumberStyles.Any;

    /// <summary>
    /// 获取数字风格
    /// </summary>
    private Func<NumberStyles> GetNumberStyles { get; } = () => NumberStyles.Any;

    /// <inheritdoc/>
    public override object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        if (T.TryParse(value?.ToString(), GetNumberStyles(), culture, out var result))
            return result;
        else
            return GetDefaultResult();
    }

    /// <inheritdoc/>
    public override object? ConvertBack(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        return value?.ToString();
    }
}
