using System.Runtime.ExceptionServices;
using System.Windows.Threading;

namespace HKW.CommonValueConverters.WPF.Tests;

internal static class PlatformThread
{
    public static void Run(Action action)
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
            Name = "WPF converter test",
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
