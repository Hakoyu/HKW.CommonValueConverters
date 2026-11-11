using System.Windows;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// 将布尔输入映射为可配置的边距, 默认回退值为零边距.
/// </summary>
public class BoolToThicknessConverter : BoolToValueConverter<Thickness>
{
    /// <inheritdoc/>
    public BoolToThicknessConverter()
    {
        TrueValue = new Thickness(5);
        FalseValue = new Thickness(0);
        NullValue = new Thickness(0);
        DefaultResult = new Thickness(0);
    }
}
