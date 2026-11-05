using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 时间范围到字符串转换器
/// </summary>
public class TimeSpanToStringConverter : ValueConverterBase
{
    /// <summary>
    /// 默认格式化
    /// </summary>
    public const string DefaultFormat = "g";

    /// <summary>
    /// 时间格式化
    /// <para>
    /// 时间格式化参考s: https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-timespan-format-strings
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
        if (value is not TimeSpan timeSpan || ConverterUtils.IsUnavailable(parameter))
            return GetDefaultResult();
        var format = parameter as string ?? GetFormat();
        try
        {
            return timeSpan.ToString(format, culture);
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
        if (ConverterUtils.IsUnavailable(parameter))
            return GetDefaultResult();
        if (value is TimeSpan timeSpan)
            return timeSpan;
        if (value is not string str)
            return GetDefaultResult();
        var format = parameter as string ?? GetFormat();
        return TimeSpan.TryParseExact(str, format, culture, out var resultTimeSpan)
            || TimeSpan.TryParse(str, culture, out resultTimeSpan)
            ? resultTimeSpan : GetDefaultResult();
    }
}
