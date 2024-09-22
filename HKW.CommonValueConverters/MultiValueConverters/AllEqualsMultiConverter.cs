using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace HKW.CommonValueConverters;

/// <summary>
/// 全部相等于值转换器
/// </summary>
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
            return values.All(x => x?.ToString() == target?.ToString() ^ isInverted);
        return values.All(x => x?.Equals(target) is true ^ isInverted);
    }
}
