using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.BoolToValueConverter{T}"/> 的 Avalonia 包装器.
/// </summary>
/// <typeparam name="T">值类型</typeparam>
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
            defaultValue: default(T)!
        );

    /// <summary>
    /// 应用反转后的布尔结果为 <see langword="true"/> 时返回的值.
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
            defaultValue: default(T)!
        );

    /// <summary>
    /// 应用反转后的布尔结果为 <see langword="false"/> 时返回的值.
    /// </summary>
    public T FalseValue
    {
        get => GetMappedValue(FalseValueProperty);
        set => SetMappedValue(FalseValueProperty, value);
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<bool> IsNullableProperty =
        CommonDependencyProperty.Register<BoolToValueConverter<T>, bool>(
            nameof(IsNullable),
            defaultValue: false
        );

    /// <summary>
    /// 是否将空输入单独映射到 <see cref="NullValue"/>.
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
            defaultValue: default(T)!
        );

    /// <summary>
    /// <see cref="IsNullable"/> 为 <see langword="true"/> 且输入为空时返回的值.
    /// </summary>
    public T NullValue
    {
        get => GetMappedValue(NullValueProperty);
        set => SetMappedValue(NullValueProperty, value);
    }
}

/// <summary>
/// 便于 XAML 实例化的 <see cref="BoolToValueConverter{T}"/> 入口, 配置值类型为 <see langword="object"/>.
/// </summary>
public class BoolToValueConverter : BoolToValueConverter<object?> { }
