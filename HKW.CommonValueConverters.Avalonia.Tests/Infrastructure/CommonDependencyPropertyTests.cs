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
            var foreign = new Core.CommonDependencyProperty<int>(new object());
            var wrong = new Core.CommonDependencyProperty<string>(
                BoolToValueConverter<int>.TrueValueProperty.Value
            );
            Assert.ThrowsExactly<ArgumentException>(() => converter.GetValue(foreign));
            Assert.ThrowsExactly<ArgumentException>(() => converter.SetValue(foreign, 1));
            Assert.ThrowsExactly<ArgumentException>(() => converter.ClearValue(foreign));
            Assert.ThrowsExactly<ArgumentException>(() => converter.GetValue(wrong));
            Assert.ThrowsExactly<ArgumentException>(() => converter.SetValue(wrong, "1"));
            Assert.ThrowsExactly<ArgumentException>(() => converter.ClearValue(wrong));
            Assert.ThrowsExactly<ArgumentNullException>(() => converter.GetValue<int>(null!));
            Assert.ThrowsExactly<ArgumentNullException>(() => converter.SetValue<int>(null!, 1));
            Assert.ThrowsExactly<ArgumentNullException>(() => converter.ClearValue<int>(null!));
        });
}
