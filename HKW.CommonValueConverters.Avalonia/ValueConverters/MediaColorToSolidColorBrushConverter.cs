using System.Globalization;
using Avalonia.Media;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// 将颜色转换为新的可变纯色笔刷, 反向转换时返回纯色笔刷的颜色.
/// 空值和类型不匹配的输入返回 DefaultResult.
/// </summary>
public class MediaColorToSolidColorBrushConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public MediaColorToSolidColorBrushConverter()
        : base(new ColorToBrushCore()) { }

    internal sealed class ColorToBrushCore : Core.ValueConverterBase
    {
        protected override object? ConvertCore(
            object? value,
            Type? targetType,
            object? parameter,
            CultureInfo culture
        ) => value is Color color ? new SolidColorBrush(color) : GetDefaultResult();

        protected override object? ConvertBackCore(
            object? value,
            Type? targetType,
            object? parameter,
            CultureInfo culture
        ) => value is SolidColorBrush brush ? brush.Color : GetDefaultResult();
    }
}
