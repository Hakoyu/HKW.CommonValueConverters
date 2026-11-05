using System.Globalization;
using HKW.HKWUtils;
using HKW.HKWUtils.Extensions;

namespace HKW.CommonValueConverters;

/// <summary>
/// 枚举到枚举信息转换器
/// </summary>
public class EnumToEnumInfoConverter : ValueConverterBase
{
    /// <inheritdoc/>
    protected override object? ConvertCore(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (value is not Enum @enum)
        {
            return GetDefaultResult();
        }
        return @enum.GetInfo();
    }

    /// <inheritdoc/>
    protected override object? ConvertBackCore(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (value is not IEnumInfo @enum)
        {
            return GetDefaultResult();
        }
        return @enum.Value;
    }
}
