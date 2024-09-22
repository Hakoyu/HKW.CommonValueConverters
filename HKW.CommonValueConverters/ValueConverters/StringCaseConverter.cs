using System;
using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 字符串大小写转换器
/// <para><![CDATA[
/// {Binding Text, Converter={StaticResource StringCaseConverter}, ConverterParameter=L}}
/// Parameter == L
/// result: culture.TextInfo.ToLower(Text)
/// Parameter == U
/// result: culture.TextInfo.ToUpper(Text)
/// Parameter == T
/// result: culture.TextInfo.ToTitleCase(Text)
/// ]]>
/// </para>
/// </summary>
public class StringCaseConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public override object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        if (value is string stringValue)
        {
            culture ??= CultureInfo.CurrentCulture;
            return parameter?.ToString() switch
            {
                // 大写
                "U" or "u" => culture.TextInfo.ToUpper(stringValue),
                // 小写
                "L" or "l" => culture.TextInfo.ToLower(stringValue),
                // 标题
                "T" or "t" => culture.TextInfo.ToTitleCase(stringValue),
                _ => GetDefaultResult(),
            };
        }

        return null;
    }
}
