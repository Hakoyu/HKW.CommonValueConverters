using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.NullToBoolConverter"/> 的 Avalonia 包装器.
/// </summary>
public class NullToBoolConverter : InvertibleValueConverterBase
{
    /// <inheritdoc/>
    public NullToBoolConverter()
        : base(new Core.NullToBoolConverter()) { }
}
