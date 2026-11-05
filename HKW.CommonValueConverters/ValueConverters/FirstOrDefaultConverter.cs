using System.Collections;
using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 第一个或默认转换器
/// </summary>
public class FirstOrDefaultConverter : ValueConverterBase
{
    /// <inheritdoc/>
    protected override object? ConvertCore(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (value is not IEnumerable enumerable)
            return GetDefaultResult();
        var enumerator = enumerable.GetEnumerator();
        try
        {
            if (!enumerator.MoveNext())
                return GetDefaultResult();
            var result = enumerator.Current;
            return ConverterUtils.IsUnavailable(result) ? GetDefaultResult() : result;
        }
        finally
        {
            (enumerator as IDisposable)?.Dispose();
        }
    }
}
