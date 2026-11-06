using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.ToBoolConverter"/> 的 WPF 包装器.
/// </summary>
public class ToBoolConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public ToBoolConverter()
        : base(new Core.ToBoolConverter())
    {
        var core = (Core.ToBoolConverter)CoreConverter;
        core.GetNullValue = () => NullValue;
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<bool?> NullValueProperty =
        CommonDependencyProperty.Register<ToBoolConverter, bool?>(nameof(NullValue), false);

    /// <summary>
    /// 无法解释为布尔值或输入为 <see langword="null"/> 时返回的可空布尔值.
    /// </summary>
    public bool? NullValue
    {
        get => GetValue(NullValueProperty);
        set => SetValue(NullValueProperty, value);
    }
}
