using global::Avalonia;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// 持有独立 <see cref="Core.CommonConverterBase"/> 实例的 <see cref="AvaloniaObject"/> 转换器基类.
/// </summary>
public abstract class ConverterBase : AvaloniaObject, Core.ICommonValueConverter
{
    private static readonly object UnconfiguredResult = new();

    /// <summary>
    /// 使用独立的元数据标记区分未配置状态与显式枚举零值, 同时保留各属性值来源的语义.
    /// </summary>
    private const Core.PreferredCulture UnconfiguredCulture = (Core.PreferredCulture)(-1);
    private readonly Func<object?> _originalDefaultResult;

    /// <summary>
    /// 使用当前包装器独占的 <paramref name="converter"/> 初始化.
    /// </summary>
    /// <param name="converter">不可与其他包装器共享的核心转换器实例.</param>
    /// <exception cref="ArgumentNullException"><paramref name="converter"/> 为 <see langword="null"/>.</exception>
    protected ConverterBase(Core.CommonConverterBase converter)
    {
        ArgumentNullException.ThrowIfNull(converter);
        CoreConverter = converter;
        _originalDefaultResult = converter.GetDefaultResult;
        converter.GetDefaultResult = () => ConverterResultMapping.ToCore(DefaultResult);
    }

    /// <summary>
    /// 当前包装器拥有的 <see cref="Core.CommonConverterBase"/> 实例.
    /// </summary>
    protected Core.CommonConverterBase CoreConverter { get; }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<object?> DefaultResultProperty =
        CommonDependencyProperty.Register<ConverterBase, object?>(
            nameof(DefaultResult),
            defaultValue: UnconfiguredResult
        );

    /// <summary>
    /// 转换失败时由 <see cref="Core.CommonConverterBase.GetDefaultResult"/> 读取的结果, 支持显式 <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// 未配置时调用核心实例原有的默认结果委托. 清除本地值后恢复较低优先级配置, 没有配置时恢复核心默认值.
    /// 此 CLR 属性映射平台哨兵与 <see cref="Core.ConverterResults"/>, 强类型原生访问方法不进行此映射.
    /// </remarks>
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

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<Core.PreferredCulture> PreferredCultureProperty =
        CommonDependencyProperty.Register<ConverterBase, Core.PreferredCulture>(
            nameof(PreferredCulture),
            defaultValue: UnconfiguredCulture
        );

    /// <summary>
    /// 同步到 <see cref="Core.CommonConverterBase.PreferredCulture"/> 的实例文化偏好.
    /// </summary>
    /// <remarks>
    /// 未配置时动态读取 <see cref="Core.ValueConvertersConfig.DefaultPreferredCulture"/>, 显式枚举零值仍是有效覆盖.
    /// 清除本地值后恢复较低优先级配置, 没有配置时恢复全局偏好; 最终文化选择由核心入口完成.
    /// </remarks>
    public Core.PreferredCulture PreferredCulture
    {
        get
        {
            var value = GetValue(PreferredCultureProperty)!;
            return value == UnconfiguredCulture
                ? Core.ValueConvertersConfig.DefaultPreferredCulture
                : value;
        }
        set => SetValue(PreferredCultureProperty, value);
    }

    /// <summary>
    /// 读取 <paramref name="property"/> 的有效原生值, 不映射转换器哨兵.
    /// </summary>
    /// <typeparam name="T">属性值类型.</typeparam>
    /// <param name="property">包含值类型为 <typeparamref name="T"/> 的 <see cref="StyledProperty{TValue}"/> 的共享句柄.</param>
    /// <returns>Avalonia 属性系统选定的有效值.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">句柄中不包含值类型为 <typeparamref name="T"/> 的 <see cref="StyledProperty{TValue}"/>.</exception>
    public T GetValue<T>(Core.CommonDependencyProperty<T> property) =>
        base.GetValue(Unwrap(property));

    /// <summary>
    /// 按 <paramref name="priority"/> 指定的优先级写入原生值, 保留 Avalonia 哨兵及值来源的语义.
    /// </summary>
    /// <typeparam name="T">属性值类型.</typeparam>
    /// <param name="property">包含值类型为 <typeparamref name="T"/> 的 <see cref="StyledProperty{TValue}"/> 的共享句柄.</param>
    /// <param name="value">待赋值的原生值, 不进行转换器哨兵映射.</param>
    /// <param name="priority">赋值优先级, 默认为 <see cref="global::Avalonia.Data.BindingPriority.LocalValue"/>.</param>
    /// <returns>释放时移除该赋值的原生句柄, 未提供可释放句柄时返回 <see langword="null"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">句柄中不包含值类型为 <typeparamref name="T"/> 的 <see cref="StyledProperty{TValue}"/>.</exception>
    public IDisposable? SetValue<T>(
        Core.CommonDependencyProperty<T> property,
        T value,
        global::Avalonia.Data.BindingPriority priority =
            global::Avalonia.Data.BindingPriority.LocalValue
    ) => base.SetValue(Unwrap(property), value, priority);

    /// <summary>
    /// 清除 <paramref name="property"/> 的本地值, 恢复较低优先级的值或元数据默认值.
    /// </summary>
    /// <typeparam name="T">属性值类型.</typeparam>
    /// <param name="property">包含值类型为 <typeparamref name="T"/> 的 <see cref="StyledProperty{TValue}"/> 的共享句柄.</param>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> 为 <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">句柄中不包含值类型为 <typeparamref name="T"/> 的 <see cref="StyledProperty{TValue}"/>.</exception>
    public void ClearValue<T>(Core.CommonDependencyProperty<T> property) =>
        base.ClearValue(Unwrap(property));

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

    private static StyledProperty<T> Unwrap<T>(Core.CommonDependencyProperty<T> property)
    {
        ArgumentNullException.ThrowIfNull(property);
        return property.Value as StyledProperty<T>
            ?? throw new ArgumentException(
                "The handle must contain an Avalonia StyledProperty with the matching value type.",
                nameof(property)
            );
    }

    /// <summary>
    /// 检查平台线程访问并同步 <see cref="PreferredCulture"/> 到 <see cref="CoreConverter"/>; 文化选择仍由核心入口完成.
    /// </summary>
    protected void PrepareConversion()
    {
        VerifyAccess();
        CoreConverter.PreferredCulture = PreferredCulture;
    }
}
