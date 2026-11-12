using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.BoolToSplitParameterConverter"/> 的 Avalonia 包装器.
/// </summary>
public class BoolToSplitParameterConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public BoolToSplitParameterConverter()
        : base(new Core.BoolToSplitParameterConverter())
    {
        var core = (Core.BoolToSplitParameterConverter)CoreConverter;
        core.GetSeparator = () => Separator;
        core.ConvertSplitValue = (s, c) => ConvertSplitValue(s, c);
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<string> SeparatorProperty =
        CommonDependencyProperty.Register<BoolToSplitParameterConverter, string>(
            nameof(Separator),
            defaultValue: Core.BoolToSplitParameterConverter.DefaultSeparator
        );

    /// <summary>
    /// 分割转换参数时使用的分隔符.
    /// </summary>
    public string Separator
    {
        get => GetValue(SeparatorProperty);
        set => SetValue(SeparatorProperty, value);
    }

    /// <summary>
    /// 转换选中的分割字符串
    /// </summary>
    public Func<string, CultureInfo, object> ConvertSplitValue { get; set; } = (s, _) => s;
}
