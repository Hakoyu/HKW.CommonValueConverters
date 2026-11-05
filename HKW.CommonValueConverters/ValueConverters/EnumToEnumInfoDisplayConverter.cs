using System.Globalization;
using HKW.HKWUtils;
using HKW.HKWUtils.Extensions;

namespace HKW.CommonValueConverters;

/// <summary>
/// 枚举到枚举信息转换器
/// </summary>
public class EnumToEnumInfoDisplayConverter : ValueConverterBase
{
    /// <summary>
    /// 默认枚举信息目标
    /// </summary>
    public const EnumInfoDisplayTarget DefaultEnumInfoDisplayTarget = EnumInfoDisplayTarget.Name;

    /// <summary>
    /// 枚举信息目标
    /// </summary>
    public Func<EnumInfoDisplayTarget> GetEnumInfoDisplayTarget { get; set; } =
        () => EnumInfoDisplayTarget.Name;

    /// <inheritdoc/>
    protected override object? ConvertCore(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (value is not Enum @enum)
            return GetDefaultResult();
        return @enum.GetDisplayInfo(GetEnumInfoDisplayTarget());
    }
}
