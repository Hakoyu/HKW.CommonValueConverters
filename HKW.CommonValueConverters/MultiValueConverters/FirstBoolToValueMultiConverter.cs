using System.Globalization;
using HKW.HKWUtils.Extensions;

namespace HKW.CommonValueConverters;

/// <summary>
/// 第一个为布尔到值转换器
/// /// <para>示例:
/// <code><![CDATA[
/// <MultiBinding Converter="{StaticResource FirstBoolToValueConverter}">
///   <Binding Path="BoolValue" />
///   <Binding Path="TrueValue" />
///   <Binding Path="FalueValue" /> // default is null
///   <Binding Path="NullValue" /> // default is FalueValue
/// </MultiBinding>
/// ]]></code></para>
/// </summary>
public class FirstBoolToValueMultiConverter : InvertibleMultiValueConverterBase
{
    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">参数数量必须为2或3</exception>
    public override object? Convert(
        IList<object?> values,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        var isInverted = GetIsInverted();
        if (values.Count < 2)
            throw new ArgumentException("Values count must be more than 2");
        var result = values[0];
        if (result is true ^ isInverted)
            return values[1];
        else if (result is false ^ isInverted)
            return values.GetValueOrDefault(2);
        else if (result is null ^ isInverted)
            return values.GetValueOrDefault(3);
        return GetDefaultResult();
    }
}
