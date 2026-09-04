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
    protected const string DefaultFormat = "D";

    /// <summary>
    /// 字符串改变, true 到大写, false 到小写, null 不变
    /// </summary>
    public Func<bool?> GetStringTo { get; set; } = () => null;

    /// <summary>
    /// 格式化
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
        if (value is not Guid guid)
            return GetDefaultResult();

        var format = parameter as string ?? GetFormat();
        var guidString = guid.ToString(format);

        var stringTo = GetStringTo();
        if (stringTo is true)
            return guidString.ToUpperInvariant();
        else if (stringTo is false)
            return guidString.ToLowerInvariant();

        return guidString;
    }

    /// <inheritdoc/>
    public override object? ConvertBack(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        var defultResult = GetDefaultResult();
        if (value is string guidString)
        {
            var guid = new Guid(guidString);
            return guid;
        }

        return defultResult;
    }
}
