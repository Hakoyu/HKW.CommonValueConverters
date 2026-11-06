using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.GetDictionaryValueMultiConverter"/> 的 WPF 包装器.
/// </summary>
public class GetDictionaryValueMultiConverter : MultiValueConverterBase
{
    /// <inheritdoc/>
    public GetDictionaryValueMultiConverter()
        : base(new Core.GetDictionaryValueMultiConverter()) { }
}
