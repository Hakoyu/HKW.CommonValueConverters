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
    protected override object? ConvertCore(
        IList<object?> values,
        Type? targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (values is null || ConverterUtils.IsUnavailable(parameter))
            return GetDefaultResult();
        var replaceValue = GetReplaceUnsetValue();
        var format = parameter as string;
        var start = 0;
        if (string.IsNullOrWhiteSpace(format))
        {
            if (values.Count == 0 || values[0] is not string source)
                return GetDefaultResult();
            format = source;
            start = 1;
        }
        var arguments = values.Skip(start).ToArray();
        for (var i = 0; i < arguments.Length; i++)
        {
            if (
                ReferenceEquals(arguments[i], ConverterResults.UnsetValue)
                && replaceValue is not null
            )
                arguments[i] = replaceValue;
            else if (ConverterUtils.IsUnavailable(arguments[i]))
                return GetDefaultResult();
        }
        System.Text.CompositeFormat parsed;
        try
        {
            parsed = System.Text.CompositeFormat.Parse(format);
        }
        catch (FormatException)
        {
            return GetDefaultResult();
        }
        if (parsed.MinimumArgumentCount > arguments.Length)
            return GetDefaultResult();
        try
        {
            return string.Format(culture, parsed, arguments);
        }
        catch (FormatException)
            when (culture.GetType() == typeof(CultureInfo)
                && arguments.All(v => v is null || v.GetType().Assembly == typeof(int).Assembly)
            )
        {
            return GetDefaultResult();
        }
    }
}
