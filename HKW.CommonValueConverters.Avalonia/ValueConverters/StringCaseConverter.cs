using System.Globalization;
using global::Avalonia;
using HKW.HKWUtils;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// <see cref="Core.StringCaseConverter"/> 的 Avalonia 包装器.
/// </summary>
public class StringCaseConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public StringCaseConverter()
        : base(new Core.StringCaseConverter()) { }
}
