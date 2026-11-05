using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 多值转换器
/// </summary>
public abstract class MultiValueConverterBase : CommonConverterBase
{
    /// <summary>使用所选文化执行多值转换。</summary>
    public object? Convert(IList<object?> values, Type? targetType, object? parameter, CultureInfo? culture) =>
        ConvertCore(values, targetType, parameter, SelectCulture(culture));

    /// <summary>使用所选文化执行反向转换，包装器按项映射结果。</summary>
    public object[] ConvertBack(object? value, IList<Type?> targetTypes, object? parameter, CultureInfo? culture) =>
        ConvertBackCore(value, targetTypes, parameter, SelectCulture(culture));

    /// <summary>实现多值转换，culture 已解析且非空。</summary>
    protected abstract object? ConvertCore(IList<object?> values, Type? targetType, object? parameter, CultureInfo culture);

    /// <summary>实现反向转换，默认不支持。</summary>
    protected virtual object[] ConvertBackCore(
        object? value,
        IList<Type?> targetTypes,
        object? parameter,
        CultureInfo culture
    )
    {
        throw new NotSupportedException(
            $"Converter '{GetType().FullName}' does not support backward conversion."
        );
    }
}
