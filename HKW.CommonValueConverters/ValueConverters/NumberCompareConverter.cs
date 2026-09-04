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
public class NumberCompareConverter : ValueConverterBase
{
    /// <summary>
    /// 默认数值类型
    /// </summary>
    public const NumberType DefaultNumberType = NumberType.Int32;

    /// <summary>
    /// 数值类型
    /// </summary>
    public Func<NumberType> GetNumberType { get; set; } = static () => DefaultNumberType;

    /// <inheritdoc/>
    public override object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        return NumberUtils.Compare(value!, parameter!, GetNumberType());
    }
}

/// <summary>
/// 相等字符串转换器
/// <para>示例:
/// <code><![CDATA[
/// {Binding Number, Converter={StaticResource NumberCompareByConverter}, ConverterParameter=">1"}
/// result: Number.CompareBy(Parameter)
/// ParameterExamples: "==1", ">1", "<=5"
/// ]]></code></para>
/// </summary>
public class NumberCompareByConverter : ValueConverterBase
{
    /// <summary>
    /// 默认数值类型
    /// </summary>
    public const NumberType DefaultNumberType = NumberType.Int32;

    /// <summary>
    /// 默认数值类型
    /// </summary>
    public const ComparisonOperatorType DefaultComparisonType = ComparisonOperatorType.Equality;

    /// <summary>
    /// 数值类型
    /// </summary>
    public Func<NumberType> GetNumberType { get; set; } = static () => DefaultNumberType;

    /// <summary>
    /// 比较类型
    /// </summary>
    public Func<ComparisonOperatorType> GetComparisonType { get; set; } =
        () => DefaultComparisonType;

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
            comparisonType = NumberUtils.GetComparisonOperatorType(str, out var operatorLength);
            if (operatorLength == 0)
                return GetDefaultResult();
            parameter = str[operatorLength..];
        }
        return NumberUtils.CompareBy(value!, parameter!, GetNumberType(), comparisonType);
    }
}
