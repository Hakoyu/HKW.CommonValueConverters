using System.Collections;
using System.Globalization;
using HKW.HKWUtils.Extensions;

namespace HKW.CommonValueConverters;

/// <summary>
/// 枚举到枚举信息转换器
/// </summary>
public class EnumsToEnumInfosConverter : ValueConverterBase
{
    /// <summary>
    /// 只显示有效值
    /// </summary>
    public Func<bool> GetOnlyValid { get; set; } = () => false;

    /// <inheritdoc/>
    protected override object? ConvertCore(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (value is not IEnumerable enums || enums.Cast<object?>().FirstOrDefault() is not Enum @enum)
            return GetDefaultResult();
        if (GetOnlyValid())
            return @enum.GetInfo().ValidInfoDictionary.Values;
        return @enum.GetInfo().InfoDictionary.Values;
    }
}
