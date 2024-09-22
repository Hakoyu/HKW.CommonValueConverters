using System.Globalization;
using System.Windows;

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
            return values.Any(o => o?.ToString() == target?.ToString() ^ isInverted);
        else
            return values.Any(o => o?.Equals(target) is true ^ isInverted);
    }
}
