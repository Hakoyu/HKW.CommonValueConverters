using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// Guid到字符串转换器
/// </summary>
public class GuidToStringConverter : ValueConverterBase
{
    /// <summary>
    /// 默认格式化
    /// </summary>
    public const string DefaultFormat = "D";

    /// <summary>
    /// 字符串改变, true 到大写, false 到小写, null 不变
    /// </summary>
    public Func<bool?> GetStringTo { get; set; } = () => null;

    /// <summary>
    /// 格式化
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
        if (value is not Guid guid || ConverterUtils.IsUnavailable(parameter))
            return GetDefaultResult();

        var format = parameter as string ?? GetFormat();
        if (!string.IsNullOrEmpty(format) && (format.Length != 1 || !"DdNnBbPpXx".Contains(format[0])))
            return GetDefaultResult();
        var guidString = guid.ToString(format);

        var stringTo = GetStringTo();
        if (stringTo is true)
            return guidString.ToUpperInvariant();
        else if (stringTo is false)
            return guidString.ToLowerInvariant();

        return guidString;
    }

    /// <inheritdoc/>
    protected override object? ConvertBackCore(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        return value is string guidString && Guid.TryParse(guidString, out var guid)
            ? guid : GetDefaultResult();
    }
}
