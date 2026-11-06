using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.DateTimeOffsetToStringConverter"/> 的 Avalonia 包装器.
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
            defaultValue: Core.DateTimeOffsetToStringConverter.DefaultFormat
        );

    /// <summary>
    /// 格式化输入值时使用的格式字符串.
    /// </summary>
    public string Format
    {
        get => GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }
}
