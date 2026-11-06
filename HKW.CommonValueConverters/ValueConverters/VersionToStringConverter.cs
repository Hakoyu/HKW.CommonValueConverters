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
    protected override object? ConvertCore(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (value is not Version version || ConverterUtils.IsUnavailable(parameter))
            return GetDefaultResult();
        if (int.TryParse(parameter as string, NumberStyles.Integer, culture, out int fieldCount))
        {
            int availableFields;
            if (version.Revision >= 0)
            {
                availableFields = 4;
            }
            else
            {
                availableFields = version.Build >= 0 ? 3 : 2;
            }

            if (fieldCount < 0 || fieldCount > availableFields)
                return GetDefaultResult();
            return version.ToString(fieldCount);
        }
        return version.ToString();
    }
}
