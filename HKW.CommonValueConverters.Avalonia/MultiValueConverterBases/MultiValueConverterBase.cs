using System.Globalization;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// 基于 <see cref="Core.MultiValueConverterBase"/> 的平台多值转换器, 不修改调用方的输入集合.
/// </summary>
public abstract class MultiValueConverterBase
    : ConverterBase,
        global::Avalonia.Data.Converters.IMultiValueConverter
{
    private readonly Core.MultiValueConverterBase _converter;

    /// <summary>
    /// 创建持有独立 <paramref name="converter"/> 实例的包装器.
    /// </summary>
    /// <param name="converter">当前包装器独占的 <see cref="Core.MultiValueConverterBase"/> 实例.</param>
    /// <exception cref="ArgumentNullException"><paramref name="converter"/> 为 <see langword="null"/>.</exception>
    protected MultiValueConverterBase(Core.MultiValueConverterBase converter)
        : base(converter) => _converter = converter;

    /// <summary>
    /// 通过 <see cref="Core.MultiValueConverterBase.Convert"/> 执行多值转换, 只检查该核心实际需要的输入.
    /// </summary>
    /// <param name="values">待转换的输入集合, 不修改调用方的集合.</param>
    /// <param name="targetType">转换结果的目标类型.</param>
    /// <param name="parameter">核心转换器使用的可选转换参数.</param>
    /// <param name="culture">提供给核心文化选择策略的文化信息.</param>
    /// <returns>映射为平台哨兵后的核心转换结果.</returns>
    public object? Convert(
        IList<object?> values,
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
}
