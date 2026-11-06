namespace HKW.CommonValueConverters;

/// <summary>
/// <see cref="System.Globalization.CultureInfo"/> 的选择方式.
/// </summary>
public enum PreferredCulture
{
    /// <summary>
    /// 使用调用者传入的文化, 为空时回退到 <see cref="System.Globalization.CultureInfo.CurrentCulture"/>.
    /// </summary>
    ConverterCulture,

    /// <summary>
    /// 使用 <see cref="System.Globalization.CultureInfo.CurrentCulture"/>.
    /// </summary>
    CurrentCulture,

    /// <summary>
    /// 使用 <see cref="System.Globalization.CultureInfo.CurrentUICulture"/>.
    /// </summary>
    CurrentUICulture,
}
