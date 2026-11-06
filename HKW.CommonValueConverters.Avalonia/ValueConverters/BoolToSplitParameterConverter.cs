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
        core.ConvertSplitValue = value => ConvertSplitValue(value);
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
    /// 转换选中的分割字符串; <see cref="Core.BoolToSplitParameterConverter.ConvertSplitValue"/> 每次调用读取最新委托.
    /// </summary>
    public Func<string, object> ConvertSplitValue { get; set; } = value => value;
}
