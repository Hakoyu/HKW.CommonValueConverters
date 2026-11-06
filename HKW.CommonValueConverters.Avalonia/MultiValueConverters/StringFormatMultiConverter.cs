using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.StringFormatMultiConverter"/> 的 Avalonia 包装器
/// </summary>
public class StringFormatMultiConverter : MultiValueConverterBase
{
    /// <inheritdoc/>
    public StringFormatMultiConverter()
        : base(new Core.StringFormatMultiConverter())
    {
        var core = (Core.StringFormatMultiConverter)CoreConverter;
        core.GetReplaceUnsetValue = () => ReplaceUnsetValue;
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<string?> ReplaceUnsetValueProperty =
        CommonDependencyProperty.Register<StringFormatMultiConverter, string?>(
            nameof(ReplaceUnsetValue),
            defaultValue: default(string?)
        );

    /// <summary>
    /// 格式化输入集合时替换未设置值的字符串.
    /// </summary>
    public string? ReplaceUnsetValue
    {
        get => GetValue(ReplaceUnsetValueProperty);
        set => SetValue(ReplaceUnsetValueProperty, value);
    }
}
