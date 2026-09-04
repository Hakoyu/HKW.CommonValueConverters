using System.Globalization;
using HKW.CommonValueConverters;

namespace HKW.CommonValueConverters;

/// <summary>
/// 字符串到布尔转换器
/// <para><![CDATA[
/// {Binding Obj, Converter={StaticResource ToBoolConverter}}
/// Obj == 0, return false
/// Obj == "1", return true
/// Obj == null, return false
/// ]]>
/// </para>
/// </summary>
public class ToBoolConverter : ValueConverterBase
{
    /// <summary>
    /// 数值类型
    /// </summary>
    public Func<bool?> GetNullValue { get; set; } = static () => false;

    /// <inheritdoc/>
    public override object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        return ConverterUtils.GetBool(value, GetNullValue());
    }
}
