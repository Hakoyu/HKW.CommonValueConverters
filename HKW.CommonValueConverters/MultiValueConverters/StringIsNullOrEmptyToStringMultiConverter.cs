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
/// </MultiBinding>
/// result:
/// Str1 is null, return Str2
/// Str1 is Empty, return Str3
/// Str1 is WhiteSpace, return Str4
/// ]]></code></para>
/// </summary>
public class FirstStringStateToOtherStringMultiConverter : MultiValueConverterBase
{
    /// <summary>
    /// 字符串检查类型
    /// </summary>
    public Func<StringCheckType> GetStringCheckType { get; set; } = () => StringCheckType.Null;

    /// <inheritdoc/>
    public override object? Convert(
        IList<object?> values,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        var defultResult = GetDefaultResult();
        if (values == null)
            return null;
        var value = values[0];
        if (value is not string str)
            return values.GetValueOrDefault(1);
        else if (str == string.Empty)
            return values.GetValueOrDefault(2);
        else if (string.IsNullOrWhiteSpace(str))
            return values.GetValueOrDefault(3);
        return defultResult;
    }
}
