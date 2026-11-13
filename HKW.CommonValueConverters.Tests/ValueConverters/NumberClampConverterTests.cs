using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;
using HKW.HKWUtils;

namespace HKW.CommonValueConvertersTest;

public static class NumberClampConverterSuite
{
    public sealed class Configuration
    {
        public PreferredCulture? PreferredCulture { get; set; }
        public Func<NumberType>? GetNumberType { get; set; }
        public Func<object?>? GetDefaultResult { get; set; }
    }

    public static IEnumerable<object[]> Cases
    {
        get
        {
            yield return
            [
                new SingleConversionCase<Configuration>("NumberClampConverter", factory =>
                    NumberClampConverter(factory)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "NumericBoundariesReturnDefaultAndPreserveSequentialArithmetic",
                    factory =>
                        NumericBoundariesReturnDefaultAndPreserveSequentialArithmetic(factory)
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

    private static void NumberClampConverter(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());

        Assert.AreEqual(true, c.Convert(5, null, "2,8", null));
    }

    private static void NumericBoundariesReturnDefaultAndPreserveSequentialArithmetic(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var unset = ConverterResults.UnsetValue;
        var clamp = factory(new Configuration());
        foreach (var parameter in new[] { "2", "2,3,4", "x,4", "8,2" })
            Assert.AreSame(unset, clamp.Convert(3, null, parameter, null));
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
public sealed class NumberClampConverterTests
{
    public static IEnumerable<object[]> Cases => NumberClampConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(NumberClampConverter), ConverterTestLayer.Behavior)]
    public void Behavior(SingleConversionCase<NumberClampConverterSuite.Configuration> scenario) =>
        scenario.Run(Create);

    private static SingleConversionSubject<NumberClampConverterSuite.Configuration> Create(
        NumberClampConverterSuite.Configuration configuration
    )
    {
        var converter = new NumberClampConverter();
        return new(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
                if (settings.GetNumberType is not null)
                    converter.GetNumberType = settings.GetNumberType;
                if (settings.GetDefaultResult is not null)
                    converter.GetDefaultResult = settings.GetDefaultResult;
            },
            converter.Convert,
            converter.ConvertBack
        );
    }
}
