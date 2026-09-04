using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 字符串格式化转换器
/// <para>示例:
/// <code><![CDATA[
/// <MultiBinding Converter="{StaticResource MarginConverter}">
///   <Binding Path="StringFormat" />
///   <Binding Path="Value1" />
///   <Binding Path="Value2" />
/// </MultiBinding>
/// OR
/// <MultiBinding Converter="{StaticResource MarginConverter}" ConverterParameter="{}{0}{1}">
///   <Binding Path="Value1" />
///   <Binding Path="Value2" />
/// </MultiBinding>
/// ]]></code></para>
/// </summary>
public class StringFormatMultiConverter : MultiValueConverterBase
{
    /// <summary>
    /// 隐藏未设置的占位符
    /// </summary>
    public Func<string?> GetReplaceUnsetValue { get; set; } = () => null;

    /// <inheritdoc/>
    public override object? Convert(
        IList<object?> values,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        var replaceValue = GetReplaceUnsetValue();
        if (values is null)
            return GetDefaultResult();
        culture ??= CultureInfo.CurrentCulture;
        if (parameter is string format && string.IsNullOrWhiteSpace(format) is false)
        {
            if (replaceValue is not null)
                return string.Format(
                    culture,
                    format,
                    values.Select(v => v == UnsetValue ? replaceValue : v).ToArray()
                );
            else
                return string.Format(culture, format, values.ToArray());
        }
        else
        {
            format = (string)values[0]!;
            var temp = values.Skip(1);
            if (replaceValue is not null)
                return string.Format(
                    culture,
                    format,
                    temp.Select(v => v == UnsetValue ? replaceValue : v).ToArray()
                );
            else
                return string.Format(culture, format, temp.ToArray());
        }
    }
}
