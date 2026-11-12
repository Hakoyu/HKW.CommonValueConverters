namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// 保留早期 Avalonia 包中错误命名空间下的公开类型及核心委托配置契约.
/// 新代码可使用 Avalonia 命名空间下支持平台属性和绑定的同名转换器.
/// 此兼容类型不依赖 WPF 程序集, 也不继承 Avalonia 平台转换器基类.
/// </summary>
public class BoolToStyleConverter
    : global::HKW.CommonValueConverters.BoolToValueConverter<global::Avalonia.Styling.Style> { }
