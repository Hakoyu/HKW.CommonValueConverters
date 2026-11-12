using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.NullToTrueConverter"/> 的 WPF 包装器.
/// </summary>
public class NullToTrueConverter : InvertibleValueConverterBase
{
    /// <inheritdoc/>
    public NullToTrueConverter()
        : base(new Core.NullToTrueConverter()) { }
}
