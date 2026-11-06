using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.CalculatorMultiConverter"/> 的 Avalonia 包装器
/// </summary>
public class CalculatorMultiConverter : MultiValueConverterBase
{
    /// <inheritdoc/>
    public CalculatorMultiConverter()
        : base(new Core.CalculatorMultiConverter())
    {
        var core = (Core.CalculatorMultiConverter)CoreConverter;
        core.GetNumberType = () => NumberType;
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<NumberType> NumberTypeProperty =
        CommonDependencyProperty.Register<CalculatorMultiConverter, NumberType>(
            nameof(NumberType),
            defaultValue: Core.CalculatorMultiConverter.DefaultNumberType
        );

    /// <summary>
    /// 数值转换或运算所使用的数值类型.
    /// </summary>
    public NumberType NumberType
    {
        get => GetValue(NumberTypeProperty)!;
        set => SetValue(NumberTypeProperty, value);
    }
}
