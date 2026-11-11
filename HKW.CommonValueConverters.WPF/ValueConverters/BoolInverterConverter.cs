using System.Globalization;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// 仅对装箱的布尔值取反. 空值和其他类型的输入返回 DefaultResult.
/// 不支持反向转换.
/// </summary>
public class BoolInverterConverter : ValueConverterBase
{
    /// <summary>
    /// 布尔值取反, 可单独设定 null 的返回值
    /// </summary>
    public BoolInverterConverter()
        : base(new Core.BoolInverterConverter())
    {
        var core = (Core.BoolInverterConverter)CoreConverter;
        core.GetNullValue = CreateCoreGetter(NullValueProperty);
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<bool> NullValueProperty =
        CommonDependencyProperty.Register<BoolInverterConverter, bool>(nameof(NullValue), false);

    /// <summary>
    /// 输入为 <see langword="null"/> 时返回的值.
    /// </summary>
    public bool NullValue
    {
        get => GetMappedValue(NullValueProperty);
        set => SetMappedValue(NullValueProperty, value);
    }
}
