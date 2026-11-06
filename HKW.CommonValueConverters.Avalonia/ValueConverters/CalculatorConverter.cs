using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.CalculatorConverter"/> 的 Avalonia 包装器.
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
            defaultValue: Core.CalculatorConverter.DefaultNumberType
        );

    /// <summary>
    /// 数值转换或运算所使用的数值类型.
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
            defaultValue: Core.CalculatorConverter.DefaultArithmeticOperatorType
        );

    /// <summary>
    /// 单值计算使用的算术运算符.
    /// </summary>
    public ArithmeticOperatorType OperatorType
    {
        get => GetValue(OperatorTypeProperty);
        set => SetValue(OperatorTypeProperty, value);
    }
}
