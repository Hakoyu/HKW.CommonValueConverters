using System;
using System.ComponentModel;
using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
///  通用附加属性
/// </summary>
public class CommonDependencyProperty<T>(object dependencyProperty)
{
    /// <summary>
    ///  附加属性
    /// </summary>
    public object Value { get; set; } = dependencyProperty;
}

/// <summary>
/// 通用值转换器
/// </summary>
public interface ICommonValueConverter
{
    /// <summary>
    /// 首选文化
    /// </summary>
    public PreferredCulture PreferredCulture { get; set; }

    /// <summary>
    /// 获取值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="dependencyProperty">附加属性</param>
    /// <returns>值</returns>
    public T GetValue<T>(CommonDependencyProperty<T> dependencyProperty);

    /// <summary>
    /// 设置值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="dependencyProperty">附加属性</param>
    /// <param name="value">值</param>
    public void SetValue<T>(CommonDependencyProperty<T> dependencyProperty, T value);
}

/// <summary>
/// 转换器基类
/// </summary>
public abstract class CommonConverterBase : ICommonValueConverter
{
    /// <summary>
    /// 默认未设置值
    /// </summary>
    [DefaultValue(null)]
    public static object UnsetValue { get; set; } = null!;

    /// <summary>
    /// 全局默认结果
    /// </summary>
    [DefaultValue(null)]
    public static object? GlobalDefaultResult { get; set; } = null;

    /// <summary>
    /// 默认结果
    /// <para>当转换器未成功执行时,返回此值</para>
    /// </summary>
    public Func<object?> GetDefaultResult { get; set; } = () => GlobalDefaultResult;

    private PreferredCulture? _preferredCulture;

    /// <summary>
    /// 首选文化
    /// </summary>
    public PreferredCulture PreferredCulture
    {
        get => _preferredCulture ?? ValueConvertersConfig.DefaultPreferredCulture;
        set => _preferredCulture = value;
    }

    T ICommonValueConverter.GetValue<T>(CommonDependencyProperty<T> dependencyProperty)
    {
        throw new NotImplementedException();
    }

    void ICommonValueConverter.SetValue<T>(CommonDependencyProperty<T> dependencyProperty, T value)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 选择文化
    /// </summary>
    /// <param name="converterCulture">转换器文化</param>
    /// <returns>文化</returns>
    public CultureInfo SelectCulture(Func<CultureInfo> converterCulture)
    {
        return PreferredCulture switch
        {
            PreferredCulture.CurrentCulture => CultureInfo.CurrentCulture,
            PreferredCulture.CurrentUICulture => CultureInfo.CurrentUICulture,
            _ => converterCulture(),
        };
    }
}
