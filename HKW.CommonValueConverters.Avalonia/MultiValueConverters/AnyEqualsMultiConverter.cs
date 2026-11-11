using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.AnyEqualsMultiConverter{T}"/> 的 Avalonia 包装器
/// </summary>
/// <typeparam name="T">值类型</typeparam>
public class AnyEqualsMultiConverter<T> : InvertibleMultiValueConverterBase
{
    /// <inheritdoc/>
    public AnyEqualsMultiConverter()
        : base(new Core.AnyEqualsMultiConverter<T>())
    {
        var core = (Core.AnyEqualsMultiConverter<T>)CoreConverter;
        core.GetValue = CreateCoreGetter(ValueProperty);
        core.GetIsStringEquals = () => IsStringEquals;
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<T> ValueProperty =
        CommonDependencyProperty.Register<AnyEqualsMultiConverter<T>, T>(
            nameof(Value),
            defaultValue: default(T)!
        );

    /// <summary>
    /// 与输入集合中的元素比较的目标值.
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
/// 便于 XAML 实例化的 <see cref="AnyEqualsMultiConverter{T}"/> 入口, 配置值类型为 <see langword="object"/>.
/// </summary>
public class AnyEqualsMultiConverter : AnyEqualsMultiConverter<object?> { }
