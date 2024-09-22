using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using HKW.HKWUtils;

namespace HKW.CommonValueConverters;

/// <summary>
/// 集合数量相等
/// <para>示例:
/// <code><![CDATA[
/// <Binding Collection, Converter="{StaticResource BoolToParameterDoubleConverter}" ConverterParameter="0"/>
/// return: Collection.Count == Parameter
/// ]]></code></para>
/// </summary>
public class CollectionCountCompareConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public CollectionCountCompareConverter()
    {
        GetDefaultResult = () => 0;
    }

    /// <summary>
    /// 数量
    /// </summary>
    public Func<int> GetDefeatCount { get; set; } = () => 0;

    /// <summary>
    /// 获取比较方式类型
    /// </summary>
    public Func<ComparisonOperatorType> GetComparisonType { get; set; } =
        () => ComparisonOperatorType.Equality;

    /// <inheritdoc/>
    public override object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        if (int.TryParse(parameter?.ToString(), out var count) is false)
            count = GetDefeatCount();
        if (value is ICollection collection)
            return NumberUtils.Compare<int>(collection.Count, count);
        else if (value is IEnumerable enumerable)
            return NumberUtils.Compare<int>(enumerable.Cast<object>().Count(), count);

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
        if (int.TryParse(parameter?.ToString(), out var count) is false)
            count = GetDefeatCount();
        if (value is ICollection collection)
            return NumberUtils.Compare<int>(collection.Count, count);
        else if (value is IEnumerable enumerable)
            return NumberUtils.Compare<int>(enumerable.Cast<object>().Count(), count);

        return GetDefaultResult();
    }
}
