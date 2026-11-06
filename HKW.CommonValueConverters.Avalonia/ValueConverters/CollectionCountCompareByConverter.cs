using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.CollectionCountCompareByConverter"/> 的 Avalonia 包装器.
/// </summary>
public class CollectionCountCompareByConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public CollectionCountCompareByConverter()
        : base(new Core.CollectionCountCompareByConverter()) { }
}
