using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.ToBoolConverter"/> 的 Avalonia 包装器.
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
        CommonDependencyProperty.Register<ToBoolConverter, bool?>(
            nameof(NullValue),
            defaultValue: false
        );

    /// <summary>
    /// 输入无法解释为布尔值时返回的值.
    /// </summary>
    public bool? NullValue
    {
        get => GetValue(NullValueProperty);
        set => SetValue(NullValueProperty, value);
    }
}
