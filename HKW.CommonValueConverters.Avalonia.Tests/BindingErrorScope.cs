using Avalonia.Logging;

namespace HKW.CommonValueConverters.Avalonia.Tests;

public sealed class BindingErrorScope : ILogSink, IDisposable
{
    private readonly ILogSink? _previous = Logger.Sink;
    private readonly List<string> _errors = [];

    public BindingErrorScope() => Logger.Sink = this;

    public bool IsEnabled(LogEventLevel level, string area) =>
        level >= LogEventLevel.Warning && area == LogArea.Binding;

    public void Log(LogEventLevel level, string area, object? source, string messageTemplate)
    {
        if (IsEnabled(level, area))
            _errors.Add(messageTemplate);
    }

    public void Log(
        LogEventLevel level,
        string area,
        object? source,
        string messageTemplate,
        params object?[] propertyValues
    )
    {
        if (IsEnabled(level, area))
            _errors.Add(messageTemplate + " " + string.Join(", ", propertyValues));
    }

    public void Dispose()
    {
        Logger.Sink = _previous;
        Assert.IsEmpty(_errors, string.Join(Environment.NewLine, _errors));
    }
}
