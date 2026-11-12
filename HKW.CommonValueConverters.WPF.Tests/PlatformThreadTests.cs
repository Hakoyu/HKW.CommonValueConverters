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
        PlatformThread.Run(() =>
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
            PlatformThread.Run(() =>
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
}
