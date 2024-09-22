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
    /// 为空时的值
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
        if (value is null ^ GetIsInverted())
            return GetNullValue();
        return ConverterUtils.GetBool(value) ^ GetIsInverted() ? GetTrueValue : GetFalseValue();
    }
}
