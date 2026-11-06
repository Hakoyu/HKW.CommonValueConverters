using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.DateTimeToStringConverter"/> 的 WPF 包装器.
/// </summary>
public class DateTimeToStringConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public DateTimeToStringConverter()
        : base(new Core.DateTimeToStringConverter())
    {
        var core = (Core.DateTimeToStringConverter)CoreConverter;
        core.GetFormat = () => Format;
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<string> FormatProperty =
        CommonDependencyProperty.Register<DateTimeToStringConverter, string>(
            nameof(Format),
            Core.DateTimeToStringConverter.DefaultFormat
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
