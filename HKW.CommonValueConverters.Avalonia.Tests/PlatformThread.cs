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
        await Task.Run(
                () => _session!.Dispatch(action, timeout.Token).GetAwaiter().GetResult(),
                timeout.Token
            )
            .WaitAsync(timeout.Token);
    }

    public static void Drain() => Dispatcher.UIThread.RunJobs();

    [TestMethod]
    public async Task Dispatcher_ProcessesPostedWork()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        await Task.Run(
                () =>
                    _session!
                        .Dispatch(
                            async () =>
                            {
                                var completion = new TaskCompletionSource(
                                    TaskCreationOptions.RunContinuationsAsynchronously
                                );
                                Dispatcher.UIThread.Post(() => completion.SetResult());
                                await completion.Task.WaitAsync(timeout.Token);
                                return true;
                            },
                            timeout.Token
                        )
                        .GetAwaiter()
                        .GetResult(),
                timeout.Token
            )
            .WaitAsync(timeout.Token);
    }
}
