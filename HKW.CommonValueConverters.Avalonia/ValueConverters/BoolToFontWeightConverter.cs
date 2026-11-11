using System.Windows;
using Avalonia.Media;

namespace HKW.CommonValueConverters.Avalonia;

/// <summary>
/// 将布尔输入映射为字体粗细, 默认真值为粗体, 假值为正常粗细
/// </summary>
public class BoolToFontWeightConverter : BoolToValueConverter<FontWeight>
{
    /// <inheritdoc/>
    public BoolToFontWeightConverter()
    {
        TrueValue = FontWeight.Bold;
        FalseValue = FontWeight.Normal;
    }
}
