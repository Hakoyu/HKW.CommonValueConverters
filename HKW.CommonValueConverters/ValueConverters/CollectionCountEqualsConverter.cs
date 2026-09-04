using System.Collections;
using System.Globalization;
using HKW.HKWUtils;

namespace HKW.CommonValueConverters;

/// <summary>
/// 集合数量相等于参数
/// <para>示例:
/// <code><![CDATA[
/// <Binding Collection, Converter="{StaticResource CollectionCountEqualsConverter}" ConverterParameter="0"/>
/// return: Collection.Count == (int)Parameter
/// ]]></code></para>
/// </summary>
public class CollectionCountEqualsConverter : ValueConverterBase
{
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

        if (value is string text)
            return text.Length == count;
        if (value is ICollection collection)
            return collection.Count == count;
        if (value is IEnumerable enumerable)
            return enumerable.Cast<object>().Count() == count;
        return GetDefaultResult();
    }
}
