using System;
using System.Threading;

namespace HKW.CommonValueConverters;

/// <summary>
/// 系统时区信息
/// </summary>
public class SystemTimeZoneInfo : ITimeZoneInfo
{
    private static readonly Lazy<ITimeZoneInfo> _current =
        new(() => new SystemTimeZoneInfo(), LazyThreadSafetyMode.PublicationOnly);

    /// <summary>
    /// 当前时区
    /// </summary>
    public static ITimeZoneInfo Current => _current.Value;

    /// <summary>
    /// UTC时区
    /// </summary>
    public TimeZoneInfo Utc => TimeZoneInfo.Utc;

    /// <summary>
    /// 本地时区
    /// </summary>
    public TimeZoneInfo Local => TimeZoneInfo.Local;
}
