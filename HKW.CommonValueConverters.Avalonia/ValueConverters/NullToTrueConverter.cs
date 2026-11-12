using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.NullToTrueConverter"/> 的 Avalonia 包装器.
/// </summary>
public class NullToTrueConverter : InvertibleValueConverterBase
{
    /// <inheritdoc/>
    public NullToTrueConverter()
        : base(new Core.NullToTrueConverter()) { }
}
