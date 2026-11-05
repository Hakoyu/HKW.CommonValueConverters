using System.Globalization;

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
    /// <remarks>输入必须为两个已就绪的值，否则返回默认结果。</remarks>
    protected override object? ConvertCore(
        IList<object?> values,
        Type? targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        var isInverted = GetIsInverted();
        if (ConverterUtils.HasUnavailable(values) || values.Count != 2)
            return GetDefaultResult();
        if (GetIsStringEquals())
            return (values[0]?.ToString() == values[1]?.ToString()) ^ isInverted;
        return Equals(values[0], values[1]) ^ isInverted;
    }
}
