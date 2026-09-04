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
public class CalculatorConverter<T> : ValueConverterBase
    where T : struct, INumber<T>
{
    /// <inheritdoc/>
    public CalculatorConverter()
    {
        GetDefaultResult = () => T.Zero;
    }

    /// <summary>
    /// 运算符类型
    /// </summary>
    public Func<ArithmeticOperatorType> GetOperatorType { get; set; } =
        () => ArithmeticOperatorType.Addition;

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
        return NumberUtils.Arithmetic<T>(value!, parameter!, GetOperatorType());
    }
}
