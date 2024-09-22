using System;

namespace HKW.CommonValueConverters;

/// <summary>
/// 时区信息接口
/// </summary>
public interface ITimeZoneInfo
{
    /// <summary>
    /// UTC时区
    /// </summary>
    public TimeZoneInfo Utc { get; }

    /// <summary>
    /// 本地时区
    /// </summary>
    public TimeZoneInfo Local { get; }
}
