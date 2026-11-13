using System.Globalization;
using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;
using HKW.HKWUtils;

namespace HKW.CommonValueConvertersTest;

public static class NumberCompareConverterSuite
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
                new SingleConversionCase<Configuration>("NumberCompareConverter", factory =>
                    NumberCompareConverter(factory)
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
                    "NumbersUseCultureInBothDirections",
                    factory => NumbersUseCultureInBothDirections(factory)
                ),
            ];
        }
    }

    private static void NumberCompareConverter(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration());

        Assert.AreEqual(0, c.Convert(5, null, 5, null));
        Assert.AreEqual(1, c.Convert(6, null, 5, null));
        Assert.AreEqual(-1, c.Convert(4, null, 5, null));
    }

    private static void NumericBoundariesReturnDefaultAndPreserveSequentialArithmetic(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        Assert.AreSame(
            ConverterResults.UnsetValue,
            factory(new Configuration()).Convert("bad", null, 1, null)
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

    private static void NumbersUseCultureInBothDirections(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var culture = CultureInfo.GetCultureInfo("fr-FR");
        Assert.AreEqual(
            0,
            factory(new Configuration { GetNumberType = () => NumberType.Decimal })
                .Convert("12,5", null, 12.5m, culture)
        );
    }
}

[TestClass]
[DoNotParallelize]
public sealed class NumberCompareConverterTests
{
    public static IEnumerable<object[]> Cases => NumberCompareConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(NumberCompareConverter), ConverterTestLayer.Behavior)]
    public void Behavior(
        SingleConversionCase<NumberCompareConverterSuite.Configuration> scenario
    ) => scenario.Run(Create);

    private static SingleConversionSubject<NumberCompareConverterSuite.Configuration> Create(
        NumberCompareConverterSuite.Configuration configuration
    )
    {
        var converter = new NumberCompareConverter();
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
