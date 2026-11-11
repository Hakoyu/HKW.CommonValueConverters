using System.Globalization;
using System.Windows.Media;
using Core = global::HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// 返回纯色笔刷的颜色, 反向转换时将颜色转换为新的可变纯色笔刷.
/// 空值和类型不匹配的输入返回 DefaultResult.
/// </summary>
public class SolidColorBrushToMediaColorConverter : ValueConverterBase
{
    /// <inheritdoc/>
    public SolidColorBrushToMediaColorConverter()
        : base(new BrushToColorCore()) { }

    internal sealed class BrushToColorCore : Core.ValueConverterBase
    {
        protected override object? ConvertCore(
            object? value,
            Type? targetType,
            object? parameter,
            CultureInfo culture
        ) => value is SolidColorBrush brush ? brush.Color : GetDefaultResult();

        protected override object? ConvertBackCore(
            object? value,
            Type? targetType,
            object? parameter,
            CultureInfo culture
        ) => value is Color color ? new SolidColorBrush(color) : GetDefaultResult();
    }
}
