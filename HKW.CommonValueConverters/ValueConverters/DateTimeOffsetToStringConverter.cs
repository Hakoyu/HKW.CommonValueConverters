using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 日期时间偏移到字符串转换器
/// </summary>
public class DateTimeOffsetToStringConverter : ValueConverterBase
{
    /// <summary>
    /// 默认格式化
    /// </summary>
    public const string DefaultFormat = "g";

    private readonly ITimeZoneInfo _timeZone;

    /// <inheritdoc/>
    public DateTimeOffsetToStringConverter()
        : this(SystemTimeZoneInfo.Current) { }

    internal DateTimeOffsetToStringConverter(ITimeZoneInfo timeZone)
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

    /// <inheritdoc/>
    public override object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        if (value is not DateTimeOffset dateTimeOffset)
            return GetDefaultResult();

        var format = parameter as string ?? GetFormat();
        return TimeZoneInfo.ConvertTime(dateTimeOffset, _timeZone.Local).ToString(format, culture);
    }

    /// <inheritdoc/>
    public override object? ConvertBack(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        if (value is string str && DateTimeOffset.TryParse(str, out var parsedDateTimeOffset))
        {
            return TimeZoneInfo.ConvertTime(parsedDateTimeOffset, _timeZone.Utc);
        }

        return GetDefaultResult();
    }
}
