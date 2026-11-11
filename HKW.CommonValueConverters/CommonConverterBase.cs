using System.ComponentModel;
using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 通用值转换器
/// </summary>
public interface ICommonValueConverter
{
    /// <summary>
    /// 首选文化, 使用 <see cref="HKW.CommonValueConverters.PreferredCulture"/> 指定文化来源.
    /// </summary>
    public PreferredCulture PreferredCulture { get; set; }
}

/// <summary>
/// 转换器基类
/// </summary>
public abstract class CommonConverterBase : ICommonValueConverter
{
    /// <summary>
    /// 全局默认结果, 默认为 <see cref="ConverterResults.UnsetValue"/>.
    /// </summary>
    public static object? GlobalDefaultResult { get; set; } = ConverterResults.UnsetValue;

    /// <summary>
    /// 默认结果, 默认读取 <see cref="GlobalDefaultResult"/>.
    /// <para>当转换器未成功执行时, 返回此值</para>
    /// </summary>
    public Func<object?> GetDefaultResult { get; set; } = () => GlobalDefaultResult;

    private PreferredCulture? _preferredCulture;

    /// <summary>
    /// 首选文化, 未设置实例值时使用 <see cref="ValueConvertersConfig.DefaultPreferredCulture"/>
    /// </summary>
    public PreferredCulture PreferredCulture
    {
        get => _preferredCulture ?? ValueConvertersConfig.DefaultPreferredCulture;
        set => _preferredCulture = value;
    }

    /// <summary>
    /// 根据 <see cref="PreferredCulture"/> 和 <paramref name="converterCulture"/> 选择有效文化.
    /// </summary>
    /// <param name="converterCulture">转换器文化</param>
    /// <returns>文化</returns>
    public CultureInfo SelectCulture(CultureInfo? converterCulture) =>
        ValueConvertersConfig.SelectCulture(PreferredCulture, converterCulture);
}
