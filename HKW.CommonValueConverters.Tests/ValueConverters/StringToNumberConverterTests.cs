using System.Globalization;
using HKW.CommonValueConverters;
using HKW.CommonValueConvertersTest.Testing;
using HKW.HKWUtils;

namespace HKW.CommonValueConvertersTest;

public static class StringToNumberConverterSuite
{
    public sealed class Configuration
    {
        public PreferredCulture? PreferredCulture { get; set; }
        public Func<NumberStyles>? GetNumberStyles { get; set; }
        public Func<NumberType>? GetNumberType { get; set; }
        public Func<object?>? GetDefaultResult { get; set; }
    }

    public static IEnumerable<object[]> Cases
    {
        get
        {
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "StringToNumberConverter_NumberTypesAndStyles",
                    factory => StringToNumberConverter_NumberTypesAndStyles(factory)
                ),
            ];
            yield return
            [
                new SingleConversionCase<Configuration>(
                    "StringToNumberConverter_InvalidInputsAndReverseConversion",
                    factory => StringToNumberConverter_InvalidInputsAndReverseConversion(factory)
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

    private static void StringToNumberConverter_NumberTypesAndStyles(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var c = factory(new Configuration { PreferredCulture = PreferredCulture.ConverterCulture });
        var culture = CultureInfo.InvariantCulture;

        Assert.AreEqual(42, c.Convert("42", null, null, culture));
        c.Configuration.GetNumberStyles = () => NumberStyles.HexNumber;
        Assert.AreEqual(255, c.Convert("FF", null, null, culture));
        c.Configuration.GetNumberStyles = () => NumberStyles.Number;
        c.Configuration.GetNumberType = () => NumberType.Decimal;
        Assert.AreEqual(12.5m, c.Convert("12,5", null, null, CultureInfo.GetCultureInfo("fr-FR")));
        Assert.AreEqual(
            "12,5",
            c.ConvertBack(12.5m, null, null, CultureInfo.GetCultureInfo("fr-FR"))
        );
    }

    private static void StringToNumberConverter_InvalidInputsAndReverseConversion(
        Func<Configuration, SingleConversionSubject<Configuration>> factory
    )
    {
        var fallback = new object();
        var c = factory(new Configuration { GetDefaultResult = () => fallback });

        foreach (
            var value in new object?[]
            {
                null,
                "",
                "bad",
                "2147483648",
                ConverterResults.UnsetValue,
                ConverterResults.DoNothing,
            }
        )
            Assert.AreSame(fallback, c.Convert(value, null, null, CultureInfo.InvariantCulture));
        foreach (
            var unavailable in new[] { ConverterResults.UnsetValue, ConverterResults.DoNothing }
        )
            Assert.AreSame(fallback, c.ConvertBack(unavailable, null, null, null));
        Assert.IsNull(c.ConvertBack(null, null, null, null));
        Assert.AreEqual("text", c.ConvertBack("text", null, null, null));
        Assert.AreEqual("42", c.ConvertBack(42, null, null, CultureInfo.InvariantCulture));
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
        var saved = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            var culture = CultureInfo.GetCultureInfo("fr-FR");
            var converter = factory(new Configuration { GetNumberType = () => NumberType.Decimal });
            Assert.AreEqual(12.5m, converter.Convert("12,5", null, null, culture));
            Assert.AreEqual(12.5m, converter.Convert(12.5m, null, null, culture));
            Assert.AreEqual("12,5", converter.ConvertBack(12.5m, null, null, culture));
            Assert.IsNull(converter.ConvertBack(null, null, null, culture));
            Assert.AreEqual("other", converter.ConvertBack("other", null, null, culture));
            Assert.AreSame(
                ConverterResults.UnsetValue,
                converter.ConvertBack(ConverterResults.UnsetValue, null, null, culture)
            );
            Assert.AreSame(
                ConverterResults.UnsetValue,
                converter.Convert(ConverterResults.DoNothing, null, null, culture)
            );
        }
        finally
        {
            CultureInfo.CurrentCulture = saved;
        }
    }
}

[TestClass]
[DoNotParallelize]
public sealed class StringToNumberConverterTests
{
    public static IEnumerable<object[]> Cases => StringToNumberConverterSuite.Cases;

    [TestMethod]
    [DynamicData(nameof(Cases))]
    [ConverterTest(typeof(StringToNumberConverter), ConverterTestLayer.Behavior)]
    public void Behavior(
        SingleConversionCase<StringToNumberConverterSuite.Configuration> scenario
    ) => scenario.Run(Create);

    private static SingleConversionSubject<StringToNumberConverterSuite.Configuration> Create(
        StringToNumberConverterSuite.Configuration configuration
    )
    {
        var converter = new StringToNumberConverter();
        return new(
            configuration,
            converter,
            settings =>
            {
                if (settings.PreferredCulture is { } culture)
                    converter.PreferredCulture = culture;
                if (settings.GetNumberStyles is not null)
                    converter.GetNumberStyles = settings.GetNumberStyles;
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
