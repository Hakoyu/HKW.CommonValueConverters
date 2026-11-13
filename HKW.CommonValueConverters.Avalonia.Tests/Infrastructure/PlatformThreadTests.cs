using Avalonia.Threading;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class PlatformThreadTests
{
    [TestMethod]
    public Task PlatformConverters_RejectOffThreadConversion() =>
        PlatformThread.Run(() =>
        {
            var converter = new BoolInverterConverter();
            var dispatcher = converter.Dispatcher;
            var uiThreadId = Environment.CurrentManagedThreadId;
            var contextMarker = new AsyncLocal<object?> { Value = new object() };
            Assert.AreSame(global::Avalonia.Threading.Dispatcher.UIThread, dispatcher);
            Assert.IsTrue(dispatcher.CheckAccess());
            Assert.IsTrue(converter.CheckAccess());
            System.Runtime.ExceptionServices.ExceptionDispatchInfo? failure = null;
            var worker = new Thread(() =>
            {
                try
                {
                    Assert.AreNotEqual(uiThreadId, Environment.CurrentManagedThreadId);
                    Assert.IsFalse(Thread.CurrentThread.IsThreadPoolThread);
                    Assert.IsNull(contextMarker.Value);
                    Assert.AreSame(dispatcher, converter.Dispatcher);
                    Assert.IsFalse(dispatcher.CheckAccess());
                    Assert.IsFalse(converter.CheckAccess());
                    Assert.ThrowsExactly<InvalidOperationException>(() =>
                        converter.Convert(true, null, null, null)
                    );
                }
                catch (Exception exception)
                {
                    failure = System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(
                        exception
                    );
                }
            })
            {
                IsBackground = true,
                Name = "Avalonia converter access probe",
            };
            // 同步等待 Task 可能内联执行任务, 独立线程并禁止上下文流动才能保证跨线程验证.
            using (ExecutionContext.SuppressFlow())
                worker.Start();
            Assert.IsTrue(
                worker.Join(TimeSpan.FromSeconds(10)),
                "The off-thread conversion probe did not finish."
            );
            failure?.Throw();
            Assert.IsTrue(dispatcher.CheckAccess());
            Assert.AreEqual(false, converter.Convert(true, null, null, null));
        });

    [TestMethod]
    public async Task Dispatcher_ProcessesPostedWork()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        await Task.Run(
                () =>
                    PlatformThread
                        .Session.Dispatch(
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
