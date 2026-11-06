using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.AllEqualsMultiConverter{T}"/> 的 Avalonia 包装器
/// </summary>
/// <typeparam name="T">值类型</typeparam>
public class AllEqualsMultiConverter<T> : InvertibleMultiValueConverterBase
{
    /// <inheritdoc/>
    public AllEqualsMultiConverter()
        : base(new Core.AllEqualsMultiConverter<T>())
    {
        var core = (Core.AllEqualsMultiConverter<T>)CoreConverter;
        core.GetValue = () => ConverterResultMapping.ToCoreValue(Value);
        core.GetIsStringEquals = () => IsStringEquals;
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<T> ValueProperty =
        CommonDependencyProperty.Register<AllEqualsMultiConverter<T>, T>(
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
        CommonDependencyProperty.Register<AllEqualsMultiConverter<T>, bool>(
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
/// 便于 XAML 实例化的 <see cref="AllEqualsMultiConverter{T}"/> 入口, 配置值类型为 <see langword="object"/>.
/// </summary>
public class AllEqualsMultiConverter : AllEqualsMultiConverter<object?> { }
