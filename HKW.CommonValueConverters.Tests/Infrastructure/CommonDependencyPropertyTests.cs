using HKW.CommonValueConverters;

namespace HKW.CommonValueConvertersTest;

[TestClass]
public sealed class CommonDependencyPropertyTests
{
    [TestMethod]
    public void StoresNativePropertyIdentityWithoutPlatformDependency()
    {
        var native = new object();
        var property = new CommonDependencyProperty<int>(native);
        Assert.AreSame(native, property.Value);
        Assert.IsFalse(
            typeof(CommonDependencyProperty<int>).GetProperty(nameof(property.Value))!.CanWrite
        );
    }

    [TestMethod]
    public void RejectsNullNativeProperty()
    {
        var error = Assert.ThrowsExactly<ArgumentNullException>(() =>
            new CommonDependencyProperty<string>(null!)
        );
        Assert.AreEqual("value", error.ParamName);
    }
}
