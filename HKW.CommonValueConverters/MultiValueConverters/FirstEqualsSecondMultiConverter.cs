using System.Globalization;
using System.Windows;

namespace HKW.CommonValueConverters;

/// <summary>
/// 相等多值转换器
/// <para>示例:
/// <code><![CDATA[
/// <MultiBinding Converter="{StaticResource EqualsMultiConverter}">
///   <Binding Path="Value1" />
///   <Binding Path="Value2" />
/// </MultiBinding>
/// result: Value1.Equals(Value2)
/// ]]></code></para>
/// </summary>
public class FirstEqualsSecondMultiConverter : InvertibleMultiValueConverterBase
{
    /// <summary>
    /// 是字符串比较
    /// </summary>
    public Func<bool> GetIsStringEquals { get; set; } = () => default!;

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">参数必须为2</exception>
    public override object? Convert(
        IList<object?> values,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        var isInverted = GetIsInverted();
        if (values.Count != 2)
            throw new NotImplementedException("Values length must be 2");
        if (GetIsStringEquals())
            return values[0]?.ToString() == values[1]?.ToString() ^ isInverted;
        return values[0]?.Equals(values[1]) ^ isInverted;
    }
}
