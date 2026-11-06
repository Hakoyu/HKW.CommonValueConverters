using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.StringToNumberConverter"/> 的 Avalonia 包装器.
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
            defaultValue: Core.StringToNumberConverter.DefaultNumberStyles
        );

    /// <summary>
    /// 解析数值字符串时允许的格式样式.
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
            defaultValue: Core.StringToNumberConverter.DefaultNumberType
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
