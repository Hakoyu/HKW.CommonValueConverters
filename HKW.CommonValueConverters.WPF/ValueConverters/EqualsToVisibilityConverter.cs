using System.Windows;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// 默认将相等的输入映射为 Visible, 不相等或空输入映射为 Collapsed.
/// 继承可配置的比较规则和可见性映射. 不支持反向转换.
/// </summary>
public class EqualsToVisibilityConverter : EqualsToValueConverter<Visibility>
{
    /// <inheritdoc/>
    public EqualsToVisibilityConverter()
    {
        TrueValue = Visibility.Visible;
        FalseValue = Visibility.Collapsed;
        NullValue = Visibility.Collapsed;
    }
}
