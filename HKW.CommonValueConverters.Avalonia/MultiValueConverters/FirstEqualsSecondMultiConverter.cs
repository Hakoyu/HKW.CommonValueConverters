using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.FirstEqualsSecondMultiConverter"/> 的 Avalonia 包装器
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
            defaultValue: default(bool)
        );

    /// <summary>
    /// 是否使用字符串表示进行相等比较, 而不是对象相等比较.
    /// </summary>
    public bool IsStringEquals
    {
        get => GetValue(IsStringEqualsProperty);
        set => SetValue(IsStringEqualsProperty, value);
    }
}
