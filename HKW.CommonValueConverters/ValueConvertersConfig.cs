using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 值转换器设置
/// </summary>
public static class ValueConvertersConfig
{
    /// <summary>
    /// 默认首选文化
    /// </summary>
    public static PreferredCulture DefaultPreferredCulture { get; set; } =
        PreferredCulture.ConverterCulture;

    /// <summary>
    /// 选择文化
    /// </summary>
    /// <param name="preferredCulture">首选文化</param>
    /// <param name="converterCulture">转换器文化</param>
    /// <returns>文化</returns>
    public static CultureInfo SelectCulture(
        PreferredCulture preferredCulture,
        Func<CultureInfo> converterCulture
    )
    {
        return preferredCulture switch
        {
            PreferredCulture.CurrentCulture => CultureInfo.CurrentCulture,
            PreferredCulture.CurrentUICulture => CultureInfo.CurrentUICulture,
            _ => converterCulture(),
        };
    }
}
