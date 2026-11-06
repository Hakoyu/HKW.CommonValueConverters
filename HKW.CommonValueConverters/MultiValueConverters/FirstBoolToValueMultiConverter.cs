using System.Globalization;
using HKW.HKWUtils.Extensions;

namespace HKW.CommonValueConverters;

/// <summary>
/// 第一个为布尔到值转换器
/// <para>示例:
/// <code><![CDATA[
/// <MultiBinding Converter="{StaticResource FirstBoolToValueMultiConverter}">
///   <Binding Path="BoolValue" />
///   <Binding Path="TrueValue" />
///   <Binding Path="FalseValue" />
///   <Binding Path="NullValue" />
/// </MultiBinding>
/// ]]></code></para>
/// </summary>
/// <remarks>
/// 缺失或未就绪的选中分支返回默认结果.
/// </remarks>
public class FirstBoolToValueMultiConverter : MultiValueConverterBase
{
    /// <inheritdoc/>
    protected override object? ConvertCore(
        IList<object?> values,
        Type? targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        if (values is null || values.Count == 0)
            return GetDefaultResult();
        var index = values[0] switch
        {
            true => 1,
            false => 2,
            null => 3,
            _ => -1,
        };
        if (index < 0 || index >= values.Count || ConverterUtils.IsUnavailable(values[index]))
            return GetDefaultResult();
        return values[index];
    }
}
