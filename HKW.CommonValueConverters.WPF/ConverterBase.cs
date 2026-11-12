using System.Windows;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// 持有独立 <see cref="Core.CommonConverterBase"/> 核心实例的 <see cref="DependencyObject"/> 平台转换器基类.
/// </summary>
public abstract class ConverterBase : DependencyObject, Core.ICommonValueConverter
{
    private static readonly object UnconfiguredResult = new();

    // 使用独立的元数据标记区分未配置状态与显式枚举零值, 同时保留各属性值来源的语义.
    private const Core.PreferredCulture UnconfiguredCulture = (Core.PreferredCulture)(-1);
    private readonly Func<object?> _originalDefaultResult;

    /// <summary>
    /// 使用当前包装器独占的 <paramref name="converter"/> 核心实例初始化.
    /// </summary>
    /// <param name="converter">仅由当前包装器使用的核心转换器.</param>
    /// <exception cref="ArgumentNullException"><paramref name="converter"/> 为 <see langword="null"/>.</exception>
    protected ConverterBase(Core.CommonConverterBase converter)
    {
        ArgumentNullException.ThrowIfNull(converter);
        CoreConverter = converter;
        _originalDefaultResult = converter.GetDefaultResult;
        converter.GetDefaultResult = () => ConverterResultMapping.ToCore(DefaultResult);
    }

    /// <summary>
    /// 当前包装器拥有的 <see cref="Core.CommonConverterBase"/> 核心实例, 不应与其他包装器共享.
    /// </summary>
    protected Core.CommonConverterBase CoreConverter { get; }

    /// <summary>
    /// <see cref="DefaultResult"/> 的共享属性句柄 <see cref="Core.CommonDependencyProperty{T}"/>, 其原生值为 <see cref="System.Windows.DependencyProperty"/>.
    /// </summary>
    public static readonly Core.CommonDependencyProperty<object?> DefaultResultProperty =
        CommonDependencyProperty.Register<ConverterBase, object?>(
            nameof(DefaultResult),
            UnconfiguredResult
        );

    /// <summary>
    /// 转换失败时的结果, 显式 <see langword="null"/> 是有效覆盖值; 清除属性恢复核心默认结果委托.
    /// </summary>
    public object? DefaultResult
    {
        get
        {
            var value = GetMappedValue(DefaultResultProperty);
            return ReferenceEquals(value, UnconfiguredResult)
                ? ConverterResultMapping.FromCore(_originalDefaultResult())
                : value;
        }
        set => SetMappedValue(DefaultResultProperty, value);
    }

    /// <summary>
    /// <see cref="PreferredCulture"/> 的共享属性句柄 <see cref="Core.CommonDependencyProperty{T}"/>, 其原生值为 <see cref="System.Windows.DependencyProperty"/>.
    /// </summary>
    public static readonly Core.CommonDependencyProperty<Core.PreferredCulture> PreferredCultureProperty =
        CommonDependencyProperty.Register<ConverterBase, Core.PreferredCulture>(
            nameof(PreferredCulture),
            UnconfiguredCulture
        );

    /// <summary>
    /// 实例的文化偏好, 显式枚举零值仍是覆盖值; 清除属性恢复动态全局偏好.
    /// </summary>
    public Core.PreferredCulture PreferredCulture
    {
        get
        {
            var value = GetMappedValue(PreferredCultureProperty);
            return value == UnconfiguredCulture
                ? Core.ValueConvertersConfig.DefaultPreferredCulture
                : value;
        }
        set => SetMappedValue(PreferredCultureProperty, value);
    }

    /// <summary>
    /// 读取 <paramref name="property"/> 的有效原生值, 不映射转换器哨兵.
    /// </summary>
    /// <typeparam name="T">与注册类型完全一致的属性值类型.</typeparam>
    /// <param name="property">封装 WPF <see cref="DependencyProperty"/> 的共享句柄.</param>
    /// <returns><see cref="DependencyObject.GetValue(DependencyProperty)"/> 返回的有效值.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">句柄中不包含值类型与 <typeparamref name="T"/> 完全一致的 WPF 属性.</exception>
    public T GetValue<T>(Core.CommonDependencyProperty<T> property) =>
        (T)base.GetValue(Unwrap(property));

