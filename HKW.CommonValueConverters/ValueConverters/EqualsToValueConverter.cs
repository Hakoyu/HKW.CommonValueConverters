using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 相等到值转换器
/// </summary>
/// <typeparam name="T">值类型</typeparam>
public class EqualsToValueConverter<T> : ValueConverterBase
{
    /// <summary>
    /// 比较值
    /// </summary>
    public Func<object?> GetOther { get; set; } = () => default;

    /// <summary>
    /// 真值
    /// </summary>
    public Func<T> GetTrueValue { get; set; } = () => default!;

    /// <summary>
    /// 假值
    /// </summary>
    public Func<T> GetFalseValue { get; set; } = () => default!;

    /// <summary>
    /// 空值
    /// </summary>
    public Func<T> GetNullValue { get; set; } = () => default!;

    /// <summary>
    /// 是字符串比较
    /// </summary>
    public Func<bool> GetIsStringEquals { get; set; } = () => default!;

    /// <inheritdoc/>
    public override object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        var target = parameter ?? GetOther();
        if (value is null)
            return GetNullValue();
        if (GetIsStringEquals())
            return value.ToString() == target?.ToString() ? GetTrueValue() : GetFalseValue();
        return value.Equals(target) ? GetTrueValue() : GetFalseValue();
    }
}
