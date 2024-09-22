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
/// <para>示例:
/// <code><![CDATA[
/// <MultiBinding Converter="{StaticResource GetDictionaryValueMulitiConverter}">
///   <Binding Path="Dictionary" />
///   <Binding Path="Key" />
/// </MultiBinding>
/// result: Dictionary[Key]
/// ]]></code></para>
/// </summary>
public class GetDictionaryValueMulitiConverter : MultiValueConverterBase
{
    /// <inheritdoc/>
    public override object? Convert(
        IList<object?> value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        var defultResult = GetDefaultResult();
        if (value.Count != 2)
            return defultResult;
        if (value[0] is not IDictionary dictionary)
            return defultResult;
        if (value[1] is null)
            return defultResult;
        if (dictionary.Contains(value[1]!) is false)
            return defultResult;
        return dictionary[value[1]!];
    }
}
