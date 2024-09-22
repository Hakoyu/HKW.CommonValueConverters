using System;
using System.Globalization;
using System.Windows;
using HKW.CommonValueConverters;

namespace HKW.CommonValueConverters;

/// <summary>
/// 日期时间到字符串转换器
/// </summary>
public class DateTimeToStringConverter : ValueConverterBase
{
    /// <summary>
    /// 默认格式化
    /// </summary>
    protected const string DefaultFormat = "g";

    /// <summary>
    /// 默认最小值
    /// </summary>
    protected const string DefaultMinValueString = "";

    private readonly ITimeZoneInfo _timeZone;

    /// <inheritdoc/>
    public DateTimeToStringConverter()
        : this(SystemTimeZoneInfo.Current) { }

    internal DateTimeToStringConverter(ITimeZoneInfo timeZone)
    {
        _timeZone = timeZone;
    }

    /// <summary>
    /// 日期时间格式化
    /// <para>
    /// 格式化参考: https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings
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
        if (value is DateTime dateTime)
        {
            var format = parameter as string ?? GetFormat();
            if (dateTime == DateTime.MinValue)
            {
                return GetMinValueString;
            }

            var localDateTime = TimeZoneInfo.ConvertTime(dateTime, _timeZone.Local);
            return localDateTime.ToString(format, culture);
        }

        return GetDefaultResult();
    }

    /// <inheritdoc/>
    public override object? ConvertBack(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        if (value is string str && DateTime.TryParse(str, out var parsedDateTime))
        {
            return TimeZoneInfo.ConvertTime(parsedDateTime, _timeZone.Utc);
        }

        return GetDefaultResult();
    }
}
