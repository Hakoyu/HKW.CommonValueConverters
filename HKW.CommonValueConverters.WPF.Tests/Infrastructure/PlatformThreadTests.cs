using System.Windows.Threading;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class PlatformThreadTests
{
    [TestMethod]
    public void Dispatcher_RunsStaQueueAndTerminatesAfterCleanup()
    {
        Thread? worker = null;
        Dispatcher? dispatcher = null;
        PlatformThread.RunIsolated(() =>
        {
            worker = Thread.CurrentThread;
            dispatcher = Dispatcher.CurrentDispatcher;
            Assert.AreEqual(ApartmentState.STA, worker.GetApartmentState());
            var executed = false;
            dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() => executed = true));
            PlatformThread.Drain();
            Assert.IsTrue(executed);
        });
        Assert.IsNotNull(worker);
        Assert.IsNotNull(dispatcher);
        Assert.IsFalse(worker.IsAlive);
        Assert.IsTrue(dispatcher.HasShutdownFinished);
    }

    [TestMethod]
    public void Dispatcher_ReturnsOriginalFailureAndStopsThread()
    {
        Thread? worker = null;
        var failure = new InvalidOperationException("Expected test failure.");
        var actual = Assert.ThrowsExactly<InvalidOperationException>(() =>
            PlatformThread.RunIsolated(() =>
            {
                worker = Thread.CurrentThread;
                throw failure;
            })
        );
        Assert.AreSame(failure, actual);
        Assert.IsNotNull(worker);
        Assert.IsFalse(worker.IsAlive);
        PlatformThread.Run(() => Assert.IsTrue(Dispatcher.CurrentDispatcher.CheckAccess()));
    }

    [TestMethod]
    public void SharedDispatcher_ReusesStaThreadAndRecoversAfterFailure()
    {
        Thread? first = null;
        Thread? second = null;
        PlatformThread.Run(() =>
        {
            first = Thread.CurrentThread;
            Assert.AreEqual(ApartmentState.STA, first.GetApartmentState());
        });
        var failure = new InvalidOperationException("Expected shared thread failure.");
        var actual = Assert.ThrowsExactly<InvalidOperationException>(() =>
            PlatformThread.Run(() => throw failure)
        );
        Assert.AreSame(failure, actual);
        PlatformThread.Run(() => second = Thread.CurrentThread);
        Assert.IsNotNull(first);
        Assert.AreSame(first, second);
    }
}
