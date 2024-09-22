using System.ComponentModel;
using System.Windows;
using HKW.CommonValueConverters;

namespace HKW.CommonValueConverters;

/// <summary>
/// 布尔到布尔参数转换器
/// <para>示例:
/// <code><![CDATA[
/// <Binding Bool, Converter="{StaticResource BoolToParameterDoubleConverter}" ConverterParameter="1,2"/>
/// return: Bool ? 1 : 2
/// ]]></code></para>
/// </summary>
public abstract class BoolToSplitParameterConverterBase : ValueConverterBase
{
    /// <summary>
    /// 分割符
    /// </summary>
    [DefaultValue(',')]
    public Func<char> GetSeparator { get; set; } = () => ',';
}
