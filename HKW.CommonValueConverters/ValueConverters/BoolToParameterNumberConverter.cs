using System.Globalization;
using System.Numerics;
using HKW.CommonValueConverters;
using HKW.HKWUtils;
using HKW.HKWUtils.Extensions;

namespace HKW.CommonValueConverters;

/// <summary>
/// 布尔到布尔参数Int32转换器
/// <para>示例:
/// <code><![CDATA[
/// <Binding Bool, Converter="{StaticResource BoolToParameterDoubleConverter}" ConverterParameter="1,2"/>
/// return: Bool ? 1 : 2
/// ]]></code></para>
/// </summary>
public class BoolToParameterNumberConverter<T> : BoolToSplitParameterConverterBase
    where T : struct, INumber<T>
{
    /// <inheritdoc/>
    public BoolToParameterNumberConverter()
    {
        GetDefaultResult = () => 0;
    }

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
        var spilt = str.AsSpan().Split(GetSeparator());
        spilt.MoveNext();
        if (spilt.MoveNext() && r)
            return NumberUtils.ConvertTo<T>(spilt.Current);
        else if (spilt.MoveNext())
            return NumberUtils.ConvertTo<T>(spilt.Current);
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
        var spilt = str.AsSpan().Split(GetSeparator());
        spilt.MoveNext();
        if (r)
            return NumberUtils.ConvertTo<T>(spilt.Current);
        else if (spilt.MoveNext())
            return NumberUtils.ConvertTo<T>(spilt.Current);
        return defultResult;
    }
}
