using System.ComponentModel;
using System.Globalization;
using System.Numerics;
using System.Windows;
using HKW.CommonValueConverters;
using HKW.HKWUtils;
using HKW.HKWUtils.Extensions;

namespace HKW.CommonValueConverters;

///// <summary>
///// bool到分割数字转换器
///// </summary>
///// <typeparam name="T"></typeparam>
//public class BoolToSplitNumberConverter<T> : BoolToSplitParameterConverter<T>
//    where T : struct, INumber<T>
//{
//    /// <inheritdoc/>
//    public BoolToSplitNumberConverter()
//    {
//        GetConvertParameter = s => (T)NumberUtils.ConvertTo<T>(str: s);
//    }
//}

/// <summary>
/// 布尔到布尔参数转换器
/// <para>示例:
/// <code><![CDATA[
/// <Binding Bool, Converter="{StaticResource BoolToParameterDoubleConverter}" ConverterParameter="1,2"/>
/// return: Bool ? 1 : 2
/// ]]></code></para>
/// </summary>
public class BoolToSplitParameterConverter : ValueConverterBase
{
    /// <summary>
    /// 分割符
    /// </summary>
    [DefaultValue(',')]
    public Func<char> GetSeparator { get; set; } = () => ',';

    /// <summary>
    /// 获取参数
    /// </summary>
    public Func<string, object> GetConvertParameter { get; set; } = x => default!;

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
        if (spilt.Length >= 1 && r)
            return GetConvertParameter(spilt[0]);
        else if (spilt.Length >= 2 && r is false)
            return GetConvertParameter(spilt[1]);
        else if (spilt.Length >= 3 && value is null)
            return GetConvertParameter(spilt[2]);
        return defultResult;
    }

    /// <inheritdoc/>
    public override object? ConvertBack(
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
        if (spilt.Length >= 1 && r)
            return GetConvertParameter(spilt[0]);
        else if (spilt.Length >= 2 && r is false)
            return GetConvertParameter(spilt[1]);
        else if (spilt.Length >= 3 && value is null)
            return GetConvertParameter(spilt[2]);
        return defultResult;
    }
}
