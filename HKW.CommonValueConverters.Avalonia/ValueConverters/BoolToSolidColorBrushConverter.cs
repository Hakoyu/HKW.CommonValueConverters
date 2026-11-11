using Avalonia.Media;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// 将布尔输入映射为纯色笔刷, 默认真值为白色, 假值为黑色.
/// </summary>
public class BoolToSolidColorBrushConverter : BoolToValueConverter<ISolidColorBrush>
{
    /// <inheritdoc/>
    public BoolToSolidColorBrushConverter()
    {
        TrueValue = Brushes.White;
        FalseValue = Brushes.Black;
    }
}
