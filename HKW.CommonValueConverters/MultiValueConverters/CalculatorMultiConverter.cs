using System.Globalization;
using System.Numerics;
using HKW.HKWUtils;

namespace HKW.CommonValueConverters;

/// <summary>
/// 计算器转换器
/// <para>示例:
/// <code><![CDATA[
/// <MultiBinding Converter="{StaticResource CalculatorMultiConverter}">
///   <Binding Path="Num1" />
///   <Binding Source="+" />
///   <Binding Path="Num2" />
///   <Binding Source="-" />
///   <Binding Path="Num3" />
///   <Binding Source="*" />
///   <Binding Path="Num4" />
///   <Binding Source="/" />
///   <Binding Path="Num5" />
/// </MultiBinding>
/// //
/// <MultiBinding Converter="{StaticResource CalculatorMultiConverter}" ConverterParameter="+-*/">
///   <Binding Path="Num1" />
///   <Binding Path="Num2" />
///   <Binding Path="Num3" />
///   <Binding Path="Num4" />
///   <Binding Path="Num5" />
/// </MultiBinding>
/// ]]></code></para>
/// </summary>
/// <remarks>数量、操作符或数值无效时返回默认结果；按输入顺序从左到右计算。</remarks>
public class CalculatorMultiConverter : MultiValueConverterBase
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
        IList<object?> values,
        Type? targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (ConverterUtils.HasUnavailable(values) || ConverterUtils.IsUnavailable(parameter) || values.Count < 2)
            return GetDefaultResult();
        var numberType = GetNumberType();
        if (!ConverterUtils.TryNumber(values[0], numberType, culture, out var result))
            return GetDefaultResult();
        if (parameter is string operators && !string.IsNullOrWhiteSpace(operators))
        {
            if (operators.Length != values.Count - 1)
                return GetDefaultResult();
            for (var i = 1; i < values.Count; i++)
            {
                if (!NumberUtils.TryGetArithmeticOperatorType(operators[i - 1], out var operation)
                    || !ConverterUtils.TryNumber(values[i], numberType, culture, out var operand)
                    || !ConverterUtils.TryArithmetic(result, operand, numberType, operation, out result))
                    return GetDefaultResult();
            }
        }
        else
        {
            if (values.Count % 2 == 0)
                return GetDefaultResult();
            for (var i = 1; i < values.Count; i += 2)
            {
                if (values[i] is not string token || token.Length != 1
                    || !NumberUtils.TryGetArithmeticOperatorType(token[0], out var operation)
                    || !ConverterUtils.TryNumber(values[i + 1], numberType, culture, out var operand)
                    || !ConverterUtils.TryArithmetic(result, operand, numberType, operation, out result))
                    return GetDefaultResult();
            }
        }
        return result;
    }
}
