using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class NullToTrueConverterSuite
{
    public sealed class Configuration
    {
        public PreferredCulture? PreferredCulture { get; set; }
        public Func<bool>? GetIsInverted { get; set; }
        public Func<object?>? GetDefaultResult { get; set; }
    }

    public static IEnumerable<object[]> Cases
    {
        get
        {
            yield return
            [
                new SingleConversionCase<Configuration>("NullToTrueConverter", factory =>
                    NullToTrueConverter(factory)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "UnavailableInputsAreNotConvertedIntoOrdinaryValues",
                    factory => UnavailableInputsAreNotConvertedIntoOrdinaryValues(factory)
                ),
            ];
        }
    }

    private static void NullToTrueConverter(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());

        Assert.AreEqual(true, c.Convert(null, null, null, null));
        Assert.AreEqual(false, c.Convert("Value", null, null, null));

        c.Configuration.GetIsInverted = () => true;

        Assert.AreEqual(false, c.Convert(null, null, null, null));
        Assert.AreEqual(true, c.Convert("Value", null, null, null));
    }

    private static void UnavailableInputsAreNotConvertedIntoOrdinaryValues(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var fallback = new object();
        var c = factory(new Configuration { GetDefaultResult = () => fallback });
        foreach (var sentinel in new[] { ConverterResults.UnsetValue, ConverterResults.DoNothing })
            Assert.AreSame(fallback, c.Convert(sentinel, null, null, null));
    }
}

[TestClass]
public sealed class NullToTrueConverterTests
{
    public static IEnumerable<object[]> Cases => NullToTrueConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(NullToTrueConverter), ConverterTestLayer.Behavior)]
    public void Behavior(SingleConversionCase<NullToTrueConverterSuite.Configuration> scenario) =>
        scenario.Run(Create);

    private static SingleConversionSubject<NullToTrueConverterSuite.Configuration> Create(
        NullToTrueConverterSuite.Configuration configuration
    )
    {
        var converter = new NullToTrueConverter();
        return new(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
                if (settings.GetIsInverted is not null)
                    converter.GetIsInverted = settings.GetIsInverted;
                if (settings.GetDefaultResult is not null)
                    converter.GetDefaultResult = settings.GetDefaultResult;
            },
            converter.Convert,
            converter.ConvertBack
        );
    }

    [TestMethod]
    public void UnsupportedReverseConversionRemainsExplicit()
    {
        Assert.ThrowsExactly<NotSupportedException>(() =>
            new NullToTrueConverter().ConvertBack(null, null, null, null)
        );
    }
}
