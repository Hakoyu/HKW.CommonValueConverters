using System.Collections;
using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 获取字典值
/// <para>示例:
/// <code><![CDATA[
/// <MultiBinding Converter="{StaticResource GetDictionaryValueMulitiConverter}">
///   <Binding Path="Dictionary" />
///   <Binding Path="Key" />
/// </MultiBinding>
/// result: Dictionary[Key]
/// ]]></code></para>
/// </summary>
public class GetDictionaryValueMultiConverter : MultiValueConverterBase
{
    /// <inheritdoc/>
    public override object? Convert(
        IList<object?> value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        var defaultResult = GetDefaultResult();
        if (value.Count != 2)
            return defaultResult;
        if (value[0] is not IDictionary dictionary)
            return defaultResult;
        if (value[1] is null)
            return defaultResult;
        if (dictionary.Contains(value[1]!) is false)
            return defaultResult;
        return dictionary[value[1]!];
    }
}
