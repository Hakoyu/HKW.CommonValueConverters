using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 首选文化
/// </summary>
public enum PreferredCulture
{
    /// <summary>
    /// 转换器文化
    /// </summary>
    ConverterCulture,

    /// <summary>
    /// 当前文化
    /// </summary>
    CurrentCulture,

    /// <summary>
    /// 当前UI文化
    /// </summary>
    CurrentUICulture,
}
