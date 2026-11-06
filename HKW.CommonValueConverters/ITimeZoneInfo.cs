namespace HKW.CommonValueConverters;

/// <summary>
/// 时区信息接口
/// </summary>
public interface ITimeZoneInfo
{
    /// <summary>
    /// UTC 时区的 <see cref="TimeZoneInfo"/>.
    /// </summary>
    public TimeZoneInfo Utc { get; }

    /// <summary>
    /// 本地时区的 <see cref="TimeZoneInfo"/>.
    /// </summary>
    public TimeZoneInfo Local { get; }
}
