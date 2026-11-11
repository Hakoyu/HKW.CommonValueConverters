using System.Collections;
using System.Globalization;
using HKW.HKWUtils;

namespace HKW.CommonValueConverters;

/// <summary>
/// 集合数量比较器
/// <para>示例:
/// <code><![CDATA[
/// IsEnabled={Binding Collection, Converter={StaticResource CollectionCountCompareConverter}, ConverterParameter="1"}
/// result: Collection.CompareTo(Parameter)
/// ]]></code></para>
/// </summary>
public class CollectionCountCompareConverter : ValueConverterBase
{
    /// <inheritdoc/>
    protected override object? ConvertCore(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (parameter is not int count)
        {
            if (
                ConverterUtils.IsUnavailable(parameter) is false
                && int.TryParse(parameter?.ToString(), NumberStyles.Integer, culture, out var i)
            )
                count = i;
            else
                return GetDefaultResult();
        }
        var collectionCount = -1;
        if (value is string str)
            collectionCount = str.Length;
        else if (value is ICollection collection)
            collectionCount = collection.Count;
        else if (value is IEnumerable enumerable)
            collectionCount = enumerable.Cast<object>().Count();

        if (collectionCount >= 0)
            return NumberUtils.CompareF<int>(collectionCount, count);
        else
            return GetDefaultResult();
    }
}
