using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.AllBoolToValueMultiConverter{T}"/> 的 Avalonia 包装器
/// </summary>
/// <typeparam name="T">值类型</typeparam>
public class AllBoolToValueMultiConverter<T> : MultiValueConverterBase
{
    /// <inheritdoc/>
    public AllBoolToValueMultiConverter()
        : base(new Core.AllBoolToValueMultiConverter<T>())
    {
        var core = (Core.AllBoolToValueMultiConverter<T>)CoreConverter;
        core.GetTrueValue = CreateCoreGetter(TrueValueProperty);
        core.GetFalseValue = CreateCoreGetter(FalseValueProperty);
        core.GetNullValue = CreateCoreGetter(NullValueProperty);
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<T> TrueValueProperty =
        CommonDependencyProperty.Register<AllBoolToValueMultiConverter<T>, T>(
            nameof(TrueValue),
            defaultValue: default(T)!
        );

    /// <summary>
    /// 全部输入为 <see langword="true"/> 时返回的值.
    /// </summary>
    public T TrueValue
    {
        get => GetMappedValue(TrueValueProperty);
        set => SetMappedValue(TrueValueProperty, value);
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<T> FalseValueProperty =
        CommonDependencyProperty.Register<AllBoolToValueMultiConverter<T>, T>(
            nameof(FalseValue),
            defaultValue: default(T)!
        );

    /// <summary>
    /// 全部输入为 <see langword="false"/> 时返回的值.
    /// </summary>
    public T FalseValue
    {
        get => GetMappedValue(FalseValueProperty);
        set => SetMappedValue(FalseValueProperty, value);
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<T> NullValueProperty =
        CommonDependencyProperty.Register<AllBoolToValueMultiConverter<T>, T>(
            nameof(NullValue),
            defaultValue: default(T)!
        );

    /// <summary>
    /// 全部输入为 <see langword="null"/> 时返回的值.
    /// </summary>
    public T NullValue
    {
        get => GetMappedValue(NullValueProperty);
        set => SetMappedValue(NullValueProperty, value);
    }
}

/// <summary>
/// 便于 XAML 实例化的 <see cref="AllBoolToValueMultiConverter{T}"/> 入口, 配置值类型为 <see langword="object"/>.
/// </summary>
public class AllBoolToValueMultiConverter : AllBoolToValueMultiConverter<object?> { }
