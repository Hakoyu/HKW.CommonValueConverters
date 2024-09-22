using System;
using System.Globalization;
using System.Numerics;
using System.Windows;
using HKW.HKWUtils;
using HKW.HKWUtils.Extensions;

namespace HKW.CommonValueConverters;

/// <summary>
/// 数值在范围内转换器
/// <para>
/// 检查数值是否在 MinValue 和 MaxValue 之间
/// </para>
/// </summary>
public class NumberClampConverter<T> : ValueConverterBase
    where T : struct, INumber<T>
{
    /// <summary>
    /// 最大值
    /// </summary>
    public Func<T> GetMaxValue { get; set; } = () => T.One;

    /// <summary>
    /// 最小值
    /// </summary>
    public Func<T> GetMinValue { get; set; } = () => T.Zero;

    /// <inheritdoc/>
    public override object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        if (value is not T number1)
            return GetDefaultResult();
        object min = GetMinValue();
        object max = GetMaxValue();
        if (parameter is string str)
        {
            var split = str.AsSpan().Split(',');
            if (split.MoveNext())
                min = NumberUtils.ConvertTo<T>(split.Current);
            if (split.MoveNext())
                max = NumberUtils.ConvertTo<T>(split.Current);
        }
        return NumberUtils.CompareX<T>(value, min, ComparisonOperatorType.LessThan) is false
            && NumberUtils.CompareX<T>(value, max, ComparisonOperatorType.GreaterThan) is false;
    }
}
