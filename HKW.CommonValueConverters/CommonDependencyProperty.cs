namespace HKW.CommonValueConverters;

/// <summary>
/// 保存原生平台属性的通用句柄, 属性值类型为 <typeparamref name="T"/>.
/// </summary>
/// <remarks>
/// <see cref="Value"/> 的平台和属性值类型由平台访问入口验证, 此类型不依赖任何 UI 框架.
/// </remarks>
/// <typeparam name="T">值类型</typeparam>
public sealed class CommonDependencyProperty<T>
{
    /// <summary>
    /// 使用原生属性 <paramref name="value"/> 创建句柄.
    /// </summary>
    /// <param name="value">需要保存的原生平台属性对象.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> 为 <see langword="null"/>.</exception>
    public CommonDependencyProperty(object value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Value = value;
    }

    /// <summary>
    /// 原生平台属性对象, 可由平台 API 解包使用.
    /// </summary>
    public object Value { get; }
}
