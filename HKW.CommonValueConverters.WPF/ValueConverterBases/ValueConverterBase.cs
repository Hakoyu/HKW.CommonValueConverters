using System.Globalization;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.ValueConverterBase"/> 的平台单值转换器基类, 统一映射输入, 参数与结果.
/// </summary>
public abstract class ValueConverterBase : ConverterBase, System.Windows.Data.IValueConverter
{
    private readonly Core.ValueConverterBase _converter;

    /// <summary>
    /// 创建持有独立 <see cref="Core.ValueConverterBase"/> 核心实例的包装器.
    /// </summary>
    /// <param name="converter">仅由当前包装器使用的核心转换器.</param>
    /// <exception cref="ArgumentNullException"><paramref name="converter"/> 为 <see langword="null"/>.</exception>
    protected ValueConverterBase(Core.ValueConverterBase converter)
        : base(converter) => _converter = converter;

    /// <summary>
    /// 按 <see cref="Core.ValueConverterBase.Convert"/> 的核心语义执行正向转换.
    /// </summary>
    /// <param name="value">输入值, 平台哨兵在调用核心前转换为核心哨兵.</param>
    /// <param name="targetType">目标类型, 可以为 <see langword="null"/>.</param>
    /// <param name="parameter">传递给核心的转换参数.</param>
    /// <param name="culture">调用方提供的文化, 最终选择由 <see cref="ConverterBase.PreferredCulture"/> 决定.</param>
    /// <returns>转换结果, 核心哨兵映射为 WPF 哨兵.</returns>
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
    /// 按 <see cref="Core.ValueConverterBase.ConvertBack"/> 的核心语义执行反向转换.
    /// </summary>
    /// <param name="value">反向转换输入, 平台哨兵在调用核心前转换为核心哨兵.</param>
    /// <param name="targetType">目标类型, 可以为 <see langword="null"/>.</param>
    /// <param name="parameter">传递给核心的转换参数.</param>
    /// <param name="culture">调用方提供的文化, 最终选择由 <see cref="ConverterBase.PreferredCulture"/> 决定.</param>
    /// <returns>反向转换结果, 核心哨兵映射为 WPF 哨兵.</returns>
    /// <exception cref="NotSupportedException">核心转换器未实现反向转换.</exception>
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
