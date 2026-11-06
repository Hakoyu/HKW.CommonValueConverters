using System.Windows;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// 注册原生 WPF 属性并封装为共享的 <see cref="Core.CommonDependencyProperty{T}"/> 句柄.
/// </summary>
public static class CommonDependencyProperty
{
    /// <summary>
    /// 使用 <typeparamref name="TProperty"/> 的原生默认值注册属性.
    /// </summary>
    /// <typeparam name="TOwner">拥有该属性的 <see cref="DependencyObject"/> 类型.</typeparam>
    /// <typeparam name="TProperty">属性值的确切类型.</typeparam>
    /// <param name="propertyName">传递给 <see cref="DependencyProperty.Register(string, Type, Type)"/> 的属性名称.</param>
    /// <returns>共享属性句柄, 其 <see cref="Core.CommonDependencyProperty{T}.Value"/> 为已注册的 <see cref="DependencyProperty"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> 为空字符串, 或原生注册过程拒绝该属性.</exception>
    public static Core.CommonDependencyProperty<TProperty> Register<TOwner, TProperty>(
        string propertyName
    )
        where TOwner : DependencyObject =>
        new(DependencyProperty.Register(propertyName, typeof(TProperty), typeof(TOwner)));

    /// <summary>
    /// 注册属性, 并将 <paramref name="defaultValue"/> 设为其原生元数据默认值.
    /// </summary>
    /// <typeparam name="TOwner">拥有该属性的 <see cref="DependencyObject"/> 类型.</typeparam>
    /// <typeparam name="TProperty">属性值的确切类型.</typeparam>
    /// <param name="propertyName">传递给 <see cref="DependencyProperty.Register(string, Type, Type, PropertyMetadata)"/> 的属性名称.</param>
    /// <param name="defaultValue"><see cref="PropertyMetadata"/> 保存的默认值.</param>
    /// <returns>共享属性句柄, 其 <see cref="Core.CommonDependencyProperty{T}.Value"/> 为已注册的 <see cref="DependencyProperty"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">属性名称或默认值不符合 WPF 原生注册规则.</exception>
    public static Core.CommonDependencyProperty<TProperty> Register<TOwner, TProperty>(
        string propertyName,
        TProperty defaultValue
    )
        where TOwner : DependencyObject =>
        new(
            DependencyProperty.Register(
                propertyName,
                typeof(TProperty),
                typeof(TOwner),
                new PropertyMetadata(defaultValue)
            )
        );
}
