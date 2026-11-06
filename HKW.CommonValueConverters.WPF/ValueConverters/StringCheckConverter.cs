using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.StringCheckConverter"/> 的 WPF 包装器.
/// </summary>
public class StringCheckConverter : InvertibleValueConverterBase
{
    /// <inheritdoc/>
    public StringCheckConverter()
        : base(new Core.StringCheckConverter())
    {
        var core = (Core.StringCheckConverter)CoreConverter;
        core.GetStringCheckType = () => StringCheckType;
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<StringCheckType> StringCheckTypeProperty =
        CommonDependencyProperty.Register<StringCheckConverter, StringCheckType>(
            nameof(StringCheckType),
            Core.StringCheckConverter.DefaultStringCheckType
        );

    /// <summary>
    /// 对输入字符串执行的状态检查类型.
    /// </summary>
    public StringCheckType StringCheckType
    {
        get => GetValue(StringCheckTypeProperty);
        set => SetValue(StringCheckTypeProperty, value);
    }
}
