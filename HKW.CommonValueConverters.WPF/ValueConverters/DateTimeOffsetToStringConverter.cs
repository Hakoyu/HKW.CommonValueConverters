using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.DateTimeOffsetToStringConverter"/> 的 WPF 包装器.
/// </summary>
public class DateTimeOffsetToStringConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public DateTimeOffsetToStringConverter()
        : base(new Core.DateTimeOffsetToStringConverter())
    {
        var core = (Core.DateTimeOffsetToStringConverter)CoreConverter;
        core.GetFormat = () => Format;
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<string> FormatProperty =
        CommonDependencyProperty.Register<DateTimeOffsetToStringConverter, string>(
            nameof(Format),
            Core.DateTimeOffsetToStringConverter.DefaultFormat
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
