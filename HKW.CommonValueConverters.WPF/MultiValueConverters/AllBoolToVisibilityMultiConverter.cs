using System.Windows;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// 默认将全真或空集合输入映射为 Visible, 其他输入映射为 Collapsed.
/// </summary>
public class AllBoolToVisibilityMultiConverter : AllBoolToValueMultiConverter<Visibility>
{
    /// <summary>
    /// 初始化可见性映射, 并将默认回退值设为折叠状态.
    /// </summary>
    public AllBoolToVisibilityMultiConverter()
    {
        TrueValue = Visibility.Visible;
        FalseValue = Visibility.Collapsed;
        NullValue = Visibility.Collapsed;
        DefaultResult = Visibility.Collapsed;
    }
}
