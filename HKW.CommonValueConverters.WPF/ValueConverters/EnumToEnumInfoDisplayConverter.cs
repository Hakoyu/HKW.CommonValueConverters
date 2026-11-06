using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.EnumToEnumInfoDisplayConverter"/> 的 WPF 包装器.
/// </summary>
public class EnumToEnumInfoDisplayConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public EnumToEnumInfoDisplayConverter()
        : base(new Core.EnumToEnumInfoDisplayConverter())
    {
        var core = (Core.EnumToEnumInfoDisplayConverter)CoreConverter;
        core.GetEnumInfoDisplayTarget = () => EnumInfoDisplayTarget;
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<EnumInfoDisplayTarget> EnumInfoDisplayTargetProperty =
        CommonDependencyProperty.Register<EnumToEnumInfoDisplayConverter, EnumInfoDisplayTarget>(
            nameof(EnumInfoDisplayTarget),
            EnumInfoDisplayTarget.Name
        );

    /// <summary>
    /// 从枚举信息中选取的显示字段.
    /// </summary>
    public EnumInfoDisplayTarget EnumInfoDisplayTarget
    {
        get => GetValue(EnumInfoDisplayTargetProperty);
        set => SetValue(EnumInfoDisplayTargetProperty, value);
    }
}
