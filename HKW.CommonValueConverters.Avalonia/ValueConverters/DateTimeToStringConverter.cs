using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.DateTimeToStringConverter"/> 的 Avalonia 包装器.
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
            defaultValue: Core.DateTimeToStringConverter.DefaultFormat
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
