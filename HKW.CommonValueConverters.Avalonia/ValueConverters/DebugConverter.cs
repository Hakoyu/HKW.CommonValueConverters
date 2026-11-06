using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.DebugConverter"/> 的 Avalonia 包装器.
/// </summary>
public class DebugConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public DebugConverter()
        : base(new Core.DebugConverter()) { }
}
