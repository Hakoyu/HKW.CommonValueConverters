using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 日期时间到字符串转换器
/// </summary>
public class DateTimeToStringConverter : ValueConverterBase
{
    /// <summary>
    /// 默认格式化
    /// </summary>
    public const string DefaultFormat = "g";

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

    /// <inheritdoc/>
    protected override object? ConvertCore(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (value is not DateTime dateTime || ConverterUtils.IsUnavailable(parameter))
            return GetDefaultResult();

        var format = parameter as string ?? GetFormat();
        var zone = _timeZone.Local;
        DateTime localDateTime;
        try
        {
            localDateTime = TimeZoneInfo.ConvertTime(dateTime, zone);
        }
        catch (ArgumentException)
        {
            return GetDefaultResult();
        }
        try
        {
            return localDateTime.ToString(format, culture);
        }
        catch (FormatException) when (culture.GetType() == typeof(CultureInfo))
        {
            return GetDefaultResult();
        }
    }

    /// <inheritdoc/>
    protected override object? ConvertBackCore(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (value is not string str || ConverterUtils.IsUnavailable(parameter))
            return GetDefaultResult();
        var format = parameter as string ?? GetFormat();
        DateTime parsedDateTime;
        bool parsed;
        try
        {
            parsed =
                DateTime.TryParseExact(
                    str,
                    format,
                    culture,
                    DateTimeStyles.RoundtripKind,
                    out parsedDateTime
                )
                || DateTime.TryParse(
                    str,
                    culture,
                    DateTimeStyles.RoundtripKind,
                    out parsedDateTime
                );
        }
        catch (FormatException) when (culture.GetType() == typeof(CultureInfo))
        {
            return GetDefaultResult();
        }
        if (!parsed)
            return GetDefaultResult();
        var zone = _timeZone.Utc;
        try
        {
            return TimeZoneInfo.ConvertTime(parsedDateTime, zone);
        }
        catch (ArgumentException)
        {
            return GetDefaultResult();
        }
    }
}
