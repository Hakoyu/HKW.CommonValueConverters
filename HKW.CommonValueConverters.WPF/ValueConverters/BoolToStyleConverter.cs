using System.Windows;

namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// 将布尔输入映射为可配置的样式. 各分支的映射初始均为 null.
/// 继承可配置的布尔映射. 不支持反向转换.
/// </summary>
public class BoolToStyleConverter : BoolToValueConverter<Style> { }
