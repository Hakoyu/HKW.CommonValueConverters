using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 值转换器设置
/// </summary>
public static class ValueConvertersConfig
{
    /// <summary>
    /// 实例未覆盖时使用的 <see cref="PreferredCulture"/>.
    /// </summary>
    public static PreferredCulture DefaultPreferredCulture { get; set; } =
        PreferredCulture.ConverterCulture;

    /// <summary>
    /// 按 <paramref name="preferredCulture"/> 选择 <see cref="CultureInfo"/>, 空调用文化回退到 <see cref="CultureInfo.CurrentCulture"/>.
    /// </summary>
    /// <param name="preferredCulture">首选文化</param>
    /// <param name="converterCulture">转换器文化</param>
    /// <returns>文化</returns>
    public static CultureInfo SelectCulture(
        PreferredCulture preferredCulture,
        CultureInfo? converterCulture
    )
    {
        return preferredCulture switch
        {
            PreferredCulture.CurrentCulture => CultureInfo.CurrentCulture,
            PreferredCulture.CurrentUICulture => CultureInfo.CurrentUICulture,
            _ => converterCulture ?? CultureInfo.CurrentCulture,
        };
    }
}
