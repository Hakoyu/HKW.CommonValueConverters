using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.NumberClampConverter"/> 的 WPF 包装器.
/// </summary>
public class NumberClampConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public NumberClampConverter()
        : base(new Core.NumberClampConverter())
    {
        var core = (Core.NumberClampConverter)CoreConverter;
        core.GetNumberType = () => NumberType;
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<NumberType> NumberTypeProperty =
        CommonDependencyProperty.Register<NumberClampConverter, NumberType>(
            nameof(NumberType),
            Core.NumberClampConverter.DefaultNumberType
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
