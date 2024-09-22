using System.Globalization;
using System.Windows;
using HKW.HKWUtils;

namespace HKW.CommonValueConverters;

/// <summary>
/// 枚举到枚举信息转换器
/// </summary>
public class EnumToEnumInfoConverter : ValueConverterBase
{
    /// <summary>
    /// 枚举信息目标
    /// </summary>
    public Func<EnumInfoTarget> GetEnumInfoTarget { get; set; } = () => EnumInfoTarget.Name;

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
        return EnumInfo.GetEnumInfo(type, @enum)?.GetInfo(GetEnumInfoTarget());
    }
}
