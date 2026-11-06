using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.EnumToEnumInfoConverter"/> 的 Avalonia 包装器.
/// </summary>
public class EnumToEnumInfoConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public EnumToEnumInfoConverter()
        : base(new Core.EnumToEnumInfoConverter()) { }
}
