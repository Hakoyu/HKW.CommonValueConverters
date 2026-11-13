using System.Runtime.ExceptionServices;
using System.Windows.Threading;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class PlatformThread
{
    private static readonly object SyncRoot = new();
    private static Thread? _thread;
    private static Dispatcher? _dispatcher;

    [AssemblyCleanup]
    public static void Cleanup()
    {
        Dispatcher? dispatcher;
        Thread? thread;
        lock (SyncRoot)
        {
            dispatcher = _dispatcher;
            thread = _thread;
            _dispatcher = null;
            _thread = null;
        }
        if (dispatcher is not null && dispatcher.HasShutdownFinished is false)
            dispatcher.BeginInvokeShutdown(DispatcherPriority.Send);
        thread?.Join(TimeSpan.FromSeconds(10));
    }

    public static void Run(Action action)
    {
        var dispatcher = EnsureDispatcher();
        if (dispatcher.CheckAccess())
        {
            action();
            return;
        }
        Exception? failure = null;
        var operation = dispatcher.BeginInvoke(
            DispatcherPriority.Normal,
            new Action(() =>
            {
                try
                {
                    action();
                }
                catch (Exception exception)
                {
                    failure = exception;
                }
                finally
                {
                    Drain();
                }
            })
        );
        if (operation.Wait(TimeSpan.FromSeconds(30)) != DispatcherOperationStatus.Completed)
        {
            ResetBrokenDispatcher(dispatcher);
            throw new TimeoutException("WPF test exceeded its 30 second deadline.");
        }
        if (failure is not null)
            ExceptionDispatchInfo.Capture(failure).Throw();
    }

    public static void RunIsolated(Action action)
    {
        Exception? failure = null;
        Dispatcher? dispatcher = null;
        var thread = new Thread(() =>
        {
            dispatcher = Dispatcher.CurrentDispatcher;
            dispatcher.BeginInvoke(() =>
            {
                try
                {
                    action();
                }
                catch (Exception exception)
                {
                    failure = exception;
                }
                finally
                {
                    dispatcher.InvokeShutdown();
                }
            });
            Dispatcher.Run();
        })
        {
            IsBackground = true,
            Name = "WPF converter isolated test",
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        if (thread.Join(TimeSpan.FromSeconds(30)) is false)
        {
            dispatcher?.BeginInvokeShutdown(DispatcherPriority.Send);
            throw new TimeoutException("WPF test exceeded its 30 second deadline.");
        }
        if (failure is not null)
            ExceptionDispatchInfo.Capture(failure).Throw();
    }

    private static Dispatcher EnsureDispatcher()
    {
        lock (SyncRoot)
        {
            if (
                _dispatcher is { HasShutdownStarted: false, HasShutdownFinished: false } existing
                && _thread is { IsAlive: true }
            )
                return existing;
            using var ready = new ManualResetEventSlim(false);
            Dispatcher? created = null;
            var thread = new Thread(() =>
            {
                created = Dispatcher.CurrentDispatcher;
                created.BeginInvoke(DispatcherPriority.Send, new Action(ready.Set));
                Dispatcher.Run();
            })
            {
                IsBackground = true,
                Name = "WPF converter test",
            };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            if (ready.Wait(TimeSpan.FromSeconds(10)) is false || created is null)
                throw new TimeoutException("WPF STA dispatcher failed to start.");
            _thread = thread;
            _dispatcher = created;
            return created;
        }
    }

    private static void ResetBrokenDispatcher(Dispatcher broken)
    {
        lock (SyncRoot)
        {
            if (ReferenceEquals(_dispatcher, broken))
            {
                _dispatcher = null;
                _thread = null;
            }
        }
        broken.BeginInvokeShutdown(DispatcherPriority.Send);
    }

    public static void Drain()
    {
        var frame = new DispatcherFrame();
        var timedOut = false;
        var timer = new DispatcherTimer(DispatcherPriority.Send)
        {
            Interval = TimeSpan.FromSeconds(5),
        };
        timer.Tick += (_, _) =>
        {
            timedOut = true;
            frame.Continue = false;
        };
        timer.Start();
        Dispatcher.CurrentDispatcher.BeginInvoke(
            DispatcherPriority.ContextIdle,
            new Action(() => frame.Continue = false)
        );
        try
        {
            Dispatcher.PushFrame(frame);
        }
        finally
        {
            timer.Stop();
        }
        if (timedOut)
            throw new TimeoutException("WPF binding queue did not drain.");
    }
}
