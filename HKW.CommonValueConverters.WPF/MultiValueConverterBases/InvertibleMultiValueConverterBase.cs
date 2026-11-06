using System.Windows;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// 仅供支持反转的 <see cref="Core.InvertibleMultiValueConverterBase"/> 核心转换器使用.
/// </summary>
public abstract class InvertibleMultiValueConverterBase : MultiValueConverterBase
{
    /// <summary>
    /// 将 <see cref="IsInverted"/> 连接到独立核心实例的 <see cref="Core.InvertibleMultiValueConverterBase.GetIsInverted"/> 委托.
    /// </summary>
    /// <param name="converter">仅由当前包装器使用的可反转核心转换器.</param>
    /// <exception cref="ArgumentNullException"><paramref name="converter"/> 为 <see langword="null"/>.</exception>
    protected InvertibleMultiValueConverterBase(Core.InvertibleMultiValueConverterBase converter)
        : base(converter) => converter.GetIsInverted = () => IsInverted;

    /// <summary>
    /// <see cref="IsInverted"/> 的共享属性句柄 <see cref="Core.CommonDependencyProperty{T}"/>, 其原生值为 <see cref="System.Windows.DependencyProperty"/>.
    /// </summary>
    public static readonly Core.CommonDependencyProperty<bool> IsInvertedProperty =
        CommonDependencyProperty.Register<InvertibleMultiValueConverterBase, bool>(
            nameof(IsInverted),
            false
        );

    /// <summary>
    /// 是否按核心转换器的规则反转转换结果.
    /// </summary>
    public bool IsInverted
    {
        get => GetValue(IsInvertedProperty);
        set => SetValue(IsInvertedProperty, value);
    }
}
