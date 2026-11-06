using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.EnumsToEnumInfosConverter"/> 的 Avalonia 包装器.
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
            defaultValue: false
        );

    /// <summary>
    /// 是否仅返回有效枚举值的信息.
    /// </summary>
    public bool OnlyValid
    {
        get => GetValue(OnlyValidProperty);
        set => SetValue(OnlyValidProperty, value);
    }
}
