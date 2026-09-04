using System.Globalization;
using System.Numerics;
using HKW.HKWUtils;
using HKW.HKWUtils.Extensions;

namespace HKW.CommonValueConverters;

/// <summary>
/// 数值在范围内转换器
/// <para>
/// 检查数值是否在 MinValue 和 MaxValue 之间
/// </para>
/// </summary>
public class NumberClampConverter : ValueConverterBase
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
        if (value is null)
            return GetDefaultResult();
        var numberType = GetNumberType();
        object min = NumberUtils.GetDefault(numberType);
        object max = NumberUtils.GetDefault(numberType);
        if (parameter is string str)
        {
            var split = str.Split(',');
            min = NumberUtils.ConvertTo(split[0], numberType);
            max = NumberUtils.ConvertTo(split[1], numberType);
        }
        return NumberUtils.CompareBy(value, min, numberType, ComparisonOperatorType.LessThan)
                is false
            && NumberUtils.CompareBy(value, max, numberType, ComparisonOperatorType.GreaterThan)
                is false;
    }
}
