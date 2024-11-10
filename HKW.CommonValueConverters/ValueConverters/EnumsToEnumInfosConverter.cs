using System.Collections;
using System.Globalization;
using HKW.HKWUtils;

namespace HKW.CommonValueConverters;

/// <summary>
/// 枚举到枚举信息转换器
/// </summary>
public class EnumsToEnumInfosConverter : ValueConverterBase
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
        if (value is not IEnumerable enums)
        {
            ArgumentNullException.ThrowIfNull(value);
            return defultResult;
        }
        var type = enums.Cast<Enum>().First().GetType();
        return EnumInfo.GetEnumInfo(type)!.Infos.Values;
    }
}
