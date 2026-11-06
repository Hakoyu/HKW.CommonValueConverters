using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.EqualsToValueConverter{T}"/> 的 Avalonia 包装器.
/// </summary>
/// <typeparam name="T">值类型</typeparam>
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
    public static readonly Core.CommonDependencyProperty<T> TrueValueProperty =
        CommonDependencyProperty.Register<EqualsToValueConverter<T>, T>(
            nameof(TrueValue),
            defaultValue: default(T)!
        );

    /// <summary>
    /// 输入与目标相等时返回的值.
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
            defaultValue: default(T)!
        );

    /// <summary>
    /// 输入与目标不相等时返回的值.
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
            defaultValue: default(T)!
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

/// <summary>
/// 便于 XAML 实例化的 <see cref="EqualsToValueConverter{T}"/> 入口, 配置值类型为 <see langword="object"/>.
/// </summary>
public class EqualsToValueConverter : EqualsToValueConverter<object?> { }
