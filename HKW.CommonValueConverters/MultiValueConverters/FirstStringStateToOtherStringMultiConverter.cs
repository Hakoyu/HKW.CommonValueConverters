using System.Globalization;
using HKW.HKWUtils.Extensions;

namespace HKW.CommonValueConverters;

/// <summary>
/// 字符串为Null或空到字符串转换器
/// <para>示例:
/// <code><![CDATA[
/// <MultiBinding Converter="{StaticResource FirstStringStateToOtherStringMultiConverter}">
///   <Binding Path="Str1" />
///   <Binding Path="Str2" />
///   <Binding Path="Str3" />
///   <Binding Path="Str4" />
///   <Binding Path="Str5" />
/// </MultiBinding>
/// result:
/// Str1 is valid string, return Str2
/// Str1 is null, return Str3
/// Str1 is Empty, return Str4
/// Str1 is WhiteSpace, return Str5
/// ]]></code></para>
/// </summary>
public class FirstStringStateToOtherStringMultiConverter : MultiValueConverterBase
{
    /// <inheritdoc/>
    public override object? Convert(
        IList<object?> values,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        if (values == null)
            throw new ConverterException("Values is null");
        var value = values[0];
        if (value is not string str)
            return values.GetValueOrDefault(2, "Str3");
        else if (str == string.Empty)
            return values.GetValueOrDefault(3, "Str4");
        else if (string.IsNullOrWhiteSpace(str))
            return values.GetValueOrDefault(4, "Str5");
        return values.GetValueOrDefault(1, "Str2");
    }
}
