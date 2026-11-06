using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 全部相等于值转换器
/// </summary>
/// <typeparam name="T">配置值的类型.</typeparam>
public class AllEqualsMultiConverter<T> : InvertibleMultiValueConverterBase
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
            return values.All(x => x?.ToString() == target?.ToString()) ^ isInverted;
        return values.All(x => Equals(x, target)) ^ isInverted;
    }
}
