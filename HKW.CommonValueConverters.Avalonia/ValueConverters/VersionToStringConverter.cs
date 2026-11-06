using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.VersionToStringConverter"/> 的 Avalonia 包装器.
/// </summary>
public class VersionToStringConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public VersionToStringConverter()
        : base(new Core.VersionToStringConverter()) { }
}
