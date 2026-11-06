using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.TimeSpanToStringConverter"/> 的 Avalonia 包装器.
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
            defaultValue: Core.TimeSpanToStringConverter.DefaultFormat
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
