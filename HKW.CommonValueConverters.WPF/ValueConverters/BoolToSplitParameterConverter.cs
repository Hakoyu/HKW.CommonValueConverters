using System.ComponentModel;
using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.BoolToSplitParameterConverter"/> 的 WPF 包装器.
/// </summary>
public class BoolToSplitParameterConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public BoolToSplitParameterConverter()
        : base(new Core.BoolToSplitParameterConverter())
    {
        var core = (Core.BoolToSplitParameterConverter)CoreConverter;
        core.GetSeparator = () => Separator;
        core.ConvertSplitValue = ConvertSplitValue;
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<string> SeparatorProperty =
        CommonDependencyProperty.Register<BoolToSplitParameterConverter, string>(
            nameof(Separator),
            Core.BoolToSplitParameterConverter.DefaultSeparator
        );

    /// <summary>
    /// 拆分转换参数以选取布尔结果时使用的分隔字符串.
    /// </summary>
    [DefaultValue(Core.BoolToSplitParameterConverter.DefaultSeparator)]
    public string Separator
    {
        get => GetValue(SeparatorProperty);
        set => SetValue(SeparatorProperty, value);
    }

    /// <summary>
    /// 转换选中的分割字符串; <see cref="Core.BoolToSplitParameterConverter.ConvertSplitValue"/> 每次调用读取最新委托. 默认原样返回选中的字符串.
    /// </summary>
    public Func<string, object> ConvertSplitValue { get; set; } = value => value;
}
