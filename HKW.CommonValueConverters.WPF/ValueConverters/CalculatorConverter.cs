using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.CalculatorConverter"/> 的 WPF 包装器.
/// </summary>
public class CalculatorConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public CalculatorConverter()
        : base(new Core.CalculatorConverter())
    {
        var core = (Core.CalculatorConverter)CoreConverter;
        core.GetNumberType = () => NumberType;
        core.GetOperatorType = () => OperatorType;
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<NumberType> NumberTypeProperty =
        CommonDependencyProperty.Register<CalculatorConverter, NumberType>(
            nameof(NumberType),
            Core.CalculatorConverter.DefaultNumberType
        );

    /// <summary>
    /// 解析或计算时使用的数值类型.
    /// </summary>
    public NumberType NumberType
    {
        get => GetValue(NumberTypeProperty);
        set => SetValue(NumberTypeProperty, value);
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<ArithmeticOperatorType> OperatorTypeProperty =
        CommonDependencyProperty.Register<CalculatorConverter, ArithmeticOperatorType>(
            nameof(OperatorType),
            Core.CalculatorConverter.DefaultArithmeticOperatorType
        );

    /// <summary>
    /// 将输入与转换参数进行计算时使用的算术运算符.
    /// </summary>
    public ArithmeticOperatorType OperatorType
    {
        get => GetValue(OperatorTypeProperty);
        set => SetValue(OperatorTypeProperty, value);
    }
}
