using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.ValueToEnumerableConverter"/> 的 WPF 包装器.
/// </summary>
public class ValueToEnumerableConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public ValueToEnumerableConverter()
        : base(new Core.ValueToEnumerableConverter()) { }
}
