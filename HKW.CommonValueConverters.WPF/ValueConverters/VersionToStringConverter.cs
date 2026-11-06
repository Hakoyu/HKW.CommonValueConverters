using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.VersionToStringConverter"/> 的 WPF 包装器.
/// </summary>
public class VersionToStringConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public VersionToStringConverter()
        : base(new Core.VersionToStringConverter()) { }
}
