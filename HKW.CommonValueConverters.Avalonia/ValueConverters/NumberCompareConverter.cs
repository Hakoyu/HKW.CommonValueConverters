using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.NumberCompareConverter"/> 的 Avalonia 包装器.
/// </summary>
public class NumberCompareConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public NumberCompareConverter()
        : base(new Core.NumberCompareConverter())
    {
        var core = (Core.NumberCompareConverter)CoreConverter;
        core.GetNumberType = () => NumberType;
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<NumberType> NumberTypeProperty =
        CommonDependencyProperty.Register<NumberCompareConverter, NumberType>(
            nameof(NumberType),
            defaultValue: Core.NumberCompareConverter.DefaultNumberType
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
