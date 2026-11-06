using System.Globalization;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.MultiValueConverterBase"/> 的平台多值转换器基类, 不修改调用方的输入集合.
/// </summary>
public abstract class MultiValueConverterBase
    : ConverterBase,
        System.Windows.Data.IMultiValueConverter
{
    private readonly Core.MultiValueConverterBase _converter;

    /// <summary>
    /// 创建持有独立 <see cref="Core.MultiValueConverterBase"/> 核心实例的包装器.
    /// </summary>
    /// <param name="converter">仅由当前包装器使用的核心转换器.</param>
    /// <exception cref="ArgumentNullException"><paramref name="converter"/> 为 <see langword="null"/>.</exception>
    protected MultiValueConverterBase(Core.MultiValueConverterBase converter)
        : base(converter) => _converter = converter;

    /// <summary>
    /// 按 <see cref="Core.MultiValueConverterBase.Convert"/> 的语义执行多值转换, 只检查该核心实际需要的输入.
    /// </summary>
    /// <param name="values">输入数组, 不修改原数组, 平台哨兵映射后传递给核心.</param>
    /// <param name="targetType">目标类型, 可以为 <see langword="null"/>.</param>
    /// <param name="parameter">传递给核心的转换参数.</param>
    /// <param name="culture">调用方提供的文化, 最终选择由 <see cref="ConverterBase.PreferredCulture"/> 决定.</param>
    /// <returns>转换结果, 核心哨兵映射为 WPF 哨兵.</returns>
    public object? Convert(
        object?[] values,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        PrepareConversion();
        return ConverterResultMapping.FromCore(
            _converter.Convert(
                ConverterResultMapping.ToCoreValues(values),
                targetType,
                ConverterResultMapping.ToCore(parameter),
                culture
            )
        );
    }

    /// <summary>
    /// 执行 <see cref="Core.MultiValueConverterBase.ConvertBack"/> 并逐项映射结果; 内置多值核心默认不支持.
    /// </summary>
    /// <param name="value">反向转换输入, 平台哨兵在调用核心前转换为核心哨兵.</param>
    /// <param name="targetTypes">传递给核心的目标类型数组.</param>
    /// <param name="parameter">传递给核心的转换参数.</param>
    /// <param name="culture">调用方提供的文化, 最终选择由 <see cref="ConverterBase.PreferredCulture"/> 决定.</param>
    /// <returns>逐项映射核心哨兵为 WPF 哨兵后的结果数组.</returns>
    /// <exception cref="NotSupportedException">核心转换器未实现反向转换.</exception>
    public object[] ConvertBack(
        object? value,
        Type[] targetTypes,
        object? parameter,
        CultureInfo? culture
    )
    {
        PrepareConversion();
        return _converter
            .ConvertBack(
                ConverterResultMapping.ToCore(value),
                targetTypes,
                ConverterResultMapping.ToCore(parameter),
                culture
            )
            .Select(v => ConverterResultMapping.FromCore(v)!)
            .ToArray();
    }
}
