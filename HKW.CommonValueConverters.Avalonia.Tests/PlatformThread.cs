using Avalonia;
using Avalonia.Headless;
using Avalonia.Threading;

[assembly: DoNotParallelize]

namespace HKW.CommonValueConverters.Avalonia.Tests;

public sealed class HeadlessTestApp : Application
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<HeadlessTestApp>().UseHeadless(new AvaloniaHeadlessPlatformOptions());
}

[TestClass]
public sealed class PlatformThread
{
    private static HeadlessUnitTestSession? _session;

    [AssemblyInitialize]
    public static void Initialize(TestContext context) =>
        _session = HeadlessUnitTestSession.StartNew(typeof(HeadlessTestApp));

    [AssemblyCleanup]
    public static async Task Cleanup()
    {
        if (_session is not null)
            await _session.DisposeAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(30));
    }

    public static async Task Run(Action action)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        await _session!.Dispatch(action, timeout.Token).WaitAsync(timeout.Token);
    }

    public static void Drain() => Dispatcher.UIThread.RunJobs();

    internal static HeadlessUnitTestSession Session => _session!;
}
