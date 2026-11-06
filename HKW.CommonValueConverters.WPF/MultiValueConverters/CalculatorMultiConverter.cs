using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.CalculatorMultiConverter"/> 的 WPF 包装器.
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
            Core.CalculatorMultiConverter.DefaultNumberType
        );

    /// <summary>
    /// 解析或计算时使用的数值类型.
    /// </summary>
    public NumberType NumberType
    {
        get => GetValue(NumberTypeProperty);
        set => SetValue(NumberTypeProperty, value);
    }
}
