using System;
using System.Collections;
using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 第一个或默认转换器
/// </summary>
public class FirstOrDefaultResultConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public override object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        var defaultResult = GetDefaultResult();
        if (value is IEnumerable enumerable)
        {
            var enumerator = enumerable.GetEnumerator();
            {
                if (enumerator.MoveNext())
                {
                    return enumerator.Current;
                }
            }
        }

        return defaultResult;
    }
}
