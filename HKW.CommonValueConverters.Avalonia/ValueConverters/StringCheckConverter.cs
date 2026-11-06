using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.StringCheckConverter"/> 的 Avalonia 包装器.
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
            defaultValue: Core.StringCheckConverter.DefaultStringCheckType
        );

    /// <summary>
    /// 检查字符串为空或空白时使用的规则.
    /// </summary>
    public StringCheckType StringCheckType
    {
        get => GetValue(StringCheckTypeProperty);
        set => SetValue(StringCheckTypeProperty, value);
    }
}
