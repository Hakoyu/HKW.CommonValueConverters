using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.NullToBoolConverter"/> 的 WPF 包装器.
/// </summary>
public class NullToBoolConverter : InvertibleValueConverterBase
{
    /// <inheritdoc/>
    public NullToBoolConverter()
        : base(new Core.NullToBoolConverter()) { }
}
