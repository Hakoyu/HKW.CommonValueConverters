using System.Globalization;
using System.Numerics;
using HKW.HKWUtils;

namespace HKW.CommonValueConverters;

/// <summary>
/// 计算器转换器
/// <para>示例:
/// <code><![CDATA[
/// <Binding Number,Converter="{StaticResource CalculatorConverter}",ConverterParameter="8"/>
/// return: Number + 8
/// ]]></code></para>
/// </summary>
public class CalculatorConverter : ValueConverterBase
{
    /// <summary>
    /// 默认数值类型
    /// </summary>
    public const NumberType DefaultNumberType = NumberType.Int32;

    /// <summary>
    /// 数值类型
    /// </summary>
    public Func<NumberType> GetNumberType { get; set; } = () => DefaultNumberType;

    /// <summary>
    /// 默认运算符类型
    /// </summary>
    public const ArithmeticOperatorType DefaultArithmeticOperatorType =
        ArithmeticOperatorType.Addition;

    /// <summary>
    /// 运算符类型
    /// </summary>
    public Func<ArithmeticOperatorType> GetOperatorType { get; set; } =
        () => DefaultArithmeticOperatorType;

    /// <inheritdoc/>
    public override object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        if (value == UnsetValue || parameter == UnsetValue)
            return GetDefaultResult();
        return NumberUtils.Arithmetic(value!, parameter!, GetNumberType(), GetOperatorType());
    }
}
