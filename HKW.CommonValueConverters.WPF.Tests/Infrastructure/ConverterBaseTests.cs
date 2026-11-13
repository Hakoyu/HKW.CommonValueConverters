using System.Windows;
using Core = HKW.CommonValueConverters;

namespace HKW.CommonValueConverters.WPF.Tests;

[TestClass]
public sealed class ConverterBaseTests
{
    [TestMethod]
    public void Conversion_RejectsForeignThreadAccess()
    {
        BoolInverterConverter? converter = null;
        PlatformThread.Run(() => converter = new());
        Assert.IsNotNull(converter);
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            converter.Convert(true, null, null, null)
        );
        Assert.ThrowsExactly<InvalidOperationException>(() =>
            converter.ConvertBack(true, null, null, null)
        );
    }
}
