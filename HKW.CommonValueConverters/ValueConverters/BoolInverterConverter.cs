using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace HKW.CommonValueConverters;

/// <summary>
/// 布尔值取反, 可单独设定 null 的返回值
/// </summary>
public class BoolInverterConverter : ValueConverterBase
{
    /// <summary>
    /// 获取布尔值
    /// </summary>
    public Func<bool> GetNullValue { get; set; } = () => false;

    /// <inheritdoc/>
    protected override object? ConvertCore(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (value is null)
            return GetNullValue();
        else
            return value is bool boolean ? boolean is false : GetDefaultResult();
    }
}
