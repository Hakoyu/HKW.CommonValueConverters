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
    protected override object? ConvertCore(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (value is not DateTimeOffset dateTimeOffset || ConverterUtils.IsUnavailable(parameter))
            return GetDefaultResult();

        var format = parameter as string ?? GetFormat();
        var local = TimeZoneInfo.ConvertTime(dateTimeOffset, _timeZone.Local);
        try
        {
            return local.ToString(format, culture);
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
        DateTimeOffset parsedDateTimeOffset;
        bool parsed;
        try
        {
            parsed =
                DateTimeOffset.TryParseExact(
                    str,
                    format,
                    culture,
                    DateTimeStyles.None,
                    out parsedDateTimeOffset
                )
                || DateTimeOffset.TryParse(
                    str,
                    culture,
                    DateTimeStyles.None,
                    out parsedDateTimeOffset
                );
        }
        catch (FormatException) when (culture.GetType() == typeof(CultureInfo))
        {
            return GetDefaultResult();
        }
        if (!parsed)
            return GetDefaultResult();
        return TimeZoneInfo.ConvertTime(parsedDateTimeOffset, _timeZone.Utc);
    }
}
