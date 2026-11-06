using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.GetDictionaryValueConverter"/> 的 Avalonia 包装器.
/// </summary>
public class GetDictionaryValueConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public GetDictionaryValueConverter()
        : base(new Core.GetDictionaryValueConverter()) { }
}
