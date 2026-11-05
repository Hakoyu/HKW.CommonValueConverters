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
    protected override object? ConvertCore(
        IList<object?> values,
        Type? targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (values is null || values.Count == 0 || ConverterUtils.IsUnavailable(values[0]))
            return GetDefaultResult();
        var value = values[0];
        var index = value is not string str ? 2
            : str.Length == 0 ? 3
            : string.IsNullOrWhiteSpace(str) ? 4 : 1;
        if (index >= values.Count || ConverterUtils.IsUnavailable(values[index]))
            return GetDefaultResult();
        return values[index];
    }
}
