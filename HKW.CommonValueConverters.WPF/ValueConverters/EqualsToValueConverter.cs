using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.EqualsToValueConverter{T}"/> 的 WPF 包装器.
/// </summary>
/// <typeparam name="T">配置和结果值的类型 <typeparamref name="T"/>.</typeparam>
public class EqualsToValueConverter<T> : ValueConverterBase
{
    /// <inheritdoc/>
    public EqualsToValueConverter()
        : base(new Core.EqualsToValueConverter<T>())
    {
        var core = (Core.EqualsToValueConverter<T>)CoreConverter;
        core.GetOther = () => ConverterResultMapping.ToCore(Other);
        core.GetTrueValue = () => ConverterResultMapping.ToCoreValue(TrueValue);
        core.GetFalseValue = () => ConverterResultMapping.ToCoreValue(FalseValue);
        core.GetNullValue = () => ConverterResultMapping.ToCoreValue(NullValue);
        core.GetIsStringEquals = () => IsStringEquals;
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<object?> OtherProperty =
        CommonDependencyProperty.Register<EqualsToValueConverter<T>, object?>(
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
    public static readonly Core.CommonDependencyProperty<T> TrueValueProperty =
        CommonDependencyProperty.Register<EqualsToValueConverter<T>, T>(
            nameof(TrueValue),
            default(T)!
        );

    /// <summary>
    /// 输入与比较目标相等时返回的值.
    /// </summary>
    public T TrueValue
    {
        get => GetMappedValue(TrueValueProperty);
        set => SetMappedValue(TrueValueProperty, value);
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<T> FalseValueProperty =
        CommonDependencyProperty.Register<EqualsToValueConverter<T>, T>(
            nameof(FalseValue),
            default(T)!
        );

    /// <summary>
    /// 输入与比较目标不相等时返回的值.
    /// </summary>
    public T FalseValue
    {
        get => GetMappedValue(FalseValueProperty);
        set => SetMappedValue(FalseValueProperty, value);
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<T> NullValueProperty =
        CommonDependencyProperty.Register<EqualsToValueConverter<T>, T>(
            nameof(NullValue),
            default(T)!
        );

    /// <summary>
    /// 输入为 <see langword="null"/> 时返回的值.
    /// </summary>
    public T NullValue
    {
        get => GetMappedValue(NullValueProperty);
        set => SetMappedValue(NullValueProperty, value);
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<bool> IsStringEqualsProperty =
        CommonDependencyProperty.Register<EqualsToValueConverter<T>, bool>(
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

/// <summary>
/// 使用可空 <see langword="object"/> 值的 <see cref="EqualsToValueConverter{T}"/> 非泛型入口, 便于 XAML 实例化.
/// </summary>
public class EqualsToValueConverter : EqualsToValueConverter<object?> { }
