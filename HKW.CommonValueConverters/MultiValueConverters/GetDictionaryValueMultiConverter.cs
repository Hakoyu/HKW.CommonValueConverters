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
    protected override object? ConvertCore(
        IList<object?> value,
        Type? targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (
            ConverterUtils.HasUnavailable(value)
            || value.Count != 2
            || value[0] is not IDictionary dictionary
            || value[1] is null
        )
            return GetDefaultResult();
        if (!dictionary.Contains(value[1]!))
            return GetDefaultResult();
        var result = dictionary[value[1]!];
        return ConverterUtils.IsUnavailable(result) ? GetDefaultResult() : result;
    }
}
