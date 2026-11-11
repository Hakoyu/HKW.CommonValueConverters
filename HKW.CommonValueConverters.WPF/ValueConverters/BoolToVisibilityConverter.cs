using System.Windows;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// 将布尔输入映射为可见性, 默认真值为 Visible, 假值为 Collapsed.
/// 继承可配置的布尔映射. 不支持反向转换.
/// </summary>
public class BoolToVisibilityConverter : BoolToValueConverter<Visibility>
{
    /// <summary>
    /// 初始化可见和折叠状态的映射.
    /// </summary>
    public BoolToVisibilityConverter()
    {
        TrueValue = Visibility.Visible;
        FalseValue = Visibility.Collapsed;
        NullValue = Visibility.Collapsed;
    }
}
