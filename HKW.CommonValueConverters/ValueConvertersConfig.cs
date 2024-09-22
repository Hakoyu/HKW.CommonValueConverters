namespace HKW.CommonValueConverters;

/// <summary>
/// 值转换器设置
/// </summary>
public static class ValueConvertersConfig
{
    /// <summary>
    /// The default culture override behavior.
    /// </summary>
    public static PreferredCulture DefaultPreferredCulture { get; set; } =
        PreferredCulture.ConverterCulture;
}
