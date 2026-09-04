using System.Collections;
using System.Globalization;
using HKW.HKWUtils;

namespace HKW.CommonValueConverters;

/// <summary>
/// 集合数量差值
/// <para>示例:
/// <code><![CDATA[
/// <Binding Collection, Converter="{StaticResource CollectionCountDifferenceConverter}" ConverterParameter="0"/>
/// return: Collection.Count - (int)Parameter
/// ]]></code></para>
/// </summary>
public class CollectionCountDifferenceConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public CollectionCountDifferenceConverter() { }

    /// <inheritdoc/>
    public override object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        if (parameter is not int count)
        {
            if (int.TryParse(parameter?.ToString(), out var i))
                count = i;
            else
                return GetDefaultResult();
        }
        if (value is string str)
            return str.Length - count;
        if (value is ICollection collection)
            return collection.Count - count;
        if (value is IEnumerable enumerable)
            return enumerable.Cast<object>().Count() - count;

        return GetDefaultResult();
    }
}
