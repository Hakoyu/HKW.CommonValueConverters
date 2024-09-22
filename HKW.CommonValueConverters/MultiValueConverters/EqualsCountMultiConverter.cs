using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 相等值的数量转换器
/// </summary>
/// <typeparam name="T">值类型</typeparam>

public class EqualsCountMultiConverter<T> : InvertibleMultiValueConverterBase
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
    public override object? Convert(
        IList<object?> values,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        var isInverted = GetIsInverted();
        var target = parameter is T t ? t : GetValue();
        if (GetIsStringEquals())
            return values.Count(o => o?.ToString() == target?.ToString() ^ isInverted);
        else
            return values.Count(o => o?.Equals(target) is true ^ isInverted);
    }
}
