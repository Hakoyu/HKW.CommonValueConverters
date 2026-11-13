using System.Windows;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class CommonDependencyPropertyTests
{
    [TestMethod]
    public void HandleValidation_RejectsForeignWrongTypedAndNullHandles() =>
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
            Assert.ThrowsExactly<ArgumentException>(() => converter.SetCurrentValue(foreign, 1));
            Assert.ThrowsExactly<ArgumentException>(() => converter.GetValue(wrong));
            Assert.ThrowsExactly<ArgumentNullException>(() => converter.GetValue<int>(null!));
        });
}
