using System.Diagnostics;

namespace HKW.CommonValueConverters.WPF.Tests;

internal sealed class BindingErrors : TraceListener, IDisposable
{
    private readonly List<string> _messages = [];
    private readonly SourceLevels _previous = PresentationTraceSources
        .DataBindingSource
        .Switch
        .Level;

    public BindingErrors()
    {
        PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Error;
        PresentationTraceSources.DataBindingSource.Listeners.Add(this);
    }

    public override void Write(string? message)
    {
        if (message is not null)
            _messages.Add(message);
    }

    public override void WriteLine(string? message) => Write(message);

    public void AssertEmpty() =>
        Assert.IsEmpty(_messages, string.Join(Environment.NewLine, _messages));

    protected override void Dispose(bool disposing)
    {
        PresentationTraceSources.DataBindingSource.Listeners.Remove(this);
        PresentationTraceSources.DataBindingSource.Switch.Level = _previous;
        base.Dispose(disposing);
    }
}
