namespace HKW.CommonValueConverters;

/// <summary>
/// 字符串检查类型
/// </summary>
public enum StringCheckType
{
    /// <summary>
    /// 仅检查 <see langword="null"/>.
    /// </summary>
    Null,

    /// <summary>
    /// 使用 <see cref="string.IsNullOrEmpty"/> 检查空值或空字符串.
    /// </summary>
    NullOrEmpty,

    /// <summary>
    /// 使用 <see cref="string.IsNullOrWhiteSpace"/> 检查空值或空白字符.
    /// </summary>
    NullOrWhiteSpace,
}
