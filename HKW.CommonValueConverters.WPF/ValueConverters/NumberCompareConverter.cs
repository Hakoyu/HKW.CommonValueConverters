using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.NumberCompareConverter"/> 的 WPF 包装器.
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
            Core.NumberCompareConverter.DefaultNumberType
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
