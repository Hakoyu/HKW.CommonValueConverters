using global::Avalonia;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// 注册原生 <see cref="StyledProperty{TValue}"/> 实例并封装为共享的 <see cref="Core.CommonDependencyProperty{T}"/> 句柄.
/// </summary>
public static class CommonDependencyProperty
{
    /// <summary>
    /// 使用 <typeparamref name="TProperty"/> 的默认值注册属性.
    /// </summary>
    /// <typeparam name="TOwner">拥有该属性的 <see cref="AvaloniaObject"/> 类型.</typeparam>
    /// <typeparam name="TProperty">属性值类型.</typeparam>
    /// <param name="propertyName">原生属性名称.</param>
    /// <returns>保存已注册的 <see cref="StyledProperty{TValue}"/> 实例的句柄.</returns>
    /// <remarks>
    /// 名称验证及注册异常遵循 <see cref="AvaloniaProperty"/> 的注册规则.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> 包含句点, 导致原生注册过程拒绝该名称.</exception>
    public static Core.CommonDependencyProperty<TProperty> Register<TOwner, TProperty>(
        string propertyName
    )
        where TOwner : AvaloniaObject =>
        new(AvaloniaProperty.Register<TOwner, TProperty>(propertyName));

    /// <summary>
    /// 注册属性, 并将 <paramref name="defaultValue"/> 设为其原生元数据默认值.
    /// </summary>
    /// <typeparam name="TOwner">拥有该属性的 <see cref="AvaloniaObject"/> 类型.</typeparam>
    /// <typeparam name="TProperty">属性值类型.</typeparam>
    /// <param name="propertyName">原生属性名称.</param>
    /// <param name="defaultValue">不存在更高优先级的值时使用的默认值.</param>
    /// <returns>保存已注册的 <see cref="StyledProperty{TValue}"/> 实例的句柄.</returns>
    /// <remarks>
    /// 名称验证及注册异常遵循 <see cref="AvaloniaProperty"/> 的注册规则.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> 包含句点, 导致原生注册过程拒绝该名称.</exception>
    public static Core.CommonDependencyProperty<TProperty> Register<TOwner, TProperty>(
        string propertyName,
        TProperty defaultValue
    )
        where TOwner : AvaloniaObject =>
        new(AvaloniaProperty.Register<TOwner, TProperty>(propertyName, defaultValue));
}
