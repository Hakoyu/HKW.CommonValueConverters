using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;
using HKW.HKWUtils;

namespace HKW.CommonValueConvertersTest;

public static class CalculatorMultiConverterSuite
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
                new MultiConversionCase<Configuration>("CalculatorMultiConverter", factory =>
                    CalculatorMultiConverter(factory)
                ),
            ];
            yield return
            [
                new MultiConversionCase<Configuration>(
                    "MultiConvertersHandleNullAndEmptyRequiredInputs",
                    factory => MultiConvertersHandleNullAndEmptyRequiredInputs(factory)
                ),
            ];
            yield return
            [
                new MultiConversionCase<Configuration>(
                    "NumericBoundariesReturnDefaultAndPreserveSequentialArithmetic",
                    factory =>
                        NumericBoundariesReturnDefaultAndPreserveSequentialArithmetic(factory)
                ),
            ];
        }
    }

    private static void CalculatorMultiConverter(
        Func<Configuration, MultiConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());

        Assert.AreEqual(10, c.Convert([2, 3, 5], null, "++", null));
        Assert.AreEqual(15, c.Convert([2, "+", 3, "*", 3], null, null, null));
        Assert.AreSame(ConverterResults.UnsetValue, c.Convert([1, 2], null, null, null));
    }

    private static void MultiConvertersHandleNullAndEmptyRequiredInputs(
        Func<Configuration, MultiConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());
        Assert.AreSame(ConverterResults.UnsetValue, c.Convert([], null, null, null));
        Assert.AreSame(ConverterResults.UnsetValue, c.Convert(null!, null, null, null));
    }

    private static void NumericBoundariesReturnDefaultAndPreserveSequentialArithmetic(
        Func<Configuration, MultiConversionSubject<Configuration>> factory
    )
    {
        var unset = ConverterResults.UnsetValue;
        var calculator = factory(new Configuration());
        Assert.AreSame(unset, calculator.Convert([1, "+", "bad"], null, null, null));
        Assert.AreSame(unset, calculator.Convert([1, "", 2], null, null, null));
        Assert.AreSame(unset, calculator.Convert([1, 2], null, "?", null));
        Assert.AreSame(unset, calculator.Convert([1, 2, 3], null, "+", null));
        Assert.AreSame(unset, calculator.Convert([1, 0], null, "/", null));
        Assert.AreEqual(15, calculator.Convert([2, "+", 3, "*", 3], null, null, null));
    }
}

[TestClass]
public sealed class CalculatorMultiConverterTests
{
    public static IEnumerable<object[]> Cases => CalculatorMultiConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(CalculatorMultiConverter), ConverterTestLayer.Behavior)]
    public void Behavior(
        MultiConversionCase<CalculatorMultiConverterSuite.Configuration> scenario
    ) => scenario.Run(Create);

    private static MultiConversionSubject<CalculatorMultiConverterSuite.Configuration> Create(
        CalculatorMultiConverterSuite.Configuration configuration
    )
    {
        var converter = new CalculatorMultiConverter();
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
