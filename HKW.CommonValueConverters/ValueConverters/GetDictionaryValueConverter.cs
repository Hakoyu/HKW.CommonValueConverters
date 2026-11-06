using System.Collections;
using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 获取字典值
/// </summary>
public class GetDictionaryValueConverter : ValueConverterBase
{
    /// <inheritdoc/>
    protected override object? ConvertCore(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (
            value is not IDictionary dictionary
            || parameter is null
            || ConverterUtils.IsUnavailable(parameter)
        )
            return GetDefaultResult();
        if (dictionary.Contains(parameter) is false)
            return GetDefaultResult();
        var result = dictionary[parameter];
        return ConverterUtils.IsUnavailable(result) ? GetDefaultResult() : result;
    }
}
