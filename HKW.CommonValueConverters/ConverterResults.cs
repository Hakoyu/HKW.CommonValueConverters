namespace HKW.CommonValueConverters;

/// <summary>
/// 平台无关的转换结果, 包装器负责与平台绑定哨兵进行映射.
/// </summary>
public static class ConverterResults
{
    /// <summary>
    /// 输入尚未就绪或转换失败; 不同于合法的 <see langword="null"/>.
    /// </summary>
    public static object UnsetValue { get; } = new();

    /// <summary>
    /// 保持目标值, 不执行更新. 不同于表示失败的 <see cref="UnsetValue"/>.
    /// </summary>
    public static object DoNothing { get; } = new();
}
