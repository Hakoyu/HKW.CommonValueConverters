using System;
using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 枚举到布尔转换器
/// </summary>
public class EnumEqualsConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public EnumEqualsConverter()
    {
        GetDefaultResult = () => false;
    }

    /// <inheritdoc/>
    public override object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        var defultResult = GetDefaultResult();
        if (value is null)
            return defultResult;
        if (parameter is string str)
        {
            var enumType = value.GetType();
            if (Enum.TryParse(enumType, str, out var parameterValue) is false)
                return defultResult;

            return parameterValue?.Equals(value) is true;
        }
        else if (parameter is Enum @enum)
        {
            return value.Equals(@enum);
        }

        return defultResult;
    }
}
