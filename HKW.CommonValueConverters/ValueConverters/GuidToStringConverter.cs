using System;
using System.Globalization;
using System.Windows;

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
    /// 转换为大写
    /// </summary>
    public Func<bool> GetToUpper { get; set; } = () => false;

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
        var defultResult = GetDefaultResult();
        if (value is Guid guid)
        {
            var format = parameter as string ?? GetFormat();
            var guidString = guid.ToString(format);

            if (GetToUpper())
                return guidString.ToUpperInvariant();

            return guidString;
        }

        return defultResult;
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
