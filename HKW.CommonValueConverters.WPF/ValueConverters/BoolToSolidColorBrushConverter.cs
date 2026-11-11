using System.Windows.Media;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// 将布尔输入映射为纯色笔刷, 默认真值为白色, 假值为黑色.
/// </summary>
public class BoolToSolidColorBrushConverter : BoolToValueConverter<SolidColorBrush>
{
    /// <inheritdoc/>
    public BoolToSolidColorBrushConverter()
    {
        TrueValue = Brushes.White;
        FalseValue = Brushes.Black;
    }
}
