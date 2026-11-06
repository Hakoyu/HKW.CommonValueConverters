using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.CollectionCountCompareConverter"/> 的 Avalonia 包装器.
/// </summary>
public class CollectionCountCompareConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public CollectionCountCompareConverter()
        : base(new Core.CollectionCountCompareConverter()) { }
}
