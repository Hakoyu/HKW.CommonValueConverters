using System.Globalization;
using System.Numerics;
using System.Windows;
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
/// <exception cref="Exception">绑定的数量不正确</exception>
public class CalculatorMultiConverter<T> : MultiValueConverterBase
    where T : struct, INumber<T>
{
    /// <inheritdoc/>
    public override object? Convert(
        IList<object?> values,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        if (values.Any(i => i == UnsetValue))
            return GetDefaultResult();
        if (values.Count == 1)
            return values[0];
        var result = (T)NumberUtils.ConvertTo<T>(values[0]);
        if (parameter is string operators && string.IsNullOrWhiteSpace(operators) is false)
        {
            if (operators.Length != values.Count - 1)
                throw new Exception("Parameter error: operator must be one more than parameter");
            for (int i = 1; i < values.Count - 1; i++)
                result = (T)
                    NumberUtils.Arithmetic<T>(
                        result,
                        NumberUtils.ConvertTo<T>(values[i]),
                        operators[i - 1]
                    );
            result = (T)
                NumberUtils.Arithmetic<T>(
                    result,
                    NumberUtils.ConvertTo<T>(values.Last()),
                    operators.Last()
                );
        }
        else
        {
            if (System.Convert.ToBoolean(values.Count & 1) is false)
                throw new Exception("Parameter error: Incorrect quantity");
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
                    var value = NumberUtils.ConvertTo<T>(values[i]);
                    result = (T)NumberUtils.Arithmetic<T>(result, value, currentOperator);
                    isNumber = false;
                }
            }
            result = (T)
                NumberUtils.Arithmetic<T>(
                    result,
                    NumberUtils.ConvertTo<T>(values.Last()),
                    currentOperator
                );
        }
        return result;
    }
}
