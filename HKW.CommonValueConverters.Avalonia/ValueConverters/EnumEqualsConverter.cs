using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.EnumEqualsConverter"/> 的 Avalonia 包装器.
/// </summary>
public class EnumEqualsConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public EnumEqualsConverter()
        : base(new Core.EnumEqualsConverter()) { }
}
