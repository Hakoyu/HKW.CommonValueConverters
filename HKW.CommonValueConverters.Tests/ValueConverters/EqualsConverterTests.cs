using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class EqualsConverterSuite
{
    public sealed class Configuration
    {
        public PreferredCulture? PreferredCulture { get; set; }
        public Func<object?>? GetOther { get; set; }
        public Func<bool>? GetIsStringEquals { get; set; }
        public Func<bool>? GetIsInverted { get; set; }
        public Func<object?>? GetDefaultResult { get; set; }
    }

    public static IEnumerable<object[]> Cases
    {
        get
        {
            yield return
            [
                new SingleConversionCase<Configuration>("EqualsConverter", factory =>
                    EqualsConverter(factory)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>("EqualsConverter_GetOther", factory =>
                    EqualsConverter_GetOther(factory)
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

    private static void EqualsConverter(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());

        Assert.AreEqual(false, c.Convert("Other", null, null, null));
        Assert.AreEqual(true, c.Convert("Parameter", null, "Parameter", null));
        Assert.AreEqual(true, c.Convert(null, null, null, null));
    }

    private static void EqualsConverter_GetOther(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration { GetOther = () => "Value" });

        Assert.AreEqual(true, c.Convert("Value", null, null, null));
        Assert.AreEqual(false, c.Convert("Other", null, null, null));
        Assert.AreEqual(true, c.Convert("Parameter", null, "Parameter", null));
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
public sealed class EqualsConverterTests
{
    public static IEnumerable<object[]> Cases => EqualsConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(EqualsConverter), ConverterTestLayer.Behavior)]
    public void Behavior(SingleConversionCase<EqualsConverterSuite.Configuration> scenario) =>
        scenario.Run(Create);

    private static SingleConversionSubject<EqualsConverterSuite.Configuration> Create(
        EqualsConverterSuite.Configuration configuration
    )
    {
        var converter = new EqualsConverter();
        return new(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
                if (settings.GetOther is not null)
                    converter.GetOther = settings.GetOther;
                if (settings.GetIsStringEquals is not null)
                    converter.GetIsStringEquals = settings.GetIsStringEquals;
                if (settings.GetIsInverted is not null)
                    converter.GetIsInverted = settings.GetIsInverted;
                if (settings.GetDefaultResult is not null)
                    converter.GetDefaultResult = settings.GetDefaultResult;
            },
            converter.Convert,
            converter.ConvertBack
        );
    }
}
