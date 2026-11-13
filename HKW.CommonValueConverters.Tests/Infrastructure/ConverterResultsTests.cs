using HKW.CommonValueConverters;

namespace HKW.CommonValueConvertersTest;

[TestClass]
[DoNotParallelize]
public sealed class ConverterResultsTests
{
    [TestMethod]
    public void ResultsAreDistinctAndDefaultCanBeOverridden()
    {
        Assert.IsNotNull(ConverterResults.UnsetValue);
        Assert.IsNotNull(ConverterResults.DoNothing);
        Assert.AreNotSame(ConverterResults.UnsetValue, ConverterResults.DoNothing);
        var converter = new StringCaseConverter();
        Assert.AreSame(ConverterResults.UnsetValue, converter.Convert(1, null, null, null));
        converter.GetDefaultResult = () => null;
        Assert.IsNull(converter.Convert(1, null, null, null));
        converter.GetDefaultResult = () => ConverterResults.DoNothing;
        Assert.AreSame(ConverterResults.DoNothing, converter.Convert(1, null, null, null));
    }

    [TestMethod]
    public void PlatformMappingsDoNotShareMutableSentinels()
    {
        var a = new PlatformMapping();
        var b = new PlatformMapping();
        Assert.AreNotSame(a.Unset, b.Unset);
        Assert.AreSame(ConverterResults.UnsetValue, a.ToCore(a.Unset));
        Assert.AreSame(ConverterResults.UnsetValue, b.ToCore(b.Unset));
        Assert.AreSame(a.Unset, a.FromCore(ConverterResults.UnsetValue));
        Assert.AreSame(b.Unset, b.FromCore(ConverterResults.UnsetValue));
        Assert.AreSame(a.DoNothing, a.FromCore(ConverterResults.DoNothing));
        Assert.AreSame(ConverterResults.DoNothing, b.ToCore(b.DoNothing));
        Assert.IsNull(a.FromCore(a.ToCore(null)));
        var ordinary = new object();
        Assert.AreSame(ordinary, b.FromCore(b.ToCore(ordinary)));
    }

    private sealed class PlatformMapping
    {
        public object Unset { get; } = new();
        public object DoNothing { get; } = new();

        public object? ToCore(object? value)
        {
            if (ReferenceEquals(value, Unset))
                return ConverterResults.UnsetValue;
            return ReferenceEquals(value, DoNothing) ? ConverterResults.DoNothing : value;
        }

        public object? FromCore(object? value)
        {
            if (ReferenceEquals(value, ConverterResults.UnsetValue))
                return Unset;
            return ReferenceEquals(value, ConverterResults.DoNothing) ? DoNothing : value;
        }
    }
}
