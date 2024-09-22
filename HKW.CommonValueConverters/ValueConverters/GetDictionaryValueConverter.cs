using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.CommonValueConverters;

/// <summary>
/// 获取字典值
/// </summary>
public class GetDictionaryValueConverter : ValueConverterBase
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
        if (value is not IDictionary dictionary)
            return defultResult;
        if (parameter is null)
            return defultResult;
        if (dictionary.Contains(parameter) is false)
            return defultResult;
        return dictionary[parameter];
    }
}
