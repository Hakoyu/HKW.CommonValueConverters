using System.Collections.Concurrent;

[assembly: DoNotParallelize]

namespace HKW.PlatformConverters.Avalonia.Tests;

internal static class ConverterResults
{
    internal static object UnsetValue => global::Avalonia.AvaloniaProperty.UnsetValue;
    internal static object DoNothing => global::Avalonia.Data.BindingOperations.DoNothing;
}

internal static class PlatformThread
{
    private static readonly BlockingCollection<Action> Work = new();
    private static readonly TaskCompletionSource Ready = new(
        TaskCreationOptions.RunContinuationsAsynchronously
    );
    private static readonly Thread Worker = Start();

    private static Thread Start()
    {
        var thread = new Thread(() =>
        {
            try
            {
                global::Avalonia.Threading.Dispatcher.UIThread.VerifyAccess();
                Ready.SetResult();
                foreach (var action in Work.GetConsumingEnumerable())
                    action();
            }
            catch (Exception exception)
            {
                Ready.TrySetException(exception);
            }
        })
        {
            IsBackground = true,
            Name = "Avalonia converter tests",
        };

        thread.Start();
        return thread;
    }

    internal static void Run(Action action)
    {
        Ready.Task.GetAwaiter().GetResult();
        var completion = new TaskCompletionSource(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        Work.Add(() =>
        {
            try
            {
                action();
                completion.SetResult();
            }
            catch (Exception exception)
            {
                completion.SetException(exception);
            }
        });
        completion.Task.GetAwaiter().GetResult();
    }

    internal static void Stop()
    {
        Work.CompleteAdding();
        Worker.Join();
        Work.Dispose();
    }
}

[TestClass]
public sealed class TestAssembly
{
    [AssemblyCleanup]
    public static void Cleanup() => PlatformThread.Stop();
}
