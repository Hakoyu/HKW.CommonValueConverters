using System.Globalization;

namespace HKW.CommonValueConverters;

/// <summary>
/// 字符串到集合转换器
/// </summary>
public class StringSplitToCollectionConverter<TCollection> : ValueConverterBase
    where TCollection : ICollection<string>
{
    /// <summary>
    /// 默认分隔符
    /// </summary>
    public const string DefaultSeparator = " ";

    /// <summary>
    /// 获取数字风格
    /// </summary>
    public Func<string> GetSeparator { get; set; } = () => DefaultSeparator;

    /// <summary>
    /// 获转换到集合
    /// </summary>
    public required Func<string[], TCollection> ToCollection { get; init; }

    /// <inheritdoc/>
    public override object? Convert(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        var separator = GetSeparator();
        if (separator is string str)
            separator = str;
        if (value is null)
            return GetDefaultResult();
        return value.ToString()!.Split(separator);
    }

    /// <inheritdoc/>
    public override object? ConvertBack(
        object? value,
        Type? targetType,
        object? parameter,
        CultureInfo? culture
    )
    {
        if (value is not TCollection collection)
            return GetDefaultResult();
        return string.Join(GetSeparator(), collection);
    }
}
