using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.EnumToEnumInfoDisplayConverter"/> 的 Avalonia 包装器.
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
            defaultValue: EnumInfoDisplayTarget.Name
        );

    /// <summary>
    /// 枚举信息用于显示的目标成员.
    /// </summary>
    public EnumInfoDisplayTarget EnumInfoDisplayTarget
    {
        get => GetValue(EnumInfoDisplayTargetProperty);
        set => SetValue(EnumInfoDisplayTargetProperty, value);
    }
}
