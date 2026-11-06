using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.EqualsCountMultiConverter{T}"/> 的 Avalonia 包装器.
/// </summary>
/// <typeparam name="T">值类型</typeparam>
public class EqualsCountMultiConverter<T> : InvertibleMultiValueConverterBase
{
    /// <inheritdoc/>
    public EqualsCountMultiConverter()
        : base(new Core.EqualsCountMultiConverter<T>())
    {
        var core = (Core.EqualsCountMultiConverter<T>)CoreConverter;
        core.GetValue = () => ConverterResultMapping.ToCoreValue(Value);
        core.GetIsStringEquals = () => IsStringEquals;
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<T> ValueProperty =
        CommonDependencyProperty.Register<EqualsCountMultiConverter<T>, T>(
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
        CommonDependencyProperty.Register<EqualsCountMultiConverter<T>, bool>(
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
/// 便于 XAML 实例化的 <see cref="EqualsCountMultiConverter{T}"/> 入口, 配置值类型为 <see langword="object"/>.
/// </summary>
public class EqualsCountMultiConverter : EqualsCountMultiConverter<object?> { }
