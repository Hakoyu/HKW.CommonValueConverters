using System;
using System.Globalization;
using System.Windows;

namespace HKW.CommonValueConverters;

/// <summary>
/// 时间范围到字符串转换器
/// </summary>
public class TimeSpanToStringConverter : ValueConverterBase
{
    /// <summary>
    /// 默认格式化
    /// </summary>
    protected const string DefaultFormat = "g";

    /// <summary>
    /// 默认最小值
    /// </summary>
    protected const string DefaultMinValueString = "";

    /// <summary>
    /// 时间格式化
    /// <para>
    /// 时间格式化参考s: https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-timespan-format-strings
    /// </para>
    /// </summary>
    public Func<string> GetFormat { get; set; } = () => DefaultFormat;

    /// <summary>
    /// 最小值
    /// </summary>
    public Func<string> GetMinValueString { get; set; } = () => DefaultMinValueString;

    /// <inheritdoc/>
    public override object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        if (value is not TimeSpan timeSpan)
            return GetDefaultResult();
        if (timeSpan == TimeSpan.MinValue)
        {
            return GetMinValueString();
        }

        return timeSpan.ToString(GetFormat(), culture);
    }

    /// <inheritdoc/>
    public override object? ConvertBack(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        if (value is TimeSpan timeSpan)
        {
            return timeSpan;
        }

        if (value is string str && TimeSpan.TryParse(str, out var resultTimeSpan))
        {
            return resultTimeSpan;
        }

        return GetDefaultResult();
    }
}
