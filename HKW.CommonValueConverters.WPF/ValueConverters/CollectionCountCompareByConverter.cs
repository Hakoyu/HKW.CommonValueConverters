using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.CollectionCountCompareByConverter"/> 的 WPF 包装器.
/// </summary>
public class CollectionCountCompareByConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public CollectionCountCompareByConverter()
        : base(new Core.CollectionCountCompareByConverter()) { }
}
