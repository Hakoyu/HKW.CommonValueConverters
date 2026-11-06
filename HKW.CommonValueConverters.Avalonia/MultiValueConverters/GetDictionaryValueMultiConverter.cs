using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.GetDictionaryValueMultiConverter"/> 的 Avalonia 包装器
/// </summary>
public class GetDictionaryValueMultiConverter : MultiValueConverterBase
{
    /// <inheritdoc/>
    public GetDictionaryValueMultiConverter()
        : base(new Core.GetDictionaryValueMultiConverter()) { }
}
