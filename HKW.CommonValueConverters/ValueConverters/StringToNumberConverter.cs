using System;
using System.Globalization;
using System.Numerics;
using HKW.HKWUtils;

namespace HKW.CommonValueConverters;

/// <summary>
/// 字符串到数字转换器
/// </summary>
public class StringToNumberConverter : ValueConverterBase
{
    /// <summary>
    /// 默认数字风格
    /// </summary>
    public const NumberStyles DefaultNumberStyles = NumberStyles.Any;

    /// <summary>
    /// 获取数字风格
    /// </summary>
    public Func<NumberStyles> GetNumberStyles { get; set; } = () => DefaultNumberStyles;

    /// <summary>
    /// 默认数值类型
    /// </summary>
    public const NumberType DefaultNumberType = NumberType.Int32;

    /// <summary>
    /// 数值类型
    /// </summary>
    public Func<NumberType> GetNumberType { get; set; } = () => DefaultNumberType;

    /// <inheritdoc/>
    public override object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        if (
            NumberUtils.TryParse(
                GetNumberType(),
                value?.ToString(),
                GetNumberStyles(),
                culture,
                out var result
            )
        )
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
