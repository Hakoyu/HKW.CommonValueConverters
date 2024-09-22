using System;
using System.Diagnostics;
using System.Globalization;
using HKW.CommonValueConverters;

namespace HKW.CommonValueConverters;

/// <summary>
/// 调试转换器
/// </summary>
public class DebugConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public override object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        Debug.WriteLine(
            $"DebugConverter.Convert(value={value}, targetType={targetType}, parameter={parameter}, culture={culture})"
        );

        return value;
    }

    /// <inheritdoc/>
    public override object? ConvertBack(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        Debug.WriteLine(
            $"DebugConverter.ConvertBack(value={value}, targetType={targetType}, parameter={parameter}, culture={culture})"
        );

        return value;
    }
}
