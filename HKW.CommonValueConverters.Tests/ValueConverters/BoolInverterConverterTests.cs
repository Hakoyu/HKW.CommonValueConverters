using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class BoolInverterConverterSuite
{
    public sealed class Configuration
    {
        public PreferredCulture? PreferredCulture { get; set; }
        public Func<bool>? GetNullValue { get; set; }
        public Func<object?>? GetDefaultResult { get; set; }
    }

    public static IEnumerable<object[]> Cases
    {
        get
        {
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "BoolInverterConverter_InvertsBoolean/0",
                    factory => BoolInverterConverter_InvertsBoolean(factory, true, false)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "BoolInverterConverter_InvertsBoolean/1",
                    factory => BoolInverterConverter_InvertsBoolean(factory, false, true)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "BoolInverterConverter_NullAndInvalidInputs",
                    factory => BoolInverterConverter_NullAndInvalidInputs(factory)
                ),
            ];
        }
    }

    private static void BoolInverterConverter_InvertsBoolean(
        Func<Configuration, SingleConversionSubject<Configuration>> factory,
        bool value,
        bool expected
    )
    {
        var c = factory(new Configuration());

        Assert.AreEqual(expected, c.Convert(value, null, null, null));
    }

    private static void BoolInverterConverter_NullAndInvalidInputs(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var fallback = new object();
        var c = factory(new Configuration { GetDefaultResult = () => fallback });

        Assert.AreEqual(false, c.Convert(null, null, null, null));
        c.Configuration.GetNullValue = () => true;
        Assert.AreEqual(true, c.Convert(null, null, null, null));
        foreach (
            var value in new object[]
            {
                "true",
                1,
                new object(),
                ConverterResults.UnsetValue,
                ConverterResults.DoNothing,
            }
        )
            Assert.AreSame(fallback, c.Convert(value, null, null, null));
    }
}

[TestClass]
public sealed class BoolInverterConverterTests
{
    public static IEnumerable<object[]> Cases => BoolInverterConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(BoolInverterConverter), ConverterTestLayer.Behavior)]
    public void Behavior(SingleConversionCase<BoolInverterConverterSuite.Configuration> scenario) =>
        scenario.Run(Create);

    private static SingleConversionSubject<BoolInverterConverterSuite.Configuration> Create(
        BoolInverterConverterSuite.Configuration configuration
    )
    {
        var converter = new BoolInverterConverter();
        return new(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
                if (settings.GetNullValue is not null)
                    converter.GetNullValue = settings.GetNullValue;
                if (settings.GetDefaultResult is not null)
                    converter.GetDefaultResult = settings.GetDefaultResult;
            },
            converter.Convert,
            converter.ConvertBack
        );
    }
}
