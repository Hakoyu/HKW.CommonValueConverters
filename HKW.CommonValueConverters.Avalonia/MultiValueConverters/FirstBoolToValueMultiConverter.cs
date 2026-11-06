using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.FirstBoolToValueMultiConverter"/> 的 Avalonia 包装器
/// </summary>
public class FirstBoolToValueMultiConverter : MultiValueConverterBase
{
    /// <inheritdoc/>
    public FirstBoolToValueMultiConverter()
        : base(new Core.FirstBoolToValueMultiConverter()) { }
}
