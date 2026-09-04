namespace HKW.CommonValueConverters;

/// <summary>
/// 转换器异常
/// </summary>
public class ConverterException : Exception
{
    /// <inheritdoc/>
    /// <param name="message">消息</param>
    public ConverterException(string message)
        : base(message) { }
}
