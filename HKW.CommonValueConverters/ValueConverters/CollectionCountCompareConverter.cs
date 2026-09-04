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

/// <summary>
/// 集合数量比较器
/// <para>示例:
/// <code><![CDATA[
/// {Binding Collection, Converter={StaticResource CollectionCountCompareByConverter}, ConverterParameter=">1"}
/// result: Collection.Count.CompareBy(Parameter)
/// ParameterExamples: "==1", ">1", "<=5"
/// ]]></code></para>
/// </summary>
public class CollectionCountCompareByConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public override object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        ComparisonOperatorType comparisonType;
        var count = 0;
        if (parameter is string str && str.Length >= 2 && char.IsNumber(str[0]) is false)
        {
            comparisonType = NumberUtils.GetComparisonOperatorType(str, out var operatorLength);
            if (operatorLength == 0)
                return GetDefaultResult();
            count = int.Parse(str[operatorLength..]);
        }
        else
            return GetDefaultResult();

        var collectionCount = -1;
        if (value is string s)
            collectionCount = s.Length;
        else if (value is ICollection collection)
            collectionCount = collection.Count;
        else if (value is IEnumerable enumerable)
            collectionCount = enumerable.Cast<object>().Count();

        if (collectionCount >= 0)
            return NumberUtils.CompareByF<int>(collectionCount, count, comparisonType);
        else
            return GetDefaultResult();
    }
}
