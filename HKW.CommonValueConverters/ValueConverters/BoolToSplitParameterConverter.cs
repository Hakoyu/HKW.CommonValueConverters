using System.ComponentModel;
using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 布尔到布尔参数转换器
/// <para>示例1:
/// <code><![CDATA[
/// <Binding Bool, Converter="{StaticResource BoolToSplitParameterConverter}" ConverterParameter="1,2"/>
/// return: Bool ? 1 : 2
/// ]]></code></para>
/// <para>示例2:
/// <code><![CDATA[
/// <Binding Bool, Converter="{StaticResource BoolToSplitParameterConverter}" ConverterParameter="1,2,3"/>
/// return: Bool is null ? 3 : (Bool is true ? 1 : 2);
/// ]]></code></para>
/// </summary>
public class BoolToSplitParameterConverter : ValueConverterBase
{
    /// <summary>
    /// 默认分割符
    /// </summary>
    public const string DefaultSeparator = ",";

    /// <summary>
    /// 分割符
    /// </summary>
    [DefaultValue(",")]
    public Func<string> GetSeparator { get; set; } = () => DefaultSeparator;

    /// <summary>
    /// 分割值转换器
    /// </summary>
    public Func<string, object> ConvertSplitValue { get; set; } = s => s;

    /// <inheritdoc/>
    public override object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        var defultResult = GetDefaultResult();
        if (parameter is not string str || string.IsNullOrWhiteSpace(str))
            return defultResult;
        var r = ConverterUtils.GetBool(value);
        var spilt = str.Split(GetSeparator(), StringSplitOptions.RemoveEmptyEntries);
        if (spilt.Length == 0)
            return defultResult;
        if (spilt.Length >= 1 && r is true)
            return ConvertSplitValue(spilt[0]);
        else if (spilt.Length >= 2 && r is false)
            return ConvertSplitValue(spilt[1]);
        else if (spilt.Length >= 3 && value is null)
            return ConvertSplitValue(spilt[2]);
        return defultResult;
    }
}
