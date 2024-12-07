using System.Collections;
using System.Globalization;
using HKW.HKWUtils;
using HKW.HKWUtils.Extensions;

namespace HKW.CommonValueConverters;

/// <summary>
/// 枚举到枚举信息转换器
/// </summary>
public class EnumsToEnumInfosConverter : ValueConverterBase
{
    /// <summary>
    /// 只显示有效值, 默认为 <see langword="true"/>
    /// </summary>
    public Func<bool> GetOnlyValid { get; set; } = () => true;

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
            return defultResult;
        var @enum = enums.Cast<Enum>().First();
        if (GetOnlyValid())
            return @enum.GetInfo().ValidInfos.Values;
        return @enum.GetInfo().Infos.Values;
    }
}
