using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.NumberClampConverter"/> 的 Avalonia 包装器.
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
            defaultValue: Core.NumberClampConverter.DefaultNumberType
        );

    /// <summary>
    /// 数值转换或运算所使用的数值类型.
    /// </summary>
    public NumberType NumberType
    {
        get => GetValue(NumberTypeProperty);
        set => SetValue(NumberTypeProperty, value);
    }
}
