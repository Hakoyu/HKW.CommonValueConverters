using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.GuidToStringConverter"/> 的 Avalonia 包装器.
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
            defaultValue: default(bool?)
        );

    /// <summary>
    /// 格式化结果的大小写选择, <see langword="true"/> 为大写, <see langword="false"/> 为小写, <see langword="null"/> 保持原样.
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
            defaultValue: Core.GuidToStringConverter.DefaultFormat
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
