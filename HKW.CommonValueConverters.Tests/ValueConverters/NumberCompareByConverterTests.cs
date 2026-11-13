using System.Globalization;
using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;
using HKW.HKWUtils;

namespace HKW.CommonValueConvertersTest;

public static class NumberCompareByConverterSuite
{
    public sealed class Configuration
    {
        public PreferredCulture? PreferredCulture { get; set; }
        public Func<NumberType>? GetNumberType { get; set; }
        public Func<ComparisonOperatorType>? GetComparisonType { get; set; }
        public Func<object?>? GetDefaultResult { get; set; }
    }

    public static IEnumerable<object[]> Cases
    {
        get
        {
            yield return
            [
                new SingleConversionCase<Configuration>("NumberCompareByConverter", factory =>
                    NumberCompareByConverter(factory)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "NumberCompareByConverter_ComparisonOperators/0",
                    factory => NumberCompareByConverter_ComparisonOperators(factory, "==5", true)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "NumberCompareByConverter_ComparisonOperators/1",
                    factory => NumberCompareByConverter_ComparisonOperators(factory, "!=5", false)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "NumberCompareByConverter_ComparisonOperators/2",
                    factory => NumberCompareByConverter_ComparisonOperators(factory, "<6", true)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "NumberCompareByConverter_ComparisonOperators/3",
                    factory => NumberCompareByConverter_ComparisonOperators(factory, "<=5", true)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "NumberCompareByConverter_ComparisonOperators/4",
                    factory => NumberCompareByConverter_ComparisonOperators(factory, ">4", true)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "NumberCompareByConverter_ComparisonOperators/5",
                    factory => NumberCompareByConverter_ComparisonOperators(factory, ">=5", true)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "NumberCompareByConverter_ComparisonOperators/6",
                    factory => NumberCompareByConverter_ComparisonOperators(factory, " > 5 ", false)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "NumberCompareByConverter_ConfiguredComparisonAndInvalidInputs",
                    factory =>
                        NumberCompareByConverter_ConfiguredComparisonAndInvalidInputs(factory)
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
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "InvalidComparisonParametersReturnDefault/0",
                    factory => InvalidComparisonParametersReturnDefault(factory, "")
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "InvalidComparisonParametersReturnDefault/1",
                    factory => InvalidComparisonParametersReturnDefault(factory, "=")
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "InvalidComparisonParametersReturnDefault/2",
                    factory => InvalidComparisonParametersReturnDefault(factory, "!")
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "InvalidComparisonParametersReturnDefault/3",
                    factory => InvalidComparisonParametersReturnDefault(factory, ">")
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "InvalidComparisonParametersReturnDefault/4",
                    factory => InvalidComparisonParametersReturnDefault(factory, "<>3")
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "InvalidComparisonParametersReturnDefault/5",
                    factory => InvalidComparisonParametersReturnDefault(factory, "===2")
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "InvalidComparisonParametersReturnDefault/6",
                    factory => InvalidComparisonParametersReturnDefault(factory, "bad")
                ),
            ];
        }
    }

    private static void NumberCompareByConverter(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());

        Assert.AreEqual(true, c.Convert(6, null, ">5", null));
        Assert.AreEqual(false, c.Convert(5, null, ">5", null));
        Assert.AreEqual(true, c.Convert(5, null, "5", null));
    }

    private static void NumberCompareByConverter_ComparisonOperators(
        Func<Configuration, SingleConversionSubject<Configuration>> factory,
        string parameter,
        bool expected
    )
    {
        var c = factory(new Configuration());

        Assert.AreEqual(expected, c.Convert(5, null, parameter, CultureInfo.InvariantCulture));
    }

    private static void NumberCompareByConverter_ConfiguredComparisonAndInvalidInputs(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var fallback = new object();
        var c = factory(
            new Configuration
            {
                GetComparisonType = () => ComparisonOperatorType.GreaterThan,
                GetDefaultResult = () => fallback,
            }
        );

        Assert.AreEqual(true, c.Convert(6, null, 5, null));
        Assert.AreEqual(false, c.Convert(5, null, "5", null));
        Assert.AreEqual(true, c.Convert(5, null, "==5", null));
        foreach (
            var parameter in new object?[]
            {
                null,
                "",
                ">bad",
                new object(),
                ConverterResults.UnsetValue,
            }
        )
            Assert.AreSame(fallback, c.Convert(5, null, parameter, null));
        Assert.AreSame(fallback, c.Convert("bad", null, 5, null));
        c.Configuration.GetComparisonType = () => (ComparisonOperatorType)int.MaxValue;
        Assert.AreSame(fallback, c.Convert(5, null, 5, null));
    }

    private static void NumericBoundariesReturnDefaultAndPreserveSequentialArithmetic(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var unset = ConverterResults.UnsetValue;
        var compare = factory(new Configuration());
        Assert.AreEqual(true, compare.Convert(-1, null, "-1", null));
        Assert.AreEqual(true, compare.Convert(-1, null, ">-2", null));
        Assert.AreSame(unset, compare.Convert(1, null, ">bad", null));
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

    private static void InvalidComparisonParametersReturnDefault(
        Func<Configuration, SingleConversionSubject<Configuration>> factory,
        string parameter
    )
    {
        Assert.AreSame(
            ConverterResults.UnsetValue,
            factory(new Configuration()).Convert(1, null, parameter, null)
        );
    }
}

[TestClass]
public sealed class NumberCompareByConverterTests
{
    public static IEnumerable<object[]> Cases => NumberCompareByConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(NumberCompareByConverter), ConverterTestLayer.Behavior)]
    public void Behavior(
        SingleConversionCase<NumberCompareByConverterSuite.Configuration> scenario
    ) => scenario.Run(Create);

    private static SingleConversionSubject<NumberCompareByConverterSuite.Configuration> Create(
        NumberCompareByConverterSuite.Configuration configuration
    )
    {
        var converter = new NumberCompareByConverter();
        return new(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
                if (settings.GetNumberType is not null)
                    converter.GetNumberType = settings.GetNumberType;
                if (settings.GetComparisonType is not null)
                    converter.GetComparisonType = settings.GetComparisonType;
                if (settings.GetDefaultResult is not null)
                    converter.GetDefaultResult = settings.GetDefaultResult;
            },
            converter.Convert,
            converter.ConvertBack
        );
    }
}
