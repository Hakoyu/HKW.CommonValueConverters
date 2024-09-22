using System;
using System.Globalization;
using System.Windows;

namespace HKW.CommonValueConverters;

/// <summary>
/// 值相等转换器
/// </summary>
/// <typeparam name="T">值类型</typeparam>
public class EqualsConverter<T> : InvertibleValueConverterBase
{
    /// <summary>
    /// 默认值
    /// </summary>
    public Func<T> GetValue { get; set; } = () => default!;

    /// <summary>
    /// 是字符串比较
    /// </summary>
    public Func<bool> GetIsStringEquals { get; set; } = () => default!;

    /// <inheritdoc/>
    public override object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        var isInverted = GetIsInverted();
        var target = parameter is T t ? t : GetValue();
        if (GetIsStringEquals())
            return value?.ToString() == target?.ToString() ^ isInverted;
        return value?.Equals(target) ^ isInverted;
    }
}
