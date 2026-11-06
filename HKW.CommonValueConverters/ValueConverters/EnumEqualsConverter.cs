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
    protected override object? ConvertCore(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (value is not Enum || ConverterUtils.IsUnavailable(parameter))
            return GetDefaultResult();
        if (parameter is string str)
        {
            if (Enum.TryParse(value.GetType(), str, out var parameterValue) is false)
                return GetDefaultResult();
            return Equals(parameterValue, value);
        }
        if (parameter is Enum @enum)
            return value.Equals(@enum);
        return GetDefaultResult();
    }
}
