using Avalonia;
using Avalonia.Data;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class ConverterBaseTests
{
    [TestMethod]
    public Task DefaultResultPriorityAndDisposalPreserveEffectiveValue() =>
        PlatformThread.Run(() =>
        {
            var converter = new StringToNumberConverter();
            var second = new StringToNumberConverter();
            var styleFallback = new object();
            var localFallback = new object();
            using (
                converter.SetValue(
                    ConverterBase.DefaultResultProperty,
                    styleFallback,
                    BindingPriority.Style
                )
            )
            {
                Assert.AreSame(styleFallback, converter.Convert("invalid", null, null, null));
                converter.DefaultResult = localFallback;
                Assert.AreSame(localFallback, converter.Convert("invalid", null, null, null));
                converter.DefaultResult = null;
                Assert.IsNull(converter.Convert("invalid", null, null, null));
                converter.ClearValue(ConverterBase.DefaultResultProperty);
                Assert.AreSame(styleFallback, converter.Convert("invalid", null, null, null));
                Assert.AreSame(
                    AvaloniaProperty.UnsetValue,
                    second.Convert("invalid", null, null, null)
                );
            }
            Assert.AreSame(
                AvaloniaProperty.UnsetValue,
                converter.Convert("invalid", null, null, null)
            );
        });
}
