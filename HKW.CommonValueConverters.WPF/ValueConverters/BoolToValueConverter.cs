using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.BoolToValueConverter{T}"/> 的 WPF 包装器.
/// </summary>
/// <typeparam name="T">配置和结果值的类型 <typeparamref name="T"/>.</typeparam>
public class BoolToValueConverter<T> : InvertibleValueConverterBase
{
    /// <inheritdoc/>
    public BoolToValueConverter()
        : base(new Core.BoolToValueConverter<T>())
    {
        var core = (Core.BoolToValueConverter<T>)CoreConverter;
        core.GetTrueValue = CreateCoreGetter(TrueValueProperty);
        core.GetFalseValue = CreateCoreGetter(FalseValueProperty);
        core.GetIsNullable = () => IsNullable;
        core.GetNullValue = CreateCoreGetter(NullValueProperty);
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<T> TrueValueProperty =
        CommonDependencyProperty.Register<BoolToValueConverter<T>, T>(
            nameof(TrueValue),
            default(T)!
        );

    /// <summary>
    /// 输入解释为布尔值并应用 <see cref="InvertibleValueConverterBase.IsInverted"/> 后为 <see langword="true"/> 时返回的值.
    /// </summary>
    public T TrueValue
    {
        get => GetMappedValue(TrueValueProperty);
        set => SetMappedValue(TrueValueProperty, value);
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<T> FalseValueProperty =
        CommonDependencyProperty.Register<BoolToValueConverter<T>, T>(
            nameof(FalseValue),
            default(T)!
        );

    /// <summary>
    /// 输入解释为布尔值并应用 <see cref="InvertibleValueConverterBase.IsInverted"/> 后为 <see langword="false"/> 时返回的值.
    /// </summary>
    public T FalseValue
    {
        get => GetMappedValue(FalseValueProperty);
        set => SetMappedValue(FalseValueProperty, value);
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<bool> IsNullableProperty =
        CommonDependencyProperty.Register<BoolToValueConverter<T>, bool>(nameof(IsNullable), false);

    /// <summary>
    /// 是否将 <see langword="null"/> 输入转换为 <see cref="NullValue"/>.
    /// </summary>
    public bool IsNullable
    {
        get => GetValue(IsNullableProperty);
        set => SetValue(IsNullableProperty, value);
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<T> NullValueProperty =
        CommonDependencyProperty.Register<BoolToValueConverter<T>, T>(
            nameof(NullValue),
            default(T)!
        );

    /// <summary>
    /// <see cref="IsNullable"/> 为 <see langword="true"/> 且输入为 <see langword="null"/> 时返回的值.
    /// </summary>
    public T NullValue
    {
        get => GetMappedValue(NullValueProperty);
        set => SetMappedValue(NullValueProperty, value);
    }
}

/// <summary>
/// 使用可空 <see langword="object"/> 值的 <see cref="BoolToValueConverter{T}"/> 非泛型入口, 便于 XAML 实例化.
/// </summary>
public class BoolToValueConverter : BoolToValueConverter<object?> { }
