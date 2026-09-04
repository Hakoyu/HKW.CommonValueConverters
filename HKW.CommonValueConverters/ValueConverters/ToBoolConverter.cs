using System.Globalization;
using HKW.CommonValueConverters;

namespace HKW.CommonValueConverters;

/// <summary>
/// 字符串到布尔转换器
/// </summary>
public class ToBoolConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public override object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        return ConverterUtils.GetBool(value);
    }
}
