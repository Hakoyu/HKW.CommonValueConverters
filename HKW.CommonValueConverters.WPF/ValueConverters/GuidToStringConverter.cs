using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.GuidToStringConverter"/> 的 WPF 包装器.
/// </summary>
public class GuidToStringConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public GuidToStringConverter()
        : base(new Core.GuidToStringConverter())
    {
        var core = (Core.GuidToStringConverter)CoreConverter;
        core.GetStringTo = () => StringTo;
        core.GetFormat = () => Format;
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<bool?> StringToProperty =
        CommonDependencyProperty.Register<GuidToStringConverter, bool?>(
            nameof(StringTo),
            default(bool?)
        );

    /// <summary>
    /// 结果字母的大小写, <see langword="true"/> 表示大写, <see langword="false"/> 表示小写, <see langword="null"/> 表示不改变.
    /// </summary>
    public bool? StringTo
    {
        get => (bool?)GetValue(StringToProperty)!;
        set => SetValue(StringToProperty, value);
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<string> FormatProperty =
        CommonDependencyProperty.Register<GuidToStringConverter, string>(
            nameof(Format),
            Core.GuidToStringConverter.DefaultFormat
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
