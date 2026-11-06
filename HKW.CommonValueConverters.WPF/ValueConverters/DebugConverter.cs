using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.DebugConverter"/> 的 WPF 包装器.
/// </summary>
public class DebugConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public DebugConverter()
        : base(new Core.DebugConverter()) { }
}
