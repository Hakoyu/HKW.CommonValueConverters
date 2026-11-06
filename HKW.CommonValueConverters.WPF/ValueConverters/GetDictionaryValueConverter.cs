using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.GetDictionaryValueConverter"/> 的 WPF 包装器.
/// </summary>
public class GetDictionaryValueConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public GetDictionaryValueConverter()
        : base(new Core.GetDictionaryValueConverter()) { }
}
