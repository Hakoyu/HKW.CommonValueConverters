namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// 默认判断所有输入是否均等于 true, 空集合返回 true.
/// </summary>
public class AllIsTrueMultiConverter : AllEqualsMultiConverter<bool>
{
    /// <inheritdoc/>
    public AllIsTrueMultiConverter()
    {
        Value = true;
    }
}
