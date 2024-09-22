using System;
using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 版本到字符串转换器
/// <example>
/// [1] Major Version
/// [2] Minor Version
/// [3] Build Number
/// [4] Revision
/// </example>
/// </summary>
public class VersionToStringConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public override object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        if (value is not Version version)
            return null;
        if (int.TryParse(parameter as string, out int fieldCount))
        {
            return version.ToString(fieldCount);
        }
        return version.ToString();
    }
}
