using System.Globalization;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// 基于 <see cref="Core.ValueConverterBase"/> 的平台单值转换器, 统一映射输入, 参数与结果.
/// </summary>
public abstract class ValueConverterBase
    : ConverterBase,
        global::Avalonia.Data.Converters.IValueConverter
{
    private readonly Core.ValueConverterBase _converter;

    /// <summary>
    /// 创建持有独立 <paramref name="converter"/> 实例的包装器.
    /// </summary>
    /// <param name="converter">当前包装器独占的 <see cref="Core.ValueConverterBase"/> 实例.</param>
    /// <exception cref="ArgumentNullException"><paramref name="converter"/> 为 <see langword="null"/>.</exception>
    protected ValueConverterBase(Core.ValueConverterBase converter)
        : base(converter) => _converter = converter;

    /// <summary>
    /// 通过 <see cref="Core.ValueConverterBase.Convert"/> 执行正向转换, 并映射平台与核心哨兵.
    /// </summary>
    /// <param name="value">待转换的输入值.</param>
    /// <param name="targetType">转换结果的目标类型.</param>
    /// <param name="parameter">核心转换器使用的可选转换参数.</param>
    /// <param name="culture">提供给核心文化选择策略的文化信息.</param>
    /// <returns>映射为平台哨兵后的核心转换结果.</returns>
    public object? Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
    {
        PrepareConversion();
        return ConverterResultMapping.FromCore(
            _converter.Convert(
                ConverterResultMapping.ToCore(value),
                targetType,
                ConverterResultMapping.ToCore(parameter),
                culture
            )
        );
    }

    /// <summary>
    /// 通过 <see cref="Core.ValueConverterBase.ConvertBack"/> 执行反向转换; 未实现时抛出 <see cref="NotSupportedException"/>.
    /// </summary>
    /// <param name="value">待转换的输入值.</param>
    /// <param name="targetType">转换结果的目标类型.</param>
    /// <param name="parameter">核心转换器使用的可选转换参数.</param>
    /// <param name="culture">提供给核心文化选择策略的文化信息.</param>
    /// <returns>映射为平台哨兵后的核心转换结果.</returns>
    public object? ConvertBack(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        PrepareConversion();
        return ConverterResultMapping.FromCore(
            _converter.ConvertBack(
                ConverterResultMapping.ToCore(value),
                targetType,
                ConverterResultMapping.ToCore(parameter),
                culture
            )
        );
    }
}
