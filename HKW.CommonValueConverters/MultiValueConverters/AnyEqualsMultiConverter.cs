using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 任意相等于值转换器
/// </summary>
/// <typeparam name="T">值类型</typeparam>
public class AnyEqualsMultiConverter<T> : InvertibleMultiValueConverterBase
{
    /// <summary>
    /// 默认值
    /// </summary>
    public Func<T> GetValue { get; set; } = () => default!;

    /// <summary>
    /// 是字符串比较
    /// </summary>
    public Func<bool> GetIsStringEquals { get; set; } = () => default!;

    /// <inheritdoc/>
    protected override object? ConvertCore(
        IList<object?> values,
        Type? targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        var isInverted = GetIsInverted();
        if (ConverterUtils.HasUnavailable(values) || ConverterUtils.IsUnavailable(parameter))
            return GetDefaultResult();
        var target = parameter is T t ? t : GetValue();
        if (ConverterUtils.IsUnavailable(target))
            return GetDefaultResult();
        if (GetIsStringEquals())
            return values.Any(o => o?.ToString() == target?.ToString()) ^ isInverted;
        return values.Any(o => Equals(o, target)) ^ isInverted;
    }
}
