using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.EqualsConverter"/> 的 Avalonia 包装器.
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
            defaultValue: default(object?)!
        );

    /// <summary>
    /// 转换参数未提供时用于相等比较的目标值.
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
