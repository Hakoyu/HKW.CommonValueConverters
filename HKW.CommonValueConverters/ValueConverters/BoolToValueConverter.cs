using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace HKW.CommonValueConverters;

/// <summary>
/// 布尔到值转换器
/// </summary>
/// <typeparam name="T">值类型</typeparam>
public class BoolToValueConverter<T> : InvertibleValueConverterBase
{
    /// <summary>
    /// 为真时的值
    /// </summary>
    public Func<T> GetTrueValue { get; set; } = () => default!;

    /// <summary>
    /// 为假时的值
    /// </summary>
    public Func<T> GetFalseValue { get; set; } = () => default!;

    /// <summary>
    /// 是可为空的
    /// </summary>
    public Func<bool> GetIsNullable { get; set; } = () => false;

    /// <summary>
    /// 为空时的值
    /// <para>只有 <see cref="GetIsNullable"/> 返回 <see langword="true"/> 时, 才对value进行判断并返回此方法的结果</para>
    /// </summary>
    public Func<T> GetNullValue { get; set; } = () => default!;

    /// <inheritdoc/>
    public override object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        var isInverted = GetIsInverted();
        if (GetIsNullable() && value is null ^ isInverted)
            return GetNullValue();
        return ConverterUtils.GetBool(value) ^ isInverted ? GetTrueValue() : GetFalseValue();
    }
}
