using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.StringFormatMultiConverter"/> 的 WPF 包装器.
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
            default(string?)
        );

    /// <summary>
    /// 格式化多值输入时替换未设置值的字符串. 为 <see langword="null"/> 时, 未设置的输入触发默认结果而不是替换.
    /// </summary>
    public string? ReplaceUnsetValue
    {
        get => GetValue(ReplaceUnsetValueProperty);
        set => SetValue(ReplaceUnsetValueProperty, value);
    }
}
