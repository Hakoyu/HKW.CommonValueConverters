using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;

namespace HKW.CommonValueConvertersTest;

public static class CollectionCountCompareByConverterSuite
{
    public sealed class Configuration
    {
        public PreferredCulture? PreferredCulture { get; set; }
        public Func<object?>? GetDefaultResult { get; set; }
    }

    public static IEnumerable<object[]> Cases
    {
        get
        {
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "CollectionCountCompareByConverter",
                    factory => CollectionCountCompareByConverter(factory)
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

    private static void CollectionCountCompareByConverter(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());

        Assert.AreEqual(true, c.Convert(new[] { 1, 2, 3 }, null, ">2", null));
        Assert.AreEqual(false, c.Convert(new[] { 1, 2, 3 }, null, ">3", null));
        Assert.AreEqual(true, c.Convert("test", null, "<=4", null));
        Assert.AreEqual(false, c.Convert(new[] { 1, 2 }, null, "<2", null));
        Assert.AreEqual(ConverterResults.UnsetValue, c.Convert(new object(), null, ">0", null));
        Assert.AreEqual(ConverterResults.UnsetValue, c.Convert(new[] { 1 }, null, "invalid", null));
    }

    private static void NumericBoundariesReturnDefaultAndPreserveSequentialArithmetic(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        Assert.AreSame(
            ConverterResults.UnsetValue,
            factory(new Configuration()).Convert(new[] { 1 }, null, ">bad", null)
        );
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
            factory(new Configuration()).Convert(new[] { 1 }, null, parameter, null)
        );
    }
}

[TestClass]
public sealed class CollectionCountCompareByConverterTests
{
    public static IEnumerable<object[]> Cases => CollectionCountCompareByConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(CollectionCountCompareByConverter), ConverterTestLayer.Behavior)]
    public void Behavior(
        SingleConversionCase<CollectionCountCompareByConverterSuite.Configuration> scenario
    ) => scenario.Run(Create);

    private static SingleConversionSubject<CollectionCountCompareByConverterSuite.Configuration> Create(
        CollectionCountCompareByConverterSuite.Configuration configuration
    )
    {
        var converter = new CollectionCountCompareByConverter();
        return new(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
                if (settings.GetDefaultResult is not null)
                    converter.GetDefaultResult = settings.GetDefaultResult;
            },
            converter.Convert,
            converter.ConvertBack
        );
    }
}
