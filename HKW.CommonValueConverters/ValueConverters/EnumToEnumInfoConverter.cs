﻿using System.Globalization;
using System.Windows;
using HKW.HKWUtils;

namespace HKW.CommonValueConverters;

/// <summary>
/// 枚举到枚举信息转换器
/// </summary>
public class EnumToEnumInfoConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public override object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        var defultResult = GetDefaultResult();
        if (value is not Enum @enum)
        {
            ArgumentNullException.ThrowIfNull(value);
            return defultResult;
        }
        var type = value.GetType();
        return EnumInfo.GetEnumInfo(type, @enum);
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
        if (value is not IEnumInfo @enum)
        {
            ArgumentNullException.ThrowIfNull(value);
            return defultResult;
        }
        return @enum.Value;
    }
}
