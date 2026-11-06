using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.AnyEqualsMultiConverter{T}"/> 的 WPF 包装器.
/// </summary>
/// <typeparam name="T">配置和结果值的类型 <typeparamref name="T"/>.</typeparam>
public class AnyEqualsMultiConverter<T> : InvertibleMultiValueConverterBase
{
    /// <inheritdoc/>
    public AnyEqualsMultiConverter()
        : base(new Core.AnyEqualsMultiConverter<T>())
    {
        var core = (Core.AnyEqualsMultiConverter<T>)CoreConverter;
        core.GetValue = () => ConverterResultMapping.ToCoreValue(Value);
        core.GetIsStringEquals = () => IsStringEquals;
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<T> ValueProperty =
        CommonDependencyProperty.Register<AnyEqualsMultiConverter<T>, T>(
            nameof(Value),
            default(T)!
        );

    /// <summary>
    /// 与各输入进行相等比较的目标值.
    /// </summary>
    public T Value
    {
        get => GetMappedValue(ValueProperty);
        set => SetMappedValue(ValueProperty, value);
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<bool> IsStringEqualsProperty =
        CommonDependencyProperty.Register<AnyEqualsMultiConverter<T>, bool>(
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
/// 使用可空 <see langword="object"/> 值的 <see cref="AnyEqualsMultiConverter{T}"/> 非泛型入口, 便于 XAML 实例化.
/// </summary>
public class AnyEqualsMultiConverter : AnyEqualsMultiConverter<object?> { }
