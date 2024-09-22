using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace HKW.CommonValueConverters;

/// <summary>
/// 全部为布尔值到值转换器
/// </summary>
public class AllBoolToValueMultiConverter<T> : InvertibleMultiValueConverterBase
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
        IList<object?> values,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        var isInverted = GetIsInverted();
        var trueCount = 0;
        var falseCount = 0;
        var nullCount = 0;
        for (var i = 0; i < values.Count; i++)
        {
            var value = values[i];
            if (value is null)
                nullCount++;
            else if (value is true)
                trueCount++;
            else if (value is false)
                falseCount++;
        }
        if (trueCount == values.Count ^ isInverted)
            return GetTrueValue();
        else if (falseCount == values.Count ^ isInverted)
            return GetFalseValue();
        else if (nullCount == values.Count ^ isInverted)
            return GetNullValue();
        else
            return GetDefaultResult();
    }
}
