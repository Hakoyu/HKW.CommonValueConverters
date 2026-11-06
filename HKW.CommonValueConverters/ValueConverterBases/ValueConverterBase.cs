using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 值转换器基类
/// </summary>
public abstract class ValueConverterBase : CommonConverterBase
{
    /// <summary>
    /// 使用 <see cref="CommonConverterBase.SelectCulture"/> 选择文化并执行正向转换.
    /// </summary>
    public object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    ) => ConvertCore(value, targetType, parameter, SelectCulture(culture));

    /// <summary>
    /// 使用 <see cref="CommonConverterBase.SelectCulture"/> 选择文化并执行反向转换.
    /// </summary>
    public object? ConvertBack(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    ) => ConvertBackCore(value, targetType, parameter, SelectCulture(culture));

    /// <summary>
    /// 实现正向转换, <paramref name="culture"/> 已解析且非空.
    /// </summary>
    protected abstract object? ConvertCore(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo culture
    );

    /// <summary>
    /// 实现反向转换, 默认抛出 <see cref="NotSupportedException"/>.
    /// </summary>
    protected virtual object? ConvertBackCore(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        throw new NotSupportedException(
            $"Converter '{GetType().FullName}' does not support backward conversion."
        );
    }
}
