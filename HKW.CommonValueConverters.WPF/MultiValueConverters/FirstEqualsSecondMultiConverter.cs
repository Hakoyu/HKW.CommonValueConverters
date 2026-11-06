using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.FirstEqualsSecondMultiConverter"/> 的 WPF 包装器.
/// </summary>
public class FirstEqualsSecondMultiConverter : InvertibleMultiValueConverterBase
{
    /// <inheritdoc/>
    public FirstEqualsSecondMultiConverter()
        : base(new Core.FirstEqualsSecondMultiConverter())
    {
        var core = (Core.FirstEqualsSecondMultiConverter)CoreConverter;
        core.GetIsStringEquals = () => IsStringEquals;
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<bool> IsStringEqualsProperty =
        CommonDependencyProperty.Register<FirstEqualsSecondMultiConverter, bool>(
            nameof(IsStringEquals),
            default(bool)
        );

    /// <summary>
    /// 是否比较值的字符串表示而不是调用对象相等比较.
    /// </summary>
    public bool IsStringEquals
    {
        get => GetValue(IsStringEqualsProperty);
        set => SetValue(IsStringEqualsProperty, value);
    }
}
