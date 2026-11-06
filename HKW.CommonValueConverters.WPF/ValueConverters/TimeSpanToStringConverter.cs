using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.TimeSpanToStringConverter"/> 的 WPF 包装器.
/// </summary>
public class TimeSpanToStringConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public TimeSpanToStringConverter()
        : base(new Core.TimeSpanToStringConverter())
    {
        var core = (Core.TimeSpanToStringConverter)CoreConverter;
        core.GetFormat = () => Format;
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<string> FormatProperty =
        CommonDependencyProperty.Register<TimeSpanToStringConverter, string>(
            nameof(Format),
            Core.TimeSpanToStringConverter.DefaultFormat
        );

    /// <summary>
    /// 未提供字符串转换参数时使用的格式字符串.
    /// </summary>
    public string Format
    {
        get => GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }
}
