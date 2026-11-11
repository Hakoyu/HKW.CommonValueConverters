using System.ComponentModel;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// 将布尔输入映射为可配置的数字, 默认回退值为零.
/// 默认数值类型为 Int32, 真/假/空值的映射初始均为零.
/// </summary>
public class BoolToNumberConverter : BoolToValueConverter<object>
{
    /// <inheritdoc/>
    public BoolToNumberConverter()
    {
        TrueValue = NumberUtils.GetDefault(NumberType);
        FalseValue = NumberUtils.GetDefault(NumberType);
        NullValue = NumberUtils.GetDefault(NumberType);
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<NumberType> NumberTypeProperty =
        CommonDependencyProperty.Register<BoolToNumberConverter, NumberType>(
            nameof(NumberType),
            NumberType.Int32
        );

    /// <summary>
    /// 解析或计算时使用的数值类型.
    /// </summary>
    [DefaultValue(NumberType.Int32)]
    public NumberType NumberType
    {
        get => GetValue(NumberTypeProperty);
        set => SetValue(NumberTypeProperty, value);
    }
}
