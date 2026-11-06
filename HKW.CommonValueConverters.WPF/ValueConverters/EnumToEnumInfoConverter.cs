using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.EnumToEnumInfoConverter"/> 的 WPF 包装器.
/// </summary>
public class EnumToEnumInfoConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public EnumToEnumInfoConverter()
        : base(new Core.EnumToEnumInfoConverter()) { }
}
