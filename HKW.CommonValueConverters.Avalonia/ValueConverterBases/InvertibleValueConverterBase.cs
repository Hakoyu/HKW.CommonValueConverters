using global::Avalonia;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// 为 <see cref="Core.InvertibleValueConverterBase"/> 提供可动态配置的反转属性.
/// </summary>
public abstract class InvertibleValueConverterBase : ValueConverterBase
{
    /// <summary>
    /// 连接 <see cref="IsInverted"/> 到独立的 <paramref name="converter"/> 实例.
    /// </summary>
    /// <param name="converter">当前包装器独占的 <see cref="Core.InvertibleValueConverterBase"/> 实例.</param>
    /// <exception cref="ArgumentNullException"><paramref name="converter"/> 为 <see langword="null"/>.</exception>
    protected InvertibleValueConverterBase(Core.InvertibleValueConverterBase converter)
        : base(converter) => converter.GetIsInverted = () => IsInverted;

    /// <summary>
    /// <see cref="IsInverted"/> 的共享属性句柄, 通过 <see cref="Core.CommonDependencyProperty{T}.Value"/> 访问原生属性.
    /// </summary>
    public static readonly Core.CommonDependencyProperty<bool> IsInvertedProperty =
        CommonDependencyProperty.Register<InvertibleValueConverterBase, bool>(
            nameof(IsInverted),
            defaultValue: false
        );

    /// <summary>
    /// 是否反转核心转换器的布尔判断, 每次转换由 <see cref="Core.InvertibleValueConverterBase.GetIsInverted"/> 读取.
    /// </summary>
    public bool IsInverted
    {
        get => GetValue(IsInvertedProperty);
        set => SetValue(IsInvertedProperty, value);
    }
}
