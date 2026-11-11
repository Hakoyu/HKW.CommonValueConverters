namespace HKW.CommonValueConverters.WPF;

/// <summary>
/// 默认将空值映射为 true, 将两种布尔结果均映射为 false.
/// </summary>
public class NullToTrueConverter : BoolToValueConverter<bool>
{
    /// <inheritdoc/>
    public NullToTrueConverter()
    {
        TrueValue = false;
        FalseValue = false;
        NullValue = true;
        IsNullable = true;
    }
}
