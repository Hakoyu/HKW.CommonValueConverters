using System.Globalization;
using HKW.HKWUtils;
using HKW.HKWUtils.Extensions;

namespace HKW.CommonValueConverters;

/// <summary>
/// 枚举到枚举信息转换器
/// </summary>
public class EnumToEnumInfoTargetConverter : ValueConverterBase
{
    /// <summary>
    /// 枚举信息目标
    /// </summary>
    public Func<EnumInfoDisplayTarget> GetEnumInfoDisplayTarget { get; set; } =
        () => EnumInfoDisplayTarget.Name;

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
            return defultResult;
        var type = value.GetType();
        return @enum.GetDisplayInfo(GetEnumInfoDisplayTarget());
    }
}
