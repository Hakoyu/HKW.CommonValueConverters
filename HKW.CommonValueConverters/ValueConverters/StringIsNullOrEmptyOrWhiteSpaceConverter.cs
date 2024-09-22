using System;
using System.Globalization;
using System.Windows;

namespace HKW.CommonValueConverters;

/// <summary>
/// 字符串是null或者空转换器
/// </summary>
public class StringIsNullOrEmptyOrWhiteSpaceConverter : InvertibleValueConverterBase
{
    /// <summary>
    /// 字符串检查类型
    /// </summary>
    public Func<StringCheckType> GetStringCheckType { get; set; } = () => StringCheckType.Null;

    /// <inheritdoc/>
    public override object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        var isInverted = GetIsInverted();
        if (Enum.TryParse<StringCheckType>(parameter?.ToString(), out var checkType) is false)
            checkType = GetStringCheckType();
        if (checkType is StringCheckType.Null)
            return value is null ^ isInverted;
        else if (checkType is StringCheckType.NullOrEmpty)
            return string.IsNullOrEmpty(value?.ToString()) ^ isInverted;
        else if (checkType is StringCheckType.NullOrWhiteSpace)
            return string.IsNullOrWhiteSpace(value?.ToString()) ^ isInverted;
        else
            return GetDefaultResult();
    }
}
