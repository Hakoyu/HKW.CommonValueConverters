using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;
using HKW.HKWUtils;

namespace HKW.CommonValueConvertersTest;

public static class CalculatorConverterSuite
{
    public sealed class Configuration
    {
        public PreferredCulture? PreferredCulture { get; set; }
        public Func<NumberType>? GetNumberType { get; set; }
        public Func<ArithmeticOperatorType>? GetOperatorType { get; set; }
        public Func<object?>? GetDefaultResult { get; set; }
    }

    public static IEnumerable<object[]> Cases
    {
        get
        {
            yield return
            [
                new SingleConversionCase<Configuration>("CalculatorConverter/0", factory =>
                    CalculatorConverter(factory, "8", 10)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>("CalculatorConverter/1", factory =>
                    CalculatorConverter(factory, "+8", 10)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>("CalculatorConverter/2", factory =>
                    CalculatorConverter(factory, "-8", -6)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>("CalculatorConverter/3", factory =>
                    CalculatorConverter(factory, "*8", 16)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>("CalculatorConverter/4", factory =>
                    CalculatorConverter(factory, "/8", 0)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>("CalculatorConverter/5", factory =>
                    CalculatorConverter(factory, "%8", 2)
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

    private static void CalculatorConverter(
        Func<Configuration, SingleConversionSubject<Configuration>> factory,
        string parameter,
        int expected
    )
    {
        var c = factory(new Configuration());
        Assert.AreEqual(expected, c.Convert(2, null, parameter, null));
    }

    private static void NumericBoundariesReturnDefaultAndPreserveSequentialArithmetic(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var unset = ConverterResults.UnsetValue;
        var single = factory(
            new Configuration { GetOperatorType = () => ArithmeticOperatorType.Division }
        );
        Assert.AreSame(unset, single.Convert(1, null, 0, null));
        Assert.AreSame(unset, single.Convert("bad", null, 2, null));
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
public sealed class CalculatorConverterTests
{
    public static IEnumerable<object[]> Cases => CalculatorConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(CalculatorConverter), ConverterTestLayer.Behavior)]
    public void Behavior(SingleConversionCase<CalculatorConverterSuite.Configuration> scenario) =>
        scenario.Run(Create);

    private static SingleConversionSubject<CalculatorConverterSuite.Configuration> Create(
        CalculatorConverterSuite.Configuration configuration
    )
    {
        var converter = new CalculatorConverter();
        return new(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
                if (settings.GetNumberType is not null)
                    converter.GetNumberType = settings.GetNumberType;
                if (settings.GetOperatorType is not null)
                    converter.GetOperatorType = settings.GetOperatorType;
                if (settings.GetDefaultResult is not null)
                    converter.GetDefaultResult = settings.GetDefaultResult;
            },
            converter.Convert,
            converter.ConvertBack
        );
    }
}
