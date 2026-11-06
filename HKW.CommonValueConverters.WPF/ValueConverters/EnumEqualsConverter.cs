using System.Globalization;
using System.Windows;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// <see cref="Core.EnumEqualsConverter"/> 的 WPF 包装器.
/// </summary>
public class EnumEqualsConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public EnumEqualsConverter()
        : base(new Core.EnumEqualsConverter()) { }
}
