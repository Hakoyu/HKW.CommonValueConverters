using System.Collections.Frozen;
using System.Globalization;
using System.Numerics;
using HKW.HKWUtils;

namespace HKW.CommonValueConverters;

/// <summary>
/// 相等字符串转换器
/// <para>示例:
/// <code><![CDATA[
/// IsEnabled={Binding Number, Converter={StaticResource NumberCompareConverter}, ConverterParameter="1"}
/// result: Number.CompareTo(Parameter)
/// ]]></code></para>
/// </summary>
public class NumberCompareConverter<T> : ValueConverterBase
    where T : struct, INumber<T>
{
    /// <inheritdoc/>
    public override object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        return NumberUtils.Compare<T>(value, parameter);
    }
}

/// <summary>
/// 相等字符串转换器
/// <para>示例:
/// <code><![CDATA[
/// {Binding Number, Converter={StaticResource NumberCompareXConverter}, ConverterParameter="1"}
/// result: Number.CompareTo(Parameter)
/// ]]></code></para>
/// </summary>
public class NumberCompareXConverter<T> : ValueConverterBase
    where T : struct, INumber<T>
{
    /// <summary>
    /// 比较类型
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
        var comparisonType = GetComparisonType();
        if (parameter is string str && str.Length >= 2 && char.IsNumber(str[0]) is false)
        {
            comparisonType = NumberUtils.GetComparisonOperatorType(str);
            parameter = str[(NumberUtils.ComparisonOperatorTypeByString[comparisonType].Length)..];
        }
        return NumberUtils.CompareX<T>(value, parameter, comparisonType);
    }
}
