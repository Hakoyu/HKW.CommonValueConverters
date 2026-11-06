using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.ValueToEnumerableConverter"/> 的 Avalonia 包装器.
/// </summary>
public class ValueToEnumerableConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public ValueToEnumerableConverter()
        : base(new Core.ValueToEnumerableConverter()) { }
}
