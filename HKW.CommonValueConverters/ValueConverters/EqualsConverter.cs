using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 值相等转换器
/// </summary>
public class EqualsConverter : InvertibleValueConverterBase
{
    /// <summary>
    /// 比较值
    /// </summary>
    public Func<object?> GetOther { get; set; } = () => default;

    /// <summary>
    /// 是字符串比较
    /// </summary>
    public Func<bool> GetIsStringEquals { get; set; } = () => default;

    /// <inheritdoc/>
    public override object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        var isInverted = GetIsInverted();
        var target = parameter is null ? GetOther() : parameter;
        if (GetIsStringEquals())
            return (value?.ToString() == target?.ToString()) ^ isInverted;
        if (value is null)
            return (target is null) ^ isInverted;
        else
            return value.Equals(target) ^ isInverted;
    }
}
