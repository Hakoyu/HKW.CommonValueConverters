using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.FirstBoolToValueMultiConverter"/> 的 WPF 包装器.
/// </summary>
public class FirstBoolToValueMultiConverter : MultiValueConverterBase
{
    /// <inheritdoc/>
    public FirstBoolToValueMultiConverter()
        : base(new Core.FirstBoolToValueMultiConverter()) { }
}
