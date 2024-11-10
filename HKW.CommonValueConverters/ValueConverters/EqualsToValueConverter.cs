using System.Globalization;
using System.Windows;
using HKW.CommonValueConverters;

namespace HKW.CommonValueConverters;

/// <summary>
/// 相等到值转换器
/// </summary>
/// <typeparam name="T">值类型</typeparam>

public class EqualsToValueConverter<T> : InvertibleValueConverterBase
{
    /// <summary>
    /// 目标值
    /// </summary>
    public Func<object?> GetTargetValue { get; set; } = () => default;

    /// <summary>
    /// 真值
    /// </summary>
    public Func<T> GetTrueValue { get; set; } = () => default!;

    /// <summary>
    /// 假值
    /// </summary>
    public Func<T> GetFalseValue { get; set; } = () => default!;

    /// <summary>
    /// 是可为空的
    /// </summary>
    public Func<bool> GetIsNullable { get; set; } = () => false;

    /// <summary>
    /// 空值
    /// <para>只有 <see cref="GetIsNullable"/> 返回 <see langword="true"/> 时, 才对value进行判断并返回此方法的结果</para>
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
        var target = parameter ?? GetTargetValue();
        var isInverted = GetIsInverted();
        if (GetIsNullable() && value is null ^ isInverted)
            return GetNullValue();
        if (GetIsStringEquals())
            return value?.ToString() == parameter?.ToString() ^ isInverted
                ? GetTrueValue()
                : GetFalseValue();
        return value?.Equals(target) is true ^ isInverted ? GetTrueValue() : GetFalseValue();
    }
}
