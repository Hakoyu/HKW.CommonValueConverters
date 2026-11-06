using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.FirstOrDefaultConverter"/> 的 Avalonia 包装器.
/// </summary>
public class FirstOrDefaultConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public FirstOrDefaultConverter()
        : base(new Core.FirstOrDefaultConverter()) { }
}
