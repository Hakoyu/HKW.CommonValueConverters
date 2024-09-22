using System.Windows;
using HKW.CommonValueConverters;

namespace HKW.CommonValueConverters;

/// <summary>
/// 可反转的值转换器基类
/// </summary>
public abstract class InvertibleValueConverterBase : ValueConverterBase
{
    /// <summary>
    /// 是反转的
    /// </summary>
    public Func<bool> GetIsInverted { get; set; } = () => false;
}
