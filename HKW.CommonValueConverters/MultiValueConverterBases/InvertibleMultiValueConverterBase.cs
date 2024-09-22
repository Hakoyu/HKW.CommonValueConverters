using System.Windows;

namespace HKW.CommonValueConverters;

/// <summary>
/// 可反转的多值转换器
/// </summary>

public abstract class InvertibleMultiValueConverterBase : MultiValueConverterBase
{
    /// <summary>
    /// 是反转的
    /// </summary>
    public Func<bool> GetIsInverted { get; set; } = () => false;
}
