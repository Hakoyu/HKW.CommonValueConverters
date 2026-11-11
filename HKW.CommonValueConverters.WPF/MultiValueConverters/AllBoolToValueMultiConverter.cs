using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.AllBoolToValueMultiConverter{T}"/> 的 WPF 包装器.
/// </summary>
/// <typeparam name="T">配置和结果值的类型 <typeparamref name="T"/>.</typeparam>
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
            default(T)!
        );

    /// <summary>
    /// 全部输入均为 <see langword="true"/> 时返回的值.
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
            default(T)!
        );

    /// <summary>
    /// 全部输入均为 <see langword="false"/> 时返回的值.
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
            default(T)!
        );

    /// <summary>
    /// 全部输入均为 <see langword="null"/> 时返回的值.
    /// </summary>
    public T NullValue
    {
        get => GetMappedValue(NullValueProperty);
        set => SetMappedValue(NullValueProperty, value);
    }
}

/// <summary>
/// 使用可空 <see langword="object"/> 值的 <see cref="AllBoolToValueMultiConverter{T}"/> 非泛型入口, 便于 XAML 实例化.
/// </summary>
public class AllBoolToValueMultiConverter : AllBoolToValueMultiConverter<object?> { }
