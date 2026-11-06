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
    protected override object? ConvertCore(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        var numberType = GetNumberType();
        if (
            ConverterUtils.IsUnavailable(parameter)
            || ConverterUtils.TryNumber(value, numberType, culture, out var number) is false
        )
            return GetDefaultResult();
        object min = NumberUtils.GetDefault(numberType);
        object max = NumberUtils.GetDefault(numberType);
        if (parameter is string str)
        {
            var split = str.Split(',');
            if (
                split.Length != 2
                || ConverterUtils.TryNumber(split[0], numberType, culture, out min) is false
                || ConverterUtils.TryNumber(split[1], numberType, culture, out max) is false
                || NumberUtils.CompareBy(min, max, numberType, ComparisonOperatorType.GreaterThan)
                    is true
            )
                return GetDefaultResult();
        }
        return NumberUtils.CompareBy(number, min, numberType, ComparisonOperatorType.LessThan)
                is false
            && NumberUtils.CompareBy(number, max, numberType, ComparisonOperatorType.GreaterThan)
                is false;
    }
}