    /// <summary>
    /// 设置 <paramref name="property"/> 的原生本地值, 不映射转换器哨兵.
    /// </summary>
    /// <typeparam name="T">与注册类型完全一致的属性值类型.</typeparam>
    /// <param name="property">封装 WPF <see cref="DependencyProperty"/> 的共享句柄.</param>
    /// <param name="value">原样传递给 <see cref="DependencyObject.SetValue(DependencyProperty, object)"/> 的值.</param>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">句柄中不包含值类型与 <typeparamref name="T"/> 完全一致的 WPF 属性, 或该值未通过原生验证.</exception>
    public void SetValue<T>(Core.CommonDependencyProperty<T> property, T value) =>
        base.SetValue(Unwrap(property), value);

    /// <summary>
    /// 使用 <see cref="DependencyObject.ClearValue(DependencyProperty)"/> 清除本地值, 恢复原生属性的值来源.
    /// </summary>
    /// <typeparam name="T">与注册类型完全一致的属性值类型.</typeparam>
    /// <param name="property">封装 WPF <see cref="DependencyProperty"/> 的共享句柄.</param>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">句柄中不包含值类型与 <typeparamref name="T"/> 完全一致的 WPF 属性.</exception>
    public void ClearValue<T>(Core.CommonDependencyProperty<T> property) =>
        base.ClearValue(Unwrap(property));

    /// <summary>
    /// 使用 <see cref="DependencyObject.SetCurrentValue(DependencyProperty, object)"/> 修改有效值, 不替换其原生值来源.
    /// </summary>
    /// <typeparam name="T">与注册类型完全一致的属性值类型.</typeparam>
    /// <param name="property">封装 WPF <see cref="DependencyProperty"/> 的共享句柄.</param>
    /// <param name="value">原样传递给原生属性系统的值.</param>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">句柄中不包含值类型与 <typeparamref name="T"/> 完全一致的 WPF 属性, 或该值未通过原生验证.</exception>
    public void SetCurrentValue<T>(Core.CommonDependencyProperty<T> property, T value) =>
        base.SetCurrentValue(Unwrap(property), value);

    /// <summary>
    /// 读取 <paramref name="property"/> 并将核心哨兵映射为平台值, 不改变 <see cref="GetValue{T}"/> 的原生访问语义.
    /// </summary>
    /// <typeparam name="T">属性值类型.</typeparam>
    /// <param name="property">待读取的共享属性句柄.</param>
    /// <returns>映射后的属性值, 普通值和 <see langword="null"/> 保持不变.</returns>
    protected T GetMappedValue<T>(Core.CommonDependencyProperty<T> property) =>
        ConverterResultMapping.FromCoreValue(GetValue(property));

    /// <summary>
    /// 将 <paramref name="value"/> 中的平台哨兵映射为核心值后写入 <paramref name="property"/>, 避免被原生属性系统当作控制指令.
    /// </summary>
    /// <typeparam name="T">属性值类型.</typeparam>
    /// <param name="property">待写入的共享属性句柄.</param>
    /// <param name="value">配置值, 普通值和 <see langword="null"/> 保持不变.</param>
    protected void SetMappedValue<T>(Core.CommonDependencyProperty<T> property, T value) =>
        SetValue(property, ConverterResultMapping.ToCoreValue(value));

    private static DependencyProperty Unwrap<T>(Core.CommonDependencyProperty<T> property)
    {
        ArgumentNullException.ThrowIfNull(property);
        if (property.Value is not DependencyProperty native || native.PropertyType != typeof(T))
            throw new ArgumentException(
                "The handle must contain a WPF dependency property with the exact value type.",
                nameof(property)
            );
        return native;
    }

    /// <summary>
    /// 创建核心获取器
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="property">属性</param>
    /// <returns>获取器</returns>
    protected Func<T> CreateCoreGetter<T>(Core.CommonDependencyProperty<T> property) =>
        () => ConverterResultMapping.ToCoreValue(GetValue(property));

    /// <summary>
    /// 检查 <see cref="DependencyObject"/> 的线程访问并同步 <see cref="PreferredCulture"/>; 文化选择仍由核心入口完成.
    /// </summary>
    protected void PrepareConversion()
    {
        VerifyAccess();
        CoreConverter.PreferredCulture = PreferredCulture;
    }
}
