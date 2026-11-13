using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.Avalonia.Tests;

[TestClass]
public sealed class CommonDependencyPropertyTests
{
    [TestMethod]
    public Task InvalidHandlesAreRejected() =>
        PlatformThread.Run(() =>
        {
            var converter = new BoolToValueConverter<int>();
            Assert.ThrowsExactly<ArgumentNullException>(() => converter.GetValue<int>(null!));
            Assert.ThrowsExactly<ArgumentException>(() =>
                converter.GetValue(new Core.CommonDependencyProperty<int>(new object()))
            );
        });
}
