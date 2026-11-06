using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.EqualsConverter"/> 的 WPF 包装器.
/// </summary>
public class EqualsConverter : InvertibleValueConverterBase
{
    /// <inheritdoc/>
    public EqualsConverter()
        : base(new Core.EqualsConverter())
    {
        var core = (Core.EqualsConverter)CoreConverter;
        core.GetOther = () => ConverterResultMapping.ToCore(Other);
        core.GetIsStringEquals = () => IsStringEquals;
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<object?> OtherProperty =
        CommonDependencyProperty.Register<EqualsConverter, object?>(
            nameof(Other),
            default(object?)!
        );

    /// <summary>
    /// 未提供转换参数时用于相等比较的目标值.
    /// </summary>
    public object? Other
    {
        get => GetMappedValue(OtherProperty);
        set => SetMappedValue(OtherProperty, value);
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<bool> IsStringEqualsProperty =
        CommonDependencyProperty.Register<EqualsConverter, bool>(
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
