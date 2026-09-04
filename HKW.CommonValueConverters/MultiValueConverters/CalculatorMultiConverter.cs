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
/// <exception cref="ConverterException">绑定的数量不正确</exception>
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
    public override object? Convert(
        IList<object?> values,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        var numberType = GetNumberType();
        if (values.Any(i => i == UnsetValue))
            return GetDefaultResult();
        if (values.Count == 1)
            return GetDefaultResult();
        var result = NumberUtils.ConvertTo(values[0]!, numberType);
        if (parameter is string operators && string.IsNullOrWhiteSpace(operators) is false)
        {
            result = GetResult(values, operators, result, numberType);
        }
        else
        {
            result = GetResult(values, result, numberType);
        }
        return result;
    }

    private static object GetResult(
        IList<object?> values,
        string operators,
        object value,
        NumberType numberType
    )
    {
        var result = value;
        if (operators.Length != values.Count - 1)
            throw new ConverterException(
                "Parameter error: operator must be one more than parameter"
            );
        for (int i = 1; i < values.Count - 1; i++)
        {
            result = NumberUtils.Arithmetic(
                result,
                NumberUtils.ConvertTo(values[i]!, numberType),
                numberType,
                operators[i - 1]
            );
        }
        result = NumberUtils.Arithmetic(
            result,
            NumberUtils.ConvertTo(values[^1]!, numberType),
            numberType,
            operators.Last()
        );
        return result;
    }

    private static object GetResult(IList<object?> values, object value, NumberType numberType)
    {
        var result = value;
        if (System.Convert.ToBoolean(values.Count & 1) is false)
            throw new ConverterException("Parameter error: Incorrect quantity");
        bool isNumber = false;
        char currentOperator = '0';
        for (int i = 1; i < values.Count - 1; i++)
        {
            if (isNumber is false)
            {
                currentOperator = ((string)values[i]!)[0];
                isNumber = true;
            }
            else
            {
                var temp = NumberUtils.ConvertTo(values[i]!, numberType);
                result = NumberUtils.Arithmetic(result, temp, numberType, currentOperator);
                isNumber = false;
            }
        }
        result = NumberUtils.Arithmetic(
            result,
            NumberUtils.ConvertTo(values[^1]!, numberType),
            numberType,
            currentOperator
        );
        return result;
    }
}
