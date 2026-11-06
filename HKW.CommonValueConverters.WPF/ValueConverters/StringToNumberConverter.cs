using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.StringToNumberConverter"/> 的 WPF 包装器.
/// </summary>
public class StringToNumberConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public StringToNumberConverter()
        : base(new Core.StringToNumberConverter())
    {
        var core = (Core.StringToNumberConverter)CoreConverter;
        core.GetNumberStyles = () => NumberStyles;
        core.GetNumberType = () => NumberType;
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<NumberStyles> NumberStylesProperty =
        CommonDependencyProperty.Register<StringToNumberConverter, NumberStyles>(
            nameof(NumberStyles),
            Core.StringToNumberConverter.DefaultNumberStyles
        );

    /// <summary>
    /// 解析输入字符串时允许的数值格式样式.
    /// </summary>
    public NumberStyles NumberStyles
    {
        get => GetValue(NumberStylesProperty);
        set => SetValue(NumberStylesProperty, value);
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<NumberType> NumberTypeProperty =
        CommonDependencyProperty.Register<StringToNumberConverter, NumberType>(
            nameof(NumberType),
            Core.StringToNumberConverter.DefaultNumberType
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
