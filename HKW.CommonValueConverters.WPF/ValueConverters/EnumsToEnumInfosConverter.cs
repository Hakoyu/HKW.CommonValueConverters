using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.EnumsToEnumInfosConverter"/> 的 WPF 包装器.
/// </summary>
public class EnumsToEnumInfosConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public EnumsToEnumInfosConverter()
        : base(new Core.EnumsToEnumInfosConverter())
    {
        var core = (Core.EnumsToEnumInfosConverter)CoreConverter;
        core.GetOnlyValid = () => OnlyValid;
    }

    /// <inheritdoc/>
    public static readonly Core.CommonDependencyProperty<bool> OnlyValidProperty =
        CommonDependencyProperty.Register<EnumsToEnumInfosConverter, bool>(
            nameof(OnlyValid),
            false
        );

    /// <summary>
    /// 是否只返回枚举类型的有效值信息.
    /// </summary>
    public bool OnlyValid
    {
        get => GetValue(OnlyValidProperty);
        set => SetValue(OnlyValidProperty, value);
    }
}
